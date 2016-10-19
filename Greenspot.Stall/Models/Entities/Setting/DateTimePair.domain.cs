using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.Settings
{

    public static class DateTimePairIListExtension
    {
        public static IList<T> Subtract<T>(this IList<T> ori, IList<T> exclusion) where T : DateTimePair, new()
        {
            var result = new List<T>();
            if (ori == null || ori.Count == 0)
            {
                return result;
            }
            ori = ori.OrderBy(x => x.From).ToList();

            if (exclusion == null || exclusion.Count == 0)
            {
                return ori;
            }

            //loop ori pairs
            foreach (var oPair in ori)
            {
                //split period by blocked time
                var currPair = oPair.Create<T>(oPair.From, oPair.To);
                T newPair = null;
                T blockedTime = null;
                do
                {
                    blockedTime = exclusion.FirstOrDefault(x =>
                                    (x.From <= currPair.From && x.To > currPair.From)
                                    || (x.From < currPair.To && x.To >= currPair.To)
                                    || (x.From >= currPair.From && x.To <= currPair.To));
                    if (blockedTime != null)
                    {
                        if (blockedTime.From <= currPair.From && blockedTime.To > currPair.From)
                        {
                            //current from in block
                            //trim left
                            currPair.From = blockedTime.To;
                        }
                        else if (blockedTime.From >= currPair.From && blockedTime.To <= currPair.To)
                        {
                            //block in current
                            //split
                            newPair = currPair.Create<T>(currPair.From, blockedTime.From);
                            currPair.From = blockedTime.To;
                        }
                        else if (blockedTime.From <= currPair.To && blockedTime.To >= currPair.To)
                        {
                            //current to in block
                            //trim right
                            currPair.To = blockedTime.From;
                        }
                    }

                    if (newPair != null)
                    {
                        result.Add(currPair.Create<T>(newPair.From, newPair.To));
                        newPair = null;
                    }

                    if (currPair.From >= currPair.To)
                    {
                        break;
                    }
                } while (blockedTime != null);

                if (currPair.From < currPair.To)
                {
                    result.Add(currPair);
                }
            }

            return result;
        }

        public static IList<T> Intersec<T>(this IList<T> ori, IList<T> exclusion)
        {
            return null;
        }
    }

    public partial class DateTimePair
    {
        public virtual T Create<T>(DateTime from, DateTime to) where T : DateTimePair, new()
        {
            return new T()
            {
                From = from,
                To = to,
                DivisionType = DivisionType,
                DivisionMinutes = DivisionMinutes
            };
        }
    }
}