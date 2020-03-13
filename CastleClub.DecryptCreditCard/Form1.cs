using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CastleClub.Tool.DecryptCreditCard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select a *.pfx file for decrypt...";
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = "Private|*.pfx";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox3.Text))
            {
                try
                {
                    X509Certificate2 certificate = new X509Certificate2(textBox1.Text, textBox2.Text);
                    RSACryptoServiceProvider rsa = certificate.PrivateKey as RSACryptoServiceProvider;

                    string row = textBox3.Text.Replace(" ","+");

                    byte[] textDecrypt = rsa.Decrypt(Convert.FromBase64String(row), true);

                    textBox3.Text = System.Text.Encoding.UTF8.GetString(textDecrypt);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
