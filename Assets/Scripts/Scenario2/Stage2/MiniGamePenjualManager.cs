using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MiniGamePenjualManager : MonoBehaviour
{
    public static MiniGamePenjualManager Instance { get; private set; }

    public enum ModeMain { PilihanGanda, KetikInput }

    [Header("--- PENGATURAN LEVEL & MODE ---")]
    [Range(1, 3)] public int levelSaatIni = 1;
    [Tooltip("Karena ini Penjual Buah, pastikan diset ke KetikInput")]
    public ModeMain modeSaatIni = ModeMain.KetikInput;
    public int targetStreak = 10;
    private int streakBenar = 0;

    [Header("--- BANK SOAL ---")]
    public List<SoalPenjualDatabase.SoalCerita> daftarSoal = new List<SoalPenjualDatabase.SoalCerita>();
    private List<int> soalBelumDimainkan = new List<int>();
    private int indeksSoalAktif = 0;

    [Header("--- REFERENSI UI ---")]
    public GameObject panelMiniGame;
    public TextMeshProUGUI teksJudulSoal;
    public Image ilustrasiGambar;
    public TextMeshProUGUI teksCounter;
    public TextMeshProUGUI teksFeedback;

    [Header("--- UI KHUSUS MODE PILIHAN GANDA (Opsional) ---")]
    public GameObject tombolAtasObjek;
    public GameObject tombolBawahObjek;
    public TextMeshProUGUI teksTombolAtas;
    public TextMeshProUGUI teksTombolBawah;

    [Header("--- UI KHUSUS MODE KETIK INPUT ---")]
    public TMP_InputField inputJawaban;
    public GameObject tombolSubmitObjek;

    [Header("--- SISTEM NYAWA & KOIN ---")]
    public int nyawa = 3;
    public GameObject[] ikonHati;
    public int totalKoin = 0;
    public TextMeshProUGUI teksKoin;
    public GameObject panelGameOver;
    public GameObject panelWin;

    private bool sedangMenampilkanPenjelasan = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InisialisasiBankSoal();
    }

    void Start()
    {
        if (panelMiniGame != null) panelMiniGame.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelWin != null) panelWin.SetActive(false);
    }

    public void InisialisasiBankSoal()
    {
        daftarSoal.Clear();
        daftarSoal = SoalPenjualDatabase.MuatSoalBerdasarkanLevel(levelSaatIni);
        IsiUlangKantongSoal();

        Debug.Log("<color=green>Berhasil memuat " + daftarSoal.Count + " Soal Penjual Buah untuk Level " + levelSaatIni + "!</color>");
    }

    void IsiUlangKantongSoal()
    {
        soalBelumDimainkan.Clear();
        for (int i = 0; i < daftarSoal.Count; i++)
        {
            soalBelumDimainkan.Add(i);
        }
    }

    // =========================================================================
    // KONTROL PERMAINAN
    // =========================================================================
    public void MulaiMiniGame()
    {
        if (panelMiniGame != null) panelMiniGame.SetActive(true);
        if (panelWin != null) panelWin.SetActive(false);

        nyawa = 3;
        totalKoin = 0;
        streakBenar = 0;

        InisialisasiBankSoal();
        UpdateUIKoin();

        if (ikonHati != null)
        {
            foreach (GameObject hati in ikonHati)
            {
                if (hati != null) hati.SetActive(true);
            }
        }

        AturTampilanUI();
        AmbilSoalBaru();
    }

    void AturTampilanUI()
    {
        if (modeSaatIni == ModeMain.KetikInput)
        {
            if (tombolAtasObjek != null) tombolAtasObjek.SetActive(false);
            if (tombolBawahObjek != null) tombolBawahObjek.SetActive(false);
            if (inputJawaban != null) inputJawaban.gameObject.SetActive(true);
            if (tombolSubmitObjek != null) tombolSubmitObjek.SetActive(true);

            // Setiap level membutuhkan 10 soal unik
            targetStreak = 10;
        }
    }

    void AmbilSoalBaru()
    {
        if (daftarSoal == null || daftarSoal.Count == 0) return;

        sedangMenampilkanPenjelasan = false;

        if (soalBelumDimainkan.Count == 0)
        {
            IsiUlangKantongSoal();
        }

        int indeksDalamKantong = Random.Range(0, soalBelumDimainkan.Count);
        indeksSoalAktif = soalBelumDimainkan[indeksDalamKantong];
        soalBelumDimainkan.RemoveAt(indeksDalamKantong);

        if (inputJawaban != null)
        {
            inputJawaban.text = "";
            inputJawaban.ActivateInputField();
        }

        TampilkanSoalKeUI();
    }

    void TampilkanSoalKeUI()
    {
        SoalPenjualDatabase.SoalCerita soal = daftarSoal[indeksSoalAktif];

        if (teksCounter != null)
            teksCounter.text = "Streak: <color=yellow>" + streakBenar + "</color> / " + targetStreak;

        if (teksJudulSoal != null)
        {
            teksJudulSoal.text = soal.narasiSoal + "\n<b><color=red>Hitung: " + soal.notasiAngka + "</color></b>";
        }

        if (ilustrasiGambar != null) ilustrasiGambar.enabled = false;

        if (teksFeedback != null)
        {
            teksFeedback.text = "Ketik jawabanmu (gunakan tanda '-' jika hasil minus), lalu tekan JAWAB:";
            teksFeedback.color = Color.white;
        }
    }

    public void CekJawabanInput()
    {
        if (sedangMenampilkanPenjelasan || inputJawaban == null) return;

        string ketikanPemain = inputJawaban.text.Trim();
        SoalPenjualDatabase.SoalCerita soal = daftarSoal[indeksSoalAktif];

        if (int.TryParse(ketikanPemain, out int hasilKetik))
        {
            string qId = $"S2-L2-S{levelSaatIni}-Q{streakBenar + 1:D2}";
            Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {streakBenar + 1}/{targetStreak}");

            if (hasilKetik == soal.jawabanBenar)
            {
                Debug.Log($"[ANSWER]\nCorrect");
                GameStatisticsManager.CatatJawabanBenar(qId);

                // --- BENAR ---
                sedangMenampilkanPenjelasan = true;
                streakBenar++;
                int koinDidapat = levelSaatIni * 5;
                totalKoin += koinDidapat;

                GameStatisticsManager.TambahKoin(koinDidapat, 2, 2, levelSaatIni);

                UpdateUIKoin();

                if (teksFeedback != null)
                {
                    teksFeedback.text = "<color=green>BENAR SEKALI!</color> Hasilnya adalah " + soal.jawabanBenar;
                }

                if (streakBenar >= targetStreak)
                {
                    Invoke("SelesaikanMiniGame", 1.5f);
                }
                else
                {
                    Invoke("AmbilSoalBaru", 1.5f);
                }
            }
            else
            {
                ProsesJawabanSalah(soal);
            }
        }
        else
        {
            if (teksFeedback != null)
            {
                teksFeedback.text = "<color=orange>PERHATIAN!</color> Masukkan angka bulat saja (Contoh: 15 atau -8).";
            }
        }
    }

    void ProsesJawabanSalah(SoalPenjualDatabase.SoalCerita soal)
    {
        string qId = $"S2-L2-S{levelSaatIni}-Q{streakBenar + 1:D2}";
        Debug.Log($"[ANSWER]\nWrong");
        GameStatisticsManager.CatatJawabanSalah(qId);

        nyawa--;
        // streak TIDAK di-reset
        sedangMenampilkanPenjelasan = true;

        if (ikonHati != null && nyawa >= 0 && nyawa < ikonHati.Length)
        {
            if (ikonHati[nyawa] != null) ikonHati[nyawa].SetActive(false);
        }

        if (nyawa <= 0)
        {
            // Sudah 3x salah → pindah ke Menu_Pencapaian
            if (teksFeedback != null)
                teksFeedback.text = "<color=red>NYAWA HABIS!</color> Kamu gagal membantu Penjual Buah!";
            Invoke("KalahGame", 2f);
        }
        else
        {
            if (teksFeedback != null)
            {
                teksFeedback.text = "<color=red>SALAH!</color> " + soal.hintJikaSalah +
                                    "\n<i><color=yellow>Streak tetap " + streakBenar + ". Soal baru akan muncul...</color></i>";
            }
            Invoke("AmbilSoalBaru", 3.5f);
        }
    }

    void UpdateUIKoin()
    {
        if (teksKoin != null) teksKoin.text = totalKoin.ToString();
    }

    // Berhasil mencapai target streak → tampilkan Win UI
    void SelesaikanMiniGame()
    {
        GameStatisticsManager.BerikanRewardLevel(2, 2, levelSaatIni, 25);
        if (panelMiniGame != null) panelMiniGame.SetActive(false);
        WinManager.TampilkanWin(panelWin);
    }

    // Nyawa habis (3x salah) → pindah ke Menu_Pencapaian
    void KalahGame()
    {
        if (panelMiniGame != null) panelMiniGame.SetActive(false);

        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Pencapaian");
    }
}