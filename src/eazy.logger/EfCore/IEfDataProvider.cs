using System.Collections.Generic;
using System.Threading.Tasks;
using eazy.logger.EfCore.Entity;

namespace eazy.logger.EfCore
{
    public interface IEfDataProvider
    {
        Task<(IEnumerable<LogModel>, int)> FetchDataAsync(
            int page,
            int count,
            string level = null,
            string searchCriteria = null
        );
    }
}