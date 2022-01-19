using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class StackController : MonoBehaviour
{
    #region Variables

    #region Properties

    public int CurrentLengthOfStack
    {
        get { return _stackedObjects.Count; }
    }

    public bool IsStackNotFull
    {
        get
        {
            return CurrentLengthOfStack < MaximumLengthOfStack;
        }
    }

    public bool IsStackFull
    {
        get
        {
            return CurrentLengthOfStack == MaximumLengthOfStack;
        }
    }

    public GameObject LeadingStack
    {
        get
        {
            return _stackedObjects[LeadingStackIndex];
        }
    }

    private int LeadingStackIndex
    {
        get { return _stackedObjects.Count - 1; }
    }

    #endregion // Properties

    #region Fields

    public int MaximumLengthOfStack { get { return 30; } }

    private List<GameObject> _stackedObjects = new List<GameObject>();

    [SerializeField]
    private GameObject _stackPrefab;

    private readonly float _expandAndShrinkDelayBetweenStacks = 0.05f, _stackingDelay = 0.1f, _endingSequanceDelay = 1f;

    #endregion // Fields

    #endregion // Variables

    #region Start

    private void Awake()
    {
        SubscribeEvents();
    }

    #endregion // Start

    #region Methods

    #region Add & Remove Elements On List

    public void AddStackToStackList(GameObject collectedStack)
    {
        if (IsStackNotFull)
        {
            _stackedObjects.Add(collectedStack);

            StartCoroutine(ExpandAndShrinkTheStackedObjects());
        }
    }

    public void RemoveStackFromStackList(GameObject spilledStack) => _stackedObjects.Remove(spilledStack);

    #endregion // Add & Remove Elements On List

    #region Stack Actions

    #region Ending Sequance

    public IEnumerator StackStackablesUpward(GameObject stackSpawnPosition)
    {
        yield return new WaitForSeconds(_endingSequanceDelay);

        foreach(GameObject stack in _stackedObjects)
        {
            Vector3 spawnPositionYOffset = Vector3.up;
            stackSpawnPosition.transform.position = stackSpawnPosition.transform.position + spawnPositionYOffset;

            stack.transform.position = stackSpawnPosition.transform.position;

            AnimationManager.Instance.ActivateParticles_TransportStacks(stackSpawnPosition.transform.position);

            yield return new WaitForSeconds(_stackingDelay);
        }

        CalculateNewCollectedGoldValueAccordingToHeight();

        yield return new WaitForSeconds(_endingSequanceDelay);

        EventManager.Instance.TriggerOnStateLevelEnd();
    }

    private void CalculateNewCollectedGoldValueAccordingToHeight()
    {
        RaycastHit hit;
        Transform leadingStackTransform = _stackedObjects[LeadingStackIndex].transform;

        if (Physics.Raycast(leadingStackTransform.position, leadingStackTransform.forward, out hit))
        {
            Transform transformOfMultiplierText = hit.collider.transform;
            Transform transformOfMultiplierCube = transformOfMultiplierText.parent.parent.transform;

            transformOfMultiplierCube.DOMove(transformOfMultiplierCube.position + Vector3.back, 1f);

            SaveSystem.CollectedGoldInLevel *= int.Parse(hit.collider.GetComponent<TMPro.TextMeshPro>().text);

            GameManager.Instance.UIController.UpdateGoldTexts();
            GameManager.Instance.UIController.BigInGameGoldFeedback();
        }
    }

    #endregion // Ending Sequance

    #region In Game

    public void PushRestOfTheStackedStacksAway(GameObject stackThatTouchedObstacle)
    {
        int indexOfStackThatTouchedObstacle = _stackedObjects.IndexOf(stackThatTouchedObstacle);

        for (int i = LeadingStackIndex; i > indexOfStackThatTouchedObstacle; i--)
        {
            _stackedObjects[i].GetComponent<StackObjectController>().PushStackAway();
            _stackedObjects[i].GetComponent<StackObjectController>().GetOffFromStackList();
        }

        _stackedObjects[indexOfStackThatTouchedObstacle].GetComponent<StackObjectController>().GetOffFromStackList();
    }

    private IEnumerator ExpandAndShrinkTheStackedObjects()
    {
        for (int i = LeadingStackIndex; i >= 0; i--)
        {
            if (i > LeadingStackIndex)
            {
                Debug.LogWarning("Changes detected on the stacked object list, expand and shrink loops has been stopped.");
                break;
            }
                

            if(_stackedObjects[i] != null)
            {
                _stackedObjects[i].GetComponent<StackObjectController>().ExpandAndShrink();
            }
            
            yield return new WaitForSeconds(_expandAndShrinkDelayBetweenStacks);
        }
    }

    private void PushAllOfTheStackedStacksAway()
    {
        for (int i = LeadingStackIndex; i >= 0; i--) // foreach (GameObject stack in _stackedObjects)
        {
            _stackedObjects[i].GetComponent<StackObjectController>().PushStackAway();
            _stackedObjects[i].GetComponent<StackObjectController>().GetOffFromStackList();
        }
    }

    #endregion // In Game

    #endregion // Stack Actions

    #region Initialization Of Stack

    private void InitializeStartingStack()
    {
        for (int i = CurrentLengthOfStack; i < SaveSystem.StartingStackCount; i++)
        {
            InstantiateStack();
        }
    }

    
    private void InstantiateStack()
    {
        if(CurrentLengthOfStack < MaximumLengthOfStack)
        {
            Vector3 stackYOffset = Vector3.up * 0.75f;
            Vector3 stackZOffset = Vector3.forward * (CurrentLengthOfStack + 1);

            Vector3 targetPosition = GameManager.Instance.CharacterTransform.position + stackZOffset + stackYOffset;

            GameObject instantiatedStack = Instantiate(_stackPrefab, targetPosition, Quaternion.identity);

            if(CurrentLengthOfStack == 0)
            {
                instantiatedStack.GetComponent<StackObjectController>().ConnectToCharacter();
            }
            else
            {
                instantiatedStack.GetComponent<StackObjectController>().ConnectToCollidedStack();
            }
        }
    }

    #endregion // Initialization Of Stack

    #region Events

    private void SubscribeEvents()
    {
        EventManager.Instance.OnPressedStackUpgrade += InitializeStartingStack;
        EventManager.Instance.OnStateTapToPlay += InitializeStartingStack;
        EventManager.Instance.OnPlayerHitObstacle += PushAllOfTheStackedStacksAway;
    }

    private void UnsubscribeEvents()
    {
        EventManager.Instance.OnPressedStackUpgrade -= InitializeStartingStack;
        EventManager.Instance.OnStateTapToPlay -= InitializeStartingStack;
        EventManager.Instance.OnPlayerHitObstacle += PushAllOfTheStackedStacksAway;
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
