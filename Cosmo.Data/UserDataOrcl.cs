using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Cosmo.Entity;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Cosmo.Data
{
    public class UserDataOrcl : BusinessEntityBaseDAO,IUserData
    {
        #region Constant variables
        private const string PackageName = "COSMO_USER_PACKAGE.";
        private const string EnvironmentPackageName = "COSMO_ENVIRONMENT_PACKAGE.";
        private const string SetupPackageName = "COSMO_SETUP_PACKAGE.";

        private static readonly string SetUser = string.Format("{0}SP_CWT_InsUpdUser", PackageName);
        private static readonly string GetUser = string.Format("{0}FN_CWT_GetUsers", PackageName);
        
        private static readonly string LogUser = string.Format("{0}FN_CWT_GetLogin",PackageName);
        private static readonly string DelUser = string.Format("{0}SP_CWT_DeleteRecord", EnvironmentPackageName);
        private static readonly string GetUserbyemail = string.Format("{0}FN_CWT_GetUserByEmail", PackageName);
        private static readonly string GetUserrole = string.Format("{0}FN_CWT_GetUserRole", PackageName);
        private static readonly string Get_RoleMenu = string.Format("{0}FN_CWT_GetMenuItems", SetupPackageName);
        private static readonly string SetUserprofile = string.Format("{0}SP_CWT_UpdateUserPassword", PackageName);


        #endregion Constant variables


        #region Insert User into database

        public string InsertUpdateUser(UserEntity user)
        {
            int recInsert = 0;
            var pList = new List<OracleParameter>
            {
                GetParameter("p_USERID", user.UserID, OracleDbType.Int32, ParameterDirection.Input),
                GetParameter("p_USERFIRSTNAME", user.FirstName, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_USERLASTNAME", user.LastName, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_USEREMAIL", user.Email, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_USERPASSWORD", user.Password, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_USERROLES", user.Roles, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_ISACTIVE", user.IsActive? 1 : 0 , OracleDbType.Int32, ParameterDirection.Input),
                new OracleParameter("p_SCOPE_OUTPUT", OracleDbType.Int32, ParameterDirection.Output)
            };
            ExecuteNonQuery(SetUser, pList);

            foreach (var oracleParameter in pList.Where(oracleParameter => oracleParameter.ParameterName == "p_SCOPE_OUTPUT"))
            {
                recInsert = OracleDecimalToInt((OracleDecimal)oracleParameter.Value);
            }
            return recInsert.ToString();
        }

        #endregion Insert User into database

        #region Update User profile database

        public string UpdateUserPassword(UserEntity user, string newPassword)
        {
            int recInsert = 0;
            var pList = new List<OracleParameter>
            {
                GetParameter("p_USERID", user.UserID, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_USEREMAIL", user.Email, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_CURRENTPASSWORD", user.Password, OracleDbType.Varchar2, ParameterDirection.Input),
                GetParameter("p_NEWPASSWORD", newPassword, OracleDbType.Varchar2, ParameterDirection.Input),
                new OracleParameter("p_SCOPE_OUTPUT", OracleDbType.Int32, ParameterDirection.Output)
            };
            ExecuteNonQuery(SetUserprofile, pList);

            foreach (var oracleParameter in pList.Where(oracleParameter => oracleParameter.ParameterName == "p_SCOPE_OUTPUT"))
            {
                recInsert = OracleDecimalToInt((OracleDecimal)oracleParameter.Value);
            }
            return recInsert.ToString();
        }

        #endregion Update User profile database


        #region Get User

        public List<UserEntity> GetUsers(int userid)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_USERID", userid, OracleDbType.Int32, ParameterDirection.Input)
            };

            var userEntity = ReadCompoundEntityList<UserEntity>(GetUser, pList, RowToUserList);
            return userEntity;
        }

        public List<UserEntity> GetUsers(string useremail, string password)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_USERID", useremail, OracleDbType.Varchar2, ParameterDirection.Input)
            };

            var userEntity = ReadCompoundEntityList<UserEntity>(GetUser, pList, RowToUserList);
            return userEntity;
        }

        private List<UserEntity> RowToUserList(OracleDataReader reader)
        {
            var list = new List<UserEntity>();

            while (reader.Read())
            {
                var entuty = new UserEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_ID"))
                    entuty.UserID = Convert.ToInt32(reader["USER_ID"]);
                else
                    entuty.UserID = 0;
                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_FIRST_NAME"))
                    entuty.FirstName = Convert.ToString(reader["USER_FIRST_NAME"]);
                else
                    entuty.FirstName = string.Empty;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_LAST_NAME"))
                    entuty.LastName = Convert.ToString(reader["USER_LAST_NAME"]);
                else
                    entuty.LastName = string.Empty;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_EMAIL_ADDRESS"))
                    entuty.Email = Convert.ToString(reader["USER_EMAIL_ADDRESS"]);
                else
                    entuty.Email = string.Empty;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_PASSWORD"))
                    entuty.Password = Convert.ToString(reader["USER_PASSWORD"]);
                else
                    entuty.Password = string.Empty;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_IS_ACTIVE"))
                    entuty.IsActive = Convert.ToBoolean(reader["USER_IS_ACTIVE"]);
                else
                    entuty.IsActive = false;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_IS_DELETED"))
                    entuty.IsDeleted = Convert.ToBoolean(reader["USER_IS_DELETED"]);
                else
                    entuty.IsDeleted = false;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_CREATED_DATE"))
                    entuty.CreatedDate = Convert.ToDateTime(reader["USER_CREATED_DATE"]);
                else
                    entuty.CreatedDate = null;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_UPDATED_DATE"))
                    entuty.UpdatedDate = Convert.ToDateTime(reader["USER_UPDATED_DATE"]);
                else
                    entuty.UpdatedDate = null;

                list.Add(entuty);
            }

            return list;
        }


        public List<UserRoleEntity> GetUserRoles(int userid)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_USERID", userid, OracleDbType.Int32, ParameterDirection.Input)
            };

            var userEntity = ReadCompoundEntityList<UserRoleEntity>(GetUserrole, pList, RowToUserRoleList);
            return userEntity;
        }

        private List<UserRoleEntity> RowToUserRoleList(OracleDataReader reader)
        {
            var list = new List<UserRoleEntity>();

            while (reader.Read())
            {
                var entuty = new UserRoleEntity();

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_ID"))
                    entuty.UserID = Convert.ToInt32(reader["USER_ID"]);
                else
                    entuty.UserID = 0;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "USER_ROLE_ID"))
                    entuty.UserRoleID = Convert.ToInt32(reader["USER_ROLE_ID"]);
                else
                    entuty.UserRoleID = 0;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_ID"))
                    entuty.RoleID = Convert.ToInt32(reader["ROLE_ID"]);
                else
                    entuty.RoleID = 0;

                if (CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_NAME"))
                    entuty.RoleName = Convert.ToString(reader["ROLE_NAME"]);
                else
                    entuty.RoleName = string.Empty;

                list.Add(entuty);
            }

            return list;
        }

        public int LoginUsers(string userid, string password)
        {
            int loginStatus = 0;
            var pList = new List<OracleParameter>();
            pList.Add(GetParameter("p_USERID", userid, OracleDbType.Varchar2, ParameterDirection.Input));
            pList.Add(GetParameter("p_PASSWORD", password, OracleDbType.Varchar2, ParameterDirection.Input));

            loginStatus = OracleDecimalToInt((OracleDecimal) ReadScalarValue(LogUser, pList, OracleDbType.Decimal));
            return loginStatus;
        }


        #endregion Get User

        #region delete user

        public int DeleteUser(int userid)
        {
            int status = 0;
            try
            {
                var pList = new List<OracleParameter>
                {
                    GetParameter("p_sID", userid, OracleDbType.Int32, ParameterDirection.Input),
                    GetParameter("p_sType", "user", OracleDbType.Varchar2, ParameterDirection.Input),
                };

                ExecuteNonQuery(DelUser, pList);
                status = 1;
            }
            catch (Exception)
            {
                status = -1;
            }
            return status;
        }

        #endregion delete user
    
        public List<RoleMenuEntity> GetRoleMenu(string roles)
        {
            var pList = new List<OracleParameter>
            {
                GetParameter("p_ROLEID", roles, OracleDbType.Varchar2, ParameterDirection.Input)
            };

            var userEntity = ReadCompoundEntityList<RoleMenuEntity>(Get_RoleMenu, pList, RowToRoleMenuList);
            return userEntity;
        }

        private static List<RoleMenuEntity> RowToRoleMenuList(OracleDataReader reader)
        {
            var list = new List<RoleMenuEntity>();

            while (reader.Read())
            {
                var entity = new RoleMenuEntity
                {
                    RoleMenuID = CommonUtility.IsColumnExistsAndNotNull(reader, "RM_ID")
                        ? Convert.ToInt32(reader["RM_ID"])
                        : 0,
                    RoleID = CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_ID")
                        ? Convert.ToInt32(reader["ROLE_ID"])
                        : 0,
                    MenuID = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_ID")
                        ? Convert.ToInt32(reader["MENU_ID"])
                        : 0,
                    RoleName = CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_NAME")
                        ? Convert.ToString(reader["ROLE_NAME"])
                        : string.Empty,
                    RoleType = CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_TYPE")
                        ? Convert.ToString(reader["ROLE_TYPE"])
                        : string.Empty,
                    MainMenu = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_MAIN")
                        ? Convert.ToString(reader["MENU_MAIN"])
                        : string.Empty,
                    SubMenu = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_SUB")
                        ? Convert.ToString(reader["MENU_SUB"])
                        : string.Empty,
                    MenuPath = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_PATH")
                        ? Convert.ToString(reader["MENU_PATH"])
                        : string.Empty,
                    MenuIsPopup = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_ISPOPUP") &&
                                  Convert.ToBoolean(reader["MENU_ISPOPUP"])
                };

                list.Add(entity);
            }

            return list;
        }
    }
}
