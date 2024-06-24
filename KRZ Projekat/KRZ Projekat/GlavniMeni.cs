using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Asn1.X9;
using System.Security.Cryptography;



namespace KRZ_Projekat
{
    public partial class GlavniMeni : Form
    {
        static AsymmetricKeyParameter privateKey, publicKey;
        public static GlavniMeni instanca;
        public static string imeKorisnika;
        bool clicked = false;
        public GlavniMeni()
        {
            InitializeComponent();
            comboBox1.Items.Add("RailFence Cypher");
            comboBox1.Items.Add("Myszkowski");
            comboBox1.Items.Add("Playfair");
            label3.Text = "Korisnik: " + imeKorisnika;
            instanca = this;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            prikazKljuca();
            string fp = $@"C:\Users\Administrator\Desktop\FILES\root\istorija\{imeKorisnika}.txt";

           
            Prijava(filePath, encryptedFilePath, hashFilePath);
        }


        void prikazKljuca()
        {
            string putanja = $@"C:\Users\Administrator\Desktop\FILES\izlaz\{imeKorisnika}.key";
            string putanja2 = $@"C:\Users\Administrator\Desktop\FILES\root\privatniKljuc\{imeKorisnika}.key";
            File.Copy(putanja, putanja2, true);
            label7.Text = "Privatni kljuc: " + imeKorisnika + ".key" + "\n" + putanja2;
        }

