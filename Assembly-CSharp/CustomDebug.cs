using System;
using System.Diagnostics;
using UnityEngine;

public static class CustomDebug
{
	public static void Init()
	{
		for (int i = 0; i < CustomDebug.s_logTypes.Length; i++)
		{
			ValueTuple<LogType, StackTraceLogType> valueTuple = CustomDebug.s_logTypes[i];
			LogType item = valueTuple.Item1;
			StackTraceLogType item2 = valueTuple.Item2;
			Application.SetStackTraceLogType(item, item2);
		}
	}

	public static void Log(string message)
	{
		global::UnityEngine.Debug.Log(message);
	}

	public static void LogWarning(string message)
	{
		global::UnityEngine.Debug.LogWarning(message);
	}

	public static void LogError(string message)
	{
		global::UnityEngine.Debug.LogError(message);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("STAGE_MODE")]
	public static void EditorLog(string message)
	{
		global::UnityEngine.Debug.Log(message);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("STAGE_MODE")]
	public static void EditorLogWarning(string message)
	{
		global::UnityEngine.Debug.LogWarning(message);
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("STAGE_MODE")]
	[Conditional("PROD_MODE")]
	public static void EditorLogError(string message)
	{
		global::UnityEngine.Debug.LogError(message);
	}

	private static readonly ValueTuple<LogType, StackTraceLogType>[] s_logTypes = new ValueTuple<LogType, StackTraceLogType>[]
	{
		new ValueTuple<LogType, StackTraceLogType>(LogType.Assert, StackTraceLogType.None),
		new ValueTuple<LogType, StackTraceLogType>(LogType.Warning, StackTraceLogType.None),
		new ValueTuple<LogType, StackTraceLogType>(LogType.Log, StackTraceLogType.None),
		new ValueTuple<LogType, StackTraceLogType>(LogType.Error, StackTraceLogType.Full),
		new ValueTuple<LogType, StackTraceLogType>(LogType.Exception, StackTraceLogType.Full)
	};
}
