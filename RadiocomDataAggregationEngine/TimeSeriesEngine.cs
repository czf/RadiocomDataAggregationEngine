using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Czf.Radiocom.Aggregation.Entities;
namespace RadiocomDataAggregationEngine
{
    public class TimeSeriesEngine : ITimeSeriesEngine
    {
        public IEnumerable<ITimeSeriesValue> ProcessTimeSeries(IEnumerable<ITimeSeriesEvent> events, TimeSeries timeSeries)
        {
            return events.GroupBy(GetTimeSeriesGroupingFunc(timeSeries)).OrderBy(x => x.Key).Select(x => new TimeSeriesValue { Timestamp= x.Key, Value = x.Count() });
        }

        private Func<ITimeSeriesEvent, DateTime> GetTimeSeriesGroupingFunc(TimeSeries timeSeries)
        {
            Func<ITimeSeriesEvent, DateTime> result = null; 
            switch (timeSeries)
            {
                
                case TimeSeries.SevenDays:
                    result = x => new DateTime(x.TimeStamp.Year, x.TimeStamp.Month, x.TimeStamp.Day);
                    break;
                case TimeSeries.ThreeMonths:
                    result = x => new DateTime(x.TimeStamp.Year, x.TimeStamp.Month, x.TimeStamp.Day).AddDays(-1 * x.TimeStamp.Day);
                    break;
                case TimeSeries.OneYear:
                    result = x => new DateTime(x.TimeStamp.Year, x.TimeStamp.Month, 1);
                    break;
                case TimeSeries.None:
                default:
                    throw new InvalidEnumArgumentException();
            }
            return result;
        }

        
    }
}
