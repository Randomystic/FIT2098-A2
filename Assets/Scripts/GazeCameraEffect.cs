using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GazeCameraEffect : MonoBehaviour
{
	public Image vignetteImage;
	public Image flashImage;

	public float vignetteFadeTime = 1f;
	public float vignetteMaxAlpha = 0.6f;

	public float flashInTime = 0.1f;
	public float flashOutTime = 0.1f;

	Coroutine vignetteRoutine;
	Coroutine flashRoutine;

	void Start()
	{
		SetAlpha(vignetteImage, 0f);
		SetAlpha(flashImage, 0f);

		if (flashImage) flashImage.gameObject.SetActive(false);
	}

	public void StartVignette()
	{
		if (vignetteRoutine != null) StopCoroutine(vignetteRoutine);
		vignetteRoutine = StartCoroutine(FadeImage(vignetteImage, vignetteImage.color.a, vignetteMaxAlpha, vignetteFadeTime));
	}

	public void StopVignette()
	{
		if (vignetteRoutine != null) StopCoroutine(vignetteRoutine);
		vignetteRoutine = StartCoroutine(FadeImage(vignetteImage, vignetteImage.color.a, 0f, vignetteFadeTime));
	}

	public void Flash()
	{
		if (flashRoutine != null) StopCoroutine(flashRoutine);
		flashRoutine = StartCoroutine(FlashRoutine());
	}

	IEnumerator FlashRoutine()
	{
		flashImage.gameObject.SetActive(true);

		yield return FadeImage(flashImage, 0f, 1f, flashInTime);
		yield return FadeImage(flashImage, 1f, 0f, flashOutTime);

		flashImage.gameObject.SetActive(false);
	}

	IEnumerator FadeImage(Image image, float from, float to, float duration)
	{
		if (!image) yield break;

		float time = 0f;

		while (time < duration)
		{
			time += Time.deltaTime;
			SetAlpha(image, Mathf.Lerp(from, to, time / duration));
			yield return null;
		}

		SetAlpha(image, to);
	}

	void SetAlpha(Image image, float alpha)
	{
		if (!image) return;

		Color c = image.color;
		c.a = alpha;
		image.color = c;
	}
}