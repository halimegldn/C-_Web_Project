
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Text.Json; // JSON işlemleri için gerekli kütüphaneyi ekleme işlemi 

namespace bitirme_porject.Pages.Customers
{
    public class SonucModel : PageModel
    {
        public List<MealsModel.Meal> FilteredMeals { get; set; } = new List<MealsModel.Meal>();

        public void OnGet()
        {
            // TempData'den JSON formatında veriyi alıp çözümleme işlemi
            string serializedMeals = TempData["FilteredMeals"] as string;
            FilteredMeals = JsonSerializer.Deserialize<List<MealsModel.Meal>>(serializedMeals) ?? new List<MealsModel.Meal>();
            // JSON formatındaki veriyi Meal listesine çözümleme işlemi
        }
    }
}

