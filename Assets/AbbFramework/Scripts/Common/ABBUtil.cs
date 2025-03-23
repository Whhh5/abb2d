using UnityEngine;
using System.Collections;
using System.Text;
using System.Threading;
using System.IO;
using UnityEditor.Connect;
using System;
using Random = UnityEngine.Random;

public static class ABBUtil
{
	private static string GetLogStr(params object[] messageList)
	{
		var str = new StringBuilder();
		foreach (var item in messageList)
		{
			str.Append(item);
        }
		var result = str.ToString();
		return result;
	}
	public static void Log(params object[] messageList)
	{
		var str = GetLogStr(messageList);
		Debug.Log($"ABB: {str}");
    }
	public static void LogWarring(params object[] messageList)
    {
        var str = GetLogStr(messageList);
        Debug.LogWarning($"ABB: {str}");
    }
	public static void LogError(params object[] messageList)
    {
        var str = GetLogStr(messageList);
        Debug.LogError($"ABB: {str}");
    }


	public static float GetRange(float from, float to)
	{
		var value = Random.Range(from, to);
		return value;
	}
    public static int GetRange(int from, int to)
    {
        var value = Random.Range(from, to);
        return value;
    }

	private static CancellationTokenSource m_SceneChangeToken = new();
    public static CancellationTokenSource GetSceneTokenSource()
	{
		return m_SceneChangeToken;
    }

	private static int m_TempKey = 0;
	public static int GetTempKey()
	{
		return ++m_TempKey;
	}


	public static string GetDataPath()
	{
		return Application.dataPath;
    }
	public static string GetPersistentDataPath()
	{
		return Application.persistentDataPath;
	}
	public static string GetUnityRootPath()
	{
		var unityPath = Path.GetDirectoryName(GetDataPath());
		return unityPath;
    }
	public static string GetFullPathByUnityPath(string unityPath)
	{
		var path = Path.Combine(GetUnityRootPath(), unityPath);
		return path;
	}
	public static string GetUnityPathByFullPath(string fullPath)
    {
        var path = Path.GetRelativePath(GetUnityRootPath(), fullPath);
		return path;
    }


	public static float GetTimeDelta()
	{
		return Time.deltaTime;
    }
    public static float GetGameTimeSeconds()
    {
        return Time.time;
    }
    public static float GetSystemTimeSeconds()
	{
        var dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        var time = (DateTime.Now - dtFrom).TotalSeconds;
		return (float)time;
    }
    public static Quaternion Dir2Quaternion(Vector3 dir)
    {
        // 将方向向量转换为角度
        Vector3 eulerAngles = dir.normalized * Mathf.Rad2Deg;

        // 转换为欧拉角
        eulerAngles.x = 0; // Mathf.Clamp(eulerAngles.x, -90f, 90f);
        eulerAngles.y = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        eulerAngles.z = 0f;

        return Quaternion.Euler(eulerAngles);
    }

	public static Vector2 GetMousePoint()
	{
		return Input.mousePosition;
    }
    public static Vector2 GetMousePositionDelta()
    {
        return Input.mousePositionDelta;
    }
    public static float GetMouseScrollDelta()
    {
        return Input.mouseScrollDelta.y;
    }
}

