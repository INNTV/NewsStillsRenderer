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
        public List<string> ImageCount { get; set; }
        public List<string> GraphicActId { get; set; }
        public List<string> ImageName { get; set; }
        public List<string> VideoDuration { get; set; }
        public List<string> Frame { get; set; }


        

                
        public Configuration()
        {
            AeRenderExePath = System.Configuration.ConfigurationSettings.AppSettings["AeRenderExePath"];
            PlayOutRoot = System.Configuration.ConfigurationSettings.AppSettings["PlayOutRoot"];
            
            SqlFilter = System.Configuration.ConfigurationSettings.AppSettings["SqlFilter"].Split(',').ToList();
            OutputPathFile = System.Configuration.ConfigurationSettings.AppSettings["OutputPathFile"].Split(',').ToList();
            AeProjectFile = System.Configuration.ConfigurationSettings.AppSettings["AeProjectFile"].Split(',').ToList();
            Composition = System.Configuration.ConfigurationSettings.AppSettings["Composition"].Split(',').ToList();
            ImageRoot = System.Configuration.ConfigurationSettings.AppSettings["ImageRoot"].Split(',').ToList();
            ImageCount = System.Configuration.ConfigurationSettings.AppSettings["ImageCount"].Split(',').ToList();
            ImageName = System.Configuration.ConfigurationSettings.AppSettings["ImageName"].Split(',').ToList();
            GraphicActId = System.Configuration.ConfigurationSettings.AppSettings["GraphicActId"].Split(',').ToList();
            VideoDuration = System.Configuration.ConfigurationSettings.AppSettings["VideoDuration"].Split(',').ToList();
            Frame = System.Configuration.ConfigurationSettings.AppSettings["Frame"].Split(',').ToList();



        }
    }
}

