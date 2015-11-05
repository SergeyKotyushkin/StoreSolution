using log4net;
using log4net.Config;

namespace StoreSolution.WebProject.Log4net
{
    public static class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            var t = XmlConfigurator.Configure();

            
        }
    }
}