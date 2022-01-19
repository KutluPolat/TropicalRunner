using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
#if UNITY_EDITOR

    [Button]
    private void UpgradeStack() => EventManager.Instance.TriggerOnPressedStackUpgrade();

    [Button]
    private void ResetStackLevel() => SaveSystem.StackLevel = 1;

    [Button]
    private void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    [Button]
    private void LogGold() => Debug.Log("Gold: " + SaveSystem.TotalGold);

    // Add private to gold set later.
    [Button]
    private void ResetGold() => SaveSystem.TotalGold = 0;

    [Button]
    private void AddMillionToGold() => SaveSystem.TotalGold = 1000000;

#endif
}
