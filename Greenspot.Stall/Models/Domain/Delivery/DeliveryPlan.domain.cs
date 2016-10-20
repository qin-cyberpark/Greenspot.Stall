using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greenspot.Stall.Models.Settings;
namespace Greenspot.Stall.Models
{
    public partial class DeliveryPlan
    {
        //#region Operation
        public IList<DeliveryOption> GetTemporaryDeliveryOptions(string countryCode, string city, string area,
            int? distanceInMeter, DateTime initDate)
        {
            string areaStr = FormatArea(countryCode, city, area);
            var result = new List<DeliveryOption>();
            //if (TemporaryPlans != null)
            //{
            //    foreach (var planItem in TemporaryPlans)
            //    {
            //        //loop plan
            //        var periods = planItem.GetDateTimePairs(initDate, MaxAdvancedOrderDays);
            //        foreach (var period in periods)
            //        {
            //            //loop date time
            //            if (planItem.IsPickUp ||
            //                (planItem.FilterByArea && !planItem.IsApplicableToArea(areaStr)))
            //            {
            //                //filter by area and pickup
            //                continue;
            //            }

            //            //get delivery fee
            //            decimal? fee = null;
            //            if (planItem.Calculators != null && planItem.Calculators.Count > 0)
            //            {
            //                //get fee
            //                fee = planItem.Calculators[0].Calculate(DateTime.Now, null, null, null, distanceInMeter);
            //            }

            //            if (fee == null)
            //            {
            //                if (planItem.IsPickUp)
            //                {
            //                    fee = 0;
            //                }
            //                else
            //                {
            //                    fee = DefaultFee;
            //                }
            //            }

            //            result.Add(new DeliveryOption()
            //            {
            //                From = period.From,
            //                To = period.To,
            //                //IsTimeDivisible = period.IsTimeDivisible,
            //                //OptionDivideMinutes = period.OptionDivideMinutes > 0 ? period.OptionDivideMinutes : OptionDivideMinutes,
            //                PickUpAddress = planItem.PickUpAddress,
            //                IsPickUp = planItem.IsPickUp,
            //                Areas = planItem.Areas,
            //                Fee = fee
            //            });
            //        }
            //    }
            //}

            return result;
        }

