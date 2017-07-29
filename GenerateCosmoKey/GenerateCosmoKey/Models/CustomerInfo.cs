using System;
using System.Data.SqlClient;
using GenerateCosmoKey.Service;

namespace GenerateCosmoKey.Models
{
    public class CustomerInfo:IFill
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNUmber { get; set; }
        public string InternalKey { get; set; }
        public string LicenseType { get; set; }
        public string LicenseTypeDesc { get; set; }
        public string PackageType { get; set; }
        public string ActivationKey { get; set; }
        public string ConfirmationNumber { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }

        public void Fill(SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ID"))
                Id = Convert.ToInt32(reader["ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ORGANIZATION_NAME"))
                OrganizationName = Convert.ToString(reader["ORGANIZATION_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "FIRST_NAME"))
                FirstName = Convert.ToString(reader["FIRST_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LAST_NAME"))
                LastName = Convert.ToString(reader["LAST_NAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "EMAIL_ID"))
                EmailId = Convert.ToString(reader["EMAIL_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PHONE_NUMBER"))
                PhoneNUmber = Convert.ToString(reader["PHONE_NUMBER"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "INTERNAL_KEY"))
                InternalKey = Convert.ToString(reader["INTERNAL_KEY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LICENSE_TYPE"))
                LicenseType = Convert.ToString(reader["LICENSE_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "LICENSE_TYPE_DESC"))
                LicenseTypeDesc = Convert.ToString(reader["LICENSE_TYPE_DESC"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACTIVATION_KEY"))
                ActivationKey = Convert.ToString(reader["ACTIVATION_KEY"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "PACKAGE_TYPE"))
                PackageType = Convert.ToString(reader["PACKAGE_TYPE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CONFIRMATION_NUMBER"))
                ConfirmationNumber = Convert.ToString(reader["CONFIRMATION_NUMBER"]); 

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CAREATED_DATE"))
                CreatedDateTime = Convert.ToDateTime(reader["CAREATED_DATE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "CREATED_BY"))
                CreatedBy = Convert.ToString(reader["CREATED_BY"]);
        }

    }
}