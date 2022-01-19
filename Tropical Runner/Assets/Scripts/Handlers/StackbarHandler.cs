using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackbarHandler : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private Slider _stackBar;

    #endregion // Variables

    #region Start

    private void Start()
    {
        SubscribeEvents();

        _stackBar.minValue = 0;
        _stackBar.maxValue = GameManager.Instance.StackControls.MaximumLengthOfStack;

        DeactivateStackbar();
    }

    #endregion // Start

    #region Methods

    #region Stackbar

    public void RefreshStackbar() => _stackBar.value = GameManager.Instance.StackControls.CurrentLengthOfStack;

    private void ActivateStackbar() => gameObject.SetActive(true);
    private void DeactivateStackbar() => gameObject.SetActive(false);

    #endregion // Stackbar

    #region Events

    private void SubscribeEvents()
    {
        EventManager.Instance.OnStateInGame += ActivateStackbar;
        EventManager.Instance.OnStateEndingSequance += DeactivateStackbar;
    }
    private void UnsubscribeEvents()
    {
        EventManager.Instance.OnStateInGame -= ActivateStackbar;
        EventManager.Instance.OnStateEndingSequance -= DeactivateStackbar;
    }

    #endregion // Events

    #region OnDestroy

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion // OnDestroy

    #endregion // Methods
}
