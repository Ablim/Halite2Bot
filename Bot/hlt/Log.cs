using System.IO;

namespace Bot.hlt
{
    public class Log
    {
        private TextWriter file;
        private static Log instance;

        private Log(TextWriter f)
        {
            file = f;
        }

        public static void Initialize(TextWriter f)
        {
            instance = new Log(f);
        }

        public static void LogMessage(string message)
        {
            try
            {
                instance.file.WriteLine(message);
                instance.file.Flush();
            }
            catch (IOException)
            {
            }
        }
    }
}
