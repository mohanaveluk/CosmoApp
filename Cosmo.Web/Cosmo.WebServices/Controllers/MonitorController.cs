using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Services.Description;
using System.Web.SessionState;
using Cosmo.Entity;
using Cosmo.Service;
using Cosmo.Web.forms;
using Cosmo.WebServices.Models;
using Newtonsoft.Json.Linq;

namespace Cosmo.WebServices.Controllers
{
    public class MonitorController : ApiController, IRequiresSessionState
    {
        private readonly MonitorService _monitorService;
        private readonly WinService _winService;
        private const string RequestSource = "Mobile";
        private const string AuthenticationFailed = "Authentication Failed";

        private static readonly string CosmoMonitorService = System.Configuration.ConfigurationManager.AppSettings["CosmoMonitorService"].ToString();
        private static readonly string AuthenticationFailureWebOnly = System.Configuration.ConfigurationManager.AppSettings["AuthenticationWebOnly"].ToString();
        private static readonly string AuthorizationFailure = System.Configuration.ConfigurationManager.AppSettings["AuthorizationFailure"].ToString();
        private static readonly string DbType = System.Configuration.ConfigurationManager.AppSettings["DatabaseType"].ToString();

        public MonitorController()
            : this(new MonitorService(), new WinService())
        {
        }

        public MonitorController(MonitorService monitorService, WinService winService)
        {
            _monitorService = monitorService;
            _winService = winService;
        }

        public int MobileUserId { get; set; }

        private ServiceMonitorResponse GetMonitorService()
        {
            Logger.Log("Source: " + RequestSource);
            Logger.Log("Calling GetMonitorService Service");

            try
            {
                var serviceList = _monitorService.GetAllMonitors(0, true);
                var cosmoServiceStatus = _winService.GetWindowsServiceStatus(CosmoMonitorService, string.Empty);

                return new ServiceMonitorResponse
                {
                    Status = ResponseStatus.Status.Success.ToString(),
                    Message = string.Empty,
                    CosmoService = new CosmoService
                    {
                        Name = CosmoMonitorService,
                        Status = cosmoServiceStatus
                    },
                    ServiceMoniters = serviceList,
                };
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return new ServiceMonitorResponse
                {
                    Status = ResponseStatus.Status.Failure.ToString(),
                    Message = ex.Message,
                    ServiceMoniters = null,
                };
            }
        }

        private ServiceMonitorResponse GetMonitorService(string envId)
        {
            var list = new List<ServiceMoniterEntity>();
            Logger.Log("Source: " + RequestSource);
            Logger.Log("Calling GetMonitorService Service for envId " + envId);

            try
            {
                var serviceList = _monitorService.GetAllMonitors(0, true);
                if (serviceList != null && serviceList.Count > 0)
                {
                    list = serviceList.Where(ml => ml.EnvID.ToString() == envId).ToList();
                }

                var cosmoServiceStatus = _winService.GetWindowsServiceStatus(CosmoMonitorService, string.Empty);

                return new ServiceMonitorResponse
                {
                    Status = ResponseStatus.Status.Success.ToString(),
                    Message = string.Empty,
                    CosmoService = new CosmoService
                    {
                        Name = CosmoMonitorService,
                        Status = cosmoServiceStatus
                    },
                    ServiceMoniters = list,
                };
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return new ServiceMonitorResponse
                {
                    Status = ResponseStatus.Status.Failure.ToString(),
                    Message = ex.Message,
                    ServiceMoniters = null,
                };
            }
        }

