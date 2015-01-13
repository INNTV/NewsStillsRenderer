using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NewsStillsRnd
{
    public partial class Form1 : Form
    {
        Configuration _config;
        public Form1()
        {
            InitializeComponent();

            _config = new Configuration();

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult Rs = MessageBox.Show("آیا از برنامه خارج می شوید", "خروج از برنامه",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Rs == System.Windows.Forms.DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        public void LogWriter(string Txt)
        {
            if (richTextBox1.Lines.Length > 500)
            {
                richTextBox1.Text = "";
            }
            richTextBox1.Text += Txt + "  [" + DateTime.Now.ToString() + "] \n";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            Application.DoEvents();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            timer1_Tick(null, null);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button1.ForeColor = Color.White;
            button1.Text = "Started";
            button1.BackColor = Color.Red;

            timer1_Tick(null, null);


            button1.ForeColor = Color.White;
            button1.Text = "Start";
            button1.BackColor = Color.Navy;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            MyDBTableAdapters.RENDERTableAdapter Ta = new MyDBTableAdapters.RENDERTableAdapter();
            MyDB.RENDERDataTable Dt;


            for (int i = 0; i < _config.SqlFilter.Count; i++)
            {
                Dt = Ta.selectTasks(_config.SqlFilter[i]);
                foreach (DataRow rw in Dt.Rows)
                {
                    //New task:
                    richTextBox1.Text = "";
                    LogWriter("Render task found: " + _config.SqlFilter[i]);

                    //Get images list:
                    if (CopyImages(i, int.Parse(rw["c_id"].ToString())))
                    {
                        if (Render(i))
                        {

                            try
                            {



                            MyDBTableAdapters.CONDUCTORTableAdapter Cond_Ta = new MyDBTableAdapters.CONDUCTORTableAdapter();

                            //create dest dir if not exist in san
                            string PlayOutFolderDate = string.Format("{0:0000}{1:00}{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            string PlayOutFolder = _config.PlayOutRoot + "\\" + PlayOutFolderDate;

                            if (!Directory.Exists(PlayOutFolder + "\\THUMB"))
                            {
                                Directory.CreateDirectory(PlayOutFolder + "\\THUMB");
                            }
                            if (!Directory.Exists(PlayOutFolder + "\\VIDEO"))
                            {
                                Directory.CreateDirectory(PlayOutFolder + "\\VIDEO");
                            }

                            string PlyFileName = Cond_Ta.InsertPly("Auto AE Renderer",
                                rw["c_id"].ToString(),
                                "1",
                                _config.VideoDuration[i],
                                "S:\\PLAYOUT\\" + PlayOutFolderDate,
                                "1",
                                "1"
                                ).ToString();

                            LogWriter("Start Copy Video:" + PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".avi");
                            File.Copy(_config.OutputPathFile[i], PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".avi");
                            LogWriter("End Copy Video:" + PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".avi");

                            LogWriter("Copy Thumbnail" + PlayOutFolder + "\\THUMB\\" + PlyFileName + ".jpg");
                            File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\P.jpg", PlayOutFolder + "\\THUMB\\" + PlyFileName + ".jpg");
                            LogWriter("End Copy Thumbnail:" + PlayOutFolder + "\\THUMB\\" + PlyFileName + ".jpg");


                            Cond_Ta.Update_FileName("playout01", PlyFileName, PlyFileName);
                            Cond_Ta.Update_Active("conductor01", rw["c_id"].ToString());
                            Ta.Update_Order(int.Parse(rw["id"].ToString()));

                            LogWriter("Task Done");

                            }
                            catch (Exception ex)
                            {
                                LogWriter(ex.Message);

                            }

                        }
                        else
                        {
                            LogWriter("Error in render: " + _config.SqlFilter[i]);
                        }
                    }
                    else
                    {
                        LogWriter("Error in copy image: " + _config.SqlFilter[i]);
                    }

                    break;
                }
            }

            //timer1.Enabled = true;
        }

        private bool CopyImages(int index, int conductorId)
        {
            try
            {
                string ImageRootDirectory = _config.ImageRoot[index];
                string ImageNameFormat = _config.ImageName[index];

                MyDBTableAdapters.SlideTableAdapter slideTa = new MyDBTableAdapters.SlideTableAdapter();
                MyDB.SlideDataTable SlideDt = slideTa.selectSlides(int.Parse(_config.ImageCount[index]), conductorId);

                try
                {
                    Directory.Delete(ImageRootDirectory, true);

                }
                catch { }
                Directory.CreateDirectory(ImageRootDirectory);

                if (SlideDt.Rows.Count ==int.Parse(_config.ImageCount[index]))
                {

                    for (int i = 0; i < SlideDt.Rows.Count; i++)
                    {
                        string FileName = ImageRootDirectory + ImageNameFormat + (i + 1).ToString() + ".png";
                        File.Copy(SlideDt.Rows[i]["FilePath"].ToString(), FileName, true);
                    }
                    return true;
                }
                else
                {
                    LogWriter("Error in image count: " + SlideDt.Rows.Count + " of " + _config.ImageCount[index]);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter(ex.Message);

                return false;
            }
        }
        protected bool Render(int index)
        {
            try
            {

                //Start Render
                RenderObject RndObj = new RenderObject();
                RndObj.AeRenderPath = _config.AeRenderExePath;
                RndObj.AeProjectPath = _config.AeProjectFile[index];
                RndObj.DestFullFileName = _config.OutputPathFile[index];
                RndObj.CompositionName = _config.Composition[index];
                try
                {
                    File.Delete(_config.OutputPathFile[index]);
                }
                catch { }

                StreamReader reader = Utilities.Renderer(RndObj);
                int Lngth = richTextBox1.Text.Length;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //richTextBox1.Text = richTextBox1.Text.Remove(Lngth, richTextBox1.Text.Length - Lngth);
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    Application.DoEvents();

                    richTextBox1.AppendText(line + " >> FROM : "+_config.Frame[index]+" FRAMES");
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    Application.DoEvents();
                }

               // richTextBox1.Text = richTextBox1.Text.Remove(Lngth, richTextBox1.Text.Length - Lngth);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();

                return true;
            }
            catch (Exception ex)
            {
                LogWriter(ex.Message);

                return false;
            }
        }

    }
}
