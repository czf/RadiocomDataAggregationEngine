
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Repository.Contracts;
using Czf.Radiocom.Shared.Contracts;
using Microsoft.Extensions.Options;
using Dapper;
namespace Czf.Radiocom.Repository.Implementations
{
    public class SqlRadiocomArtistWorkRepository : IRadiocomArtistWorkRepository
    {
        private readonly SqlRadiocomArtistWorkRepositoryOptions _sqlRadiocomArtistWorkRepositoryOptions;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlRadiocomArtistWorkRepository(IOptions<SqlRadiocomArtistWorkRepositoryOptions> sqlRadiocomArtistWorkRepositoryOptions, IDbConnectionFactory dbConnectionFactory)
        {
            _sqlRadiocomArtistWorkRepositoryOptions = sqlRadiocomArtistWorkRepositoryOptions.Value;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<int> GetArtistIdForArtistWork(int artistWorkId)
        {
            using IDbConnection conn = await _dbConnectionFactory.GetConnection(_sqlRadiocomArtistWorkRepositoryOptions.ConnectionString);
            return await conn.ExecuteScalarAsync<int>("SELECT ArtistWork.ArtistId FROM dbo.ArtistWork WHERE ArtistWork.Id = @Id;", new { Id = artistWorkId});

        }

        public async Task<IEnumerable<int>> GetArtistWorkIds()
        {
            using IDbConnection conn = await _dbConnectionFactory.GetConnection(_sqlRadiocomArtistWorkRepositoryOptions.ConnectionString);
            return (await conn.QueryAsync<int>("SELECT ArtistWork.Id FROM dbo.ArtistWork;")).AsList();
        }

        public class SqlRadiocomArtistWorkRepositoryOptions
        {
            public const string SqlRadiocomRepository = "SqlRadiocomArtistWorkRepositoryOptions";
            public string ConnectionString { get; set; }
        }
    }
}
