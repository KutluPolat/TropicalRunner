using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovementController : MonoBehaviour
{
    #region Variables

    private Vector3 _movementDirection;
    public bool IsMovementBlocked { get; private set; }

    private readonly float _characterVerticalSpeed = 0.12f, _characterHorizontalSpeed = 0.2f;

    #endregion // Variables

    #region Start

    private void Awake()
    {
        SubscribeEvents();
    }

    private void Start()
    {
        _movementDirection = new Vector3(0, 0, _characterVerticalSpeed);
    }

    #endregion // Start

    #region Updates

    private void FixedUpdate()
    {
        Move(InputManager.Instance.GetHorizontalInput());
    }

    #endregion // Updates

    #region Methods

    private void Move(float horizontalInput)
    {
        if(IsMovementBlocked == false)
        {
            _movementDirection.x = horizontalInput * _characterHorizontalSpeed;

            GameManager.Instance.CharacterController.Move(_movementDirection);
        }
    }

    private void PushPlayerBack()
    {
        StartCoroutine(PushPlayerBackCoroutine());
    }

    private IEnumerator PushPlayerBackCoroutine()
    {
        BlockContinuousMovement();

        Vector3 offset = Vector3.back * 3;
        Vector3 endValue = GameManager.Instance.CharacterTransform.position + offset;

        float jumpPower = 2f;
        int numJumps = 1;
        float duration = 1f;

        GameManager.Instance.CharacterTransform.DOJump(endValue, jumpPower, numJumps, duration);

        yield return new WaitForSeconds(duration);

        UnblockContinuousMovement();
        AnimationManager.Instance.ActivateAnimation_Run1();
    }

    private void BlockContinuousMovement()
    {
        IsMovementBlocked = true;
    }

    private void UnblockContinuousMovement()
    {
        IsMovementBlocked = false;
    }

    private void SubscribeEvents()
    {
        EventManager.Instance.OnStateTapToPlay += BlockContinuousMovement;
        EventManager.Instance.OnStateInGame += UnblockContinuousMovement;
        EventManager.Instance.OnStateEndingSequance += BlockContinuousMovement;

        EventManager.Instance.OnPlayerHitObstacle += PushPlayerBack;
    }

    private void UnsubscribeEvents()
    {
        EventManager.Instance.OnStateTapToPlay -= BlockContinuousMovement;
        EventManager.Instance.OnStateInGame -= UnblockContinuousMovement;
        EventManager.Instance.OnStateEndingSequance -= BlockContinuousMovement;

        EventManager.Instance.OnPlayerHitObstacle -= PushPlayerBack;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion // Methods
}
