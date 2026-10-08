using System;
using UnityEngine;

/// <summary>
/// Data class untuk menyimpan semua analytics gameplay
/// Bisa di-serialize ke JSON untuk dikirim ke Moodle
/// </summary>
[System.Serializable]
public class GameAnalytics
{
    // ==========================================
    // IDENTITAS SESI
    // ==========================================
    [Header("Session Info")]
    public string sessionId;           // Unique ID untuk sesi ini
    public string levelName;           // Nama scene/level yang dimainkan
    public DateTime sessionStartTime;  // Waktu mulai bermain
    public DateTime sessionEndTime;    // Waktu selesai

    // ==========================================
    // PERFORMA GAMEPLAY
    // ==========================================
    [Header("Gameplay Performance")]
    public int totalCoinsCollected;    // Total koin yang dikumpulkan
    public int finalScore;             // Score akhir (bisa berbeda dari koin)
    public float playDuration;         // Lama bermain dalam detik
    public bool levelCompleted;        // Apakah level selesai dengan sukses?

    // ==========================================
    // SISTEM NYAWA & KEMATIAN
    // ==========================================
    [Header("Health & Deaths")]
    public int totalDeaths;            // Total berapa kali mati
    public int deathsByFalling;        // Mati karena jatuh ke jurang
    public int deathsByEnemy;          // Mati karena kena musuh/damage
    public int finalHealthRemaining;   // Sisa nyawa di akhir
    public int livesUsed;              // Berapa nyawa yang terpakai

    // ==========================================
    // PROGRESS & EXPLORATION
    // ==========================================
    [Header("Progress Tracking")]
    public int checkpointsReached;     // Berapa checkpoint yang dicapai
    public float furthestDistance;     // Jarak terjauh yang dicapai (posisi X)
    public int kunangKunangCollected;  // Kunang-kunang yang dikumpulkan
    public int missionsCompleted;      // Misi yang diselesaikan
    public float completionPercentage; // % penyelesaian level (0-100)

    // ==========================================
    // INTERAKSI & PEMBELAJARAN
    // ==========================================
    [Header("Learning Metrics")]
    public int questionsAnswered;      // Total soal yang dijawab
    public int correctAnswers;         // Jawaban benar
    public int wrongAnswers;           // Jawaban salah
    public float averageAnswerTime;    // Rata-rata waktu menjawab (detik)
    public int hintsUsed;              // Berapa kali minta petunjuk

    // ==========================================
    // PERGERAKAN & MEKANIK
    // ==========================================
    [Header("Movement & Mechanics")]
    public int totalJumps;             // Total lompatan
    public int doubleJumpsUsed;        // Penggunaan double jump
    public float totalDistanceMoved;   // Total jarak yang ditempuh
    public int enemiesDefeated;        // Musuh yang dikalahkan

    // ==========================================
    // RETRY & PERSISTENCE
    // ==========================================
    [Header("Retry Metrics")]
    public int sceneReloads;           // Berapa kali scene di-reload (respawn)
    public int fullRestarts;           // Berapa kali restart dari awal
    public int pauseCount;             // Berapa kali pause game
    public float totalPausedTime;      // Total waktu dalam status pause

    // ==========================================
    // ENGAGEMENT & BEHAVIOR
    // ==========================================
    [Header("Engagement")]
    public int dialogsRead;            // Dialog yang dibaca
    public int npcsInteracted;         // NPC yang diajak bicara
    public bool tutorialCompleted;     // Apakah tutorial selesai?
    public int achievementsUnlocked;   // Achievement yang didapat

    // ==========================================
    // CONSTRUCTOR & METHODS
    // ==========================================
    public GameAnalytics()
    {
        // Generate unique session ID
        sessionId = System.Guid.NewGuid().ToString();
        sessionStartTime = DateTime.Now;
        levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Initialize default values
        levelCompleted = false;
        completionPercentage = 0f;
    }

    /// <summary>
    /// Panggil saat sesi berakhir untuk finalisasi data
    /// </summary>
    public void FinalizeSession()
    {
        sessionEndTime = DateTime.Now;
        playDuration = (float)(sessionEndTime - sessionStartTime).TotalSeconds;

        // Hitung accuracy jika ada soal
        if (questionsAnswered > 0)
        {
            float accuracy = ((float)correctAnswers / questionsAnswered) * 100f;
            Debug.Log("📊 Quiz Accuracy: " + accuracy.ToString("F1") + "%");
        }
    }

    /// <summary>
    /// Convert ke JSON string untuk dikirim ke server
    /// </summary>
    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    /// <summary>
    /// Get formatted summary untuk logging
    /// </summary>
    public string GetSummary()
    {
        return "Session: " + sessionId + "\n" +
            "Level: " + levelName + "\n" +
            "Duration: " + playDuration.ToString("F1") + "s\n" +
            "Coins: " + totalCoinsCollected + "\n" +
            "Deaths: " + totalDeaths + "\n" +
            "Completed: " + levelCompleted + "\n" +
            "Score: " + finalScore;
    }
}