        private AuthenticationResponse Login(string userId, string password)
        {
            var userService = new UserService();
            var isAuthenticated = "No";
            var message = string.Empty;
            Logger.Log("Source: " + RequestSource);
            Logger.Log("Calling Login Service");

            try
            {
                /*
                 var dUserId = CommonUtility.DecryptString(userId);
                 var dPassword = CommonUtility.DecryptString(password);

                 var a = CommonUtility.Encrypt_ActivationKey("admin@cosmo.com");
                 var b = CommonUtility.Encrypt_ActivationKey("admincosmo");

                 var keys = CommonUtility.Encryption_MobileApp("admincosmo");
                 var keys = CommonUtility.Encryption_MobileApp("admincosmo");
                 var d = CommonUtility.Decryption_MobileApp(keys);
                 */

                //var uEmail = CommonUtility.Encryption_MobileApp("admin@cosmo.com");
                //var uPassword = CommonUtility.Encryption_MobileApp("admincosmo");


                var userIdText = "userId=";
                var passwordText = "&password=";
                var raw = HttpContext.Current.Request.RawUrl;
                Logger.Log("Incoming uri: "+raw);

                var queryString = raw.Substring(raw.IndexOf("?", StringComparison.Ordinal));
                var qsUserId = queryString.Substring(queryString.IndexOf(userIdText, StringComparison.Ordinal) + userIdText.Length,
                    queryString.IndexOf(passwordText, StringComparison.Ordinal) - (userIdText.Length + 1));
                var qsPassword = queryString.Substring(queryString.IndexOf(passwordText, StringComparison.Ordinal) + passwordText.Length);

                Logger.Log("User Id Key:" + qsUserId);
                Logger.Log("Password Key:" + qsPassword);

                string[] stringSeparators = new string[] { "'@'" };
                var sepUserId = qsUserId.Split(stringSeparators, StringSplitOptions.None);
                var sepPassword = qsPassword.Split(stringSeparators, StringSplitOptions.None);

                var userIdKeys=new EncryptionKeys();
                var passwordKeys = new EncryptionKeys();

                if (sepUserId.Length >0)
                {
                    userIdKeys = new EncryptionKeys {IvText = sepUserId[1], CiperText = sepUserId[0]};
                }
                if (sepPassword.Length > 0)
                {
                    passwordKeys = new EncryptionKeys {IvText = sepPassword[1], CiperText = sepPassword[0]};
                }


                var ddUserId = CommonUtility.DecryptTripleDes(userId);
                var ddPassword = CommonUtility.DecryptTripleDes(password);

                Logger.Log("User Id: " + ddUserId + "  Password: " + ddPassword);

                var loginStatus = userService.LoginUsers(ddUserId, ddPassword);

                if (loginStatus >= 1)
                {
                    isAuthenticated = "Yes";
                    var validUser = userService.GetUsers(loginStatus);
                    if (validUser != null && validUser.Count > 0)
                    {
                        if (HttpContext.Current.Session != null)
                        {
                            HttpContext.Current.Session["_LOGGED_USERD_ID"] = validUser[0].UserID;
                            HttpContext.Current.Session["_LOGGED_USERD_EMAIL"] = validUser[0].Email;
                            HttpContext.Current.Session["_LOGGED_USERNAME"] =
                                HttpContext.Current.Session["_LOGGED_USERFIRSTNAME"] = validUser[0].FirstName;
                        }
                    }
                    Logger.Log("User '" + ddUserId + "' was authenticated");
                }
                else if (loginStatus == 0)
                    message = "Invalid user / email address";
                else if (loginStatus == -1)
                    message = "Invalid password";

                if (!string.IsNullOrEmpty(message))
                {
                    Logger.Log("User '" + ddUserId + "' was not authenticated because of " + message);
                }

                return new AuthenticationResponse { Status = ResponseStatus.Status.Success.ToString(), Message = message, IsAuthenticated = isAuthenticated };
            }
            catch (Exception ex)
            {
                Logger.Log("Source: " + RequestSource);
                Logger.Log(ex.ToString());
                return new AuthenticationResponse
                {
                    Status = ResponseStatus.Status.Failure.ToString(),
                    Message = ex.Message,
                    IsAuthenticated = isAuthenticated
                };
            }
        }

