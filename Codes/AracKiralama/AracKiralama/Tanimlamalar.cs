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
    public partial class Tanimlamalar : Form
    {
        public Tanimlamalar()
        {
            InitializeComponent();
        }

        DataSet ds = new DataSet();

        private void adresEkle()
        {
            try
            {
                SqlCommand komut = new SqlCommand("Insert into adresler (Cadde, Mahalle, Sokak, BinaNo, Sehir, Ulke) Values (@Cadde, @Mahalle, @Sokak, @BinaNo, @Sehir, @Ulke)", giris.Baglanti);
                komut.Parameters.AddWithValue("@Cadde", tbCadde.Text);
                komut.Parameters.AddWithValue("@Mahalle", tbMahalle.Text);
                komut.Parameters.AddWithValue("@Sokak", tbSokak.Text);
                komut.Parameters.AddWithValue("@BinaNo", int.Parse(tbBinaNo.Text));
                komut.Parameters.AddWithValue("@Sehir", tbSehir.Text);
                komut.Parameters.AddWithValue("@Ulke", tbUlke.Text);
                if (komut.ExecuteNonQuery() == 1)
                    MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Boolean varMi(string tablo, string field)
        {
            ds.Clear();

            bool kontrol = false;
            SqlDataAdapter daKontrolMusteri = new SqlDataAdapter("Select COUNT(musteri.MusteriAdi) AS 'MusteriSayisi' FROM adresler INNER JOIN musteri ON musteri.AdresID = adresler.AdresID WHERE musteri.AdresID=" + int.Parse(tbAdresSil.Text), giris.Baglanti);
            daKontrolMusteri.Fill(ds, "KontrolMusteri");
            SqlDataAdapter daKontrolPersonel = new SqlDataAdapter("Select COUNT(personel.PersonelAdi) AS 'PersonelSayisi' FROM adresler INNER JOIN personel ON personel.AdresID = adresler.AdresID WHERE personel.AdresID=" + int.Parse(tbAdresSil.Text), giris.Baglanti);
            daKontrolPersonel.Fill(ds, "KontrolPersonel");
            SqlDataAdapter daKontrolOfis = new SqlDataAdapter("Select COUNT(rentACar.OfisAdi) AS 'OfisSayisi' FROM adresler INNER JOIN rentACar ON rentACar.AdresID = adresler.AdresID WHERE rentACar.AdresID=" + int.Parse(tbAdresSil.Text), giris.Baglanti);
            daKontrolOfis.Fill(ds, "KontrolOfis");
            if (int.Parse(ds.Tables[tablo].Rows[0][field].ToString()) > 0)
                kontrol = true;
            else
                kontrol = false;

            return kontrol;
        }


        private void adresSilKontrol()
        {
            try
            {
                bool musteriKontrol = varMi("KontrolMusteri", "MusteriSayisi");
                bool personelKontrol = varMi("KontrolPersonel", "PersonelSayisi");
                bool ofisKontrol = varMi("KontrolOfis", "OfisSayisi");
                if (musteriKontrol)
                    MessageBox.Show("Bu adres en az bir müşteri ile ilişkilidir. Önce Müşteriyi/Müşterileri siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (personelKontrol)
                    MessageBox.Show("Bu adres en az bir personel ile ilişkilidir. Önce Personeli/Personelleri siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (ofisKontrol)
                    MessageBox.Show("Bu adres en az bir ofis ile ilişkilidir. Önce Ofisi/Ofisleri siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    SqlCommand komut = new SqlCommand("Delete from adresler WHERE AdresID=@AdresID", giris.Baglanti);
                    komut.Parameters.AddWithValue("@AdresID", int.Parse(tbAdresSil.Text));
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Böyle bir ID mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private Boolean uniqueKontrolMarka(string kno)
        {
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("select * from markalar", giris.Baglanti);
            da.Fill(ds, "Markalar");
            for (int i = 0; i < ds.Tables["Markalar"].Rows.Count; i++)
            {
                if (kno == ds.Tables["Markalar"].Rows[i]["MarkaAdi"].ToString())
                {
                    return true;
                }
            }
            return false;
        }

        private void markaEkle()
        {
            try
            {
                if (uniqueKontrolMarka(tbMarkaAdi.Text) == false)
                {
                    SqlCommand komut = new SqlCommand("Insert into markalar (MarkaAdi) Values (@MarkaAdi)", giris.Baglanti);
                    komut.Parameters.AddWithValue("@MarkaAdi", tbMarkaAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Bu kayıt Mevcut", "Kayıt Eklenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool varMiMarka(string tablo, string field)
        {
            ds.Clear();
            bool kontrol = true;
            SqlDataAdapter daKontrol = new SqlDataAdapter("Select COUNT(markalar.MarkaAdi) AS 'MarkaSayisi' FROM markalar INNER JOIN arac ON arac.MarkaID = markalar.MarkaID WHERE markalar.MarkaAdi='" + tbMarkaAdi.Text + "'", giris.Baglanti);
            daKontrol.Fill(ds, "KontrolMarka");
            if (int.Parse(ds.Tables[tablo].Rows[0][field].ToString()) > 0)
                kontrol = true;
            else
                kontrol = false;

            return kontrol;
        }

        private void markaSil()
        {
            /*  try
              {*/
            bool markaKontrol = varMiMarka("KontrolMarka", "MarkaSayisi");
            if (markaKontrol)
                MessageBox.Show("Bu marka en az bir araç ile ilişkilidir. Önce Araç/Araçları siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                SqlCommand komut = new SqlCommand("Delete from markalar WHERE MarkaAdi=@MarkaAdi", giris.Baglanti);
                komut.Parameters.AddWithValue("@MarkaAdi", tbMarkaAdi.Text);
                if (komut.ExecuteNonQuery() == 1)
                    MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Böyle bir marka mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            /*   }
               catch (Exception Hata)
               {
                   MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }*/

        }

        private Boolean uniqueKontrolModel(string kno)
        {
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("select * from modeller", giris.Baglanti);
            da.Fill(ds, "Modeller");
            for (int i = 0; i < ds.Tables["Modeller"].Rows.Count; i++)
            {
                if (kno == ds.Tables["Modeller"].Rows[i]["ModelAdi"].ToString())
                {
                    return true;
                }
            }
            return false;
        }

        private void modelEkle()
        {
            try
            {
                if (uniqueKontrolModel(tbModelAdi.Text) == false)
                {
                    SqlCommand komut = new SqlCommand("Insert into modeller (ModelAdi) Values (@ModelAdi)", giris.Baglanti);
                    komut.Parameters.AddWithValue("@ModelAdi", tbModelAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Bu kayıt Mevcut", "Kayıt Eklenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool varMiModel(string tablo, string field)
        {
            ds.Clear();
            bool kontrol = true;
            SqlDataAdapter daKontrol = new SqlDataAdapter("Select COUNT(modeller.ModelAdi) AS 'ModelSayisi' FROM modeller INNER JOIN arac ON arac.ModelID = modeller.ModelID WHERE modeller.ModelAdi='" + tbModelAdi.Text + "'", giris.Baglanti);
            daKontrol.Fill(ds, "KontrolModel");
            if (int.Parse(ds.Tables[tablo].Rows[0][field].ToString()) > 0)
                kontrol = true;
            else
                kontrol = false;

            return kontrol;
        }

        private void modelSil()
        {
            try
            {
                bool kontrol = varMiModel("KontrolModel", "ModelSayisi");
                if (kontrol)
                    MessageBox.Show("Bu model en az bir araç ile ilişkilidir. Önce Araç/Araçları siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    SqlCommand komut = new SqlCommand("Delete from modeller WHERE ModelAdi=@ModelAdi", giris.Baglanti);
                    komut.Parameters.AddWithValue("@ModelAdi", tbModelAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Böyle bir model mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private Boolean uniqueKontrolRenk(string kno)
        {
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("select * from renkler", giris.Baglanti);
            da.Fill(ds, "Renkler");
            for (int i = 0; i < ds.Tables["Renkler"].Rows.Count; i++)
            {
                if (kno == ds.Tables["Renkler"].Rows[i]["RenkAdi"].ToString())
                {
                    return true;
                }

            }
            return false;
        }

        private void renkEkle()
        {
            try
            {
                if (uniqueKontrolRenk(tbRenkAdi.Text) == false)
                {
                    SqlCommand komut = new SqlCommand("Insert into renkler (RenkAdi) Values (@renkAdi)", giris.Baglanti);
                    komut.Parameters.AddWithValue("@renkAdi", tbRenkAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Bu kayıt Mevcut", "Kayıt Eklenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool varMiRenk(string tablo, string field)
        {
            ds.Clear();
            bool kontrol = true;
            SqlDataAdapter daKontrol = new SqlDataAdapter("Select COUNT(renkler.RenkAdi) AS 'RenkSayisi' FROM renkler INNER JOIN arac ON arac.RenkID = renkler.RenkID WHERE renkler.RenkAdi='" + tbRenkAdi.Text + "'", giris.Baglanti);
            daKontrol.Fill(ds, "KontrolRenk");
            if (int.Parse(ds.Tables[tablo].Rows[0][field].ToString()) > 0)
                kontrol = true;
            else
                kontrol = false;
            return kontrol;
        }

        private void renkSil()
        {
            try
            {
                bool kontrol = varMiRenk("KontrolRenk", "RenkSayisi");
                if (kontrol)
                    MessageBox.Show("Bu renk en az bir araç ile ilişkilidir. Önce Araç/Araçları siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    SqlCommand komut = new SqlCommand("Delete from renkler WHERE RenkAdi=@renkAdi", giris.Baglanti);
                    komut.Parameters.AddWithValue("@renkAdi", tbRenkAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Böyle bir renk mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private Boolean uniqueKontrolTur(string kno)
        {
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("select * from turler", giris.Baglanti);
            da.Fill(ds, "Turler");
            for (int i = 0; i < ds.Tables["Turler"].Rows.Count; i++)
            {
                if (kno == ds.Tables["Turler"].Rows[i]["TurAdi"].ToString())
                {
                    return true;
                }
            }
            return false;
        }

        private void turEkle()
        {
            try
            {
                if (uniqueKontrolTur(tbTurAdi.Text) == false)
                {
                    SqlCommand komut = new SqlCommand("Insert into turler (TurAdi) Values (@turAdi)", giris.Baglanti);
                    komut.Parameters.AddWithValue("@turAdi", tbTurAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Bu kayıt Mevcut", "Kayıt Eklenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool varMiTur(string tablo, string field)
        {
            ds.Clear();
            bool kontrol = true;
            SqlDataAdapter daKontrol = new SqlDataAdapter("Select COUNT(turler.TurAdi) AS 'TurSayisi' FROM turler INNER JOIN arac ON turler.TurID = arac.TurID WHERE turler.TurAdi='" + tbTurAdi.Text + "'", giris.Baglanti);
            daKontrol.Fill(ds, "KontrolTur");
            if (int.Parse(ds.Tables[tablo].Rows[0][field].ToString()) > 0)
                kontrol = true;
            else
                kontrol = false;
            return kontrol;
        }

        private void turSil()
        {
            try
            {
                bool kontrol = varMiTur("KontrolTur", "TurSayisi");
                if (kontrol)
                    MessageBox.Show("Bu tür en az bir araç ile ilişkilidir. Önce Araç/Araçları siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    SqlCommand komut = new SqlCommand("Delete from turler WHERE TurAdi=@turAdi", giris.Baglanti);
                    komut.Parameters.AddWithValue("@turAdi", tbTurAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Böyle bir tür mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private Boolean uniqueKontrolKasa(string kno)
        {
            ds.Clear();
            SqlDataAdapter da = new SqlDataAdapter("select * from kasaTipi", giris.Baglanti);
            da.Fill(ds, "KasaTipi");
            for (int i = 0; i < ds.Tables["KasaTipi"].Rows.Count; i++)
            {
                if (kno == ds.Tables["KasaTipi"].Rows[i]["KasaAdi"].ToString())
                {
                    return true;
                }

            }
            return false;
        }

        private void KasaTipiEkle()
        {
            try
            {
                if (uniqueKontrolKasa(tbKasaTipiAdi.Text) == false)
                {
                    SqlCommand komut = new SqlCommand("Insert into kasaTipi (KasaAdi) Values (@kasaAdi)", giris.Baglanti);
                    komut.Parameters.AddWithValue("@kasaAdi", tbKasaTipiAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Eklendi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Bu kayıt Mevcut", "Kayıt Eklenemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool varMiKasa(string tablo, string field)
        {
            ds.Clear();
            bool kontrol = true;
            SqlDataAdapter daKontrol = new SqlDataAdapter("Select COUNT(kasaTipi.KasaAdi) AS 'KasaSayisi' FROM kasaTipi INNER JOIN arac ON kasaTipi.KasaID = arac.KasaID WHERE kasaTipi.KasaAdi='" + tbKasaTipiAdi.Text + "'", giris.Baglanti);
            daKontrol.Fill(ds, "KontrolKasa");
            if (int.Parse(ds.Tables[tablo].Rows[0][field].ToString()) > 0)
                kontrol = true;
            else
                kontrol = false;
            return kontrol;
        }

        private void kasaSil()
        {
            try
            {
                bool kontrol = varMiKasa("KontrolKasa", "KasaSayisi");
                if (kontrol)
                    MessageBox.Show("Bu kasa tipi en az bir araç ile ilişkilidir. Önce Araç/Araçları siliniz.", "İlişkili Tablo Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    SqlCommand komut = new SqlCommand("Delete from kasaTipi WHERE KasaAdi=@kasaAdi", giris.Baglanti);
                    komut.Parameters.AddWithValue("@kasaAdi", tbKasaTipiAdi.Text);
                    if (komut.ExecuteNonQuery() == 1)
                        MessageBox.Show("Kayıt Silindi", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Böyle bir kasa tipi mevcut değildir.", "Kayıt Silinemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception Hata)
            {
                MessageBox.Show(Hata.Message, "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private Boolean boslukKontrol(string tb)
        {
            if (tb.Length == 0)
            {
                MessageBox.Show("Lütfen gerekli alanları doldurununz.", "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void Tanimlamalar_Load(object sender, EventArgs e)
        {

        }

        private void btnAdres_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            adresEkle();
            giris.Baglanti.Close();
        }

        private void btnMarkaSil_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            markaSil();
            giris.Baglanti.Close();
        }

        private void btnAdresSil_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            adresSilKontrol();
            giris.Baglanti.Close();
        }

        private void btnMarkaEkle_Click(object sender, EventArgs e)
        {
            if (boslukKontrol(tbMarkaAdi.Text))
            {
                giris.baglantiAc();
                markaEkle();
                giris.Baglanti.Close();
            }
        }

        private void btnModelEkle_Click(object sender, EventArgs e)
        {
            if (boslukKontrol(tbModelAdi.Text))
            {
                giris.baglantiAc();
                modelEkle();
                giris.Baglanti.Close();
            }
        }

        private void btnKasaTipiSil_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            kasaSil();
            giris.Baglanti.Close();
        }

        private void btnRenkEkle_Click(object sender, EventArgs e)
        {
            if (boslukKontrol(tbRenkAdi.Text))
            {
                giris.baglantiAc();
                renkEkle();
                giris.Baglanti.Close();
            }
        }

        private void btnTurAdi_Click(object sender, EventArgs e)
        {
            if (boslukKontrol(tbTurAdi.Text))
            {
                giris.baglantiAc();
                turEkle();
                giris.Baglanti.Close();
            }
        }

        private void btnKasaTipiEkle_Click(object sender, EventArgs e)
        {
            if (boslukKontrol(tbKasaTipiAdi.Text))
            {
                giris.baglantiAc();
                KasaTipiEkle();
                giris.Baglanti.Close();
            }
        }

        private void btnTurSil_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            turSil();
            giris.Baglanti.Close();
        }

        private void btnModelSil_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            modelSil();
            giris.Baglanti.Close();
        }

        private void btnRenkSil_Click(object sender, EventArgs e)
        {
            giris.baglantiAc();
            renkSil();
            giris.Baglanti.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
