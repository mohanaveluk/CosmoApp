using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmo.Entity
{
    public class RoleMenuEntity: TRW.NamedBusinessCode, IFill
    {
        #region property declaration

        public int RoleMenuID { get; set; }
        public int RoleID { get; set; }
        public int MenuID { get; set; }
        public string RoleName { get; set; }
        public string RoleType { get; set; }
        public string MainMenu { get; set; }
        public string SubMenu { get; set; }
        public string MenuPath { get; set; }
        public bool MenuIsPopup { get; set; }
        
        #endregion property declaration

        #region IFill Logging of job details


        public void Fill(System.Data.SqlClient.SqlDataReader reader)
        {
            this.RoleMenuID = CommonUtility.IsColumnExistsAndNotNull(reader, "RM_ID")
                ? Convert.ToInt32(reader["RM_ID"])
                : 0;

            this.RoleID = CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_ID")
                ? Convert.ToInt32(reader["ROLE_ID"])
                : 0;

            this.MenuID = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_ID")
                ? Convert.ToInt32(reader["MENU_ID"])
                : 0;

            this.RoleName = CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_NAME")
                ? Convert.ToString(reader["ROLE_NAME"])
                : string.Empty;

            this.RoleType = CommonUtility.IsColumnExistsAndNotNull(reader, "ROLE_TYPE")
                ? Convert.ToString(reader["ROLE_TYPE"])
                : string.Empty;

            this.MainMenu = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_MAIN")
                ? Convert.ToString(reader["MENU_MAIN"])
                : string.Empty;

            this.SubMenu = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_SUB")
                ? Convert.ToString(reader["MENU_SUB"])
                : string.Empty;

            this.MenuPath = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_PATH")
                ? Convert.ToString(reader["MENU_PATH"])
                : string.Empty;

            this.MenuIsPopup = CommonUtility.IsColumnExistsAndNotNull(reader, "MENU_ISPOPUP") &&
                               Convert.ToBoolean(reader["MENU_ISPOPUP"]);



        }

        #endregion IFill Logging of job details
    }
}