        public IList<DeliveryOption> GetDefaultDeliveryOptions(string countryCode, string city, string area, int? distanceInMeter, DateTime initDate)
        {
            string areaStr = FormatArea(countryCode, city, area);

            var result = new List<DeliveryOption>();
            //var tmpPeriod = new List<DateTimePair>();

            //#region get temp plan
            //if (TemporaryPlans != null)
            //{
            //    foreach (var planItem in TemporaryPlans)
            //    {
            //        //loop temp plan
            //        var periods = planItem.GetDateTimePairs(initDate, MaxAdvancedOrderDays, false);
            //        tmpPeriod.AddRange(periods);
            //    }
            //}
            //#endregion

            ////sort temp period
            //tmpPeriod = tmpPeriod.OrderBy(x => x.From).ToList();

            //if (DefaultPlans != null)
            //{
            //    foreach (var planItem in DefaultPlans)
            //    {
            //        //loop date time
            //        if (planItem.IsPickUp ||
            //            (planItem.FilterByArea && !planItem.IsApplicableToArea(areaStr)))
            //        {
            //            //filter by area and pickup
            //            continue;
            //        }

            //        //get delivery fee
            //        decimal? fee = null;
            //        if (planItem.Calculators != null && planItem.Calculators.Count > 0)
            //        {
            //            //get fee
            //            fee = planItem.Calculators[0].Calculate(DateTime.Now, null, null, null, distanceInMeter);
            //        }

            //        if (fee == null)
            //        {
            //            fee = DefaultFee;
            //        }

            //        //loop default plan
            //        var periods = planItem.GetDateTimePairs(initDate, MaxAdvancedOrderDays);
            //        foreach (var period in periods)
            //        {
            //            //split period by blocked time
            //            var currPair = new DateTimePair() { From = period.From, To = period.To };
            //            DateTimePair newPair = null;
            //            DateTimePair blockedTime = null;
            //            do
            //            {
            //                blockedTime = tmpPeriod.FirstOrDefault(x =>
            //                                (x.From <= currPair.From && x.To > currPair.From)
            //                                || (x.From < currPair.To && x.To >= currPair.To)
            //                                || (x.From > currPair.From && x.To < currPair.To));
            //                if (blockedTime != null)
            //                {
            //                    if (currPair.From >= blockedTime.From && currPair.From < blockedTime.To)
            //                    {
            //                        //current from in block
            //                        //trim left
            //                        currPair.From = blockedTime.To;
            //                    }
            //                    else if (currPair.From <= blockedTime.From && currPair.To >= blockedTime.To)
            //                    {
            //                        //block in current
            //                        //split
            //                        newPair = new DateTimePair()
            //                        {
            //                            From = currPair.From,
            //                            To = blockedTime.From,
            //                        };
            //                        currPair.From = blockedTime.To;
            //                    }
            //                    else if (currPair.To >= blockedTime.From && currPair.To < blockedTime.To)
            //                    {
            //                        //current to in block
            //                        //trim right
            //                        currPair.To = blockedTime.From;
            //                    }
            //                    else if (currPair.From == currPair.To)
            //                    {
            //                        break;
            //                    }
            //                }
            //                if (newPair != null)
            //                {
            //                    result.Add(new DeliveryOption()
            //                    {
            //                        From = newPair.From,
            //                        To = newPair.To,
            //                        //IsTimeDivisible = period.IsTimeDivisible,
            //                        //OptionDivideMinutes = period.OptionDivideMinutes > 0 ? period.OptionDivideMinutes : OptionDivideMinutes,
            //                        PickUpAddress = planItem.PickUpAddress,
            //                        IsPickUp = planItem.IsPickUp,
            //                        Areas = planItem.Areas,
            //                        Fee = fee
            //                    });
            //                    newPair = null;
            //                }
            //            } while (blockedTime != null);

            //            if (currPair.From != currPair.To)
            //            {
            //                result.Add(new DeliveryOption()
            //                {
            //                    From = currPair.From,
            //                    To = currPair.To,
            //                    //IsTimeDivisible = period.IsTimeDivisible,
            //                    //OptionDivideMinutes = period.OptionDivideMinutes > 0 ? period.OptionDivideMinutes : OptionDivideMinutes,
            //                    PickUpAddress = planItem.PickUpAddress,
            //                    IsPickUp = planItem.IsPickUp,
            //                    Areas = planItem.Areas,
            //                    Fee = fee
            //                });
            //            }
            //        }
            //    }
            //}

            return result;
        }

