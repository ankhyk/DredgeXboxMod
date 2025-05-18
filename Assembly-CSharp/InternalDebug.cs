using System;
using System.Diagnostics;
using UnityEngine;

public static class InternalDebug
{
	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, global::UnityEngine.Object context)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, object message)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void Assert(bool condition, object message, global::UnityEngine.Object context)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void AssertFormat(bool condition, string format, params object[] args)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void AssertFormat(bool condition, global::UnityEngine.Object context, string format, params object[] args)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void Break()
	{
		global::UnityEngine.Debug.Break();
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void ClearDeveloperConsole()
	{
		global::UnityEngine.Debug.ClearDeveloperConsole();
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		global::UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		global::UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void Log(object message, bool condition = true)
	{
		if (condition)
		{
			global::UnityEngine.Debug.Log(message);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void Log(object message, global::UnityEngine.Object context, bool condition = true)
	{
		if (condition)
		{
			global::UnityEngine.Debug.Log(message, context);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void LogAssertion(object message, bool condition = true)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void LogAssertion(object message, global::UnityEngine.Object context, bool condition = true)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void LogAssertionFormat(string format, params object[] args)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void LogAssertionFormat(string format, bool condition = true, params object[] args)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void LogAssertionFormat(global::UnityEngine.Object context, string format, params object[] args)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	[Conditional("UNITY_ASSERTIONS")]
	public static void LogAssertionFormat(global::UnityEngine.Object context, string format, bool condition = true, params object[] args)
	{
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogError(object message, bool condition = true)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogError(message);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogError(object message, global::UnityEngine.Object context, bool condition = true)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogError(message, context);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorFormat(string format, params object[] args)
	{
		global::UnityEngine.Debug.LogErrorFormat(format, args);
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorFormat(string format, bool condition = true, params object[] args)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogErrorFormat(format, args);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorFormat(global::UnityEngine.Object context, string format, params object[] args)
	{
		global::UnityEngine.Debug.LogErrorFormat(context, format, args);
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorFormat(global::UnityEngine.Object context, string format, bool condition = true, params object[] args)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogErrorFormat(context, format, args);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogException(Exception exception, bool condition = true)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogException(exception);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogException(Exception exception, global::UnityEngine.Object context, bool condition = true)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogException(exception, context);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogFormat(string format, params object[] args)
	{
		global::UnityEngine.Debug.LogFormat(format, args);
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogFormat(string format, bool condition = true, params object[] args)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogFormat(format, args);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogFormat(global::UnityEngine.Object context, string format, params object[] args)
	{
		global::UnityEngine.Debug.LogFormat(context, format, args);
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogFormat(global::UnityEngine.Object context, string format, bool condition = true, params object[] args)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogFormat(context, format, args);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarning(object message, bool condition = true)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogWarning(message);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarning(object message, global::UnityEngine.Object context, bool condition = true)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogWarning(message, context);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarningFormat(string format, params object[] args)
	{
		global::UnityEngine.Debug.LogWarningFormat(format, args);
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarningFormat(string format, bool condition = true, params object[] args)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogWarningFormat(format, args);
		}
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarningFormat(global::UnityEngine.Object context, string format, params object[] args)
	{
		global::UnityEngine.Debug.LogWarningFormat(context, format, args);
	}

	[Conditional("DEMO_MODE")]
	[Conditional("STAGE_MODE")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarningFormat(global::UnityEngine.Object context, string format, bool condition = true, params object[] args)
	{
		if (condition)
		{
			global::UnityEngine.Debug.LogWarningFormat(context, format, args);
		}
	}
}
