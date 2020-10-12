using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(CameraBrain))]
public class CameraBrainInspector : Editor
{
	int cIndex = 0;
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		CameraBrain IpctDic = target as CameraBrain;
		if (IpctDic.Keys == null)
		{
			IpctDic.Keys = new CameraTargetType[0];
			IpctDic.Values = new Transform[0];
		}
		DrawHeader("相机目标类型添加");
		GUILayout.BeginVertical("Box");
		for (int i = 0; i < IpctDic.Keys.Length; i++)
		{
			if (DrawItem(IpctDic.Keys[i], ref IpctDic.Values[i], i))
			{
				ArrayDeleteAt(ref IpctDic.Keys, i, 1);
				ArrayDeleteAt(ref IpctDic.Values, i, 1);
			}
		}
		GUILayout.BeginHorizontal();
		string[] keys = Enum.GetNames(typeof(CameraTargetType));
		cIndex = EditorGUILayout.Popup(cIndex, keys);

		if (GUILayout.Button("添加类型"))
		{
			bool have = false;
			CameraTargetType ct = (CameraTargetType)cIndex;
			for (int i = 0; i < IpctDic.Keys.Length; i++)
			{
				if (IpctDic.Keys[i] == ct)
				{
					have = true;
					break;
				}
			}
			if (!have)
			{
				System.Array.Resize(ref IpctDic.Keys, IpctDic.Keys.Length + 1);
				System.Array.Resize(ref IpctDic.Values, IpctDic.Values.Length + 1);
				IpctDic.Keys[IpctDic.Keys.Length - 1] = ct;
			}
		}
		if (GUILayout.Button("清除所有"))
		{
			for (int i = 0; i < IpctDic.Keys.Length; i++)
			{
				ArrayDeleteAt(ref IpctDic.Keys, i, 1);
				ArrayDeleteAt(ref IpctDic.Values, i, 1);
				i--;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		EditorUtility.SetDirty(IpctDic);
	}
	private bool DrawItem(CameraTargetType key, ref Transform value, int index)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("类型:", GUILayout.Width(30));
		EditorGUILayout.LabelField(key.ToString(),GUILayout.Width(120));
		EditorGUILayout.LabelField("目标:", GUILayout.Width(40));
		value = (Transform)EditorGUILayout.ObjectField(value, typeof(Transform), true);
		if (GUILayout.Button("删除"))
		{
			return true;
		}
		EditorGUILayout.EndHorizontal();
		return false;
	}
	public static void ArrayDeleteAt<T>(ref T[] source, int index, int count)
	{
		int offset = 0;
		for (int i = 0; i < source.Length - count; i++)
		{
			if (i >= index && i < count + index)
			{
				source[i] = source[source.Length - offset - 1];
				offset++;
			}
		}
		Array.Resize<T>(ref source, source.Length - count);
	}

	static public bool DrawHeader(string text)
	{
		bool state = EditorPrefs.GetBool(text, true);

		if (!state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUI.changed = false;

		text = "<b><size=11>" + text + "</size></b>";
		if (state) text = "\u25BC " + text;
		else text = "\u25BA " + text;
		if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;

		if (GUI.changed) EditorPrefs.SetBool(text, state);

		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!state) GUILayout.Space(3f);
		return state;
	}
}
