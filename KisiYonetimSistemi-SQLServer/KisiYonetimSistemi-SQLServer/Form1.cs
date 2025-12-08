using System;                 
using System.Windows.Forms;   
using System.Data.SqlClient;   // SQL Server bağlantısı ve sorgular (SqlConnection, SqlCommand vb.)
using System.Collections.Generic; // List<T> gibi koleksiyon sınıfları için (List, Dictionary vb.)

namespace KisiYonetimSistemi_SQLServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            VeritabaniYardimcisi.InitializeDatabase();

            ListeyiYenile();

            // DataGridView'deki bir hücreye tıklandığında TextBox'ların dolması için olay bağla
            dgvKisiler.CellClick += dgvKisiler_CellClick;
        }

        private void ListeyiYenile()
        {
            dgvKisiler.DataSource = null;
            dgvKisiler.DataSource = VeritabaniYardimcisi.KisileriListele();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            var kisi = new VeritabaniYardimcisi.Kisi
            {
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                Telefon = txtTelefon.Text,
                Email = txtEmail.Text
            };

            VeritabaniYardimcisi.KisiEkle(kisi);
            ListeyiYenile();
            MessageBox.Show("Kişi başarıyla eklendi!");
        }
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dgvKisiler.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellenecek kişiyi seçin!");
                return;
            }

            int id = Convert.ToInt32(dgvKisiler.SelectedRows[0].Cells["Id"].Value);

            var kisi = new VeritabaniYardimcisi.Kisi
            {
                Id = id,
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                Telefon = txtTelefon.Text,
                Email = txtEmail.Text
            };

            VeritabaniYardimcisi.KisiGuncelle(kisi);
            ListeyiYenile();
            MessageBox.Show("Kişi başarıyla güncellendi!");
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dgvKisiler.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek kişiyi seçin!");
                return;
            }

            int id = Convert.ToInt32(dgvKisiler.SelectedRows[0].Cells["Id"].Value);
            VeritabaniYardimcisi.KisiSil(id);
            ListeyiYenile();
            MessageBox.Show("Kişi başarıyla silindi!");
        }

        private void btnListele_Click(object sender, EventArgs e)
        {
            ListeyiYenile();
        }

        private void dgvKisiler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvKisiler.Rows[e.RowIndex];

                txtAd.Text = row.Cells[1].Value?.ToString() ?? "";
                txtSoyad.Text = row.Cells[2].Value?.ToString() ?? "";
                txtTelefon.Text = row.Cells[3].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells[4].Value?.ToString() ?? "";
            }
        }

        private void KisileriYukle(string aramaMetni = "")
        {
            dgvKisiler.DataSource = null;
            dgvKisiler.DataSource = VeritabaniYardimcisi.KisileriListele(aramaMetni);
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            string aramaMetni = txtArama.Text.Trim();
            KisileriYukle(aramaMetni);
        }

        private void txtArama_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = txtArama.Text.Trim();
            KisileriYukle(aramaMetni);
        }
    }
}
