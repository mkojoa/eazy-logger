using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using eazy.logger.EfCore.Entity;
using eazy.logger.Logging;
using eazy.logger.Logging.Options;
using Microsoft.Data.SqlClient;

namespace eazy.logger.EfCore.Repo
{
    public class EfCoreDataProvider : IEfDataProvider
    {
        private readonly DatabaseOptions _options;

        public EfCoreDataProvider(LoggerOptions options)
        {
            _options = options.Database;
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
                queryBuilder.Append(whereIncluded
                    ? "AND [Message] LIKE @Search OR [Exception] LIKE @Search "
                    : "WHERE [Message] LIKE @Search OR [Exception] LIKE @Search ");

            queryBuilder.Append("ORDER BY Id DESC OFFSET @Offset ROWS FETCH NEXT @Count ROWS ONLY");

            var fullConec = "" +
                            $"Server={_options.Instance};" +
                            $"Database={_options.Name};" +
                            $"User ID={_options.UserName};" +
                            $"Password={_options.Password};" +
                            $"Trusted_Connection={_options.TrustedConnection};" +
                            $"Encrypt={_options.Encrypt};" +
                            $"TrustServerCertificate={_options.TrustServerCertificate}";
            using IDbConnection connection = new SqlConnection(fullConec);


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
                log.RowNo = page * count + index++;

            return logs;
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
                queryBuilder.Append(whereIncluded
                    ? "AND [Message] LIKE @Search OR [Exception] LIKE @Search "
                    : "WHERE [Message] LIKE @Search OR [Exception] LIKE @Search ");

            var fullConec = "" +
                            $"Server={_options.Instance};" +
                            $"Database={_options.Name};" +
                            $"User ID={_options.UserName};" +
                            $"Password={_options.Password};" +
                            $"Trusted_Connection={_options.TrustedConnection};" +
                            $"Encrypt={_options.Encrypt};" +
                            $"TrustServerCertificate={_options.TrustServerCertificate}";


            using IDbConnection connection = new SqlConnection(fullConec);
            return await connection.ExecuteScalarAsync<int>(queryBuilder.ToString(),
                new
                {
                    Level = level,
                    Search = searchCriteria != null ? "%" + searchCriteria + "%" : null
                });
        }

        public void ProgramaticallyCreateTable(string fullConec)
        {
            var queryBuilder = new StringBuilder();
            var myConn = new SqlConnection(fullConec);

            queryBuilder.Append("IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[");
            queryBuilder.Append("dbo");
            queryBuilder.Append("].[");
            queryBuilder.Append(_options.Table);
            queryBuilder.Append("]");
            queryBuilder.Append("') AND type in (N'U'))");
            queryBuilder.Append("BEGIN");
            queryBuilder.Append("CREATE TABLE[");
            queryBuilder.Append("dbo");
            queryBuilder.Append("].[");
            queryBuilder.Append(_options.Table);
            queryBuilder.Append("]");
            queryBuilder.Append("[Id][int] IDENTITY(1, 1) NOT NULL,");
            queryBuilder.Append("[Message] [nvarchar](max)NULL,");
            queryBuilder.Append("[MessageTemplate] [nvarchar](max)NULL,");
            queryBuilder.Append("[Level] [nvarchar](128) NULL,");
            queryBuilder.Append("[TimeStamp] [datetimeoffset](7) NOT NULL,");
            queryBuilder.Append("[Exception] [nvarchar](max)NULL,");
            queryBuilder.Append("[Properties] [xml] NULL,");
            queryBuilder.Append("[LogEvent] [nvarchar](max)NULL,");
            queryBuilder.Append("CONSTRAINT[PK_Logs] PRIMARY KEY CLUSTERED");
            queryBuilder.Append("([Id] ASC)");
            queryBuilder.Append(
                "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]");
            queryBuilder.Append(") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]");
            queryBuilder.Append("END");

            var myCommand = new SqlCommand(queryBuilder.ToString(), myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
            }
            finally
            {
                if (myConn.State == ConnectionState.Open) myConn.Close();
            }
        }
    }
}