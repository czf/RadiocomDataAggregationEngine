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
    public class SqlArtistWorkEventRepository : IArtistWorkEventsRepository
    {
        private readonly SqlArtistWorkEventRepositoryOptions _sqlArtistWorkEventRepositoryOptions;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlArtistWorkEventRepository(IOptions<SqlArtistWorkEventRepositoryOptions> sqlArtistWorkEventRepositoryOptions, IDbConnectionFactory dbConnectionFactory)
        {
            _sqlArtistWorkEventRepositoryOptions = sqlArtistWorkEventRepositoryOptions.Value;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<ArtistWorkEvent>> GetEventsForTimeSeriesAsync(TimeSeries timeSeries, int artistWorkId)
        {
            using IDbConnection conn = await _dbConnectionFactory.GetConnection(_sqlArtistWorkEventRepositoryOptions.ConnectionString);
            DynamicParameters dynamicParameters = new DynamicParameters();
            var timeRange = timeSeries.GetTimeSeriesDateRange();
            dynamicParameters.Add("StartBound", timeRange.start, DbType.DateTime);
            dynamicParameters.Add("EndBound", timeRange.end, DbType.DateTime);//not inclusive
            dynamicParameters.Add("ArtistWorkId", artistWorkId);
            IEnumerable<ArtistWorkEvent> result = await conn.QueryAsync<ArtistWorkEvent>(
@"SELECT awso.StartTime AS 'TimeStamp' 
  FROM dbo.ArtistWorkStationOccurrence awso
  INNER JOIN dbo.ArtistWork aw ON aw.Id = awso.ArtistWorkId
WHERE StartTime BETWEEN @StartBound AND @EndBound AND aw.Id = @ArtistWorkId", dynamicParameters);
            return result;
        }
            


        public class SqlArtistWorkEventRepositoryOptions
        {
            public const string SqlRadiocomRepository = "SqlArtistWorkEventRepositoryOptions";
            public string ConnectionString { get; set; }
        }

    }
}
