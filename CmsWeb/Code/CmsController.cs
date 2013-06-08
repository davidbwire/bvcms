using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Routing;
using UtilityExtensions;
using CmsData;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.IO;

namespace CmsWeb
{
    [CMSLog]
    [MyRequireHttps]
    public class CmsController : CmsControllerNoHttps
    {
    }

    [CMSLog]
    public class CmsControllerNoHttps : Controller
    {
        protected override void HandleUnknownAction(string actionName)
        {
            //base.HandleUnknownAction(actionName);
            throw new HttpException(404, "404");
        }
        public static string HeaderHtml(string altcontent, string headertext, string logoimg)
        {
            var c = DbUtil.Content("Site2Header" + altcontent) ?? DbUtil.Content("Site2Header");
            if (c != null)
                return c.Body;
            return @"
		<div id=""header"">
		   <div id=""title"">
		      <h1><img alt=""logo"" src='{0}' align=""middle"" />&nbsp;{1}</h1>
		   </div>
		</div>".Fmt(logoimg, headertext);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Util.Helpfile = "_{0}_{1}".Fmt(
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);
            DbUtil.Db.UpdateLastActivity(Util.UserId);
        }
        public string AuthenticateDeveloper(bool log = false, string addrole = "")
        {
            var auth = Request.Headers["Authorization"];
            if (auth.HasValue())
            {
                var cred = System.Text.ASCIIEncoding.ASCII.GetString(
                    Convert.FromBase64String(auth.Substring(6))).Split(':');
                var username = cred[0];
                var password = cred[1];

                string ret = null;
                var valid = CMSMembershipProvider.provider.ValidateUser(username, password);
                if (valid)
                {
                    var roles = CMSRoleProvider.provider;
                    var u = CmsWeb.Models.AccountModel.SetUserInfo(username, Session);
                    if (!roles.IsUserInRole(username, "Developer"))
                        valid = false;
                    if (addrole.HasValue() && !roles.IsUserInRole(username, addrole))
                        valid = false;
                }
                if (valid)
                    ret = " API {0} authenticated".Fmt(username);
                else
                    ret = "!API {0} not authenticated".Fmt(username);
                if (log)
                    DbUtil.LogActivity(ret.Substring(1));
                return ret;
            }
            return "!API no Authorization Header";
        }
    }
    [CMSLog]
    [MyRequireHttps]
    public class CmsStaffController : Controller
    {
        public bool NoCheckRole { get; set; }

        protected override void HandleUnknownAction(string actionName)
        {
            //base.HandleUnknownAction(actionName);
            throw new HttpException(404, "404");
        }
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var s = "/Logon?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl);
                if (Request.QueryString.Count > 0)
                    s += "&" + Request.QueryString.ToString();
                filterContext.Result = Redirect(s);
            }
            else if (!NoCheckRole)
            {
                var r = Models.AccountModel.CheckAccessRole(Util.UserName);
                if (r.HasValue())
                    filterContext.Result = Redirect(r);
            }
            base.OnActionExecuting(filterContext);
            Util.Helpfile = "_{0}_{1}".Fmt(
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);
            DbUtil.Db.UpdateLastActivity(Util.UserId);
        }
        public ActionResult RedirectShowError(string message)
        {
            return new RedirectResult(
                "/Home/ShowError/?error={0}&url={1}".Fmt(Server.UrlEncode(message),
                Request.Url.OriginalString));
        }
    }
    [CMSLog]
    [MyRequireHttps]
    public class CmsStaffAsyncController : System.Web.Mvc.AsyncController
    {
        public bool NoCheckRole { get; set; }

        protected override void HandleUnknownAction(string actionName)
        {
            //base.HandleUnknownAction(actionName);
            throw new HttpException(404, "404");
        }
        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var s = "/Logon?ReturnUrl=" + HttpUtility.UrlEncode(Request.RawUrl);
                if (Request.QueryString.Count > 0)
                    s += "&" + Request.QueryString.ToString();
                filterContext.Result = Redirect(s);
            }
            else if (!NoCheckRole)
            {
                var r = Models.AccountModel.CheckAccessRole(Util.UserName);
                if (r.HasValue())
                    filterContext.Result = Redirect(r);
            }
            base.OnActionExecuting(filterContext);
            Util.Helpfile = "_{0}_{1}".Fmt(
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);
            DbUtil.Db.UpdateLastActivity(Util.UserId);
        }
    }
    public class RequireBasicAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            if (!req.Headers["Authorization"].HasValue() && !req.Headers["username"].HasValue())
            {
                var res = filterContext.HttpContext.Response;
                res.StatusCode = 401;
                res.AddHeader("WWW-Authenticate", "Basic realm=\"{0}\"".Fmt(DbUtil.Db.Host));
                res.End();
            }
        }
    }

    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
