using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager _instance;
    public static CheckpointManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindFirstObjectByType<CheckpointManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CheckpointManager");
                    _instance = go.AddComponent<CheckpointManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
        private set => _instance = value;
    }

    public const int TOTAL_STAGE = 6;
    public const int TOTAL_LEVEL = 3;
    public const int TOTAL_SUBLEVEL = 3;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // =====================================================
    // HELPER STRUKTUR SUB LEVEL (FUNGSI UTAMA SECTION F)
    // =====================================================

    /// <summary>
    /// Menentukan apakah kombinasi Stage + Level memiliki Sub Level (3 sublevel).
    /// Stage 1: Level 1, 2, 3 -> TRUE
    /// Stage 2-6: Level 1, 2 -> TRUE; Level 3 -> FALSE (1 gameplay langsung)
    /// </summary>
    public static bool ApakahMemilikiSubLevel(int stage, int level)
    {
        if (stage == 1) return true;
        if (stage >= 2 && stage <= 6)
        {
            if (level == 1 || level == 2) return true;
            if (level == 3) return false;
        }
        return false;
    }

    public static int GetJumlahSubLevel(int stage, int level)
    {
        return ApakahMemilikiSubLevel(stage, level) ? 3 : 0;
    }

    // =====================================================
    // CEK SUB LEVEL
    // =====================================================

    public static bool IsSubLevelSelesaiStatic(int stage, int level, int subLevel)
    {
        string key1 = $"Stage_{stage}_Level_{level}_Sub_{subLevel}";
        string key2 = $"Stage_{stage}_Level_{level}_SubLevel_{subLevel}";
        return PlayerPrefs.GetInt(key1, 0) == 1 || PlayerPrefs.GetInt(key2, 0) == 1;
    }

    public bool IsSubLevelSelesai(int stage, int level, int subLevel)
    {
        return IsSubLevelSelesaiStatic(stage, level, subLevel);
    }

    // =====================================================
    // MENYELESAIKAN SUB LEVEL
    // =====================================================

    public void SelesaikanSubLevel(int stage, int level, int subLevel)
    {
        if (stage < 1 || stage > TOTAL_STAGE) return;
        if (level < 1 || level > TOTAL_LEVEL) return;
        if (subLevel < 1 || subLevel > TOTAL_SUBLEVEL) return;

        string key1 = GetSubLevelKey(stage, level, subLevel);
        string key2 = $"Stage_{stage}_Level_{level}_SubLevel_{subLevel}";

        PlayerPrefs.SetInt(key1, 1);
        PlayerPrefs.SetInt(key2, 1);
        PlayerPrefs.Save();

        Debug.Log($"[PROGRESS] Stage {stage} Level {level} SubLevel {subLevel} = SELESAI");

        // Cek apakah 1 level sudah selesai penuh (seluruh 3 sublevel selesai)
        if (IsLevelSelesai(stage, level))
        {
            PlayerPrefs.SetInt($"Stage_{stage}_Level_{level}_Selesai", 1);
            PlayerPrefs.Save();
            Debug.Log($"[PROGRESS] LEVEL {level} STAGE {stage} SELESAI PENUH! ⭐");
        }

        // Cek apakah seluruh stage selesai
        if (IsStageSelesai(stage))
        {
            PlayerPrefs.SetInt($"Stage_{stage}_Selesai", 1);
            PlayerPrefs.Save();
            GameStatisticsManager.CatatStageSelesai(stage);
            Debug.Log($"[PROGRESS] STAGE {stage} SELESAI! 🎉");
        }
    }

    // =====================================================
    // MENYELESAIKAN LEVEL TANPA SUB LEVEL (STAGE 2-6 LEVEL 3)
    // =====================================================

    public void SelesaikanLevelTanpaSubLevel(int stage, int level)
    {
        if (stage < 1 || stage > TOTAL_STAGE || level < 1 || level > TOTAL_LEVEL) return;

        string keyDirect = $"Stage_{stage}_Level_{level}_Selesai";
        string keySub1 = GetSubLevelKey(stage, level, 1);
        string keySubLevel1 = $"Stage_{stage}_Level_{level}_SubLevel_1";

        PlayerPrefs.SetInt(keyDirect, 1);
        PlayerPrefs.SetInt(keySub1, 1);
        PlayerPrefs.SetInt(keySubLevel1, 1);
        PlayerPrefs.Save();

        Debug.Log($"[PROGRESS] Stage {stage} Level {level} (Tanpa SubLevel) = SELESAI");
        Debug.Log($"[PROGRESS] LEVEL {level} STAGE {stage} SELESAI PENUH! ⭐");

        if (IsStageSelesai(stage))
        {
            PlayerPrefs.SetInt($"Stage_{stage}_Selesai", 1);
            PlayerPrefs.Save();
            GameStatisticsManager.CatatStageSelesai(stage);
            Debug.Log($"[PROGRESS] STAGE {stage} SELESAI! 🎉");
        }
    }

    // =====================================================
    // CEK LEVEL
    // =====================================================

    public static bool IsLevelSelesaiStatic(int stage, int level)
    {
        if (stage < 1 || stage > TOTAL_STAGE || level < 1 || level > TOTAL_LEVEL)
            return false;

        if (ApakahMemilikiSubLevel(stage, level))
        {
            // Level memiliki 3 sublevel: SEMUA 3 sublevel HARUS selesai
            for (int sub = 1; sub <= TOTAL_SUBLEVEL; sub++)
            {
                if (!IsSubLevelSelesaiStatic(stage, level, sub))
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            // Level TIDAK memiliki sublevel (hanya 1 gameplay langsung)
            string keyDirect = $"Stage_{stage}_Level_{level}_Selesai";
            string keySub1 = $"Stage_{stage}_Level_{level}_Sub_1";
            string keySubLevel1 = $"Stage_{stage}_Level_{level}_SubLevel_1";
            string keyLegacy = $"Stage_{stage}_Level_{level}_Sub_3";
            return PlayerPrefs.GetInt(keyDirect, 0) == 1 ||
                   PlayerPrefs.GetInt(keySub1, 0) == 1 ||
                   PlayerPrefs.GetInt(keySubLevel1, 0) == 1 ||
                   PlayerPrefs.GetInt(keyLegacy, 0) == 1;
        }
    }

    public bool IsLevelSelesai(int stage, int level)
    {
        return IsLevelSelesaiStatic(stage, level);
    }

    // =====================================================
    // CEK STAGE & BINTANG
    // =====================================================

    public static int HitungBintangStageStatic(int stage)
    {
        int stars = 0;
        for (int lvl = 1; lvl <= TOTAL_LEVEL; lvl++)
        {
            if (IsLevelSelesaiStatic(stage, lvl))
            {
                stars++;
            }
        }
        return stars;
    }

    public int HitungBintangStage(int stage)
    {
        return HitungBintangStageStatic(stage);
    }

    public static bool IsStageSelesaiStatic(int stage)
    {
        return HitungBintangStageStatic(stage) == TOTAL_LEVEL;
    }

    public bool IsStageSelesai(int stage)
    {
        return IsStageSelesaiStatic(stage);
    }

    public static int HitungTotalStageSelesaiStatic()
    {
        int count = 0;
        for (int s = 1; s <= TOTAL_STAGE; s++)
        {
            if (IsStageSelesaiStatic(s)) count++;
        }
        return count;
    }

    public bool IsStageTerbuka(int stage)
    {
        if (stage <= 1) return true;
        return IsStageSelesaiStatic(stage - 1);
    }

    // =====================================================
    // BUKA SCENE STAGE (MEMBUKA LEVEL & SUBLEVEL YANG BELUM SELESAI)
    // =====================================================

    public void BukaStage(int stage)
    {
        if (!IsStageTerbuka(stage))
        {
            Debug.Log($"[CHECKPOINT] Stage {stage} masih terkunci!");
            return;
        }

        // Cari level pertama yang belum diselesaikan
        int targetLevel = 1;
        for (int lvl = 1; lvl <= TOTAL_LEVEL; lvl++)
        {
            if (!IsLevelSelesai(stage, lvl))
            {
                targetLevel = lvl;
                break;
            }
        }

        int targetSubLevel = 1;
        if (ApakahMemilikiSubLevel(stage, targetLevel))
        {
            for (int sub = 1; sub <= TOTAL_SUBLEVEL; sub++)
            {
                if (!IsSubLevelSelesai(stage, targetLevel, sub))
                {
                    targetSubLevel = sub;
                    break;
                }
            }
        }

        string sceneName = GetNamaScene(stage, targetLevel, targetSubLevel);

        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"[CHECKPOINT] Membuka Stage {stage} Level {targetLevel} SubLevel {targetSubLevel} -> Scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"[CHECKPOINT] Scene untuk Stage {stage} Level {targetLevel} tidak ditemukan!");
        }
    }

    // =====================================================
    // MAPPING RESMI NAMA SCENE KE STAGE + LEVEL + SUBLEVEL
    // =====================================================

    public static string GetNamaScene(int stage, int level, int subLevel = 1)
    {
        switch (stage)
        {
            case 1:
                if (level == 1)
                {
                    if (subLevel == 1) return "Stage1_Eksplorasi";
                    if (subLevel == 2) return "Stage1_Eksplorasi_1";
                    if (subLevel == 3) return "Stage1_Eksplorasi_2";
                }
                else if (level == 2)
                {
                    if (subLevel == 1) return "Stage1_TebingKabut";
                    if (subLevel == 2) return "Stage1_TebingKabut1";
                    if (subLevel == 3) return "Stage1_TebingKabut2";
                }
                else if (level == 3)
                {
                    if (subLevel == 1) return "Stage1_GarisBilangan";
                    if (subLevel == 2) return "Stage1_GarisBilangan1";
                    if (subLevel == 3) return "Stage1_GarisBilangan2";
                }
                break;

            case 2:
                if (level == 1)
                {
                    if (subLevel == 1) return "Stage2_PengenalanPasar";
                    if (subLevel == 2) return "Stage2_PengenalanPasar1";
                    if (subLevel == 3) return "Stage2_PengenalanPasar2";
                }
                else if (level == 2)
                {
                    if (subLevel == 1) return "Stage2_Hitung_Penjual_Buah";
                    if (subLevel == 2) return "Stage2_Hitung_Penjual_Buah1";
                    if (subLevel == 3) return "Stage2_Hitung_Penjual_Buah2";
                }
                else if (level == 3)
                {
                    return "Stage2_EvaluasiPasar";
                }
                break;

            case 3:
                if (level == 1)
                {
                    if (subLevel == 1) return "Stage3_PengaturSuhu";
                    if (subLevel == 2) return "Stage3_PengaturSuhu1";
                    if (subLevel == 3) return "Stage3_PengaturSuhu2";
                }
                else if (level == 2)
                {
                    if (subLevel == 1) return "Stage3_SuhuLanjutan";
                    if (subLevel == 2) return "Stage3_SuhuLanjutan1";
                    if (subLevel == 3) return "Stage3_SuhuLanjutan2";
                }
                else if (level == 3)
                {
                    return "Stage3_SuhuCampuran";
                }
                break;

            case 4:
                if (level == 1)
                {
                    if (subLevel == 1) return "Stage4_PemecahDinding";
                    if (subLevel == 2) return "Stage4_PemecahDinding1";
                    if (subLevel == 3) return "Stage4_PemecahDinding2";
                }
                else if (level == 2)
                {
                    if (subLevel == 1) return "Stage4_SoalObor";
                    if (subLevel == 2) return "Stage4_SoalObor1";
                    if (subLevel == 3) return "Stage4_SoalObor2";
                }
                else if (level == 3)
                {
                    return "Stage4_EnergiPenghancur";
                }
                break;

            case 5:
                if (level == 1)
                {
                    if (subLevel == 1) return "Stage5_KonsepFPB";
                    if (subLevel == 2) return "Stage5_KonsepFPB1";
                    if (subLevel == 3) return "Stage5_KonsepFPB2";
                }
                else if (level == 2)
                {
                    if (subLevel == 1) return "Stage5_PohonFaktor";
                    if (subLevel == 2) return "Stage5_PohonFaktor1";
                    if (subLevel == 3) return "Stage5_PohonFaktor2";
                }
                else if (level == 3)
                {
                    return "Stage5_BentengUtama";
                }
                break;

            case 6:
                if (level == 1)
                {
                    if (subLevel == 1) return "Stage6_GembokGerbang";
                    if (subLevel == 2) return "Stage6_GembokGerbang1";
                    if (subLevel == 3) return "Stage6_GembokGerbang2";
                }
                else if (level == 2)
                {
                    if (subLevel == 1) return "Stage6_TembusPerisai";
                    if (subLevel == 2) return "Stage6_TembusPerisai1";
                    if (subLevel == 3) return "Stage6_TembusPerisai2";
                }
                else if (level == 3)
                {
                    return "Stage6_FinalBoss";
                }
                break;
        }

        return "";
    }

    public string GetSceneNameForLevel(int stage, int level)
    {
        return GetNamaScene(stage, level, 1);
    }

    private string GetSceneName(int stage)
    {
        return GetSceneNameForLevel(stage, 1);
    }

    // =====================================================
    // SISTEM DETEKSI PROGRES SCENE
    // =====================================================

    public static bool TryGetInfoScene(string sceneName, out int stage, out int level, out int subLevel)
    {
        return TryGetInfoScene(sceneName, out stage, out level, out subLevel, out _);
    }

    public static bool TryGetInfoScene(string sceneName, out int stage, out int level, out int subLevel, out bool isAkhirLevel)
    {
        stage = 1;
        level = 1;
        subLevel = 1;
        isAkhirLevel = false;

        if (string.IsNullOrEmpty(sceneName)) return false;

        switch (sceneName)
        {
            // STAGE 1 (Semua level punya 3 sublevel)
            case "Stage1_Eksplorasi":     stage = 1; level = 1; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage1_Eksplorasi_1":   stage = 1; level = 1; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage1_Eksplorasi_2":   stage = 1; level = 1; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage1_TebingKabut":    stage = 1; level = 2; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage1_TebingKabut1":   stage = 1; level = 2; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage1_TebingKabut2":   stage = 1; level = 2; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage1_GarisBilangan":  stage = 1; level = 3; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage1_GarisBilangan1": stage = 1; level = 3; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage1_GarisBilangan2": stage = 1; level = 3; subLevel = 3; isAkhirLevel = true;  return true;

            // STAGE 2
            case "Stage2_PengenalanPasar":    stage = 2; level = 1; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage2_PengenalanPasar1":   stage = 2; level = 1; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage2_PengenalanPasar2":   stage = 2; level = 1; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage2_Hitung_Penjual_Buah":  stage = 2; level = 2; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage2_Hitung_Penjual_Buah1": stage = 2; level = 2; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage2_Hitung_Penjual_Buah2": stage = 2; level = 2; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage2_EvaluasiPasar":      stage = 2; level = 3; subLevel = 1; isAkhirLevel = true;  return true; // Tanpa SubLevel

            // STAGE 3
            case "Stage3_PengaturSuhu":   stage = 3; level = 1; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage3_PengaturSuhu1":  stage = 3; level = 1; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage3_PengaturSuhu2":  stage = 3; level = 1; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage3_SuhuLanjutan":   stage = 3; level = 2; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage3_SuhuLanjutan1":  stage = 3; level = 2; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage3_SuhuLanjutan2":  stage = 3; level = 2; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage3_SuhuCampuran":   stage = 3; level = 3; subLevel = 1; isAkhirLevel = true;  return true; // Tanpa SubLevel

            // STAGE 4
            case "Stage4_PemecahDinding":   stage = 4; level = 1; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage4_PemecahDinding1":  stage = 4; level = 1; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage4_PemecahDinding2":  stage = 4; level = 1; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage4_SoalObor":         stage = 4; level = 2; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage4_SoalObor1":        stage = 4; level = 2; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage4_SoalObor2":        stage = 4; level = 2; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage4_EnergiPenghancur": stage = 4; level = 3; subLevel = 1; isAkhirLevel = true;  return true; // Tanpa SubLevel

            // STAGE 5
            case "Stage5_KonsepFPB":   stage = 5; level = 1; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage5_KonsepFPB1":  stage = 5; level = 1; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage5_KonsepFPB2":  stage = 5; level = 1; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage5_PohonFaktor": stage = 5; level = 2; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage5_PohonFaktor1": stage = 5; level = 2; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage5_PohonFaktor2": stage = 5; level = 2; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage5_BentengUtama": stage = 5; level = 3; subLevel = 1; isAkhirLevel = true;  return true; // Tanpa SubLevel

            // STAGE 6
            case "Stage6_GembokGerbang":  stage = 6; level = 1; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage6_GembokGerbang1": stage = 6; level = 1; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage6_GembokGerbang2": stage = 6; level = 1; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage6_TembusPerisai":  stage = 6; level = 2; subLevel = 1; isAkhirLevel = false; return true;
            case "Stage6_TembusPerisai1": stage = 6; level = 2; subLevel = 2; isAkhirLevel = false; return true;
            case "Stage6_TembusPerisai2": stage = 6; level = 2; subLevel = 3; isAkhirLevel = true;  return true;
            case "Stage6_FinalBoss":      stage = 6; level = 3; subLevel = 1; isAkhirLevel = true;  return true; // Tanpa SubLevel

            default:
                return false;
        }
    }

    public static void CatatProgressSceneSelesai(string sceneName)
    {
        if (TryGetInfoScene(sceneName, out int stage, out int level, out int subLevel, out _))
        {
            if (ApakahMemilikiSubLevel(stage, level))
            {
                // Level memiliki sublevel: HANYA catat sublevel yang bersangkutan
                if (Instance != null)
                {
                    Instance.SelesaikanSubLevel(stage, level, subLevel);
                }
                else
                {
                    PlayerPrefs.SetInt($"Stage_{stage}_Level_{level}_Sub_{subLevel}", 1);
                    PlayerPrefs.SetInt($"Stage_{stage}_Level_{level}_SubLevel_{subLevel}", 1);
                    PlayerPrefs.Save();
                    Debug.Log($"[PROGRESS] Stage {stage} Level {level} SubLevel {subLevel} = SELESAI");

                    if (IsLevelSelesaiStatic(stage, level))
                    {
                        PlayerPrefs.SetInt($"Stage_{stage}_Level_{level}_Selesai", 1);
                        PlayerPrefs.Save();
                        Debug.Log($"[PROGRESS] LEVEL {level} STAGE {stage} SELESAI PENUH! ⭐");
                    }
                }
            }
            else
            {
                // Level TIDAK memiliki sublevel (hanya 1 gameplay langsung)
                if (Instance != null)
                {
                    Instance.SelesaikanLevelTanpaSubLevel(stage, level);
                }
                else
                {
                    PlayerPrefs.SetInt($"Stage_{stage}_Level_{level}_Selesai", 1);
                    PlayerPrefs.SetInt($"Stage_{stage}_Level_{level}_Sub_1", 1);
                    PlayerPrefs.SetInt($"Stage_{stage}_Level_{level}_SubLevel_1", 1);
                    PlayerPrefs.Save();
                    Debug.Log($"[PROGRESS] Stage {stage} Level {level} (Tanpa SubLevel) = SELESAI");
                    Debug.Log($"[PROGRESS] LEVEL {level} STAGE {stage} SELESAI PENUH! ⭐");
                }
            }
        }
        else
        {
            Debug.LogWarning($"[PROGRESS] Scene '{sceneName}' tidak terdaftar dalam konfigurasi stage/level.");
        }
    }

    public static bool ApakahLevelTerakhirSublevel(string sceneName)
    {
        if (TryGetInfoScene(sceneName, out _, out _, out _, out bool isAkhirLevel))
        {
            return isAkhirLevel;
        }
        return false;
    }

    // =====================================================
    // KEY PLAYER PREFS
    // =====================================================

    private string GetSubLevelKey(int stage, int level, int subLevel)
    {
        return $"Stage_{stage}_Level_{level}_Sub_{subLevel}";
    }

    // =====================================================
    // RESET PROGRESS KHUSUS CHECKPOINT (SECTION N)
    // =====================================================

    [ContextMenu("Reset Progress Checkpoint")]
    public void ResetProgress()
    {
        ResetProgressKhususCheckpoint();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Reset Progress Checkpoint")]
#endif
    public static void ResetProgressKhususCheckpoint()
    {
        for (int s = 1; s <= TOTAL_STAGE; s++)
        {
            for (int lvl = 1; lvl <= TOTAL_LEVEL; lvl++)
            {
                PlayerPrefs.DeleteKey($"Stage_{s}_Level_{lvl}_Selesai");
                PlayerPrefs.DeleteKey($"Stage_{s}_Level_{lvl}_Completed");
                for (int sub = 1; sub <= TOTAL_SUBLEVEL; sub++)
                {
                    PlayerPrefs.DeleteKey($"Stage_{s}_Level_{lvl}_Sub_{sub}");
                    PlayerPrefs.DeleteKey($"Stage_{s}_Level_{lvl}_SubLevel_{sub}");
                }
            }
            PlayerPrefs.DeleteKey($"Stage_{s}_Selesai");
            PlayerPrefs.DeleteKey($"Stage_{s}_Stars");
        }
        PlayerPrefs.DeleteKey("StagePetunjukTujuan");
        PlayerPrefs.DeleteKey("StageSelesai");
        PlayerPrefs.Save();

        Debug.Log("[CHECKPOINT] Progress checkpoint berhasil direset (TotalKoin, StatSoal, WaktuBermain dsb tetap aman).");
    }
}