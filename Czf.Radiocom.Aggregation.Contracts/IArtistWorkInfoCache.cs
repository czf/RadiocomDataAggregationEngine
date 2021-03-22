using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Radiocom.Aggregation.Contracts
{
    public interface IArtistWorkInfoCache
    {
        public bool TryGetArtistWorkInfo(int artistWorkId, out IArtistWorkInfo artistWorkInfo);
        public Task StoreArtistWorkInfosAsync(IEnumerable<IArtistWorkInfo> artistWorkInfos);
    }
}
