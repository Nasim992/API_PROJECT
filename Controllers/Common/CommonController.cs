using Dapper;
using Dextor.API.Data.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Dextor.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/common")]
    public class CommonController : ControllerBase
    {
        private readonly ICommonRepository _commonRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public CommonController(
            ICommonRepository commonRepository,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _commonRepository = commonRepository;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("search")]
        public IActionResult GetSearchResult([FromQuery] string query)
        {
            try
            {
                var results = _commonRepository.GetSearchResult(query);
                if (!results.Any()) return BadRequest("Data Not Available");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("search-data")]
        public IActionResult Search([FromBody] SearchRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Table) || request.Filters == null)
                    return BadRequest("Table and filters are required.");

                var results = _commonRepository.GetSearchResultData(request.Table, request.Filters);

                if (!results.Any()) return NotFound();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("search-post")]
        public IActionResult PostSearchResult([FromBody] QueryRequest request)
        {
            try
            {
                var results = _commonRepository.GetSearchResult(request.query);
                if (!results.Any()) return BadRequest("Data Not Available");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("get-result-post")]
        public IActionResult PostSearchResult_Result([FromBody] QueryRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.query))
                {
                    return Ok(new ApiDbResponse
                    {
                        Success = false,
                        Message = request == null ? "Request missing." : "Query missing.",
                        Data = new List<Dictionary<string, object>>()
                    });
                }

                var data = _commonRepository.GetResult(request.query);

                if (data == null || data.Count == 0)
                {
                    return Ok(new ApiDbResponse
                    {
                        Success = false,
                        Message = "Data Not Available",
                        Data = new List<Dictionary<string, object>>()
                    });
                }

                bool success = true;
                string message = "OK";

                if (data[0].TryGetValue("Status", out var statusObj) && int.TryParse(Convert.ToString(statusObj), out int status))
                {
                    success = status == 1;
                }

                if (data[0].TryGetValue("Message", out var messageObj))
                {
                    string spMessage = Convert.ToString(messageObj);
                    if (!string.IsNullOrWhiteSpace(spMessage)) message = spMessage;
                }

                return Ok(new ApiDbResponse { Success = success, Message = message, Data = data });
            }
            catch (SqlException ex)
            {
                return Ok(new ApiDbResponse
                {
                    Success = false,
                    SqlErrorNumber = ex.Number,
                    Message = ex.Errors != null && ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message,
                    Data = new List<Dictionary<string, object>>()
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiDbResponse { Success = false, Message = ex.Message, Data = new List<Dictionary<string, object>>() });
            }
        }

        [HttpPost("search-pos-master")]
        public IActionResult SearchResultPos([FromBody] QueryRequest request)
        {
            try
            {
                var results = _commonRepository.GetSearchResult_POS_Master(request.query);
                if (!results.Any()) return BadRequest("Data Not Available");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("search-pos-master-app")]
        public IActionResult SearchResultPos_forapp([FromBody] QueryRequest request)
        {
            try
            {
                var results = _commonRepository.GetSearchResult_POS_Master(request.query);
                if (!results.Any()) return BadRequest("Data Not Available");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search-secondary")]
        public IActionResult GetSearchResult_Secondary([FromQuery] string query)
        {
            try
            {
                var results = _commonRepository.GetSearchResult_secondary(query);
                if (!results.Any()) return BadRequest("Data Not Available");

                return Ok(results);
            }
            catch
            {
                return BadRequest("Data Not Available");
            }
        }

        [HttpPost("insert-update-json")]
        public IActionResult InsertUpdateByJson([FromBody] QueryRequestJson request)
        {
            try
            {
                if (request == null) return Ok(new ApiDbResponse { Success = false, Message = "Request missing." });
                if (string.IsNullOrWhiteSpace(request.procedure)) return Ok(new ApiDbResponse { Success = false, Message = "Procedure name missing." });

                int timeout = request.timeoutSec <= 0 ? 300 : request.timeoutSec;

                var data = _commonRepository.ExecProcWithJson(request.procedure.Trim(), request.json, timeout);

                bool success = true;
                string msg = "OK";

                if (data != null && data.Count > 0)
                {
                    if (data[0].TryGetValue("Status", out var stObj) && int.TryParse(Convert.ToString(stObj), out int st))
                        success = (st == 1);

                    if (data[0].TryGetValue("Message", out var msgObj))
                        msg = Convert.ToString(msgObj) ?? msg;
                }

                return Ok(new ApiDbResponse { Success = success, Message = msg, Data = data ?? new List<Dictionary<string, object>>() });
            }
            catch (SqlException ex)
            {
                return Ok(new ApiDbResponse
                {
                    Success = false,
                    SqlErrorNumber = ex.Number,
                    Message = (ex.Errors != null && ex.Errors.Count > 0) ? ex.Errors[0].Message : ex.Message,
                    Data = new List<Dictionary<string, object>>()
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiDbResponse { Success = false, Message = ex.Message, Data = new List<Dictionary<string, object>>() });
            }
        }

        [HttpPost("search-ifs-db")]
        public IActionResult SearchResultIFS([FromBody] QueryRequest request)
        {
            try
            {
                var results = _commonRepository.GetSearchResult_IFS(request.query ?? "");
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("search-dw-db")]
        public IActionResult search_dwdb([FromQuery] QueryRequest request)
        {
            try
            {
                var results = _commonRepository.GetSearchResult_DWDB(request.query ?? "");
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        //[HttpPost("send-sms")]
        //public async Task<IActionResult> SENDSMS([FromBody] SMS request)
        //{
        //    try
        //    {
        //        SmsResponse response = await send_sms(request.mobile_no, request.sms_body, request.unique_id);

        //        if (response.status_code == 200)
        //        {
        //            return Ok(response);
        //        }

        //        return BadRequest(response.error_message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}

        //// Converted to async Task and refactored to use HttpClient
        //private async Task<SmsResponse> send_sms(string toMobileNumber, string Body = "", string cSmsId = "")
        //{
        //    string API_Token = "oij51vgh-fhayosmx-3zgdojcy-hdcgxia5-ckis4skt";
        //    string SID = "APEXAPI";
        //    string smsBody = string.IsNullOrEmpty(Body)
        //        ? "This a system generated SMS from Apex HR-IS.<br/><br/>Thanks.<br/><br/>Apex ICURES.<br/>Developed by Apex IT"
        //        : Body;

        //    SMSModel sms = new SMSModel
        //    {
        //        api_token = API_Token,
        //        sid = SID,
        //        msisdn = toMobileNumber,
        //        sms = smsBody,
        //        csms_id = cSmsId
        //    };

        //    string smsUrl = "https://smsplus.sslwireless.com/api/v3/send-sms";
        //    string jsonData = JsonConvert.SerializeObject(sms);

        //    string response = await CallApiAsync(smsUrl, jsonData);
        //    return JsonConvert.DeserializeObject<SmsResponse>(response);
        //}

        // Replaced legacy HttpWebRequest with modern HttpClient
        private async Task<string> CallApiAsync(string url, string parameters)
        {
            // Note: Bypassing SSL validation is not recommended for production
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using (var client = new HttpClient(handler))
            {
                var content = new StringContent(parameters, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        //[HttpPost("queue")]
        //public async Task<CommonResponse> Queue(NotificationQueueRequest request)
        //{
        //    var commonResponse = new CommonResponse();
        //    try
        //    {
        //        // Replaced ConfigurationManager with IConfiguration
        //        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            string sql = @"
        //            INSERT INTO t_NotificationQueue (EmployeeID, Title, Body, NotificationType, ReferenceID)
        //            VALUES (@EmployeeID, @Title, @Body, @NotificationType, @ReferenceID)";

        //            await connection.ExecuteAsync(sql, request);
        //        }

        //        commonResponse.Status = 200;
        //        commonResponse.Message = "Notification queued successfully.";
        //        commonResponse.Data = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        commonResponse.Status = 500;
        //        commonResponse.Message = ex.Message;
        //        commonResponse.Data = false;
        //    }

        //    return commonResponse;
        //}

        //[HttpPost("send-notification")]
        //public async Task<string> SendNotification(NotificationRequest request)
        //{
        //    // Consider injecting these services in the constructor in the future
        //    DAL tokenService = new DAL();
        //    GoogleTokenService googleTokenService = new GoogleTokenService();
        //    NotificationBackgroundService firebaseService = new NotificationBackgroundService();

        //    string token = googleTokenService.GetAccessToken(tokenService.ReturnSDK(request.ProjectID));
        //    request.GoogleToken = token;

        //    return await firebaseService.SendNotification(request);
        //}

        #region SpCalling

        // ✅ whitelist: only SPs you want to expose
        private static readonly HashSet<string> AllowedProcs = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "dbo.sp_InsertProductStockTranItemScan",
            "dbo.sp_StockTranScan_StartStop",
            "dbo.sp_GetStockTranScanData_ByTranID",
            "dbo.sp_CancelDocument_CO_PO",
            "dbo.sp_PostDocument_CO_PO_ToInvoiceOrStock",
            "dbo.usp_ProductBulkUpload_Api"
        };

        //[HttpPost("sp-exec")]
        //public IActionResult SP_Exec_Generic([FromBody] SpExecRequest request)
        //{
        //    try
        //    {
        //        if (request == null || string.IsNullOrWhiteSpace(request.ProcName))
        //            return Ok(new ApiResponses { StatusCode = 400, Message = "ProcName is required", Data = "Error!!" });

        //        string proc = request.ProcName.Trim();

        //        if (!IsSafeProcName(proc))
        //            return Ok(new ApiResponses { StatusCode = 400, Message = "Invalid ProcName format.", Data = "Error!!" });

        //        if (!AllowedProcs.Contains(proc))
        //            return Ok(new ApiResponses { StatusCode = 403, Message = "SP not allowed.", Data = "Error!!" });

        //        // Replaced ConfigurationManager with IConfiguration
        //        string cs = _configuration.GetConnectionString("WEBConnection");

        //        using (SqlConnection conn = new SqlConnection(cs))
        //        using (SqlCommand cmd = new SqlCommand(proc, conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.CommandTimeout = request.CommandTimeout > 0 ? request.CommandTimeout : 120;

        //            if (request.Params != null)
        //            {
        //                foreach (SpExecParam p in request.Params)
        //                {
        //                    if (p == null || string.IsNullOrWhiteSpace(p.Name)) continue;

        //                    string pName = p.Name.Trim();
        //                    SqlDbType dbType = ParseSqlDbType(p.SqlDbType);
        //                    SqlParameter prm;

        //                    if (dbType == SqlDbType.Variant || string.IsNullOrWhiteSpace(p.SqlDbType))
        //                    {
        //                        prm = cmd.Parameters.AddWithValue(pName, p.Value ?? DBNull.Value);
        //                    }
        //                    else
        //                    {
        //                        prm = cmd.Parameters.Add(pName, dbType);
        //                        if (p.Size > 0) prm.Size = p.Size;
        //                        prm.Value = p.Value ?? DBNull.Value;
        //                    }
        //                }
        //            }

        //            if (request.Tvps != null)
        //            {
        //                foreach (SpExecTvp tvp in request.Tvps)
        //                {
        //                    if (tvp == null || string.IsNullOrWhiteSpace(tvp.Name) || string.IsNullOrWhiteSpace(tvp.TypeName))
        //                        continue;

        //                    DataTable dt = BuildTvpDataTable(tvp);
        //                    SqlParameter pTvp = cmd.Parameters.AddWithValue(tvp.Name.Trim(), dt);
        //                    pTvp.SqlDbType = SqlDbType.Structured;
        //                    pTvp.TypeName = tvp.TypeName.Trim();
        //                }
        //            }

        //            conn.Open();

        //            List<List<Dictionary<string, object>>> resultSets = new List<List<Dictionary<string, object>>>();
        //            int apiStatusCode = 200;
        //            string apiMessage = "Success!!";

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                int resultIndex = 0;
        //                do
        //                {
        //                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        //                    while (reader.Read())
        //                    {
        //                        Dictionary<string, object> row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        //                        for (int i = 0; i < reader.FieldCount; i++)
        //                        {
        //                            string col = reader.GetName(i);
        //                            object val = reader.IsDBNull(i) ? null : reader.GetValue(i);
        //                            row[col] = val;
        //                        }
        //                        rows.Add(row);
        //                    }

        //                    if (resultIndex == 0 && rows.Count > 0)
        //                    {
        //                        var firstRow = rows[0];
        //                        if (firstRow.ContainsKey("Success"))
        //                        {
        //                            int spSuccess = Convert.ToInt32(firstRow["Success"] ?? 0);
        //                            apiStatusCode = spSuccess == 1 ? 200 : 400;
        //                        }

        //                        if (firstRow.ContainsKey("Message") && firstRow["Message"] != null)
        //                        {
        //                            apiMessage = firstRow["Message"].ToString();
        //                        }
        //                    }

        //                    resultSets.Add(rows);
        //                    resultIndex++;

        //                } while (reader.NextResult());
        //            }

        //            return Ok(new ApiResponses
        //            {
        //                StatusCode = apiStatusCode,
        //                Message = apiMessage,
        //                Data = new { Proc = proc, ResultSets = resultSets }
        //            });
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        return Ok(new ApiResponses { StatusCode = 409, Message = ex.Message, Data = "SQL Error!!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new ApiResponses { StatusCode = 500, Message = ex.Message, Data = "Error!!" });
        //    }
        //}

        private static bool IsSafeProcName(string proc)
        {
            if (string.IsNullOrWhiteSpace(proc) || proc.Length > 200) return false;
            if (proc.Contains(";") || proc.Contains("--") || proc.Contains("/*") || proc.Contains("]") || proc.Contains("["))
                return false;
            return Regex.IsMatch(proc, @"^[a-zA-Z_][a-zA-Z0-9_]*\.[a-zA-Z_][a-zA-Z0-9_]*$");
        }

        private static SqlDbType ParseSqlDbType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return SqlDbType.Variant;
            if (Enum.TryParse(typeName.Trim(), true, out SqlDbType t)) return t;
            return SqlDbType.Variant;
        }

        private static DataTable BuildTvpDataTable(SpExecTvp tvp)
        {
            DataTable dt = new DataTable();
            if (tvp.Columns != null)
            {
                for (int i = 0; i < tvp.Columns.Count; i++)
                {
                    SpExecTvpColumn c = tvp.Columns[i];
                    if (c == null || string.IsNullOrWhiteSpace(c.Name)) continue;

                    Type colType = MapDotNetType(c.DataType);
                    DataColumn dc = new DataColumn(c.Name.Trim(), colType);
                    if (colType == typeof(string) && c.Size > 0) dc.MaxLength = c.Size;
                    dt.Columns.Add(dc);
                }
            }

            if (tvp.Rows != null)
            {
                foreach (Dictionary<string, object> r in tvp.Rows)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn c in dt.Columns)
                    {
                        object v = null;
                        if (r != null && r.ContainsKey(c.ColumnName)) v = r[c.ColumnName];
                        dr[c.ColumnName] = v ?? DBNull.Value;
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        private static Type MapDotNetType(string dataType)
        {
            string t = (dataType ?? "").Trim().ToLowerInvariant();
            if (t == "bigint") return typeof(long);
            if (t == "int") return typeof(int);
            if (t == "smallint") return typeof(short);
            if (t == "tinyint") return typeof(byte);
            if (t == "bit") return typeof(bool);
            if (t == "decimal" || t == "numeric" || t == "money") return typeof(decimal);
            if (t == "float") return typeof(double);
            if (t == "real") return typeof(float);
            if (t == "datetime" || t == "smalldatetime" || t == "date") return typeof(DateTime);
            if (t == "uniqueidentifier") return typeof(Guid);
            return typeof(string);
        }

        #endregion

        // Put your helper classes (QueryRequest, ApiDbResponse, etc.) here or in a separate Models folder
        public class SearchRequest { public string Table { get; set; } public Dictionary<string, object> Filters { get; set; } }
        public class QueryRequest { public string query { get; set; } }
        public class QueryRequestJson { public string query { get; set; } public string procedure { get; set; } public string json { get; set; } public int timeoutSec { get; set; } = 300; }
        public class ApiDbResponse { public bool Success { get; set; } public string Message { get; set; } public int? SqlErrorNumber { get; set; } public List<Dictionary<string, object>> Data { get; set; } public List<Dictionary<string, object>> NotificationRecipients { get; set; } }
        public class NotificationQueueRequest { public long EmployeeID { get; set; } public string Title { get; set; } public string Body { get; set; } public string NotificationType { get; set; } public long? ReferenceID { get; set; } }
        public class NotificationRequest { public long EmployeeID { get; set; } public string DeviceToken { get; set; } public string NotificationTitle { get; set; } public string NotificationText { get; set; } public string DefaultDashboard { get; set; } public string PermissionKey { get; set; } public string TabIndex { get; set; } public string ProjectID { get; set; } public string GoogleToken { get; set; } public long? ReferenceID { get; set; } public int TYear { get; set; } public int TMonth { get; set; } }
        public class SpExecRequest { public string ProcName { get; set; } public int CommandTimeout { get; set; } = 120; public List<SpExecParam> Params { get; set; } = new List<SpExecParam>(); public List<SpExecTvp> Tvps { get; set; } = new List<SpExecTvp>(); }
        public class SpExecParam { public string Name { get; set; } public object Value { get; set; } public string SqlDbType { get; set; } public int Size { get; set; } }
        public class SpExecTvp { public string Name { get; set; } public string TypeName { get; set; } public List<SpExecTvpColumn> Columns { get; set; } = new List<SpExecTvpColumn>(); public List<Dictionary<string, object>> Rows { get; set; } = new List<Dictionary<string, object>>(); }
        public class SpExecTvpColumn { public string Name { get; set; } public string DataType { get; set; } public int Size { get; set; } }
    }
}