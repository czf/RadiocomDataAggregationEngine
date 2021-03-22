using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;

namespace Czf.Radiocom.Repository.Contracts
{
    public interface IRadiocomArtistWorkInfoRepository
    {
        public Task<IEnumerable<IArtistWorkInfo>> GetArtistWorkInfosAsync(IEnumerable<int> ids);
    }
}
