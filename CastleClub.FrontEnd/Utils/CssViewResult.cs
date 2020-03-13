using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CastleClub.FrontEnd.Utils
{
    public class CssViewResult : PartialViewResult
    {
        public CssViewResult() { }

        public CssViewResult(object model)
        {
            this.ViewData = new ViewDataDictionary(model);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "text/css";
            base.ExecuteResult(context);
        }
    }
}