using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// WinManager: Pengelola terpusat untuk kondisi kemenangan (Win) di seluruh Stage (1-6).
/// Memastikan:
/// 1. Semua kemenangan selalu menampilkan Win UI terlebih dahulu.
/// 2. Validasi misi HANYA dijalankan untuk Stage 1 Level 1.
/// 3. Catatan progress sublevel disimpan ke CheckpointManager.
/// 4. Tombol Win UI menentukan alur berikutnya secara dinamis (sublevel berikutnya atau Map_Checkpoint).
/// 5. Mencegah duplikasi eksekusi proses menang dalam satu gameplay.
/// 6. Mereset seluruh state kemenangan sementara setiap kali scene baru dimuat (bebas bug antar-level).
/// 7. Mencari Win UI di scene aktif secara andal termasuk objek non-aktif (inactive).
/// 8. Memulihkan Time.timeScale ke 1f saat masuk ke scene baru.
/// </summary>
public class WinManager : MonoBehaviour
{
    private static WinManager _instance;
    public static WinManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindFirstObjectByType<WinManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("WinManager");
                    _instance = go.AddComponent<WinManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    // State Kemenangan Sementara (Reset setiap scene baru dimuat)
    public static bool winSudahDitampilkan = false;
    public static bool isProcessingWin = false;
    public static bool winTriggered = false;
    private static bool sedangPindahScene = false;
    private static bool winTriggerLogged = false;

    // Cache Win UI untuk scene yang sedang aktif (tidak pernah menyimpan referensi scene lama)
    private static GameObject targetWinUICache = null;

    // Alias kompatibilitas jika ada script lama yang mengakses 'sudahMenang'
    public static bool sudahMenang
    {
        get => winSudahDitampilkan;
        set => winSudahDitampilkan = value;
    }

    // =========================================================================
    // RUNTIME INITIALIZATION: MENJAMIN LISTENER ON SCENE LOADED SELALU TERPASANG
    // =========================================================================
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InisialisasiRuntime()
    {
        // Pastikan listener hanya terdaftar 1 kali
        SceneManager.sceneLoaded -= OnSceneLoadedStatic;
        SceneManager.sceneLoaded += OnSceneLoadedStatic;

        // Reset state awal
        ResetStateForNewScene();
    }

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

    // =========================================================================
    // EVENT SCENE LOADED: RESET STATE SEMENTARA & PERSIAPAN SCENE BARU
    // =========================================================================
    private static void OnSceneLoadedStatic(Scene scene, LoadSceneMode mode)
    {
        // 1. Pastikan Time.timeScale dipulihkan ke 1f
        Time.timeScale = 1f;

        // 2. Reset state sementara untuk scene baru (Progress PlayerPrefs tetap utuh)
        ResetStateForNewScene();

        // 3. Debug log wajib saat scene baru masuk
        Debug.Log($"[SCENE INIT]\nScene = {scene.name}");
        Debug.Log("[WIN STATE RESET]\nwinSudahDitampilkan = false\nisProcessingWin = false");

        // 4. Cari Win UI di scene baru dan siapkan referensinya
        targetWinUICache = CariWinUIDiSceneAktif();
        if (targetWinUICache != null)
        {
            Debug.Log("[WIN UI]\nWinPanel ditemukan pada scene aktif");
        }
    }

    /// <summary>
    /// Mereset semua state sementara gameplay win untuk scene baru.
    /// </summary>
    public static void ResetStateForNewScene()
    {
        winSudahDitampilkan = false;
        isProcessingWin = false;
        winTriggered = false;
        sedangPindahScene = false;
        winTriggerLogged = false;
        targetWinUICache = null;
    }

    // =========================================================================
    // VALIDASI MISI: HANYA BERLAKU UNTUK STAGE 1 LEVEL 1
    // =========================================================================
    public static bool ApakahSceneMembutuhkanMisi(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return false;

        // Hanya scene-scene pada Stage 1 Level 1 (Eksplorasi) yang menggunakan MisiManager
        if (sceneName == "Stage1_Eksplorasi" ||
            sceneName == "Stage1_Eksplorasi_1" ||
            sceneName == "Stage1_Eksplorasi_2")
        {
            return true;
        }

        if (CheckpointManager.TryGetInfoScene(sceneName, out int stage, out int level, out _))
        {
            return stage == 1 && level == 1;
        }

        return false;
    }

