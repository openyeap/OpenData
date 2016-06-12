﻿using OpenData.Data;
using System;

namespace OpenData.Framework.Entity
{

  
    public class WechatUserShareByHour : BaseEntity
    {

        public string OfficialAccount { get; set; }
        public DateTime RefDateTime { get; set; }
        public WechatUserShareScene Scene { get; set; }
        public int PageShareUser { get; set; }
        public int PageShareCount { get; set; }
    }
}