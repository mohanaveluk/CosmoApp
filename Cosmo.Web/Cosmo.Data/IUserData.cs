using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmo.Entity;

namespace Cosmo.Data
{
    public interface IUserDataFactory
    {
        IUserData Create(string dbType);
    }

    public class UserDataFactory : IUserDataFactory
    {
        public IUserData Create(string dbType)
        {
            return dbType == Convert.ToInt32(DatabaseType.Oracle).ToString()
                ? (IUserData)new UserDataOrcl()
                : new UserData(); 
        }
    }

    public interface IUserData
    {
        string InsertUpdateUser(UserEntity user);

        string UpdateUserPassword(UserEntity user, string newPassword);

        List<UserEntity> GetUsers(int userid);
        List<UserEntity> GetUsers(string useremail, string password);
        List<UserRoleEntity> GetUserRoles(int userid);
        int LoginUsers(string userid, string password);

        int DeleteUser(int userid);

        List<RoleMenuEntity> GetRoleMenu(string roles);
    }
}
