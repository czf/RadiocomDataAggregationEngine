using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Czf.Radiocom.Event.Repository.Contracts
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> GetConnection(string connectionString);

    }
}
