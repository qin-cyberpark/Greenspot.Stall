using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Greenspot.Stall.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Greenspot.Stall.Models.Settings;
namespace Greenspot.Stall.Tests
{
    [TestClass]
    public class DeliveryFeeTest
    {
        private StallEntities _db;

        [TestInitialize]
        public void InitTest()
        {
            _db = new StallEntities();
        }

        [TestMethod]
        public void ParseDeliveryPlan()
        {
            var stall = Stall.Models.Stall.FindById(103, _db);

            var plan = stall.DeliveryPlan;

            Assert.IsNotNull(plan.DefaultFee);
            Assert.IsNotNull(plan.DefaultPlans);

            var tmpOptsCity = plan.GetTemporaryDeliveryOptions("NZ","Auckland","Auckland City",10, new DateTime(2016, 8, 22));
            Assert.IsTrue(tmpOptsCity.Count == 6);

            var tmpOptsNorth = plan.GetTemporaryDeliveryOptions("NZ", "Auckland", "North Shore", 10, new DateTime(2016, 8, 22));
            Assert.IsTrue(tmpOptsNorth.Count == 0);

            var AvailOptsNorth = plan.GetDefaultDeliveryOptions("NZ", "Auckland", "North Shore", 10, new DateTime(2016, 8, 23));
            Assert.IsTrue(AvailOptsNorth.Count == 4);
        }

        [TestMethod]
        public void CalcPeriod()
        {
            //specify dates
            var p = new DeliveryPlanPeriod()
            {
                Times = new List<string> { "11:30-12:30", "14:30-18:30" },
                Dates = new List<string> { "10/8/2016-12/8/2016", "15/8/2016" }
            };

            var r = p.GetDateTimePairs(new DateTime(2016, 8, 10), 7);
            Verify(r);

            //specify day of month
            p = new DeliveryPlanPeriod()
            {
                DaysOfMonth = new List<string> { "10-12", "15" },
                Times = new List<string> { "11:30-12:30", "14:30-18:30" }
            };

            r = p.GetDateTimePairs(new DateTime(2016, 8, 10), 7);
            Verify(r);

            //specify day of week
            p = new DeliveryPlanPeriod()
            {
                DaysOfWeek = new List<string> { "1", "3-5" },
                Times = new List<string> { "11:30-12:30", "14:30-18:30" }
            };

            r = p.GetDateTimePairs(new DateTime(2016, 8, 10), 7);
            Verify(r);
        }

