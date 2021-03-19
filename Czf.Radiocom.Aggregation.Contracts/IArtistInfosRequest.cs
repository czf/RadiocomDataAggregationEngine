using System;
using System.Collections.Generic;
using System.Text;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistInfosRequest
    {
        public IEnumerable<int> ArtistIds { get; set; }
    }
}
