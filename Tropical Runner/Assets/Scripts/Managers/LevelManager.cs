using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kupinteractive.TropicalRunner.Enums;

public class LevelManager : MonoBehaviour
{
    #region Singleton

    public static LevelManager Instance { get; private set; }
    void Awake()
    {
        InitializeLevel();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SubscribeEvents();
    }

    #endregion // Singleton

    #region Variables
    public GameStates CurrentGameState;
    #endregion // Variables

    #region Methods

    #region Game State Controls

    public bool IsGameStateEqualsTo(GameStates state)
    {
        return CurrentGameState == state;
    }
    private void SetGameStateTo(GameStates newGameState) => CurrentGameState = newGameState;

    #endregion // Game State Controls

    #region Scene Management

    public void InitializeLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex != SaveSystem.IndexOfLevelInSceneBuild)
            ReloadActiveScene();
    }

    private void NextLevel()
    {
        SaveSystem.Level++;

        if (SaveSystem.Level > SceneManager.sceneCountInBuildSettings)
            SaveSystem.Level = 1;

        SceneManager.LoadScene(SaveSystem.IndexOfLevelInSceneBuild);
    }

    private void ReloadActiveScene() => SceneManager.LoadScene(SaveSystem.IndexOfLevelInSceneBuild);

    #endregion // Scene Management

    #region Sub & Unsub Events

    private void SubscribeEvents()
    {
        EventManager.Instance.OnStateTapToPlay += () => SetGameStateTo(GameStates.TapToPlay);
        EventManager.Instance.OnStateInGame += () => SetGameStateTo(GameStates.InGame);
        EventManager.Instance.OnStateEndingSequance += () => SetGameStateTo(GameStates.EndingSequance);
        EventManager.Instance.OnStateLevelEnd += () => SetGameStateTo(GameStates.LevelEnd);

        EventManager.Instance.OnPressedRestart += ReloadActiveScene;
        EventManager.Instance.OnPressedNextLevel += NextLevel;
    }
    private void UnsubscribeEvents()
    {
        EventManager.Instance.OnStateTapToPlay -= () => SetGameStateTo(GameStates.TapToPlay);
        EventManager.Instance.OnStateInGame -= () => SetGameStateTo(GameStates.InGame);
        EventManager.Instance.OnStateEndingSequance -= () => SetGameStateTo(GameStates.EndingSequance);
        EventManager.Instance.OnStateLevelEnd -= () => SetGameStateTo(GameStates.LevelEnd);

        EventManager.Instance.OnPressedRestart -= ReloadActiveScene;
        EventManager.Instance.OnPressedNextLevel -= NextLevel;
    }

    #endregion // Sub & Unsub Events

    #region OnDestroy

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion // OnDestroy

    #endregion // Methods
}
