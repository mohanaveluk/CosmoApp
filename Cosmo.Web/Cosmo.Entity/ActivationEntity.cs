using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace Cosmo.Entity
{
    public class ActivationEntity :TRW.NamedBusinessCode, IFill
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime CreatedDateTime { get; set; }


        public void Fill(System.Data.SqlClient.SqlDataReader reader)
        {
            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACCESS_ID"))
                Id = Convert.ToInt32(reader["ACCESS_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACCESS_CODE"))
                Key = Convert.ToString(reader["ACCESS_CODE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACCESS_FIRSTNAME"))
                FirstName = Convert.ToString(reader["ACCESS_FIRSTNAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACCESS_LASTTNAME"))
                LastName = Convert.ToString(reader["ACCESS_LASTTNAME"]);

        }

        public ActivationEntity Fill(OracleDataReader reader)
        {
            var entity = new ActivationEntity();

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACCESS_ID"))
                entity.Id = Convert.ToInt32(reader["ACCESS_ID"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACCESS_CODE"))
                entity.Key = Convert.ToString(reader["ACCESS_CODE"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACCESS_FIRSTNAME"))
                entity.FirstName = Convert.ToString(reader["ACCESS_FIRSTNAME"]);

            if (CommonUtility.IsColumnExistsAndNotNull(reader, "ACCESS_LASTTNAME"))
                entity.LastName = Convert.ToString(reader["ACCESS_LASTTNAME"]);


            return entity;
        }
    }
}