        private void playfairMatrix(IList<char> key, Dictionary<char, string> chPosMatrix, Dictionary<string, char> posCharMatrix)
        {
            Dictionary<char, int> alphabet;
            alphabet = new Dictionary<char, int>();
            char c = 'a';
            alphabet.Add(c, 0);
            for (int i = 1; i <= 25; i++)
                alphabet.Add(++c, i);

            char[,] matrix = new char[5, 5];
            int keyPosition = 0, charPosition = 0;
            List<char> alphabetPF = alphabet.Keys.ToList();
            alphabetPF.Remove('j');

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (charPosition < key.Count)
                    {
                        matrix[i, j] = key[charPosition]; 
                        alphabetPF.Remove(key[charPosition]);
                        charPosition++;
                    }
                    else 
                    {
                        matrix[i, j] = alphabetPF[keyPosition];
                        keyPosition++;
                    }
                    string position = i.ToString() + j.ToString();
                    chPosMatrix.Add(matrix[i, j], position);
                    posCharMatrix.Add(position, matrix[i, j]);
                }
            }
        }

        private int setPos(int number)
        {
            if (number < 0) number += 5;
            return number;
        }

        private string Playfair(string text, string key)
        {
            Dictionary<char, string> chPosMatrix = new Dictionary<char, string>();
            Dictionary<string, char> posCharMatrix = new Dictionary<string, char>();
            playfairMatrix(key.Distinct().ToArray(), chPosMatrix, posCharMatrix);

            string trimmed = text.Replace(" ", "");
            string message = "";
            for (int i = 0; i < trimmed.Length; i++)
            {
                message += trimmed[i];
                if (i < trimmed.Length - 1 && text[i] == text[i + 1])
                     message += 'x';
                
            }
            if (message.Length % 2 != 0)
                message += 'x';
            
            string res = "";

            for (int i = 0; i < message.Length; i += 2)
            {
                string substring_of_2 = message.Substring(i, 2);
                string rc1 = chPosMatrix[substring_of_2[0]];
                string rc2 = chPosMatrix[substring_of_2[1]];

                if (rc1[0] == rc2[0])  //isti red, razlicita kolona
                {
                    int newC1 = 0, newC2 = 0;
                    newC1 = (int.Parse(rc1[1].ToString()) + 1) % 5;
                    newC2 = (int.Parse(rc2[1].ToString()) + 1) % 5;
                    newC1 = setPos(newC1);
                    newC2 = setPos(newC2);
                    res += posCharMatrix[rc1[0].ToString() + newC1.ToString()];
                    res += posCharMatrix[rc2[0].ToString() + newC2.ToString()];
                }
                else if (rc1[1] == rc2[1])  //ista kolona, razliciti red
                {
                    int newR1 = 0, newR2 = 0;
                    newR1 = (int.Parse(rc1[0].ToString()) + 1) % 5;
                    newR2 = (int.Parse(rc2[0].ToString()) + 1) % 5;
                    newR1 = setPos(newR1);
                    newR2 = setPos(newR2);
                    res += posCharMatrix[newR1.ToString() + rc1[1].ToString()];
                    res += posCharMatrix[newR2.ToString() + rc2[1].ToString()];
                }
                else  //razliciti redovi i kolone
                {
                    res += posCharMatrix[rc1[0].ToString() + rc2[1].ToString()];
                    res += posCharMatrix[rc2[0].ToString() + rc1[1].ToString()];
                }
            }
            return res;
        }

        private string railfence(string text, int rail)
        {
            char[,] fence = new char[rail, text.Length];
            bool down = false;
            int row = 0, col = 0;
            foreach (char c in text)
            {
                if (row == 0 || row == rail - 1)
                    down = !down;
                fence[row, col++] = c;
                if (down) row++;
                  else row--;
            }
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < rail; i++)
            {
                for (int j = 0; j < text.Length; j++)
                {
                    if (fence[i, j] != '\0') //ako pozicija nije popunjena
                        res.Append(fence[i, j]);
                }
            }
            return res.ToString();
        }

        public static string Myszkowski(string text, string keyword)
        {
            int keywordLength = keyword.Length;
            int textLength = text.Length;
            int rows = textLength / keywordLength + (textLength % keywordLength == 0 ? 0 : 1);
            char[,] grid = new char[rows, keywordLength];
            int textIndex = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < keywordLength; j++)
                {
                    if (textIndex < textLength)
                        grid[i, j] = text[textIndex++];
                    else
                        grid[i, j] = ' ';
                }
            }
            StringBuilder res = new StringBuilder();
            foreach (char c in keyword.OrderBy(c => c))
            {
                int column = keyword.IndexOf(c);
                for (int i = 0; i < rows; i++)
                {
                    res.Append(grid[i, column]);
                }
            }
            return res.ToString().Replace(" ", "");
        }


        private void upisiTekst(string korisnik, string text, string key, string algo, string sifrat)
        {
            string putanja = $@"C:\Users\Administrator\Desktop\FILES\root\istorija\{korisnik}.txt";
            string vrijemeIzvrsavanja = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                using (StreamWriter writer = new StreamWriter(putanja, true))
                {
                    writer.WriteLine("Vrijeme izvrsavanja: " + vrijemeIzvrsavanja);
                    writer.WriteLine("Algoritam: " + algo);
                    writer.WriteLine("Kljuc: " + key);
                    writer.WriteLine("Tekst: " + text);
                    writer.WriteLine("Sifrat: " + sifrat); 
                    writer.WriteLine("=================");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Greska prilikom upisa!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tekst = textBox2.Text;
            string kljuc = textBox1.Text;
            string rez = "";
            textBox3.Clear();
            switch (comboBox1.SelectedItem.ToString())
            {
                case "RailFence Cypher":
                    rez = railfence(tekst = tekst.Replace(" ", ""), Convert.ToInt32(kljuc));
                    upisiTekst(imeKorisnika, tekst, kljuc, "RailFence Cypher", rez);
                    break;
                case "Myszkowski":
                    rez = Myszkowski(tekst = tekst.Replace(" ", ""), kljuc);
                    upisiTekst(imeKorisnika, tekst, kljuc, "Myszkowski", rez);
                    break;
                case "Playfair":
                    rez = Playfair(tekst, kljuc);
                    upisiTekst(imeKorisnika, tekst, kljuc, "Playfair", rez);
                    break;
                default:
                    MessageBox.Show("Unsupported algorithm!");
                    break;
            }
            textBox3.Text += rez;
            clicked = true;
        }


        //Prikaz istorije
        private void button2_Click(object sender, EventArgs e)
        {
            /*
            string decryptedContent = DecryptFile(encryptedFilePath);
            string putanja = $@"C:\Users\Administrator\Desktop\FILES\root\istorija\{imeKorisnika}.txt";
            File.WriteAllText(putanja, decryptedContent);
            */

            Process.Start("notepad.exe", filePath);
        }


        //ZA ODJAVU
        private void button3_Click(object sender, EventArgs e)
        {
            Odjava(filePath, encryptedFilePath, hashFilePath);
            Registracija.instanca.Show();
            this.Hide();
        }


        //SISTEM ZA ISTORIJU .txt
        string filePath = $@"C:\Users\Administrator\Desktop\FILES\root\istorija\{imeKorisnika}.txt";
        string encryptedFilePath = $@"C:\Users\Administrator\Desktop\FILES\root\istorija\{imeKorisnika}_encrypted.txt";
        string hashFilePath = $@"C:\Users\Administrator\Desktop\FILES\root\istorija\{imeKorisnika}_hash.txt";
        private static readonly byte[] key = Encoding.UTF8.GetBytes("OvoJeVrloJakaLozinka123456789012"); // 32 bajta                                                                                              // Ensure IV is 16 bytes for AES
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("OvoJeIV123456789"); // 16 bajtova 
        //IV: isti blok plaintext-a šifrovan istim ključem daje isti blok ciphertext-a
        static void Prijava(string filePath, string encryptedFilePath, string hashFilePath)
        {
            if (CheckFileIntegrity(encryptedFilePath, hashFilePath))
            {
                string decryptedContent = DecryptFile(encryptedFilePath);
                File.WriteAllText(filePath, decryptedContent);
            }
            else
            {
                MessageBox.Show("UPOZORENJE: Datoteka je izmjenjena!!!");
                File.WriteAllText(filePath, string.Empty);
            }
        }

        static void Odjava(string filePath, string encryptedFilePath, string hashFilePath)
        {
            EncryptFile(filePath, encryptedFilePath);
            string newHash = GenerateFileHash(encryptedFilePath);
            File.WriteAllText(hashFilePath, newHash);
        }

        static void EncryptFile(string inputFile, string outputFile)
        {
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream fsEncrypted = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                //cipher block chaining, svaki blok plaintext-a se XOR-uje sa prethodnim blokom ciphertext-a pre nego što se šifruje
                //isti blok plaintext-a neće biti šifrovan na isti način, čak i ako se pojavljuje više puta.
                aes.Padding = PaddingMode.PKCS7;
                //popunjavanje poslednjeg bloka plaintext-a kada njegova dužina nije dovoljna da popuni cijeli blok. 

                using (CryptoStream cryptoStream = new CryptoStream(fsEncrypted, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        static string DecryptFile(string inputFile)
        {
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC; 
                aes.Padding = PaddingMode.PKCS7;
                using (CryptoStream cryptoStream = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader reader = new StreamReader(cryptoStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        static string GenerateFileHash(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = sha256.ComputeHash(fs);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        static bool CheckFileIntegrity(string filePath, string hashFilePath)
        {
            string currentHash = GenerateFileHash(filePath);
            string originalHash = File.ReadAllText(hashFilePath);
            return originalHash == currentHash;
        }
    }

}

