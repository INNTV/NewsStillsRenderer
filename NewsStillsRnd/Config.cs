using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewsStillsRnd
{
    class Configuration
    {

        public string AeRenderExePath { get; set; }
        public string PlayOutRoot { get; set; }
        public List<string> SqlFilter { get; set; }
        public List<string> OutputPathFile { get; set; }
        public List<string> AeProjectFile { get; set; }
        public List<string> Composition { get; set; }
        public List<string> ImageRoot { get; set; }
        public List<string> GraphicActId { get; set; }
        public List<string> ImageName { get; set; }
        public List<string> VideoDuration { get; set; }

        

                
        public Configuration()
        {
            AeRenderExePath = System.Configuration.ConfigurationSettings.AppSettings["AeRenderExePath"].Trim();
            PlayOutRoot = System.Configuration.ConfigurationSettings.AppSettings["PlayOutRoot"].Trim();
            
            SqlFilter = System.Configuration.ConfigurationSettings.AppSettings["SqlFilter"].Trim().Split(',').ToList();
            OutputPathFile = System.Configuration.ConfigurationSettings.AppSettings["OutputPathFile"].Trim().Split(',').ToList();
            AeProjectFile = System.Configuration.ConfigurationSettings.AppSettings["AeProjectFile"].Trim().Split(',').ToList();
            Composition = System.Configuration.ConfigurationSettings.AppSettings["Composition"].Trim().Split(',').ToList();
            ImageRoot = System.Configuration.ConfigurationSettings.AppSettings["ImageRoot"].Trim().Split(',').ToList();
            ImageName = System.Configuration.ConfigurationSettings.AppSettings["ImageName"].Trim().Split(',').ToList();
            GraphicActId = System.Configuration.ConfigurationSettings.AppSettings["GraphicActId"].Trim().Split(',').ToList();
            VideoDuration = System.Configuration.ConfigurationSettings.AppSettings["VideoDuration"].Trim().Split(',').ToList();


        }
    }
}

