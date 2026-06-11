using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecklistMonitor : MonoBehaviour
{
	public GameObject[] checkmarks = new GameObject[9];

	public TimeOfDayController timeOfDayController;
	public float timeIncreasePerTask = 10f;

	bool[] completed = new bool[9];

	string[] taskNames =
	{
		"Chop and store firewood",
		"Clear fallen logs",
		"Repair sign",
		"Pickup rubbish",
		"Shovel out mudslide",
		"Check pest trap",
		"Take photo of kiwi nest",
		"Enjoy the sunset",
		"Go home and rest"
	};

	void Start()
	{
		for (int i = 0; i < checkmarks.Length; i++)
			if (checkmarks[i]) checkmarks[i].SetActive(false);
	}

	public void CompleteFirewood() => CompleteTask(0);

	public void CompleteFallenLogs()
	{
		if (!RequireCompleted(0)) return;
		CompleteTask(1);
	}

	public void CompleteRepairSign() => CompleteTask(2);
	public void CompletePickupRubbish() => CompleteTask(3);

	public void CompleteMudslide()
	{
		if (!CanCompleteMudslide()) return;
		CompleteTask(4);
	}

	public void CompletePestTrap() => CompleteTask(5);
	public void CompleteKiwiPhoto() => CompleteTask(6);

	public void CompleteSunset()
	{
		if (!CanCompleteSunset()) return;
		CompleteTask(7);
	}

	public void CompleteGoHome()
	{
		if (!CanGoHome()) return;
		CompleteTask(8);
	}

	bool CanCompleteMudslide()
	{
		return RequireCompleted(0, 1, 2, 3);
	}

	bool CanCompleteSunset()
	{
		return RequireCompleted(0, 1, 2, 3, 4, 5, 6);
	}

	bool CanGoHome()
	{
		return RequireCompleted(0, 1, 2, 3, 4, 5, 6, 7);
	}

	bool RequireCompleted(params int[] requiredTasks)
	{
		bool canComplete = true;

		foreach (int taskIndex in requiredTasks)
		{
			if (!completed[taskIndex])
			{
				Debug.Log("Still needs to be completed: " + taskNames[taskIndex]);
				canComplete = false;
			}
		}

		return canComplete;
	}

	void CompleteTask(int taskIndex)
	{
		if (completed[taskIndex])
		{
			Debug.Log(taskNames[taskIndex] + " already completed.");
			return;
		}

		completed[taskIndex] = true;

		if (checkmarks[taskIndex])
			checkmarks[taskIndex].SetActive(true);

		if (timeOfDayController)
			timeOfDayController.AddTime(timeIncreasePerTask);

		Debug.Log("Task completed: " + taskNames[taskIndex]);
	}
}