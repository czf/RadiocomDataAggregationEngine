using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;

namespace Czf.Radiocom.Repository.Contracts
{
    public interface IRadiocomArtistInfoRepository
    {
        public Task<IEnumerable<IArtistInfo>> GetArtistInfosAsync(IEnumerable<int> ids);
    }
}
