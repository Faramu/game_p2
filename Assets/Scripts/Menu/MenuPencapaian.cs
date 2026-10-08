using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuPencapaian : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    [Tooltip("Nama scene Map Checkpoint")]
    public string namaSceneMap = "Map_Checkpoint";

    [Header("Referensi UI Teks Utama")]
    public TextMeshProUGUI teksKoin;
    public TextMeshProUGUI teksWaktu;
    public TextMeshProUGUI teksStage;

    [Header("Referensi UI Teks Statistik")]
    public TextMeshProUGUI teksStatBenar;
    public TextMeshProUGUI teksStatSalah;
    public TextMeshProUGUI teksStatBos;

    void Start()
    {
        // Pastikan waktu game berjalan normal (jika sebelumnya Game Over/Pause)
        Time.timeScale = 1f;
        MuatDataPencapaian();
    }

    public void MuatDataPencapaian()
    {
        int totalKoin = GameStatisticsManager.GetTotalKoin();
        if (teksKoin != null)
            teksKoin.text = $"Total Koin Terkumpul: \n{totalKoin}";

        float waktuBermain = GameStatisticsManager.GetWaktuBermain();
        string waktuFormat = FormatWaktu(waktuBermain);
        if (teksWaktu != null)
            teksWaktu.text = $"Total Waktu Bermain: \n{waktuFormat}";

        // Ambil stage selesai dari statistik (sinkron dengan CheckpointManager)
        int stageSelesai = GameStatisticsManager.GetStageSelesai();
        if (stageSelesai == 0)
        {
            stageSelesai = CheckpointManager.HitungTotalStageSelesaiStatic();
            if (stageSelesai > 0)
            {
                GameStatisticsManager.CatatStageSelesai(stageSelesai);
            }
        }
        if (teksStage != null)
            teksStage.text = $"Stage Diselesaikan: \n{stageSelesai} Stage";

        int soalBenar = GameStatisticsManager.GetStatSoalBenar();
        int soalSalah = GameStatisticsManager.GetStatSoalSalah();
        int bosKalah = GameStatisticsManager.GetStatBosKalah();
        float akurasi = GameStatisticsManager.HitungAkurasi();

        if (teksStatBenar != null)
            teksStatBenar.text = $"Tebakan Tepat (Hit): {soalBenar} (<color=green>Akurasi {akurasi:F1}%</color>)";
        if (teksStatSalah != null)
            teksStatSalah.text = $"Tebakan Meleset (Miss): {soalSalah}";
        if (teksStatBos != null)
            teksStatBos.text = $"Bos Terkalahkan: {bosKalah} Naga";

        // Log [ACHIEVEMENT] sesuai format audit
        Debug.Log($"[ACHIEVEMENT]\nCoin = {totalKoin}\nTime = {waktuFormat} ({waktuBermain:F1}s)\nStage = {stageSelesai}\nCorrect = {soalBenar}\nWrong = {soalSalah}\nBoss = {bosKalah}\nAccuracy = {akurasi:F1}%");
    }

    private string FormatWaktu(float totalDetik)
    {
        int jam = Mathf.FloorToInt(totalDetik / 3600f);
        int menit = Mathf.FloorToInt((totalDetik % 3600f) / 60f);
        int detik = Mathf.FloorToInt(totalDetik % 60f);

        if (jam > 0)
            return $"{jam}j {menit}m {detik}d";
        else
            return $"{menit}m {detik}d";
    }

    public void TombolKembali()
    {
        SceneManager.LoadScene(namaSceneMap);
    }
}