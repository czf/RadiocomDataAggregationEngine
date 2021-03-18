using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Radiocom.Repository.Contracts
{
    public interface IRadiocomArtistWorkRepository
    {
        Task<IEnumerable<int>> GetArtistWorkIds();

    }
}
