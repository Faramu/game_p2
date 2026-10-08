using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manager untuk komunikasi dengan Moodle Dashboard Analitik
/// Menggunakan Singleton pattern agar bisa diakses dari script lain
/// </summary>
public class LMSManager : MonoBehaviour
{
    // ============================================
    // SINGLETON INSTANCE
    // ============================================
    /// <summary>
    /// Instance singleton untuk akses global
    /// </summary>
    public static LMSManager Instance { get; private set; }

    // ============================================
    // SETTING - GANTI SESUAI INSTALASI MOODLE ANDA
    // ============================================
    [Header("Moodle Settings")]
    [Tooltip("URL Moodle Web Service")]
    private string moodleURL = "http://localhost/learn/server/moodle/webservice/rest/server.php";

    [Tooltip("Token dari Moodle (Site administration > Web services > Manage tokens)")]
    private string token = "29e616f0de26ca4d3486dacb7bcd9f94";

    // ============================================
    // DYNAMIC USER & COURSE DATA
    // ============================================
    [Header("User & Course Data (Diambil dari URL)")]
    [Tooltip("User ID yang diambil dari URL query parameter. Fallback jika tidak ada di URL.")]
    public int userID = 6; // Default value untuk testing di editor

    [Tooltip("Course ID yang diambil dari URL query parameter. Fallback jika tidak ada di URL.")]
    public int courseID = 2; // Default value untuk testing di editor

    [Header("Debug Info")]
    [Tooltip("Apakah data diambil dari URL atau menggunakan fallback?")]
    public bool dataFromURL = false;

    [Tooltip("URL yang diparse (hanya terisi di WebGL build)")]
    public string parsedURL = "";

    // ============================================
    // UNITY LIFECYCLE
    // ============================================
    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist antar scene
        }
        else
        {
            Destroy(gameObject); // Hancurkan duplikat
            return;
        }
    }

    private void Start()
    {
        // Parse URL query parameters saat game dimulai
        ParseURLParameters();

        // Log hasil parsing
        LogUserCourseInfo();
    }

    // ============================================
    // URL PARSING METHODS
    // ============================================
    /// <summary>
    /// Parse URL query parameters untuk mendapatkan userid dan courseid
    /// Contoh URL: https://mygame.com/index.html?userid=123&courseid=45
    /// </summary>
    private void ParseURLParameters()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Di WebGL build, gunakan Application.absoluteURL
        string url = Application.absoluteURL;
        parsedURL = url;
        Debug.Log("🌐 Running in WebGL. Full URL: " + url);

        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("⚠️ Application.absoluteURL is empty. Using fallback values.");
            dataFromURL = false;
            return;
        }

        // Parse query string
        if (url.Contains("?"))
        {
            string queryString = url.Substring(url.IndexOf('?') + 1);
            Debug.Log("📋 Query String: " + queryString);

            string[] parameters = queryString.Split('&');
            bool foundUserId = false;
            bool foundCourseId = false;

            foreach (string param in parameters)
            {
                string[] keyValue = param.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].ToLower().Trim();
                    string value = keyValue[1].Trim();

                    if (key == "userid")
                    {
                        if (int.TryParse(value, out int parsedUserId))
                        {
                            userID = parsedUserId;
                            foundUserId = true;
                            Debug.Log("✅ User ID from URL: " + userID);
                        }
                        else
                        {
                            Debug.LogWarning("⚠️ Invalid userid value: " + value);
                        }
                    }
                    else if (key == "courseid")
                    {
                        if (int.TryParse(value, out int parsedCourseId))
                        {
                            courseID = parsedCourseId;
                            foundCourseId = true;
                            Debug.Log("✅ Course ID from URL: " + courseID);
                        }
                        else
                        {
                            Debug.LogWarning("⚠️ Invalid courseid value: " + value);
                        }
                    }
                }
            }

            dataFromURL = foundUserId && foundCourseId;

            if (!dataFromURL)
            {
                Debug.LogWarning("⚠️ Tidak semua parameter ditemukan di URL. Menggunakan nilai fallback.");
                if (!foundUserId) Debug.LogWarning("   - userid tidak ditemukan, menggunakan fallback: " + userID);
                if (!foundCourseId) Debug.LogWarning("   - courseid tidak ditemukan, menggunakan fallback: " + courseID);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No query parameters in URL. Using fallback values.");
            dataFromURL = false;
        }
