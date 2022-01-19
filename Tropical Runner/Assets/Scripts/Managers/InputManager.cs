using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kupinteractive.TropicalRunner.Enums;

public class InputManager : MonoBehaviour
{
    #region Singleton

    public static InputManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    #endregion // Singleton

    #region Variables

    private Vector3 _inputDirection;

    [Range(0.01f, 1f)]
    private readonly float _touchInputSpeed = 0.1f;

    #endregion // Variables

    #region Updates
    void FixedUpdate()
    {
        Inputs();
    }
    #endregion // Updates

    #region Methods

    public float GetHorizontalInput()
    {
        Vector3 inputHolder = _inputDirection;

        _inputDirection = Vector3.zero;

        return inputHolder.x;
    }

    private void Inputs()
    {
#if UNITY_EDITOR
        UnityEditorInputs();
#elif PLATFORM_ANDROID
        AndroidInputs();
#endif

        StartGameIfPlayerSwipes();
    }

    private void UnityEditorInputs()
    {
        if (Input.GetKey(KeyCode.A))
            _inputDirection += -GameManager.Instance.CharacterTransform.right;

        if (Input.GetKey(KeyCode.D))
            _inputDirection += GameManager.Instance.CharacterTransform.right;
    }

    private void AndroidInputs()
    {
        if (Input.touchCount > 0)
        {
            float horizontalTouchDeltaPosition = Input.touches[0].deltaPosition.x;

            _inputDirection += GameManager.Instance.CharacterTransform.right * horizontalTouchDeltaPosition * _touchInputSpeed;
        }
    }

    private void StartGameIfPlayerSwipes()
    {
        if (_inputDirection != Vector3.zero && LevelManager.Instance.CurrentGameState == GameStates.TapToPlay)
            EventManager.Instance.TriggerOnStateInGame();
    }
    #endregion // Methods
}