        [TestMethod]
        public void CalcDeliveryOption()
        {
            var plan = new DeliveryPlan()
            {

                DefaultFee = 20,
                FreeDeliveryOrderAmount = 9999,
                MaxAdvancedOrderDays = 3,
                //OptionIntervalMinutes = 60,


                DefaultPlans = new List<DeliveryPlanItem>
                {
                    #region default plan
                    new DeliveryPlanItem()
                    {
                        Periods = new List<DeliveryPlanPeriod>
                        {
                            new DeliveryPlanPeriod() {
                                Times = new List<string> { "10:30-21:30" },
                            }
                        },
                        Calculators = new List<DeliveryFeeCalculator>
                        {
                            new ByDistanceRangeCalculator()
                            {
                                Ranges = new List<ByDistanceRangeCalculator.DistanceRange>
                                {
                                    new ByDistanceRangeCalculator.DistanceRange ()
                                    {
                                        From = 0,
                                        To = 10,
                                        Fee = 5
                                    },
                                    new ByDistanceRangeCalculator.DistanceRange ()
                                    {
                                        From = 10,
                                        To = 20,
                                        Fee = 10
                                    },
                                    new ByDistanceRangeCalculator.DistanceRange ()
                                    {
                                        From = 20,
                                        To = 30,
                                        Fee = 15
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                },
                TemporaryPlans = new List<DeliveryPlanItem>
                {
                    new DeliveryPlanItem()
                    {
                        #region city
                        Periods = new List<DeliveryPlanPeriod>
                        {
                            new DeliveryPlanPeriod() {
                                Times = new List<string> { "12:30-13:30"},
                                Dates = new List<string> { "11/8/2016-12/8/2016"}
                            }
                        },
                        FilterByArea = false,
                        Areas = new string[] {
                            "NZ-Auckland-Auckland City"
                        },
                        IsExclusive = true,
                        ExclusiveExtension = new DeliveryPlanItem.ExclusiveExtensionMinutes()
                        {
                            Before = 60,
                            After = 60
                        },
                        Calculators = new List<DeliveryFeeCalculator>
                        {
                            new FixedFeeCalculator()
                            {
                               Fee = 5
                            }
                        }
                        #endregion
                    },
                    new DeliveryPlanItem()
                    {
                        #region central
                        Periods = new List<DeliveryPlanPeriod>
                        {
                            new DeliveryPlanPeriod() {
                                Times = new List<string> { "17:30-18:30"},
                                Dates = new List<string> { "12/8/2016-13/8/2016"}
                            }
                        },
                        FilterByArea = true,
                        Areas = new string[] {
                            "NZ-Auckland-Central Auckland",
                        },
                        IsExclusive = true,
                        ExclusiveExtension = new DeliveryPlanItem.ExclusiveExtensionMinutes()
                        {
                            Before = 60,
                            After = 60
                        },
                        Calculators = new List<DeliveryFeeCalculator>
                        {
                            new FixedFeeCalculator()
                            {
                               Fee = 5
                            }
                        }
                        #endregion
                    }
                }
            };

            var tmpOptsCity = plan.GetTemporaryDeliveryOptions("NZ", "Auckland", "Auckland City", 10, new DateTime(2016, 8, 10));
            Assert.IsTrue(tmpOptsCity.Count == 2);

            var tmpOptsCentral = plan.GetTemporaryDeliveryOptions("NZ", "Auckland", "Central Auckland", 10, new DateTime(2016, 8, 10));
            Assert.IsTrue(tmpOptsCentral.Count == 4);

            var availOptsNorth = plan.GetDefaultDeliveryOptions("NZ", "Auckland", "North Shore", 10, new DateTime(2016, 8, 10));
            Assert.IsTrue(availOptsNorth.Count == 8);
        }

        private void Verify(IList<DateTimePair> result)
        {
            //8/10
            Assert.IsTrue(result.Any(x => "10/8/2016 11:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "10/8/2016 12:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            Assert.IsTrue(result.Any(x => "10/8/2016 14:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "10/8/2016 18:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            //8/11
            Assert.IsTrue(result.Any(x => "11/8/2016 11:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "11/8/2016 12:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            Assert.IsTrue(result.Any(x => "11/8/2016 14:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "11/8/2016 18:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            //8/12
            Assert.IsTrue(result.Any(x => "12/8/2016 11:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "12/8/2016 12:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            Assert.IsTrue(result.Any(x => "12/8/2016 14:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "12/8/2016 18:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            //8/13
            Assert.IsFalse(result.Any(x => "13/8/2016 11:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "13/8/2016 12:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            Assert.IsFalse(result.Any(x => "13/8/2016 14:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "13/8/2016 18:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            //8/14
            Assert.IsFalse(result.Any(x => "14/8/2016 11:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "13/8/2016 12:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            Assert.IsFalse(result.Any(x => "14/8/2016 14:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "13/8/2016 18:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            //8/15
            Assert.IsTrue(result.Any(x => "15/8/2016 11:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "15/8/2016 12:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));

            Assert.IsTrue(result.Any(x => "15/8/2016 14:30".Equals(x.From.ToString("d/M/yyyy HH:mm"))
                            && "15/8/2016 18:30".Equals(x.To.ToString("d/M/yyyy HH:mm"))));
        }
    }
}