    // =========================================================================
    // METODE UTAMA: TAMPILKAN WIN
    // =========================================================================
    public static bool TampilkanWin(GameObject customWinUI = null)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // 1. Tampilkan log WIN TRIGGER jika belum pernah dicetak di scene ini
        if (!winTriggerLogged)
        {
            winTriggerLogged = true;
            if (CheckpointManager.TryGetInfoScene(currentScene, out int stg, out int lvl, out int subLvl))
            {
                Debug.Log($"[WIN TRIGGER]\nStage = {stg}\nLevel = {lvl}\nSubLevel = {subLvl}");
            }
            else
            {
                Debug.Log($"[WIN TRIGGER]\nScene = {currentScene}");
            }
        }

        // 2. Log pemanggilan WinManager
        Debug.Log("[WIN MANAGER]\nTampilkanWin() dipanggil");

        // 3. Cegah duplikasi proses kemenangan dalam scene yang sama
        if (winSudahDitampilkan || isProcessingWin)
        {
            Debug.Log("[WIN MANAGER] Panggilan kemenangan diabaikan - sudah dalam status menang.");
            return true;
        }

        // 4. Cek apakah scene ini memerlukan validasi misi (HANYA STAGE 1 LEVEL 1)
        if (ApakahSceneMembutuhkanMisi(currentScene))
        {
            MisiManager mm = Object.FindFirstObjectByType<MisiManager>();
            if (mm != null && !mm.ApakahSemuaMisiSelesai())
            {
                Debug.Log("[WIN MANAGER] Belum dapat menang - mission belum selesai.");

                // Tampilkan dialog peringatan misi belum selesai
                DialogManager dm = DialogManager.Instance;
                if (dm != null)
                {
                    BarisDialog[] percakapan = new BarisDialog[]
                    {
                        new BarisDialog
                        {
                            namaPembicara = "Aki Aruna",
                            kalimat = "Kamu belum menyelesaikan semua target misi pada level ini! Selesaikan terlebih dahulu sebelum melangkah ke garis finish."
                        }
                    };
                    dm.TampilkanDialog(percakapan, () => { });
                }

                return false;
            }
        }

        // 5. Kunci status menang agar tidak terduplikasi
        winSudahDitampilkan = true;
        isProcessingWin = true;
        winTriggered = true;

        // 6. Catat progress selesai di CheckpointManager
        CheckpointManager.CatatProgressSceneSelesai(currentScene);

        // --- MENGIRIM DATA KE LMS (LEARN) KETIKA MENANG ---
        try
        {
            int koinS = GameStatisticsManager.GetTotalKoin();
            if (AnalyticsTracker.Instance != null)
            {
                AnalyticsTracker.Instance.TrackScore(koinS);
                AnalyticsTracker.Instance.TrackCompletionPercentage(100f);
                AnalyticsTracker.Instance.EndSession(completed: true);

                if (LMSManager.Instance != null)
                {
                    LMSManager.Instance.KirimAnalyticsKeMoodle(AnalyticsTracker.Instance.CurrentSession);
                    Debug.Log("✅ [WinManager] Mengirim analytics WIN ke Moodle");
                }
            }
            else if (LMSManager.Instance != null)
            {
                LMSManager.Instance.KirimKoinKeMoodle(koinS);
                Debug.Log("✅ [WinManager] Mengirim Koin WIN ke Moodle");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[WinManager] Gagal mengirim data LMS: " + e.Message);
        }
        // ----------------------------------------------------

        // 7. Tentukan objek Win UI pada scene aktif
        GameObject targetWinUI = null;

        // Jika customWinUI valid dan berasal dari scene aktif, gunakan itu
        if (customWinUI != null && customWinUI.scene == SceneManager.GetActiveScene())
        {
            targetWinUI = customWinUI;
        }
        else if (targetWinUICache != null && targetWinUICache.scene == SceneManager.GetActiveScene())
        {
            targetWinUI = targetWinUICache;
        }
        else
        {
            targetWinUI = CariWinUIDiSceneAktif();
        }

        if (targetWinUI != null)
        {
            Debug.Log("[WIN UI]\nWinPanel ditemukan pada scene aktif");

            // Matikan semua panel gameplay lain yang mengganggu
            Flag.NonaktifkanSemuaPanelLain(targetWinUI);

            // Sambungkan semua button di dalam Win UI ke LanjutSetelahWin
            PasangListenerTombol(targetWinUI);

            targetWinUI.SetActive(true);
            Time.timeScale = 0f;

            Debug.Log("[WIN UI]\nWinPanel.SetActive(true)");
        }
        else
        {
            Debug.LogWarning("[WIN UI] Objek Win UI tidak ditemukan di scene! Membuat Win UI fallback generic...");
            BuatWinUIFallback();
        }

        return true;
    }

