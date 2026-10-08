using UnityEngine;

public class StagePoint : MonoBehaviour
{
    [Header("IDENTITAS STAGE")]
    public int stageNumber;

    [Header("ICON STAGE")]
    public SpriteRenderer stageIcon;

    [Header("3 BINTANG LEVEL")]
    public SpriteRenderer[] bintang = new SpriteRenderer[3];

    [Header("WARNA")]
    public Color warnaTerbuka = Color.white;
    public Color warnaTerkunci = Color.gray;

    public Color warnaBintangAktif = Color.yellow;
    public Color warnaBintangKosong = Color.gray;

    [Header("HIGHLIGHT PETUNJUK")]
    [Tooltip("Warna saat efek kedip / highlight (lebih terang)")]
    public Color warnaHighlight = new Color(1f, 0.95f, 0.4f, 1f);

    [Tooltip("Jumlah siklus kedip (terang - normal)")]
    public int jumlahKedip = 4;

    [Tooltip("Durasi setiap perubahan warna dalam detik")]
    public float durasiPerKedip = 0.3f;

    private Collider2D colliderStage;

    private void Awake()
    {
        // Ambil Collider2D yang ada di object StagePoint
        colliderStage = GetComponent<Collider2D>();

        if (stageIcon == null)
        {
            Transform iconChild = transform.Find("Icon");
            if (iconChild != null)
            {
                stageIcon = iconChild.GetComponent<SpriteRenderer>();
            }
            if (stageIcon == null)
            {
                stageIcon = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (colliderStage == null)
        {
            Debug.LogError(
                $"[STAGE {stageNumber}] TIDAK ADA COLLIDER 2D!"
            );
        }
    }

    private void Start()
    {
        SetupStage();
    }

    // =====================================================
    // SETUP STAGE
    // =====================================================

    public void SetupStage()
    {
        if (CheckpointManager.Instance == null)
        {
            Debug.LogWarning(
                $"[STAGE {stageNumber}] CheckpointManager belum ditemukan!"
            );

            return;
        }

        bool stageTerbuka =
            CheckpointManager.Instance.IsStageTerbuka(stageNumber);

        Debug.Log(
            $"[STAGE {stageNumber}] Terbuka = {stageTerbuka}"
        );

        // ===============================
        // WARNA ICON
        // ===============================

        if (stageIcon != null && kedipCoroutine == null)
        {
            stageIcon.color = GetWarnaNormal();
        }

        // ===============================
        // COLLIDER
        // ===============================

        if (colliderStage != null)
        {
            colliderStage.enabled = stageTerbuka;
        }

        // ===============================
        // BINTANG
        // ===============================

        UpdateBintang();
    }

    // =====================================================
    // UPDATE BINTANG
    // =====================================================

    public void UpdateBintang()
    {
        if (CheckpointManager.Instance == null)
            return;

        for (int i = 0; i < 3; i++)
        {
            if (bintang == null ||
                i >= bintang.Length ||
                bintang[i] == null)
            {
                continue;
            }

            int level = i + 1;

            bool selesai =
                CheckpointManager.Instance.IsLevelSelesai(
                    stageNumber,
                    level
                );

            bintang[i].color =
                selesai
                ? warnaBintangAktif
                : warnaBintangKosong;
        }
    }

    // =====================================================
    // KLIK STAGE
    // =====================================================

private void OnMouseDown()
{
    Debug.Log($"========================================");
    Debug.Log($"[STAGE {stageNumber}] OnMouseDown TERPANGGIL!");
    Debug.Log($"Collider enabled = {(colliderStage != null && colliderStage.enabled)}");
    Debug.Log($"CheckpointManager.Instance = {(CheckpointManager.Instance != null)}");

    KlikStage();
}

    // =====================================================
    // BUKA GAMEPLAY
    // =====================================================

    private void KlikStage()
    {
        HentikanBerkedip();

        if (CheckpointManager.Instance == null)
        {
            Debug.LogError(
                "[STAGE] CheckpointManager tidak ditemukan!"
            );

            return;
        }

        bool terbuka =
            CheckpointManager.Instance.IsStageTerbuka(
                stageNumber
            );

        Debug.Log(
            $"[STAGE {stageNumber}] Terbuka = {terbuka}"
        );

        if (!terbuka)
        {
            Debug.Log(
                $"[STAGE {stageNumber}] MASIH TERKUNCI!"
            );

            return;
        }

        Debug.Log(
            $"[STAGE {stageNumber}] MEMBUKA GAMEPLAY..."
        );

        CheckpointManager.Instance.BukaStage(
            stageNumber
        );
    }

    // =====================================================
    // WARNA NORMAL STAGE
    // =====================================================

    public Color GetWarnaNormal()
    {
        bool stageTerbuka = false;
        if (CheckpointManager.Instance != null)
        {
            stageTerbuka = CheckpointManager.Instance.IsStageTerbuka(stageNumber);
        }
        else
        {
            stageTerbuka = (stageNumber == 1);
        }

        return stageTerbuka ? warnaTerbuka : warnaTerkunci;
    }

    // =====================================================
    // EFEK KEDIP WARNA ICON (NAVIGASI DARI PETUNJUK)
    // =====================================================

    private Coroutine kedipCoroutine;

    public void MulaiBerkedip()
    {
        if (kedipCoroutine != null)
        {
            StopCoroutine(kedipCoroutine);
            kedipCoroutine = null;
        }

        kedipCoroutine = StartCoroutine(KedipIcon());
    }

    public void HentikanBerkedip()
    {
        if (kedipCoroutine != null)
        {
            StopCoroutine(kedipCoroutine);
            kedipCoroutine = null;
            Debug.Log($"[STAGE POINT] Highlight Stage {stageNumber} selesai");
        }

        if (stageIcon != null)
        {
            stageIcon.color = GetWarnaNormal();
        }
    }

    private System.Collections.IEnumerator KedipIcon()
    {
        if (stageIcon == null) yield break;

        Debug.Log($"[STAGE POINT] Highlight Stage {stageNumber} dimulai");

        // 1. Simpan warna normal saat ini (terbuka = warna asli, terkunci = abu-abu)
        Color warnaAsli = GetWarnaNormal();
        stageIcon.color = warnaAsli;

        // Tentukan warna lebih terang / highlight
        Color warnaTerang = warnaHighlight;
        if (warnaTerang.a <= 0.01f || warnaTerang == warnaAsli)
        {
            warnaTerang = Color.yellow;
        }

        int totalLoop = jumlahKedip > 0 ? jumlahKedip : 4;
        float durasi = durasiPerKedip > 0.05f ? durasiPerKedip : 0.3f;

        for (int i = 0; i < totalLoop; i++)
        {
            // Ubah warna menjadi lebih terang
            stageIcon.color = warnaTerang;
            yield return new WaitForSeconds(durasi);

            // Kembalikan ke warna asli
            stageIcon.color = warnaAsli;
            yield return new WaitForSeconds(durasi);
        }

        // Pastikan kembali ke warna asli normal
        if (stageIcon != null)
        {
            stageIcon.color = warnaAsli;
        }

        kedipCoroutine = null;
        Debug.Log($"[STAGE POINT] Highlight Stage {stageNumber} selesai");
    }

    private void OnDisable()
    {
        HentikanBerkedip();
    }
}