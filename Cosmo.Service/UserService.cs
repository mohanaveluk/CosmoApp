using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using Cosmo.Data;
using Cosmo.Entity;

namespace Cosmo.Service
{
    public class UserService
    {
        private static readonly string DbType = ConfigurationManager.AppSettings["DatabaseType"];
        private readonly static string RememberMeExpiry = ConfigurationManager.AppSettings["RememberMeExpires"] ?? "30";

        IUserData userData;

        public UserService()
        {
            var iDbType = DbType == DatabaseType.Oracle.ToString()
                ? Convert.ToInt32(DatabaseType.Oracle).ToString()
                : Convert.ToInt32(DatabaseType.SqlServer).ToString();

            userData = new UserDataFactory().Create(iDbType);
        }

        #region Insert user

        public string InsertUpdateUser(UserEntity user)
        {
            try
            {
                return userData.InsertUpdateUser(user);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw ex;
            }
        }

        #endregion Insert user

        #region Update User profile

        public string UpdateUserPassword(UserEntity user, string newPassword)
        {
            try
            {
                return userData.UpdateUserPassword(user, newPassword);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                throw ex;
            }
        }

        #endregion Update User profile
        
        #region Get User
        public List<UserEntity> GetUsers(int userid)
        {
            List<UserEntity> users = new List<UserEntity>();
            List<UserRoleEntity> userRoles = new List<UserRoleEntity>();

            users = userData.GetUsers(userid);
            userRoles = userData.GetUserRoles(userid);
            foreach (UserEntity user in users)
            {
                user.Password = CommonUtility.DecryptString(user.Password);
                user.UserRoles = new List<UserRoleEntity>();
                if (userRoles.Any(ur => ur.UserID == user.UserID))
                {
                    user.UserRoles = userRoles.Where(ur => ur.UserID == user.UserID).ToList<UserRoleEntity>();
                }
            }

            return users;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int LoginUsers(string userid, string password)
        {
            int loginStatus = userData.LoginUsers(userid, CommonUtility.EnryptString( password));

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
            try
            {
                status = userData.DeleteUser(userid);
            }
            catch (Exception ex)
            {
                status = -1;
                Logger.Log(ex.ToString());
            }
            return status;
        }
        
        #endregion delete user

        #region RoleMenuEntity
        /// <summary>
        /// Get Menu based on the assigned roles for the user
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public List<RoleMenuEntity> GetRoleMenu(string roles)
        {
            List<RoleMenuEntity> tempRoleMenu = new List<RoleMenuEntity>();
            List<RoleMenuEntity> roleMenu = new List<RoleMenuEntity>();
            roleMenu = userData.GetRoleMenu(roles);
            if (roleMenu != null && roleMenu.Count > 0)
            {
                foreach (RoleMenuEntity menu in roleMenu)
                {
                    if(!tempRoleMenu.Any(rm => rm.MenuID == menu.MenuID))
                    {
                        tempRoleMenu.Add(menu);
                    }
                }
            }

            return tempRoleMenu; // roleMenu;
        }
        #endregion RoleMenuEntity

        #region RememberMe

        public void SetRememberMe(string userId, string password, bool rememberMe)
        {
            HttpContext.Current.Response.Cookies.Clear();
            
            if (!rememberMe) return;


            var expiryDate = !string.IsNullOrEmpty(RememberMeExpiry)
                ? DateTime.Now.AddDays(Convert.ToInt32(RememberMeExpiry))
                : DateTime.Now.AddDays(30);

            var ticket = new FormsAuthenticationTicket(2, userId, DateTime.Now, expiryDate, true, password);
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.Expires = ticket.Expiration;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        #endregion RememberMe

    }
}
