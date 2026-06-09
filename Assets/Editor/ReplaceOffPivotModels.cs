using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ReplaceOffPivotModels : EditorWindow
{
	public GameObject fixedRoot;
	public GameObject targetRoot;

	static Regex suffix = new Regex(@"\s*\(\d+\)$");

	[MenuItem("Tools/Replace Off Pivot Models")]
	static void Open() => GetWindow<ReplaceOffPivotModels>();

	void OnGUI()
	{
		fixedRoot = (GameObject)EditorGUILayout.ObjectField("Fixed Model Root", fixedRoot, typeof(GameObject), true);
		targetRoot = (GameObject)EditorGUILayout.ObjectField("Target Root", targetRoot, typeof(GameObject), true);

		if (GUILayout.Button("Replace"))
			Replace();
	}

	void Replace()
	{
		if (!fixedRoot || !targetRoot)
		{
			Debug.LogError("Assign both Fixed Model Root and Target Root.");
			return;
		}

		int success = 0;
		int total = 0;

		foreach (Transform oldObj in targetRoot.GetComponentsInChildren<Transform>(true))
		{
			if (oldObj == targetRoot.transform)
				continue;

			string oldName = Clean(oldObj.name);
			Transform fixedObj = FindFixedModel(oldName);

			if (!fixedObj)
				continue;

			total++;

			Vector3 pos = oldObj.position;
			Quaternion rot = oldObj.rotation;
			Vector3 scale = oldObj.localScale;
			Transform parent = oldObj.parent;
			int index = oldObj.GetSiblingIndex();

			GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(fixedObj.gameObject);
			if (!newObj)
				newObj = Instantiate(fixedObj.gameObject);

			Undo.RegisterCreatedObjectUndo(newObj, "Replace Model");

			newObj.name = oldObj.name;
			newObj.transform.SetParent(parent);
			newObj.transform.SetSiblingIndex(index);
			newObj.transform.position = pos;
			newObj.transform.rotation = rot;
			newObj.transform.localScale = scale;

			Undo.DestroyObjectImmediate(oldObj.gameObject);

			success++;
			Debug.Log($"SUCCESS: Replaced {oldName}");
		}

		Debug.Log($"DONE: {success}/{total} replacements succeeded.");
	}

	Transform FindFixedModel(string cleanName)
	{
		foreach (Transform t in fixedRoot.GetComponentsInChildren<Transform>(true))
		{
			if (t == fixedRoot.transform)
				continue;

			if (Clean(t.name) == cleanName)
				return t;
		}

		Debug.LogWarning($"FAIL: No fixed model found for {cleanName}");
		return null;
	}

	string Clean(string name)
	{
		name = name.Replace("(Clone)", "").Trim();
		name = suffix.Replace(name, "").Trim();
		return name;
	}
}