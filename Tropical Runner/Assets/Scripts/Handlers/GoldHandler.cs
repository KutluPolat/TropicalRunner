using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kupinteractive.TropicalRunner.Enums;

public class GoldHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectGold();
        }
        if (other.CompareTag("Stack"))
        {
            if (other.GetComponent<StackObjectController>().IsStackStateEqualsTo(StackableState.Collected))
            {
                CollectGold();
            }
        }
    }

    private void CollectGold()
    {
        SaveSystem.CollectCurrency();
        GameManager.Instance.UIController.UpdateGoldTexts();
        AnimationManager.Instance.ActivateParticles_CollectGold(transform.position);

        Destroy(gameObject);
    }
}
