using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CombineFiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox1.Text = dialog.FileName;
            }
        }


        private void CombineFilesInFolder(string path)
        {
            var files = Directory.GetFiles(path);
            if(files.Length == 0)
            {
                MessageBox.Show("No files in directory.");
                return;
            }

            files.OrderBy(o => o).ToList();

            var nameFirst =
                H.GetUntilOrEmpty(
                    Path.GetFileName(
                        files.First()));

            var nameLast =
                H.GetAfter(
                    Path.GetFileName(
                        files.Last()));

            var resultName = H.AvoidDuplicates($@"{Path.GetDirectoryName(files.First())}\{nameFirst}{nameLast}");

            using (var outputStream = File.Create(resultName))
            {
                foreach (var inputFilePath in files)
                {
                    using (var inputStream = File.OpenRead(inputFilePath))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(!Directory.Exists(textBox1.Text))
            {
                MessageBox.Show($"Directory {textBox1.Text} not found.");
                return;
            }
            CombineFilesInFolder(textBox1.Text);
            MessageBox.Show("Completed");
        }
    }

    public static class H
    {
        public static string GetUntilOrEmpty(this string text, string stopAt = "--")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        public static string GetAfter(this string text, string stopAt = "--")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(charLocation, text.Length - charLocation);
                }
            }

            return String.Empty;
        }


        public static string AvoidDuplicates(string desiredName)
        {
            FileInfo fileInfo = new FileInfo(desiredName);
            string nameNoExtension = Path.GetFileNameWithoutExtension(desiredName);
            string extension = fileInfo.Extension;
            string dir = fileInfo.DirectoryName;
            if (File.Exists(desiredName))
            {
                int copies = 2;
                while (File.Exists(string.Format(@"{0}\{1}({2}){3}", dir, nameNoExtension, copies, extension)))
                {
                    copies++;
                }
                return string.Format(@"{0}\{1}({2}){3}", dir, nameNoExtension, copies, extension);
            }
            else
            {
                return desiredName;
            }
        }
    }

    

}
