using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game state manager. Responsible for:
/// - Managing world/stage, coins, lives
/// - Handling level loading, resets, and game over
/// - Pause state and global events (coins, 1-ups, music, etc.)
/// - Ensuring a single persistent instance (singleton)
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        
        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #endregion

    #region Events

    public static event Action OnCoinCollect;
    public static event Action OnOneUp;
    public static event Action OnAboveBackgroundMusic;
    public static event Action OnPause;

    public static event Action OnLevel;

    #endregion

    #region Game State

    [Header("Game State")]
    public int World { get; private set; }
    public int Stage { get; private set; }
    public int Lives { get; private set; }
    public int Coins { get; private set; }

    public bool IsPaused { get; private set; }

    private bool _onLevel;

    private const int InitialLives = 3;
    private const int MaxCoins = 100;
    private const float DefaultResetDelay = 3f;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        Application.targetFrameRate = 100;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        HandlePauseInput();
    }

    #endregion

    #region Pause Handling

    /// <summary>
    /// Handles the pause/unpause toggle when the pause key is pressed.
    /// </summary>
    private void HandlePauseInput()
    {
        if (_onLevel)
        {
            if (Input.GetKeyDown(KeyCode.Pause) || Input.GetKeyDown(KeyCode.Escape))
            {
                IsPaused = !IsPaused;
                Time.timeScale = IsPaused ? 0f : 1f;
                OnPause?.Invoke();
            }
        } 
    }

    #endregion

    #region Game Flow

    /// <summary>
    /// Starts a new game with initial lives, coins, and level.
    /// </summary>
    public void NewGame()
    {
        Lives = InitialLives;
        Coins = 0;
        World = 1;
        Stage = 1;

        UI.Instance.UpdateCoinsCounter();
        UI.Instance.UpdateLivesCounter();
        UI.Instance.UpdateWorldStage();

        _onLevel = true;

        LoadLevel(1, 1);
        OnLevel?.Invoke();
        OnAboveBackgroundMusic?.Invoke();
    }

    /// <summary>
    /// Loads the specified world and stage.
    /// </summary>
    public void LoadLevel(int toWorld, int toStage)
    {
        World = toWorld;
        Stage = toStage;

        SceneManager.LoadScene($"{toWorld}-{toStage}");

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusic(SoundManager.Instance.backgroundMusic);
        }
    }

    /// <summary>
    /// Schedules a level reset after a delay (usually when the player dies).
    /// </summary>
    public void ResetLevel(float delay)
    {
        Lives--;
        UI.Instance.UpdateLivesCounter();
        Invoke(nameof(ResetLevel), delay);
    }

    /// <summary>
    /// Resets the current level immediately, or ends the game if out of lives.
    /// </summary>
    public void ResetLevel()
    {
        if (Lives > 0)
        {
            LoadLevel(World, Stage);
        }
        else
        {
            GameOver();
        }
    }

    /// <summary>
    /// Restarts the game from the beginning.
    /// </summary>
    public void GameOver()
    {
        NewGame();
    }

    #endregion

    #region Coins & Lives

    /// <summary>
    /// Adds a coin and checks if a new life should be awarded.
    /// </summary>
    public void AddCoin()
    {
        Coins++;

        if (Coins >= MaxCoins)
        {
            AddLife();
            Coins = 0;
            OnOneUp?.Invoke();
        }
        else
        {
            OnCoinCollect?.Invoke();
        }

        UI.Instance.UpdateCoinsCounter();
    }

    /// <summary>
    /// Awards an extra life.
    /// </summary>
    public void AddLife()
    {
        Lives++;
        UI.Instance.UpdateLivesCounter();
        OnOneUp?.Invoke();
    }

    #endregion
}
