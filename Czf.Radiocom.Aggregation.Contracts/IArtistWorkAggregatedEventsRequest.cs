using System;
using System.Collections.Generic;
using System.Text;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistWorkAggregatedEventsRequest
    {
        public IEnumerable<int> ArtistWorkIds { get; set; }
        public TimeSeries TimeSeries { get; set; }
    }
}
