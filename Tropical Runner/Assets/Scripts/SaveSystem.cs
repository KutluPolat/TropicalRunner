using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem
{
    #region Variables

    #region Level

    public static int StackLevel
    {
        get { return PlayerPrefs.GetInt("StackLevel", 1); }
        set { PlayerPrefs.SetInt("StackLevel", value); }
    }

    public static int StartingStackCount
    {
        get { return StackLevel - 1; }
    }

    public static int Level
    {
        get { return PlayerPrefs.GetInt("Level", 1); }
        set { PlayerPrefs.SetInt("Level", value); }
    }

    public static int IndexOfLevelInSceneBuild
    {
        get { return Level - 1; }
    }

    public static int StackLevelUpgradePrice
    {
        get { return Mathf.FloorToInt(Mathf.Pow(StackLevel, 2) * 100); }
    }

    #endregion // Level

    #region Gold

    public static float TotalGold
    {
        get { return PlayerPrefs.GetFloat("Gold"); }
        set { PlayerPrefs.SetFloat("Gold", value); }
    }

    public static float CollectedGoldInLevel;

    #endregion // Gold

    #endregion // Variables

    #region Methods

    public static void CollectCurrency() => CollectedGoldInLevel++;

    public static void AddToTotalGold(float value) => TotalGold += value;

    private static void IncreaseStackLevel()
    {
        if(TotalGold > StackLevelUpgradePrice)
        {
            AddToTotalGold(-StackLevelUpgradePrice); 
            StackLevel++;
        }
    }

    private static void ResetCollectedGoldInLevel() => CollectedGoldInLevel = 0;

    public static void SubscribeEvents()
    {
        EventManager.Instance.OnPressedStackUpgrade += IncreaseStackLevel;

        EventManager.Instance.OnStateTapToPlay += ResetCollectedGoldInLevel;
    }
    public static void UnsubscribeEvents()
    {
        EventManager.Instance.OnPressedStackUpgrade -= IncreaseStackLevel;

        EventManager.Instance.OnStateTapToPlay -= ResetCollectedGoldInLevel;
    }

    #endregion
}
