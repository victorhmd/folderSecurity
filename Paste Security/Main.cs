using FolderLocker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paste_Security
{
    public partial class Form1 : Form
    {
        private String appdata, dir, dir2;
        public Form1()
        {
            InitializeComponent();

            appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dir = Path.Combine(appdata, @"Security\Private");
            dir2 = Path.Combine(appdata, @"Security\psw.txt");
            check();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {           
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                passCheck(textBox1.Text);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }           
        }

        private void passCheck(string password)
        {
            if (textBox1.Text.Trim().Length > 0)
            {
                string hashPass = String.Empty;
                using (StreamReader sr = File.OpenText(dir2))
                {
                    string s;
                    if ((s = sr.ReadLine()) != null)
                    {
                        hashPass = getPass(s);
                    }
                }

                if (password.Equals(hashPass))
                {
                    Process prc = new Process();
                    prc.StartInfo.FileName = dir;
                    prc.Start();
                    Close();
                }
                else
                {
                    MessageBox.Show("Wrong Password", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Text = String.Empty;
                    textBox1.Focus();
                }
            }
            else
            {
                MessageBox.Show("Type the Password!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
            }
        }

        private void check()
        {
            if (!Directory.Exists(dir))
            {
                DirectoryInfo info = Directory.CreateDirectory(dir);
                info.Attributes = FileAttributes.Hidden;                
            }

            if (!File.Exists(dir2))
            {
                NewPassword form = new NewPassword();
                if (form.ShowDialog() == DialogResult.Cancel)
                {
                    MessageBox.Show("Couldn't Create the Password!\nClose the program and try again.",
                        "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }     

        private string getPass(string hashPass)
        {
            string hash = "f0xle@rn";
            string password;
            byte[] data = Convert.FromBase64String(hashPass);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider triDes = new TripleDESCryptoServiceProvider()
                { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = triDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    password = Encoding.UTF8.GetString(results);
                }
            }
            return password;
        }


        /*private void folderLocker()
        {
            // vendo se a pasta está invisível (bloqueada)
            DirectoryInfo info = Directory.GetParent(dir);
            if (info.Attributes.Equals(FileAttributes.Normal))
            {
                // confirmação de bloqueio
                DialogResult msg = MessageBox.Show("Lock the Folder?", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (msg == DialogResult.Yes)
                {
                    // aceitou o bloqueio
                    info.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                    MessageBox.Show("Folder Locked!", "Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Close();
                }
            }
        }*/


    }
}
