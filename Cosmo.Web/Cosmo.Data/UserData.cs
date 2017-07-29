using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using System.Data;
using System.Data.SqlClient;

namespace Cosmo.Data
{
    public class UserData : IUserData
    {
        #region Constant variables
        private const string SET_USER = "CWT_InsUpdUser";
        private const string GET_USER = "CWT_GetUsers";
        private const string GET_USERBYEMAIL = "CWT_GetUserByEmail";
        private const string GET_USERROLE = "CWT_GetUserRole";
        private const string DEL_USER = "CWT_DeleteRecord";
        private const string LOG_USER = "CWT_GetLogin";
        private const string GET_ROLEMENU = "CWT_GetMenuItems";
        private const string SET_USERPROFILE = "CWT_UpdateUserPassword";
        #endregion Constant variables

        #region Insert User into database

        public string InsertUpdateUser(UserEntity user)
        {
            string recInsert = string.Empty;
            try
            {

                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@USERID", user.UserID));
                pList.Add(new SqlParameter("@USERFIRSTNAME", user.FirstName));
                pList.Add(new SqlParameter("@USERLASTNAME", user.LastName));
                pList.Add(new SqlParameter("@USEREMAIL", user.Email));
                pList.Add(new SqlParameter("@USERROLES", user.Roles));
                pList.Add(new SqlParameter("@ISACTIVE", user.IsActive));
                pList.Add(new SqlParameter("@USERPASSWORD", user.Password));

                SqlParameter retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.VarChar, -1);
                retVal.Direction = ParameterDirection.Output;
                pList.Add(retVal);
                //pList.Add(new SqlParameter("@SCOPE_OUTPUT", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output });
                recInsert = Convert.ToString(UtilityDL.ExecuteNonQuery(SET_USER, pList, true));
            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return recInsert;
        }
        
        #endregion Insert User into database

        #region Update User profile database

        public string UpdateUserPassword(UserEntity user, string newPassword)
        {
            string recInsert = string.Empty;
            try
            {
                List<SqlParameter> pList = new List<SqlParameter>();
                pList.Add(new SqlParameter("@USERID", user.UserID));
                pList.Add(new SqlParameter("@USEREMAIL", user.Email));
                pList.Add(new SqlParameter("@CURRENTPASSWORD", user.Password));
                pList.Add(new SqlParameter("@NEWPASSWORD", newPassword));

                SqlParameter retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.VarChar, -1);
                retVal.Direction = ParameterDirection.Output;
                pList.Add(retVal);
                //pList.Add(new SqlParameter("@SCOPE_OUTPUT", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output });
                recInsert = Convert.ToString(UtilityDL.ExecuteNonQuery(SET_USERPROFILE, pList, true));
            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return recInsert;
        }

        #endregion Update User profile database

        #region Get User

        /// <summary>
        /// Get users list
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<UserEntity> GetUsers(int userid)
        {
            var pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@USERID", userid));
                string stProc = GET_USER;
                return UtilityDL.FillData<UserEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserEntity> GetUsers(string useremail, string password)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@USEREMAIL", useremail));
                pList.Add(new SqlParameter("@PASSWORD", password));
                string stProc = GET_USERBYEMAIL;
                return UtilityDL.FillData<UserEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get user rols list
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<UserRoleEntity> GetUserRoles(int userid)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@USERID", userid));
                string stProc = GET_USERROLE;
                return UtilityDL.FillData<UserRoleEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int LoginUsers(string userid, string password)
        {
            //object scope_output;
            int loginStatus = 0;
            List<SqlParameter> pList = new List<SqlParameter>();
            //List<UserEntity> validUuser = new List<UserEntity>();
            try
            {
                pList.Add(new SqlParameter("@USERID", userid));
                pList.Add(new SqlParameter("@PASSWORD", password));
                SqlParameter retVal = new SqlParameter("@SCOPE_OUTPUT", SqlDbType.Int, -1);
                retVal.Direction = ParameterDirection.Output;
                pList.Add(retVal);
                string stProc = LOG_USER;
                //validUuser = UtilityDL.FillData<UserEntity>(stProc, pList, true, out scope_output); 
                loginStatus = Convert.ToInt32(UtilityDL.ExecuteNonQuery(stProc, pList, true));
            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return loginStatus;
        }

        #endregion Get User

        #region delete user
        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int DeleteUser(int userid)
        {
            int status = 0;
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@sID", userid));
                pList.Add(new SqlParameter("@sType", "user"));
                UtilityDL.ExecuteNonQuery(DEL_USER, pList);
                status = 1;
            }
            catch (Exception ex)
            {
                status = -1;
                throw ex;
            }
            return status;
        }
        #endregion delete user

        //RoleMenuEntity
        /// <summary>
        /// Get Menu based on the assigned roles for the user
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public List<RoleMenuEntity> GetRoleMenu(string roles)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            try
            {
                pList.Add(new SqlParameter("@ROLEID", roles));
                string stProc = GET_ROLEMENU;
                return UtilityDL.FillData<RoleMenuEntity>(stProc, pList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
//UtilityDL.ExecuteNonQuery(SET_USER, pList, true)
//Edit link --http://www.intstrings.com/ramivemula/articles/getting-started-with-angularjs-update-delete-operations-with-database-connectivity/