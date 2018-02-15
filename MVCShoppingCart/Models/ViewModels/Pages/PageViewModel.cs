﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCShoppingCart.Models.Data;

namespace MVCShoppingCart.Models.ViewModels.Pages
{
    public class PageViewModel
    {
        public PageViewModel()
        {
        }

        public PageViewModel(PageDto row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [AllowHtml]
        public string Body { get; set; }

        public int Sorting { get; set; }

        public bool HasSidebar { get; set; }
    }
}