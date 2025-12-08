using System;
using System.Windows.Forms;
using System.Collections.Generic;
using KisiYonetimSistemi_SQLServer;

namespace KisiYonetimSistemi_SQLServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            VeritabaniYardimcisi.InitializeDatabase(); // tablo yoksa oluştur
            ListeyiYenile();

            dgvKisiler.CellClick += dgvKisiler_CellClick; // textbox doldurma
        }

        private void ListeyiYenile()
        {
            dgvKisiler.DataSource = null;
            dgvKisiler.DataSource = VeritabaniYardimcisi.KisileriListele();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            var kisi = new Kisi
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

            var kisi = new Kisi
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

                txtAd.Text = row.Cells["Ad"].Value?.ToString() ?? "";
                txtSoyad.Text = row.Cells["Soyad"].Value?.ToString() ?? "";
                txtTelefon.Text = row.Cells["Telefon"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            }
        }

        private void KisileriYukle(string aramaMetni = "")
        {
            dgvKisiler.DataSource = null;
            dgvKisiler.DataSource = VeritabaniYardimcisi.KisileriListele(aramaMetni);
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            KisileriYukle(txtArama.Text.Trim());
        }

        private void txtArama_TextChanged(object sender, EventArgs e)
        {
            KisileriYukle(txtArama.Text.Trim());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvKisiler.AllowUserToAddRows = false;
            dgvKisiler.Columns["Id"].ReadOnly = true;

            dgvKisiler.DataError += (s, err) =>
            {
                err.ThrowException = false;
            };
            dgvKisiler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}
