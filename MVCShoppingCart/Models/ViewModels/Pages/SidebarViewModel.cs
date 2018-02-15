using System.Web.Mvc;
using MVCShoppingCart.Models.Data;

namespace MVCShoppingCart.Models.ViewModels.Pages
{
    public class SidebarViewModel
    {
        public SidebarViewModel()
        {

        }

        public SidebarViewModel(SidebarDto sidebarDto)
        {
            Id = sidebarDto.Id;
            Body = sidebarDto.Body;
        }
        public int Id { get; set; }
        [AllowHtml]
        public string Body { get; set; }
    }
}