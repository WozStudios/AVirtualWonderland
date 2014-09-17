#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class DebugUtils
    {
        public static void Assert(bool condition)
        {
#if DEBUG
//            if (!condition)
//            {
//                var trace = new System.Diagnostics.StackTrace();
//                Debug.LogWarning("Assert in method: " + trace.GetFrame(1).GetMethod().Name);
//
//                throw new Exception();
//            }
#endif
        }

        public static void Assert(bool condition, string message)
        {
#if DEBUG
            if (!condition)
            {
                Debug.LogError(message);

                var trace = new System.Diagnostics.StackTrace();
                Debug.LogWarning("Assert in method: " + trace.GetFrame(1).GetMethod().Name);

                throw new Exception();
            }
#endif
        }
    }
}
