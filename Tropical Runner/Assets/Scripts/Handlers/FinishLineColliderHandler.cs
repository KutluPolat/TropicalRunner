using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineColliderHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.Instance.TriggerOnStateEndingSequance();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stack"))
        {
            other.GetComponent<StackObjectController>().MoveToLeft();
        }
    }
}