#else
        // Di Unity Editor atau platform lain, gunakan nilai fallback
        Debug.Log("🎮 Running in Unity Editor or non-WebGL platform. Using fallback values.");
        parsedURL = "Not available (Editor mode)";
        dataFromURL = false;
#endif
    }

    /// <summary>
    /// Log informasi user dan course yang akan digunakan
    /// </summary>
    private void LogUserCourseInfo()
    {
        Debug.Log("====================================");
        Debug.Log("📊 LMS MANAGER - USER & COURSE INFO");
        Debug.Log("====================================");
        Debug.Log("User ID: " + userID);
        Debug.Log("Course ID: " + courseID);
        Debug.Log("Data Source: " + (dataFromURL ? "URL Query Parameters" : "Fallback Values (Default)"));
        Debug.Log("====================================");
    }

    // ============================================
    // PUBLIC METHODS
    // ============================================
    /// <summary>
    /// Kirim SEMUA data analytics ke Moodle Dashboard Analitik
    /// CARA BARU: Menggunakan GameAnalytics untuk data lengkap
    /// </summary>
    /// <param name="analytics">Object GameAnalytics berisi semua data gameplay</param>
    public void KirimAnalyticsKeMoodle(GameAnalytics analytics)
    {
        if (analytics == null)
        {
            Debug.LogError("❌ GameAnalytics is null!");
            return;
        }

        StartCoroutine(PostAnalyticsCoroutine(analytics, userID, courseID));
    }

    /// <summary>
    /// Kirim data analytics dengan manual override userId dan courseId
    /// </summary>
    public void KirimAnalyticsKeMoodle(GameAnalytics analytics, int userId, int courseId)
    {
        if (analytics == null)
        {
            Debug.LogError("❌ GameAnalytics is null!");
            return;
        }

        StartCoroutine(PostAnalyticsCoroutine(analytics, userId, courseId));
    }

    /// <summary>
    /// Kirim data koin sederhana ke Moodle (LEGACY - untuk backward compatibility)
    /// OVERLOAD 1: Menggunakan userID dan courseID yang sudah diparsing dari URL
    /// </summary>
    /// <param name="totalKoin">Jumlah koin yang dikumpulkan player</param>
    public void KirimKoinKeMoodle(int totalKoin)
    {
        StartCoroutine(PostScoreCoroutine(totalKoin, userID, courseID));
    }

    /// <summary>
    /// Kirim data koin sederhana ke Moodle (LEGACY - untuk backward compatibility)
    /// OVERLOAD 2: Manual override userId dan courseId
    /// </summary>
    /// <param name="totalKoin">Jumlah koin yang dikumpulkan player</param>
    /// <param name="userId">Moodle User ID (manual override)</param>
    /// <param name="courseId">Moodle Course ID (manual override)</param>
    public void KirimKoinKeMoodle(int totalKoin, int userId, int courseId)
    {
        StartCoroutine(PostScoreCoroutine(totalKoin, userId, courseId));
    }

    /// <summary>
    /// Kirim data koin dengan score terpisah
    /// OVERLOAD 1: Menggunakan userID dan courseID yang sudah diparsing dari URL
    /// </summary>
    /// <param name="totalKoin">Jumlah koin</param>
    /// <param name="score">Score game (bisa berbeda dari koin)</param>
    public void KirimDataKeMoodle(int totalKoin, int score)
    {
        StartCoroutine(PostScoreCoroutine(totalKoin, userID, courseID, score));
    }

    /// <summary>
    /// Kirim data koin dengan score terpisah
    /// OVERLOAD 2: Manual override userId dan courseId (untuk backward compatibility)
    /// </summary>
    /// <param name="totalKoin">Jumlah koin</param>
    /// <param name="score">Score game (bisa berbeda dari koin)</param>
    /// <param name="userId">Moodle User ID (manual override)</param>
    /// <param name="courseId">Moodle Course ID (manual override)</param>
    public void KirimDataKeMoodle(int totalKoin, int score, int userId, int courseId)
    {
        StartCoroutine(PostScoreCoroutine(totalKoin, userId, courseId, score));
    }

    // ============================================
    // PRIVATE METHODS - ANALYTICS (NEW)
    // ============================================
    /// <summary>
    /// POST analytics lengkap ke Moodle
    /// </summary>
    private IEnumerator PostAnalyticsCoroutine(GameAnalytics analytics, int userId, int courseId)
    {
        WWWForm form = new WWWForm();

        // Parameter Wajib Moodle Web Service
        form.AddField("wstoken", token);
        form.AddField("wsfunction", "local_analyticsboard_submit_game_data");
        form.AddField("moodlewsrestformat", "json");

        // Parameter Dasar
        form.AddField("userid", userId);
        form.AddField("courseid", courseId);

        // ===== GAMEPLAY DATA =====
        form.AddField("coins", analytics.totalCoinsCollected);
        form.AddField("score", analytics.finalScore);
        form.AddField("levelname", analytics.levelName);
        form.AddField("sessionid", analytics.sessionId);
        form.AddField("completed", analytics.levelCompleted ? 1 : 0);

        // ===== TIME & DURATION =====
        form.AddField("playduration", Mathf.RoundToInt(analytics.playDuration)); // seconds
        form.AddField("sessionstart", analytics.sessionStartTime.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("sessionend", analytics.sessionEndTime.ToString("yyyy-MM-dd HH:mm:ss"));

        // ===== HEALTH & DEATHS =====
        form.AddField("totaldeaths", analytics.totalDeaths);
        form.AddField("deathsfall", analytics.deathsByFalling);
        form.AddField("deathsenemy", analytics.deathsByEnemy);
        form.AddField("finalhp", analytics.finalHealthRemaining);
        form.AddField("livesused", analytics.livesUsed);

        // ===== PROGRESS =====
        form.AddField("checkpoints", analytics.checkpointsReached);
        form.AddField("distance", Mathf.RoundToInt(analytics.furthestDistance));
        form.AddField("kunangkunang", analytics.kunangKunangCollected);
        form.AddField("missions", analytics.missionsCompleted);
        form.AddField("completion", Mathf.RoundToInt(analytics.completionPercentage));

        // ===== LEARNING =====
        form.AddField("questions", analytics.questionsAnswered);
        form.AddField("correct", analytics.correctAnswers);
        form.AddField("wrong", analytics.wrongAnswers);
        form.AddField("avganswertime", Mathf.RoundToInt(analytics.averageAnswerTime));
        form.AddField("hints", analytics.hintsUsed);

        // ===== MOVEMENT =====
        form.AddField("jumps", analytics.totalJumps);
        form.AddField("doublejumps", analytics.doubleJumpsUsed);
        form.AddField("totaldistance", Mathf.RoundToInt(analytics.totalDistanceMoved));
        form.AddField("enemies", analytics.enemiesDefeated);

        // ===== RETRY & PERSISTENCE =====
        form.AddField("respawns", analytics.sceneReloads);
        form.AddField("restarts", analytics.fullRestarts);
        form.AddField("pauses", analytics.pauseCount);
        form.AddField("pausetime", Mathf.RoundToInt(analytics.totalPausedTime));

        // ===== ENGAGEMENT =====
        form.AddField("dialogs", analytics.dialogsRead);
        form.AddField("npcs", analytics.npcsInteracted);
        form.AddField("tutorial", analytics.tutorialCompleted ? 1 : 0);
        form.AddField("achievements", analytics.achievementsUnlocked);

        // ===== JSON FULL DATA (Optional - untuk backup) =====
        form.AddField("analyticsjson", analytics.ToJson());

        Debug.Log("🚀 ===== MENGIRIM ANALYTICS KE MOODLE =====");
        Debug.Log("User: " + userId + ", Course: " + courseId);
        Debug.Log("Level: " + analytics.levelName);
        Debug.Log("Duration: " + analytics.playDuration.ToString("F1") + "s");
        Debug.Log("Coins: " + analytics.totalCoinsCollected + ", Deaths: " + analytics.totalDeaths);
        Debug.Log("Questions: " + analytics.questionsAnswered + " (" + analytics.correctAnswers + " correct)");
        Debug.Log("==========================================");

        // Kirim Request POST ke Moodle
        using (UnityWebRequest www = UnityWebRequest.Post(moodleURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ GAGAL mengirim analytics ke Moodle!");
                Debug.LogError("Error: " + www.error);
                Debug.LogError("Response Code: " + www.responseCode);
                Debug.LogError("Response: " + www.downloadHandler.text);
                ShowTroubleshootingHints(www.responseCode);
            }
            else
            {
                Debug.Log("✅ BERHASIL mengirim analytics ke Dashboard Analitik Moodle!");
                Debug.Log("Response: " + www.downloadHandler.text);
                ParseMoodleResponse(www.downloadHandler.text);
            }
        }
    }

    // ============================================
    // PRIVATE METHODS - LEGACY (SIMPLE COINS)
    // ============================================
    private IEnumerator PostScoreCoroutine(int totalKoin, int userId, int courseId, int score = -1)
    {
        // Jika score tidak diset, gunakan totalKoin sebagai score
        if (score == -1)
        {
            score = totalKoin;
        }

        WWWForm form = new WWWForm();

        // Parameter Wajib Moodle Web Service
        form.AddField("wstoken", token);
        form.AddField("wsfunction", "local_analyticsboard_submit_game_data");
        form.AddField("moodlewsrestformat", "json");

        // Parameter Custom untuk Dashboard Analitik
        form.AddField("userid", userId);
        form.AddField("courseid", courseId);
        form.AddField("coins", totalKoin);
        form.AddField("score", score);

        Debug.Log("🚀 Mengirim data ke Moodle: User=" + userId + ", Course=" + courseId + ", Coins=" + totalKoin + ", Score=" + score);

        // Kirim Request POST ke Moodle
        using (UnityWebRequest www = UnityWebRequest.Post(moodleURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                // Error terjadi
                Debug.LogError("❌ GAGAL mengirim data ke Moodle!");
                Debug.LogError("Error: " + www.error);
                Debug.LogError("Response Code: " + www.responseCode);
                Debug.LogError("Response: " + www.downloadHandler.text);

                // Tampilkan troubleshooting hints
                ShowTroubleshootingHints(www.responseCode);
            }
            else
            {
                // Sukses!
                Debug.Log("✅ BERHASIL mengirim data ke Dashboard Analitik Moodle!");
                Debug.Log("Response: " + www.downloadHandler.text);

                // Parse response untuk info tambahan
                ParseMoodleResponse(www.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Parse JSON response dari Moodle
    /// </summary>
    private void ParseMoodleResponse(string jsonResponse)
    {
        try
        {
            // Check if success
            if (jsonResponse.Contains("\"success\":true"))
            {
                Debug.Log("✅ Data tersimpan di database Moodle!");

                // Extract record ID
                if (jsonResponse.Contains("\"recordid\":"))
                {
                    int recordIdStart = jsonResponse.IndexOf("\"recordid\":") + 11;
                    int recordIdEnd = jsonResponse.IndexOf(",", recordIdStart);
                    if (recordIdEnd == -1) recordIdEnd = jsonResponse.IndexOf("}", recordIdStart);
                    string recordId = jsonResponse.Substring(recordIdStart, recordIdEnd - recordIdStart);
                    Debug.Log("📝 Database Record ID: " + recordId);
                }

                // Extract total coins
                if (jsonResponse.Contains("\"totalcoins\":"))
                {
                    int coinsStart = jsonResponse.IndexOf("\"totalcoins\":") + 13;
                    int coinsEnd = jsonResponse.IndexOf("}", coinsStart);
                    string totalCoins = jsonResponse.Substring(coinsStart, coinsEnd - coinsStart);
                    Debug.Log("💰 Total Koin Akumulasi di Moodle: " + totalCoins);
                }

                Debug.Log("👉 Lihat hasilnya di: Dashboard Analitik (top bar menu Moodle)");
            }
            else if (jsonResponse.Contains("\"exception\""))
            {
                Debug.LogError("⚠️ Moodle mengembalikan exception!");
                Debug.LogError("Response: " + jsonResponse);
            }
            else
            {
                Debug.LogWarning("⚠️ Response tidak mengandung status success");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("⚠️ Tidak bisa parse response JSON: " + e.Message);
            Debug.Log("Raw response: " + jsonResponse);
        }
    }

    /// <summary>
    /// Tampilkan hint troubleshooting berdasarkan error code
    /// </summary>
    private void ShowTroubleshootingHints(long responseCode)
    {
        Debug.Log("====================================");
        Debug.Log("💡 TROUBLESHOOTING HINTS:");
        Debug.Log("====================================");

        switch (responseCode)
        {
            case 0:
                Debug.Log("• Tidak bisa connect ke server Moodle");
                Debug.Log("• Cek URL Moodle sudah benar?");
                Debug.Log("• Cek XAMPP Apache sudah running?");
                Debug.Log("• Cek firewall tidak block?");
                break;
            case 401:
            case 403:
                Debug.Log("• Token tidak valid atau expired");
                Debug.Log("• Generate token baru di:");
                Debug.Log("  Site administration > Server > Web services > Manage tokens");
                Debug.Log("• Pastikan Web Services sudah enabled");
                Debug.Log("• Pastikan REST protocol sudah enabled");
                break;
            case 404:
                Debug.Log("• URL atau function tidak ditemukan");
                Debug.Log("• Pastikan plugin sudah terinstall");
                Debug.Log("• Cek URL: " + moodleURL);
                break;
            case 500:
                Debug.Log("• Internal server error di Moodle");
                Debug.Log("• Cek Moodle error logs");
                Debug.Log("• Cek PHP error logs di XAMPP");
                break;
            default:
                Debug.Log("• HTTP Response Code: " + responseCode);
                Debug.Log("• Cek Moodle debugging: Site administration > Development > Debugging");
                break;
        }

        Debug.Log("====================================");
    }

    // ============================================
    // TESTING / DEBUG
    // ============================================
    /// <summary>
    /// Test koneksi ke Moodle (panggil dari Inspector atau code untuk testing)
    /// Menggunakan userID dan courseID yang sudah diparsing
    /// </summary>
    [ContextMenu("Test Connection to Moodle")]
    public void TestConnection()
    {
        Debug.Log("🧪 Testing koneksi ke Moodle...");
        Debug.Log("URL: " + moodleURL);
        Debug.Log("Token: " + token.Substring(0, Mathf.Min(10, token.Length)) + "...");
        Debug.Log("User ID: " + userID);
        Debug.Log("Course ID: " + courseID);

        // Send test data menggunakan userID dan courseID yang sudah diparsing
        KirimKoinKeMoodle(999);
    }

    /// <summary>
    /// Re-parse URL parameters (berguna jika URL berubah dinamis)
    /// </summary>
    [ContextMenu("Re-parse URL Parameters")]
    public void ReParseURL()
    {
        ParseURLParameters();
        LogUserCourseInfo();
    }
}