using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Models;
using CmsData;
using UtilityExtensions;
using System.Linq.Dynamic;

namespace CmsWeb.Areas.Main.Controllers
{
    public class ExportController : CmsStaffController
    {
        public ActionResult UpdatePeople(int id)
        {
            return new UpdatePeopleModel(id);
        }
        public ActionResult MucketyMap(int id)
        {
            return new MucketyMapResult(id);
        }
        public ActionResult StatusFlags(int id)
        {
            return new StatusFlagsExcelResult(id);
        }
        public ActionResult ExtraValues(int id)
        {
            return new ExtraValueExcelResult(id);
        }
        [Authorize(Roles="Admin")]
        public ActionResult FreshBooks(int id)
        {
            return new FreshBooksResult(id);
        }
        [HttpPost]
        [Authorize(Roles="Finance")]
        public ActionResult Contributions(string id, ContributionsExcelResult m)
        {
            m.type = id;
        	return m;
        }
    }
}
