using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.ApplicationServices;
using Org.BouncyCastle.X509;

namespace KRZ_Projekat
{
    public partial class Prijava : Form
    {
        public static Prijava instanca;

        public Prijava()
        {
            InitializeComponent();
            instanca = this;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = Path.GetFullPath(openFileDialog1.FileName);
                label2.Visible = false;
                textBox2.Visible = false;
                label4.Visible = false;
                textBox3.Visible = false;
                button3.Visible = false;
            }
            else
                MessageBox.Show("Molimo izaberite file!");
        }

        private bool CheckCert(string certPath)
        {
            try
            {
                X509CertificateParser parser = new X509CertificateParser();
                X509Certificate caCert = parser.ReadCertificate(File.OpenRead(@"C:\Users\Administrator\Desktop\FILES\root\ca.crt"));
                X509Certificate certToCheck = parser.ReadCertificate(File.OpenRead(certPath));
                certToCheck.Verify(caCert.GetPublicKey());
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Sertifikat nije validan / Pogresan format fajla!");
                return false;
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            Registracija.instanca.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text;
            if (CheckCert(path))
            {
                label2.Visible = true;
                textBox2.Visible = true;
                label4.Visible = true;
                textBox3.Visible = true;
                button3.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string korisnik = textBox2.Text;
            GlavniMeni.imeKorisnika = korisnik;
            string lozinka= textBox3.Text;
            lozinka = Registracija.passwordEncrypt(lozinka, 15);
            string putanjaDoFajla = @"C:\Users\Administrator\Desktop\FILES\root\korisnici.txt";
            try
            {
                if (!File.Exists(putanjaDoFajla))
                    MessageBox.Show("Datoteka korisnici.txt ne postoji");
                string[] linije = File.ReadAllLines(putanjaDoFajla);
                foreach (string linija in linije)
                {
                    string[] podaci = linija.Split('-');
                    if (podaci[0] == korisnik && podaci[1]== lozinka) //ako postoji preusmjerava dalje na GlavniMeni
                    {
                        GlavniMeni gm = new GlavniMeni();
                        gm.Show();
                        instanca.Hide();
                        return;
                    }
                }
                MessageBox.Show("Pogresni podaci!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške prilikom provjere korisnika: " + ex.Message);
                
            }

        }
    }
}
