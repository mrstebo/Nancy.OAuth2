using System;
using System.Collections.Generic;

namespace Nancy.OAuth2.Tests
{
    internal static class StubFactory
    {
        private static readonly Dictionary<Type, Type> Registrations = new Dictionary<Type, Type>();

        public static T1 Get<T1>()
        {
            return Registrations.ContainsKey(typeof (T1)) 
                ? (T1) Activator.CreateInstance(Registrations[typeof(T1)])
                : default(T1);
        }

        public static void Set<T1, T2>()
        {
            if (!Registrations.ContainsKey(typeof (T1)))
                Registrations.Add(typeof (T1), typeof (T2));
            Registrations[typeof (T1)] = typeof (T2);
        }

        public static void Unset<T1>()
        {
            if (Registrations.ContainsKey(typeof (T1)))
                Registrations.Remove(typeof (T1));
        }
    }
}
