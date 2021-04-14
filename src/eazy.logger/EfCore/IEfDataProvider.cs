using eazy.logger.EfCore.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
