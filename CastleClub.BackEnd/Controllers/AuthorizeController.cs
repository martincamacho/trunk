using CastleClub.BackEnd.Models;
using CastleClub.BusinessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CastleClub.BackEnd.Controllers
{
    [Authorize(Roles="Super Admin, Admin")]
    public class AuthorizeController : Controller
    {
        public ActionResult Synchronize()
        {
            AuthorizeFileVM model = new AuthorizeFileVM()
            {
                OnlyRefund = true
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Synchronize(AuthorizeFileVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Upload = false;
                return View(model);
            }

            string fileName = CastleClub.BusinessLogic.Data.GlobalParameters.ExcelOutPath + "\\" + Guid.NewGuid().ToString();
            byte[] content= new byte[model.File.InputStream.Length];

            model.File.InputStream.Read(content, 0, model.File.ContentLength);

            System.IO.File.WriteAllBytes(fileName, content);

            bool result=AuthorizeManager.ReadReportFile(fileName, model.OnlyRefund);
            if (result)
            {
                SitesManager.UpdateTotalsReferrers();
            }

            model.File = null;
            model.Upload = result;

            return View(model);
        }
	}
}