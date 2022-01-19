using System.Collections;
using UnityEngine;
using DG.Tweening;
using Kupinteractive.TropicalRunner.Enums;

public class StackObjectController : MonoBehaviour
{

    #region Variables

    private bool _isConnectedToPlayer, _isAtCheckstand;
    private GameObject _connectedNode;

    [SerializeField]
    private float _stacksHorizontalFollowingSpeed = 10f, _distanceBetweenStacks = 1f;

    private readonly float _maxZPush = 10f, _minZPush = 5f, _maxXPush = 2f, _minXPush = -2f, _rightEdgeOfPlatform = 2f, _leftEdgeOfPlatform = -2f, _jumpPower = 2f;

    private StackableState _currentState;


    private Coroutine _expandAndShrinkCoroutine;
    private readonly float _maxScale = 1.4f, _originalScale = 1f, _expandAndShrinkPeriod = 0.15f;

    #endregion // Variables

    #region Updates

    private void FixedUpdate()
    {
        Move();
    }

    #endregion // Updates

    #region Methods

    #region Stack List Controls

    public void GetOffFromStackList()
    {
        GameManager.Instance.StackControls.RemoveStackFromStackList(gameObject);

        if(IsStackStateEqualsTo(StackableState.Collected))
        {
            SetStackStateTo(StackableState.NotCollected);
        }

        _isConnectedToPlayer = false;
        _connectedNode = null;

        GameManager.Instance.Stackbar.RefreshStackbar();
    }

    private void GetInToStackList()
    {
        GameManager.Instance.StackControls.AddStackToStackList(gameObject);

        GameManager.Instance.Stackbar.RefreshStackbar();
    }

    #endregion // Stack List Controls

    #region Connections

    public void ConnectToCollidedStack()
    {
        _connectedNode = GameManager.Instance.StackControls.LeadingStack;

        GetInToStackList();
        GameManager.Instance.UIController.UpdateStackCounterText();

        SetStackStateTo(StackableState.Collected);

        if(GameManager.Instance.MovementControls.IsMovementBlocked == false && LevelManager.Instance.IsGameStateEqualsTo(GameStates.InGame))
        {
            if (GameManager.Instance.StackControls.IsStackFull)
                AnimationManager.Instance.ActivateAnimation_Run2();
            else
                AnimationManager.Instance.ActivateAnimation_Run1();
        }
    }

    public void ConnectToCharacter()
    {
        GetInToStackList();
        GameManager.Instance.UIController.UpdateStackCounterText();

        _isConnectedToPlayer = true;
        SetStackStateTo(StackableState.Collected);

        _connectedNode = GameManager.Instance.CharacterTransform.gameObject;
    }

    #endregion // Connections

    #region Expand And Shrink

    public void ExpandAndShrink()
    {
        if(_expandAndShrinkCoroutine != null)
        {
            StopCoroutine(_expandAndShrinkCoroutine);
            _expandAndShrinkCoroutine = null;
        }

        _expandAndShrinkCoroutine = StartCoroutine(ExpandAndShrinkCoroutine());
    }

    private IEnumerator ExpandAndShrinkCoroutine()
    {
        float halfPeriod = _expandAndShrinkPeriod / 2f;

        transform.DOScale(_maxScale, halfPeriod);
        yield return new WaitForSeconds(halfPeriod);
        transform.DOScale(_originalScale, halfPeriod);
    }

    #endregion // Expand And Shrink

    #region Movement

    public void MoveToLeft()
    {
        _isAtCheckstand = true;
        transform.DOMoveX(_leftEdgeOfPlatform * 5f, 0.5f);
    }

    public void PushStackAway()
    {
        Vector3 pushDirection = new Vector3(Random.Range(_minXPush, _maxXPush), 0, Random.Range(_minZPush, _maxZPush));
        Vector3 targetPosition = transform.position + pushDirection;

        targetPosition.x = Mathf.Clamp(targetPosition.x, _leftEdgeOfPlatform, _rightEdgeOfPlatform);

        StartCoroutine(MoveToGivenPosition(targetPosition));
    }

    private IEnumerator MoveToGivenPosition(Vector3 targetPosition)
    {
        SetStackStateTo(StackableState.NotCollected_PushedAway);
        transform.DOJump(targetPosition, _jumpPower, 1, 0.5f);

        yield return new WaitForSeconds(0.5f);

        ExpandAndShrink();
        SetStackStateTo(StackableState.NotCollected);
    }


    private void Move()
    {
        if (LevelManager.Instance.IsGameStateEqualsTo(GameStates.InGame) && IsStackStateEqualsTo(StackableState.Collected)  && _isAtCheckstand == false)
        {
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, _connectedNode.transform.position.x, Time.deltaTime * _stacksHorizontalFollowingSpeed),
                transform.position.y,
                _connectedNode.transform.position.z + _distanceBetweenStacks);
        }
    }
    
    #endregion // Movement

    #region Destroy

    private void DestroyStack()
    {
        if (IsStackStateEqualsTo(StackableState.NotCollected_PushedAway))
        {
            // Do nothing.
        }
        else
        {
            GameManager.Instance.StackControls.PushRestOfTheStackedStacksAway(gameObject);
        }

        GameManager.Instance.StackControls.StopAllCoroutines();
        AnimationManager.Instance.ActivateParticles_DestroyStack(gameObject);


        Destroy(gameObject);
    }

    #endregion // Destroy

    #region Controlling States

    public void SetStackStateTo(StackableState newState) => _currentState = newState;
    public bool IsStackStateEqualsTo(StackableState state)
    {
        return _currentState == state;
    }

    #endregion // Controlling States

    #endregion // Methods

    #region Unity Methods

    #region OnTriggerEnter

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.StackControls.IsStackNotFull && IsStackStateEqualsTo(StackableState.NotCollected))
        {
            if (other.CompareTag("Stack") && other.GetComponent<StackObjectController>().IsStackStateEqualsTo(StackableState.Collected))
            {
                ConnectToCollidedStack();
            }
            else if (other.CompareTag("Player") && GameManager.Instance.StackControls.CurrentLengthOfStack == 0)
            {
                ConnectToCharacter();
            }
            else if (other.CompareTag("Player"))
            {
                ConnectToCollidedStack();
            }
        }

        if (other.CompareTag("Obstacle"))
        {
            DestroyStack();
        }
    }

    #endregion // OnTriggerEnter

    #endregion // Unity Methods
}