        private OnDemandResponse OnDemandAction(string serviceName, string action, string systemName, string serviceId)
        {
            var message = string.Empty;
            var windowServiceStatus = string.Empty;

            Logger.Log(Request.Headers.UserAgent.ToString());
            Logger.Log("Source: " + RequestSource);
            Logger.Log("Calling OnDemandAction Service");
            Logger.Log(string.Format("Request to {0} the Service Name: {1} from server {2}", action, serviceName, systemName));

            try
            {
                if (string.IsNullOrEmpty(systemName))
                {
                    message = "Oops, Server Name is Missing";
                }
                else if (string.IsNullOrEmpty(serviceName))
                {
                    message = "Oops, Cognos Service Name is Missing";
                }
                else if (string.IsNullOrEmpty(serviceName))
                {
                    message = "Oops, Service Action is Missing";
                }
                if (!string.IsNullOrEmpty(message))
                    return new OnDemandResponse
                    {
                        Status = "Success",
                        Message = message,
                        ActionStatus = windowServiceStatus
                    };


                windowServiceStatus = _winService.GetWindowssService(serviceName, action, systemName, serviceId,
                    RequestSource);
                Logger.Log("Service Status: " + windowServiceStatus);
                Logger.Log(string.Format("Request completed to status: {0} for Service Name: {1} from server {2} ",
                    windowServiceStatus, serviceName, systemName));
                return new OnDemandResponse
                {
                    Status = ResponseStatus.Status.Success.ToString(),
                    Message = message,
                    ActionStatus = windowServiceStatus,
                };
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return new OnDemandResponse
                {
                    Status = ResponseStatus.Status.Failure.ToString(),
                    Message = ex.Message,
                    ActionStatus = null,
                };

            }
        }

        private OnDemandResponse OnDemandAction(string serviceName, string action, string systemName)
        {
            var message = string.Empty;
            var windowServiceStatus = string.Empty;

            Logger.Log(Request.Headers.UserAgent.ToString());
            Logger.Log("Source: " + RequestSource);
            Logger.Log("Calling OnDemandAction Service");
            Logger.Log(string.Format("Request to {0} the Service Name: {1} from server {2}", action, serviceName, systemName));
            try
            {
                if (string.IsNullOrEmpty(serviceName))
                {
                    message = "Oops, Cognos Service Name is Missing";
                }
                else if (string.IsNullOrEmpty(serviceName))
                {
                    message = "Oops, Service Action is Missing";
                }
                if (!string.IsNullOrEmpty(message))
                    return new OnDemandResponse
                    {
                        Status = "Success",
                        Message = message,
                        ActionStatus = windowServiceStatus
                    };

                windowServiceStatus = _winService.WindowServiceFuction(serviceName, action, systemName, RequestSource);

                Logger.Log("Service Status: " + windowServiceStatus);
                Logger.Log(string.Format("Request completed to status: {0} for Service Name: {1} from server {2} ",
                    windowServiceStatus, serviceName, systemName));
                return new OnDemandResponse
                {
                    Status = ResponseStatus.Status.Success.ToString(),
                    Message = message,
                    ActionStatus = windowServiceStatus,
                };
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return new OnDemandResponse
                {
                    Status = ResponseStatus.Status.Failure.ToString(),
                    Message = ex.Message,
                    ActionStatus = null,
                };

            }
        }

        [AcceptVerbs("GET")]
        [HttpGet]
        public string TestService()
        {
            return "Success";
        }

        [AcceptVerbs("POST")]
        [HttpPost]
        public AuthenticationResponse Login([FromBody] JObject jsonData)
        {
            var isAuthenticated = "No";
            Logger.Log("Source: " + RequestSource);
            Logger.Log("Calling Login Service");

            if (jsonData == null)
                return new AuthenticationResponse
                {
                    Status = ResponseStatus.Status.Success.ToString(),
                    Message = "Authentication details are required",
                    IsAuthenticated = isAuthenticated
                };


            try
            {
                dynamic json = jsonData;

                string userId = json.userId;
                string password = json.password;

                var userStatus = ValidateUser(userId, password);
                if (userStatus.Status == 1) isAuthenticated = "Yes";
                return new AuthenticationResponse
                {
                    Status = ResponseStatus.Status.Success.ToString(),
                    Message = userStatus.Message,
                    IsAuthenticated = isAuthenticated
                };
            }
            catch (Exception ex)
            {
                Logger.Log("Source: " + RequestSource);
                Logger.Log(ex.ToString());
                return new AuthenticationResponse
                {
                    Status = ResponseStatus.Status.Failure.ToString(),
                    Message = ex.Message,
                    IsAuthenticated = isAuthenticated
                };
            }
        }

