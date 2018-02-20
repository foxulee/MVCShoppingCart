using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCShoppingCart.Models.Data;

namespace MVCShoppingCart.Models.ViewModels.Shop
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            
        }

        public ProductViewModel(ProductDto productDto)
        {
            Id = productDto.Id;
            Name = productDto.Name;
            Slug = productDto.Slug;
            Price = productDto.Price;
            Description = productDto.Description;
            CategoryId = productDto.CategoryId;
            CategoryName = productDto.CategoryName;
            ImageName = productDto.ImageName;
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}