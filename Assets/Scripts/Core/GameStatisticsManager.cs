using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sumber tunggal (Single Source of Truth) untuk statistik dan koin game "Petualangan Bilangan Bulan".
/// Mengelola: TotalKoin, WaktuBermain, StageSelesai, StatSoalBenar, StatSoalSalah, StatBosKalah.
/// </summary>
public static class GameStatisticsManager
{
    // Canonical PlayerPrefs Keys
    public const string KEY_TOTAL_KOIN = "TotalKoin";
    public const string KEY_WAKTU_BERMAIN = "WaktuBermain";
    public const string KEY_STAGE_SELESAI = "StageSelesai";
    public const string KEY_STAT_SOAL_BENAR = "StatSoalBenar";
    public const string KEY_STAT_SOAL_SALAH = "StatSoalSalah";
    public const string KEY_STAT_BOS_KALAH = "StatBosKalah";

    // Reward key prefix
    public const string KEY_REWARD_PREFIX = "RewardClaimed_";

    // Auto-bootstrap Persistent Time Tracker
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InisialisasiOtomatis()
    {
        GameObject runner = new GameObject("[GameStatisticsManager_Tracker]");
        Object.DontDestroyOnLoad(runner);
        runner.AddComponent<StatistikTimeTracker>();
    }

    // =========================================================
    // KOIN & REWARD
    // =========================================================

    public static int GetTotalKoin()
    {
        return PlayerPrefs.GetInt(KEY_TOTAL_KOIN, 0);
    }

    public static void TambahKoin(int jumlahKoin, int stage = 0, int level = 0, int subLevel = 0)
    {
        if (jumlahKoin <= 0) return;

        int sebelum = GetTotalKoin();
        int sesudah = sebelum + jumlahKoin;

        PlayerPrefs.SetInt(KEY_TOTAL_KOIN, sesudah);
        PlayerPrefs.Save();

        Debug.Log($"[COIN]\nBefore = {sebelum}\nAdded = {jumlahKoin}\nAfter = {sesudah}");
    }

    public static bool BerikanRewardLevel(int stage, int level, int subLevel, int rewardCoin)
    {
        string rewardKey = $"{KEY_REWARD_PREFIX}S{stage}_L{level}_Sub{subLevel}";
        bool sudahDiklaim = PlayerPrefs.GetInt(rewardKey, 0) == 1;

        Debug.Log($"[REWARD]\nStage = {stage}\nLevel = {level}\nSubLevel = {subLevel}\nReward Coin = {rewardCoin}\nReward already claimed = {sudahDiklaim.ToString().ToLower()}");

        if (!sudahDiklaim)
        {
            PlayerPrefs.SetInt(rewardKey, 1);
            PlayerPrefs.Save();
            TambahKoin(rewardCoin, stage, level, subLevel);
            return true;
        }

        return false;
    }

    public static bool ApakahRewardSudahDiklaim(int stage, int level, int subLevel)
    {
        string rewardKey = $"{KEY_REWARD_PREFIX}S{stage}_L{level}_Sub{subLevel}";
        return PlayerPrefs.GetInt(rewardKey, 0) == 1;
    }

    // =========================================================
    // STATISTIK SOAL & JAWABAN
    // =========================================================

    public static int GetStatSoalBenar()
    {
        return PlayerPrefs.GetInt(KEY_STAT_SOAL_BENAR, 0);
    }

    public static int GetStatSoalSalah()
    {
        return PlayerPrefs.GetInt(KEY_STAT_SOAL_SALAH, 0);
    }

    public static void CatatJawabanBenar(string questionId = "")
    {
        int benar = GetStatSoalBenar() + 1;
        PlayerPrefs.SetInt(KEY_STAT_SOAL_BENAR, benar);
        PlayerPrefs.Save();

        int salah = GetStatSoalSalah();
        string qIdLog = string.IsNullOrEmpty(questionId) ? "-" : questionId;
        Debug.Log($"[STAT]\nQuestion ID = {qIdLog}\nResult = CORRECT\nCorrect = {benar}\nWrong = {salah}");
    }