#if DEBUG
#else
			var context = filterContext.HttpContext;
			if (context.Session != null)
				if (context.Session.IsNewSession)
				{
					string sessionCookie = context.Request.Headers["Cookie"];
					if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
						filterContext.Result = new RedirectResult("/Errors/SessionTimeout.htm");
				}
#endif
            base.OnActionExecuting(filterContext);
        }
    }
    public class DataGridResult : ActionResult
    {
        DataGrid dg;
        bool excel;
        public DataGridResult(IEnumerable list, bool excel = false)
        {
            dg = new DataGrid();
            dg.DataSource = list;
            this.excel = excel;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;

            if (excel)
            {
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=CMSPeople.xls");
            }
            Response.Charset = "";
            Response.Write(@"
<html>
<head>
<style>
body, html {
    font-family: Verdana, Arial, Helvetica, sans-serif;
    font-size: 12px;
    color: #222;
}
table, tbody, tfoot, thead, tr, th, td {
    margin: 0;
    padding: 0;
    border: 0;
    font-size: 100%;
    font: inherit;
}
table.grid {
    border-collapse: collapse;
    text-align: left;
}
    table.grid td {
        padding: 5px;
    }
table.grid > tbody > tr:nth-child(odd) {
    background-color: #eee;
}
table.grid tr:nth-child(1) {
    border-bottom: 2px solid #555;
    font-weight: bold;
    color: #555;
}
</style>
</head>
<body>
");
            dg.CssClass = @"grid";
            dg.DataBind();
            dg.RenderControl(new HtmlTextWriter(Response.Output));
            Response.Write("</body></head>");
        }
    }
    public class MyRequireHttpsAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext != null)
            {
                if (filterContext.HttpContext.Request.IsLocal)
                    return;
                if (ConfigurationManager.AppSettings["INSERT_X-FORWARDED-PROTO"] == "true")
                {
                    if (filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"] == "http")
                    {
                        var s = filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:");
                        filterContext.Result = new RedirectResult(s);
                    }
                    return;
                }
                else if (!ConfigurationManager.AppSettings["cmshost"].StartsWith("https:"))
                    return;
            }
            base.OnAuthorization(filterContext);
        }
        public static bool NeedRedirect(HttpRequestBase Request)
        {
            if (ConfigurationManager.AppSettings["INSERT_X-FORWARDED-PROTO"] == "true")
                return Request.Headers["X-Forwarded-Proto"] == "http";
            return ConfigurationManager.AppSettings["cmshost"].StartsWith("https:");
        }
    }
    public class CMSLogAttribute : ActionFilterAttribute
    {
        protected DateTime start_time;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            start_time = DateTime.Now;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var cs = ConfigurationManager.ConnectionStrings["CmsLogging"];
            if (cs != null)
            {
                var routeData = filterContext.RouteData;
                var ts = (DateTime.Now - start_time);
                var duration = ts.TotalSeconds;
                var method = filterContext.HttpContext.Request.HttpMethod;
                var controller = (string)routeData.Values["controller"];
                var action = (string)routeData.Values["action"];
                var userid = Util.UserName;
                var dbname = Util.Host;
                var cn = new SqlConnection(cs.ConnectionString);
                cn.Open();
                var cmd = new SqlCommand("LogRequest", cn) {CommandType = CommandType.StoredProcedure};
                cmd.Parameters.AddWithValue("dbname", dbname);
                cmd.Parameters.AddWithValue("method", method);
                cmd.Parameters.AddWithValue("controller", controller);
                cmd.Parameters.AddWithValue("action", action);
                cmd.Parameters.AddWithValue("userid", userid);
                cmd.Parameters.AddWithValue("duration", duration);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
    }
}
