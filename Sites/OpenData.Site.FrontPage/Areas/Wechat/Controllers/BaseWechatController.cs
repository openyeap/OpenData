﻿using OpenData.Data;
using OpenData.Site.FrontPage.Controllers;

namespace OpenData.Site.FrontPage.Areas.Wechats.Controllers
{
    public class BaseWechatManageController : BaseController
    {
        public IDatabase db
        {
            get
            {
                return this.SiteManager.GetSiteDataBase();
            }
        }

        public string CurrentOfficialAccount
        {
            get
            {
                if (this.Session["OfficialAccount"] == null)
                {
                    return string.Empty;
                }
                return this.Session["OfficialAccount"].ToString();
            }
            set
            {
                this.Session["OfficialAccount"] = value;
            }
        }
    }
}