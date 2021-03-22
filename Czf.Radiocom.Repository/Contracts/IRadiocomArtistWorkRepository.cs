using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Radiocom.Repository.Contracts
{
    public interface IRadiocomArtistWorkRepository
    {
        Task<IEnumerable<int>> GetArtistWorkIds();
        /// <summary>
        /// Given an artistWorkId, the function will return the owning ArtistId
        /// </summary>
        Task<int> GetArtistIdForArtistWork(int artistWorkId);
    }
}
