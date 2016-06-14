﻿using System;
using System.Web.Mvc;
using OpenData.Caching;

namespace OpenData.Framework.Core
{
    public class BzwaySiteController : Controller
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string UserID { get { return this.User.ToString(); } }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


    }
}