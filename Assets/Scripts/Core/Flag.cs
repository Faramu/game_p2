using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject winUI;

    // OPSIONAL: Jika ingin tetap menggunakan reference manual di Inspector
    // Jika null, akan otomatis menggunakan LMSManager.Instance
    [Tooltip("(Optional) Drag LMSManager ke sini. Jika kosong, akan otomatis menggunakan Instance.")]
    public LMSManager lmsManager;

    [Header("--- VALIDASI MISI ---")]
    [Tooltip("Wajib menyelesaikan semua misi sebelum level dianggap selesai?")]
    public bool wajibSelesaikanMisi = true;

    [Tooltip("Referensi MisiManager di scene (opsional, jika kosong akan dicari otomatis)")]
    public MisiManager misiManager;

    [Tooltip("Referensi Panel Misi GameObject (opsional)")]
    public GameObject panelMisi;

    [Tooltip("Referensi DialogManager (opsional, memakai DialogManager.Instance jika kosong)")]
    public DialogManager dialogManager;

    [Header("--- PESAN DIALOG MISI BELUM SELESAI ---")]
    public string namaPembicaraDialog = "Aki Aruna";
    [TextArea(2, 4)]
    public string pesanMisiBelumSelesai = "Kamu belum menyelesaikan semua target misi pada level ini! Selesaikan terlebih dahulu sebelum melangkah ke garis finish.";

    private bool levelSudahSelesai = false;

    private void Awake()
    {
        levelSudahSelesai = false;
    }

    private void OnEnable()
    {
        levelSudahSelesai = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ProsesTriggerPlayer(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !levelSudahSelesai)
        {
            // Jika player masih di area finish setelah misi diselesaikan
            if (ApakahMisiSudahSelesai())
            {
                ProsesTriggerPlayer(collision.gameObject);
            }
        }
    }

    private void ProsesTriggerPlayer(GameObject playerObj)
    {
        if (levelSudahSelesai) return;

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (CheckpointManager.TryGetInfoScene(currentScene, out int stage, out int level, out int subLevel))
        {
            Debug.Log($"[WIN TRIGGER]\nStage = {stage}\nLevel = {level}\nSubLevel = {subLevel}");
        }

        // Delegasikan validasi, pencatatan progress, dan penanganan Win UI ke WinManager
        bool berhasilMenang = WinManager.TampilkanWin(winUI);
        if (!berhasilMenang)
        {
            return;
        }

        levelSudahSelesai = true;

        Player player = playerObj != null ? playerObj.GetComponent<Player>() : null;
        if (player != null)
        {
            // End analytics session dan kirim data jika tersedia
            if (AnalyticsTracker.Instance != null)
            {
                AnalyticsTracker.Instance.TrackScore(player.coins);
                AnalyticsTracker.Instance.TrackHealthChanged(player.health);
                AnalyticsTracker.Instance.TrackCompletionPercentage(100f);
                AnalyticsTracker.Instance.EndSession(completed: true);

                LMSManager manager = lmsManager != null ? lmsManager : LMSManager.Instance;
                if (manager != null)
                {
                    manager.KirimAnalyticsKeMoodle(AnalyticsTracker.Instance.CurrentSession);
                    Debug.Log("✅ Flag: Mengirim analytics lengkap untuk User " + manager.userID + " di Course " + manager.courseID);
                }
            }
            else
            {
                LMSManager manager = lmsManager != null ? lmsManager : LMSManager.Instance;
                if (manager != null)
                {
                    manager.KirimKoinKeMoodle(player.coins);
                }
            }
        }
    }

    public bool ApakahMisiSudahSelesai()
    {
        // Validasi misi HANYA berlaku untuk Stage 1 Level 1
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!WinManager.ApakahSceneMembutuhkanMisi(currentScene))
        {
            return true;
        }

        if (!wajibSelesaikanMisi) return true;

        // Cek melalui MisiManager di scene Stage 1 Level 1
        MisiManager mm = misiManager != null ? misiManager : Object.FindFirstObjectByType<MisiManager>();
        if (mm != null)
        {
            return mm.ApakahSemuaMisiSelesai();
        }

        return true;
    }

    /// <summary>
    /// Menonaktifkan seluruh panel UI lain (seperti Panel Misi, HUD, Mobile Controls, dsb.)
    /// saat panel Win UI ditampilkan.
    /// </summary>
    public static void NonaktifkanSemuaPanelLain(GameObject targetWinUI)
    {
        if (targetWinUI == null) return;

        // 1. Nonaktifkan semua sibling di dalam Canvas yang sama
        Transform parentTransform = targetWinUI.transform.parent;
        if (parentTransform != null)
        {
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                if (child.gameObject != targetWinUI)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        // 2. Sembunyikan panel misi secara spesifik jika ada Skenario1Manager
        if (Skenario1Manager.Instance != null)
        {
            Skenario1Manager.Instance.SembunyikanUIMisi();
        }

        // 3. Sembunyikan panel misi melalui MisiManager jika ada di scene
        MisiManager mm = Object.FindFirstObjectByType<MisiManager>();
        if (mm != null)
        {
            mm.SembunyikanPanelMisi();
        }

        // 4. Pastikan objek panel tertentu nonaktif jika berada di luar Canvas utama
        string[] panelNames = new string[]
        {
            "Panel_Misi_BG",
            "Container_Misi",
            "MobileControls",
            "HealtyBar",
            "Header",
            "Panel_Dialog",
            "PauseMenu",
            "Panel Evaluasi",
            "Btn_Serang",
            "InputField (TMP)"
        };

        foreach (string pName in panelNames)
        {
            GameObject obj = GameObject.Find(pName);
            if (obj != null && obj != targetWinUI && !obj.transform.IsChildOf(targetWinUI.transform))
            {
                obj.SetActive(false);
            }
        }
    }
}