    public static bool TampilkanWin(int stage, int level, int subLevel, GameObject customWinUI = null)
    {
        return TampilkanWin(customWinUI);
    }

    // =========================================================================
    // PENCARIAN ROBUST WIN UI PADA SCENE AKTIF (TERMASUK GAMEOBJECT INACTIVE)
    // =========================================================================
    public static GameObject CariWinUIDiSceneAktif()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.isLoaded) return null;

        // Strategi 1: Cari melalui komponen NextLevel di scene aktif
        NextLevel[] allNextLevels = Resources.FindObjectsOfTypeAll<NextLevel>();
        foreach (NextLevel nl in allNextLevels)
        {
            if (nl != null && nl.gameObject.scene == activeScene)
            {
                Transform t = nl.transform;
                while (t != null)
                {
                    if (t.name.Equals("WinUI", System.StringComparison.OrdinalIgnoreCase) ||
                        t.name.IndexOf("Win", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        t.name.IndexOf("Evaluasi", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return t.gameObject;
                    }

                    if (t.parent == null || t.parent.GetComponent<Canvas>() != null)
                    {
                        return t.gameObject;
                    }
                    t = t.parent;
                }
                return nl.gameObject;
            }
        }

        // Strategi 2: Cari melalui seluruh Root GameObjects dan anaknya (termasuk inactive)
        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        string[] candidateNames = new string[] {
            "WinUI",
            "panelWinUi",
            "panelWin",
            "Panel Evaluasi",
            "Panel_Evaluasi",
            "PanelWinUI",
            "Win_UI",
            "WinPanel"
        };

        foreach (string cName in candidateNames)
        {
            foreach (GameObject root in rootObjects)
            {
                if (root == null) continue;

                if (root.name.Equals(cName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return root;
                }

                Transform[] children = root.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    if (child != null && child.name.Equals(cName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return child.gameObject;
                    }
                }
            }
        }

        return null;
    }

    // =========================================================================
    // SAMBUNGKAN LISTENER TOMBOL PADA WIN UI
    // =========================================================================
    private static void PasangListenerTombol(GameObject winUIRoot)
    {
        if (winUIRoot == null) return;

        // Cari semua Button di dalam panel Win UI (termasuk inactive)
        Button[] buttons = winUIRoot.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (btn == null) continue;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                LanjutSetelahWin();
            });
        }
    }

    // =========================================================================
    // PENENTUAN TUJUAN DINAMIS SETELAH WIN (SECTION 12 & 16)
    // =========================================================================
    public static void LanjutSetelahWin()
    {
        if (sedangPindahScene) return;
        sedangPindahScene = true;

        Time.timeScale = 1f;

        Debug.Log("[WIN NEXT]\nMenentukan tujuan setelah Win");

        string currentScene = SceneManager.GetActiveScene().name;
        if (!CheckpointManager.TryGetInfoScene(currentScene, out int stage, out int level, out int subLevel))
        {
            Debug.LogWarning($"[WIN NEXT] Scene '{currentScene}' tidak terdaftar! Kembali ke Map_Checkpoint sebagai fallback.");
            SceneManager.LoadScene("Map_Checkpoint");
            return;
        }

        bool hasSub = CheckpointManager.ApakahMemilikiSubLevel(stage, level);

        if (hasSub)
        {
            if (subLevel < 3)
            {
                int nextSub = subLevel + 1;
                string nextScene = CheckpointManager.GetNamaScene(stage, level, nextSub);

                Debug.Log($"[WIN NEXT]\nMasih ada SubLevel\nMenuju Stage {stage} Level {level} Sub {nextSub}");

                if (!string.IsNullOrEmpty(nextScene))
                {
                    SceneManager.LoadScene(nextScene);
                }
                else
                {
                    Debug.LogError($"[WIN NEXT] Scene berikutnya untuk Stage {stage} Level {level} Sub {nextSub} tidak ditemukan! Kembali ke Map_Checkpoint.");
                    SceneManager.LoadScene("Map_Checkpoint");
                }
            }
            else
            {
                // SubLevel 3 (terakhir)
                Debug.Log("[WIN NEXT]\nSubLevel terakhir / level tanpa sublevel\nMenuju Map_Checkpoint");
                SceneManager.LoadScene("Map_Checkpoint");
            }
        }
        else
        {
            // Level tidak memiliki sublevel (misal: Stage 2..6 Level 3)
            Debug.Log("[WIN NEXT]\nSubLevel terakhir / level tanpa sublevel\nMenuju Map_Checkpoint");
            SceneManager.LoadScene("Map_Checkpoint");
        }
    }

    // =========================================================================
    // KEMBALI KE MAP_CHECKPOINT (KOMPATIBILITAS JIKA DIPANGGIL LANGSUNG)
    // =========================================================================
    public static void PindahKeMapCheckpoint()
    {
        if (sedangPindahScene) return;
        sedangPindahScene = true;

        Time.timeScale = 1f;

        Debug.Log("[WIN NEXT]\nSubLevel terakhir / level tanpa sublevel\nMenuju Map_Checkpoint");
        SceneManager.LoadScene("Map_Checkpoint");
    }

    // =========================================================================
    // FALLBACK GENERIC WIN UI (JIKA SUATU SCENE TIDAK MEMILIKI OBJEK WIN UI)
    // =========================================================================
    private static void BuatWinUIFallback()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        GameObject canvasObj = canvas != null ? canvas.gameObject : null;

        if (canvasObj == null)
        {
            canvasObj = new GameObject("Canvas_Fallback_Win");
            Canvas c = canvasObj.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject panel = new GameObject("WinUI_Fallback");
        panel.transform.SetParent(canvasObj.transform, false);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.85f);

        // Teks Title
        GameObject textObj = new GameObject("Text_Title");
        textObj.transform.SetParent(panel.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchoredPosition = new Vector2(0, 50);
        textRt.sizeDelta = new Vector2(600, 100);
        Text txt = textObj.AddComponent<Text>();
        txt.text = "LEVEL SELESAI!";
        txt.fontSize = 42;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.yellow;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Tombol Lanjut
        GameObject btnObj = new GameObject("Button_Lanjut");
        btnObj.transform.SetParent(panel.transform, false);
        RectTransform btnRt = btnObj.AddComponent<RectTransform>();
        btnRt.anchoredPosition = new Vector2(0, -60);
        btnRt.sizeDelta = new Vector2(240, 70);

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.7f, 0.2f, 1f);

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            LanjutSetelahWin();
        });

        // Label Tombol
        GameObject btnTextObj = new GameObject("Text_Btn");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        RectTransform btnTextRt = btnTextObj.AddComponent<RectTransform>();
        btnTextRt.sizeDelta = Vector2.zero;
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        Text btnTxt = btnTextObj.AddComponent<Text>();
        btnTxt.text = "Lanjut";
        btnTxt.fontSize = 26;
        btnTxt.alignment = TextAnchor.MiddleCenter;
        btnTxt.color = Color.white;
        btnTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Time.timeScale = 0f;
        Debug.Log("[WIN UI]\nWinPanel.SetActive(true)");
    }
}

