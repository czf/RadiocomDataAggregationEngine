using System;
using System.ComponentModel;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Event.Repository
{
    public static class TimeSeriesValueUtil
    {
        public static (DateTime start, DateTime end) GetTimeSeriesDateRange(this TimeSeries timeSeries)
        {
            DateTime now = DateTime.Now;
            DateTime endTime = new DateTime(now.Year, now.Month, now.Day, now.Hour,now.Minute,0);
            DateTime startTime;
            switch (timeSeries)
            {   
                case TimeSeries.SevenDays:
                    startTime = now.AddDays(-7);
                    break;
                case TimeSeries.ThreeMonths:
                    startTime = now.AddMonths(-3);
                    break;
                case TimeSeries.OneYear:
                    startTime = now.AddYears(-1);
                    break;
                case TimeSeries.None:
                default:
                    throw new InvalidEnumArgumentException();
            }
            return (new DateTime(startTime.Year, startTime.Month, startTime.Day), endTime);
        }
    }
}