        [AcceptVerbs("POST")]
        [HttpPost]
        public ServiceMonitorResponse GetMonitorService(JObject jsonData)
        {
            Logger.Log("Source: " + RequestSource);
            Logger.Log("Calling GetMonitorService Service");

            try
            {
                if (jsonData == null)
                    throw new ApplicationException("Authentication details are required");

                dynamic json = jsonData;

                string userId = json.userId;
                string password = json.password;
                string envId = json.envId ?? string.Empty;

                var userStatus = ValidateUser(userId, password);

                if (userStatus.Status < 1) throw new Web.login.ApplicationException(userStatus.Message);
                //throw new Web.login.ApplicationException(AuthenticationFailed);

                var serviceList = _monitorService.GetAllMonitors(0, true);
                if (!string.IsNullOrEmpty(envId))
                {
                    if (serviceList != null && serviceList.Count > 0)
                    {
                        serviceList = serviceList.Where(ml => ml.EnvID.ToString() == envId).ToList();
                    }
                }
                var cosmoServiceStatus = _winService.GetWindowsServiceStatus(CosmoMonitorService, string.Empty);

                return new ServiceMonitorResponse
                {
                    Status = ResponseStatus.Status.Success.ToString(),
                    Message = string.Empty,
                    CosmoService = new CosmoService
                    {
                        Name = CosmoMonitorService,
                        Status = cosmoServiceStatus
                    },
                    ServiceMoniters = serviceList,
                };
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return new ServiceMonitorResponse
                {
                    Status = ResponseStatus.Status.Failure.ToString(),
                    Message = ex.Message,
                    ServiceMoniters = null,
                };
            }
        }

        [AcceptVerbs("POST")]
        [HttpPost]
        public OnDemandResponse OnDemandAction([FromBody]JObject jsonData)
        {
            var message = string.Empty;
            var windowServiceStatus = string.Empty;
            Logger.Log(Request.Headers.UserAgent.ToString());
            Logger.Log("Source: " + RequestSource);
            Logger.Log("Calling OnDemandAction Service");

            try
            {
                if (jsonData == null)
                    throw new ApplicationException("Authentication details are required");

                dynamic json = jsonData;
                string userId = json.userId;
                string password = json.password;

                string serviceName = json.serviceName ?? string.Empty;
                string action = json.action ?? string.Empty;
                string systemName = json.systemName ?? string.Empty;
                string serviceId = json.serviceId ?? string.Empty;

                Logger.Log(string.Format("Request to '{0}' the Service'{1}' from server '{2}'", action, serviceName, systemName));

                var userStatus = ValidateUser(userId, password);

                if (userStatus.Status < 1) throw new Web.login.ApplicationException(userStatus.Message);

                if (string.IsNullOrEmpty(serviceName))
                {
                    message = "Oops, Cognos Service Name is Missing";
                }

                if (string.IsNullOrEmpty(action))
                {
                    message = "Oops, Service Action is Missing";
                }
                if (!string.IsNullOrEmpty(message))
                {
                    Logger.Log("Validation Message: " + message);
                    return new OnDemandResponse
                    {
                        Status = "Success",
                        Message = message,
                        ActionStatus = windowServiceStatus
                    };
                }
                
                if (string.IsNullOrEmpty(systemName))
                {
                    windowServiceStatus = _winService.WindowServiceFuction(serviceName, action, systemName, RequestSource);
                }
                else
                {
                    windowServiceStatus = _winService.GetWindowssService(serviceName, action, systemName, serviceId,
                        RequestSource, MobileUserId);
                }
                Logger.Log("Service Status: " + windowServiceStatus);
                Logger.Log(string.Format("Request completed to status: {0} for Service Name: {1} from server {2} ",
                    windowServiceStatus, serviceName, systemName));

                if (windowServiceStatus.Equals( "unabletoconnect"))
                    return new OnDemandResponse
                    {
                        Status = ResponseStatus.Status.Failure.ToString(),
                        Message = "Unable to connect to target server. Please check Agent is running.",
                        ActionStatus = string.Empty,
                    };


                return new OnDemandResponse
                {
                    Status = ResponseStatus.Status.Success.ToString(),
                    Message = message,
                    ActionStatus = windowServiceStatus,
                };

            }
            catch (Exception exception)
            {

                Logger.Log(exception.StackTrace);
                return new OnDemandResponse
                {
                    Status = ResponseStatus.Status.Failure.ToString(),
                    Message = exception.Message,
                    ActionStatus = null,
                };
            }
        }

