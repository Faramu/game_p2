using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton untuk tracking semua analytics event di game
/// Persist antar scene dan otomatis tracking berbagai event
/// </summary>
public class AnalyticsTracker : MonoBehaviour
{
    // ==========================================
    // SINGLETON INSTANCE
    // ==========================================
    public static AnalyticsTracker Instance { get; private set; }

    // ==========================================
    // DATA ANALYTICS
    // ==========================================
    public GameAnalytics CurrentSession { get; private set; }

    // ==========================================
    // TRACKING VARIABLES
    // ==========================================
    private Vector3 lastPosition;
    private float sessionStartTime;
    private bool isSessionActive = false;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    // ==========================================
    // UNITY LIFECYCLE
    // ==========================================
    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Start new session
            StartNewSession();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (!isSessionActive) return;

        // Track distance moved (jika Player ada)
        TrackPlayerMovement();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ==========================================
    // SESSION MANAGEMENT
    // ==========================================
    /// <summary>
    /// Mulai sesi baru
    /// </summary>
    public void StartNewSession()
    {
        CurrentSession = new GameAnalytics();
        sessionStartTime = Time.time;
        isSessionActive = true;

        if (enableDebugLogs)
            Debug.Log("📊 Analytics: New session started - " + CurrentSession.sessionId);
    }

    /// <summary>
    /// End current session dan prepare data untuk dikirim
    /// </summary>
    public void EndSession(bool completed = false)
    {
        if (!isSessionActive) return;

        CurrentSession.levelCompleted = completed;
        CurrentSession.FinalizeSession();
        isSessionActive = false;

        if (enableDebugLogs)
        {
            Debug.Log("📊 ===== SESSION ENDED =====");
            Debug.Log(CurrentSession.GetSummary());
        }
    }

    // ==========================================
    // TRACKING METHODS - COINS & SCORE
    // ==========================================
    public void TrackCoinCollected()
    {
        CurrentSession.totalCoinsCollected++;
        LogEvent("Coin collected", CurrentSession.totalCoinsCollected);
    }

    public void TrackScore(int score)
    {
        CurrentSession.finalScore = score;
        LogEvent("Score updated", score);
    }

    // ==========================================
    // TRACKING METHODS - HEALTH & DEATHS
    // ==========================================
    public void TrackDeath(string deathType = "unknown")
    {
        CurrentSession.totalDeaths++;

        switch (deathType.ToLower())
        {
            case "fall":
            case "falling":
            case "jurang":
                CurrentSession.deathsByFalling++;
                break;
            case "enemy":
            case "damage":
            case "musuh":
                CurrentSession.deathsByEnemy++;
                break;
        }

        LogEvent("Death (" + deathType + ")", CurrentSession.totalDeaths);
    }

    public void TrackHealthChanged(int currentHealth)
    {
        CurrentSession.finalHealthRemaining = currentHealth;
        CurrentSession.livesUsed = 3 - currentHealth; // Assuming max 3 hearts
        LogEvent("Health changed", currentHealth);
    }

    public void TrackSceneReload()
    {
        CurrentSession.sceneReloads++;
        LogEvent("Scene reloaded (respawn)", CurrentSession.sceneReloads);
    }

    public void TrackFullRestart()
    {
        CurrentSession.fullRestarts++;
        LogEvent("Full restart", CurrentSession.fullRestarts);
    }

    // ==========================================
    // TRACKING METHODS - PROGRESS
    // ==========================================
    public void TrackCheckpoint(int checkpointNumber)
    {
        CurrentSession.checkpointsReached = Mathf.Max(CurrentSession.checkpointsReached, checkpointNumber);
        LogEvent("Checkpoint reached", checkpointNumber);
    }

    public void TrackKunangKunang(int totalCollected)
    {
        CurrentSession.kunangKunangCollected = totalCollected;
        LogEvent("Kunang-kunang collected", totalCollected);
    }

    public void TrackMissionComplete(int missionNumber)
    {
        CurrentSession.missionsCompleted++;
        LogEvent("Mission completed", missionNumber);
    }

    public void TrackFurthestDistance(float distance)
    {
        if (distance > CurrentSession.furthestDistance)
        {
            CurrentSession.furthestDistance = distance;
        }
    }

    public void TrackCompletionPercentage(float percentage)
    {
        CurrentSession.completionPercentage = Mathf.Clamp(percentage, 0f, 100f);
        LogEvent("Completion %", percentage);
    }

