using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Utils
{
    public class EncryptionHelper
    {
        private static char[] keySep = { '=' };
        private static char[] valSep = { '&' };

        public static int CreateRandomSalt()
        {
            return new Random().Next();
        }

        public static string ComputeSaltedHash(string password, int salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] input = Encoding.Default.GetBytes(password + salt.ToString());
            byte[] output = algorithm.ComputeHash(input);
            return Convert.ToBase64String(output);
        }

        public static string CreateRandomPassword(int length)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            return new String(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string Encrypt(string data, string key)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(data);

            keyArray = UTF8Encoding.UTF8.GetBytes(key);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string data, string key)
        {
            // encoding issue
            data = data.Replace(" ", "+");

            byte[] keyArray;
            try
            {
                Convert.FromBase64String(data);
            }
            catch(Exception)
            {
                Utils.EventViewer.Writte("Castle Club", "BASE64", "data: " + data, System.Diagnostics.EventLogEntryType.Information);
            }
            byte[] toEncryptArray = Convert.FromBase64String(data);

            keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.Zeros;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray).Replace("\0", "");
        }

        public static String Pack(Hashtable hashData)
        {
            string pack = String.Empty;
            IDictionaryEnumerator _en = hashData.GetEnumerator();
            while (_en.MoveNext())
            {
                pack += _en.Key.ToString() + keySep[0] + _en.Value.ToString() + valSep[0];
            }
            if (pack.EndsWith("&")) { pack = pack.Substring(0, (pack.Length - 1)); }
            return pack;
        }

        public static Hashtable Unpack(String Data)
        {
            string[] unpack = Data.Split(valSep[0]);
            Hashtable hash = new Hashtable(unpack.Length);
            foreach (string valpair in unpack)
            {
                string[] keyval = valpair.Split(keySep[0]);
                if (keyval.Length == 2)
                {
                    if (hash[keyval[0]] == null)
                    {
                        hash.Add(keyval[0], keyval[1]);
                    }
                }
            }
            return hash;
        }

        public static string EncryptRSAXML(string text)
        {
            using(System.Security.Cryptography.RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024))
            {
                string xmlFile = new System.IO.StreamReader(CastleClub.BusinessLogic.Data.GlobalParameters.PublicEncrypt).ReadToEnd();

                rsa.FromXmlString(xmlFile);
                byte[] encryptData = rsa.Encrypt(Encoding.ASCII.GetBytes(text), false);
                return Encoding.ASCII.GetString(encryptData);
            }
        }

        public static string EncryptRSACertificate(string text)
        {
            X509Certificate2 certificate = new X509Certificate2(CastleClub.BusinessLogic.Data.GlobalParameters.Certificate);
            RSACryptoServiceProvider rsa = certificate.PublicKey.Key as RSACryptoServiceProvider;

            byte[] textEncrypt = rsa.Encrypt(Encoding.ASCII.GetBytes(text), true);

            return Convert.ToBase64String(textEncrypt);
        }
    }
}
