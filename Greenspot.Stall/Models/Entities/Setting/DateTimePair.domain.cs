using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.Settings
{

    public static class DateTimePairIListExtension
    {
        public static IList<TOri> Subtract<TOri, TExc>(this IList<TOri> ori, IList<TExc> exclusion)
            where TOri : DateTimePair, new()
            where TExc : DateTimePair, new()
        {
            var result = new List<TOri>();
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
                var currPair = oPair.Create<TOri>(oPair.From, oPair.To);
                TOri newPair = null;
                TExc blockedTime = null;
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
                            newPair = currPair.Create<TOri>(currPair.From, blockedTime.From);
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
                        result.Add(currPair.Create<TOri>(newPair.From, newPair.To));
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

        public static IList<TOri> Intersect<TOri, TInc>(this IList<TOri> ori, IList<TInc> inclusion)
            where TOri : DateTimePair, new()
            where TInc : DateTimePair, new()
        {
            var result = new List<TOri>();
            if (ori == null || ori.Count == 0)
            {
                return result;
            }
            ori = ori.OrderBy(x => x.From).ToList();

            if (inclusion == null || inclusion.Count == 0)
            {
                return result;
            }

            //loop ori pairs
            foreach (var oPair in ori)
            {
                //union by inc time
                var currPair = oPair.Create<TOri>(oPair.From, oPair.To);
                TOri newPair = null;
                TInc unionTime = null;

                do
                {
                    unionTime = inclusion.FirstOrDefault(x =>
                                        (x.From <= currPair.From && x.To > currPair.From)
                                        || (x.From < currPair.To && x.To >= currPair.To)
                                        || (x.From >= currPair.From && x.To <= currPair.To));
                    if (unionTime != null)
                    {
                        if (unionTime.From <= currPair.From && unionTime.To > currPair.From)
                        {
                            //current from in union
                            //trim left
                            newPair = currPair.Create<TOri>(currPair.From, unionTime.To);
                            currPair.From = unionTime.To;
                        }
                        else if (unionTime.From >= currPair.From && unionTime.To <= currPair.To)
                        {
                            //union in current
                            newPair = currPair.Create<TOri>(unionTime.From, unionTime.To);
                            currPair.From = unionTime.To;
                        }
                        else if (unionTime.From <= currPair.To && unionTime.To >= currPair.To)
                        {
                            //current to in union
                            //trim right
                            newPair = currPair.Create<TOri>(unionTime.From, currPair.To);
                            currPair.From = unionTime.To;
                        }

                        result.Add(newPair);
                    }

                    if (currPair.From >= currPair.To)
                    {
                        break;
                    }
                } while (unionTime != null);
            }

            return result;
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