using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOnStart : MonoBehaviour
{
	public Image fadeImage;
	public float fadeDuration = 3f;

	void Start()
	{
		StartCoroutine(FadeIn());
	}

	IEnumerator FadeIn()
	{
		float time = 0f;
		Color color = fadeImage.color;
		color.a = 1f;
		fadeImage.color = color;

		while (time < fadeDuration)
		{
			time += Time.deltaTime;
			color.a = Mathf.Lerp(1f, 0f, time / fadeDuration);
			fadeImage.color = color;
			yield return null;
		}

		color.a = 0f;
		fadeImage.color = color;
		fadeImage.gameObject.SetActive(false);
	}
}