using System;
using System.Collections.Generic;
using System.Text;
using Czf.Radiocom.Aggregation.Contracts.Values;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtist_TopArtistWorkAggregatedEventsRequest
    {
        public int ArtistId { get; set; }
        public int TopN { get; set; }
        public TimeSeries TimeSeries { get; set; }
    }
}
