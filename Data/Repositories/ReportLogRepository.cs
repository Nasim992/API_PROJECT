using Dextor.API.Models.Core;
using Dextor.API.Models.BindingModel;
using Dextor.API.Data.IRepositories;

namespace Dextor.API.Data.Repositories
{
    public class ReportLogRepository : GenericRepository<ReportLog>, IReportLogRepository
    {
        // Add this constructor to fix the error!
        public ReportLogRepository(DatabaseContext context) : base(context)
        {
        }
    }
}