using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using TRW;

namespace Cog.CSM.Data
{
    public abstract class BusinessEntityBaseDAO : BaseDAO
    {
        #region Constants

        protected const string DATAREADER_ID_COL = "ID";
        protected const string DATAREADER_NAME_COL = "NAME";

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public BusinessEntityBaseDAO()
            : base()
        {
            //Empty Constructor
        }

        /// <summary>
        /// Uses the given connection string to connection to a database
        /// </summary>
        /// <param name="connectionString"></param>
        public BusinessEntityBaseDAO(string connectionString)
            : base(connectionString)
        {
        }

        #endregion

        /// <summary>
        /// Implementation of the base RowToEntityDelegate. Returns an NamedBusinessEntity from a 
        /// row in the OracleDataReader. Expects an OracleDataReader that contains
        /// a numeric ID in the first column and a string name in the second
        /// </summary>
        /// <param name="r">An open OracleDataReader that has a current row containing
        /// a numeric ID in the first column and a name in the second</param>
        /// <returns>An NamedBusinessEntity instanted from the data in the current row of the 
        /// OracleDataReader.</returns>
        protected NamedBusinessEntity RowToNamedBusinessEntity(OracleDataReader r)
        {
            NamedBusinessEntity i = new NamedBusinessEntity { ID = OracleDecimalToInt(r.GetDecimal(0)) };

            if (r.FieldCount > 1)
                i.Name = r.GetString(1);
            return i;
        }
    }
}
