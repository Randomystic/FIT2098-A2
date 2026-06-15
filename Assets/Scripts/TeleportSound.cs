using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportSound : MonoBehaviour
{
	public TeleportationProvider teleportationProvider;
	public AudioSource audioSource;
	public AudioClip teleportClip;

	void OnEnable()
	{
		if (teleportationProvider)
			teleportationProvider.endLocomotion += PlayTeleportSound;
	}

	void OnDisable()
	{
		if (teleportationProvider)
			teleportationProvider.endLocomotion -= PlayTeleportSound;
	}

	void PlayTeleportSound(LocomotionSystem system)
	{
		if (audioSource && teleportClip)
			audioSource.PlayOneShot(teleportClip);
	}
}