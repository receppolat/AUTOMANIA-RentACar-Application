using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace AracKiralama
{
    public partial class MusteriForm : Form
    {
        public MusteriForm()
        {
            InitializeComponent();
        }

        DataSet ds = new DataSet();
        DataSet ds1 = new DataSet();
        string satir = "-1";
        string siralaQuery = "", siralaQuery1 = "";
        int[] ilkMi = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        public void veriCek()
        {
            ds.Clear();
            listView1.Items.Clear();
            try
            {
                SqlDataAdapter daAdresYeni = new SqlDataAdapter("Select adresler.AdresID, Cadde +' Caddesi '+ Mahalle +' Mahallesi '+ Sokak + ' Sokağı ' + Sehir +'/'+ Ulke AS 'Adress' FROM adresler", giris.Baglanti);
                daAdresYeni.Fill(ds, "Adresler");
                cbAdres.DataSource = ds.Tables["Adresler"];
                cbAdres.ValueMember = "AdresID";
                cbAdres.DisplayMember = "Adress";

                SqlDataAdapter da = new SqlDataAdapter("Select * from musteri " + siralaQuery, giris.Baglanti);
                da.Fill(ds, "Musteri");
                SqlDataAdapter daAdresMusteri = new SqlDataAdapter("Select adresler.AdresID, Cadde +' Caddesi '+ Mahalle +' Mahallesi '+ Sokak + ' Sokağı ' + Sehir +'/'+ Ulke AS 'Adres' FROM musteri INNER JOIN adresler ON musteri.AdresID=adresler.AdresID " + siralaQuery1, giris.Baglanti);
                daAdresMusteri.Fill(ds, "MAdresler");

                for (int i = 0; i < ds.Tables["Musteri"].Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();

                    lvi.Text = ds.Tables["Musteri"].Rows[i]["MusteriTC"].ToString();
                    lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriAdi"].ToString());
                    lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriSoyadi"].ToString());
                    lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriDogumTarihi"].ToString());
                    lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriCinsiyet"].ToString());
                    lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriTelefon"].ToString());
                    lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriPosta"].ToString());
                    lvi.SubItems.Add(ds.Tables["MAdresler"].Rows[i]["Adres"].ToString());
                    lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["EhliyetTipi"].ToString());
                    lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["EhliyeTarihi"].ToString());
                    listView1.Items.Add(lvi);
                }



            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void veriEkle()
        {
            try
            {
                if (uniqueKontrolTC(tbTC.Text))
                {
                    SqlCommand komut = new SqlCommand("Insert into musteri (MusteriAdi, MusteriSoyadi, AdresID, MusteriTelefon, MusteriPosta, MusteriTC, MusteriCinsiyet, MusteriDogumTarihi, EhliyetTipi, EhliyeTarihi) Values (@adi,@soyadi,@adres,@tel,@posta,@tc,@cinsiyet,@dtarihi,@tip,@etarihi)", giris.Baglanti);
                    komut.Parameters.AddWithValue("@adi", tbIsim.Text);
                    komut.Parameters.AddWithValue("@soyadi", tbSoyisim.Text);
                    komut.Parameters.AddWithValue("@adres", cbAdres.SelectedValue);
                    komut.Parameters.AddWithValue("@tel", tbTel.Text);
                    komut.Parameters.AddWithValue("@posta", tbEPosta.Text);
                    komut.Parameters.AddWithValue("@tc", tbTC.Text);
                    if (rbErkek.Checked)
                        komut.Parameters.AddWithValue("@cinsiyet", rbErkek.Text);
                    else
                        komut.Parameters.AddWithValue("@cinsiyet", rbKadin.Text);
                    komut.Parameters.AddWithValue("@dtarihi", tbDogum.Text);
                    komut.Parameters.AddWithValue("@tip", cbTip.Text);
                    komut.Parameters.AddWithValue("@etarihi", tbEhliyet.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    veriCek();
                }
                else
                    MessageBox.Show("Bu kayıt Mevcut", "Kayıt Düzenlenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool varMiMusteri(string tablo, string field)
        {
            ds.Clear();
            bool kontrol = true;
            SqlDataAdapter daKontrol = new SqlDataAdapter("Select COUNT(musteri.MusteriTC) AS 'MusteriSayisi' FROM rezervasyonlar INNER JOIN musteri ON musteri.MusteriID = rezervasyonlar.MusteriID " +
                "INNER JOIN arac ON arac.AracID = rezervasyonlar.AracID  WHERE musteri.MusteriTC='" + satir + "'", giris.Baglanti);
            daKontrol.Fill(ds, "Kontrol");
            if (int.Parse(ds.Tables[tablo].Rows[0][field].ToString()) > 0)
                kontrol = true;
            else
                kontrol = false;
            return kontrol;
        }

        private void veriSil()
        {
            try
            {
                bool kontrol = varMiMusteri("Kontrol", "MusteriSayisi");
                if (kontrol)
                    MessageBox.Show("Bu araç bir rezervasyon ile ilişkilidir. Önce Araç/Araçları siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    SqlCommand komut = new SqlCommand("Delete from musteri WHERE MusteriTC=@tc", giris.Baglanti);
                    komut.Parameters.AddWithValue("@tc", satir);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Böyle bir araç mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                veriCek();
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Boolean uniqueKontrolTC(string kno)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from musteri", giris.Baglanti);
            da.Fill(ds1, "Musteri");
            for (int i = 0; i < ds1.Tables["Musteri"].Rows.Count; i++)
            {
                if (kno == ds1.Tables["Musteri"].Rows[i]["MusteriTC"].ToString())
                {
                    return false;
                }
            }
            return true;
        }

        private void veriDuzenle()
        {
            try
            {
                if ((uniqueKontrolTC(tbTC.Text) != true && satir == tbTC.Text) || uniqueKontrolTC(tbTC.Text))
                {
                    SqlCommand komut = new SqlCommand("Update musteri set MusteriTC=@TC, MusteriAdi=@adi, MusteriSoyadi=@soyadi, AdresID=@adres, MusteriTelefon=@tel, MusteriPosta=@posta, MusteriCinsiyet=@cinsiyet, MusteriDogumTarihi=@dtarihi, EhliyetTipi=@etip, EhliyeTarihi=@etarihi WHERE MusteriTC=@satir", giris.Baglanti);
                    komut.Parameters.AddWithValue("TC", tbTC.Text);
                    komut.Parameters.AddWithValue("@adi", tbIsim.Text);
                    komut.Parameters.AddWithValue("@soyadi", tbSoyisim.Text);
                    komut.Parameters.AddWithValue("@adres", cbAdres.SelectedValue);
                    komut.Parameters.AddWithValue("@tel", tbTel.Text);
                    komut.Parameters.AddWithValue("@posta", tbEPosta.Text);
                    if (rbErkek.Checked)
                        komut.Parameters.AddWithValue("@cinsiyet", rbErkek.Text);
                    else
                        komut.Parameters.AddWithValue("@cinsiyet", rbKadin.Text);
                    komut.Parameters.AddWithValue("@dtarihi", tbDogum.Text);
                    komut.Parameters.AddWithValue("@etip", cbTip.Text);
                    komut.Parameters.AddWithValue("@etarihi", tbEhliyet.Text);
                    komut.Parameters.AddWithValue("@satir", satir);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Düzenlendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    veriCek();
                }
                else
                    MessageBox.Show("Bu kayıt Mevcut", "Kayıt Düzenlenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private Boolean boslukKontrol()
        {
            bool kontrol = true;
            lblEPosta.Visible = false;
            lblIsim.Visible = false;
            lblSoyisim.Visible = false;
            lblTC.Visible = false;
            lblTel.Visible = false;
            lblAdres.Visible = false;
            lblTip.Visible = false;
            lblCinsiyet.Visible = false;

            if (tbEPosta.Text == "")
            {
                lblEPosta.Visible = true;
                kontrol = false;
            }
            if (tbIsim.Text == "")
            {
                lblIsim.Visible = true;
                kontrol = false;
            }
            if (tbSoyisim.Text == "")
            {
                lblSoyisim.Visible = true;
                kontrol = false;
            }
            if (tbTC.Text == "")
            {
                lblTC.Visible = true;
                kontrol = false;
            }
            if (tbTel.Text == "")
            {
                lblTel.Visible = true;
                kontrol = false;
            }
            if (cbAdres.Text == "")
            {
                lblAdres.Visible = true;
                kontrol = false;
            }
            if (cbTip.Text == "Seçiniz" || cbTip.Text == "")
            {
                lblTip.Visible = true;
                kontrol = false;
            }
            if (rbErkek.Checked != true && rbKadin.Checked != true)
            {
                lblCinsiyet.Visible = true;
                kontrol = false;
            }
            return kontrol;
        }


        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (boslukKontrol())
            {
                giris.baglantiAc();
                veriEkle();
                giris.Baglanti.Close();
            }
            else
            {
                MessageBox.Show("Yıldızlı alanları dolurunuz.", "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MusteriForm_Load(object sender, EventArgs e)
        {
            tbDogum.MaxDate = new DateTime(DateTime.Now.Year - 19, DateTime.Now.Month, DateTime.Now.Day);
            tbEhliyet.MaxDate = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day);
            giris.baglantiAc();
            veriCek();
            giris.Baglanti.Close();
        }

        private void cbAdres_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            veriSil();
            giris.Baglanti.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    tbTC.Text = satir = listView1.SelectedItems[0].SubItems[0].Text;
                    tbIsim.Text = listView1.SelectedItems[0].SubItems[1].Text;
                    tbSoyisim.Text = listView1.SelectedItems[0].SubItems[2].Text;
                    tbDogum.Text = listView1.SelectedItems[0].SubItems[3].Text;
                    if (listView1.SelectedItems[0].SubItems[4].Text == "Erkek")
                        rbErkek.Checked = true;
                    else
                        rbKadin.Checked = true;
                    tbTel.Text = listView1.SelectedItems[0].SubItems[5].Text;
                    tbEPosta.Text = listView1.SelectedItems[0].SubItems[6].Text;
                    cbAdres.Text = listView1.SelectedItems[0].SubItems[7].Text;
                    cbTip.Text = listView1.SelectedItems[0].SubItems[8].Text;
                    tbEhliyet.Text = listView1.SelectedItems[0].SubItems[9].Text;
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDuzenle_Click(object sender, EventArgs e)
        {
            if (boslukKontrol())
            {
                giris.baglantiAc();
                veriDuzenle();
                giris.Baglanti.Close();
            }
            else
            {
                MessageBox.Show("Yıldızlı alanları dolurunuz.", "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbArama_TextChanged(object sender, EventArgs e)
        {
            if (tbArama.Text.Length != 0)
            {
                listView1.Items.Clear();
                ds.Clear();
                listView1.View = View.Details;
                listView1.FullRowSelect = true;
                listView1.GridLines = true;
                try
                {

                    SqlDataAdapter daAdresYeni = new SqlDataAdapter("Select adresler.AdresID, Cadde +' Caddesi '+ Mahalle +' Mahallesi '+ Sokak + ' Sokağı ' + Sehir +'/'+ Ulke AS 'Adress' FROM adresler", giris.Baglanti);
                    daAdresYeni.Fill(ds, "Adresler");
                    cbAdres.DataSource = ds.Tables["Adresler"];
                    cbAdres.ValueMember = "AdresID";
                    cbAdres.DisplayMember = "Adress";

                    SqlDataAdapter da = new SqlDataAdapter("Select * from musteri WHERE MusteriTC LIKE '%" + tbArama.Text + "%'", giris.Baglanti);
                    da.Fill(ds, "Musteri");
                    SqlDataAdapter daAdresMusteri = new SqlDataAdapter("Select adresler.AdresID, Cadde +' Caddesi '+ Mahalle +' Mahallesi '+ Sokak + ' Sokağı ' + Sehir +'/'+ Ulke AS 'Adres' FROM musteri INNER JOIN adresler ON musteri.AdresID=adresler.AdresID", giris.Baglanti);
                    daAdresMusteri.Fill(ds, "MAdresler");

                    for (int i = 0; i < ds.Tables["Musteri"].Rows.Count; i++)
                    {
                        ListViewItem lvi = new ListViewItem();

                        lvi.Text = ds.Tables["Musteri"].Rows[i]["MusteriTC"].ToString();
                        lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriSoyadi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriDogumTarihi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriCinsiyet"].ToString());
                        lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriTelefon"].ToString());
                        lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["MusteriPosta"].ToString());
                        lvi.SubItems.Add(ds.Tables["MAdresler"].Rows[i]["Adres"].ToString());
                        lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["EhliyetTipi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Musteri"].Rows[i]["EhliyeTarihi"].ToString());
                        listView1.Items.Add(lvi);
                    }
                }
                catch (Exception Hata)
                {
                    MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                veriCek();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader1")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MusteriTC ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MusteriTC DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader2")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MusteriAdi ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MusteriAdi DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader3")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MusteriSoyadi ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MusteriSoyadi DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader4")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MusteriDogumTarihi ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MusteriDogumTarihi DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader5")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MusteriCinsiyet ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MusteriCinsiyet DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader6")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MusteriTelefon ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MusteriTelefon DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader7")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MusteriPosta ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MusteriPosta DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader8")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery1 = "ORDER BY Adres ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery1 = "ORDER BY Adres DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader9")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY EhliyetTipi ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY EhliyetTipi DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader10")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY EhliyeTarihi ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY EhliyeTarihi DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
        }
    }
}