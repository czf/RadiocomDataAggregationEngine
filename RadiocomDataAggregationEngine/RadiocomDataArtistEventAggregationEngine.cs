using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Czf.Radiocom.Aggregation.Entities;
using Czf.Radiocom.Event.Repository.Contracts;
using Czf.Radiocom.Repository.Contracts;

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
            IArtistTimeSeriesCache artistTimeSeriesCache)
        {
            _artistEventsRepository = artistEventsRepository;
            _timeSeriesEngine = timeSeriesEngine;
            _artistTimeSeriesCache = artistTimeSeriesCache;
        }
        
        public async Task ProcessArtist(int artistId)
        {
            //Possible optimization if no results at large time series skip subsequent fetch and write 0 
            await ProcessArtistForTimeSeriesAsync(TimeSeries.OneYear, artistId);//due to limited 30 requests to sql only run one at once.
            await ProcessArtistForTimeSeriesAsync(TimeSeries.ThreeMonths, artistId);
            await ProcessArtistForTimeSeriesAsync(TimeSeries.SevenDays, artistId);             
        }

        private async Task ProcessArtistForTimeSeriesAsync(TimeSeries timeSeries, int artistId)
        {
            IEnumerable<ArtistEvent> events = await _artistEventsRepository.GetEventsForTimeSeriesAsync(timeSeries, artistId);
            IEnumerable<ITimeSeriesValue> timeSeriesEvents = _timeSeriesEngine.ProcessTimeSeries(events, timeSeries);
            await _artistTimeSeriesCache.StoreTimeSeriesValuesAsync(timeSeriesEvents, artistId, timeSeries);
        }
    }

    


   

    
  

    

    
}
