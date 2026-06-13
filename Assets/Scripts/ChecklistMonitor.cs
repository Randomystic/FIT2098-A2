using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChecklistMonitor : MonoBehaviour
{
	public GameObject[] checkmarks = new GameObject[9];

	public TimeOfDayController timeOfDayController;
	public float timeIncreasePerTask = 10f;

	public UnityEvent onFallenLogsUnlocked;
	public UnityEvent onMudslideUnlocked;
	public UnityEvent onSunsetUnlocked;
	public UnityEvent onGoHomeUnlocked;

	bool[] completed = new bool[9];

	bool fallenLogsUnlocked;
	bool mudslideUnlocked;
	bool sunsetUnlocked;
	bool goHomeUnlocked;

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
			if (checkmarks[i])
				checkmarks[i].SetActive(false);
	}

	public void CompleteFirewood()
	{
		CompleteTask(0);
	}

	public void CompleteFallenLogs()
	{
		if (!RequireCompleted(0))
			return;

		CompleteTask(1);
	}

	public void CompleteRepairSign()
	{
		CompleteTask(2);
	}

	public void CompletePickupRubbish()
	{
		CompleteTask(3);
	}

	public void CompleteMudslide()
	{
		if (!RequireCompleted(0, 1, 2, 3))
			return;

		CompleteTask(4);
	}

	public void CompletePestTrap()
	{
		CompleteTask(5);
	}

	public void CompleteKiwiPhoto()
	{
		CompleteTask(6);
	}

	public void CompleteSunset()
	{
		if (!RequireCompleted(0, 1, 2, 3, 4, 5, 6))
			return;

		CompleteTask(7);
	}

	public void CompleteGoHome()
	{
		if (!RequireCompleted(0, 1, 2, 3, 4, 5, 6, 7))
			return;

		CompleteTask(8);
	}

	void CheckUnlocks()
	{
		if (!fallenLogsUnlocked && AreCompleted(0))
		{
			fallenLogsUnlocked = true;
			onFallenLogsUnlocked.Invoke();
			Debug.Log("Unlocked: Clear fallen logs");
		}

		if (!mudslideUnlocked && AreCompleted(0, 1, 2, 3))
		{
			mudslideUnlocked = true;
			onMudslideUnlocked.Invoke();
			Debug.Log("Unlocked: Shovel out mudslide");
		}

		if (!sunsetUnlocked && AreCompleted(0, 1, 2, 3, 4, 5, 6))
		{
			sunsetUnlocked = true;
			onSunsetUnlocked.Invoke();
			Debug.Log("Unlocked: Enjoy the sunset");
		}

		if (!goHomeUnlocked && AreCompleted(0, 1, 2, 3, 4, 5, 6, 7))
		{
			goHomeUnlocked = true;
			onGoHomeUnlocked.Invoke();
			Debug.Log("Unlocked: Go home and rest");
		}
	}

	bool AreCompleted(params int[] tasks)
	{
		foreach (int task in tasks)
			if (!completed[task])
				return false;

		return true;
	}

	bool RequireCompleted(params int[] requiredTasks)
	{
		bool allowed = true;

		foreach (int task in requiredTasks)
		{
			if (!completed[task])
			{
				Debug.Log("Still needs to be completed: " + taskNames[task]);
				allowed = false;
			}
		}

		return allowed;
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
{
		if (taskIndex == 7)
			timeOfDayController.SetTime(100f);
		else
			timeOfDayController.AddTime(timeIncreasePerTask);
}

		Debug.Log("Task completed: " + taskNames[taskIndex]);

		CheckUnlocks();
	}
}