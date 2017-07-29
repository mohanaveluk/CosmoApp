using System;
using System.Web.Mvc;
using GenerateCosmoKey.Models;
using GenerateCosmoKey.Service;

namespace GenerateCosmoKey.Controllers
{
    public class CreateLicenseController : Controller
    {
        //
        // GET: /CreateLicense/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetActivationList(string internalKey, string id)
        {
            string status = null;
            try
            {
                if (string.IsNullOrEmpty(internalKey) && string.IsNullOrEmpty(id))
                    return new JsonResult { Data = new { Status = "No data", List = new CustomerInfo() } };

                var decryptedInternalKey = CommonUtility.Decrypt(internalKey);
                var internalKeySplit = decryptedInternalKey.Split(new[] { "@@" }, StringSplitOptions.None);
                if (internalKeySplit.Length > 1)
                {
                    ViewBag.PackageType = internalKeySplit[0];
                    ViewBag.InternalKey = internalKeySplit[1];
                }

                var activationService = new ActivationService();
                var licId = string.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(id);
                var list = activationService.GetActivationList(internalKey, licId);
                return new JsonResult {Data = new {Status = "Success", PackageType = ViewBag.PackageType, List = list}};
            }
            catch (Exception e)
            {
                status = e.Message;
            }
            return new JsonResult { Data = new { Status = status, PackageType = ViewBag.PackageType, List = new CustomerInfo() } };
        }

        [HttpPost]
        public JsonResult SubmitInfo(string orgName, string firstName, string lastName, string emailId,
            string contactNumber, string licenseType, string internalKey, string confirmationNumber)
        {
            string status = null;
            string keyStatus = null;
            try
            {
                var customerInfo = new CustomerInfo
                {
                    OrganizationName = orgName,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailId = emailId,
                    PhoneNUmber = contactNumber,
                    InternalKey = internalKey,
                    LicenseType = licenseType,
                    ConfirmationNumber = confirmationNumber
                };

                if (!string.IsNullOrEmpty(internalKey))
                {
                    var decryptedInternalKey = CommonUtility.Decrypt(internalKey);
                    var newInternalKey = decryptedInternalKey;

                    var internalKeySplit = decryptedInternalKey.Split(new []{"@@"},StringSplitOptions.None);
                    if (internalKeySplit.Length > 1)
                    {
                        newInternalKey = internalKeySplit[1];
                        customerInfo.PackageType = internalKeySplit[0];
                    }


                    var activationKey = licenseType == "1"
                        ? newInternalKey + "&" + DateTime.Now.AddDays(30).ToShortDateString() + "&" + licenseType + "&" +
                          customerInfo.PackageType + "&" + confirmationNumber
                        : newInternalKey + "&" + DateTime.Now + "&" + licenseType + "&" +
                          customerInfo.PackageType + "&" + confirmationNumber;

                    
                    customerInfo.ActivationKey = CommonUtility.Encrypt_ActivationKey(activationKey);

                    var activationService = new ActivationService();
                    keyStatus = activationService.InsertActivationKey(customerInfo);
                    return new JsonResult {Data = new {Status = keyStatus, CustomerInfo = customerInfo}};
                }
            }
            catch (Exception e)
            {
                status = e.Message;
            }
            return new JsonResult {Data = new {Status = "Unsuccess :" + keyStatus + "-" + status}};
        }
    }
}
