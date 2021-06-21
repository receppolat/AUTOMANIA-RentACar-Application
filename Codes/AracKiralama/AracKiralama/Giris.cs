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
    public partial class giris : Form
    {
        public giris()
        {
            InitializeComponent();
        }

        public static SqlConnection Baglanti = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AracKiralama;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False");
        public static int adminMi = -1;

        public static void baglantiAc()
        {
            try
            {
                if (Baglanti.State == ConnectionState.Closed)
                    Baglanti.Open();
                
            }
            catch(Exception hata)
            {
                MessageBox.Show(hata.Message,"Hata Mesajı",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void veriCek()
        {
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter("Select PersonelID, PersonelTC from personel", Baglanti);
            SqlDataAdapter daAdmin = new SqlDataAdapter("Select OfisAdi, Parola from rentACar", Baglanti);
            daAdmin.Fill(ds, "Admin");
            da.Fill(ds,"Personel");
            if(ds.Tables["Admin"].Rows[0]["OfisAdi"].ToString() == tbID.Text && ds.Tables["Admin"].Rows[0]["Parola"].ToString() == tbParola.Text)
            {
                adminMi = 1;
                AnaSayfa anaSayfa = new AnaSayfa();
                anaSayfa.ShowDialog();
            }
            else
            { 
                if(ds.Tables["Personel"].Rows.Count > 0)
                    for (int i = 0; i < ds.Tables["Personel"].Rows.Count; i++)
                        if (ds.Tables["Personel"].Rows[i]["PersonelID"].ToString() == tbID.Text && ds.Tables["Personel"].Rows[i]["PersonelTC"].ToString() == tbParola.Text)
                        {
                            adminMi = 0;
                            AnaSayfa anaSayfa = new AnaSayfa();
                            anaSayfa.ShowDialog();
                        }
            }
            if(adminMi == -1)
                MessageBox.Show("'Kullanıcı adı' veya 'Parola' hatalıdır.","Hata Mesajı",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        private void giris_Load(object sender, EventArgs e)
        {
            
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            baglantiAc();
            veriCek();
            Baglanti.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
