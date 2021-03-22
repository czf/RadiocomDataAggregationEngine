using System;
using System.Collections.Generic;
using System.Text;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistWorkInfoRequest
    {
        public IEnumerable<int> ArtistWorkIds { get; set; }
    }
}
