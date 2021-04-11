using System.Collections.Generic;
using eazy.logger.Logging.Options;

namespace eazy.logger.Logging
{
    public class LoggerOptions
    {
        public string ApplicationName { get; set; }
        public string Level { get; set; }
        public FileOptions File { get; set; }
        public SeqOptions Seq { get; set; }
        public DatabaseOptions Database { get; set; }
        public IDictionary<string, string> MinimumLevelOverrides { get; set; }
        public IEnumerable<string> ExcludePaths { get; set; }
        public IEnumerable<string> ExcludeProperties { get; set; }
        public IDictionary<string, object> Tags { get; set; }
    }
}