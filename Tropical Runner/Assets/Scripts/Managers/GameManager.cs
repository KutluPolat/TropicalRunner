using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

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

        // These method call order is also represents subscription(adding listener) order to events. Thus, it's important to be in exact this order.
        InitializeLevelElements();
        SaveSystem.SubscribeEvents();
        InitializeControllerInstances();
    }



    #endregion // Singleton

    #region Start

    private void Start()
    {
        EventManager.Instance.TriggerOnStateTapToPlay();
    }

    #endregion // Start

    #region Variables

    public Transform CharacterTransform;
    public Animator CharacterAnimator, CameraAnimator;
    public CharacterController CharacterController;

    [SerializeField]
    private GameObject _inputManager, _animationManager, _levelManager, _eventManager;

    [SerializeField]
    private GameObject _movementController, _stackController;

    public EndingSequance EndingSequance;
    public SaveSystem SaveSystem;
    public UIController UIController;
    public Feedbacks Feedback;
    public StackbarHandler Stackbar;

    public StackController StackControls { get; private set; }
    public MovementController MovementControls { get; private set; }

    #endregion // Variables

    #region Methods

    private void InitializeLevelElements()
    {
        Instantiate(_eventManager, transform.parent);
        Instantiate(_inputManager, transform.parent);
        Instantiate(_animationManager, transform.parent);
        Instantiate(_levelManager, transform.parent);
    }

    private void InitializeControllerInstances()
    {
        GameObject controllers = new GameObject();
        controllers.name = "Controllers";
        controllers.transform.parent = transform.parent.parent;

        StackControls = Instantiate(_stackController, controllers.transform).GetComponent<StackController>();
        MovementControls = Instantiate(_movementController, controllers.transform).GetComponent<MovementController>();
    }

    private void OnDestroy()
    {
        SaveSystem.UnsubscribeEvents();
    }
    #endregion // Methods
}