    // ==========================================
    // TRACKING METHODS - LEARNING
    // ==========================================
    public void TrackQuestionAnswered(bool isCorrect, float answerTime = 0f)
    {
        CurrentSession.questionsAnswered++;

        if (isCorrect)
            CurrentSession.correctAnswers++;
        else
            CurrentSession.wrongAnswers++;

        // Update average answer time
        if (answerTime > 0)
        {
            float totalTime = CurrentSession.averageAnswerTime * (CurrentSession.questionsAnswered - 1);
            CurrentSession.averageAnswerTime = (totalTime + answerTime) / CurrentSession.questionsAnswered;
        }

        LogEvent("Question answered (" + (isCorrect ? "correct" : "wrong") + ")", CurrentSession.questionsAnswered);
    }

    public void TrackHintUsed()
    {
        CurrentSession.hintsUsed++;
        LogEvent("Hint used", CurrentSession.hintsUsed);
    }

    // ==========================================
    // TRACKING METHODS - MOVEMENT
    // ==========================================
    public void TrackJump(bool isDoubleJump = false)
    {
        CurrentSession.totalJumps++;

        if (isDoubleJump)
            CurrentSession.doubleJumpsUsed++;

        LogEvent(isDoubleJump ? "Double jump" : "Jump", CurrentSession.totalJumps);
    }

    public void TrackEnemyDefeated()
    {
        CurrentSession.enemiesDefeated++;
        LogEvent("Enemy defeated", CurrentSession.enemiesDefeated);
    }

    private void TrackPlayerMovement()
    {
        // Cari Player jika belum ada referensi
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 currentPos = player.transform.position;

        // Track distance moved
        if (lastPosition != Vector3.zero)
        {
            float distance = Vector3.Distance(lastPosition, currentPos);
            CurrentSession.totalDistanceMoved += distance;
        }

        // Track furthest distance (X axis)
        TrackFurthestDistance(currentPos.x);
        lastPosition = currentPos;
    }

    // ==========================================
    // TRACKING METHODS - ENGAGEMENT
    // ==========================================
    public void TrackDialogRead()
    {
        CurrentSession.dialogsRead++;
        LogEvent("Dialog read", CurrentSession.dialogsRead);
    }

    public void TrackNPCInteraction()
    {
        CurrentSession.npcsInteracted++;
        LogEvent("NPC interaction", CurrentSession.npcsInteracted);
    }

    public void TrackTutorialComplete()
    {
        CurrentSession.tutorialCompleted = true;
        LogEvent("Tutorial completed");
    }

    public void TrackAchievement(string achievementName)
    {
        CurrentSession.achievementsUnlocked++;
        LogEvent("Achievement: " + achievementName, CurrentSession.achievementsUnlocked);
    }

    public void TrackPause(bool isPaused)
    {
        if (isPaused)
        {
            CurrentSession.pauseCount++;
            LogEvent("Game paused", CurrentSession.pauseCount);
        }
    }

    // ==========================================
    // EVENTS
    // ==========================================
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update level name jika berubah
        if (CurrentSession != null)
        {
            CurrentSession.levelName = scene.name;
            LogEvent("Scene loaded: " + scene.name);
        }
    }

    // ==========================================
    // UTILITY
    // ==========================================
    private void LogEvent(string eventName, float value = 0)
    {
        if (!enableDebugLogs) return;

        if (value != 0)
            Debug.Log("📊 Analytics: " + eventName + " = " + value);
        else
            Debug.Log("📊 Analytics: " + eventName);
    }

    /// <summary>
    /// Get current analytics data summary
    /// </summary>
    public void PrintCurrentStats()
    {
        if (CurrentSession == null)
        {
            Debug.LogWarning("No active session!");
            return;
        }

        Debug.Log("=================================");
        Debug.Log("📊 CURRENT SESSION ANALYTICS");
        Debug.Log("=================================");
        Debug.Log(CurrentSession.GetSummary());
        Debug.Log("Jumps: " + CurrentSession.totalJumps);
        Debug.Log("Distance: " + CurrentSession.totalDistanceMoved.ToString("F1") + "m");
        Debug.Log("Questions: " + CurrentSession.questionsAnswered + " (" + CurrentSession.correctAnswers + " correct)");
        Debug.Log("=================================");
    }

    /// <summary>
    /// Reset analytics (untuk testing)
    /// </summary>
    [ContextMenu("Reset Analytics")]
    public void ResetAnalytics()
    {
        StartNewSession();
        Debug.Log("🔄 Analytics reset!");
    }
}