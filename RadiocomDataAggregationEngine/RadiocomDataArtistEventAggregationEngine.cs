using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiocomDataAggregationEngine
{
    public class RadiocomDataArtistEventAggregationEngine
    {
        private readonly IArtistEventsRepository _artistEventsRepository;
        private readonly ITimeSeriesEngine _timeSeriesEngine;
        private readonly IArtistTimeSeriesCache _artistTimeSeriesCache;
        public RadiocomDataArtistEventAggregationEngine(
            IArtistEventsRepository artistEventsRepository, 
            ITimeSeriesEngine timeSeriesEngine,
            IArtistTimeSeriesCache artistTimeSeriesCache
            )
        {
            _artistEventsRepository = artistEventsRepository;
            _timeSeriesEngine = timeSeriesEngine;
            _artistTimeSeriesCache = artistTimeSeriesCache;
        }
        public Task ProcessArtist(int artistId)
        {
            Task sevenDaysTask = ProcessArtistForTimeSeriesAsync(TimeSeries.SevenDays, artistId);
            Task threeMonthsTask = ProcessArtistForTimeSeriesAsync(TimeSeries.ThreeMonths, artistId);
            Task oneYearTask = ProcessArtistForTimeSeriesAsync(TimeSeries.OneYear, artistId);

            return Task.WhenAll(sevenDaysTask, threeMonthsTask, oneYearTask);
        }

        private async Task ProcessArtistForTimeSeriesAsync(TimeSeries timeSeries, int artistId)
        {
            IEnumerable<ArtistEvent> events = await _artistEventsRepository.GetEventsForTimeSeriesAsync(timeSeries, artistId);
            IEnumerable<ITimeSeriesValue> timeSeriesEvents = _timeSeriesEngine.ProcessTimeSeries(events, timeSeries);
            await _artistTimeSeriesCache.StoreTimeSeriesValuesAsync(timeSeriesEvents, artistId, timeSeries);

        }
    }

    public interface IArtistTimeSeriesCache
    {
        Task StoreTimeSeriesValuesAsync(IEnumerable<ITimeSeriesValue> timeSeriesEvents, int artistId, TimeSeries timeSeries);
    }

    public interface ITimeSeriesEngine
    {
        IEnumerable<ITimeSeriesValue> ProcessTimeSeries(IEnumerable<ITimeSeriesEvent> events, TimeSeries timeSeries);
    }

    public interface ITimeSeriesValue
    {
        public DateTime Timestamp { get; set; }
        public long Value { get; set; }
    }


    public class ArtistEvent : ITimeSeriesEvent
    {
        public DateTime TimeStamp { get; set; }

    }

    public interface ITimeSeriesEvent
    {
        DateTime TimeStamp { get; set; }
    }
  

    public enum TimeSeries
    {
        None,
        SevenDays,
        ThreeMonths,
        OneYear
    }

    public interface IArtistEventsRepository
    {

        /// <summary>
        /// Return events in ascending order by timestamp
        /// </summary>
        Task<IEnumerable<ArtistEvent>> GetEventsForTimeSeriesAsync(TimeSeries timeSeries, int artistId);
    }
}
