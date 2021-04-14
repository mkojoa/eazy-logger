using Dapper;
using eazy.logger.EfCore.Entity;
using eazy.logger.Logging;
using eazy.logger.Logging.Options;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;



namespace eazy.logger.EfCore.Repo
{
    public class EfCoreDataProvider : IEfDataProvider
    {

        private readonly LoggerOptions _main;
        private readonly DatabaseOptions _options;

        public EfCoreDataProvider(LoggerOptions options)
        {
            _main = options;
            _options = _main.Database;
        }

        public async Task<(IEnumerable<LogModel>, int)> FetchDataAsync(
            int page, int count, string level = null, string searchCriteria = null
            )
        {
            var logsTask = GetLogsAsync(page - 1, count, level, searchCriteria);
            var logCountTask = CountLogsAsync(level, searchCriteria);

            await Task.WhenAll(logsTask, logCountTask);

            return (await logsTask, await logCountTask);
        }

        private async Task<IEnumerable<LogModel>> GetLogsAsync(int page, int count, string level, string searchCriteria)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT [Id], [Message], [Level], [TimeStamp], [Exception], [Properties] FROM[");
            queryBuilder.Append("dbo");
            queryBuilder.Append("].[");
            queryBuilder.Append(_options.Table);
            queryBuilder.Append("] ");

            var whereIncluded = false;

            if (!string.IsNullOrEmpty(level))
            {
                queryBuilder.Append("WHERE [LEVEL] = @Level ");
                whereIncluded = true;
            }

            if (!string.IsNullOrEmpty(searchCriteria))
            {
                queryBuilder.Append(whereIncluded
                    ? "AND [Message] LIKE @Search OR [Exception] LIKE @Search "
                    : "WHERE [Message] LIKE @Search OR [Exception] LIKE @Search ");
            }

            queryBuilder.Append("ORDER BY Id DESC OFFSET @Offset ROWS FETCH NEXT @Count ROWS ONLY");

            var fullConec = "" +
                            $"Server={_options.Instance};" +
                            $"Database={_options.Name};" +
                            $"User ID={_options.UserName};" +
                            $"Password={_options.Password};" +
                            $"Trusted_Connection={_options.TrustedConnection};" +
                            $"Encrypt={_options.Encrypt};" +
                            $"TrustServerCertificate={_options.TrustServerCertificate}";

            using (IDbConnection connection = new SqlConnection(fullConec)) 
            {
                
                var logs = await connection.QueryAsync<SqlServerLogModel>(queryBuilder.ToString(),
                    new
                    {
                        Offset = page * count,
                        Count = count,
                        Level = level,
                        Search = searchCriteria != null ? "%" + searchCriteria + "%" : null
                    });

                var index = 1;
                foreach (var log in logs)
                    log.RowNo = (page * count) + index++;

                return logs;
            }
        }

        public async Task<int> CountLogsAsync(string level, string searchCriteria)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT COUNT(Id) FROM[");
            queryBuilder.Append("dbo");
            queryBuilder.Append("].[");
            queryBuilder.Append(_options.Table);
            queryBuilder.Append("] ");

            var whereIncluded = false;

            if (!string.IsNullOrEmpty(level))
            {
                queryBuilder.Append("WHERE [LEVEL] = @Level ");
                whereIncluded = true;
            }

            if (!string.IsNullOrEmpty(searchCriteria))
            {
                queryBuilder.Append(whereIncluded
                    ? "AND [Message] LIKE @Search OR [Exception] LIKE @Search "
                    : "WHERE [Message] LIKE @Search OR [Exception] LIKE @Search ");
            }

            var fullConec = "" +
                            $"Server={_options.Instance};" +
                            $"Database={_options.Name};" +
                            $"User ID={_options.UserName};" +
                            $"Password={_options.Password};" +
                            $"Trusted_Connection={_options.TrustedConnection};" +
                            $"Encrypt={_options.Encrypt};" +
                            $"TrustServerCertificate={_options.TrustServerCertificate}";


            using (IDbConnection connection = new SqlConnection(fullConec))
            {
                return await connection.ExecuteScalarAsync<int>(queryBuilder.ToString(),
                    new
                    {
                        Level = level,
                        Search = searchCriteria != null ? "%" + searchCriteria + "%" : null
                    });
            }
        }
    }
}
