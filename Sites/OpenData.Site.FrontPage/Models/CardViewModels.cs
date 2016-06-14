﻿using OpenData.Framework.Entity;
using OpenData.Common;
using System.ComponentModel.DataAnnotations;

namespace OpenData.Framework.WebApp.Models
{
    public class CardSearchViewModel
    {

        [Display(Name = "CardNumber", ResourceType = typeof(ViewModelResource))]
        public string CardNumber { get; set; }
        [Display(Name = "Grade", ResourceType = typeof(ViewModelResource))]
        public GradeType? CardGrade { get; set; }
        [Display(Name = "IsUsed", ResourceType = typeof(ViewModelResource))]
        public bool? IsUsed { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public PagedList<UserCard> SearchResult { get; set; }

    }

    public class CardSearchResultViewModel
    {
        public string CardNumber { get; set; }
        public string Name { get; set; }
        public GenderType Gender { get; set; }
        public string Birthday { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }
}