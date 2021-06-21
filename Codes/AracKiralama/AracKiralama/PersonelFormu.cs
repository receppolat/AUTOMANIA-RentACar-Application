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
    public partial class PersonelFormu : Form
    {
        public PersonelFormu()
        {
            InitializeComponent();
        }

        DataSet ds = new DataSet();
        DataSet ds1 = new DataSet();

        string satir = "-1";
        string plaka, siralaQuery = "", siralaQuery1 = "";
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

                SqlDataAdapter da = new SqlDataAdapter("Select * from personel " + siralaQuery, giris.Baglanti);
                da.Fill(ds, "Personel");
                SqlDataAdapter daAdresMusteri = new SqlDataAdapter("Select adresler.AdresID, Cadde +' Caddesi '+ Mahalle +' Mahallesi '+ Sokak + ' Sokağı ' + Sehir +'/'+ Ulke AS 'Adres' FROM personel INNER JOIN adresler ON personel.AdresID=adresler.AdresID " + siralaQuery, giris.Baglanti);
                daAdresMusteri.Fill(ds, "PAdresler");

                for (int i = 0; i < ds.Tables["Personel"].Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();

                    lvi.Text = ds.Tables["Personel"].Rows[i]["PersonelTC"].ToString();
                    lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelAdi"].ToString());
                    lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelSoyadi"].ToString());
                    lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelDogumTarihi"].ToString());
                    lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelCinsiyet"].ToString());
                    lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelTelefon"].ToString());
                    lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelPosta"].ToString());
                    lvi.SubItems.Add(ds.Tables["PAdresler"].Rows[i]["Adres"].ToString());
                    lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelMaas"].ToString());
                    listView1.Items.Add(lvi);
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private Boolean uniqueKontrolTC(string kno)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from personel", giris.Baglanti);
            da.Fill(ds1, "Personel");
            for (int i = 0; i < ds1.Tables["Personel"].Rows.Count; i++)
            {
                if (kno == ds1.Tables["Personel"].Rows[i]["PersonelTC"].ToString())
                {
                    return false;
                }
            }
            return true;
        }

        private void veriEkle()
        {
            try
            {
                if (uniqueKontrolTC(tbTC.Text))
                {
                    SqlCommand komut = new SqlCommand("Insert into personel (PersonelAdi, PersonelSoyadi, AdresID, PersonelTelefon, PersonelPosta, PersonelTC, PersonelCinsiyet, PersonelDogumTarihi, PersonelMaas) Values (@adi,@soyadi,@adres,@tel,@posta,@tc,@cinsiyet,@dtarihi,@maas)", giris.Baglanti);
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
                    komut.Parameters.AddWithValue("@maas", int.Parse(tbMaas.Text));
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

        private void veriSil()
        {
            try
            {

                SqlCommand komut = new SqlCommand("Delete from personel WHERE PersonelTC=@tc", giris.Baglanti);
                komut.Parameters.AddWithValue("@tc", satir);
                if (komut.ExecuteNonQuery() == 1)
                    MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Böyle bir personel mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                veriCek();
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void veriDuzenle()
        {
            try
            {
                if ((uniqueKontrolTC(tbTC.Text) != true && satir == tbTC.Text) || uniqueKontrolTC(tbTC.Text))
                {
                    SqlCommand komut = new SqlCommand("Update personel set PersonelTC=@TC, PersonelAdi=@adi, PersonelSoyadi=@soyadi, AdresID=@adres, PersonelTelefon=@tel, PersonelPosta=@posta, PersonelCinsiyet=@cinsiyet, PersonelDogumTarihi=@dtarihi, PersonelMaas=@maas WHERE PersonelTC=@satir", giris.Baglanti);
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
                    komut.Parameters.AddWithValue("@maas", tbMaas.Text);
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
            lblMaas.Visible = false;
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
            if (tbMaas.Text == "")
            {
                lblMaas.Visible = true;
                kontrol = false;
            }
            if (rbErkek.Checked != true && rbKadin.Checked != true)
            {
                lblCinsiyet.Visible = true;
                kontrol = false;
            }
            return kontrol;
        }

        private void PersonelFormu_Load(object sender, EventArgs e)
        {
            tbDogum.MaxDate = new DateTime(DateTime.Now.Year - 18, DateTime.Now.Month, DateTime.Now.Day);
            giris.baglantiAc();
            veriCek();
            giris.Baglanti.Close();
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

        private void btnSil_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            veriSil();
            giris.Baglanti.Close();
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

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
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
                    cbAdres.SelectedText = listView1.SelectedItems[0].SubItems[7].Text;
                    tbMaas.Text = listView1.SelectedItems[0].SubItems[8].Text;
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader1")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY PersonelTC ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY PersonelTC DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader2")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY PersonelAdi ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY PersonelAdi DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader3")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY PersonelSoyadi ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY PersonelSoyadi DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader4")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY PersonelDogumTarihi ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY PersonelDogumTarihi DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader5")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY PersonelCinsiyet ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY PersonelCinsiyet DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader6")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY PersonelTelefon ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY PersonelTelefon DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }

            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader7")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY PersonelPosta ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY PersonelPosta DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader8")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery1 = "ORDER BY Adres ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery1 = "ORDER BY Adres DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader9")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY PersonelMaas ASC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY PersonelMaas DESC";
                    giris.baglantiAc();
                    veriCek();
                    giris.Baglanti.Close();
                    ilkMi[e.Column] = 1;

                }
            }


        }

        private void tbArama_TextChanged(object sender, EventArgs e)
        {
            if (tbArama.Text.Length != 0)
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

                    SqlDataAdapter da = new SqlDataAdapter("Select * from personel WHERE PersonelTC LIKE '%" + tbArama.Text + "%'", giris.Baglanti);
                    da.Fill(ds, "Personel");
                    SqlDataAdapter daAdresMusteri = new SqlDataAdapter("Select adresler.AdresID, Cadde +' Caddesi '+ Mahalle +' Mahallesi '+ Sokak + ' Sokağı ' + Sehir +'/'+ Ulke AS 'Adres' FROM personel INNER JOIN adresler ON personel.AdresID=adresler.AdresID", giris.Baglanti);
                    daAdresMusteri.Fill(ds, "PAdresler");

                    for (int i = 0; i < ds.Tables["Personel"].Rows.Count; i++)
                    {
                        ListViewItem lvi = new ListViewItem();

                        lvi.Text = ds.Tables["Personel"].Rows[i]["PersonelTC"].ToString();
                        lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelSoyadi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelDogumTarihi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelCinsiyet"].ToString());
                        lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelTelefon"].ToString());
                        lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelPosta"].ToString());
                        lvi.SubItems.Add(ds.Tables["PAdresler"].Rows[i]["Adres"].ToString());
                        lvi.SubItems.Add(ds.Tables["Personel"].Rows[i]["PersonelMaas"].ToString());
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
    }
}