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
    public class SqlRadiocomArtistRepository : IRadiocomArtistRepository
    {
        private readonly SqlRadiocomArtistRepositoryOptions _sqlRadiocomArtistRepositoryOptions;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlRadiocomArtistRepository(IOptions<SqlRadiocomArtistRepositoryOptions> sqlRadiocomArtistRepositoryOptions, IDbConnectionFactory dbConnectionFactory)
        {
            _sqlRadiocomArtistRepositoryOptions = sqlRadiocomArtistRepositoryOptions.Value;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<int>> GetArtistIds()
        {
            using IDbConnection conn = await _dbConnectionFactory.GetConnection(_sqlRadiocomArtistRepositoryOptions.ConnectionString);
            return (await conn.QueryAsync<int>("SELECT Artist.Id FROM dbo.Artist;")).AsList();
        }

        public class SqlRadiocomArtistRepositoryOptions
        {
            public const string SqlRadiocomRepository = "SqlRadiocomArtistRepositoryOptions";
            public string ConnectionString { get; set; }
        }
    }
}
