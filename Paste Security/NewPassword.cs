using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderLocker
{
    public partial class NewPassword : Form
    {
        private String appdata, dir;
        string pass;
        public NewPassword()
        {
            InitializeComponent();
            appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dir = Path.Combine(appdata, @"Security\psw.txt");
        }

        private bool verifPass()
        {
            if (txtNew.Text.Trim().Length>0 && txtConfirm.Text.Trim().Length>0)
            {
                if (txtNew.Text.Equals(txtConfirm.Text))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Passwords aren't Equals!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtConfirm.Text = String.Empty;
                    txtNew.Text = String.Empty;
                    txtNew.Focus();
                    return false;
                }
            }
            else
            {               
                MessageBox.Show("Type the Password!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirm.Text = String.Empty;
                txtNew.Text = String.Empty;
                txtNew.Focus();
                return false;
            }          
        }

        private void writePass(string password)
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(dir))
            {
                sw.WriteLine(password);
            }
        }

        private void txtKey_Down(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                if (verifPass())
                {
                    writePass(setPass(txtConfirm.Text));
                    DialogResult = DialogResult.OK;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private string setPass(string password)
        {
            string hash = "f0xle@rn";
            string hashPass;
            byte[] data = Encoding.UTF8.GetBytes(password);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider triDes = new TripleDESCryptoServiceProvider()
                { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = triDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    hashPass = Convert.ToBase64String(results, 0, results.Length);
                }
            }
            return hashPass;
        }
    }
}
