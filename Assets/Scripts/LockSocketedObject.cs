using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LockSocketedObject : MonoBehaviour
{
	public MeshRenderer socketMeshRenderer;

	public void DisableObject(SelectEnterEventArgs args)
	{
		GameObject socketedObject = args.interactableObject.transform.gameObject;

		socketedObject.SetActive(false);

		if (socketMeshRenderer)
			socketMeshRenderer.enabled = true;

		Debug.Log("Socket used: " + gameObject.name);
		Debug.Log("Disabled grabbable object: " + socketedObject.name);
		Debug.Log("Enabled socket mesh: " + socketMeshRenderer.gameObject.name);
	}
}