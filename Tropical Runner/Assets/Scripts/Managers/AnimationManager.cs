using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region Singleton

    public static AnimationManager Instance { get; private set; }

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

        SubscribeToEvents();
    }

    #endregion // Singleton

    #region Variables

    private readonly float _animationBlendSpeed = 0.1f;
    private float _targetVertical, _targetHorizontal;
    private float _currentVertical, _currentHorizontal;

    [SerializeField]
    private GameObject _stackDestroyParticleSystemObject, _stackTransportParticleSystemObject, _goldCollectParticleSystemObject;

    #endregion // Variables

    #region Updates

    private void FixedUpdate()
    {
        SetBlendTreeParameters();
        SetAnimation();
    }

    #endregion // Updates

    #region Methods

    private void SetAnimation()
    {
        GameManager.Instance.CharacterAnimator.SetFloat("VerticalForce", _currentVertical);
        GameManager.Instance.CharacterAnimator.SetFloat("HorizontalForce", _currentHorizontal);
    }

    private void SetBlendTreeParameters()
    {
        _currentVertical = Mathf.Lerp(_currentVertical, _targetVertical, _animationBlendSpeed);
        _currentHorizontal = Mathf.Lerp(_currentHorizontal, _targetHorizontal, _animationBlendSpeed);
    }

    #region Activate Animation

    public void ActivateAnimation_Idle()
    {
        _targetVertical = 0f;
    }

    public void ActivateAnimation_Run1()
    {
        _targetVertical = 0.5f;
    }

    public void ActivateAnimation_Run2()
    {
        _targetVertical = 1f;
    }

    private void ActivateAnimation_PushBackCharacter()
    {
        _targetVertical = -1f;
    }

    public void ActivateAnimation_Dance()
    {
        _targetHorizontal = 1f;
        _targetVertical = 0f;
    }


    #endregion // Activate Animation

    #region Activate Particles

    public void ActivateParticles_TransportStacks(Vector3 spawnPosition)
    {
        GameObject stackTransportParticles =
             Instantiate(_stackTransportParticleSystemObject, spawnPosition, Quaternion.identity);

        Destroy(stackTransportParticles, 1f);
    }

    public void ActivateParticles_DestroyStack(GameObject destroyedObject)
    {
        GameObject stackDestroyingParticles =
            Instantiate(_stackDestroyParticleSystemObject, destroyedObject.transform.position, destroyedObject.transform.rotation);

        Destroy(stackDestroyingParticles, 1f);
    }

    public void ActivateParticles_CollectGold(Vector3 spawnPosition)
    {
        GameObject goldCollectParticles =
            Instantiate(_goldCollectParticleSystemObject, spawnPosition, Quaternion.identity);

        Destroy(goldCollectParticles, 1f);
    }
    #endregion // Activate Particles

    #region Camera

    private void FollowCharacter() => GameManager.Instance.CameraAnimator.Play("FollowCharacter");
    private void FollowStack() => GameManager.Instance.CameraAnimator.Play("FollowStack");

    #endregion // Camera

    #region EventHandlers

    private void SubscribeToEvents()
    {
        EventManager.Instance.OnStateTapToPlay += FollowCharacter;
        EventManager.Instance.OnStateEndingSequance += FollowStack;
        EventManager.Instance.OnStateLevelEnd += FollowCharacter;

        EventManager.Instance.OnPlayerHitObstacle += ActivateAnimation_PushBackCharacter;

        EventManager.Instance.OnStateInGame += ActivateAnimation_Run1;
        EventManager.Instance.OnStateEndingSequance += ActivateAnimation_Idle;
        EventManager.Instance.OnStateLevelEnd += ActivateAnimation_Dance;
    }

    private void UnsubscribeFromEvents()
    {
        EventManager.Instance.OnStateTapToPlay -= FollowCharacter;
        EventManager.Instance.OnStateEndingSequance -= FollowStack;
        EventManager.Instance.OnStateLevelEnd -= FollowCharacter;

        EventManager.Instance.OnPlayerHitObstacle -= ActivateAnimation_PushBackCharacter;

        EventManager.Instance.OnStateInGame -= ActivateAnimation_Run1;
        EventManager.Instance.OnStateEndingSequance -= ActivateAnimation_Idle;
        EventManager.Instance.OnStateLevelEnd -= ActivateAnimation_Dance;
    }

    #endregion // EventHandlers

    #endregion // Methods

    #region OnDestroy
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion // OnDestroy
}
