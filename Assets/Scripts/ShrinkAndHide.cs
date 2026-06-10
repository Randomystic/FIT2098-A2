using System.Collections;
using System.Collections.Generic;using UnityEngine;
using UnityEngine.Events;

public class HideAndShrink : MonoBehaviour
{
	public float shrinkAmount = 0.2f;
	public float hideAtScale = 0.4f;

	public UnityEvent OnFullyShrunk;

	public void Shrink()
	{
		Vector3 s = transform.localScale;
		s -= Vector3.one * shrinkAmount;

		if (s.x <= hideAtScale || s.y <= hideAtScale || s.z <= hideAtScale)
		{
			transform.localScale = Vector3.one * hideAtScale;
			OnFullyShrunk.Invoke();
			HideObject();
			return;
		}

		transform.localScale = s;
	}

	void HideObject()
	{
		gameObject.SetActive(false);

		// foreach (Renderer r in GetComponentsInChildren<Renderer>())
		// 	r.enabled = false;

		// foreach (Collider c in GetComponentsInChildren<Collider>())
		// 	c.enabled = false;
	}
}