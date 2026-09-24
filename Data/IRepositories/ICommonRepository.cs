using Dextor.API.Data.Repositories;
using System.Collections.Generic;
using System.Data;

namespace Dextor.API.Data.IRepositories
{
    public interface ICommonRepository : IRepository<Dictionary<string, object>>
    {
        List<Dictionary<string, object>> GetSearchResult(string query);
        string GetJsonResult(string query);

        List<Dictionary<string, object>> GetSearchResultData(string table, Dictionary<string, object> filters);
        List<Dictionary<string, object>> GetSearchResult_secondary(string query);

        List<Dictionary<string, object>> GetSearchResult_POS_Master(string query);

        List<Dictionary<string, object>> GetSearchResult_IFS(string query);
        List<Dictionary<string, object>> GetSearchResult_DWDB(string query);

        List<Dictionary<string, object>> ExecProcWithJson(string procedure, string json, int timeoutSec);
        List<List<Dictionary<string, object>>> ExecProcWithJsonMultiResult(string procedureName, string json, int timeoutSec);

        DataSet ExecuteStoredProcedure(string procedureName,Dictionary<string, object> parameters = null,int timeoutSec = 300);

        List<Dictionary<string, object>> GetResult(string query);


    }
}
