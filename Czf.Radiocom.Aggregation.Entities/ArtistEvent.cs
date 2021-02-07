using System;
using Czf.Radiocom.Aggregation.Contracts;

namespace Czf.Radiocom.Aggregation.Entities
{
    public class ArtistEvent : ITimeSeriesEvent
    {
        public DateTimeOffset TimeStamp { get; set; }

    }
}
