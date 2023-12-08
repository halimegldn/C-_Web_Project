using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace bitirme_porject.Pages.Customers
{
    public class MealsModel : PageModel
    {
        [BindProperty]
        public string FilterIngredient1 { get; set; }

        [BindProperty]
        public string FilterIngredient2 { get; set; }

        [BindProperty]
        public string FilterIngredient3 { get; set; }

        [BindProperty]
        public string FilterIngredient4 { get; set; }

        public class Meal
        {
            public string ImageUrl { get; set; }
            public string Name { get; set; }
            public string Ingredients { get; set; }
            public string Recipe { get; set; }
        }

        public void OnGet()
        {
            // Sayfa yüklendiğinde yapılacak işlemler
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Filtreleme işlemleri
            List<Meal> filteredMeals = LoadFilteredMeals();
            // TempData'ye eklemek için JSON formatına çevirme
            TempData["FilteredMeals"] = JsonSerializer.Serialize(filteredMeals);
            // Sonuc sayfasına yönlendirme
            return RedirectToPage("Sonuc");
        }


        private List<Meal> LoadFilteredMeals()
        {
            List<Meal> filteredMeals = new List<Meal>();

            using (NpgsqlConnection connection = new NpgsqlConnection())
            {   //Veritabanı bağlantısı oluşturma 
                connection.ConnectionString = "Server=localhost;Database=new_database;Username=postgres;Password=1234";
                try
                {
                    connection.Open();

                    string query = "SELECT image_url, name, ingredients, recipe FROM meals";
                    // Filtreleri kontrol etme ve sorguyu ekleme 
                    if (!string.IsNullOrEmpty(FilterIngredient1))
                        query += $" WHERE ingredients LIKE '%{FilterIngredient1}%'";

                    if (!string.IsNullOrEmpty(FilterIngredient2))
                        query += (query.Contains("WHERE") ? " AND" : " WHERE") + $" ingredients LIKE '%{FilterIngredient2}%'";

                    if (!string.IsNullOrEmpty(FilterIngredient3))
                        query += (query.Contains("WHERE") ? " AND" : " WHERE") + $" ingredients LIKE '%{FilterIngredient3}%'";

                    if (!string.IsNullOrEmpty(FilterIngredient4))
                        query += (query.Contains("WHERE") ? " AND" : " WHERE") + $" ingredients LIKE '%{FilterIngredient4}%'";
                    // NpgsqlCommand nesnesi ile PostgreSQL veritabanına sorgu göndermek için bir komut oluşturma işlemi
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        // Sorguyu çalıştırıp sonuçları okumak için nesne oluşturma işlemi
                        var reader = command.ExecuteReader();
                        // Her bir satırı tek tek okumak için bir döngünün içinde çalıştırma işlemi
                        while (reader.Read())
                        {
                            //Her bir satır oluşturulan listeye ekleme işlemi
                            filteredMeals.Add(new Meal
                            {
                                ImageUrl = reader.GetString(0),
                                Name = reader.GetString(1),
                                Ingredients = reader.GetString(2),
                                Recipe = reader.GetString(3)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunda yazılacak mesaj
                }
            }
            return filteredMeals;
        }
    }
}
