using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Web;
using GenerateCosmoKey.Models;

namespace GenerateCosmoKey.Data
{
    public class ActivationData
    {
        private const string SetActivationkey = "dbo.INS_ActivationKey";
        private const string GetActivationkey = "dbo.GET_ActivationKeys";

        public int InsertActivationKey(CustomerInfo customerInfo)
        {
            SqlParameter retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.Int);
            retVal.Direction = ParameterDirection.Output;

            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ORG_NAME", customerInfo.OrganizationName.NullSafe()),
                new SqlParameter("@FIRSTNAME", customerInfo.FirstName.NullSafe()),
                new SqlParameter("@LASTNAME", customerInfo.LastName.NullSafe()),
                new SqlParameter("@EMAILID", customerInfo.EmailId.NullSafe()),
                new SqlParameter("@CONTACTNUMBER", customerInfo.PhoneNUmber.NullSafe()),
                new SqlParameter("@INTERNALKEY", customerInfo.InternalKey.NullSafe()),
                new SqlParameter("@LICENSETYPE", customerInfo.LicenseType),
                new SqlParameter("@PACKAGETYPE", customerInfo.PackageType),
                new SqlParameter("@ACTIVATIONKEY", customerInfo.ActivationKey.NullSafe()),
                new SqlParameter("@CONFIRMATION_NUMBER", customerInfo.ConfirmationNumber.NullSafe()),
                new SqlParameter("@CREATEDBY", string.IsNullOrEmpty(customerInfo.CreatedBy) ? "Admin": customerInfo.CreatedBy),
                retVal
            };

            return Convert.ToInt32(UtilityDL.ExecuteNonQuery(SetActivationkey, pList, true));
        }

        #region Get all ActivationKeyList

        public List<CustomerInfo> GetActivationList(string internalKey, int id)
        {
            var pList = new List<SqlParameter>
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@INTERNAL_KEY", internalKey)
            };
            return UtilityDL.FillData<CustomerInfo>(GetActivationkey, pList);
        }
        #endregion Get all ActivationKeyList
    }

    public static class StringHelper
    {
        public static string NullSafe(this string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text;
        }
    }
}