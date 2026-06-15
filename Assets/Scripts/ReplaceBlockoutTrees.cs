using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReplaceBlockoutTrees : MonoBehaviour
{
	public Transform blockoutTreesParent;
	public Transform treeModelsParent;

	public LayerMask groundLayer;
	public float raycastHeight = 100f;
	public float treeScale = 0.3f;

	[ContextMenu("Replace Blockout Trees")]
	public void ReplaceTrees()
	{
		if (!blockoutTreesParent || !treeModelsParent)
			return;

		if (treeModelsParent.childCount == 0)
			return;

		List<Transform> blockoutTrees = new List<Transform>();

		foreach (Transform tree in blockoutTreesParent)
			blockoutTrees.Add(tree);

		foreach (Transform blockoutTree in blockoutTrees)
		{
			Transform randomModel = treeModelsParent.GetChild(
				Random.Range(0, treeModelsParent.childCount)
			);

			Vector3 spawnPosition = blockoutTree.position;

			GameObject newTree = Instantiate(
				randomModel.gameObject,
				spawnPosition,
				Quaternion.Euler(0f, Random.Range(0f, 360f), 0f),
				blockoutTreesParent
			);

			newTree.transform.localScale = Vector3.one * treeScale;
			newTree.name = randomModel.name;
			newTree.SetActive(true);

			PlaceTreeOnGround(newTree, spawnPosition);

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				Undo.RegisterCreatedObjectUndo(newTree, "Create Tree");
				Undo.DestroyObjectImmediate(blockoutTree.gameObject);
			}
			else
			{
				Destroy(blockoutTree.gameObject);
			}
#else
			Destroy(blockoutTree.gameObject);
#endif
		}

		Debug.Log("Replaced " + blockoutTrees.Count + " blockout trees.");
	}

	void PlaceTreeOnGround(GameObject tree, Vector3 originalPosition)
	{
		Vector3 rayOrigin = originalPosition + Vector3.up * raycastHeight;

		if (!Physics.Raycast(
			rayOrigin,
			Vector3.down,
			out RaycastHit hit,
			raycastHeight * 2f,
			groundLayer))
		{
			Debug.LogWarning("No ground found below " + tree.name);
			return;
		}

		Renderer[] renderers = tree.GetComponentsInChildren<Renderer>();

		if (renderers.Length == 0)
			return;

		Bounds bounds = renderers[0].bounds;

		for (int i = 1; i < renderers.Length; i++)
			bounds.Encapsulate(renderers[i].bounds);

		float heightDifference = hit.point.y - bounds.min.y;

		tree.transform.position += Vector3.up * heightDifference;
	}
}