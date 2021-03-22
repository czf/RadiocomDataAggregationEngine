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
    public class SqlRadiocomArtistWorkInfoRepository : IRadiocomArtistWorkInfoRepository
    {
        private readonly SqlRadiocomArtistWorkInfoRepositoryOptions _sqlRadiocomArtistWorkInfoRepositoryOptions;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlRadiocomArtistWorkInfoRepository(IOptions<SqlRadiocomArtistWorkInfoRepositoryOptions> sqlRadiocomArtistWorkInfoRepositoryOptions, IDbConnectionFactory dbConnectionFactory)
        {
            _sqlRadiocomArtistWorkInfoRepositoryOptions = sqlRadiocomArtistWorkInfoRepositoryOptions.Value;
            _dbConnectionFactory = dbConnectionFactory;
        }
     
        public async Task<IEnumerable<IArtistWorkInfo>> GetArtistWorkInfosAsync(IEnumerable<int> ids)
        {
            List<IArtistWorkInfo> result = new List<IArtistWorkInfo>();
            using IDbConnection conn = await _dbConnectionFactory.GetConnection(_sqlRadiocomArtistWorkInfoRepositoryOptions.ConnectionString);
            foreach (int id in ids)
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("ArtistId", id, DbType.Int32);
                ArtistWorkInfo ArtistWorkInfo =
                    conn.QuerySingleOrDefault<ArtistWorkInfo>("SELECT * FROM dbo.ArtistWork WHERE Id = @ArtistId;", dynamicParameters);
                if(ArtistWorkInfo != null)
                {
                    result.Add(ArtistWorkInfo);
                }
            }
            return result;

        }
        public class SqlRadiocomArtistWorkInfoRepositoryOptions
        {
            public const string SqlRadiocomRepository = "SqlRadiocomArtistWorkInfoRepositoryOptions";
            public string ConnectionString { get; set; }
        }
        public class ArtistWorkInfo : IArtistWorkInfo
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int ArtistId { get; set; }
        }
    }
}
