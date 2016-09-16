using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryPlanPeriod
    {
        public IList<DateTimePair> GetDateTimePairs(DateTime initDate, int nextDays)
        {
            var result = new List<DateTimePair>();
            if (nextDays <= 0)
            {
                return result;
            }

            //parse time
            var availTimes = ParseTimes(Times);
            if (availTimes == null || availTimes.Count == 0)
            {
                //time is necessary
                return result;
            }

            //parse day of month
            var availDaysOfMonth = ParseDays(DaysOfMonth);
            if (availDaysOfMonth == null)
            {
                //daysOfMonth parse error
                return result;
            }

            //parse day of week
            var availDaysOfWeek = ParseDays(DaysOfWeek);
            if (availDaysOfWeek == null)
            {
                //daysOfMonth parse error
                return result;
            }


            //parse date
            var availDates = ParseDates(Dates);
            if (availDates == null)
            {
                //daysOfMonth parse error
                return result;
            }

            //calc
            var dtStart = initDate.Date;
            var dtTo = dtStart.AddDays(nextDays);
            while (dtStart <= dtTo)
            {
                //check day of month
                if (availDaysOfMonth.Count > 0 && !availDaysOfMonth.Contains(dtStart.Day))
                {
                    dtStart = dtStart.AddDays(1);
                    continue;
                }

                //check day of week
                if (availDaysOfWeek.Count > 0 && !availDaysOfWeek.Contains((int)dtStart.DayOfWeek))
                {
                    dtStart = dtStart.AddDays(1);
                    continue;
                }

                //check dates
                if (availDates.Count > 0 && !availDates.Contains(dtStart))
                {
                    dtStart = dtStart.AddDays(1);
                    continue;
                }


                foreach (var timePair in availTimes)
                {
                    var newPair = new DateTimePair()
                    {
                        //add date time
                        From = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, timePair[0].Hour, timePair[0].Minute, 0),
                        To = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, timePair[1].Hour, timePair[1].Minute, 0),
                        IsTimeDivisible = IsTimeDivisible,
                        OptionDivideMinutes = OptionDivideMinutes
                    };
                    if (newPair.To < newPair.From)
                    {
                        newPair.To = newPair.To.AddDays(1);
                    }

                    result.Add(newPair);
                }

                dtStart = dtStart.AddDays(1);
            }

            return result;
        }

        //parse days setting to list
        //eg.["1","4-6","10"] => [1,4,5,6,10]
        private IList<int> ParseDays(IList<string> settings)
        {
            var days = new List<int>();
            if (settings == null || settings.Count == 0)
            {
                return days;
            }

            foreach (var str in settings)
            {
                try
                {
                    var arr = str.Split('-');
                    if (arr.Length == 1)
                    {
                        //single
                        days.Add(int.Parse(arr[0]));
                    }
                    else if (arr.Length == 2)
                    {
                        //range
                        var start = int.Parse(arr[0].Trim());
                        var to = int.Parse(arr[1].Trim());
                        if (to >= start)
                        {
                            for (var i = start; i <= to; i++)
                            {
                                days.Add(i);
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }

            return days;
        }

        //parse dates setting to list
        //date format is d/M/yyyy
        //eg.["8/8/2016", "9/8/2016-11/8/2016", "13/8/2016"] => 
        //   [8/8/2016, 9/8/2016, 10/8/2016, 11/8/2016, 13/8/2016]
        private IList<DateTime> ParseDates(IList<string> settings)
        {
            var dates = new List<DateTime>();

            if (settings == null || settings.Count == 0)
            {
                return dates;
            }


            foreach (var str in settings)
            {
                try
                {
                    var arr = str.Split('-');
                    if (arr.Length == 1)
                    {
                        //single
                        dates.Add(DateTime.ParseExact(arr[0].Trim(), DateFormatString, CultureInfo.InvariantCulture));
                    }
                    else if (arr.Length == 2)
                    {
                        //range
                        var start = DateTime.ParseExact(arr[0].Trim(), DateFormatString, CultureInfo.InvariantCulture);
                        var to = DateTime.ParseExact(arr[1].Trim(), DateFormatString, CultureInfo.InvariantCulture);
                        if (to < start)
                        {
                            return null;
                        }

                        while (start <= to)
                        {
                            dates.Add(start);
                            start = start.AddDays(1);
                        }
                    }
                }
                catch
                {
                    return null;
                }
            }

            return dates;
        }

        //parse time setting to list
        //date format is HH:mm-HH:mm
        private IList<DateTime[]> ParseTimes(IList<string> settings)
        {
            var times = new List<DateTime[]>();

            if (settings == null || settings.Count == 0)
            {
                return times;
            }

            foreach (var str in settings)
            {
                try
                {
                    var arr = str.Split('-');
                    if (arr.Length == 2)
                    {
                        //range
                        var start = DateTime.ParseExact(arr[0].Trim(), TimeFormatString, CultureInfo.InvariantCulture);
                        var to = DateTime.ParseExact(arr[1].Trim(), TimeFormatString, CultureInfo.InvariantCulture);
                        times.Add(new DateTime[] { start, to });
                    }
                }
                catch
                {
                    return null;
                }
            }

            return times;
        }

    }
}