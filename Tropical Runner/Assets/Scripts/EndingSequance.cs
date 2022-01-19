using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSequance : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private GameObject _stackableStackingPoint;

    #endregion // Variables

    #region Start

    private void Start()
    {
        SubscribeEvents();
    }

    #endregion // Start

    #region Ending Sequance Action

    private void StackStackablesUp()
    {
        StackController stackControls = GameManager.Instance.StackControls;

        stackControls.StartCoroutine(stackControls.StackStackablesUpward(_stackableStackingPoint));
    }

    #endregion 

    #region Events

    private void SubscribeEvents()
    {
        EventManager.Instance.OnStateEndingSequance += StackStackablesUp;
    }

    private void UnsubscribeEvents()
    {
        EventManager.Instance.OnStateEndingSequance -= StackStackablesUp;
    }

    #endregion // Events

    #region OnDestroy

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion // OnDestroy
}
