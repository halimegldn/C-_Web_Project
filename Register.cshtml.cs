using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace bitirme_porject.Pages.Customers
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UserName { get; set; }

        [BindProperty]
        public string UserMail { get; set; }

        [BindProperty]
        public string PasswordHash { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public IActionResult OnPost()
        {
            // Veritabanı bağlantısını oluşturma
            string connectionString = "Server=localhost;Database=new_database;Username=postgres;Password=1234";

            // Kullanıcının girdiği mail ile sorgu oluşturma
            string queryCheckUser = "SELECT * FROM user_information WHERE user_mail = @UserMail";
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                // Kullanıcının varlığını kontrol etme işlemi
                using (NpgsqlCommand commandCheckUser = new NpgsqlCommand(queryCheckUser, connection))
                {
                    commandCheckUser.Parameters.AddWithValue("@UserMail", UserMail);
                    //Sorguyu çalıştırıp sonuçları okuma işlemi
                    using (var reader = commandCheckUser.ExecuteReader())
                    {
                        // Kullanıcı kayıtlıysa hata mesajı gönderme
                        if (reader.Read())
                        {
                            ErrorMessage = "Bu e-posta adresi zaten kullanımda.";
                            return Page();
                        }
                    }
                }

                // Şifrelerin eşleşmesini kontrol etme işlemi
                if (PasswordHash != ConfirmPassword)
                {
                    ErrorMessage = "Şifreler eşleşmiyor.";
                    return Page();
                }

                // Kullanıcıyı veritabanına ekleme işlemi
                string queryInsertUser = "INSERT INTO user_information (user_id, user_name, user_mail, password_hash) VALUES (DEFAULT, @UserName, @UserMail, @PasswordHash)";
                using (NpgsqlCommand insertCommand = new NpgsqlCommand(queryInsertUser, connection))
                {
                    insertCommand.Parameters.AddWithValue("@UserName", UserName);
                    insertCommand.Parameters.AddWithValue("@UserMail", UserMail);
                    insertCommand.Parameters.AddWithValue("@PasswordHash", PasswordHash);

                    insertCommand.ExecuteNonQuery();
                }

                // Kaydedilen kullanıcının Giriş sayfasına yönlendirilmesi
                return RedirectToPage("Login");
            }
        }
    }
}
