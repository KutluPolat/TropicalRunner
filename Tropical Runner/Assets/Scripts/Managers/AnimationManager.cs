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

    private void Start()
    {
        InitializeAnimatorParameters();
    }

    #endregion // Singleton

    #region Variables

    private List<string> _animatorParameters = new List<string>();

    [SerializeField]
    private GameObject _stackDestroyParticleSystemObject, _stackTransportParticleSystemObject, _goldCollectParticleSystemObject;

    #endregion // Variables

    #region Methods

    #region Activate Animation

    public void ActivateAnimation_Run1()
    {
        SetAllAnimatorParametersFalse();
        GameManager.Instance.CharacterAnimator.SetBool("Run1", true);
    }
    public void ActivateAnimation_Run2()
    {
        SetAllAnimatorParametersFalse();
        GameManager.Instance.CharacterAnimator.SetBool("Run2", true);
    }
    public void ActivateAnimation_Dance()
    {
        SetAllAnimatorParametersFalse();
        GameManager.Instance.CharacterAnimator.SetBool("Dance", true);
    }
    public void ActivateAnimation_Idle()
    {
        SetAllAnimatorParametersFalse();
        GameManager.Instance.CharacterAnimator.SetBool("Idle", true);
    }

    private void ActivateAnimation_PushBackCharacter()
    {
        SetAllAnimatorParametersFalse();
        GameManager.Instance.CharacterAnimator.SetBool("PushBack", true);
    }

    #endregion // Activate Animation

    #region Animator
    public void InitializeAnimatorParameters()
    {
        foreach (AnimatorControllerParameter parameter in GameManager.Instance.CharacterAnimator.parameters)
        {
            _animatorParameters.Add(parameter.name);
        }
    }

    private void SetAllAnimatorParametersFalse()
    {
        foreach (string parameter in _animatorParameters)
            GameManager.Instance.CharacterAnimator.SetBool(parameter, false);
    }
    #endregion // Animator

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
