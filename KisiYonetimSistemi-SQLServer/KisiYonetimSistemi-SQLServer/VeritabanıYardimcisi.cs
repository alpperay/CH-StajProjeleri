using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

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
                    Console.WriteLine("Veritabanı bağlantısı başarılı!");

                    // Tablo oluşturma sorgusu
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
                    throw new Exception("Veritabanı bağlantısı kurulurken veya tablo oluşturulurken hata oluştu: " + ex.Message);
                }
            }
        }

        // KisiEkle metodu
        public static void KisiEkle(Kisi kisi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) 
            {
                conn.Open();
                string insertQuery = "INSERT INTO Kisiler (Ad, Soyad, Telefon, Email ) VALUES (@Ad, @Soyad, @Telefon, @Email)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn)) 
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

        // Veritabanından tüm kişileri veya arama metnine göre filtrelenmiş kişileri listeleyen metot
        public static List<Kisi> KisileriListele(string aramaMetni = "")
        {
            // Boş bir liste oluşturuyoruz; veritabanından gelecek veriler buraya eklenecek
            List<Kisi> kisiler = new List<Kisi>();

            // SqlConnection nesnesi oluşturuluyor ve using ile otomatik kapanması sağlanıyor
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open(); // Bağlantıyı aç

                string selectQuery = "SELECT Id, Ad, Soyad, Telefon, Email FROM Kisiler";

                // Arama metni girilmişse WHERE koşulu ekle
                if (!string.IsNullOrEmpty(aramaMetni))
                {
                    selectQuery += " WHERE Ad LIKE @Arama OR Soyad LIKE @Arama OR Telefon LIKE @Arama OR Email LIKE @Arama";
                }

                // SqlCommand nesnesi oluştur, sorguyu ve bağlantıyı ata
                using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                {
                    // Eğer arama metni varsa parametreyi ekle
                    if (!string.IsNullOrEmpty(aramaMetni))
                    {
                        cmd.Parameters.AddWithValue("@Arama", $"%{aramaMetni}%"); // %aramaMetni% → LIKE için
                    }

                    // SqlDataReader ile verileri oku
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Her bir satırı Kisi nesnesine çevir ve listeye ekle
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

        public static void KisiGuncelle(Kisi kisi)
        // Parametre: kisi -> güncellenecek kişinin Ad, Soyad, Telefon, Email ve Id bilgileri
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) // SqlConnection
            {
                conn.Open();
                // Güncelleme sorgusu (parametreli kullanım SQL Injection'a karşı güvenlidir)
                string updateQuery = "UPDATE Kisiler SET Ad = @Ad, Soyad = @Soyad, Telefon = @Telefon, Email = @Email WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn)) // SqlCommand
                {
                    // Parametreleri sorguya ekle
                    cmd.Parameters.AddWithValue("@Ad", kisi.Ad);
                    cmd.Parameters.AddWithValue("@Soyad", kisi.Soyad);
                    cmd.Parameters.AddWithValue("@Telefon", kisi.Telefon);
                    cmd.Parameters.AddWithValue("@Email", kisi.Email);
                    cmd.Parameters.AddWithValue("@Id", kisi.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Veritabanından belirtilen ID'ye sahip kişiyi silen metot
        public static void KisiSil(int id)
        {
            // SqlConnection nesnesi oluştur ve using ile otomatik kapat
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Bağlantıyı aç
                conn.Open();

                string deleteQuery = "DELETE FROM Kisiler WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}