using System.ComponentModel.DataAnnotations;
using MVCShoppingCart.Models.Data;

namespace MVCShoppingCart.Models.ViewModels.Shop
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {

        }

        public CategoryViewModel(CategoryDto categoryDto)
        {
            Id = categoryDto.Id;
            Name = categoryDto.Name;
            Slug = categoryDto.Slug;
            Sorting = categoryDto.Sorting;
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }

    }
}