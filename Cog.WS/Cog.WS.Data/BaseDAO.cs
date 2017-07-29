using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Cog.WS.Entity;
using log4net;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using TRW;

namespace Cog.WS.Data
{
    public abstract class BaseDAO : CODataSource
    {

        #region constants

        private const string MSG_COMMIT_TXN_ERROR = "Caught this excption trying to commit the current transaction";
        private const string MSG_ROLLBACK_TXN_ERROR = "Caught this excption trying to rollback the current transaction";

        private const string FETCH_SIZE_TAG = "NumberOfRowsForFetchSize";
        private const string MSG_MISSING_CONNECTION_STRING = "ConnectionString configuration is missing from you web.config. It should contain a key for OracleDBConnection";
        private const string MSG_NULL_PARAMETER = "Received an unexpected NULL parameter";
        protected const string SQL_WILDCARD_CHARACTER = "%";
        private const string OUTPUT_CURSOR_PARAM_NAME = "o_Cursor";
        private const string MSG_READ_ENTITY_EXCEPTION = "Caught exception in ReadEntityList";

        //Constant for Oracle error numbers (Why didn't Oracle define their own???)
        protected const int ORACLE_UNIQUE_CONSTRAINT_VIOLATED = 1;
        protected const int ORACLE_NO_DATA_FOUND = 1403;
        protected const int ORACLE_REF_INTEGRITY = 2292;

        //Return value constants
        public int EMPTY_INT = Int32.MinValue;

        #endregion

        #region delegates
        protected delegate BusinessEntity RowToEntityDelegate(OracleDataReader r);
        protected delegate void BusinessEntityToCommandDelegate(OracleCommand c, BusinessEntity v);
        protected delegate List<T> ReaderToEntityListDelegate<T>(OracleDataReader r);
        #endregion

        #region instance variables

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //Holds the active transaction
        protected OracleTransaction _txn = null;

        //If there's an active transaction, then this holds a reference to the active connection as well.
        //We keep this in a separate variable because the Oracle ODP libraries null out the Connection 
        //property on the Transaction object when the Commit() method is called.
        protected OracleConnection _connection = null;

        #endregion

        #region constructors

        /// <summary>
        /// Instantiates a BaseDAO object
        /// </summary>
        public BaseDAO()
            : base()
        {
            //this.ConnectionString = TRW.ConfigurationUtility.GetConfigValue(this.GetType(), "ConnectionString", false);
            //this.ConnectionString = ConfigurationUtility.GetConfigValue(this.GetType(), "ConnectionString", false);
            this.ConnectionString = CommonUtility.GetParsedConnectionString(ConfigurationManager.ConnectionStrings["CSMConn"].ConnectionString);
        }

        /// <summary>
        /// Instantiates a BaseDAO object with the given connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAO(string connectionString)
            : base()
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Instantiates a BaseDAO object and associates it with an open transaction. All
        /// database activities performed by this BaseDAO will be enrolled in the open transaction
        /// </summary>
        /// <param name="txn"></param>
        public BaseDAO(OracleTransaction txn)
        {
            _txn = txn;
            if (_txn != null)
            {
                _connection = _txn.Connection;
            }
        }
        #endregion

        #region properties

        /// <summary>
        /// Connection string for the application database
        /// </summary>
        /// <value>A <see langref="string"/>containing the ADO connection string for the application database</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Returns a reference to an open Transaction object. Null if no transactions are open
        /// </summary>
        public OracleTransaction OpenTransaction
        {
            get { return _txn; }
        }

        /// <summary>
        /// Indicates the number of database roles the Oracle ODP.NET libraries will pull from 
        /// the database at once. This number should always be greater than 1. The larger this number,
        /// the fewer times Oracle ODP.NET will have to request data across the network. Test 
        /// to find the optimal setting
        /// </summary>
        protected int NumberOfRowsForFetchSize { get; set; }


        #endregion

        #region public methods

        /// <summary>
        /// Closes and disposes of any open connections and transaction resources
        /// </summary>
        public void CloseConnections()
        {
            if (_connection != null)
            {
                //OracleConnection.ClearPool(_connection);
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }

            if (_txn != null)
            {
                _txn.Dispose();
                _txn = null;
            }
        }

