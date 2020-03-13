using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CastleClub.BusinessLogic.Utils
{
    public class EventViewer
    {
        public static void Writte(string application, string from, string events, EventLogEntryType type)
        {
            EventLog.WriteEntry(application + " - " + from, events, type);
        }

        public static void WriteToFile(string header, string body, string path)
        {
            LoggingUtilities.Logger.LogEntry(header + " - " + body);
            //if (!System.IO.File.Exists(path))
            //{
            //    System.IO.File.Create(path);
            //}

            //System.IO.StreamWriter sw = new System.IO.StreamWriter(path,true);

            //sw.WriteLine(header);
            //sw.WriteLine("\t" + body);

            //sw.Close();
        }
    }
}
