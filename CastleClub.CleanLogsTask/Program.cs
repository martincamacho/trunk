using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.CleanLogsTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var listFiles = System.IO.Directory.EnumerateFiles(System.Configuration.ConfigurationManager.AppSettings["path"]);
            int filestodelete = 0;
           foreach (var files in listFiles.ToList())
	        {
                var fileinfo = new System.IO.FileInfo(files);
                var file = new System.IO.StreamReader(files);
                string filecontent= file.ReadToEnd();
                file.Close();
                /*
                if (filecontent.Contains("System.NullReferenceException"))
                {
                    filestodelete++;
                    System.IO.File.Delete(files);
                }
               */
                if ((DateTime.Now - fileinfo.CreationTime).TotalDays >= int.Parse(System.Configuration.ConfigurationManager.AppSettings["expiredDays"]))
                 {
                     System.IO.File.Delete(files);
                 }
	        }
           System.Console.WriteLine("Files Deleted: " + filestodelete);
        }
    }
}
