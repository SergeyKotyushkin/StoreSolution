using System;
using System.Collections.Generic;
using System.Reflection;

namespace StoreSolution.MyIoC
{
    public static class SimpleContainer
    {
        private static readonly Dictionary<Type, Type> Links = new Dictionary<Type, Type>();

        public static void Register<T>(Type service)
        {
            Links[typeof (T)] = service;
        }

        public static T Resolve<T>()
        {
            return (T) Activator.CreateInstance(Links[typeof (T)]);
        }
    }
}
