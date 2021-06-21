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
    public partial class AracFormu : Form
    {
        public AracFormu()
        {
            InitializeComponent();
        }

        DataSet ds = new DataSet();
        DataSet ds1 = new DataSet();

        string plaka = "", siralaQuery = "";
        int[] ilkMi = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private Boolean uniqueKontrol(string kno)
        {
            SqlDataAdapter da = new SqlDataAdapter("Select * from arac", giris.Baglanti);
            da.Fill(ds1, "UArac");
            for (int i = 0; i < ds1.Tables["UArac"].Rows.Count; i++)
            {
                if (kno == ds1.Tables["UArac"].Rows[i]["Plaka"].ToString())
                {
                    return false;
                }
            }
            return true;
        }

        private void veriCek()
        {
            ds.Clear();
            lwArac.Items.Clear();

            try
            {

                SqlDataAdapter daTur = new SqlDataAdapter("Select * FROM turler", giris.Baglanti);
                daTur.Fill(ds, "Turler");
                cbAracTuru.DataSource = ds.Tables["Turler"];
                cbAracTuru.ValueMember = "TurID";
                cbAracTuru.DisplayMember = "TurAdi";

                SqlDataAdapter daMarka = new SqlDataAdapter("Select * FROM markalar", giris.Baglanti);
                daMarka.Fill(ds, "Markalar");
                cbMarka.DataSource = ds.Tables["Markalar"];
                cbMarka.ValueMember = "MarkaID";
                cbMarka.DisplayMember = "MarkaAdi";

                SqlDataAdapter daModel = new SqlDataAdapter("Select * FROM modeller", giris.Baglanti);
                daModel.Fill(ds, "Modeller");
                cbModel.DataSource = ds.Tables["Modeller"];
                cbModel.ValueMember = "ModelID";
                cbModel.DisplayMember = "ModelAdi";

                SqlDataAdapter daRenk = new SqlDataAdapter("Select * FROM renkler", giris.Baglanti);
                daRenk.Fill(ds, "Renkler");
                cbRenk.DataSource = ds.Tables["Renkler"];
                cbRenk.ValueMember = "RenkID";
                cbRenk.DisplayMember = "RenkAdi";

                SqlDataAdapter daKasa = new SqlDataAdapter("Select * FROM kasaTipi", giris.Baglanti);
                daKasa.Fill(ds, "Kasalar");
                cbKasaTipi.DataSource = ds.Tables["Kasalar"];
                cbKasaTipi.ValueMember = "KasaID";
                cbKasaTipi.DisplayMember = "KasaAdi";

                SqlDataAdapter daArac = new SqlDataAdapter("Select * FROM arac " + siralaQuery, giris.Baglanti);
                daArac.Fill(ds, "LArac");
                SqlDataAdapter daTurler = new SqlDataAdapter("Select TurAdi FROM arac INNER JOIN turler ON arac.TurID=turler.TurID", giris.Baglanti);
                daTurler.Fill(ds, "LTurler");
                SqlDataAdapter daMarkalar = new SqlDataAdapter("Select MarkaAdi FROM arac INNER JOIN markalar ON arac.MarkaID=markalar.MarkaID", giris.Baglanti);
                daMarkalar.Fill(ds, "LMarkalar");
                SqlDataAdapter daModeller = new SqlDataAdapter("Select ModelAdi FROM arac INNER JOIN modeller ON arac.ModelID=modeller.ModelID", giris.Baglanti);
                daModeller.Fill(ds, "LModeller");
                SqlDataAdapter daRenkler = new SqlDataAdapter("Select RenkAdi FROM arac  INNER JOIN renkler ON arac.RenkID=renkler.RenkID", giris.Baglanti);
                daRenkler.Fill(ds, "LRenkler");
                SqlDataAdapter daKasaTipi = new SqlDataAdapter("Select KasaAdi FROM arac INNER JOIN kasaTipi ON arac.KasaID=kasaTipi.KasaID", giris.Baglanti);
                daKasaTipi.Fill(ds, "LKasalar");

                for (int i = 0; i < ds.Tables["LArac"].Rows.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = ds.Tables["LArac"].Rows[i]["Plaka"].ToString();
                    lvi.SubItems.Add(ds.Tables["LMarkalar"].Rows[i]["MarkaAdi"].ToString());
                    lvi.SubItems.Add(ds.Tables["LModeller"].Rows[i]["ModelAdi"].ToString());
                    lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["Fiyat"].ToString());
                    lvi.SubItems.Add(ds.Tables["LTurler"].Rows[i]["TurAdi"].ToString());
                    lvi.SubItems.Add(ds.Tables["LRenkler"].Rows[i]["RenkAdi"].ToString());
                    lvi.SubItems.Add(ds.Tables["LKasalar"].Rows[i]["KasaAdi"].ToString());
                    lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["Kilometre"].ToString());
                    lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["YakitTuru"].ToString());
                    lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["Vites"].ToString());
                    lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["MotorGucu"].ToString());
                    lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["MotorHacmi"].ToString());
                    lwArac.Items.Add(lvi);
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
                if (uniqueKontrol(tbPlaka.Text))
                {
                    SqlCommand komut = new SqlCommand("Insert into arac (Plaka, MarkaID, ModelID, Fiyat, Kilometre, TurID, RenkID, KasaID, YakitTuru, Vites, MotorGucu, MotorHacmi) Values (@Plaka, @Marka, @Model, @Fiyat, @Kilometre, @Tur, @Renk, @KasaTipi, @YakitTuru, @Vites, @MotorGucu, @MotorHacmi)", giris.Baglanti);
                    komut.Parameters.AddWithValue("@Plaka", tbPlaka.Text);
                    komut.Parameters.AddWithValue("@Marka", cbMarka.SelectedValue);
                    komut.Parameters.AddWithValue("@Model", cbModel.SelectedValue);
                    komut.Parameters.AddWithValue("@Fiyat", int.Parse(tbFiyat.Text));
                    komut.Parameters.AddWithValue("@Kilometre", int.Parse(tbKm.Text));
                    komut.Parameters.AddWithValue("@Tur", cbAracTuru.SelectedValue);
                    komut.Parameters.AddWithValue("@Renk", cbRenk.SelectedValue);
                    komut.Parameters.AddWithValue("@KasaTipi", cbKasaTipi.SelectedValue);
                    komut.Parameters.AddWithValue("@YakitTuru", cbYakit.Text);
                    komut.Parameters.AddWithValue("@Vites", cbVites.Text);
                    komut.Parameters.AddWithValue("@MotorGucu", tbMotorGucu.Text);
                    komut.Parameters.AddWithValue("@MotorHacmi", tbMotorHacmi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    veriCek();
                    temizle();
                }
                else
                    MessageBox.Show("Bu plakaya ait kayıt mevcut", "Kayıt Eklenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                "INNER JOIN arac ON arac.AracID = rezervasyonlar.AracID  WHERE arac.Plaka='" + plaka + "'", giris.Baglanti);
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
                    SqlCommand komut = new SqlCommand("Delete from arac WHERE Plaka=@Plaka", giris.Baglanti);
                    komut.Parameters.AddWithValue("@Plaka", plaka);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Böyle bir araç mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                veriCek();
                temizle();
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
                if ((uniqueKontrol(tbPlaka.Text) != true && plaka == tbPlaka.Text) || uniqueKontrol(tbPlaka.Text))
                {
                    SqlCommand komut = new SqlCommand("Update arac set Plaka=@Plaka, MarkaID=@Marka, ModelID=@Model, Fiyat=@Fiyat, Kilometre=@Kilometre, TurID=@Tur, RenkID=@Renk, KasaID=@KasaTipi, YakitTuru=@YakitTuru, Vites=@Vites, MotorGucu=@MotorGucu, MotorHacmi=@MotorHacmi WHERE Plaka=@plakaa", giris.Baglanti);
                    komut.Parameters.AddWithValue("@Plaka", tbPlaka.Text);
                    komut.Parameters.AddWithValue("@Marka", cbMarka.SelectedValue);
                    komut.Parameters.AddWithValue("@Model", cbModel.SelectedValue);
                    komut.Parameters.AddWithValue("@Fiyat", int.Parse(tbFiyat.Text));
                    komut.Parameters.AddWithValue("@Kilometre", int.Parse(tbKm.Text));
                    komut.Parameters.AddWithValue("@Tur", cbAracTuru.SelectedValue);
                    komut.Parameters.AddWithValue("@Renk", cbRenk.SelectedValue);
                    komut.Parameters.AddWithValue("@KasaTipi", cbKasaTipi.SelectedValue);
                    komut.Parameters.AddWithValue("@YakitTuru", cbYakit.Text);
                    komut.Parameters.AddWithValue("@Vites", cbVites.Text);
                    komut.Parameters.AddWithValue("@MotorGucu", tbMotorGucu.Text);
                    komut.Parameters.AddWithValue("@MotorHacmi", tbMotorHacmi.Text);
                    komut.Parameters.AddWithValue("@plakaa", plaka);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Düzenlendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    veriCek();
                    temizle();
                }
                else
                    MessageBox.Show("Bu plakaya ait kayıt mevcut", "Kayıt Düzenlenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private Boolean boslukKontrol()
        {
            bool kontrol = true;
            lblPlaka.Visible = false;
            lblFiyat.Visible = false;
            lblKasaTipi.Visible = false;
            lblKm.Visible = false;
            lblMarka.Visible = false;
            lblModel.Visible = false;
            lblMotorGucu.Visible = false;
            lblMotorHacmi.Visible = false;
            lblRenk.Visible = false;
            lblTur.Visible = false;
            lblVites.Visible = false;
            lblYakit.Visible = false;

            if (tbPlaka.Text == "")
            {
                lblPlaka.Visible = true;
                kontrol = false;
            }
            if (tbFiyat.Text == "")
            {
                lblFiyat.Visible = true;
                kontrol = false;
            }
            if (tbKm.Text == "")
            {
                lblKm.Visible = true;
                kontrol = false;
            }
            if (tbMotorGucu.Text == "")
            {
                lblMotorGucu.Visible = true;
                kontrol = false;
            }
            if (tbMotorHacmi.Text == "")
            {
                lblMotorHacmi.Visible = true;
                kontrol = false;
            }
            if (cbAracTuru.Text == "")
            {
                lblTur.Visible = true;
                kontrol = false;
            }
            if (cbKasaTipi.Text == "")
            {
                lblKasaTipi.Visible = true;
                kontrol = false;
            }
            if (cbMarka.Text == "")
            {
                lblMarka.Visible = true;
                kontrol = false;
            }
            if (cbModel.Text == "")
            {
                lblModel.Visible = true;
                kontrol = false;
            }
            if (cbRenk.Text == "")
            {
                lblRenk.Visible = true;
                kontrol = false;
            }
            if (cbVites.Text == "Seçiniz" || cbVites.Text == "")
            {
                lblVites.Visible = true;
                kontrol = false;
            }
            if (cbYakit.Text == "Seçiniz" || cbYakit.Text == "")
            {
                lblYakit.Visible = true;
                kontrol = false;
            }
            return kontrol;
        }

        private void veriCekAnaSayfa()
        {
            AnaSayfa frm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f.Text == "AnaSayfa")
                {
                    frm = (AnaSayfa)f;
                }
            }
            if (frm != null)
            {
                foreach (Control c in frm.groupBox1.Controls)
                {
                    if (c is ListView)
                    {
                        if (frm.lblListview1.Text == "1")
                        {
                            frm.lblListview1.Text = "0";
                            break;
                        }
                        else
                        {
                            frm.lblListview1.Text = "1";
                            break;
                        }
                    }
                }
            }

        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            veriCekAnaSayfa();
            this.Close();
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

        private void AracFormu_Load(object sender, EventArgs e)
        {
            temizle();
            giris.baglantiAc();
            veriCek();
            giris.Baglanti.Close();

        }

        private void tbArama_TextChanged(object sender, EventArgs e)
        {
            if (tbArama.Text.Length != 0)
            {
                lwArac.Items.Clear();
                ds.Clear();
                try
                {
                    SqlDataAdapter daTur = new SqlDataAdapter("Select * FROM turler", giris.Baglanti);
                    daTur.Fill(ds, "Turler");
                    cbAracTuru.DataSource = ds.Tables["Turler"];
                    cbAracTuru.ValueMember = "TurID";
                    cbAracTuru.DisplayMember = "TurAdi";

                    SqlDataAdapter daMarka = new SqlDataAdapter("Select * FROM markalar", giris.Baglanti);
                    daMarka.Fill(ds, "Markalar");
                    cbMarka.DataSource = ds.Tables["Markalar"];
                    cbMarka.ValueMember = "MarkaID";
                    cbMarka.DisplayMember = "MarkaAdi";

                    SqlDataAdapter daModel = new SqlDataAdapter("Select * FROM modeller", giris.Baglanti);
                    daModel.Fill(ds, "Modeller");
                    cbModel.DataSource = ds.Tables["Modeller"];
                    cbModel.ValueMember = "ModelID";
                    cbModel.DisplayMember = "ModelAdi";

                    SqlDataAdapter daRenk = new SqlDataAdapter("Select * FROM renkler", giris.Baglanti);
                    daRenk.Fill(ds, "Renkler");
                    cbRenk.DataSource = ds.Tables["Renkler"];
                    cbRenk.ValueMember = "RenkID";
                    cbRenk.DisplayMember = "RenkAdi";

                    SqlDataAdapter daKasa = new SqlDataAdapter("Select * FROM kasaTipi", giris.Baglanti);
                    daKasa.Fill(ds, "Kasalar");
                    cbKasaTipi.DataSource = ds.Tables["Kasalar"];
                    cbKasaTipi.ValueMember = "KasaID";
                    cbKasaTipi.DisplayMember = "KasaAdi";

                    SqlDataAdapter daArac = new SqlDataAdapter("Select * from arac WHERE Plaka LIKE '%" + tbArama.Text + "%'", giris.Baglanti);
                    daArac.Fill(ds, "LArac");
                    SqlDataAdapter daTurler = new SqlDataAdapter("Select TurAdi FROM arac INNER JOIN turler ON arac.TurID=turler.TurID", giris.Baglanti);
                    daTurler.Fill(ds, "LTurler");
                    SqlDataAdapter daMarkalar = new SqlDataAdapter("Select MarkaAdi FROM arac INNER JOIN markalar ON arac.MarkaID=markalar.MarkaID", giris.Baglanti);
                    daMarkalar.Fill(ds, "LMarkalar");
                    SqlDataAdapter daModeller = new SqlDataAdapter("Select ModelAdi FROM arac INNER JOIN modeller ON arac.ModelID=modeller.ModelID", giris.Baglanti);
                    daModeller.Fill(ds, "LModeller");
                    SqlDataAdapter daRenkler = new SqlDataAdapter("Select RenkAdi FROM arac  INNER JOIN renkler ON arac.RenkID=renkler.RenkID", giris.Baglanti);
                    daRenkler.Fill(ds, "LRenkler");
                    SqlDataAdapter daKasaTipi = new SqlDataAdapter("Select KasaAdi FROM arac INNER JOIN kasaTipi ON arac.KasaID=kasaTipi.KasaID", giris.Baglanti);
                    daKasaTipi.Fill(ds, "LKasalar");

                    for (int i = 0; i < ds.Tables["LArac"].Rows.Count; i++)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = ds.Tables["LArac"].Rows[i]["Plaka"].ToString();
                        lvi.SubItems.Add(ds.Tables["LMarkalar"].Rows[i]["MarkaAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["LModeller"].Rows[i]["ModelAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["Fiyat"].ToString());
                        lvi.SubItems.Add(ds.Tables["LTurler"].Rows[i]["TurAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["LRenkler"].Rows[i]["RenkAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["LKasalar"].Rows[i]["KasaAdi"].ToString());
                        lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["Kilometre"].ToString());
                        lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["YakitTuru"].ToString());
                        lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["Vites"].ToString());
                        lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["MotorGucu"].ToString());
                        lvi.SubItems.Add(ds.Tables["LArac"].Rows[i]["MotorHacmi"].ToString());
                        lwArac.Items.Add(lvi);
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

        private void temizle()
        {
            tbFiyat.Clear();
            tbKm.Clear();
            tbMotorGucu.Clear();
            tbMotorHacmi.Clear();
            tbPlaka.Clear();
            cbVites.Text = "Seçiniz";
            cbYakit.Text = "Seçiniz";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lwArac.SelectedItems.Count > 0)
                {
                    tbPlaka.Text = plaka = lwArac.SelectedItems[0].SubItems[0].Text;
                    cbMarka.Text = lwArac.SelectedItems[0].SubItems[1].Text;
                    cbModel.Text = lwArac.SelectedItems[0].SubItems[2].Text;
                    tbFiyat.Text = lwArac.SelectedItems[0].SubItems[3].Text;
                    cbAracTuru.Text = lwArac.SelectedItems[0].SubItems[4].Text;
                    cbRenk.Text = lwArac.SelectedItems[0].SubItems[5].Text;
                    cbKasaTipi.Text = lwArac.SelectedItems[0].SubItems[6].Text;
                    tbKm.Text = lwArac.SelectedItems[0].SubItems[7].Text;
                    cbYakit.Text = lwArac.SelectedItems[0].SubItems[8].Text;
                    cbVites.Text = lwArac.SelectedItems[0].SubItems[9].Text;
                    tbMotorGucu.Text = lwArac.SelectedItems[0].SubItems[10].Text;
                    tbMotorHacmi.Text = lwArac.SelectedItems[0].SubItems[11].Text;
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void lwArac_ColumnClick(object sender, ColumnClickEventArgs e)
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
    }
}
