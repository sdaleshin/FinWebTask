using FinWebTask.DTOs;
using FinWebTask.Managers;
using FinWebTask.Models;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FinWebTask.Controllers
{
    public class HomeController : Controller
    {
        private FinContext context;
        private TickerManager tickerManager;
        private Random random;
        public HomeController() : base() {
            context = new FinContext();
            tickerManager = new TickerManager(context);
            random = new Random();
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.TickerData = tickerManager.GetTickers().OrderByDescending(s => s.Date).Take(50); 

            return View();
        }
        [HttpPost]
        public ActionResult GetPredictionData(ConditionModel model) {
            var trainStartDate = DateTime.Today.AddYears(-1);
            var trainEndDate = DateTime.Today;

            var testStartDate = DateTime.Today;
            var testEndDate = model.EndDate;

            var tickers = tickerManager
                .GetTickers()
                .Where(s => s.Date >= trainStartDate && s.Date <= trainEndDate && s.Ticker == model.Ticker)
                .ToList();

            var max = tickers.Max(s => s.AVG);
            var min = tickers.Min(s => s.AVG);
            var k = 1.0 / (max - min);
            var i = 0;
            tickers.ForEach(s => { 
                s.Norm = (s.AVG - min) * k;
                s.Index = i;
                i++;
            });

            var numberOfOutputNeurons = 1;
            var numberOfInputNeurons = 2;
            var numberOfHiddenNeurons = 10;

            var epochs = 300;
            var index1 = tickers.Select(s => s.Index).ToList();
            var index2 = index1.Select(s => s + numberOfOutputNeurons).ToList();
            var index0 = index2.Select(s => s + numberOfOutputNeurons).ToList();

            var p = index1.Select((s,ind) => new NormPriceModel() {
                Index0 = tickers.Count > index0[ind] ? tickers[index0[ind]].Norm : 0,
                Index1 = tickers.Count > index1[ind] ? tickers[index1[ind]].Norm : 0,
                Index2 = tickers.Count > index2[ind] ? tickers[index2[ind]].Norm : 0,
            });

            var learningRate = 0.1;
            var momentum = 0.5;
            var stopCrit = 0.5;
            var pepErr = 10;
            var dflag = 0;

            var numPats = Math.Floor(p.Count() / (double)numberOfOutputNeurons);
            DenseMatrix w1t = (DenseMatrix)DenseMatrix.Create(numberOfHiddenNeurons, numberOfInputNeurons + 1, GetRandom);
            DenseMatrix w2t = (DenseMatrix)DenseMatrix.Create(numberOfOutputNeurons, numberOfHiddenNeurons + 1, GetRandom);

            DenseMatrix dw1l = DenseMatrix.Create(numberOfHiddenNeurons, numberOfInputNeurons + 1, 0);
            DenseMatrix dw2l = DenseMatrix.Create(numberOfOutputNeurons, numberOfHiddenNeurons + 1, 0);

            DenseMatrix learningRate1 = (DenseMatrix)DenseMatrix.Create(numberOfHiddenNeurons, numberOfInputNeurons + 1, 0.1).Transpose();
            DenseMatrix learningRate2 = (DenseMatrix)DenseMatrix.Create(numberOfOutputNeurons, numberOfHiddenNeurons + 1, 0.1).Transpose();

            var epoch = 1;
            var errEpoch = new Dictionary<int, double>();
            var epErr = new Dictionary<int, double>();
            while (epoch < epochs && dflag == 0)
            {
                for (var j = 1; j <= numPats; j++)
                {
                    var int_ = GetArrayOfIndexes((j - 1) * numberOfOutputNeurons + 1, j * numberOfOutputNeurons);
                    var target = DenseMatrix.OfColumnArrays(p.Where((s, ind) => int_.Contains(ind)).Select(s => s.Index0).ToArray());
                    var i1 = p.Where((s, ind) => int_.Contains(ind)).Select(s => s.Index1);
                    var i2 = p.Where((s, ind) => int_.Contains(ind)).Select(s => s.Index2);
                    var inputArray = new List<double>();
                    inputArray.AddRange(i1);
                    inputArray.AddRange(i2);
                    inputArray.Add(1);
                    var input = DenseMatrix.OfColumnArrays(inputArray.ToArray());
                    var sumHidden = w1t * input;
                    var outHidden = bpm_phi(sumHidden);
                    var sumOutput = w2t * (DenseMatrix)(outHidden.Transpose().InsertColumn(outHidden.RowCount, DenseVector.Create(outHidden.ColumnCount, 1))).Transpose();
                    var outOutput = bpm_phi(sumOutput);
                    var outputError = target - outOutput;
                    if (errEpoch.ContainsKey(j))
                    {
                        errEpoch[j] = outputError.Enumerate().Select(s => s *s).Sum();
                    }
                    else
                    {
                        errEpoch.Add(j, outputError.Enumerate().Select(s => s * s).Sum());
                    }

                    var dc = bpm_phi_d(sumOutput) * outputError;
                    var dc1 = DenseMatrix.Create(dc.ColumnCount, numberOfHiddenNeurons + 1,(r,c) => dc[0,r]);
                    var outHidden1 = DenseMatrix.Create(outHidden.ColumnCount, numberOfHiddenNeurons,(r,c) => outHidden[0,r]);
                    var outHidden2 = outHidden1.Transpose().InsertRow(outHidden1.ColumnCount, DenseVector.Create(outHidden1.RowCount, 1));
                    var dw2 = learningRate2 * dc1 * outHidden2;

                    var dc2 = DenseMatrix.Create(dc.ColumnCount, numberOfHiddenNeurons, (r, c) => dc[0, r]);
                    var w2t1 = GetSubMatrix(w2t, numberOfOutputNeurons, numberOfHiddenNeurons).Transpose();
                    var sum1 = (w2t1 * dc2).RowSums();
                    var db = bpm_phi_d(sumHidden) * DenseMatrix.OfColumnVectors(sum1).Transpose();
                    var in1 = DenseMatrix.Create(input.RowCount, numberOfHiddenNeurons, (r, c) => input[r, 0]);
                    var db1 = DenseMatrix.Create(db.RowCount, numberOfInputNeurons + 1, (r, c) => db[0, r]);
                    var dw1 = learningRate1 * db1 * in1;

                    w1t = (DenseMatrix)(w1t + dw1.Transpose() + momentum * dw1l);
                    w2t = (DenseMatrix)(w2t + dw2.Transpose() + momentum * dw2l);
                    dw1l = (DenseMatrix)dw1;
                    dw2l = (DenseMatrix)dw2;
                }

                if (epErr.ContainsKey(epoch))
                {
                    epErr[epoch] = errEpoch.Sum(s => s.Value);
                }
                else
                {
                    epErr.Add(epoch, errEpoch.Sum(s => s.Value));
                }
                
            }

            return Json(new { tickers = tickers });
        }

        private DenseMatrix GetSubMatrix(DenseMatrix matrix, int rows, int columns)
        {
            return DenseMatrix.Create(rows,columns, (r,c) => matrix[r ,c]);
        }

        private DenseMatrix bpm_phi(DenseMatrix matrix)
        {
            return (DenseMatrix)matrix.Clone().Map(s => 1 / (1 + Math.Pow(Math.E, -s)));
        }

        private DenseMatrix bpm_phi_d(DenseMatrix matrix)
        {
            return (DenseMatrix)matrix.Clone().Map(s => Math.Pow(Math.E, -s) / ((1 + Math.Pow(Math.E, -s))*(1 + Math.Pow(Math.E, -s))));
        }

        private List<int> GetArrayOfIndexes(int start, int end)
        {
            var retVal = new List<int>();
            for (var i = start; i <= end; i++)
            {
                retVal.Add(i);
            }
            return retVal;
        }

        private double GetRandom(int a, int b)
        {
            //reuse this if you are generating many
            double u1 = random.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         0 + 1 * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
