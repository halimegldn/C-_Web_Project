using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace bitirme_porject.Pages.Customers
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Sayfa yüklendiğinde yapılacak işlemler 
        }

        public IActionResult OnPost()
        {
            // Veritabanı bağlantısını oluşturma
            using (NpgsqlConnection connection = new NpgsqlConnection())
            {
                connection.ConnectionString = "Server=localhost;Database=new_database;Username=postgres;Password=1234";
                connection.Open();

                // Kullanıcının girdiği mail ve şifreyle sorgu oluşturma
                string query = $"SELECT * FROM user_information WHERE user_mail = '{Request.Form["user_mail"]}' AND password_hash = '{Request.Form["password_hash"]}'";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();

                    // Kullanıcı girişi doğru olması halinde Anasayfaya yönlendirme
                    if (reader.Read())
                    {
                        connection.Close();
                        return RedirectToPage("Index");
                    }
                }

                // Kullanıcı bulunmaması halinde Hata mesajı
                ErrorMessage = "Geçersiz e-posta veya şifre.";
                connection.Close();
                return Page();
            }
        }
    }
}
