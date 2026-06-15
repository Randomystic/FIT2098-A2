using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractionAudioManager : MonoBehaviour
{
	[Header("Audio Source")]
	public AudioSource audioSource;

	[Header("Teleport")]
	public TeleportationProvider teleportationProvider;
	public AudioClip teleportSound;

	[Header("Grab Interactables")]
	public AudioClip hoverSound;
	public AudioClip grabSound;

	[Header("Socket Interactors")]
	public AudioClip socketSound;

	[Header("Climb Interactables")]
	public AudioClip climbGrabSound;

	XRGrabInteractable[] grabInteractables;
	XRSocketInteractor[] socketInteractors;
	ClimbInteractable[] climbInteractables;

	void Start()
	{
		if (!audioSource)
			audioSource = GetComponent<AudioSource>();

		if (!audioSource)
			return;

		audioSource.playOnAwake = false;
		audioSource.loop = false;
		audioSource.spatialBlend = 0f;

		if (teleportationProvider)
			teleportationProvider.endLocomotion += PlayTeleportSound;

		grabInteractables = FindObjectsOfType<XRGrabInteractable>(true);
		socketInteractors = FindObjectsOfType<XRSocketInteractor>(true);
		climbInteractables = FindObjectsOfType<ClimbInteractable>(true);

		foreach (XRGrabInteractable interactable in grabInteractables)
		{
			interactable.firstHoverEntered.AddListener(PlayHoverSound);
			interactable.firstSelectEntered.AddListener(PlayGrabSound);
		}

		foreach (XRSocketInteractor socket in socketInteractors)
			socket.selectEntered.AddListener(PlaySocketSound);

		foreach (ClimbInteractable interactable in climbInteractables)
			interactable.firstSelectEntered.AddListener(PlayClimbSound);
	}

	void OnDestroy()
	{
		if (teleportationProvider)
			teleportationProvider.endLocomotion -= PlayTeleportSound;

		if (grabInteractables != null)
		{
			foreach (XRGrabInteractable interactable in grabInteractables)
			{
				if (!interactable)
					continue;

				interactable.firstHoverEntered.RemoveListener(PlayHoverSound);
				interactable.firstSelectEntered.RemoveListener(PlayGrabSound);
			}
		}

		if (socketInteractors != null)
		{
			foreach (XRSocketInteractor socket in socketInteractors)
			{
				if (socket)
					socket.selectEntered.RemoveListener(PlaySocketSound);
			}
		}

		if (climbInteractables != null)
		{
			foreach (ClimbInteractable interactable in climbInteractables)
			{
				if (interactable)
					interactable.firstSelectEntered.RemoveListener(PlayClimbSound);
			}
		}
	}

	void PlayTeleportSound(LocomotionSystem system)
	{
		PlaySound(teleportSound);
	}

	void PlayHoverSound(HoverEnterEventArgs args)
	{
		if (args.interactorObject is XRSocketInteractor)
			return;

		PlaySound(hoverSound);
	}

	void PlayGrabSound(SelectEnterEventArgs args)
	{
		if (args.interactorObject is XRSocketInteractor)
			return;

		PlaySound(grabSound);
	}

	void PlaySocketSound(SelectEnterEventArgs args)
	{
		PlaySound(socketSound);
	}

	void PlayClimbSound(SelectEnterEventArgs args)
	{
		PlaySound(climbGrabSound);
	}

	void PlaySound(AudioClip clip)
	{
		if (audioSource && clip)
			audioSource.PlayOneShot(clip);
	}
}