using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Radiocom.Repository.Contracts
{
    public interface IRadiocomAggregationJobPublisher
    {
        Task PublishArtistAsync(int id);
        Task PublishArtistWorkAsync(int id);
    }

}
