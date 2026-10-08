using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCheckpointManager : MonoBehaviour
{
    [Header("Scene Tujuan Tombol Kembali")]
    public string namaSceneMap = "MainMenuScene";

    [Header("Daftar Stage")]
    public StagePoint[] semuaStage;

    private void Start()
    {
        PlayerMarker.LogTargetMapJikaAda();
        RefreshMap();
    }

    public void RefreshMap()
    {
        Debug.Log("[MAP] Refresh progress");

        // Jika array di Inspector kosong atau berisi elemen null, cari otomatis di scene
        if (semuaStage == null || semuaStage.Length == 0 || System.Array.Exists(semuaStage, s => s == null))
        {
            StagePoint[] foundStages = Object.FindObjectsByType<StagePoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            System.Array.Sort(foundStages, (a, b) => a.stageNumber.CompareTo(b.stageNumber));
            semuaStage = foundStages;
        }

        foreach (StagePoint stage in semuaStage)
        {
            if (stage != null)
            {
                stage.SetupStage();
            }
        }

        // Tampilkan debug log status setiap stage sesuai format spesifikasi (Section L)
        if (CheckpointManager.Instance != null)
        {
            for (int s = 1; s <= CheckpointManager.TOTAL_STAGE; s++)
            {
                bool l1 = CheckpointManager.Instance.IsLevelSelesai(s, 1);
                bool l2 = CheckpointManager.Instance.IsLevelSelesai(s, 2);
                bool l3 = CheckpointManager.Instance.IsLevelSelesai(s, 3);
                int stars = CheckpointManager.Instance.HitungBintangStage(s);

                Debug.Log($"[MAP]\nStage {s}:\nLevel 1 = {l1.ToString().ToLower()}\nLevel 2 = {l2.ToString().ToLower()}\nLevel 3 = {l3.ToString().ToLower()}\nStars = {stars}");
            }
        }

        Debug.Log("[MAP] Bintang diperbarui");
    }

    public void TombolKembali()
    {
        SceneManager.LoadScene(namaSceneMap);
    }

    [ContextMenu("Reset Progress Checkpoint")]
    public void ResetProgress()
    {
        CheckpointManager.ResetProgressKhususCheckpoint();
        RefreshMap();
    }
}