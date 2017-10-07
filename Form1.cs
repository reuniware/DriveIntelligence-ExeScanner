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

namespace DriveIntelligence
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                string path = d.Name;
                //TreeScan("C:\\");
                checkedListBox1.Items.Add(path, CheckState.Checked);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            nbfiles = 0;
            fileList.Clear();

            //TreeScan("F:\\AUDIO");

            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                string path = checkedListBox1.CheckedItems[i].ToString();
                log("Scanning path = " + path);
                TreeScan(path);
            }
            long milliseconds2 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            log("Scanning Done : " + nbfiles + " files in " + (milliseconds2-milliseconds) + " ms.");

            for (int i = 0; i < fileList.Count(); i++)
            {
                string f = fileList[i];
                if (f.ToLower().EndsWith(".exe"))
                {
                    //log(f);
                    try
                    {
                        long len = 0;
                        FileStream fs = File.Open(f, FileMode.Open);
                        long length = new System.IO.FileInfo(f).Length;
                        string fname = new System.IO.FileInfo(f).Name;

                        Byte[] byteArray = new Byte[length];
                        long indexByteArray = 0;

                        while (true)
                        {
                            int ii = fs.ReadByte();
                            if (ii == -1)
                            {
                                log(fname + " : End of file ; len = " + len + " " + length);
                                break;
                            } else
                            {
                                len++;
                                byteArray[indexByteArray] = (Byte)ii;
                                indexByteArray++;
                            }
                        }
                        fs.Close();

                        try
                        {
                            // Copie du fichier vers D:\TMP
                            /*FileStream ff = new FileStream("D:\\tmp\\" + fname, FileMode.CreateNew);
                            for (int jj = 0; jj < length; jj++)
                            {
                                ff.WriteByte(byteArray[jj]);
                            }
                            ff.Close();*/

                        } catch(Exception ex)
                        {
                            log(fname + " : Duplication exception : " + ex.Message);
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        //log(f + " : UnauthorizedAccessException");
                    }

                }

            }
        }

        private void log(string str)
        {
            listBox1.Items.Add(str);
        }

        int nbfiles = 0;
        List<String> fileList = new List<string>();

        private void TreeScan(string sDir)
        {
            bool authorized = true;
            try
            {
                Directory.GetFiles(sDir);
            }
            catch (UnauthorizedAccessException)
            {
                authorized = false;
                //log("Unauthorized : " + sDir);
            }

            if (authorized == false)
            {
                return;
            }

            string[] files = Directory.GetFiles(sDir);
            if (files.Count() == 0)
            {
                //log("no files in folder : " + sDir);
                return;
            }

            foreach (string f in Directory.GetFiles(sDir))
            {
                nbfiles++;
                fileList.Add(f);
            }

            foreach (string d in Directory.GetDirectories(sDir))
                TreeScan(d); // recursive call to get files of directory
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
