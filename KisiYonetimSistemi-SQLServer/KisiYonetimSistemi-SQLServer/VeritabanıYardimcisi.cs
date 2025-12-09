using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace KisiYonetimSistemi_SQLServer
{
    public static class VeritabaniYardimcisi
    {
        // App.config içindeki KisiYonetimDB bağlantısını okur
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["KisiYonetimDB"].ConnectionString;

        public static void InitializeDatabase()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Veritabanı bağlantısı başarılı!");

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

                    using (SqlCommand cmd = new SqlCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                    throw;
                }
            }
        }

        public static void KisiEkle(Kisi kisi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string insertQuery =
                    "INSERT INTO Kisiler (Ad, Soyad, Telefon, Email) VALUES (@Ad, @Soyad, @Telefon, @Email)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Ad", kisi.Ad);
                    cmd.Parameters.AddWithValue("@Soyad", kisi.Soyad);
                    cmd.Parameters.AddWithValue("@Telefon", kisi.Telefon);
                    cmd.Parameters.AddWithValue("@Email", kisi.Email);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<Kisi> KisileriListele(string aramaMetni = "")
        {
            List<Kisi> kisiler = new List<Kisi>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Id, Ad, Soyad, Telefon, Email FROM Kisiler";

                if (!string.IsNullOrEmpty(aramaMetni))
                    query += " WHERE Ad LIKE @Arama OR Soyad LIKE @Arama OR Telefon LIKE @Arama OR Email LIKE @Arama";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(aramaMetni))
                        cmd.Parameters.AddWithValue("@Arama", $"%{aramaMetni}%");

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            kisiler.Add(new Kisi
                            {
                                Id = rd.GetInt32(0),
                                Ad = rd.GetString(1),
                                Soyad = rd.GetString(2),
                                Telefon = rd.IsDBNull(3) ? "" : rd.GetString(3),
                                Email = rd.IsDBNull(4) ? "" : rd.GetString(4)
                            });
                        }
                    }
                }
            }

            return kisiler;
        }

        public static void KisiGuncelle(Kisi kisi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query =
                    "UPDATE Kisiler SET Ad=@Ad, Soyad=@Soyad, Telefon=@Telefon, Email=@Email WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
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

        public static void KisiSil(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "DELETE FROM Kisiler WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
