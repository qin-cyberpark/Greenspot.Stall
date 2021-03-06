﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Greenspot.Stall.Models;
using System.Collections.Generic;
using Greenspot.Stall.Models.Settings;

namespace Greenspot.Stall.Tests
{
    [TestClass]
    public class SettingTest
    {
        private StallEntities _db;

        [TestInitialize]
        public void InitTest()
        {
            _db = new StallEntities();
        }

        [TestMethod]
        public void TestDateTimePairIntersect()
        {
            IList<DateTimePair> ori = new List<DateTimePair>()
            {
                new DateTimePair() {From = new DateTime(2016,10,20, 9,0,0), To = new DateTime(2016,10,20, 15,0,0) },
                new DateTimePair() {From = new DateTime(2016,10,20, 17,0,0), To = new DateTime(2016,10,20, 23,0,0) }
            };

            IList<DateTimePair> inc = new List<DateTimePair>()
            {
                new DateTimePair() {From = new DateTime(2016,10,20, 8,0,0), To = new DateTime(2016,10,20, 12,0,0) },
                new DateTimePair() {From = new DateTime(2016,10,20, 14,0,0), To = new DateTime(2016,10,20, 18,0,0) },
                new DateTimePair() {From = new DateTime(2016,10,20, 20,0,0), To = new DateTime(2016,10,20, 22,0,0) }
            };

            var result = ori.Intersect(inc);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 20, 9, 0, 0), result[0].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 12, 0, 0), result[0].To);

            Assert.AreEqual(new DateTime(2016, 10, 20, 14, 0, 0), result[1].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 15, 0, 0), result[1].To);

            Assert.AreEqual(new DateTime(2016, 10, 20, 17, 0, 0), result[2].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 18, 0, 0), result[2].To);

            Assert.AreEqual(new DateTime(2016, 10, 20, 20, 0, 0), result[3].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 22, 0, 0), result[3].To);
        }

        [TestMethod]
        public void TestDateTimePeriod()
        {
            //time only
            var period = new DateTimePeriod()
            {
                Times = { "6:00-8:00", "10:00-18:00", "20:00-22:00", "23:00-2:00" }
            };

            var pairs = period.GetDateTimePairs(new DateTime(2016, 10, 18), 7, TimeDivisionTypes.Undivisible, 0);
            Assert.AreEqual(32, pairs.Count);
            Assert.AreEqual(new DateTime(2016, 10, 18, 6, 0, 0), pairs[0].From);
            Assert.AreEqual(new DateTime(2016, 10, 18, 8, 0, 0), pairs[0].To);
            Assert.AreEqual(new DateTime(2016, 10, 25, 23, 0, 0), pairs[31].From);
            Assert.AreEqual(new DateTime(2016, 10, 26, 2, 0, 0), pairs[31].To);
            Assert.AreEqual(TimeDivisionTypes.Undivisible, pairs[31].DivisionType);
            Assert.AreEqual(1, pairs[31].DivisionMinutes);

            //add day of month
            period.DaysOfMonth = new List<string> { "18", "20", "22-24" };
            pairs = period.GetDateTimePairs(new DateTime(2016, 10, 18), 7, TimeDivisionTypes.DivideToTime, 1);

            Assert.AreEqual(20, pairs.Count);
            Assert.AreEqual(new DateTime(2016, 10, 18, 6, 0, 0), pairs[0].From);
            Assert.AreEqual(new DateTime(2016, 10, 18, 8, 0, 0), pairs[0].To);
            Assert.AreEqual(new DateTime(2016, 10, 20, 6, 0, 0), pairs[4].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 8, 0, 0), pairs[4].To);
            Assert.AreEqual(new DateTime(2016, 10, 24, 23, 0, 0), pairs[19].From);
            Assert.AreEqual(new DateTime(2016, 10, 25, 2, 0, 0), pairs[19].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToTime, pairs[19].DivisionType);
            Assert.AreEqual(1, pairs[19].DivisionMinutes);

            //add day of week
            period.DaysOfWeek = new List<string> { "2", "4", "6" };
            pairs = period.GetDateTimePairs(new DateTime(2016, 10, 18), 7, TimeDivisionTypes.DivideToPeriod, 30);

            Assert.AreEqual(12, pairs.Count);
            Assert.AreEqual(new DateTime(2016, 10, 18, 6, 0, 0), pairs[0].From);
            Assert.AreEqual(new DateTime(2016, 10, 18, 8, 0, 0), pairs[0].To);
            Assert.AreEqual(new DateTime(2016, 10, 20, 6, 0, 0), pairs[4].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 8, 0, 0), pairs[4].To);
            Assert.AreEqual(new DateTime(2016, 10, 22, 23, 0, 0), pairs[11].From);
            Assert.AreEqual(new DateTime(2016, 10, 23, 2, 0, 0), pairs[11].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToPeriod, pairs[11].DivisionType);
            Assert.AreEqual(30, pairs[11].DivisionMinutes);

            //add date
            period.Dates = new List<string> { "2016/10/20-2016/10/21" };
            pairs = period.GetDateTimePairs(new DateTime(2016, 10, 18), 7, TimeDivisionTypes.Undivisible, 0);

            Assert.AreEqual(4, pairs.Count);
            Assert.AreEqual(new DateTime(2016, 10, 20, 6, 0, 0), pairs[0].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 8, 0, 0), pairs[0].To);
            Assert.AreEqual(new DateTime(2016, 10, 20, 23, 0, 0), pairs[3].From);
            Assert.AreEqual(new DateTime(2016, 10, 21, 2, 0, 0), pairs[3].To);
        }

        [TestMethod]
        public void TestExcluceDateTimePairs()
        {
            var ori = new DateTimePeriod()
            {
                Times = { "10:00-2:00" }
            };


            //trim left
            var exclude1 = new DateTimePeriod()
            {
                Times = { "10:00-17:00" },
                Dates = { "2016/10/19" }
            };

            var include = ori.GetDateTimePairs(new DateTime(2016, 10, 19), 3, TimeDivisionTypes.Undivisible, 0);
            var exclude = exclude1.GetDateTimePairs(new DateTime(2016, 10, 19), 3);
            var result = include.Subtract(exclude);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 19, 17, 0, 0), result[0].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 2, 0, 0), result[0].To);
            Assert.AreEqual(TimeDivisionTypes.Undivisible, result[0].DivisionType);
            Assert.AreEqual(0, result[0].DivisionMinutes);

            //trim right
            var exclude2 = new DateTimePeriod()
            {
                Times = { "18:00-2:00" },
                Dates = { "2016/10/20" }
            };


            include = ori.GetDateTimePairs(new DateTime(2016, 10, 19), 3, TimeDivisionTypes.DivideToTime, 1);
            exclude = exclude2.GetDateTimePairs(new DateTime(2016, 10, 19), 3);
            result = include.Subtract(exclude);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 20, 10, 0, 0), result[1].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 18, 0, 0), result[1].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToTime, result[1].DivisionType);
            Assert.AreEqual(1, result[1].DivisionMinutes);

            //block all
            var exclude3 = new DateTimePeriod()
            {
                Times = { "10:00-2:00" },
                Dates = { "2016/10/20" }
            };

            include = ori.GetDateTimePairs(new DateTime(2016, 10, 19), 3, TimeDivisionTypes.DivideToTime, 1);
            exclude = exclude3.GetDateTimePairs(new DateTime(2016, 10, 19), 3);
            result = include.Subtract(exclude);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 21, 10, 0, 0), result[1].From);
            Assert.AreEqual(new DateTime(2016, 10, 22, 2, 0, 0), result[1].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToTime, result[1].DivisionType);
            Assert.AreEqual(1, result[1].DivisionMinutes);

            //split
            var exclude4 = new DateTimePeriod()
            {
                Times = { "13:30-17:30" },
                Dates = { "2016/10/20" }
            };
            include = ori.GetDateTimePairs(new DateTime(2016, 10, 19), 3, TimeDivisionTypes.DivideToPeriod, 60);
            exclude = exclude4.GetDateTimePairs(new DateTime(2016, 10, 19), 3);
            result = include.Subtract(exclude);
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 20, 10, 0, 0), result[1].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 13, 30, 0), result[1].To);
            Assert.AreEqual(new DateTime(2016, 10, 20, 17, 30, 0), result[2].From);
            Assert.AreEqual(new DateTime(2016, 10, 21, 2, 0, 0), result[2].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToPeriod, result[2].DivisionType);
            Assert.AreEqual(60, result[2].DivisionMinutes);
        }

        [TestMethod]
        public void TestDateTimeTerm()
        {
            var term = new DateTimeTerm()
            {
                Inclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "10:00-2:00" } } },
                Exclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "10:00-17:00" }, Dates = { "2016/10/19" } } },
                DivisionType = TimeDivisionTypes.DivideToPeriod,
                DivisionMinutes = 60
            };

            //trim left
            var result = term.GetDateTimePairs(new DateTime(2016, 10, 19), 3);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 19, 17, 0, 0), result[0].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 2, 0, 0), result[0].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToPeriod, result[0].DivisionType);
            Assert.AreEqual(60, result[0].DivisionMinutes);

            //trim right
            term.Exclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "18:00-2:00" }, Dates = { "2016/10/20" } } };
            result = term.GetDateTimePairs(new DateTime(2016, 10, 19), 3);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 20, 10, 0, 0), result[1].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 18, 0, 0), result[1].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToPeriod, result[1].DivisionType);
            Assert.AreEqual(60, result[1].DivisionMinutes);

            //block all
            term.Exclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "10:00-2:00" }, Dates = { "2016/10/20" } } };
            result = term.GetDateTimePairs(new DateTime(2016, 10, 19), 3);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 21, 10, 0, 0), result[1].From);
            Assert.AreEqual(new DateTime(2016, 10, 22, 2, 0, 0), result[1].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToPeriod, result[1].DivisionType);
            Assert.AreEqual(60, result[1].DivisionMinutes);

            //split
            term.Exclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "13:30-17:30" }, Dates = { "2016/10/20" } } };
            result = term.GetDateTimePairs(new DateTime(2016, 10, 19), 3);
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(new DateTime(2016, 10, 20, 10, 0, 0), result[1].From);
            Assert.AreEqual(new DateTime(2016, 10, 20, 13, 30, 0), result[1].To);
            Assert.AreEqual(new DateTime(2016, 10, 20, 17, 30, 0), result[2].From);
            Assert.AreEqual(new DateTime(2016, 10, 21, 2, 0, 0), result[2].To);
            Assert.AreEqual(TimeDivisionTypes.DivideToPeriod, result[2].DivisionType);
            Assert.AreEqual(60, result[2].DivisionMinutes);
        }

        [TestMethod]
        public void TestPickup()
        {
            //oping hours
            var openingHours = new DateTimeTerm()
            {
                Inclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "10:00-2:00" } } },
                Exclusive = new List<DateTimePeriod> {
                    new DateTimePeriod { Times = { "10:00-2:00" }, Dates = { "2016/10/20" }},
                     new DateTimePeriod { Times = { "13:00-18:00" }, Dates = { "2016/10/21" }}}
            };

            //definition
            var definition = new PickupDefinition()
            {
                Available = true,
                Rules = new List<PickupRule>()
            };


            definition.Rules.Add(new PickupRule()
            {
                Addresses = { "Address A", "Address D" },
                DateTimes = new DateTimeTerm()
                {
                    Inclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "10:00-17:00" }, DaysOfWeek = {"3","4"} } }
                }
            });
            definition.Rules.Add(new PickupRule()
            {
                Addresses = { "Address B", "Address E" },
                DateTimes = new DateTimeTerm()
                {
                    Inclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "12:00-18:00" }, DaysOfWeek = { "2", "4" } } }
                }
            });
            definition.Rules.Add(new PickupRule()
            {
                SameAsOpeningHours = true,
                Addresses = { "Address C" }
            });


            //result
            var opts = definition.GetOptions(new DateTime(2016, 10, 19), 6, openingHours);
            Assert.AreEqual(15, opts.Count);
            Assert.AreEqual("Address A", opts[0].Address);
            Assert.AreEqual("Address B", opts[2].Address);
            Assert.AreEqual("Address C", opts[4].Address);
            Assert.AreEqual("Address D", opts[11].Address);
            Assert.AreEqual("Address E", opts[13].Address);
        }

        [TestMethod]
        public void TestDelivery()
        {
            //definition
            var definition = new DeliveryDefinition()
            {
                MinOrderAmount = 25,
                FreeDeliveryOrderAmount = 50,
                DefaultCalculator = new DeliveryFeeCalculator()
                {
                    Rules = new List<IDeliveryFeeRule>()
                        {
                            new BasicDeliveryFeeRule()
                            {
                                Fee = 10
                            }
                        }
                },
                Rules = new List<DeliveryRule>(),
            };

            definition.Rules.Add(new DeliveryRule()
            {
                Areas = { "Area A", "Area B" },
                DateTimes = new DateTimeTerm()
                {
                    DivisionType = TimeDivisionTypes.DivideToPeriod,
                    DivisionMinutes = 45,
                    Inclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "11:00-13:00" }, DaysOfWeek = { "5" } } ,
                                                            new DateTimePeriod { Times = { "14:00-18:00" }, DaysOfMonth = { "20" } }},
                },
                Calculator = new DeliveryFeeCalculator()
                {
                    Rules = new List<IDeliveryFeeRule>()
                    {
                        new BasicDeliveryFeeRule()
                        {
                            Fee = 10
                        }
                    }
                }
            });

            definition.Rules.Add(new DeliveryRule()
            {
                Areas = { "Area C", "Area D" },
                DateTimes = new DateTimeTerm()
                {
                    DivisionType = TimeDivisionTypes.DivideToTime,
                    DivisionMinutes = 1,
                    Inclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "11:00-21:00" } } },
                    Exclusive = new List<DateTimePeriod> { new DateTimePeriod { Times = { "11:00-21:00" }, DaysOfWeek = { "4-5" } } },
                },
                Calculator = new DeliveryFeeCalculator
                {
                    Rules = new List<IDeliveryFeeRule>()
                    {
                        new BasicDeliveryFeeRule()
                        {
                            Fee = 6
                        }
                    }
                }
            });

            //result
            var opts = definition.GetOptions(new DateTime(2016, 10, 19), 3, null);
            Assert.AreEqual(4, opts.Count);
            Assert.AreEqual("Area C", opts[0].Areas[0]);
            Assert.AreEqual(new DateTime(2016, 10, 19, 21, 0, 0), opts[0].To);

            Assert.AreEqual("Area B", opts[1].Areas[1]);
            Assert.AreEqual(new DateTime(2016, 10, 20, 18, 0, 0), opts[1].To);
        }

        [TestMethod]
        public void TestDeliveryFeeCalculator()
        {
            var calc = new DeliveryFeeCalculator();

            //combine
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                Areas = { "nz:auckland:auckland:auckland-city" },
                DistanceTo = 20,
                OrderAmountFrom = 50,
                Fee = 50
            });
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                Areas = { "nz:auckland:auckland:north-shore" },
                DistanceTo = 30,
                OrderAmountFrom = 70,
                Fee = 51
            });
            //with order amount
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                OrderAmountFrom = 30,
                OrderAmountTo = 60,
                Fee = 42
            });
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                OrderAmountTo = 60,
                Fee = 41
            });
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                OrderAmountFrom = 30,
                Fee = 40
            });
            //with distance
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                DistanceFrom = 2000,
                DistanceTo = 3000,
                Fee = 32
            });
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                DistanceTo = 3000,
                Fee = 31
            });
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                DistanceFrom = 2000,
                Fee = 30
            });
            //area
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                Areas = { "nz:auckland:auckland:auckland-city" },
                Fee = 21
            });
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                Areas = { "nz:auckland:auckland" },
                Fee = 20
            });
            //fixed
            calc.Rules.Add(new BasicDeliveryFeeRule()
            {
                Fee = 10
            });


            Assert.AreEqual(10, calc.Calculate(DateTime.Now, null, null, null));
            Assert.AreEqual(20, calc.Calculate(DateTime.Now, "nz:auckland:auckland:central-auckland", null, null));
            Assert.AreEqual(21, calc.Calculate(DateTime.Now, "nz:auckland:auckland:auckland-city", null, null));
            Assert.AreEqual(30, calc.Calculate(DateTime.Now, null, 3001, null));
            Assert.AreEqual(31, calc.Calculate(DateTime.Now, null, 1999, null));
            Assert.AreEqual(32, calc.Calculate(DateTime.Now, null, 2000, null));
            Assert.AreEqual(32, calc.Calculate(DateTime.Now, null, 3000, null));
            Assert.AreEqual(40, calc.Calculate(DateTime.Now, null, null, 61));
            Assert.AreEqual(41, calc.Calculate(DateTime.Now, null, null, 29));
            Assert.AreEqual(42, calc.Calculate(DateTime.Now, null, null, 30));
            Assert.AreEqual(42, calc.Calculate(DateTime.Now, null, null, 60));
            Assert.AreEqual(50, calc.Calculate(DateTime.Now, "nz:auckland:auckland:auckland-city", 19, 51));
            Assert.AreEqual(51, calc.Calculate(DateTime.Now, "nz:auckland:auckland:north-shore", 29, 71));
        }

        [TestMethod]
        public void TestParseSetting()
        {
            var stall = Stall.Models.Stall.FindById(109, _db);

            var setting = stall.Setting;

            Assert.IsNotNull(setting.MaxAdvancedOrderDays);
            Assert.IsNotNull(setting.OpeningHours);
            Assert.IsNotNull(setting.Pickup);
            Assert.IsNotNull(setting.Delivery);
        }
    }
}
