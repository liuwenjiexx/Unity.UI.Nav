#if UI_NAV_DEBUG && UNITY_EDITOR
#define UI_NAV_LOG
#endif

using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


 

namespace Unity.UI.Navs
{
    public class NavUtility
    {
        public static string LogPrefix = "[UINav] ";
        //NavScope
        static bool? logEnabled;

        /// <summary>
        /// UINavBehaviour Log开关
        /// </summary>
        public static bool LogEnabled
        {
            get
            {
                if (logEnabled == null)
                {
                    logEnabled = PlayerPrefs.GetInt("UI.Navs.LogEnabled") != 0;
                }
                return logEnabled.Value;
            }
            set
            {
                if (logEnabled != value)
                {
                    logEnabled = value;
                    PlayerPrefs.SetInt("UI.Navs.LogEnabled", logEnabled.Value ? 1 : 0);
                    PlayerPrefs.Save();
                }
            }
        }

        [Conditional("UI_NAV_DEBUG")]
        [DebuggerStepThrough]
        public static void Log(string message)
        {
#if UNITY_EDITOR
            //if (!LogEnabled)
            //    return;

            Debug.Log($"{LogPrefix}{message}");
#endif
        }
    }
}
