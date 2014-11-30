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
        public Form1()
        {
            InitializeComponent();
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
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            MyDBTableAdapters.RENDERTableAdapter Ta = new MyDBTableAdapters.RENDERTableAdapter();
            MyDB.RENDERDataTable Dt = Ta.selectTasks("aksekhabari");

            if (Dt.Rows.Count == 1)
            {
                richTextBox1.Text = "";


                LogWriter("Render task found");
                //New task:

                //Get images list:
                int imagesCount = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["imagecount"].Trim());
                string imagesRootDirectory = System.Configuration.ConfigurationSettings.AppSettings["footagedirectory"].Trim();

                MyDBTableAdapters.SlideTableAdapter slideTa = new MyDBTableAdapters.SlideTableAdapter();
                MyDB.SlideDataTable slideDt = slideTa.selectSlides(imagesCount, int.Parse(Dt.Rows[0]["c_id"].ToString()));
                if (imagesCount == slideDt.Rows.Count)
                {
                    //All expected images are exist:
                    for (int i = 0; i < slideDt.Rows.Count; i++)
                    {
                        //Replace network drive with network path:

                        //Copy images to local folder:

                        Image imgSrc = Image.FromFile(slideDt.Rows[i]["FilePath"].ToString());
                        imgSrc.Save(imagesRootDirectory + "Image " + (i + 1).ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        imgSrc.Dispose();
                    }



                    //Start render video:
                    Render();

                    //Insert playout:
                    MyDBTableAdapters.CONDUCTORTableAdapter Cond_Ta = new MyDBTableAdapters.CONDUCTORTableAdapter();


                    string PlayOutFolderDate = string.Format("{0:0000}", DateTime.Now.Year) + string.Format("{0:00}", DateTime.Now.Month) + string.Format("{0:00}", DateTime.Now.Day);
                    string PlayOutFolder = System.Configuration.ConfigurationSettings.AppSettings["PlayOutRoot"] + "\\" + PlayOutFolderDate;
                    if (!Directory.Exists(PlayOutFolder + "\\THUMB"))
                    {
                        Directory.CreateDirectory(PlayOutFolder + "\\THUMB");
                    }
                    if (!Directory.Exists(PlayOutFolder + "\\VIDEO"))
                    {
                        Directory.CreateDirectory(PlayOutFolder + "\\VIDEO");
                    }



                    string PlyFileName = Cond_Ta.InsertPly("playout01",
                        Dt.Rows[0]["c_id"].ToString(),
                        "1",
                        "00:01:03",
                        "S:\\PLAYOUT\\" + PlayOutFolderDate,
                        "1",
                        "1"
                        ).ToString();


                    LogWriter("Start Copy Video:" + PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".avi");

                    File.Copy(System.Configuration.ConfigurationSettings.AppSettings["OutputPathFile"], PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".avi");


                    LogWriter("End Copy Video:" + PlayOutFolder + "\\VIDEO\\" + PlyFileName + ".avi");


                    LogWriter("Copy Thumbnail" + PlayOutFolder + "\\THUMB\\" + PlyFileName + ".jpg");

                    File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\P.jpg", PlayOutFolder + "\\THUMB\\" + PlyFileName + ".jpg");



                    Cond_Ta.Update_FileName("playout01", PlyFileName, PlyFileName);
                    Cond_Ta.Update_Active("conductor01", Dt.Rows[0]["c_id"].ToString());

                    //Mark task as done:
                    Ta.Update_Order(int.Parse(Dt.Rows[0]["id"].ToString()));

                    LogWriter("Task Done");




                }
                else
                {
                    //Insert image  for render is less than expected
                    LogWriter("Expected image count is:" + imagesCount);

                    //Task status will change to done for new task
                    LogWriter("Task skipped");
                    //Mark task as done:
                    Ta.Delete_Order(int.Parse(Dt.Rows[0]["id"].ToString()));

                }


            }
            else
            {
               // LogWriter("No new task found");

            }

            timer1.Enabled = true;
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
        protected void Render()
        {

            //Start Render
            RenderObject RndObj = new RenderObject();
            RndObj.AeProjectPath = System.Configuration.ConfigurationSettings.AppSettings["AeProjectFile"];
            RndObj.AeRenderPath = System.Configuration.ConfigurationSettings.AppSettings["AeRenderPath"];
            RndObj.CompositionName = System.Configuration.ConfigurationSettings.AppSettings["Composition"];
            RndObj.DestDirectory = System.Configuration.ConfigurationSettings.AppSettings["OutputPath"];
            RndObj.DestFullFileName = System.Configuration.ConfigurationSettings.AppSettings["OutputPathFile"];
            try
            {
                File.Delete(System.Configuration.ConfigurationSettings.AppSettings["OutputPathFile"]);
            }
            catch
            {

            }

            StreamReader reader = Utilities.Renderer(RndObj);
            int Lngth = richTextBox1.Text.Length;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                richTextBox1.Text = richTextBox1.Text.Remove(Lngth, richTextBox1.Text.Length - Lngth);
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();



                richTextBox1.AppendText(line + " >> FROM : 1589 FRAMES");
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();

            }

            richTextBox1.Text = richTextBox1.Text.Remove(Lngth, richTextBox1.Text.Length - Lngth);
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            Application.DoEvents();



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
    }
}
