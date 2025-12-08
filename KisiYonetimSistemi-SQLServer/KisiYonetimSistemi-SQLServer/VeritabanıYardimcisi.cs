using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Burası değişti! Artık System.Data.SQLite değil
using System.IO; // Veritabanı dosyasıyla doğrudan işlem yapmadığımız için aslında buna ihtiyaç kalmayacak ama dursun

namespace KisiYonetimSistemi_SQLServer
{
    public static class VeritabaniYardimcisi // Statik sınıf olarak kalması uygundur
    {
        // SQL Server bağlantı dizeniz! Lütfen kendi sunucu adınıza göre güncelleyin.
        // Genellikle yerel SQL Server Express için "Server=.\\SQLEXPRESS;" kullanılır.
        // Eğer SQL Server'ınız farklı bir isimdeyse veya IP adresindeyse ona göre değiştirin.
        // Integrated Security=True; Windows kimlik doğrulaması kullanır.
        // User ID=sa; Password=YourStrongPassword; şeklinde SQL Server kimlik doğrulaması da kullanabilirsiniz.
        private static string connectionString = "Server=.\\SQLEXPRESS;Database=KisiYonetimDB;Integrated Security=True;TrustServerCertificate=True;";

        // InitializeDatabase metodunu SQL Server'a göre uyarlıyoruz
        public static void InitializeDatabase()
        {
            // MSSQL'de dosya varlığını kontrol etmeye gerek yok, veritabanı sunucuda yönetilir.
            // Bu metot daha çok, ilk çalışmada bağlantının kurulabilir olduğunu test etmek ve
            // gerekli tabloların varlığını kontrol etmek için kullanılabilir.
            // Ancak genellikle tablo oluşturma işini migrations (EF Core ile) veya
            // elle (SSMS ile) yaparız. Yine de bağlantıyı test etmek için durabilir.

            using (SqlConnection conn = new SqlConnection(connectionString)) // SqlConnection kullanıyoruz
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Veritabanı bağlantısı başarılı!"); // Konsol çıktısı (isteğe bağlı)

                    // Tablo oluşturma sorgusu, SQL Server'a göre düzenlendi
                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='Kisiler' AND xtype='U')
                        BEGIN
                            CREATE TABLE Kisiler (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                Ad NVARCHAR(100) NOT NULL,
                                Soyad NVARCHAR(100) NOT NULL,
                                Telefon NVARCHAR(20),
                                Email NVARCHAR(100)
                            );
                        END;
                    ";
                    using (SqlCommand cmd = new SqlCommand(createTableQuery, conn)) // SqlCommand kullanıyoruz
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Veritabanı bağlantısı veya tablo oluşturma hatası: " + ex.Message);
                    // Hata mesajını kullanıcıya göstermek için bir MessageBox.Show() de ekleyebilirsiniz
                    throw new Exception("Veritabanı bağlantısı kurulurken veya tablo oluşturulurken hata oluştu: " + ex.Message);
                }
            }
        }

        // KisiEkle metodu
        public static void KisiEkle(Kisi kisi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) // SqlConnection
            {
                conn.Open();
                string insertQuery = "INSERT INTO Kisiler (Ad, Soyad, Telefon, Email ) VALUES (@Ad, @Soyad, @Telefon, @Email)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn)) // SqlCommand
                {
                    // SqlParameter kullanıyoruz
                    cmd.Parameters.AddWithValue("@Ad", kisi.Ad);
                    cmd.Parameters.AddWithValue("@Soyad", kisi.Soyad);
                    cmd.Parameters.AddWithValue("@Telefon", kisi.Telefon);
                    cmd.Parameters.AddWithValue("@Email", kisi.Email);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        // KisileriGetir metodu
        public static List<Kisi> KisileriListele(string aramaMetni = "")
        {
            List<Kisi> kisiler = new List<Kisi>();
            using (SqlConnection conn = new SqlConnection(connectionString)) // SqlConnection
            {
                conn.Open();
                string selectQuery = "SELECT Id, Ad, Soyad, Telefon, Email FROM Kisiler";

                if (!string.IsNullOrEmpty(aramaMetni))
                {
                    // SQL Server'da LIKE operatörü için % kullanımı aynıdır
                    selectQuery += " WHERE Ad LIKE @Arama OR Soyad LIKE @Arama OR Telefon LIKE @Arama OR Email LIKE @Arama";
                }

                using (SqlCommand cmd = new SqlCommand(selectQuery, conn)) // SqlCommand
                {
                    if (!string.IsNullOrEmpty(aramaMetni))
                    {
                        cmd.Parameters.AddWithValue("@Arama", $"%{aramaMetni}%"); // SqlParameter
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader()) // SqlDataReader
                    {
                        while (reader.Read())
                        {
                            kisiler.Add(new Kisi
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Ad = reader.GetString(reader.GetOrdinal("Ad")),
                                Soyad = reader.GetString(reader.GetOrdinal("Soyad")),
                                Telefon = reader.IsDBNull(reader.GetOrdinal("Telefon")) ? "" : reader.GetString(reader.GetOrdinal("Telefon")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                            });
                        }
                    }
                }
            }
            return kisiler;
        }

        // KisiGuncelle metodu
        public static void KisiGuncelle(Kisi kisi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) // SqlConnection
            {
                conn.Open();
                string updateQuery = "UPDATE Kisiler SET Ad = @Ad, Soyad = @Soyad, Telefon = @Telefon, Email = @Email WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn)) // SqlCommand
                {
                    cmd.Parameters.AddWithValue("@Ad", kisi.Ad);
                    cmd.Parameters.AddWithValue("@Soyad", kisi.Soyad);
                    cmd.Parameters.AddWithValue("@Telefon", kisi.Telefon);
                    cmd.Parameters.AddWithValue("@Email", kisi.Email);
                    cmd.Parameters.AddWithValue("@Id", kisi.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // KisiSil metodu
        public static void KisiSil(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) // SqlConnection
            {
                conn.Open();
                string deleteQuery = "DELETE FROM Kisiler WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, conn)) // SqlCommand
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public class Kisi
        {
            public int Id { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string Telefon { get; set; }
            public string Email { get; set; }

        }
    }
}