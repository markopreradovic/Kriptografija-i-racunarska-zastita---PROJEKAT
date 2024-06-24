using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;

namespace KRZ_Projekat
{
    public partial class Registracija : Form
    {
        public static Registracija instanca;

        public Registracija()
        {
            InitializeComponent();
            instanca = this;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; 
            this.MaximizeBox = false; 
            this.MinimizeBox = false;
        }

        public static void CreateUserCertificate(string caCertPath, string caPKPath, string username, string email)
        {
            try
            {
                // ucitavanje CA 
                X509CertificateParser parser = new X509CertificateParser();
                X509Certificate caCert = parser.ReadCertificate(File.OpenRead(caCertPath));
                AsymmetricCipherKeyPair caKeyPair = GetKeyPair(caPKPath);

                // generise RSA
                RsaKeyPairGenerator generator = new RsaKeyPairGenerator();
                generator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
                AsymmetricCipherKeyPair userKeyPair = generator.GenerateKeyPair();

                // generise zahtjev
                X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();
                certGen.SetSerialNumber(BigInteger.ProbablePrime(120, new Random()));
                certGen.SetIssuerDN(new X509Name(caCert.SubjectDN.ToString()));
                certGen.SetNotBefore(DateTime.UtcNow.Date);
                certGen.SetNotAfter(DateTime.UtcNow.Date.AddYears(2));
                certGen.SetSubjectDN(new X509Name("CN=" + username + ", EMAILADDRESS=" + email));
                certGen.SetPublicKey(userKeyPair.Public);

                // dodavanje identifikacionih podataka
                certGen.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));
                certGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(caCert.GetPublicKey()));
                certGen.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(userKeyPair.Public));
                certGen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment));

                // potpis
                ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", caKeyPair.Private);

                // generise korisnicki sertifikat
                X509Certificate userCert1 = certGen.Generate(signatureFactory);
                X509Certificate userCert2 = certGen.Generate(signatureFactory);

                // cuvanje u datoteku
                string outputPath = @"C:\Users\Administrator\Desktop\FILES\izlaz\";
                string issuedCertPath = @"C:\Users\Administrator\Desktop\FILES\root\izdatiSertifikati\";

                string userCertFileName = $"{username}.crt";
                string userKeyFileName = $"{username}.key";

                // Sertifikat za korisnika
                using (var fs = File.Create(Path.Combine(outputPath, userCertFileName)))
                {
                    byte[] certBytes = userCert1.GetEncoded();
                    fs.Write(certBytes, 0, certBytes.Length);
                }

                // Sertifikat za bazu
                using (var fs = File.Create(Path.Combine(issuedCertPath, userCertFileName)))
                {
                    byte[] certBytes = userCert2.GetEncoded();
                    fs.Write(certBytes, 0, certBytes.Length);
                }

                // Privatni kljuc
                using (TextWriter tw = new StreamWriter(Path.Combine(outputPath, userKeyFileName)))
                {
                    PrivateKeyInfo pkInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(userKeyPair.Private);
                    byte[] pkBytes = pkInfo.ToAsn1Object().GetEncoded();
                    tw.Write(Convert.ToBase64String(pkBytes));
                }

                string userPublicKeyFileName = $"{username}Public.key";
                using (TextWriter tw = new StreamWriter(Path.Combine(outputPath, userPublicKeyFileName)))
                {
                    SubjectPublicKeyInfo pkInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(userKeyPair.Public);
                    byte[] pkBytes = pkInfo.ToAsn1Object().GetEncoded();
                    tw.Write(Convert.ToBase64String(pkBytes));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Doslo je do greske prilikom kreiranja korisnickog sertifikata: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static AsymmetricCipherKeyPair GetKeyPair(string pkPath)
        {
            AsymmetricCipherKeyPair keyPair;
            using (StreamReader reader = File.OpenText(pkPath))
            {
                PemReader pR = new PemReader(reader);
                RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)pR.ReadObject();
                RsaKeyParameters publicKey = new RsaKeyParameters(false, privateKey.Modulus, privateKey.PublicExponent);
                keyPair = new AsymmetricCipherKeyPair(publicKey, privateKey);
            }
            return keyPair;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string korisnik, lozinka, email;
            korisnik = textBox1.Text;
            email = textBox2.Text;
            lozinka = textBox3.Text;
            if (upisKorisnika(korisnik, lozinka) && !string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox3.Text))
            {
                string caCertPath = "C:\\Users\\Administrator\\Desktop\\FILES\\root\\ca.crt";
                string caPKPath = "C:\\Users\\Administrator\\Desktop\\FILES\\root\\ca.key";
                string userCert = "C:\\Users\\Administrator\\Desktop\\FILES\\root\\izdatiSertifikati\\novi.crt";
                CreateUserCertificate(caCertPath, caPKPath, korisnik, email);


                string encryptedFilePath = $@"C:\Users\Administrator\Desktop\FILES\root\istorija\{korisnik}_encrypted.txt";
                string hashFilePath = $@"C:\Users\Administrator\Desktop\FILES\root\istorija\{korisnik}_hash.txt";
                File.Create(encryptedFilePath);
                File.Create(hashFilePath);

                string message = $"Uspjesno generisan korisnicki certifikat i kljuc!\n\n" +
                             $"Korisnik: {korisnik}\n" +
                             $"Email: {email}\n" +
                             $"Lokacija certifikata: {userCert.Replace("novi", korisnik)}\n" +
                             $"Lokacija kljuca: {userCert.Replace("novi.crt", korisnik + ".key")}";

                MessageBox.Show(message, "Informacija", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("Unesite podatke!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            string caCertPath = "C:\\Users\\Administrator\\Desktop\\FILES\\root\\ca.crt";
            string caPKPath = "C:\\Users\\Administrator\\Desktop\\FILES\\root\\ca.key";
            string crlPath = "C:\\Users\\Administrator\\Desktop\\FILES\\root\\crl.pem";
            GenerateCRL(caCertPath, caPKPath, crlPath);
            */
            Prijava pr = new Prijava();
            pr.Show();
            instanca.Hide();
        }


        public static string passwordEncrypt(string input, int shift)
        {
            string result = "";
            foreach (char ch in input)
            {
                if (char.IsLetter(ch))
                {
                    char shifted = (char)(ch + shift);
                    if ((char.IsLower(ch) && shifted > 'z') || (char.IsUpper(ch) && shifted > 'Z'))
                    { 
                        shifted = (char)(ch - (26 - shift));
                    }
                    result += shifted;
                }
                else
                {
                    result += ch;
                }
            }
            return result;
        }

        public static string passwordDecrypt(string input, int shift)
        {
            return passwordEncrypt(input, -shift);
        }

        public static bool upisKorisnika(string imeKorisnika, string lozinka)
        {
            string putanja = @"C:\Users\Administrator\Desktop\FILES\root\korisnici.txt";
            if (!File.Exists(putanja))
            {
                 MessageBox.Show("Datoteka korisnici.txt ne postoji");
                 return false;
            }
                
            string[] linije = File.ReadAllLines(putanja);
            foreach (string linija in linije)
            {
               if (linija.StartsWith(imeKorisnika + "-"))
               {
                        MessageBox.Show("Korisnik vec postoji");
                        return false;
               }
            } 
            using (StreamWriter writer = File.AppendText(putanja))
            {
                string lozinkaEnc = passwordEncrypt(lozinka, 15);
                writer.WriteLine($"{imeKorisnika}-{lozinkaEnc}");
            }
            
            return true;
        }

        public static void GenerateCRL(string caCertPath, string caKeyPath, string crlPath)
        {
            X509CertificateParser parser = new X509CertificateParser();
            X509Certificate caCert = parser.ReadCertificate(File.ReadAllBytes(caCertPath));
            AsymmetricKeyParameter caPrivateKey = readPrivate(caKeyPath);
            X509V2CrlGenerator crlGen = new X509V2CrlGenerator();
            crlGen.SetIssuerDN(caCert.SubjectDN);
            crlGen.SetThisUpdate(DateTime.Now);
            crlGen.SetNextUpdate(DateTime.Now.AddYears(1));
            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", caPrivateKey);
            X509Crl crl = crlGen.Generate(signatureFactory);
            File.WriteAllBytes(crlPath, crl.GetEncoded());
        }

        private static AsymmetricKeyParameter readPrivate(string privateKeyPath)
        {
            using (StreamReader reader = File.OpenText(privateKeyPath))
            {
                PemReader pemReader = new PemReader(reader);
                object obj = pemReader.ReadObject();
                if (obj is AsymmetricKeyParameter)
                {
                    return (AsymmetricKeyParameter)obj;
                }
                throw new InvalidOperationException("Nije moguce ucitati privatni kljuc.");
            }
        }

    }

}