        /// <summary>
        /// Begins a transaction and returns a reference to the transaction object. If there is 
        /// currently a transaction open, it is rolledback and disposed of. All subsequent database
        /// activities on this DAO will be enlisted in this transaction. IMPORTANT! IF YOU CALL 
        /// StartTransaction(), BE SURE AND CALL CloseConnections() FROM A finally CLAUSE TO 
        /// ENSURE THAT RESOURCES ARE FREED UP!
        /// </summary>
        /// <returns></returns>
        public OracleTransaction StartTransaction()
        {
            if (_txn != null)
            {
                _txn.Rollback();
                _txn.Dispose();
                _txn = null;
            }

            OracleConnection conn = GetOpenConnection();
            _txn = conn.BeginTransaction();
            _connection = _txn.Connection;
            return _txn;
        }

        /// <summary>
        /// Commits the current transaction 
        /// </summary>
        public void CommitCurrentTransaction()
        {
            if (_txn == null)
                return;

            try
            {
                _txn.Commit();
            }
            catch (Exception e)
            {
                throw new Exception(MSG_COMMIT_TXN_ERROR, e);
            }
        }

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        public void RollbackCurrentTransaction()
        {
            if (_txn == null)
                return;

            try
            {
                _txn.Rollback();
            }
            catch (Exception e)
            {
                throw new Exception(MSG_ROLLBACK_TXN_ERROR, e);
            }
        }

        #endregion

        #region protected methods

        /// <summary>
        /// Returns an open OracleConnection object. If a transaction is currently active, this method
        /// returns the connection associated with that transaction. If a transaction is not active,
        /// this method returns a new connection.
        /// </summary>
        /// <returns></returns>
        protected OracleConnection GetOpenConnection()
        {
            //Log.Debug("In GetOpenConnection");

            if ((_txn != null) && (_connection != null))
            {
                //Log.Debug("Connection already exists - returning the already opened connection");
                return _connection;
            }
            else
            {
                //Log.DebugFormat("Creating a new connection - connection string is {0}", this.ConnectionString);

                OracleConnection conn = new OracleConnection(this.ConnectionString);

                //Log.Debug("Opening the new connection");

                conn.Open();

                //Log.Debug("Returning the newly opened connection");

                return conn;
            }
        }