    public static void CatatJawabanSalah(string questionId = "")
    {
        int salah = GetStatSoalSalah() + 1;
        PlayerPrefs.SetInt(KEY_STAT_SOAL_SALAH, salah);
        PlayerPrefs.Save();

        int benar = GetStatSoalBenar();
        string qIdLog = string.IsNullOrEmpty(questionId) ? "-" : questionId;
        Debug.Log($"[STAT]\nQuestion ID = {qIdLog}\nResult = WRONG\nCorrect = {benar}\nWrong = {salah}");
    }

    // =========================================================
    // STATISTIK BOS
    // =========================================================

    public static int GetStatBosKalah()
    {
        return PlayerPrefs.GetInt(KEY_STAT_BOS_KALAH, 0);
    }

    public static void CatatBossKalah(string namaBos = "Bos")
    {
        int total = GetStatBosKalah() + 1;
        PlayerPrefs.SetInt(KEY_STAT_BOS_KALAH, total);
        PlayerPrefs.Save();

        Debug.Log($"[STAT] Bos Berhasil Dikalahkan: {namaBos}! Total Bos Kalah = {total}");
    }

    // =========================================================
    // STAGE PROGRES
    // =========================================================

    public static int GetStageSelesai()
    {
        return PlayerPrefs.GetInt(KEY_STAGE_SELESAI, 0);
    }

    public static void CatatStageSelesai(int nomorStage)
    {
        int stageSebelum = GetStageSelesai();
        if (nomorStage > stageSebelum)
        {
            PlayerPrefs.SetInt(KEY_STAGE_SELESAI, nomorStage);
            PlayerPrefs.Save();
            Debug.Log($"[PROGRESS] Rekor Stage Selesai bertambah: {stageSebelum} -> {nomorStage}");
        }
    }

    // =========================================================
    // WAKTU BERMAIN
    // =========================================================

    public static float GetWaktuBermain()
    {
        return PlayerPrefs.GetFloat(KEY_WAKTU_BERMAIN, 0f);
    }

    public static void TambahWaktuBermain(float detikTambahan)
    {
        if (detikTambahan <= 0f) return;
        float waktu = GetWaktuBermain() + detikTambahan;
        PlayerPrefs.SetFloat(KEY_WAKTU_BERMAIN, waktu);
        PlayerPrefs.Save();
    }

    public static float HitungAkurasi()
    {
        int benar = GetStatSoalBenar();
        int salah = GetStatSoalSalah();
        int total = benar + salah;
        if (total <= 0) return 0f;
        return ((float)benar / total) * 100f;
    }

    // Helper Mono untuk menjalankan akumulasi waktu secara background di gameplay scenes
    private class StatistikTimeTracker : MonoBehaviour
    {
        private float timerSimpan = 0f;
        private float bufferWaktu = 0f;

        void Update()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            // Jangan hitung waktu ketika player sedang berada di menu santai
            if (sceneName == "MainMenuScene" || sceneName == "Menu_Pencapaian" || sceneName == "Menu_Petunjuk")
            {
                return;
            }

            // Gunakan unscaledDeltaTime agar tetap terhitung saat slowmo/pause gameplay
            float dt = Time.unscaledDeltaTime;
            bufferWaktu += dt;
            timerSimpan += dt;

            // Simpan setiap 1 detik
            if (timerSimpan >= 1f)
            {
                float waktuSekarang = PlayerPrefs.GetFloat(KEY_WAKTU_BERMAIN, 0f) + bufferWaktu;
                PlayerPrefs.SetFloat(KEY_WAKTU_BERMAIN, waktuSekarang);
                PlayerPrefs.Save();

                bufferWaktu = 0f;
                timerSimpan = 0f;
            }
        }

        void OnApplicationQuit()
        {
            if (bufferWaktu > 0f)
            {
                float waktuSekarang = PlayerPrefs.GetFloat(KEY_WAKTU_BERMAIN, 0f) + bufferWaktu;
                PlayerPrefs.SetFloat(KEY_WAKTU_BERMAIN, waktuSekarang);
                PlayerPrefs.Save();
                bufferWaktu = 0f;
            }
        }
    }
}
