using UnityEngine;
using System.Diagnostics;

namespace StarlightVigil
{
    /// <summary>
    /// 统一的调试工具类
    /// </summary>
    public static class GameDebug
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log($"[StarlightVigil] {message}");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"[StarlightVigil] {message}");
        }

        [Conditional("UNITY_EDITOR")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[StarlightVigil] {message}");
        }
    }
}