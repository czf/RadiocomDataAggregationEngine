using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistInfoCache
    {
        public bool TryGetArtistInfo(int artistId, out IArtistInfo artistInfo);
        public Task StoreArtistInfosAsync(IEnumerable<IArtistInfo> artistInfos);
    }
}
