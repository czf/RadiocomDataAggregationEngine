using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Czf.Radiocom.Aggregation.Contracts.Values;
using Czf.Radiocom.Aggregation.Entities;
using Czf.Radiocom.Event.Repository.Contracts;
using Czf.Radiocom.Shared.Contracts;
using Dapper;
using Microsoft.Extensions.Options;

namespace Czf.Radiocom.Event.Repository.Implementations
{
    public class SqlArtistEventRepository : IArtistEventsRepository
    {
        private readonly SqlArtistEventRepositoryOptions _sqlArtistEventRepositoryOptions;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlArtistEventRepository(IOptions<SqlArtistEventRepositoryOptions> sqlArtistEventRepositoryOptions, IDbConnectionFactory dbConnectionFactory)
        {
            _sqlArtistEventRepositoryOptions = sqlArtistEventRepositoryOptions.Value;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<ArtistEvent>> GetEventsForTimeSeriesAsync(TimeSeries timeSeries, int artistId)
        {
            using IDbConnection conn = await _dbConnectionFactory.GetConnection(_sqlArtistEventRepositoryOptions.ConnectionString);
            DynamicParameters dynamicParameters = new DynamicParameters();
            var timeRange = timeSeries.GetTimeSeriesDateRange();
            dynamicParameters.Add("StartBound", timeRange.start, DbType.DateTime);
            dynamicParameters.Add("EndBound", timeRange.end, DbType.DateTime);//not inclusive
            dynamicParameters.Add("ArtistId", artistId);
            IEnumerable<ArtistEvent> result = await conn.QueryAsync<ArtistEvent>(
@"SELECT awso.StartTime AS 'TimeStamp' 
  FROM dbo.ArtistWorkStationOccurrence awso
  INNER JOIN dbo.ArtistWork aw ON aw.Id = awso.ArtistWorkId
WHERE StartTime BETWEEN @StartBound AND @EndBound AND aw.ArtistId = @ArtistId", dynamicParameters);
            return result;
        }
            


        public class SqlArtistEventRepositoryOptions
        {
            public const string SqlRadiocomRepository = "SqlArtistEventRepositoryOptions";
            public string ConnectionString { get; set; }
        }

    }
}
