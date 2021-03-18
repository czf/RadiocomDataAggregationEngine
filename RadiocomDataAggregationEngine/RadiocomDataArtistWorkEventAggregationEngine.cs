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
    public class RadiocomDataArtistWorkEventAggregationEngine
    {
        private readonly IArtistWorkEventsRepository _ArtistWorkEventsRepository;
        private readonly ITimeSeriesEngine _timeSeriesEngine;
        private readonly IArtistWorkTimeSeriesCache _ArtistWorkTimeSeriesCache;
        public RadiocomDataArtistWorkEventAggregationEngine(
            IArtistWorkEventsRepository ArtistWorkEventsRepository, 
            ITimeSeriesEngine timeSeriesEngine,
            IArtistWorkTimeSeriesCache ArtistWorkTimeSeriesCache)
        {
            _ArtistWorkEventsRepository = ArtistWorkEventsRepository;
            _timeSeriesEngine = timeSeriesEngine;
            _ArtistWorkTimeSeriesCache = ArtistWorkTimeSeriesCache;
        }
        public async Task ProcessArtistWork(int ArtistWorkId)
        {
            //Possible optimization if no results at large time series skip subsequent fetch and write 0 
            await ProcessArtistWorkForTimeSeriesAsync(TimeSeries.OneYear, ArtistWorkId); //due to limititaion of 30 requests to sql only run one at once.
            await ProcessArtistWorkForTimeSeriesAsync(TimeSeries.ThreeMonths, ArtistWorkId);
            await ProcessArtistWorkForTimeSeriesAsync(TimeSeries.SevenDays, ArtistWorkId);
        }

        private async Task ProcessArtistWorkForTimeSeriesAsync(TimeSeries timeSeries, int ArtistWorkId)
        {
            IEnumerable<ArtistWorkEvent> events = await _ArtistWorkEventsRepository.GetEventsForTimeSeriesAsync(timeSeries, ArtistWorkId);
            IEnumerable<ITimeSeriesValue> timeSeriesEvents = _timeSeriesEngine.ProcessTimeSeries(events, timeSeries);
            await _ArtistWorkTimeSeriesCache.StoreTimeSeriesValuesAsync(timeSeriesEvents, ArtistWorkId, timeSeries);
        }
    }

    


   

    
  

    

    
}
