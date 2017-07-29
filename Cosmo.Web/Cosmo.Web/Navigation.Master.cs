using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cosmo.Entity;
using Cosmo.Service;

namespace Cosmo.Web
{
    public partial class Navigation : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonService.ValidateUser();

            if (Session["LicenseStatus"] == null)
            {
                var commonService = new CommonService();
                var licenseStatus = commonService.GetUserAccess();
                Session["LicenseStatus"] = licenseStatus;
            }

            if (Session["LicenseStatus"] != null)
            {
                var licenseStatus = (LicenseStatus)Session["LicenseStatus"];
            }
            else
            {
                Session["LicenseStatus"] = new LicenseStatus { ExpiryInDays = 30, Message = "TRIAL VERSION EXPIRES IN <font color='#FF1352'> 30 </font> DAYS", Status = "Failure", Type = "Trial" };
            }

            List<RoleMenuEntity> rolewiseMenu = new List<RoleMenuEntity>();
            List<string> mainMenu = new List<string>();
            StringBuilder menuBuilder = new StringBuilder();

            if (HttpContext.Current.Session["ROLEMENU"] != null)
            {
                //populate menu
                rolewiseMenu = (List<RoleMenuEntity>)HttpContext.Current.Session["ROLEMENU"];
                if (rolewiseMenu != null && rolewiseMenu.Count > 0)
                {
                    Dictionary<string, string> menuIcon = new Dictionary<string, string>();
                    menuIcon.Add("Home", "icon icon108");
                    menuIcon.Add("Dashboard", "glyphicon glyphicon-stats");
                    menuIcon.Add("Setup", "glyphicon glyphicon-edit");
                    menuIcon.Add("Action", "glyphicon glyphicon-cog");
                    menuIcon.Add("Reports", "glyphicon glyphicon-list-alt");
                    mainMenu = rolewiseMenu.Select(mm => mm.MainMenu).Distinct().ToList<string>();

                    var highlightMenu="";
                    var singleOrDefault = rolewiseMenu.SingleOrDefault(sub => Request.Path.Contains(sub.MenuPath));

                    if (singleOrDefault != null)
                    {
                        highlightMenu = singleOrDefault.MainMenu;
                    }
                    foreach (string mm in mainMenu)
                    {
                        if (rolewiseMenu.Count > 0 && rolewiseMenu.Count(sub => sub.MainMenu == mm) <= 1)
                        {
                            var filePath = rolewiseMenu.Find(sub => sub.MainMenu == mm).MenuPath;
                            menuBuilder.Append(highlightMenu == mm ? "<li class=\"active\">" : "<li>");

                            menuBuilder.Append("<a href=\"" + filePath + "\"><span class=\"" +
                                               menuIcon[mm] + "\"></span> " + mm + " </a></li>");

                            continue;
                        }

                        var mailMenuList = rolewiseMenu.Where(rm => rm.MainMenu == mm).ToList();
                        var firstOrDefault = mailMenuList.FirstOrDefault(rm => rm.MainMenu == mm);
                        var lastOrDefault = mailMenuList.LastOrDefault(rm => rm.MainMenu == mm);
                        if (lastOrDefault != null && lastOrDefault.MenuPath == "-")
                        {
                            mailMenuList.RemoveAt(mailMenuList.Count - 1);
                        }
                        if (firstOrDefault != null && firstOrDefault.MenuPath == "-")
                        {
                            mailMenuList.RemoveAt(0);
                        }

                        menuBuilder.Append(highlightMenu == mm
                            ? "<li class=\"active dropdown\">"
                            : "<li class=\"dropdown\">");
                        menuBuilder.Append(
                            "<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" data-hover=\"dropdown\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"false\"><span class=\"" +
                            menuIcon[mm] + "\"></span> " + mm + "<span class=\"caret\"></span></a>");

                        menuBuilder.Append("		<ul class=\"dropdown-menu\">");
                        foreach (RoleMenuEntity submenu in mailMenuList)
                        {
                            if (submenu.MainMenu == mm)
                            {
                                if (submenu.MenuPath == "-")
                                {
                                    menuBuilder.Append("<li role=\"separator\" class=\"divider\"></li>");
                                    continue;
                                }

                                if (submenu.MenuIsPopup)
                                {
                                    if (submenu.MenuPath.Contains(".aspx"))
                                    {
                                        menuBuilder.Append("<li><a href=\"#\" onclick=\"OpenNewWindow('" +
                                                           submenu.MenuPath +
                                                           "')\" tablindex=\"-1\">" + submenu.SubMenu + "</a></li>");
                                    }
                                    else
                                    {
                                        menuBuilder.Append(
                                            "<li><a href=\"#\" class=\"head-right-link\" data-toggle=\"modal\" data-target=\"#" +
                                            submenu.MenuPath +
                                            "\"> "+ submenu.SubMenu +"</a></li>");
                                    }
                                }
                                else
                                    menuBuilder.Append("<li><a href=\"" + submenu.MenuPath + "\" tablindex=\"-1\">" +
                                                       submenu.SubMenu + "</a></li>");


                            }
                        }
                        menuBuilder.Append("		</ul>");
                        menuBuilder.Append("	</li>");
                    }

                    ulMenuItems.InnerHtml = menuBuilder.ToString();
                }
                //Populate user details
                //if (HttpContext.Current.Session["_LOGGED_USERNAME"] != null)
                //    LoggedUserName.InnerText = Convert.ToString(HttpContext.Current.Session["_LOGGED_USERNAME"]);

            }

        }
    }
}