        /// <summary>
        /// Returns a list of BusinessEntity objects from the database. 
        /// </summary>
        /// <remarks>This method assumes that the SQLText is the name of a Oracle Function that returns an RetCursor</remarks>
        /// <typeparam name="T">Identifies the type of object returned. Must be a subclass of NamedBusinessEntity</typeparam>
        /// <param name="SQLText">The SQL statement to execute. This is presumed to be a stored procedure returning a refcursor</param>
        /// <param name="p">A list of <see cref="OracleParameter">OracleParameters</see>. These are parameters required by the SQL
        /// statement. If the SQL statement requires no parameters, then this must be an empty list.</param>
        /// <param name="f">A method that knows how to instantiate a ValueObject from the rows returned by executing the given SQL statement</param>
        /// <returns>A list of 'T' object which were populated via the database query in the SQLText parameter
        /// parameter</returns>
        protected List<T> ReadEntityList<T>(String SQLCommandText, List<OracleParameter> parameterList, RowToEntityDelegate f) where T : BusinessEntity, new()
        {
            return ReadEntityList<T>(SQLCommandText, parameterList, f, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Returns a list of BusinessEntity objects from the database. 
        /// </summary>
        /// <remarks>This method assumes that the SQLText is the name of a Oracle Function that returns an RetCursor</remarks>
        /// <typeparam name="T">Identifies the type of object returned. Must be a subclass of BusinessEntity</typeparam>
        /// <param name="SQLText">The SQL statement to execute. This is presumed to be a stored procedure returning a refcursor</param>
        /// <param name="p">A list of <see cref="OracleParameter">OracleParameters</see>. These are parameters required by the SQL
        /// statement. If the SQL statement requires no parameters, then this must be an empty list.</param>
        /// <param name="f">A method that knows how to instantiate a ValueObject from the rows returned by executing the given SQL statement</param>
        /// <returns>A list of 'T' object which were populated via the database query in the SQLText parameter
        /// parameter</returns>
        protected List<T> ReadEntityList<T>(String SQLCommandText, List<OracleParameter> parameterList, RowToEntityDelegate f, CommandType commandType) where T : BusinessEntity, new()
        {
            //Log.DebugFormat("In ReadEntityList with command text of {0}", SQLCommandText);

            //Log.Debug("Making a call to open the connection");

            OracleConnection conn = GetOpenConnection();
            try
            {
                //Log.DebugFormat("Creating the command object - connection state is {0}", conn.State.ToString());

                OracleCommand command = conn.CreateCommand();
                //command.FetchSize = 100000;

                //Log.DebugFormat("Setting command object command type to {0}", commandType.ToString());

                command.CommandType = commandType;
                command.XmlCommandType = OracleXmlCommandType.None;

                //Log.DebugFormat("Setting command object command text to {0}", SQLCommandText);

                command.CommandText = SQLCommandText;

                //Assume we're always calling a stored procedure

                //Log.Debug("Creating output parameter for REFCURSOR variable");

                //furthermore, assume we're always calling a function that is going to return a RefCursor. Make that first in
                //the parameter list, then add the ones supplied by the caller

                command.Parameters.Add(new OracleParameter(OUTPUT_CURSOR_PARAM_NAME, OracleDbType.RefCursor, ParameterDirection.ReturnValue));

                //Log.Debug("Adding parameters to command's parameter list");

                if (parameterList != null)
                {
                    foreach (OracleParameter p1 in parameterList)
                    {
                        command.Parameters.Add(p1);
                    }
                }

                //Log.Debug("executing the command and returning a reader object");

                using (OracleDataReader myDataReader = command.ExecuteReader())
                {
                    //if ((command.RowSize > 0) && (this.NumberOfRowsForFetchSize > 0))
                    //{
                    //    myDataReader.FetchSize = command.RowSize * this.NumberOfRowsForFetchSize;
                    //}

                    //Log.Debug("Fetching rows from reader");

                    List<T> l = new List<T>();
                    while (myDataReader.Read())
                    {
                        l.Add((T)f(myDataReader));
                    }

                    //Log.DebugFormat("{0} rows fetched - returning the row data list", l.Count.ToString());

                    return l;
                }
            }
            catch (Exception e)
            {
                throw new Exception(MSG_READ_ENTITY_EXCEPTION + " " + e.Message, e);
            }
            finally
            {
                //Don't close the connection if a transaction is open. 
                if (_txn == null)
                {
                    //OracleConnection.ClearPool(conn);
                    conn.Close();
                    conn.Dispose();
                }
            }

        }

        /// <summary>
        /// Returns a list of compound BusinessEntity objects from the database. 
        /// </summary>
        /// <remarks>This method assumes that the SQLText is the name of a Oracle Function that returns an RetCursor. This methods
        /// delegate parameter is of type ReaderToEntityListDelegate. This is a method that takes an OracleDataReader as a parameter
        /// and returns a List of BusinessEntities. This delegate is used when a compound BusinessEntity is created (i.e. a 
        /// BusinessEntity that contains a collection of other BusinessEntities)"/></remarks>
        /// <typeparam name="T">Identifies the type of object returned. Must be a subclass of BusinessEntity</typeparam>
        /// <param name="SQLText">The SQL statement to execute. This is presumed to be a stored procedure returning a refcursor</param>
        /// <param name="p">A list of <see cref="OracleParameter">OracleParameters</see>. These are parameters required by the SQL
        /// statement. If the SQL statement requires no parameters, then this must be an empty list.</param>
        /// <param name="f">A method that knows how to instantiate a list of BusinessEntities from a OracleDataReader</param>
        /// <returns>A list of 'T' object which were populated via the database query in the SQLText parameter
        /// parameter</returns>
        protected List<T> ReadCompoundEntityList<T>(String SQLCommandText, List<OracleParameter> parameterList, ReaderToEntityListDelegate<T> f) where T : BusinessEntity, new()
        {
            OracleConnection conn = GetOpenConnection();
            try
            {
                OracleCommand command = conn.CreateCommand();
                command.CommandText = SQLCommandText;
                //Assume we're always calling a stored procedure
                command.CommandType = CommandType.StoredProcedure;

                //furthermore, assume we're always calling a function that is going to return a RefCursor. Make that first in
                //the parameter list, then add the ones supplied by the caller
                command.Parameters.Add(new OracleParameter(OUTPUT_CURSOR_PARAM_NAME, OracleDbType.RefCursor, ParameterDirection.ReturnValue));
                if (parameterList != null)
                {
                    foreach (OracleParameter p1 in parameterList)
                    {
                        command.Parameters.Add(p1);
                    }
                }



                using (OracleDataReader myDataReader = command.ExecuteReader())
                {
                    //if ((command.RowSize > 0) && (this.NumberOfRowsForFetchSize > 0))
                    //{
                    //    myDataReader.FetchSize = command.RowSize * this.NumberOfRowsForFetchSize;
                    //}

                    List<T> l = f(myDataReader);

                    return l;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //Don't close the connection if a transaction is open. 
                if (_txn == null)
                {
                    //OracleConnection.ClearPool(conn);
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Returns a list of compound BusinessEntity objects from the database. 
        /// </summary>
        /// <remarks>This method assumes that the SQLText is the name of a Oracle Function that returns an RetCursor. This methods
        /// delegate parameter is of type ReaderToEntityListDelegate. This is a method that takes an OracleDataReader as a parameter
        /// and returns a List of BusinessEntities. This delegate is used when a compound BusinessEntity is created (i.e. a 
        /// BusinessEntity that contains a collection of other BusinessEntities)"/></remarks>
        /// <typeparam name="T">Identifies the type of object returned. Must be a subclass of BusinessEntity</typeparam>
        /// <param name="SQLText">The SQL statement to execute. This is presumed to be a stored procedure returning a refcursor</param>
        /// <param name="p">A list of <see cref="OracleParameter">OracleParameters</see>. These are parameters required by the SQL
        /// statement. If the SQL statement requires no parameters, then this must be an empty list.</param>
        /// <param name="f">A method that knows how to instantiate a list of BusinessEntities from a OracleDataReader</param>
        /// <returns>A list of 'T' object which were populated via the database query in the SQLText parameter
        /// parameter</returns>
        protected List<T> ReadCompoundEntityListProc<T>(String SQLCommandText, List<OracleParameter> parameterList, ReaderToEntityListDelegate<T> f) where T : BusinessEntity, new()
        {
            OracleConnection conn = GetOpenConnection();
            try
            {
                OracleCommand command = conn.CreateCommand();
                command.CommandText = SQLCommandText;
                //Assume we're always calling a stored procedure
                command.CommandType = CommandType.StoredProcedure;

                //furthermore, assume we're always calling a function that is going to return a RefCursor. Make that first in
                //the parameter list, then add the ones supplied by the caller
                //command.Parameters.Add(new OracleParameter(OUTPUT_CURSOR_PARAM_NAME, OracleDbType.RefCursor, ParameterDirection.ReturnValue));
                if (parameterList != null)
                {
                    foreach (OracleParameter p1 in parameterList)
                    {
                        command.Parameters.Add(p1);
                    }
                }

                command.Parameters.Add(new OracleParameter("result", OracleDbType.RefCursor, ParameterDirection.Output));


                using (OracleDataReader myDataReader = command.ExecuteReader())
                {
                    //if ((command.RowSize > 0) && (this.NumberOfRowsForFetchSize > 0))
                    //{
                    //    myDataReader.FetchSize = command.RowSize * this.NumberOfRowsForFetchSize;
                    //}

                    List<T> l = f(myDataReader);

                    return l;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //Don't close the connection if a transaction is open. 
                if (_txn == null)
                {
                    //OracleConnection.ClearPool(conn);
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        /// <summary>
        /// Saves the contents of a BusinessEntity to the database
        /// </summary>
        /// <param name="SQLText">The SQL statement to execute. This is presumed to be a stored procedure that
        /// either inserts or updates data
        /// </param>
        /// <param name="f">Reference to a delegate that knows how to take a given Business Entity
        /// and populate the parameters of an OracleCommand object from that Entity</param>
        /// <param name="vo">The NamedBusinessEntity (or a child of NamedBusinessEntity) whose values are to be saved
        /// in the database</param>
        protected void SaveBusinessEntity(string SQLText, BusinessEntityToCommandDelegate f, BusinessEntity vo)
        {
            OracleConnection conn = GetOpenConnection();
            try
            {
                OracleCommand command = conn.CreateCommand();
                command.CommandText = SQLText;
                //Assume we're always calling a stored procedure
                command.CommandType = CommandType.StoredProcedure;

                //Get the values of the Business Entity into the command's parameter list
                f(command, vo);

                //Now run the command. 
                command.ExecuteNonQuery();
            }
            finally
            {
                if (_txn == null)
                {
                    //OracleConnection.ClearPool(conn);
                    conn.Close();
                    conn.Dispose();
                }
            }

        }

        /// <summary>
        /// Saves the contents of a BusinessEntity to the database
        /// </summary>
        /// <param name="SQLText">The SQL statement to execute. This is presumed to be a stored procedure that
        /// either inserts or updates data
        /// </param>
        /// <param name="f">Reference to a delegate that knows how to take a given Business Entity
        /// and populate the parameters of an OracleCommand object from that Entity</param>
        /// <param name="vo">The NamedBusinessEntity (or a child of NamedBusinessEntity) whose values are to be saved
        /// in the database</param>
        protected int SaveBusinessEntityReturnInt(string SQLText, BusinessEntityToCommandDelegate f, BusinessEntity vo)
        {
            int returnValue = -1;

            OracleConnection conn = GetOpenConnection();
            try
            {
                OracleCommand command = conn.CreateCommand();
                command.CommandText = SQLText;
                //Assume we're always calling a stored procedure
                command.CommandType = CommandType.StoredProcedure;

                //Get the values of the Business Entity into the command's parameter list
                f(command, vo);

                //Now run the command. 
                command.ExecuteNonQuery();

                foreach (OracleParameter p in command.Parameters)
                {
                    if (p.Direction == ParameterDirection.ReturnValue)
                    {
                        returnValue = (int)p.Value;
                    }
                }


            }
            finally
            {
                if (_txn == null)
                {
                    //OracleConnection.ClearPool(conn);
                    conn.Close();
                    conn.Dispose();
                }
            }

            return returnValue;

        }

        /// <summary>
        /// Builds an OracleParameter object from the given information
        /// and assumes the parameter is an input parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="oraType"></param>
        /// <returns></returns>
        protected OracleParameter GetParameter(string paramName, object paramValue, OracleDbType oraType)
        {
            return GetParameter(paramName, paramValue, oraType, ParameterDirection.Input);
        }

        /// <summary>
        /// Builds an OracleParameter object from the given information
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="oraType"></param>
        /// <param name="paramDirection"></param>
        /// <returns></returns>
        protected OracleParameter GetParameter(string paramName, object paramValue, OracleDbType oraType, ParameterDirection paramDirection)
        {
            var param = new OracleParameter(paramName, oraType)
            {
                Direction = paramDirection,
                Value = paramValue
            };
            return param;
        }

        /// <summary>
        /// Runs the given query and assumes that the only OUT parameter
        /// is an integer ID value, which is returned from the call.  
        /// Primarily used for inserting a new row and returning that
        /// row's ID value.
        /// 
        /// NOTE: As written this method assumes too much and has no
        /// safeguards to protect from improper use.
        /// </summary>
        /// <param name="SQLText"></param>
        /// <param name="f"></param>
        /// <param name="vo"></param>
        /// <returns></returns>
        protected int SaveBusinessEntityOutputInt(string SQLText, BusinessEntityToCommandDelegate f, BusinessEntity vo)
        {
            int returnValue = -1;

            OracleConnection conn = GetOpenConnection();
            try
            {
                OracleCommand command = conn.CreateCommand();
                command.CommandText = SQLText;
                //Assume we're always calling a stored procedure
                command.CommandType = CommandType.StoredProcedure;

                //Get the values of the Business Entity into the command's parameter list
                f(command, vo);

                //Now run the command. 
                command.ExecuteNonQuery();

                foreach (OracleParameter p in command.Parameters)
                {
                    if (p.Direction == ParameterDirection.Output)
                    {
                        returnValue = (int)p.Value;
                    }
                }


            }

            finally
            {
                if (_txn == null)
                {
                    //OracleConnection.ClearPool(conn);
                    conn.Close();
                    conn.Dispose();
                }
            }

            return returnValue;

        }

        /// <summary>
        /// This method is a wrapper around the ExecuteNonQuery() method on the ADO Command object. Use
        /// it when executing a stored procedure that deletes data. Or whenever you want to execute a 
        /// stored procedure that does not return data, and you simply want to pass in a prebuilt parameter
        /// list (rather than a BusinessEntity)
        /// </summary>
        /// <param name="SQLText">The SQL statement to execute. This is presumed to be a stored procedure 
        /// </param>
        /// <param name="deleteKey">Contains the "key" for the delete (i.e. the arguments to the
        /// given stored procedure)</param>        
        /// <returns>If the stored procedure returned an ID value, then this value is
        /// returned. Otherwise, returns null</returns>
        protected int? ExecuteNonQuery(string SQLText, List<OracleParameter> paramList)
        {
            int? returnValue = null;
            OracleConnection conn = GetOpenConnection();
            try
            {
                OracleCommand command = conn.CreateCommand();
                command.CommandText = SQLText;
                //Assume we're always calling a stored procedure
                command.CommandType = CommandType.StoredProcedure;
                foreach (OracleParameter p in paramList)
                {
                    command.Parameters.Add(p);
                }
                //Now run the command. 
                command.ExecuteNonQuery();

                foreach (OracleParameter p in command.Parameters)
                {
                    if (p.Direction == ParameterDirection.ReturnValue)
                    {
                        try
                        {
                            returnValue = (int)p.Value;
                        }
                        catch
                        {
                            throw new Exception("Got a return value from a stored proc that wasn't an int. This is unexpected. You may need to go in and modify this method. Value returned [" + p.Value + "]");
                        }
                    }
                }
            }
            finally
            {
                if (_txn == null)
                {
                    //OracleConnection.ClearPool(conn);
                    conn.Close();
                    conn.Dispose();
                }

            }

            return returnValue;

        }

        /// <summary>
        /// Reads a single numeric value for the given query. 
        /// Assumes a size of 1.
        /// </summary>
        /// <param name="SQLText"></param>
        /// <param name="p"></param>
        /// <param name="anOracleType"></param>
        /// <returns></returns>
        protected Object ReadScalarValue(String SQLText, List<OracleParameter> p, OracleDbType anOracleType)
        {
            return ReadScalarValue(SQLText, p, anOracleType, 1);
        }

        /// <summary>
        /// Reads a single numeric value (e.g. a select count(*))
        /// </summary>
        /// <param name="SQLText">The SQL statement to execute. This is presumed to be a stored procedure returning a numeric value</param>
        /// <param name="p">A list of <see cref="OracleParameter">OracleParameters</see>. These are parameters required by the SQL
        /// statement. If the SQL statement requires no parameters, then this must be an empty list.</param>
        /// <returns>The scalar OracleDecimal returned by the stored procedure</returns>
        protected Object ReadScalarValue(String SQLText, List<OracleParameter> p, OracleDbType anOracleType, int size)
        {
            OracleConnection conn = GetOpenConnection();
            try
            {
                OracleCommand command = conn.CreateCommand();
                command.CommandText = SQLText;
                //Assume we're always calling a stored procedure
                command.CommandType = CommandType.StoredProcedure;
                //We assume that this is a function that returns either a decimal or string
                //value.  Make the return value the first in the parameter list, 
                //then add the ones supplied by the caller.
                command.Parameters.Add(new OracleParameter("retvalue", anOracleType, size, "", ParameterDirection.ReturnValue));
                foreach (OracleParameter p1 in p)
                {
                    command.Parameters.Add(p1);
                }

                command.ExecuteNonQuery();

                //Return the return value from the stored function call in
                //the appropriate type of object.
                if (anOracleType == OracleDbType.Decimal)
                {
                    if (!string.IsNullOrEmpty(command.Parameters["retvalue"].Value.ToString()))
                        return (OracleDecimal)command.Parameters["retvalue"].Value;
                    else
                        return (OracleDecimal)("0".ToString());  //Temporary fix. Needs to be removed
                }
                else
                {
                    if (!string.IsNullOrEmpty(command.Parameters["retvalue"].Value.ToString()))
                        return (OracleString)command.Parameters["retvalue"].Value;
                    else
                        return (OracleString)(string.Empty);
                }
            }
            finally
            {
                if (_txn == null)
                {
                    //OracleConnection.ClearPool(conn);
                    conn.Close();
                    conn.Dispose();
                }
            }

        }

        /// <summary>
        /// Utilty used to set up the OracleParameterStatus for the associative arrays. Creates
        /// an array of OracleParamterStatus enums set to OracleParameterStatus.Success
        /// </summary>
        /// <param name="length">The size of the OracleParameterStatus array</param>
        /// <returns>An array of OracleParameterStatus values. Size of the array is equal to the length parameter</returns>
        protected OracleParameterStatus[] SetArrayBindStatus(int length)
        {
            List<OracleParameterStatus> l = new List<OracleParameterStatus>();
            for (int i = 0; i < length; i++)
                l.Add(OracleParameterStatus.Success);
            return l.ToArray();
        }

        /// <summary>
        /// Function to convert an OracleDecimal to an C# int. If the OracleDecimal
        /// can't be converted to an integer, this will throw and exception and write to the //Log.
        /// </summary>
        /// <param name="d">An OracleDecimal</param>
        /// <returns>The OracleDecimal converted to an int</returns>
        /// <exception cref="BESSException">If the underlying value is not an integer</exception>
        /// <exception cref="ArgumentNullException">If the underlying value is not an integer</exception>
        protected int OracleDecimalToInt(OracleDecimal oDecimal)
        {

            if (oDecimal.IsNull)
            {
                throw new ArgumentNullException(MSG_NULL_PARAMETER);
            }
            if (!oDecimal.IsInt)
            {
                //this should NEVER happen! If it does, let the folks know about it.
                throw new Exception("Value from database was not an int as expected. Value is [" + oDecimal + "]");
            }
            return oDecimal.ToInt32();
        }

        /// <summary>
        /// This method returns a .NET string value from an
        /// OracleString object.  Throws an error if the
        /// OracleString is null.
        /// </summary>
        /// <param name="oString"></param>
        /// <returns></returns>
        protected string OracleStringToString(OracleString oString)
        {

            if (oString.IsNull)
            {
                throw new ArgumentNullException(MSG_NULL_PARAMETER);
            }

            return oString.ToString();
        }

        /// <summary>
        /// Convert oracle decimal to 0 if null value is returned.
        /// This function if for the condition where null value is
        /// not an error.
        /// </summary>
        /// <param name="oDecimal"></param>
        /// <returns></returns>
        protected int OracleDecimalToInt32(object oDecimal)
        {
            if (oDecimal == System.DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(oDecimal);
        }

        /// <summary>
        /// Returns an empty string value if the given
        /// Oracle object is null.  Otherwise, returns
        /// the object converted to a string
        /// </summary>
        /// <param name="oraObject"></param>
        /// <returns></returns>
        protected string OracleNullToString(object oraObject)
        {
            if (oraObject == System.DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(oraObject);
        }

        /// <summary>
        /// Converts a boolean value that's stored in the database as a char(1) column
        /// to a .NET bool. 'Y' is true. Anything else (including null) is false. Case is ignored.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected bool OracleCharToBool(OracleString os)
        {
            if (os.IsNull)
            {
                return false;
            }
            String s = os.ToString().Trim();
            return (String.Compare(s, "Y", true) == 0);
        }

        /// <summary>
        /// Utility routine that will return DBNull.Value if the given double is NaN,
        /// otherwise it will return the given double.
        /// </summary>
        /// <param name="value">A double</param>
        /// <returns>If the double is equal to NaN, returns DBNull.Value. Otherwise, returns
        /// the double</returns>
        protected Object ConvertDouble(double value)
        {
            return (Double.IsNaN(value) ? DBNull.Value : (Object)value);
        }

        /// <summary>
        /// Takes an int value and converts it to a DBNull value
        /// if necessary, otherwise returns the int as an Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Object ConvertInt(int value)
        {
            return (value == Int32.MinValue ? DBNull.Value : (Object)value);
        }

        /// <summary>
        /// Takes a NamedBusinessEntity object and returns a DBNull value
        /// if necessary, otherwise returns the ID as an Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Object ConvertNamedBusinessEntity(NamedBusinessEntity value)
        {
            return (value.IsNull ? DBNull.Value : (Object)value.ID);
        }

        /// <summary>
        /// Takes a DateTime and converts it to a DBNull if necessary,
        /// othewise it returns an OracleDate object for the DateTime object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Object ConvertDate(DateTime value)
        {
            return (value == null || value == DateTime.MinValue ? DBNull.Value : (Object)new OracleDate(value));
        }

        /// <summary>
        /// Converts a string value to and OracleString value.
        /// Returns a DBNull value if necessary.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Object ConvertString(string value)
        {
            return (value != null && value.Length > 0 ? (Object)new OracleString(value) : DBNull.Value);
        }

        /// <summary>
        /// Convets a bool value to a OracleString value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected Object ConvertBoolean(bool value)
        {
            return (Object)new OracleString(value ? "Y" : "N");
        }

        /// <summary>
        /// Returns the string value from the reader that is in the
        /// column with the given name.  Makes sure the column exists
        /// and that it is not null before reading the string.  If the
        /// column name does not exist, it throws an error.  If the value
        /// is null, it returns an empty string.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected string GetString(OracleDataReader reader, string columnName)
        {
            return GetString(reader, columnName, true);
        }

        /// <summary>
        /// Returns the string value from the reader that is in the 
        /// column with the given name.  Makes sure that the column exists
        /// and that it is not null before reading the string.  If the 
        /// name does not exist in the column list and it is required, an error
        /// is thrown.  If the column does not exist and it is not required
        /// or the value is null, this method returns an empty string.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected string GetString(OracleDataReader reader, string columnName, bool required)
        {
            string returnValue = string.Empty;
            int columnNumber = -1;

            try
            {
                columnNumber = reader.GetOrdinal(columnName);

                if (columnNumber >= 0 && !reader.IsDBNull(columnNumber))
                {
                    returnValue = reader.GetString(columnNumber).Trim();
                }
            }

            catch (IndexOutOfRangeException e)
            {
                if (required)
                    throw new ColumnNotFoundException(columnName);
            }

            return returnValue;
        }

        /// <summary>
        /// Returns a boolean value from the reader for the given column
        /// name.  If the column does not exist or the value returned is a
        /// null, it returns a false value.  If the value is 'Y' (comparison is 
        /// not case-sensitive) in the database, it returns true, 
        /// otherwise it returns false.  
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected bool GetBool(OracleDataReader reader, string columnName)
        {
            return GetBool(reader, columnName, false);
        }

        /// <summary>
        /// Returns a boolean value from the reader for the given column
        /// name.  If the column does not exist or the value returned is a
        /// null, it returns the given default value.  If the value is 'Y' 
        /// (comparison is not case-sensitive) in the database, it returns true, 
        /// otherwise it returns false.  
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected bool GetBool(OracleDataReader reader, string columnName, bool defaultValue)
        {
            bool returnValue = defaultValue;

            string charValue = GetString(reader, columnName);

            if (charValue != null)
            {
                returnValue = OracleCharToBool(charValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Returns the int value associated with the given column name in the row.
        /// Does error checking for column name existence (throws error) or a null
        /// value (returns a default value).  The default value here is the 
        /// BusinessEntity.EMPTY_ID value.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected int GetInt(OracleDataReader reader, string columnName)
        {
            return GetInt(reader, columnName, -1);
        }

        /// <summary>
        /// Returns the int value associated with the given column name in the row.
        /// assumes that the column is required and throws an error if the reader
        /// does not find the column. If the column value is null, returns the
        /// default value.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected int GetInt(OracleDataReader reader, string columnName, int defaultValue)
        {
            return GetInt(reader, columnName, defaultValue, true);
        }

        /// <summary>
        /// Returns the int value associated with the given column name in the row.
        /// If the column is required, it throws an error.  If the column is not required 
        /// and is not found or contains a null value it returns the given default value.  
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected int GetInt(OracleDataReader reader, string columnName, int defaultValue, bool required)
        {
            int returnValue = defaultValue;
            int columnNumber = -1;

            try
            {
                columnNumber = reader.GetOrdinal(columnName);

                if (columnNumber >= 0 && !reader.IsDBNull(columnNumber))
                {
                    returnValue = OracleDecimalToInt(reader.GetDecimal(columnNumber));
                }
            }

            catch (IndexOutOfRangeException e)
            {
                if (required)
                    throw new ColumnNotFoundException(columnName);
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the date value for the row associated with the given column name.
        /// Error checks for the column existence and that it is a valid date value 
        /// (throws an error in these cases). If the value is null it returns a 
        /// default value of DateTime.MinValue.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected DateTime GetDateTime(OracleDataReader reader, string columnName)
        {
            DateTime returnValue = DateTime.MinValue;
            int columnNumber = -1;

            try
            {
                columnNumber = reader.GetOrdinal(columnName);

                if (!reader.IsDBNull(columnNumber))
                {
                    returnValue = reader.GetDateTime(columnNumber);
                }
            }

            catch (IndexOutOfRangeException e)
            {
                throw new ColumnNotFoundException(columnName);
            }

            return returnValue;
        }

        /// <summary>
        /// Returns the double value associated with the given column name in the row.
        /// Does error checking for column name existence (throws error) or a null
        /// value (returns the given default value).  
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected double GetDouble(OracleDataReader reader, string columnName, double defaultValue)
        {
            double returnValue = defaultValue;
            int columnNumber = -1;

            try
            {
                columnNumber = reader.GetOrdinal(columnName);

                if (columnNumber >= 0 && !reader.IsDBNull(columnNumber))
                {
                    returnValue = (double)reader.GetDecimal(columnNumber);
                }
            }

            catch (IndexOutOfRangeException e)
            {
                throw new ColumnNotFoundException(columnName);
            }

            return returnValue;
        }

        #endregion
    }

    #region User Defined Exception Classes

    /// <summary>
    /// This exception is defined to add the column name to the
    /// regular exception so we know which column caused an
    /// IndexOutOfRangeException to be raised when reading a
    /// column's data from an Oracle reader.
    /// </summary>
    public class ColumnNotFoundException : Exception
    {
        public ColumnNotFoundException(string columnName)
            : base(string.Format(
                "Could not find column \"{0}\" in the result set for a query",
                columnName))
        {
        }

    }

    #endregion
}
