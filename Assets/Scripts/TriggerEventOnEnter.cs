using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class TriggerEventOnEnter : MonoBehaviour
{
    public UnityEvent OnTriggered;
    public string tagName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider c)
    {
        if (!isActiveAndEnabled)
			return;

        if (c.gameObject.tag == tagName)
        {
            OnTriggered.Invoke();
        }
    }
}
