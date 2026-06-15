using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientTrackManager : MonoBehaviour
{
	public TimeOfDayController timeOfDayController;

	public AudioClip morningTrack;
	public AudioClip noonTrack;
	public AudioClip eveningTrack;
	public AudioClip nightTrack;

	[Range(0f, 100f)] public float noonSwitchTime = 30f;
	[Range(0f, 100f)] public float eveningSwitchTime = 70f;
	[Range(0f, 100f)] public float nightSwitchTime = 90f;

	[Range(0f, 1f)] public float ambientVolume = 0.5f;

	public float crossfadeTime = 3f;

	public AudioSource audioSourceA;
	public AudioSource audioSourceB;

	AudioSource currentSource;
	AudioSource nextSource;

	AudioClip currentTrack;
	Coroutine crossfadeRoutine;

	void Start()
	{
		SetupAudioSource(audioSourceA);
		SetupAudioSource(audioSourceB);

		currentSource = audioSourceA;
		nextSource = audioSourceB;

		if (!timeOfDayController || !currentSource || !nextSource)
			return;

		currentTrack = GetTrack(timeOfDayController.timeOfDay);

		if (currentTrack)
		{
			currentSource.clip = currentTrack;
			currentSource.volume = ambientVolume;
			currentSource.Play();
		}
	}

	void Update()
	{
		if (!timeOfDayController)
			return;

		AudioClip targetTrack = GetTrack(timeOfDayController.timeOfDay);

		if (targetTrack && targetTrack != currentTrack)
			SwitchTrack(targetTrack);

		if (crossfadeRoutine == null && currentSource)
			currentSource.volume = ambientVolume;
	}

	AudioClip GetTrack(float time)
	{
		if (time >= nightSwitchTime)
			return nightTrack;

		if (time >= eveningSwitchTime)
			return eveningTrack;

		if (time >= noonSwitchTime)
			return noonTrack;

		return morningTrack;
	}

	void SwitchTrack(AudioClip newTrack)
	{
		currentTrack = newTrack;

		if (crossfadeRoutine != null)
			StopCoroutine(crossfadeRoutine);

		crossfadeRoutine = StartCoroutine(Crossfade(newTrack));
	}

	IEnumerator Crossfade(AudioClip newTrack)
	{
		nextSource.Stop();
		nextSource.clip = newTrack;
		nextSource.volume = 0f;
		nextSource.Play();

		float duration = Mathf.Max(0.01f, crossfadeTime);
		float timer = 0f;
		float currentStartVolume = currentSource.volume;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			float blend = Mathf.Clamp01(timer / duration);

			currentSource.volume =
				Mathf.Lerp(currentStartVolume, 0f, blend);

			nextSource.volume =
				Mathf.Lerp(0f, ambientVolume, blend);

			yield return null;
		}

		currentSource.Stop();
		currentSource.volume = 0f;
		nextSource.volume = ambientVolume;

		AudioSource oldSource = currentSource;
		currentSource = nextSource;
		nextSource = oldSource;

		crossfadeRoutine = null;
	}

	void SetupAudioSource(AudioSource source)
	{
		if (!source)
			return;

		source.playOnAwake = false;
		source.loop = true;
		source.spatialBlend = 0f;
	}
}