using System.Collections;
using Blocks;
using Enemies;
using Enemies.Base;
using Level;
using Player;
using UnityEngine;

/// <summary>
/// Centralized audio manager handling all sound effects and music in the game.
/// - Subscribes to global events from gameplay systems and plays corresponding audio.
/// - Controls background music switching, SFX, and temporary effects like invincibility.
/// - Implements a persistent singleton pattern.
/// </summary>
public class SoundManager : MonoBehaviour
{
    #region Singleton

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            if (transform.parent != null)
                transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Audio Sources & Clips

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Tracks")]
    public AudioClip backgroundMusic;
    public AudioClip undergroundMusic;

    [Header("System SFX")]
    public AudioClip gameOver;
    public AudioClip stageClear;
    public AudioClip invincibility;
    public AudioClip pause;

    [Header("Gameplay SFX - Blocks & Items")]
    public AudioClip blockBreak;
    public AudioClip bump;
    public AudioClip coin;
    public AudioClip powerUpAppears;
    public AudioClip powerUp;
    public AudioClip pipePowerDown;

    [Header("Gameplay SFX - Player")]
    public AudioClip jumpSmall;
    public AudioClip jumpBig;
    public AudioClip die;
    public AudioClip oneUp;
    public AudioClip flagPole;

    [Header("Gameplay SFX - Enemies")]
    public AudioClip stomp;
    public AudioClip kick;
    

    #endregion

    #region Private Fields

    private Coroutine _invincibleRoutine;
    private AudioClip _currentMusic;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    #endregion

    #region Event Subscriptions

    private void SubscribeEvents()
    {
        // Blocks & Items
        BlockHit.OnBump += HandleBlockBump;
        BlockHit.OnBlockBreak += HandleBlockBreak;
        BlockItem.OnPowerUpAppears += HandlePowerUpAppears;

        // Player
        PlayerMovement.OnSmallJump += HandleSmallJump;
        PlayerMovement.OnBigJump += HandleBigJump;
        PlayerController.OnGrow += HandlePowerUp;
        PlayerController.OnShrink += HandlePipeEnterPowerDown;
        PlayerController.OnDeath += HandleDeath;
        PlayerController.OnInvincible += HandleInvincible;

        // Game State
        GameManager.OnOneUp += HandleOneUp;
        GameManager.OnCoinCollect += HandleCoinCollect;
        GameManager.OnAboveBackgroundMusic += HandleAboveBackgroundMusic;
        GameManager.OnPause += HandlePause;

        // Level Transitions
        Pipe.OnPipeEnter += HandlePipeEnterPowerDown;
        Pipe.OnAbove += HandleAboveBackgroundMusic;
        Pipe.OnUndeground += HandleUndergroundMusic;
        FlagPole.OnFlagPole += HandleFlagPole;
        FlagPole.OnLevelFinish += HandleLevelFinish;
        DeathBarrier.OnTouchDeathBarrier += HandleDeath;

        // Enemies
        Goomba.OnGoombaFlatten += HandleGoombaFlatten;
        Koopa.OnShellEnter += HandleKoopaShellEnter;
        Koopa.OnShellKick += HandleKoopaShellKick;
        Enemy.OnShellHit += HandleShellHit;
        Enemy.OnHit += HandleKoopaShellKick;
    }

    private void UnsubscribeEvents()
    {
        // Blocks & Items
        BlockHit.OnBump -= HandleBlockBump;
        BlockHit.OnBlockBreak -= HandleBlockBreak;
        BlockItem.OnPowerUpAppears -= HandlePowerUpAppears;

        // Player
        PlayerMovement.OnSmallJump -= HandleSmallJump;
        PlayerMovement.OnBigJump -= HandleBigJump;
        PlayerController.OnGrow -= HandlePowerUp;
        PlayerController.OnShrink -= HandlePipeEnterPowerDown;
        PlayerController.OnDeath -= HandleDeath;
        PlayerController.OnInvincible -= HandleInvincible;

        // Game State
        GameManager.OnOneUp -= HandleOneUp;
        GameManager.OnCoinCollect -= HandleCoinCollect;
        GameManager.OnAboveBackgroundMusic -= HandleAboveBackgroundMusic;
        GameManager.OnPause -= HandlePause;

        // Level Transitions
        Pipe.OnPipeEnter -= HandlePipeEnterPowerDown;
        Pipe.OnAbove -= HandleAboveBackgroundMusic;
        Pipe.OnUndeground -= HandleUndergroundMusic;
        FlagPole.OnFlagPole -= HandleFlagPole;
        FlagPole.OnLevelFinish -= HandleLevelFinish;
        DeathBarrier.OnTouchDeathBarrier -= HandleDeath;

        // Enemies
        Goomba.OnGoombaFlatten -= HandleGoombaFlatten;
        Koopa.OnShellEnter -= HandleKoopaShellEnter;
        Koopa.OnShellKick -= HandleKoopaShellKick;
        Enemy.OnShellHit -= HandleShellHit;
        Enemy.OnHit -= HandleKoopaShellKick;
    }

    #endregion

    #region Event Handlers - Gameplay

    private void HandleBlockBump() => PlaySfx(bump);
    private void HandleBlockBreak() => PlaySfx(blockBreak);
    private void HandlePowerUpAppears() => PlaySfx(powerUpAppears);
    private void HandlePowerUp() => PlaySfx(powerUp);
    private void HandlePipeEnterPowerDown() => PlaySfx(pipePowerDown);
    private void HandleCoinCollect() => PlaySfx(coin);
    private void HandleOneUp() => PlaySfx(oneUp);

    #endregion

    #region Event Handlers - Player

    private void HandleSmallJump() => PlaySfx(jumpSmall);
    private void HandleBigJump() => PlaySfx(jumpBig);
    private void HandleDeath()
    {
        StopMusic();
        PlaySfx(die);
    }

    private void HandleInvincible()
    {
        if (_invincibleRoutine != null)
        {
            StopCoroutine(_invincibleRoutine);
        }

        _currentMusic = musicSource.clip;

        StopMusic();
        PlayMusic(invincibility);
        _invincibleRoutine = StartCoroutine(InvincibilityRoutine());
    }

    private IEnumerator InvincibilityRoutine()
    {
        yield return new WaitForSecondsRealtime(9f);

        PlayMusic(_currentMusic);
        _invincibleRoutine = null;
    }

    #endregion

    #region Event Handlers - Enemies

    private void HandleShellHit() => PlaySfx(stomp);
    private void HandleGoombaFlatten() => PlaySfx(stomp);
    private void HandleKoopaShellEnter() => PlaySfx(stomp);
    private void HandleKoopaShellKick() => PlaySfx(kick);

    #endregion

    #region Event Handlers - Level & System

    private void HandleFlagPole()
    {
        StopMusic();
        PlaySfx(flagPole);
    }

    private void HandleLevelFinish()
    {
        StopMusic();
        PlaySfx(stageClear);
    }

    private void HandleAboveBackgroundMusic()
    {
        StopMusic();
        PlayMusic(backgroundMusic);
    }

    private void HandleUndergroundMusic()
    {
        StopMusic();
        PlayMusic(undergroundMusic);
    }

    private void HandlePause()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            musicSource.Pause();
        else
            musicSource.Play();

        PlaySfx(pause);
    }

    #endregion

    #region Audio Utilities

    /// <summary>Plays background music and loops it until stopped or replaced.</summary>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    /// <summary>Stops the currently playing background music.</summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }

    /// <summary>Plays a one-shot sound effect.</summary>
    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    #endregion
}
