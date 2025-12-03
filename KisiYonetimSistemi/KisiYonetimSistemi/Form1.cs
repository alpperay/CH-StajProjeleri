using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KisiYonetimSistemi
{
    public partial class Form1 : Form
    {
        List<Kisi> kisiler = new List<Kisi>();
        int secilenKisiId = -1;
        int sayac = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvKisiler.AutoGenerateColumns = false;

            dgvKisiler.Columns.Clear();
            dgvKisiler.Columns.Add("Id", "Id");
            dgvKisiler.Columns.Add("Ad", "Ad");
            dgvKisiler.Columns.Add("Soyad", "Soyad");
            dgvKisiler.Columns.Add("Email", "Email");
            dgvKisiler.Columns.Add("Telefon", "Telefon");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            kisiEkle();
        }

        private void kisiEkle()
        {
            Kisi k = new Kisi()
            {
                Id = sayac++,
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                Email = txtEmail.Text,
                Telefon = txtTelefon.Text
            };

            kisiler.Add(k);
            ListeyiGuncelle();
            Temizle();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (secilenKisiId == -1) return;

            kisiler.RemoveAll(x => x.Id == secilenKisiId);
            ListeyiGuncelle();
            Temizle();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (secilenKisiId == -1) return;

            Kisi k = kisiler.Find(x => x.Id == secilenKisiId);

            if (k != null)
            {
                k.Ad = txtAd.Text;
                k.Soyad = txtSoyad.Text;
                k.Email = txtEmail.Text;
                k.Telefon = txtTelefon.Text;
            }

            ListeyiGuncelle();
        }

        private void dgvKisiler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKisiler.Rows[e.RowIndex];

                secilenKisiId = Convert.ToInt32(row.Cells["Id"].Value);

                txtAd.Text = row.Cells["Ad"].Value?.ToString();
                txtSoyad.Text = row.Cells["Soyad"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtTelefon.Text = row.Cells["Telefon"].Value?.ToString();
            }
        }

        private void ListeyiGuncelle()
        {
            dgvKisiler.Rows.Clear();

            foreach (var p in kisiler)
            {
                dgvKisiler.Rows.Add(p.Id, p.Ad, p.Soyad, p.Email, p.Telefon);
            }
        }

        private void Temizle()
        {
            secilenKisiId = -1;

            txtAd.Clear();
            txtSoyad.Clear();
            txtEmail.Clear();
            txtTelefon.Clear();
        }

        private void btnKaydetXml_Click(object sender, EventArgs e)
        {
            if (kisiler.Count == 0)
            {
                MessageBox.Show("Kaydedilecek veri yok!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML dosyası (*.xml)|*.xml";
            saveFileDialog.Title = "XML olarak kaydet";
            saveFileDialog.FileName = "Kisiler.xml";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Kisi>));
                    using (var stream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                    {
                        serializer.Serialize(stream, kisiler);
                    }

                    MessageBox.Show("XML başarıyla kaydedildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public class Kisi
        {
            public int Id { get; set; }
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string Email { get; set; }
            public string Telefon { get; set; }
        }

    }
}
