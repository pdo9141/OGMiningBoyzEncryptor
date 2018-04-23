using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace OGMiningBoyzEncryptorApp
{
    public partial class Form1 : Form
    {
        // Symmetric Fields
        private const string initVector = "pemgail9uzpgzl88";
        private const int keysize = 256;

        public Form1()
        {
            InitializeComponent();
            cbAlgorithm.SelectedIndex = 0;
        }

        private static string EncryptRijndael(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        private static string DecryptRijndael(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public static string EncryptTripleDES(string plainText, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Encoding.UTF8.GetBytes(plainText);

            string encoded = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }

        public static string DecryptTripleDES(string cipherText, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Convert.FromBase64String(cipherText);

            string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            var password = tbPassword.Text;
            var text = tbText.Text;
            var algorithm = cbAlgorithm.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Input a password bro");
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Input something to encrypt fool");
                return;
            }

            var encryptedText = algorithm.Equals("Rijndael-256") ? EncryptRijndael(text, password) : EncryptTripleDES(text, password);
            tbText.Text = encryptedText;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            var password = tbPassword.Text;
            var text = tbText.Text;
            var algorithm = cbAlgorithm.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("You need a password to decrypt sucka");
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Input something to decrypt ya bum");
                return;
            }

            var decryptedText = algorithm.Equals("Rijndael-256") ? DecryptRijndael(text, password) : DecryptTripleDES(text, password);
            tbText.Text = decryptedText;
        }
    }
}
