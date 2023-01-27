using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Extensions
{
    public class Singleton<T> where T : class
    {
        private static volatile T instance;
        private static object syncRoot = new Object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            ConstructorInfo constructorInfo = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                            instance = (T)constructorInfo.Invoke(new object[0]);
                        }
                    }
                }

                return instance;
            }
        }
    }
}
