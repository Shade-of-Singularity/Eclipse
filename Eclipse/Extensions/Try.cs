using System;
using UnityEngine;

namespace Eclipse.Extensions
{
    public static class Try
    {
        public static void WithLog(Action action) => Execute(action, Debug.LogException);
        public static void Execute(Action action, Action<Exception>? callback = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                callback?.Invoke(ex);
            }
        }

    }
}
