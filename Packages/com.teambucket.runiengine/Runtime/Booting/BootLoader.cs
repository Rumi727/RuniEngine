#nullable enable
using Cysharp.Threading.Tasks;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.LowLevel;

namespace RuniEngine.Booting
{
    public static class BootLoader
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Booting()
        {
            //Player Loop Setting
            {
                PlayerLoopSystem loopSystems = PlayerLoop.GetCurrentPlayerLoop();

                //UniTask Setting
                PlayerLoopHelper.Initialize(ref loopSystems);

                //Awaken Invoke
                AttributeInvoke<AwakenAttribute>();

                //Custom Update Setting
                CustomPlayerLoopSetter.EventRegister(ref loopSystems);
                PlayerLoop.SetPlayerLoop(loopSystems);
            }
        }

        static void AttributeInvoke<T>() where T : Attribute
        {
            Type[] types = ReflectionManager.types;
            for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
            {
                MethodInfo[] methodInfos = types[typesIndex].GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                for (int methodInfoIndex = 0; methodInfoIndex < methodInfos.Length; methodInfoIndex++)
                {
                    MethodInfo methodInfo = methodInfos[methodInfoIndex];
                    if (Attribute.GetCustomAttributes(methodInfo, typeof(T)).Length > 0 && methodInfo.GetParameters().Length <= 0)
                        methodInfo.Invoke(null, null);
                }
            }
        }
    }
}
