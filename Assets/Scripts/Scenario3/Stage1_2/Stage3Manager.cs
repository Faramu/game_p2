using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class DataSoalSuhu
{
    public int idSoal;
    public string jenisOperasi;
    public int jawabanBenar;
    public string teksTombolA;
    public int nilaiA;
    public string teksTombolB;
    public int nilaiB;
    public string narasi;
    public string pertanyaan;
    public string penjelasanBenar;
    public string penjelasanSalah;
}

public class Stage3Manager : MonoBehaviour
{
    public static Stage3Manager Instance { get; private set; }

    public enum PilihanLevel { Level_1, Level_2, Level_3 }

    [Header("Pengaturan Database Soal")]
    [Tooltip("Pilih database soal mana yang akan dimuat di Scene ini")]
    public PilihanLevel levelAktif = PilihanLevel.Level_1;

    [Header("Pengaturan Suhu Utama")]
    [SerializeField] private int suhuAwal = -12;              // Mulai dari dingin
    [SerializeField] private int kenaikanSuhuBenar = 5;       // Naik berapa kalau benar
    [SerializeField] private int penurunanSuhuSalah = 4;      // Turun berapa kalau salah
    private int suhuSaatIni;

    [Header("Referensi UI Indikator Suhu")]
    public GameObject panelIndikatorSuhu;
    public TextMeshProUGUI teksIndikatorSuhu;

    [Header("Pengaturan Warna Suhu")]
    [SerializeField] private Color warnaSuhuPositif = new Color(1f, 0.4f, 0f);   // Oranye
    [SerializeField] private Color warnaSuhuNegatif = new Color(0f, 0.75f, 1f);  // Biru
    [SerializeField] private Color warnaSuhuNol = Color.white;

    private Dictionary<int, DataSoalSuhu> databaseSoal = new Dictionary<int, DataSoalSuhu>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        MuatDatabaseSesuaiLevel();
    }

    private void MuatDatabaseSesuaiLevel()
    {
        if (levelAktif == PilihanLevel.Level_1) BankSoal_Stage3_Lv1.MuatSoal(databaseSoal);
        else if (levelAktif == PilihanLevel.Level_2) BankSoal_Stage3_Lv2.MuatSoal(databaseSoal);
        else if (levelAktif == PilihanLevel.Level_3) BankSoal_Stage3_Lv3.MuatSoal(databaseSoal);
    }

    private void Start()
    {
        suhuSaatIni = suhuAwal;
        InisialisasiUI();
    }

    private void InisialisasiUI()
    {
        if (panelIndikatorSuhu != null && teksIndikatorSuhu == null)
            teksIndikatorSuhu = panelIndikatorSuhu.GetComponentInChildren<TextMeshProUGUI>();

        UpdateUISuhu();
    }

    public void AktifkanPanelSuhu()
    {
        if (panelIndikatorSuhu != null)
            panelIndikatorSuhu.SetActive(true);

        UpdateUISuhu();
    }

    // ===== FUNGSI BARU =====
    public void NaikkanSuhu()
    {
        suhuSaatIni += kenaikanSuhuBenar;
        UpdateUISuhu();
    }

    public void TurunkanSuhu()
    {
        suhuSaatIni -= penurunanSuhuSalah;
        UpdateUISuhu();
    }

    // Tetap ada agar tidak error dengan kode lama
    public void UbahSuhu(int angkaOperasi, string jenisOperasi)
    {
        if (jenisOperasi == "Kali")
            suhuSaatIni *= angkaOperasi;
        else if (jenisOperasi == "Bagi" && angkaOperasi != 0)
            suhuSaatIni /= angkaOperasi;

        UpdateUISuhu();
    }

    public void SetSuhuLangsung(int nilaiBaru)
    {
        suhuSaatIni = nilaiBaru;
        UpdateUISuhu();
    }

    public int GetSuhuSaatIni()
    {
        return suhuSaatIni;
    }

    private void UpdateUISuhu()
    {
        if (panelIndikatorSuhu != null && !panelIndikatorSuhu.activeSelf)
            panelIndikatorSuhu.SetActive(true);

        if (teksIndikatorSuhu == null && panelIndikatorSuhu != null)
            teksIndikatorSuhu = panelIndikatorSuhu.GetComponentInChildren<TextMeshProUGUI>();

        if (teksIndikatorSuhu == null) return;

        string tanda = suhuSaatIni > 0 ? "+" : "";
        teksIndikatorSuhu.text = $"Suhu: {tanda}{suhuSaatIni}°C";

        if (suhuSaatIni > 0)
            teksIndikatorSuhu.color = warnaSuhuPositif;
        else if (suhuSaatIni < 0)
            teksIndikatorSuhu.color = warnaSuhuNegatif;
        else
            teksIndikatorSuhu.color = warnaSuhuNol;

        teksIndikatorSuhu.ForceMeshUpdate();
    }

    public DataSoalSuhu AmbilSoalAcak(int subLevelMesin)
    {
        int idMin = 1, idMax = 10;

        if (subLevelMesin == 1) { idMin = 1; idMax = 10; }
        else if (subLevelMesin == 2) { idMin = 11; idMax = 20; }
        else if (subLevelMesin == 3) { idMin = 21; idMax = 30; }
        else if (subLevelMesin == 4) { idMin = 31; idMax = 40; }
        else if (subLevelMesin == 5) { idMin = 41; idMax = 50; }
        else if (subLevelMesin == 6) { idMin = 51; idMax = 60; }
        else if (subLevelMesin == 7) { idMin = 61; idMax = 70; }

        List<int> soalTersedia = new List<int>();
        for (int i = idMin; i <= idMax; i++)
        {
            if (databaseSoal.ContainsKey(i))
                soalTersedia.Add(i);
        }

        if (soalTersedia.Count > 0)
        {
            int randomId = soalTersedia[Random.Range(0, soalTersedia.Count)];
            return databaseSoal[randomId];
        }

        Debug.LogError($"Soal untuk Sub-Level {subLevelMesin} tidak ditemukan!");
        return null;
    }

    public DataSoalSuhu AmbilDataSoal(int idDiminta)
    {
        if (databaseSoal.ContainsKey(idDiminta))
            return databaseSoal[idDiminta];
        return null;
    }
}