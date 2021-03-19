using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts;
using Czf.Radiocom.Repository.Contracts;
using Czf.Radiocom.Shared.Contracts;
using Dapper;
using Microsoft.Extensions.Options;

namespace Czf.Radiocom.Repository.Implementations
{
    public class SqlRadiocomArtistInfoRepository : IRadiocomArtistInfoRepository
    {
        private readonly SqlRadiocomArtistInfoRepositoryOptions _sqlRadiocomArtistInfoRepositoryOptions;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlRadiocomArtistInfoRepository(IOptions<SqlRadiocomArtistInfoRepositoryOptions> sqlRadiocomArtistInfoRepositoryOptions, IDbConnectionFactory dbConnectionFactory)
        {
            _sqlRadiocomArtistInfoRepositoryOptions = sqlRadiocomArtistInfoRepositoryOptions.Value;
            _dbConnectionFactory = dbConnectionFactory;
        }
     
        public async Task<IEnumerable<IArtistInfo>> GetArtistInfosAsync(IEnumerable<int> ids)
        {
            List<IArtistInfo> result = new List<IArtistInfo>();
            using IDbConnection conn = await _dbConnectionFactory.GetConnection(_sqlRadiocomArtistInfoRepositoryOptions.ConnectionString);
            foreach (int id in ids)
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("ArtistId", id, DbType.Int32);
                ArtistInfo artistInfo =
                    conn.QuerySingleOrDefault<ArtistInfo>("SELECT * FROM dbo.Artist WHERE Id = @ArtistId;", dynamicParameters);
                if(artistInfo != null)
                {
                    result.Add(artistInfo);
                }
            }
            return result;

        }
        public class SqlRadiocomArtistInfoRepositoryOptions
        {
            public const string SqlRadiocomRepository = "SqlRadiocomArtistInfoRepositoryOptions";
            public string ConnectionString { get; set; }
        }
        public class ArtistInfo : IArtistInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
