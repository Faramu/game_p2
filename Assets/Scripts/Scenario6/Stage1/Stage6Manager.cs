using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Stage6Manager : MonoBehaviour
{
    public static Stage6Manager Instance;

    public enum GameState
    {
        Dialog,
        MemilihKunci,
        Transisi,
        Selesai
    }

    [Header("--- STATE ---")]
    public GameState currentState = GameState.Dialog;

    // =========================================================
    // PENGATURAN PER SCENE
    // =========================================================
    [Header("--- PENGATURAN SCENE INI ---")]
    [Tooltip("Sub-Level yang dimainkan di scene ini (1, 2, atau 3)")]
    [Range(1, 3)]
    public int subLevelSaatIni = 1;

    [Tooltip("Berapa soal yang ingin dimainkan di scene ini")]
    public int jumlahSoalDiSceneIni = 3;

    [Tooltip("Setelah selesai, otomatis pindah ke scene berikutnya?")]
    public bool otomatisPindahScene = false;

    [Tooltip("Setelah selesai, lanjut ke sub-level berikutnya di scene yang sama?")]
    public bool lanjutKeSubLevelBerikutnya = false;

    // =========================================================
    // UI DIALOG
    // =========================================================
    [Header("--- REFERENSI UI DIALOG ---")]
    public GameObject panelDialog;
    public TextMeshProUGUI teksDialog;
    public Button tombolLanjutDialog;
    public GameObject panelFeedback;
    public TextMeshProUGUI teksFeedback;
    public Button tombolLanjutFeedback;          // ← TOMBOL BARU (wajib diisi)

    // =========================================================
    // KUNCI
    // =========================================================
    [Header("--- REFERENSI KUNCI ---")]
    public KunciKPK[] daftarKunci;

    // =========================================================
    // GEMBOK & GERBANG
    // =========================================================
    [Header("--- VISUAL GEMBOK & GERBANG ---")]
    [Tooltip("Gembok versi TERTUTUP (2 buah)")]
    public GameObject[] daftarGembok;

    [Tooltip("Gembok versi TERBUKA (2 buah). Boleh dikosongkan jika tidak ada")]
    public GameObject[] daftarGembokTerbuka;

    [Tooltip("TextMeshPro 3D di dalam gembok tertutup")]
    public TextMeshPro[] teksAngkaGembok;

    public GameObject gerbangTertutup;
    public GameObject gerbangTerbuka;
    public GameObject objekMeriamTaktis;

    // =========================================================
    // NYAWA
    // =========================================================
    [Header("--- SISTEM NYAWA ---")]
    public int nyawaMaksimal = 3;
    private int nyawaSaatIni;
    public Image[] ikonHatiUI;
    public Sprite spriteHatiUtuh;
    public Sprite spriteHatiRetak;
    public GameObject panelGameOver;
    public GameObject panelFlashMerah;

    // =========================================================
    // KOIN
    // =========================================================
    [Header("--- SISTEM KOIN ---")]
    public int koinSaatIni = 0;
    public int koinPerJawabanBenar = 20;
    public TextMeshProUGUI teksKoinUI;

    // =========================================================
    // PLAYER
    // =========================================================
    [Header("--- PLAYER ---")]
    public MonoBehaviour scriptPergerakanPlayer;
    public Transform posisiAwalPlayer;

    // =========================================================
    // PINDAH SCENE
    // =========================================================
    [Header("--- PINDAH SCENE ---")]
    public string namaSceneBerikutnya;
    public GameObject objekPortalKeluar;
    public string namaSceneGameOver = "Menu_Pencapaian";   // ← scene saat nyawa habis

    // =========================================================
    // WAKTU
    // =========================================================
    [Header("--- PENGATURAN WAKTU ---")]
    public float delaySetelahBenar = 1.2f;
    public float delaySetelahSelesai = 1.5f;

    // =========================================================
    // DATA INTERNAL
    // =========================================================
    private string[] dialogAktif;
    private int dialogIndex = 0;

    private List<DataSoalKPK> bankSoal = new List<DataSoalKPK>();
    private DataSoalKPK soalAktif;
    private int soalTerjawab = 0;
    private int totalSoal = 0;

    private bool sedangMemproses = false;

    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        nyawaSaatIni = nyawaMaksimal;
        koinSaatIni = GameStatisticsManager.GetTotalKoin();
        UpdateUIHati();
        UpdateUIKoin();

        if (panelDialog != null) panelDialog.SetActive(false);
        if (panelFeedback != null) panelFeedback.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);
        if (objekPortalKeluar != null) objekPortalKeluar.SetActive(false);
        if (objekMeriamTaktis != null) objekMeriamTaktis.SetActive(false);
        if (gerbangTerbuka != null) gerbangTerbuka.SetActive(false);
        if (gerbangTertutup != null) gerbangTertutup.SetActive(true);

        MatikanSemuaKunci();
        SetGembokTertutup();

        // Setup tombol dialog
        if (tombolLanjutDialog != null)
        {
            tombolLanjutDialog.onClick.RemoveAllListeners();
            tombolLanjutDialog.onClick.AddListener(LanjutDialog);
        }

        // Setup tombol lanjut setelah feedback
        if (tombolLanjutFeedback != null)
        {
            tombolLanjutFeedback.onClick.RemoveAllListeners();
            tombolLanjutFeedback.onClick.AddListener(LanjutSetelahFeedback);
            tombolLanjutFeedback.gameObject.SetActive(false);
        }

        IntroVideoManager introMgr = FindObjectOfType<IntroVideoManager>();
        if (introMgr != null && introMgr.gameObject.activeInHierarchy)
        {
            return;
        }
        LanjutkanSetelahVideo();
    }

    public void LanjutkanSetelahVideo()
    {
        MulaiLevel();
    }

    // =========================================================
    // MULAI LEVEL
    // =========================================================
    public void MulaiLevel()
    {
        soalTerjawab = 0;
        sedangMemproses = false;

        dialogAktif = Stage6Database.DapatkanDialog(subLevelSaatIni);

        List<DataSoalKPK> semuaSoal = Stage6Database.DapatkanSoal(subLevelSaatIni);
        AcakList(semuaSoal);

        bankSoal.Clear();
        int jumlahAmbil = Mathf.Min(jumlahSoalDiSceneIni, semuaSoal.Count);

        for (int i = 0; i < jumlahAmbil; i++)
        {
            bankSoal.Add(semuaSoal[i]);
        }

        totalSoal = bankSoal.Count;

        Debug.Log($"<color=cyan>Sub-Level {subLevelSaatIni} | Soal acak: {totalSoal}</color>");

        SetGembokTertutup();

        dialogIndex = 0;
        currentState = GameState.Dialog;

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = false;

        if (panelDialog != null) panelDialog.SetActive(true);
        TampilkanTeksDialog();
    }

    private void AcakList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // =========================================================
    // SISTEM DIALOG
    // =========================================================
    private void TampilkanTeksDialog()
    {
        if (teksDialog != null && dialogAktif != null && dialogIndex < dialogAktif.Length)
        {
            teksDialog.text = dialogAktif[dialogIndex];
        }
    }

    public void LanjutDialog()
    {
        if (currentState != GameState.Dialog) return;

        dialogIndex++;

        if (dialogIndex < dialogAktif.Length)
        {
            TampilkanTeksDialog();
        }
        else
        {
            if (panelDialog != null) panelDialog.SetActive(false);
            MulaiSoalPertama();
        }
    }

    // =========================================================
    // SISTEM SOAL & KUNCI
    // =========================================================
    private void MulaiSoalPertama()
    {
        if (bankSoal.Count == 0)
        {
            Debug.LogWarning("Tidak ada soal!");
            SelesaiLevel();
            return;
        }

        currentState = GameState.MemilihKunci;
        TampilkanSoalBerikutnya();
    }

    private void TampilkanSoalBerikutnya()
    {
        if (bankSoal.Count == 0 || soalTerjawab >= totalSoal)
        {
            SelesaiLevel();
            return;
        }

        soalAktif = bankSoal[0];
        bankSoal.RemoveAt(0);

        SetupKunciDanGembok(soalAktif);

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = true;

        currentState = GameState.MemilihKunci;
        sedangMemproses = false;
    }

    private void SetupKunciDanGembok(DataSoalKPK soal)
    {
        // Tampilkan angka di gembok
        if (teksAngkaGembok != null && teksAngkaGembok.Length >= 2)
        {
            if (teksAngkaGembok[0] != null)
                teksAngkaGembok[0].text = soal.angkaPertama.ToString();

            if (teksAngkaGembok[1] != null)
                teksAngkaGembok[1].text = soal.angkaKedua.ToString();
        }

        // Setiap soal baru → gembok kembali TERTUTUP
        SetGembokTertutup();

        // Setup 3 kunci
        if (daftarKunci == null || daftarKunci.Length < 3)
        {
            Debug.LogError("daftarKunci harus berisi minimal 3 objek KunciKPK!");
            return;
        }

        string[] pilihan = { soal.pilihan1, soal.pilihan2, soal.pilihan3 };
        bool[] benar = new bool[3];
        benar[soal.indeksJawabanBenar] = true;

        for (int i = 0; i < 3; i++)
        {
            if (daftarKunci[i] != null)
            {
                daftarKunci[i].SetupKunci(pilihan[i], benar[i]);
            }
        }
    }

    // =========================================================
    // LOGIKA GEMBOK
    // =========================================================
    private void SetGembokTertutup()
    {
        if (daftarGembok != null)
        {
            foreach (var g in daftarGembok)
            {
                if (g != null) g.SetActive(true);
            }
        }

        if (daftarGembokTerbuka != null)
        {
            foreach (var g in daftarGembokTerbuka)
            {
                if (g != null) g.SetActive(false);
            }
        }
    }

    private void SetGembokTerbuka()
    {
        if (daftarGembok != null)
        {
            foreach (var g in daftarGembok)
            {
                if (g != null) g.SetActive(false);
            }
        }

        if (daftarGembokTerbuka != null)
        {
            foreach (var g in daftarGembokTerbuka)
            {
                if (g != null) g.SetActive(true);
            }
        }
    }

    private void MatikanSemuaKunci()
    {
        if (daftarKunci == null) return;
        foreach (var kunci in daftarKunci)
        {
            if (kunci != null)
                kunci.gameObject.SetActive(false);
        }
    }

    // =========================================================
    // DIPANGGIL DARI KunciKPK
    // =========================================================
    public void JawabanBenar()
    {
        if (sedangMemproses || currentState != GameState.MemilihKunci) return;
        sedangMemproses = true;

        koinSaatIni += koinPerJawabanBenar;
        GameStatisticsManager.TambahKoin(koinPerJawabanBenar);
        GameStatisticsManager.CatatJawabanBenar(soalAktif != null ? $"S6L1-{soalAktif.instruksiTeks}" : "S6L1");
        UpdateUIKoin();

        TampilkanFeedback(soalAktif.feedbackBenar, true);

        soalTerjawab++;

        StartCoroutine(ProsesSetelahBenar());
    }

    public void PemainTerkenaDamage()
    {
        if (sedangMemproses) return;

        GameStatisticsManager.CatatJawabanSalah(soalAktif != null ? $"S6L1-{soalAktif.instruksiTeks}" : "S6L1");

        nyawaSaatIni--;
        UpdateUIHati();
        StartCoroutine(EfekFlashMerah());

        if (soalAktif != null)
            TampilkanFeedback(soalAktif.feedbackSalah, false);

        if (nyawaSaatIni <= 0)
        {
            ProsesGameOver();
        }
        else
        {
            sedangMemproses = false;
        }
    }

    private IEnumerator ProsesSetelahBenar()
    {
        currentState = GameState.Transisi;
        MatikanSemuaKunci();

        // Jawaban benar → kedua gembok menjadi TERBUKA
        SetGembokTerbuka();
        Debug.Log("Kedua gembok terbuka!");

        // Tunggu sebentar agar player melihat gembok terbuka + feedback
        yield return new WaitForSeconds(delaySetelahBenar);

        // Tampilkan tombol Lanjut (player harus klik dulu)
        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.gameObject.SetActive(true);
    }

    // =========================================================
    // TOMBOL LANJUT SETELAH FEEDBACK
    // =========================================================
    public void LanjutSetelahFeedback()
    {
        // Sembunyikan feedback & tombol
        if (panelFeedback != null) panelFeedback.SetActive(false);
        if (tombolLanjutFeedback != null) tombolLanjutFeedback.gameObject.SetActive(false);

        if (soalTerjawab >= totalSoal)
        {
            StartCoroutine(SelesaiLevelCoroutine());
        }
        else
        {
            // Soal berikutnya akan membuat gembok kembali tertutup
            TampilkanSoalBerikutnya();
        }
    }

    // =========================================================
    // SELESAI LEVEL
    // =========================================================
    private IEnumerator SelesaiLevelCoroutine()
    {
        currentState = GameState.Transisi;

        Debug.Log($"<color=green>Sub-Level {subLevelSaatIni} Selesai!</color>");

        yield return new WaitForSeconds(delaySetelahSelesai);

        SelesaiLevel();
    }

    private void SelesaiLevel()
    {
        currentState = GameState.Selesai;

        // Hanya di sini gerbang besar berubah
        if (gerbangTertutup != null) gerbangTertutup.SetActive(false);
        if (gerbangTerbuka != null) gerbangTerbuka.SetActive(true);

        if (objekMeriamTaktis != null) objekMeriamTaktis.SetActive(true);
        if (objekPortalKeluar != null) objekPortalKeluar.SetActive(true);

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = true;

        Debug.Log("<color=cyan>=== SEMUA SOAL SELESAI → GERBANG TERBUKA ===</color>");

        GameStatisticsManager.BerikanRewardLevel(6, 1, subLevelSaatIni, 25);
        WinManager.TampilkanWin();
    }

    // =========================================================
    // UI HELPER
    // =========================================================
    private void UpdateUIHati()
    {
        if (ikonHatiUI == null) return;

        for (int i = 0; i < ikonHatiUI.Length; i++)
        {
            if (ikonHatiUI[i] == null) continue;
            ikonHatiUI[i].sprite = (i < nyawaSaatIni) ? spriteHatiUtuh : spriteHatiRetak;
        }
    }

    private void UpdateUIKoin()
    {
        if (teksKoinUI != null)
            teksKoinUI.text = koinSaatIni.ToString();
    }

    private void TampilkanFeedback(string pesan, bool benar)
    {
        if (panelFeedback == null || teksFeedback == null) return;

        teksFeedback.text = pesan;
        teksFeedback.color = benar ? Color.green : Color.red;
        panelFeedback.SetActive(true);
    }

    private IEnumerator EfekFlashMerah()
    {
        if (panelFlashMerah != null)
        {
            panelFlashMerah.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            panelFlashMerah.SetActive(false);
        }
    }

    // =========================================================
    // GAME OVER → Pindah ke Menu_Pencapaian
    // =========================================================
    private void ProsesGameOver()
    {
        currentState = GameState.Selesai;

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = false;

        MatikanSemuaKunci();

        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        // Setelah 2 detik pindah ke Menu_Pencapaian
        Invoke(nameof(PindahKeMenuPencapaian), 2f);
    }

    private void PindahKeMenuPencapaian()
    {
        SceneManager.LoadScene(namaSceneGameOver);
    }

    private void MuatUlangScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PindahKeSceneBerikutnya()
    {
        WinManager.TampilkanWin();
    }
}