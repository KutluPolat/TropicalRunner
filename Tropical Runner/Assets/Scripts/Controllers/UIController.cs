using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private Button _upgradeStack, _restart, _nextLevel;

    [SerializeField]
    private TextMeshProUGUI _bigTotalGoldText, _stackedObjectCountText, _smallInGameCollectedGoldText, _bigInGameCollectedGoldText, _levelText, _titleText,
        _upgradeLevelText, _upgradePriceText;

    #endregion // Variables

    #region Start

    private void Start()
    {
        SubscribeEvents();

        UpdateLevelText();
        UpdateGoldTexts();
        UpdateUpgradeButtonTexts();
        UpdateStackCounterText();
    }

    #endregion // Start

    #region Methods

    #region Button Triggers

    public void TriggerUpgradeStack() => EventManager.Instance.TriggerOnPressedStackUpgrade();
    public void TriggerRestart() => EventManager.Instance.TriggerOnPressedRestart();
    public void TriggerNextLevel() => EventManager.Instance.TriggerOnPressedNextLevel();

    #endregion // Button Triggers

    #region Switch Screen

    private void SwitchScreenToTapToPlay()
    {
        _restart.gameObject.SetActive(false);
        _nextLevel.gameObject.SetActive(false);

        _upgradeStack.gameObject.SetActive(true);

        TapToPlayTexts();
    }

    private void SwitchScreenToInGame()
    {
        _nextLevel.gameObject.SetActive(false);
        _upgradeStack.gameObject.SetActive(false);

        _restart.gameObject.SetActive(true);

        InGameTexts();
    }

    private void SwitchScreenToEndingSequance()
    {
        LevelEndTexts();
    }

    private void SwitchScreenToLevelEnd()
    {
        _upgradeStack.gameObject.SetActive(false);
        _restart.gameObject.SetActive(false);

        EndingSequanceTexts();

        StartCoroutine(AddCollectedGoldToTotalGold());
    }

    private IEnumerator AddCollectedGoldToTotalGold()
    {
        float oneOfFifthCollectedGold = SaveSystem.CollectedGoldInLevel / 5f;

        yield return new WaitForSeconds(1f);

        for (int i= 0; i< 5; i++)
        {
            SaveSystem.AddToTotalGold(oneOfFifthCollectedGold);
            SaveSystem.CollectedGoldInLevel -= oneOfFifthCollectedGold;
            UpdateGoldTexts();

            Haptic.Vibrate();
            yield return new WaitForSeconds(0.1f);
        }

        _nextLevel.gameObject.SetActive(true);
    }

    #endregion // Switch Screen

    #region Switch Texts

    private void TapToPlayTexts()
    {
        _bigInGameCollectedGoldText.gameObject.SetActive(false);
        _smallInGameCollectedGoldText.gameObject.SetActive(false);
        _stackedObjectCountText.gameObject.SetActive(false);

        _titleText.gameObject.SetActive(true);
        _bigTotalGoldText.gameObject.SetActive(true);
        OpenUpgradeTexts();
    }

    private void InGameTexts()
    {
        _titleText.gameObject.SetActive(false);
        _bigTotalGoldText.gameObject.SetActive(false);
        _bigInGameCollectedGoldText.gameObject.SetActive(false);

        _smallInGameCollectedGoldText.gameObject.SetActive(true);
        _stackedObjectCountText.gameObject.SetActive(true);
        CloseUpgradeTexts();
    }

    private void EndingSequanceTexts()
    {
        _smallInGameCollectedGoldText.gameObject.SetActive(false);
        _stackedObjectCountText.gameObject.SetActive(false);

        _bigTotalGoldText.gameObject.SetActive(true);
        _bigInGameCollectedGoldText.gameObject.SetActive(true);
    }

    private void LevelEndTexts()
    {
        _bigTotalGoldText.gameObject.SetActive(false);
    }


    private void CloseUpgradeTexts()
    {
        _upgradeLevelText.gameObject.SetActive(false);
        _upgradePriceText.gameObject.SetActive(false);
    }

    private void OpenUpgradeTexts()
    {
        _upgradeLevelText.gameObject.SetActive(true);
        _upgradePriceText.gameObject.SetActive(true);
    }

    #endregion // Switch Texts

    #region Sub & Unsub Events

    private void SubscribeEvents()
    {
        EventManager.Instance.OnStateInGame += SwitchScreenToInGame;
        EventManager.Instance.OnStateTapToPlay += SwitchScreenToTapToPlay;
        EventManager.Instance.OnStateEndingSequance += SwitchScreenToEndingSequance;
        EventManager.Instance.OnStateLevelEnd += SwitchScreenToLevelEnd;

        EventManager.Instance.OnPressedStackUpgrade += UpdateGoldTexts;
        EventManager.Instance.OnPressedStackUpgrade += UpdateUpgradeButtonTexts;
    }

    private void UnsubscribeEvents()
    {
        EventManager.Instance.OnStateInGame -= SwitchScreenToInGame;
        EventManager.Instance.OnStateTapToPlay -= SwitchScreenToTapToPlay;
        EventManager.Instance.OnStateEndingSequance -= SwitchScreenToEndingSequance;
        EventManager.Instance.OnStateLevelEnd -= SwitchScreenToLevelEnd;

        EventManager.Instance.OnPressedStackUpgrade -= UpdateGoldTexts;
        EventManager.Instance.OnPressedStackUpgrade -= UpdateUpgradeButtonTexts;
    }

    #endregion // Sub & Unsub Events

    #region Update Texts

    public void UpdateUpgradeButtonTexts()
    {
        _upgradeLevelText.text = "Lvl. " + SaveSystem.StackLevel;
        _upgradePriceText.text = "Price: " + SaveSystem.StackLevelUpgradePrice;
    }

    public void UpdateStackCounterText() => _stackedObjectCountText.text = "Stacked: " + GameManager.Instance.StackControls.CurrentLengthOfStack;

    public void UpdateGoldTexts()
    {
        _bigTotalGoldText.text = "Total: " + Mathf.FloorToInt(SaveSystem.TotalGold);

        SaveSystem.CollectedGoldInLevel = Mathf.Clamp(SaveSystem.CollectedGoldInLevel, 0, Mathf.Infinity);

        _smallInGameCollectedGoldText.text = "Collected: " + Mathf.FloorToInt(SaveSystem.CollectedGoldInLevel);
        _bigInGameCollectedGoldText.text = "Collected: " + Mathf.FloorToInt(SaveSystem.CollectedGoldInLevel);
    }

    private void UpdateLevelText()
    {
        _levelText.text = "Level: " + SaveSystem.Level;
    }

    #endregion // Update Texts

    #region Feedbacks

    public void BigInGameGoldFeedback()
    {
        GameManager.Instance.Feedback.StartFeedback(_bigInGameCollectedGoldText.gameObject);
    }

    #endregion // Feedbacks

    #endregion // Methods

    #region OnDestroy

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion // OnDestroy
}
