using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColliderHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            EventManager.Instance.TriggerOnPlayerHitObstacle();
        }
    }
}
