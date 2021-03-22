using System;
using System.Collections.Generic;
using System.Text;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistWorkInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
    }
}