        private ValidateUser ValidateUser(string qsUserId, string qsPassword)
        {
            var validateUser = new ValidateUser();
            var message = string.Empty;
            var userService = new UserService();

            Logger.Log("User Id Key:" + qsUserId);
            Logger.Log("Password Key:" + qsPassword);

            /*
            string[] stringSeparators = new string[] {"'@'"};
            var sepUserId = qsUserId.Split(stringSeparators, StringSplitOptions.None);
            var sepPassword = qsPassword.Split(stringSeparators, StringSplitOptions.None);

            var userIdKeys = sepUserId.Length > 0 ? new EncryptionKeys { IvText = sepUserId[1], CiperText = sepUserId[0] } : new EncryptionKeys();
            var passwordKeys = sepPassword.Length > 0 ? new EncryptionKeys { IvText = sepPassword[1], CiperText = sepPassword[0] } : new EncryptionKeys();
            */

            var ddUserId = CommonUtility.DecryptTripleDes(qsUserId);// CommonUtility.Decryption_MobileApp(userIdKeys);
            var ddPassword = CommonUtility.DecryptTripleDes(qsPassword);  //CommonUtility.Decryption_MobileApp(passwordKeys);
            
            //Logger.Log("User Id: " + ddUserId + "  Password: " + ddPassword);

            var loginStatus = userService.LoginUsers(ddUserId, ddPassword);

            //Validate License key
            var commonService = new CommonService();
            var licenseStatus = commonService.GetUserAccess();

            if (licenseStatus.Status.ToLower(CultureInfo.CurrentCulture).Equals("failure"))
            {
                validateUser.Status = -2;
                validateUser.Message = licenseStatus.Message;
                Logger.Log("License Key status: " + licenseStatus.Message);
                return validateUser;
            }
            else if (licenseStatus.PackageMode.ToLower().Equals("w"))
            {
                validateUser.Status = -2;
                validateUser.Message = AuthenticationFailureWebOnly;
                Logger.Log(
                    string.Format(
                        "License Key status: {0} because system has Web Only {1}",
                        AuthenticationFailureWebOnly, licenseStatus.PackageMode));
                return validateUser;
            }

            validateUser.Status = loginStatus;
            
            if (loginStatus >= 1)
            {
                MobileUserId = loginStatus;
                var validUser = userService.GetUsers(loginStatus);
                if (validUser != null && validUser.Count > 0)
                {
                    var role = validUser[0].UserRoles.Count(_ => _.RoleName.ToLower().Contains("mobile") || _.RoleName.ToLower().Contains("admin"));

                    if (role <= 0)
                    {
                        validateUser.Status = -3;
                        message = AuthorizationFailure;
                    }
                    else
                    { 
                        validateUser.Status = 1;
                    }
                    
                    if (HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session["_LOGGED_USERD_ID"] = validUser[0].UserID;
                        HttpContext.Current.Session["_LOGGED_USERD_EMAIL"] = validUser[0].Email;
                        HttpContext.Current.Session["_LOGGED_USERNAME"] =
                            HttpContext.Current.Session["_LOGGED_USERFIRSTNAME"] = validUser[0].FirstName;
                    }
                }
                Logger.Log("User '" + ddUserId + "' was authenticated");
            }
            else if (loginStatus == 0)
                message = "Invalid user / email address";
            else if (loginStatus == -1)
                message = "Invalid password";

            validateUser.Message = message;

            if (!string.IsNullOrEmpty(message))
            {
                Logger.Log("User '" + ddUserId + "' was not allowed because of " + message);
                throw new ApplicationException(message);
            }

            Logger.Log("User Status " + validateUser.Status + ", Message " + validateUser.Message);
            return validateUser;
        }
    }
}
