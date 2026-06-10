using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntCounter : MonoBehaviour
{
    [Tooltip("The current number - this value can only be changed in events")]
    [SerializeField] int count;
    [SerializeField] private Condition condition = Condition.None;
    [Tooltip("This is the target number that the condition will be checked against")]
    [SerializeField] private int targetCount = 0;
    
    [SerializeField] private Text displayText;
    
    private int lastCount;

    [SerializeField] private OnCountChanged onCountChanged = new OnCountChanged();
    [SerializeField] private OnCountMeetsCondition onCountMeetsCondition = new OnCountMeetsCondition();

    private void Start()
    {
        if (displayText == null) displayText = GetComponent<Text>();
    }

    public void SetNumber(int newNumber)
    {
        count = newNumber;
        UpdateNumber();
    }

    public void Increment()
    {
        count++;
        UpdateNumber();
    }
    public void Decrement()
    {
        count--;
        UpdateNumber();
    }
    
    public void Add(int value)
    {
        count+=value;
        UpdateNumber();
    }
    
    public void Subtract(int value)
    {
        count-=value;
        UpdateNumber();
    }

    private void UpdateNumber()
    {
        bool conditionMet = false;

        switch (condition)
        {
            case Condition.None:
                break;
            case Condition.GreaterThan:
                conditionMet = (count > targetCount);
                break;
            case Condition.LessThan:
                conditionMet = (count < targetCount);
                break;
            case Condition.EqualTo:
                conditionMet = (count == targetCount);
                break;
        }
        if (lastCount != count) onCountChanged.Invoke(count);
        if (conditionMet && onCountMeetsCondition != null) onCountMeetsCondition.Invoke(count);
        lastCount = count;
        if (displayText != null) displayText.text = "" + count;
    }

    [Serializable] public class OnCountChanged : UnityEvent<int> { }

    [Serializable] public class OnCountMeetsCondition : UnityEvent<int> { }

    public enum Condition
    {
        None, GreaterThan, LessThan, EqualTo
    }

    public void PrintAllTrashCollected(int value)
    {
        Debug.Log($"All Trash Collected: {value}");
    }

    public void PrintAllFirewoodChopped(int value)
    {
        Debug.Log($"All Firewood Chopped: {value}");
    }


}
