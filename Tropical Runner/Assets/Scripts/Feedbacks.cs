using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;

public class Feedbacks : MonoBehaviour
{
    #region Variables

    private const float FEEDBACK_INTERVAL = 0.2f;

    #endregion // Variables

    #region Methods

    public void StartFeedback(GameObject textObject)
    {
        StartCoroutine(SendFeedback(textObject));
    }

    private IEnumerator SendFeedback(GameObject textObject)
    {
        textObject.transform.localScale = Vector3.one;
        textObject.transform.DOScale(1.5f, FEEDBACK_INTERVAL);
        textObject.transform.DOPunchRotation(Vector3.one * 4, FEEDBACK_INTERVAL);

        yield return new WaitForSeconds(FEEDBACK_INTERVAL);

        textObject.transform.DOScale(1, FEEDBACK_INTERVAL);

        yield return new WaitForSeconds(FEEDBACK_INTERVAL);
    }

    #endregion // Methods
}