        public IList<DeliveryOption> GetPickUpOptions(DateTime initDate)
        {
            var result = new List<DeliveryOption>();
            //var tmpPeriod = new List<DateTimePair>();

            //#region get temp plan
            //if (TemporaryPlans != null)
            //{
            //    foreach (var planItem in TemporaryPlans)
            //    {
            //        //loop temp plan
            //        var periods = planItem.GetDateTimePairs(initDate, MaxAdvancedOrderDays, false);
            //        tmpPeriod.AddRange(periods);
            //    }
            //}
            //#endregion

            ////sort temp period
            //tmpPeriod = tmpPeriod.OrderBy(x => x.From).ToList();

            //#region temp pickup
            ////add temp pickup
            //if (TemporaryPlans != null)
            //{
            //    foreach (var planItem in TemporaryPlans)
            //    {
            //        //loop plan
            //        var periods = planItem.GetDateTimePairs(initDate, MaxAdvancedOrderDays);
            //        foreach (var period in periods)
            //        {
            //            //loop date time
            //            if (!planItem.IsPickUp)
            //            {
            //                //filter not pickup
            //                continue;
            //            }

            //            //get delivery fee
            //            decimal? fee = null;
            //            if (planItem.Calculators != null && planItem.Calculators.Count > 0)
            //            {
            //                //get fee
            //                fee = planItem.Calculators[0].Calculate(DateTime.Now, null, null, null, null);
            //            }

            //            if (fee == null)
            //            {
            //                fee = 0;
            //            }

            //            result.Add(new DeliveryOption()
            //            {
            //                From = period.From,
            //                To = period.To,
            //                //IsTimeDivisible = period.IsTimeDivisible,
            //                //OptionDivideMinutes = period.OptionDivideMinutes > 0 ? period.OptionDivideMinutes : OptionDivideMinutes,
            //                PickUpAddress = planItem.PickUpAddress,
            //                IsPickUp = planItem.IsPickUp,
            //                Areas = planItem.Areas,
            //                Fee = fee
            //            });
            //        }
            //    }
            //}
            //#endregion

            //#region default pickup
            ////add default pickup
            //if (DefaultPlans != null)
            //{
            //    foreach (var planItem in DefaultPlans)
            //    {
            //        //loop date time
            //        if (!planItem.IsPickUp)
            //        {
            //            //filter not pickup
            //            continue;
            //        }

            //        //get delivery fee
            //        decimal? fee = null;
            //        if (planItem.Calculators != null && planItem.Calculators.Count > 0)
            //        {
            //            //get fee
            //            fee = planItem.Calculators[0].Calculate(DateTime.Now, null, null, null, null);
            //        }

            //        if (fee == null)
            //        {
            //            fee = 0;
            //        }

            //        //loop default plan
            //        var periods = planItem.GetDateTimePairs(initDate, MaxAdvancedOrderDays);
            //        foreach (var period in periods)
            //        {
            //            //split period by blocked time
            //            var currPair = new DateTimePair() { From = period.From, To = period.To };
            //            DateTimePair newPair = null;
            //            DateTimePair blockedTime = null;
            //            do
            //            {
            //                blockedTime = tmpPeriod.FirstOrDefault(x =>
            //                                (x.From <= currPair.From && x.To > currPair.From)
            //                                || (x.From <= currPair.To && x.To >= currPair.To)
            //                                || (x.From >= currPair.From && x.To < currPair.To));
            //                if (blockedTime != null)
            //                {
            //                    if (currPair.From >= blockedTime.From && currPair.From < blockedTime.To)
            //                    {
            //                        //current from in block
            //                        //trim left
            //                        currPair.From = blockedTime.To;
            //                    }
            //                    else if (currPair.From <= blockedTime.From && currPair.To >= blockedTime.To)
            //                    {
            //                        //block in current
            //                        //split
            //                        newPair = new DateTimePair()
            //                        {
            //                            From = currPair.From,
            //                            To = blockedTime.From,
            //                        };
            //                        currPair.From = blockedTime.To;
            //                    }
            //                    else if (currPair.To >= blockedTime.From && currPair.To < blockedTime.To)
            //                    {
            //                        //current to in block
            //                        //trim right
            //                        currPair.To = blockedTime.From;
            //                    }
            //                }
            //                if (newPair != null)
            //                {
            //                    result.Add(new DeliveryOption()
            //                    {
            //                        From = newPair.From,
            //                        To = newPair.To,
            //                        //IsTimeDivisible = period.IsTimeDivisible,
            //                        //OptionDivideMinutes = period.OptionDivideMinutes > 0 ? period.OptionDivideMinutes : OptionDivideMinutes,
            //                        PickUpAddress = planItem.PickUpAddress,
            //                        IsPickUp = planItem.IsPickUp,
            //                        Areas = planItem.Areas,
            //                        Fee = fee
            //                    });
            //                    newPair = null;
            //                }
            //            } while (blockedTime != null);

            //            result.Add(new DeliveryOption()
            //            {
            //                From = currPair.From,
            //                To = currPair.To,
            //                //IsTimeDivisible = period.IsTimeDivisible,
            //                //OptionDivideMinutes = period.OptionDivideMinutes > 0 ? period.OptionDivideMinutes : OptionDivideMinutes,
            //                PickUpAddress = planItem.PickUpAddress,
            //                IsPickUp = planItem.IsPickUp,
            //                Areas = planItem.Areas,
            //                Fee = fee
            //            });
            //        }
            //    }
            //}
            //#endregion

            return result;
        }

        private string FormatArea(string countryCode, string city, string area)
        {
            return string.Format("{0}-{1}-{2}", countryCode, city, area);
        }
    }
}
