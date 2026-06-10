using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutOnEnd : MonoBehaviour
{
	public Image fadeImage;
	public float fadeDuration = 3f;

	public void StartFadeOut()
	{
		fadeImage.gameObject.SetActive(true);
		StartCoroutine(FadeOut());
	}

	IEnumerator FadeOut()
	{
		float time = 0f;
		Color color = fadeImage.color;
		color.a = 0f;
		fadeImage.color = color;

		while (time < fadeDuration)
		{
			time += Time.deltaTime;
			color.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
			fadeImage.color = color;
			yield return null;
		}

		color.a = 1f;
		fadeImage.color = color;
	}
}
