using log4net;
using log4net.Config;

namespace StoreSolution.BusinessLogic.Log4net
{
    public static class Logger
    {
        private static readonly ILog Log4NetLog = LogManager.GetLogger(typeof(Logger));

        public static void Init()
        {
            XmlConfigurator.Configure();
        }

        public static ILog Log
        {
            get { return Log4NetLog; }
        }
    }
}