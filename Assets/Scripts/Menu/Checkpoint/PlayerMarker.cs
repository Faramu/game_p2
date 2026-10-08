using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    // Target stage dari Menu Petunjuk (static & PlayerPrefs agar persisten)
    public static int StagePetunjukTujuan = 0;
    public static int targetStageDariPetunjuk
    {
        get => StagePetunjukTujuan;
        set => StagePetunjukTujuan = value;
    }
    public static bool hasLoggedMapTarget = false;

    [Header("Posisi Icon Setiap Stage")]
    [Tooltip("Masukkan Transform Icon Stage 1 sampai Stage 6")]
    public Transform[] posisiStage;

    [Header("Pengaturan Gerakan")]
    public float kecepatan = 5f;

    private int stageSekarang;
    private Vector3 targetPosisi;
    private Coroutine gerakDanHighlightCoroutine;

    public static void LogTargetMapJikaAda()
    {
        int target = StagePetunjukTujuan > 0 ? StagePetunjukTujuan : PlayerPrefs.GetInt("StagePetunjukTujuan", 0);
        if (target > 0 && !hasLoggedMapTarget)
        {
            Debug.Log($"[MAP] Target Stage dari Petunjuk = {target}");
            hasLoggedMapTarget = true;
        }
    }

    private void Start()
    {
        Debug.Log("========================================");
        Debug.Log("[PLAYER MARKER] START");
        Debug.Log("Posisi awal: " + transform.position);

        LogTargetMapJikaAda();
        UpdateMarker();

        Debug.Log("========================================");
    }

    private void Update()
    {
        if (posisiStage == null || posisiStage.Length == 0)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosisi,
            kecepatan * Time.deltaTime
        );
    }

    public void UpdateMarker()
    {
        if (posisiStage == null || posisiStage.Length < 6)
        {
            Debug.LogWarning(
                "[PLAYER MARKER] Posisi Stage belum lengkap! " +
                "Pastikan ada 6 posisi."
            );

            return;
        }

        int targetStage = 1;
        int targetDariPetunjuk = StagePetunjukTujuan > 0 ? StagePetunjukTujuan : PlayerPrefs.GetInt("StagePetunjukTujuan", 0);
        bool dariPetunjuk = false;

        if (targetDariPetunjuk > 0)
        {
            targetStage = targetDariPetunjuk;
            dariPetunjuk = true;

            // Reset target setelah diambil agar kunjungan berikutnya normal
            StagePetunjukTujuan = 0;
            PlayerPrefs.DeleteKey("StagePetunjukTujuan");
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log(
                "[PLAYER MARKER] Tidak ada target dari Menu Petunjuk"
            );

            Debug.Log(
                "[PLAYER MARKER] Menggunakan progress checkpoint normal"
            );

            if (CheckpointManager.Instance == null)
            {
                Debug.LogWarning(
                    "[PLAYER MARKER] CheckpointManager belum ditemukan!"
                );

                return;
            }

            // ==========================================
            // CARI STAGE TERAKHIR YANG SELESAI
            // ==========================================

            targetStage = 1;

            for (int stage = 1; stage <= 6; stage++)
            {
                bool selesai =
                    CheckpointManager.Instance.IsStageSelesai(stage);

                Debug.Log(
                    $"[PLAYER MARKER] Stage {stage} selesai = {selesai}"
                );

                if (selesai)
                {
                    targetStage = stage + 1;
                }
            }
        }

        // Jangan melebihi Stage 6 dan minimal Stage 1
        stageSekarang = Mathf.Clamp(targetStage, 1, 6);

        // ==========================================
        // AMBIL POSISI ICON STAGE
        // ==========================================

        int index = stageSekarang - 1;

        if (posisiStage[index] == null)
        {
            Debug.LogWarning(
                $"[PLAYER MARKER] Icon Stage {stageSekarang} belum diisi!"
            );

            return;
        }

        targetPosisi =
            posisiStage[index].position;

        // Pastikan Z sama dengan marker
        targetPosisi.z = transform.position.z;

        Debug.Log(
            $"[PLAYER MARKER] PLAYER BERADA DI STAGE {stageSekarang}"
        );

        Debug.Log(
            $"[PLAYER MARKER] TARGET = {posisiStage[index].name}"
        );

        Debug.Log(
            $"[PLAYER MARKER] TARGET POSITION = {targetPosisi}"
        );

        if (dariPetunjuk)
        {
            if (gerakDanHighlightCoroutine != null)
                StopCoroutine(gerakDanHighlightCoroutine);

            gerakDanHighlightCoroutine = StartCoroutine(ProsesGerakDanHighlight(stageSekarang));
        }
        else
        {
            Debug.Log($"[PLAYER MARKER] Menuju Stage {stageSekarang}");
        }
    }

    private System.Collections.IEnumerator ProsesGerakDanHighlight(int targetStage)
    {
        Debug.Log($"[PLAYER MARKER] Menuju Stage {targetStage}");

        // Tunggu hingga marker sampai di posisi stage target
        float batasWaktu = 5f;
        while (Vector3.Distance(transform.position, targetPosisi) > 0.05f && batasWaktu > 0f)
        {
            batasWaktu -= Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosisi;

        // Setelah marker tiba di stage target, picu kedip icon StagePoint target
        TriggerStageBerkedip(targetStage);

        gerakDanHighlightCoroutine = null;
    }

    public void TriggerStageBerkedip(int stage)
    {
        StagePoint[] allStages = Object.FindObjectsByType<StagePoint>(FindObjectsSortMode.None);
        foreach (var sp in allStages)
        {
            if (sp != null)
            {
                if (sp.stageNumber == stage)
                {
                    sp.MulaiBerkedip();
                }
                else
                {
                    sp.HentikanBerkedip();
                }
            }
        }
    }
}