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
    public partial class AnaSayfa : Form
    {
        public AnaSayfa()
        {
            InitializeComponent();
        }

        private DataSet ds = new DataSet();
        string plaka = "", marka, model, renk, aracPlakaSil, KiralanmaTarihi, KiralanmaSaati;
        int MusteriID, AracID, ucret, KiralanmaUcreti, KiralanmaSuresi;
        string siralaQuery = "", siralaQuery1 = "", siralaQuery2 = "";
        int[] ilkMi = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        int[] ilkMi1 = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        int[] ilkMi2 = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        private int ucretHesapla(string veris, string alis)
        {
            int ucret = 0;
            DateTime verisTarihi, alisTarihi;
            verisTarihi = DateTime.Parse(veris);
            alisTarihi = DateTime.Parse(alis);
            ucret = int.Parse((verisTarihi - alisTarihi).TotalDays.ToString());
            return ucret;
        }

        public void veriCek()
        {
            ds.Clear();
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            SqlDataAdapter daRezervasyonlar = new SqlDataAdapter("Select rezervasyonlar.AracID from arac INNER JOIN rezervasyonlar ON arac.AracID = rezervasyonlar.AracID", giris.Baglanti);
            daRezervasyonlar.Fill(ds, "Rez");

            SqlDataAdapter daArac = new SqlDataAdapter("Select * from arac " + siralaQuery, giris.Baglanti);
            daArac.Fill(ds, "Arac");
            SqlDataAdapter daTurler = new SqlDataAdapter("Select TurAdi FROM arac INNER JOIN turler ON arac.TurID=turler.TurID", giris.Baglanti);
            daTurler.Fill(ds, "Turler");
            SqlDataAdapter daMarkalar = new SqlDataAdapter("Select MarkaAdi FROM arac INNER JOIN markalar ON arac.MarkaID=markalar.MarkaID", giris.Baglanti);
            daMarkalar.Fill(ds, "Markalar");
            SqlDataAdapter daModeller = new SqlDataAdapter("Select ModelAdi FROM arac INNER JOIN modeller ON arac.ModelID=modeller.ModelID", giris.Baglanti);
            daModeller.Fill(ds, "Modeller");
            SqlDataAdapter daRenkler = new SqlDataAdapter("Select RenkAdi FROM arac  INNER JOIN renkler ON arac.RenkID=renkler.RenkID", giris.Baglanti);
            daRenkler.Fill(ds, "Renkler");
            SqlDataAdapter daKasaTipi = new SqlDataAdapter("Select KasaAdi FROM arac INNER JOIN kasaTipi ON arac.KasaID=kasaTipi.KasaID", giris.Baglanti);
            daKasaTipi.Fill(ds, "Kasalar");
            SqlDataAdapter Rezervasyonlar = new SqlDataAdapter("Select arac.Fiyat,rezervasyonlar.AlisTarih,rezervasyonlar.AlisSaat,rezervasyonlar.VerisSaat,rezervasyonlar.VerisTarih,rezervasyonlar.AracID,arac.Plaka, " +
                "musteri.MusteriTC from musteri INNER JOIN rezervasyonlar ON musteri.MusteriID = rezervasyonlar.MusteriID INNER JOIN arac ON arac.AracID = rezervasyonlar.AracID " + siralaQuery1, giris.Baglanti);
            Rezervasyonlar.Fill(ds, "Rezervasyonlar");
            SqlDataAdapter daKiralanan = new SqlDataAdapter("Select kiralananArabalar.KiralananAraclarID, musteri.MusteriTC, arac.Plaka, kiralananArabalar.KiralanmaTarihi, kiralananArabalar.KiralanmaSaati, kiralananArabalar.KiralanmaSuresi, " +
                "kiralananArabalar.KiralanmaUcreti FROM kiralananArabalar INNER JOIN musteri ON musteri.MusteriID = kiralananArabalar.MusteriID INNER JOIN arac ON arac.AracID = kiralananArabalar.AracID " + siralaQuery2, giris.Baglanti);
            daKiralanan.Fill(ds, "Kiralanma");

            for (int i = 0; i < ds.Tables["Kiralanma"].Rows.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = ds.Tables["Kiralanma"].Rows[i]["KiralananAraclarID"].ToString();
                lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["MusteriTC"].ToString());
                lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["Plaka"].ToString());
                lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["KiralanmaTarihi"].ToString());
                lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["KiralanmaSaati"].ToString());
                lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["KiralanmaSuresi"].ToString());
                lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["KiralanmaUcreti"].ToString());
                listView3.Items.Add(lvi);
            }

            for (int i = 0; i < ds.Tables["Rezervasyonlar"].Rows.Count; i++)
            {
                ucret = 0;
                ucret = ucretHesapla(ds.Tables["Rezervasyonlar"].Rows[i]["VerisTarih"].ToString(), ds.Tables["Rezervasyonlar"].Rows[i]["AlisTarih"].ToString());
                ucret *= int.Parse(ds.Tables["Rezervasyonlar"].Rows[i]["Fiyat"].ToString());
                ListViewItem lvi = new ListViewItem();
                lvi.Text = ds.Tables["Rezervasyonlar"].Rows[i]["MusteriTC"].ToString();
                lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["Plaka"].ToString());
                lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["AlisTarih"].ToString());
                lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["AlisSaat"].ToString());
                lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["VerisTarih"].ToString());
                lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["VerisSaat"].ToString());
                lvi.SubItems.Add(ucret.ToString());
                listView2.Items.Add(lvi);
            }

            for (int i = 0; i < ds.Tables["Arac"].Rows.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.BackColor = Color.Green;
                lvi.Text = ds.Tables["Arac"].Rows[i]["Plaka"].ToString();
                lvi.SubItems.Add(ds.Tables["Markalar"].Rows[i]["MarkaAdi"].ToString());
                lvi.SubItems.Add(ds.Tables["Modeller"].Rows[i]["ModelAdi"].ToString());
                lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["Fiyat"].ToString());
                lvi.SubItems.Add(ds.Tables["Turler"].Rows[i]["TurAdi"].ToString());
                lvi.SubItems.Add(ds.Tables["Renkler"].Rows[i]["RenkAdi"].ToString());
                lvi.SubItems.Add(ds.Tables["Kasalar"].Rows[i]["KasaAdi"].ToString());
                lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["Kilometre"].ToString());
                lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["YakitTuru"].ToString());
                lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["Vites"].ToString());
                lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["MotorGucu"].ToString());
                lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["MotorHacmi"].ToString());

                for (int j = 0; j < ds.Tables["Rez"].Rows.Count; j++)
                    if (ds.Tables["Rez"].Rows[j]["AracID"].ToString() == ds.Tables["Arac"].Rows[i]["AracID"].ToString())
                        lvi.BackColor = Color.Red;

                listView1.Items.Add(lvi);
            }


        }

        private void Kirala()
        {
            SqlCommand komut = new SqlCommand("Insert into rezervasyonlar (MusteriID,AracID,AlisTarih,AlisSaat,VerisTarih,VerisSaat) Values (@MusteriID,@AracID,@AlisTarih,@AlisSaat,@VerisTarih,@VerisSaat)", giris.Baglanti);
            komut.Parameters.AddWithValue("MusteriID", MusteriID);
            komut.Parameters.AddWithValue("AracID", AracID);
            komut.Parameters.AddWithValue("AlisTarih", dtAlis.Text);
            komut.Parameters.AddWithValue("AlisSaat", dtAlistSaat.Text);
            komut.Parameters.AddWithValue("VerisTarih", dtVeris.Text);
            komut.Parameters.AddWithValue("VerisSaat", dtVerisSaat.Text);
            if (komut.ExecuteNonQuery() == 1)
                MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
            veriCek();

        }

        private void adminMi()
        {
            if (giris.adminMi == 0)
            {
                musteriForm.Visible = true;
                personelForm.Visible = false;
                aracForm.Visible = false;
                tanimlamalar.Visible = false;
                cizik1.Visible = true;
                cizik2.Visible = cizik3.Visible = cizik4.Visible = false;
            }
            else if (giris.adminMi == 1)
            {
                musteriForm.Visible = true;
                personelForm.Visible = true;
                aracForm.Visible = true;
                tanimlamalar.Visible = true;
                cizik1.Visible = cizik2.Visible = cizik3.Visible = cizik4.Visible = true;
            }
            else
            {
                musteriForm.Visible = false;
                personelForm.Visible = false;
                aracForm.Visible = false;
                tanimlamalar.Visible = false;
                cizik1.Visible = cizik2.Visible = cizik3.Visible = cizik4.Visible = false;
                label1.Text = "AUTOMANIA | Ana Sayfa";
            }
        }

        private void teslimEt()
        {
            try
            {
                SqlCommand komutEkle = new SqlCommand("Insert into kiralananArabalar (MusteriID, AracID, KiralanmaTarihi, KiralanmaSaati, KiralanmaSuresi, KiralanmaUcreti) Values (@MusteriID, @AracID, @KiralanmaTarihi, @KiralanmaSaati, @KiralanmaSuresi, @KiralanmaUcreti)", giris.Baglanti);
                komutEkle.Parameters.AddWithValue("MusteriID", KMusteriID);
                komutEkle.Parameters.AddWithValue("AracID", KAracID);
                komutEkle.Parameters.AddWithValue("KiralanmaTarihi", KiralanmaTarihi);
                komutEkle.Parameters.AddWithValue("KiralanmaSaati", KiralanmaSaati);
                komutEkle.Parameters.AddWithValue("KiralanmaSuresi", KiralanmaSuresi);
                komutEkle.Parameters.AddWithValue("KiralanmaUcreti", KiralanmaUcreti);
                if (komutEkle.ExecuteNonQuery() == 1)
                {
                    SqlCommand komut = new SqlCommand("Delete from rezervasyonlar where AracID=@kno", giris.Baglanti);
                    komut.Parameters.AddWithValue("@kno", aracPlakaSil);
                    if (komut.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Araç Teslim edildi.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        veriCek();
                    }
                }


            }

            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AnaSayfa_Load(object sender, EventArgs e)
        {
            dtAlis.MinDate = DateTime.Now;
            dtVeris.MinDate = DateTime.Now.AddDays(1);
            btnCikis.Location = new Point(1500, 5);
            //adminMi();
            veriCek();
        }

        private void musteriForm_Click(object sender, EventArgs e)
        {
            MusteriForm musteriForm = new MusteriForm();
            musteriForm.ShowDialog();
        }

        private void personelForm_Click(object sender, EventArgs e)
        {
            PersonelFormu personelFormu = new PersonelFormu();
            personelFormu.ShowDialog();
        }

        private void aracForm_Click(object sender, EventArgs e)
        {
            AracFormu aracFormu = new AracFormu();
            aracFormu.ShowDialog();

        }

        private void tbAramaTeslim_TextChanged(object sender, EventArgs e)
        {
            if (tbAramaTeslim.Text.Length != 0)
            {
                listView2.Items.Clear();
                ds.Clear();
                try
                {
                    SqlDataAdapter Rezervasyonlar = new SqlDataAdapter("Select arac.Fiyat,rezervasyonlar.AlisTarih,rezervasyonlar.AlisSaat,rezervasyonlar.VerisSaat,rezervasyonlar.VerisTarih,rezervasyonlar.AracID,arac.Plaka, " +
               "rezervasyonlar.MusteriID, musteri.MusteriTC from musteri INNER JOIN rezervasyonlar ON musteri.MusteriID = rezervasyonlar.MusteriID INNER JOIN arac ON arac.AracID = rezervasyonlar.AracID WHERE arac.Plaka LIKE '%" + tbAramaTeslim.Text + "%'", giris.Baglanti);
                    Rezervasyonlar.Fill(ds, "Rezervasyonlar");

                    for (int i = 0; i < ds.Tables["Rezervasyonlar"].Rows.Count; i++)
                    {
                        ucret = 0;
                        ucret = ucretHesapla(ds.Tables["Rezervasyonlar"].Rows[i]["VerisTarih"].ToString(), ds.Tables["Rezervasyonlar"].Rows[i]["AlisTarih"].ToString());
                        ucret *= int.Parse(ds.Tables["Rezervasyonlar"].Rows[i]["Fiyat"].ToString());
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = ds.Tables["Rezervasyonlar"].Rows[i]["MusteriTC"].ToString();
                        lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["Plaka"].ToString());
                        lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["AlisTarih"].ToString());
                        lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["AlisSaat"].ToString());
                        lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["VerisTarih"].ToString());
                        lvi.SubItems.Add(ds.Tables["Rezervasyonlar"].Rows[i]["VerisSaat"].ToString());
                        lvi.SubItems.Add(ucret.ToString());
                        listView2.Items.Add(lvi);
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

        private void tbAramaKiralanma_TextChanged(object sender, EventArgs e)
        {
            if (tbAramaKiralanma.Text.Length != 0)
            {
                listView3.Items.Clear();
                ds.Clear();
                try
                {
                    SqlDataAdapter kiralanma = new SqlDataAdapter("Select kiralananArabalar.KiralananAraclarID, musteri.MusteriTC, arac.Plaka, kiralananArabalar.KiralanmaTarihi, kiralananArabalar.KiralanmaSaati, kiralananArabalar.KiralanmaSuresi, " +
                        "kiralananArabalar.KiralanmaUcreti FROM kiralananArabalar INNER JOIN musteri ON musteri.MusteriID = kiralananArabalar.MusteriID INNER JOIN arac ON arac.AracID = kiralananArabalar.AracID WHERE arac.Plaka LIKE '%" + tbAramaKiralanma.Text + "%'", giris.Baglanti);
                    kiralanma.Fill(ds, "Kiralanma");
                    for (int i = 0; i < ds.Tables["Kiralanma"].Rows.Count; i++)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = ds.Tables["Kiralanma"].Rows[i]["KiralananAraclarID"].ToString();
                        lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["MusteriTC"].ToString());
                        lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["Plaka"].ToString());
                        lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["KiralanmaTarihi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["KiralanmaSaati"].ToString());
                        lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["KiralanmaSuresi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Kiralanma"].Rows[i]["KiralanmaUcreti"].ToString());
                        listView3.Items.Add(lvi);
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

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView3.SelectedItems.Count > 0)
                {
                    ds.Clear();
                    SqlDataAdapter daArac = new SqlDataAdapter("Select markalar.MarkaAdi, modeller.ModelAdi from arac INNER JOIN markalar ON markalar.MarkaID = arac.MarkaID INNER JOIN modeller ON modeller.ModelID = arac.ModelID " +
                        "WHERE Plaka ='" + listView3.SelectedItems[0].SubItems[2].Text + "'", giris.Baglanti);
                    daArac.Fill(ds, "AracKontrol");
                    SqlDataAdapter daMusteri = new SqlDataAdapter("Select MusteriAdi, MusteriSoyadi from Musteri WHERE MusteriTC LIKE '%" + listView3.SelectedItems[0].SubItems[1].Text + "%'", giris.Baglanti);
                    daMusteri.Fill(ds, "MusteriKontrol");
                    lbl3MusteriAdi.Text = ds.Tables["MusteriKontrol"].Rows[0]["MusteriAdi"].ToString();
                    lbl3MusteriSoyadi.Text = ds.Tables["MusteriKontrol"].Rows[0]["MusteriSoyadi"].ToString();
                    lbl2Marka.Text = ds.Tables["AracKontrol"].Rows[0]["MarkaAdi"].ToString();
                    lbl3Model.Text = ds.Tables["AracKontrol"].Rows[0]["ModelAdi"].ToString();
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView1_BackColorChanged(object sender, EventArgs e)
        {
        }

        private void lblListview1_TextChanged(object sender, EventArgs e)
        {
            veriCek();
        }

        private void personelForm_MouseEnter(object sender, EventArgs e)
        {
            personelForm.BackColor = Color.Black;

        }

        private void musteriForm_MouseLeave(object sender, EventArgs e)
        {
            musteriForm.BackColor = Color.Transparent;

        }

        private void personelForm_MouseLeave(object sender, EventArgs e)
        {
            personelForm.BackColor = Color.Transparent;

        }

        private void aracForm_MouseEnter(object sender, EventArgs e)
        {
            aracForm.BackColor = Color.Black;
        }

        private void tanimlamalar_MouseLeave(object sender, EventArgs e)
        {
            tanimlamalar.BackColor = Color.Transparent;
        }

        private void tanimlamalar_MouseEnter(object sender, EventArgs e)
        {
            tanimlamalar.BackColor = Color.Black;
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {

            if ("c" + (e.Column + 1).ToString() == "c1")
            {
                if (ilkMi1[e.Column] == 1)
                {
                    siralaQuery1 = "ORDER BY MusteriTC ASC";
                    ilkMi1[e.Column] = 0;
                }
                else
                {
                    siralaQuery1 = "ORDER BY MusteriTC DESC";
                    ilkMi1[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("c" + (e.Column + 1).ToString() == "c2")
            {
                if (ilkMi1[e.Column] == 1)
                {
                    siralaQuery1 = "ORDER BY Plaka ASC";
                    ilkMi1[e.Column] = 0;
                }
                else
                {
                    siralaQuery1 = "ORDER BY Plaka DESC";
                    ilkMi1[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("c" + (e.Column + 1).ToString() == "c3")
            {
                if (ilkMi1[e.Column] == 1)
                {
                    siralaQuery1 = "ORDER BY AlisTarih ASC";
                    ilkMi1[e.Column] = 0;
                }
                else
                {
                    siralaQuery1 = "ORDER BY AlisTarih DESC";
                    ilkMi1[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("c" + (e.Column + 1).ToString() == "c4")
            {
                if (ilkMi1[e.Column] == 1)
                {
                    siralaQuery1 = "ORDER BY AlisSaat ASC";
                    ilkMi1[e.Column] = 0;
                }
                else
                {
                    siralaQuery1 = "ORDER BY AlisSaat DESC";
                    ilkMi1[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("c" + (e.Column + 1).ToString() == "c5")
            {
                if (ilkMi1[e.Column] == 1)
                {
                    siralaQuery1 = "ORDER BY VerisTarih ASC";
                    ilkMi1[e.Column] = 0;
                }
                else
                {
                    siralaQuery1 = "ORDER BY VerisTarih DESC";
                    ilkMi1[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("c" + (e.Column + 1).ToString() == "c6")
            {
                if (ilkMi1[e.Column] == 1)
                {
                    siralaQuery1 = "ORDER BY VerisSaat ASC";
                    ilkMi1[e.Column] = 0;
                }
                else
                {
                    siralaQuery1 = "ORDER BY VerisSaat DESC";
                    ilkMi1[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
        }

        private void listView3_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if ("ch" + (e.Column + 1).ToString() == "ch1")
            {
                if (ilkMi2[e.Column] == 1)
                {
                    siralaQuery2 = "ORDER BY KiralananAraclarID ASC";
                    ilkMi2[e.Column] = 0;
                }
                else
                {
                    siralaQuery2 = "ORDER BY KiralananAraclarID DESC";
                    ilkMi2[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("ch" + (e.Column + 1).ToString() == "ch2")
            {
                if (ilkMi2[e.Column] == 1)
                {
                    siralaQuery2 = "ORDER BY MusteriTC ASC";
                    ilkMi2[e.Column] = 0;
                }
                else
                {
                    siralaQuery2 = "ORDER BY MusteriTC DESC";
                    ilkMi2[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("ch" + (e.Column + 1).ToString() == "ch3")
            {
                if (ilkMi2[e.Column] == 1)
                {
                    siralaQuery2 = "ORDER BY Plaka ASC";
                    ilkMi2[e.Column] = 0;
                }
                else
                {
                    siralaQuery2 = "ORDER BY Plaka DESC";
                    ilkMi2[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("ch" + (e.Column + 1).ToString() == "ch4")
            {
                if (ilkMi2[e.Column] == 1)
                {
                    siralaQuery2 = "ORDER BY KiralanmaTarihi ASC";
                    ilkMi2[e.Column] = 0;
                }
                else
                {
                    siralaQuery2 = "ORDER BY KiralanmaTarihi DESC";
                    ilkMi2[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("ch" + (e.Column + 1).ToString() == "ch5")
            {
                if (ilkMi2[e.Column] == 1)
                {
                    siralaQuery2 = "ORDER BY KiralanmaSaati ASC";
                    ilkMi2[e.Column] = 0;
                }
                else
                {
                    siralaQuery2 = "ORDER BY KiralanmaSaati DESC";
                    ilkMi2[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("ch" + (e.Column + 1).ToString() == "ch6")
            {
                if (ilkMi2[e.Column] == 1)
                {
                    siralaQuery2 = "ORDER BY KiralanmaSuresi ASC";
                    ilkMi2[e.Column] = 0;
                }
                else
                {
                    siralaQuery2 = "ORDER BY KiralanmaSuresi DESC";
                    ilkMi2[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("ch" + (e.Column + 1).ToString() == "ch7")
            {
                if (ilkMi2[e.Column] == 1)
                {
                    siralaQuery2 = "ORDER BY KiralanmaUcreti ASC";
                    ilkMi2[e.Column] = 0;
                }
                else
                {
                    siralaQuery2 = "ORDER BY KiralanmaUcreti DESC";
                    ilkMi2[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
        }

        private void listView1_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {

        }

        private void aracForm_MouseLeave(object sender, EventArgs e)
        {
            aracForm.BackColor = Color.Transparent;
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader1")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY Plaka ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY Plaka DESC";
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
                    siralaQuery = "ORDER BY MarkaID ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MarkaID DESC";
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
                    siralaQuery = "ORDER BY ModelID ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY ModelID DESC";
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
                    siralaQuery = "ORDER BY Fiyat ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY Fiyat DESC";
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
                    siralaQuery = "ORDER BY TurID ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY TurID DESC";
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
                    siralaQuery = "ORDER BY RenkID ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY RenkID DESC";
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
                    siralaQuery = "ORDER BY KasaID ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY KasaID DESC";
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
                    siralaQuery = "ORDER BY Kilometre ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY Kilometre DESC";
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
                    siralaQuery = "ORDER BY YakitTuru ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY YakitTuru DESC";
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
                    siralaQuery = "ORDER BY Vites ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY Vites DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader11")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MotorGucu ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MotorGucu DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
            if ("columnHeader" + (e.Column + 1).ToString() == "columnHeader12")
            {
                if (ilkMi[e.Column] == 1)
                {
                    siralaQuery = "ORDER BY MotorHacmi ASC";
                    ilkMi[e.Column] = 0;
                }
                else
                {
                    siralaQuery = "ORDER BY MotorHacmi DESC";
                    ilkMi[e.Column] = 1;
                }
                giris.baglantiAc();
                veriCek();
                giris.Baglanti.Close();
            }
        }

        private void tanimlamalar_Click(object sender, EventArgs e)
        {
            Tanimlamalar tanimlamalar = new Tanimlamalar();
            tanimlamalar.ShowDialog();
        }

        private void musteriForm_MouseEnter(object sender, EventArgs e)
        {
            musteriForm.BackColor = Color.Black;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lbl2Marka.Text = "";
            lbl2Model.Text = "";
            lbl2MusteriAdi.Text = "";
            lbl2MusteriSoyadi.Text = "";
            giris.baglantiAc();
            teslimEt();
            giris.Baglanti.Close();
        }
        int KMusteriID, KAracID;
        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView2.SelectedItems.Count > 0)
                {
                    ds.Clear();
                    SqlDataAdapter aracID = new SqlDataAdapter("SELECT arac.AracID, markalar.MarkaAdi, modeller.ModelAdi FROM arac INNER JOIN  markalar ON arac.MarkaID = markalar.MarkaID INNER JOIN modeller ON arac.ModelID = modeller.ModelID WHERE Plaka ='" + listView2.SelectedItems[0].SubItems[1].Text + "'", giris.Baglanti);
                    aracID.Fill(ds, "AracIDler");
                    aracPlakaSil = ds.Tables["AracIDler"].Rows[0]["AracID"].ToString();
                    SqlDataAdapter dambilgi = new SqlDataAdapter("SELECT MusteriAdi, MusteriSoyadi, MusteriID FROM musteri WHERE MusteriTC =" + listView2.SelectedItems[0].SubItems[0].Text, giris.Baglanti);
                    dambilgi.Fill(ds, "Musteriler");
                    lbl2MusteriAdi.Text = ds.Tables["Musteriler"].Rows[0]["MusteriAdi"].ToString();
                    lbl2MusteriSoyadi.Text = ds.Tables["Musteriler"].Rows[0]["MusteriSoyadi"].ToString();
                    lbl2Marka.Text = ds.Tables["AracIDler"].Rows[0]["MarkaAdi"].ToString();
                    lbl2Model.Text = ds.Tables["AracIDler"].Rows[0]["ModelAdi"].ToString();
                    KMusteriID = int.Parse(ds.Tables["Musteriler"].Rows[0]["MusteriID"].ToString());
                    KAracID = int.Parse(ds.Tables["AracIDler"].Rows[0]["AracID"].ToString());
                    KiralanmaTarihi = listView2.SelectedItems[0].SubItems[2].Text;
                    KiralanmaSaati = listView2.SelectedItems[0].SubItems[3].Text;
                    KiralanmaSuresi = ucretHesapla(listView2.SelectedItems[0].SubItems[4].Text, listView2.SelectedItems[0].SubItems[2].Text);
                    KiralanmaUcreti = KiralanmaSuresi * int.Parse(listView2.SelectedItems[0].SubItems[6].Text);
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbArama_TextChanged(object sender, EventArgs e)
        {
            if (tbArama.Text.Length != 0)
            {
                listView1.Items.Clear();
                ds.Clear();
                try
                {
                    SqlDataAdapter daRezervasyonlar = new SqlDataAdapter("Select rezervasyonlar.AracID from arac INNER JOIN rezervasyonlar ON arac.AracID = rezervasyonlar.AracID", giris.Baglanti);
                    daRezervasyonlar.Fill(ds, "Rez");

                    SqlDataAdapter daArac = new SqlDataAdapter("Select * from arac WHERE Plaka LIKE '%" + tbArama.Text + "%'", giris.Baglanti);
                    daArac.Fill(ds, "Arac");
                    SqlDataAdapter daTurler = new SqlDataAdapter("Select TurAdi FROM arac INNER JOIN turler ON arac.TurID=turler.TurID", giris.Baglanti);
                    daTurler.Fill(ds, "Turler");
                    SqlDataAdapter daMarkalar = new SqlDataAdapter("Select MarkaAdi FROM arac INNER JOIN markalar ON arac.MarkaID=markalar.MarkaID", giris.Baglanti);
                    daMarkalar.Fill(ds, "Markalar");
                    SqlDataAdapter daModeller = new SqlDataAdapter("Select ModelAdi FROM arac INNER JOIN modeller ON arac.ModelID=modeller.ModelID", giris.Baglanti);
                    daModeller.Fill(ds, "Modeller");
                    SqlDataAdapter daRenkler = new SqlDataAdapter("Select RenkAdi FROM arac  INNER JOIN renkler ON arac.RenkID=renkler.RenkID", giris.Baglanti);
                    daRenkler.Fill(ds, "Renkler");
                    SqlDataAdapter daKasaTipi = new SqlDataAdapter("Select KasaAdi FROM arac INNER JOIN kasaTipi ON arac.KasaID=kasaTipi.KasaID", giris.Baglanti);
                    daKasaTipi.Fill(ds, "Kasalar");

                    for (int i = 0; i < ds.Tables["Arac"].Rows.Count; i++)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.BackColor = Color.Green;
                        lvi.Text = ds.Tables["Arac"].Rows[i]["Plaka"].ToString();
                        lvi.SubItems.Add(ds.Tables["Markalar"].Rows[i]["MarkaAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Modeller"].Rows[i]["ModelAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["Fiyat"].ToString());
                        lvi.SubItems.Add(ds.Tables["Turler"].Rows[i]["TurAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Renkler"].Rows[i]["RenkAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Kasalar"].Rows[i]["KasaAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["Kilometre"].ToString());
                        lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["YakitTuru"].ToString());
                        lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["Vites"].ToString());
                        lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["MotorGucu"].ToString());
                        lvi.SubItems.Add(ds.Tables["Arac"].Rows[i]["MotorHacmi"].ToString());
                        for (int j = 0; j < ds.Tables["Rez"].Rows.Count; j++)
                        {
                            if (ds.Tables["Rez"].Rows[j]["AracID"].ToString() == ds.Tables["Arac"].Rows[i]["AracID"].ToString())
                                lvi.BackColor = Color.Red;
                        }

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

        private void btnKirala_Click(object sender, EventArgs e)
        {
            lblAdi.Text = "";
            lblTC.Text = "";
            lblSoyadi.Text = "";
            lblMarka.Text = "";
            lblModel.Text = "";
            lblPlaka.Text = "";
            lblRenk.Text = "";
            giris.baglantiAc();
            Kirala();
            giris.Baglanti.Close();
            groupBox4.Enabled = false;
            groupBox3.Enabled = false;
            dtAlistSaat.Clear();
            dtVerisSaat.Clear();
            tbAracPlaka.Clear();
            tbTC.Clear();
        }

        private void btnIlerle1_Click(object sender, EventArgs e)
        {
            if (int.Parse(dtVerisSaat.Text.Substring(0, 2)) < 24 && int.Parse(dtAlistSaat.Text.Substring(0, 2)) < 24 && int.Parse(dtAlistSaat.Text.Substring(3, 2)) < 60 && int.Parse(dtVerisSaat.Text.Substring(3, 2)) < 60)
            {
                ds.Clear();
                SqlDataAdapter daTC = new SqlDataAdapter("Select MusteriAdi, MusteriSoyadi, MusteriID from musteri WHERE MusteriTC=" + tbTC.Text, giris.Baglanti);
                daTC.Fill(ds, "Musteri");
                SqlDataAdapter daArac1 = new SqlDataAdapter("Select AracID from arac WHERE plaka ='" + tbAracPlaka.Text + "'", giris.Baglanti);
                daArac1.Fill(ds, "AracKirala");
                if (dtVerisSaat.Text.Trim().Length != 1 && dtAlistSaat.Text.Trim().Length != 1)
                {
                    AracID = int.Parse(ds.Tables["AracKirala"].Rows[0]["AracID"].ToString());
                    MusteriID = int.Parse(ds.Tables["Musteri"].Rows[0]["MusteriID"].ToString());
                    lblTC.Text = tbTC.Text;
                    lblAdi.Text = ds.Tables["Musteri"].Rows[0]["MusteriAdi"].ToString();
                    lblSoyadi.Text = ds.Tables["Musteri"].Rows[0]["MusteriSoyadi"].ToString();
                    lblMarka.Text = marka;
                    lblModel.Text = model;
                    lblPlaka.Text = plaka;
                    lblRenk.Text = renk;
                    groupBox4.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Saat bilgileri girilmelidir.", "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    groupBox4.Enabled = false;
                }
            }
            else
                MessageBox.Show("Saat bilgileri yanlış girilmiştir.", "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].BackColor != Color.Red)
                {
                    tbAracPlaka.Text = plaka = listView1.SelectedItems[0].SubItems[0].Text;
                    marka = listView1.SelectedItems[0].SubItems[1].Text;
                    model = listView1.SelectedItems[0].SubItems[2].Text;
                    renk = listView1.SelectedItems[0].SubItems[5].Text;
                }
                else
                    tbAracPlaka.Clear();
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnIlerle_Click(object sender, EventArgs e)
        {
            if (tbTC.Text.Length == 11 && tbAracPlaka.Text.Length != 0)
            {
                ds.Clear();
                SqlDataAdapter daTC = new SqlDataAdapter("Select COUNT(MusteriTC) AS 'count' from musteri WHERE MusteriTC=" + tbTC.Text, giris.Baglanti);
                daTC.Fill(ds, "Musteri");

                if (ds.Tables["Musteri"].Rows[0]["count"].ToString() == "1")
                {
                    groupBox3.Enabled = true;
                }
                else
                {
                    groupBox3.Enabled = false;
                }
            }
            else
                MessageBox.Show("TC Kimlik uzunluğu 11 haneli olmalıdır ve Araç seçilmelidir.", "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}