using log4net;

namespace StoreSolution.WebProject.Log4net
{
    public static class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        public static ILog Log
        {
            get { return log; }
        }
    }
}