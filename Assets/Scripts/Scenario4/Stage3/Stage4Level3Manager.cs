using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Stage4Level3Manager : MonoBehaviour
{
    public static Stage4Level3Manager Instance;

    public enum GameState { DialogAwal, Gameplay, CutsceneAkhir, EvaluasiAkhir, GameOver }

    [Header("--- STATE PERMAINAN ---")]
    public GameState currentState;

    [Header("--- CUTSCENE JALAN (DI AKHIR) ---")]
    public GameObject playerKarakter;
    public Transform titikBerhentiPlayer;
    public float kecepatanJalan = 3f;

    [Header("--- UI PANEL DIALOG (AWAL) ---")]
    public GameObject panelDialog;
    public TextMeshProUGUI teksDialog;
    public Button tombolLanjutDialog;

    [TextArea(3, 5)]
    public string[] dialogTutorial = {
        "Ksatria, selamat datang di ujung Goa Faktor! Pintu keluar menuju tahap selanjutnya terhalang oleh dinding batu raksasa bersitus magis.",
        "Ini adalah evaluasi akhirmu. Dinding ini membutuhkan 5 hantaman energi berturut-turut untuk bisa hancur.",
        "Ketikkan semua angka faktor dari target soal yang muncul secara lengkap (pisahkan dengan koma) untuk memecahkan dinding ini selapis demi selapis!"
    };
    private int indeksDialog = 0;

    [Header("Pengaturan Evaluasi Akhir")]
    public int targetSoalBerhasil = 5;
    public int hadiahKoinLevelSelesai = 50;
    private int jumlahBenar = 0;

    [Header("Referensi Bank Soal")]
    public BankSoalKetikLevel3 bankSoal;
    private SoalKetikLevel3 soalAktif;

    [Header("UI Panel Tantangan (Kuis Ketik)")]
    public GameObject panelTantanganInput;
    public TMP_Text textInstruksi;
    public TMP_InputField inputFieldJawaban;
    public TMP_Text textFeedback;
    public Button buttonKonfirmasi;

    [Header("Petunjuk Arrow / Panah + Tulisan 3D")]
    public GameObject arrowPetunjuk;
    public GameObject tulisan3D;          // <-- TAMBAHAN: drag objek TextMeshPro 3D ke sini

    [Header("UI Lain")]
    public GameObject panelHeader;

    [Header("HUD Koin")]
    public TextMeshProUGUI teksJumlahKoin;

    [Header("Sistem Nyawa")]
    public int nyawaPemain = 3;
    public Image[] slotGambarHati;
    public Sprite spriteHatiRetak;
    public GameObject panelGameOver;

    [Header("--- UI PANEL EVALUASI (AKHIR) ---")]
    public GameObject panelEvaluasi;
    public TextMeshProUGUI teksRingkasanMateri;
    public TextMeshProUGUI teksTotalKoin;
    public Button tombolLanjutKeStage5;
    public string namaSceneStage5 = "Stage5_Level1";

    [Header("Referensi Objek Dinding")]
    public DindingAkhir32 dindingAkhir;

    // Proteksi
    private bool sedangProsesDamage = false;
    private bool sedangGantiSoal = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        targetSoalBerhasil = 5;
        jumlahBenar = 0;
        sedangGantiSoal = false;

        // Matikan semua panel di awal
        if (panelTantanganInput) panelTantanganInput.SetActive(false);
        if (panelGameOver) panelGameOver.SetActive(false);
        if (panelEvaluasi) panelEvaluasi.SetActive(false);
        if (panelDialog) panelDialog.SetActive(false);
        if (arrowPetunjuk) arrowPetunjuk.SetActive(false);
        if (tulisan3D) tulisan3D.SetActive(false);          // <-- TAMBAHAN

        UpdateTeksKoin();

        // Setup tombol
        if (buttonKonfirmasi != null)
        {
            buttonKonfirmasi.onClick.RemoveAllListeners();
            buttonKonfirmasi.onClick.AddListener(ValidasiJawabanPemain);
        }

        if (tombolLanjutDialog != null)
        {
            tombolLanjutDialog.onClick.RemoveAllListeners();
            tombolLanjutDialog.onClick.AddListener(LanjutDialog);
        }

        if (tombolLanjutKeStage5 != null)
        {
            tombolLanjutKeStage5.onClick.RemoveAllListeners();
            tombolLanjutKeStage5.onClick.AddListener(PindahKeStage5);
        }

        MulaiDialogAwal();
    }

    // ==========================================
    // DIALOG AWAL
    // ==========================================
    private void MulaiDialogAwal()
    {
        currentState = GameState.DialogAwal;
        indeksDialog = 0;

        if (panelDialog != null) panelDialog.SetActive(true);
        TampilkanTeksDialogSaatIni();
    }

    private void TampilkanTeksDialogSaatIni()
    {
        if (teksDialog != null && dialogTutorial != null && dialogTutorial.Length > 0)
        {
            teksDialog.text = dialogTutorial[indeksDialog];
        }
    }

    private void LanjutDialog()
    {
        indeksDialog++;

        if (indeksDialog < dialogTutorial.Length)
        {
            TampilkanTeksDialogSaatIni();
        }
        else
        {
            if (panelDialog != null) panelDialog.SetActive(false);
            MulaiGameplay();
        }
    }

    // ==========================================
    // GAMEPLAY
    // ==========================================
    private void MulaiGameplay()
    {
        currentState = GameState.Gameplay;
        Debug.Log("<color=green>Gameplay Dimulai!</color>");

        // Tampilkan petunjuk (arrow + tulisan 3D) saat mulai gameplay
        if (arrowPetunjuk != null)
            arrowPetunjuk.SetActive(true);
        if (tulisan3D != null)
            tulisan3D.SetActive(true);                       // <-- TAMBAHAN

        if (playerKarakter != null)
        {
            Player scriptPlayer = playerKarakter.GetComponent<Player>();
            if (scriptPlayer != null) scriptPlayer.canMove = true;
        }
    }

    public void BukaPanelKuis()
    {
        if (currentState != GameState.Gameplay) return;

        // Sembunyikan petunjuk saat masuk area trigger & buka panel kuis
        if (arrowPetunjuk != null)
            arrowPetunjuk.SetActive(false);
        if (tulisan3D != null)
            tulisan3D.SetActive(false);                      // <-- TAMBAHAN

        if (panelTantanganInput)
            panelTantanganInput.SetActive(true);

        if (soalAktif == null && bankSoal != null)
        {
            soalAktif = bankSoal.AmbilSoalBerikutnya();
            Debug.Log("<color=cyan>[SOAL]</color> Soal pertama diambil");
        }

        ResetAreaInput();
        TampilkanSoalKeUI();
    }

    private void ResetAreaInput()
    {
        if (inputFieldJawaban != null)
        {
            inputFieldJawaban.interactable = true;
            inputFieldJawaban.text = "";
            inputFieldJawaban.Select();
            inputFieldJawaban.ActivateInputField();
        }
        if (buttonKonfirmasi != null) buttonKonfirmasi.interactable = true;
        if (textFeedback != null) textFeedback.text = "";
    }

    private void TampilkanSoalKeUI()
    {
        if (soalAktif != null && textInstruksi != null)
        {
            textInstruksi.text = $"[Energi {jumlahBenar}/{targetSoalBerhasil}]\n{soalAktif.narasiSoal}";
        }
    }

    public void ValidasiJawabanPemain()
    {
        if (inputFieldJawaban == null || soalAktif == null) return;
        if (sedangGantiSoal) return;

        string[] inputPisah = inputFieldJawaban.text.Split(',');
        List<int> daftarAngkaInput = new List<int>();

        foreach (string s in inputPisah)
        {
            if (int.TryParse(s.Trim(), out int angka))
            {
                if (!daftarAngkaInput.Contains(angka))
                    daftarAngkaInput.Add(angka);
            }
        }

        daftarAngkaInput.Sort();
        string hasilPemain = string.Join(",", daftarAngkaInput);

        if (hasilPemain == soalAktif.kunciJawaban)
        {
            // ========== JAWABAN BENAR ==========
            sedangGantiSoal = true;

            UpdatePencapaian.CatatJawaban(true);
            jumlahBenar++;

            // Tambah koin setiap jawaban benar ke sumber data terpusat
            GameStatisticsManager.TambahKoin(10, 4, 3, 1);
            int koinSekarang = GameStatisticsManager.GetTotalKoin();

            UpdateTeksKoin();

            Debug.Log($"<color=green>[BENAR]</color> Jumlah Benar: {jumlahBenar}/{targetSoalBerhasil} | Total Koin: {koinSekarang}");

            if (dindingAkhir != null)
                dindingAkhir.ProsesKerusakanBertahap(jumlahBenar);

            if (textFeedback != null)
                textFeedback.text = "<color=green>Tepat! (+10 Koin)</color>\n" + soalAktif.penjelasanBenar;

            if (buttonKonfirmasi != null)
                buttonKonfirmasi.interactable = false;

            if (jumlahBenar >= targetSoalBerhasil)
            {
                // Hadiah tambahan level selesai
                GameStatisticsManager.BerikanRewardLevel(4, 3, 1, hadiahKoinLevelSelesai);
                UpdateTeksKoin();

                StartCoroutine(ProsesMenangLevel());
            }
            else
            {
                StartCoroutine(ProsesSoalBerikutnya());
            }
        }
        else
        {
            // ========== JAWABAN SALAH ==========
            UpdatePencapaian.CatatJawaban(false);

            if (textFeedback != null)
                textFeedback.text = "<color=red>Tenaga tidak cukup!</color>\n" + soalAktif.penjelasanSalah;

            PemainTerkenaDamage();
        }
    }

    private void UpdateTeksKoin()
    {
        if (teksJumlahKoin == null)
        {
            Debug.LogWarning("[KOIN] teksJumlahKoin belum di-assign di Inspector!");
            return;
        }

        int jumlahKoin = GameStatisticsManager.GetTotalKoin();
        teksJumlahKoin.text = jumlahKoin.ToString();
    }

    private IEnumerator ProsesSoalBerikutnya()
    {
        if (inputFieldJawaban != null)
            inputFieldJawaban.interactable = false;

        yield return new WaitForSeconds(1.5f);

        if (bankSoal != null)
        {
            soalAktif = bankSoal.AmbilSoalBerikutnya();
        }

        if (soalAktif == null)
        {
            Debug.LogWarning("Soal habis, langsung selesai.");
            jumlahBenar = targetSoalBerhasil;
            StartCoroutine(ProsesMenangLevel());
            yield break;
        }

        Debug.Log($"<color=cyan>[SOAL]</color> Lanjut ke soal berikutnya | Benar: {jumlahBenar}");

        ResetAreaInput();
        TampilkanSoalKeUI();

        sedangGantiSoal = false;
    }

    // ==========================================
    // CUTSCENE AKHIR
    // ==========================================
    private IEnumerator ProsesMenangLevel()
    {
        currentState = GameState.CutsceneAkhir;
        sedangGantiSoal = true;

        if (buttonKonfirmasi != null) buttonKonfirmasi.interactable = false;
        if (inputFieldJawaban != null) inputFieldJawaban.interactable = false;
        if (arrowPetunjuk != null) arrowPetunjuk.SetActive(false);
        if (tulisan3D != null) tulisan3D.SetActive(false);   // <-- TAMBAHAN

        if (textFeedback != null)
            textFeedback.text = "<color=cyan><b>ENERGI MAKSIMAL TERCAPAI!</b></color>\nDinding mulai hancur...";

        yield return new WaitForSeconds(1.5f);

        if (panelTantanganInput != null)
            panelTantanganInput.SetActive(false);

        if (dindingAkhir != null)
            dindingAkhir.HancurkanDindingRaksasa();

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(CutsceneJalanAkhir());

        TampilkanEvaluasiAkhir();
    }

    private IEnumerator CutsceneJalanAkhir()
    {
        if (playerKarakter != null)
        {
            Player scriptPlayer = playerKarakter.GetComponent<Player>();
            if (scriptPlayer != null) scriptPlayer.canMove = false;
        }

        if (playerKarakter != null && titikBerhentiPlayer != null)
        {
            Vector3 posisiTarget = titikBerhentiPlayer.position;
            posisiTarget.z = playerKarakter.transform.position.z;

            while (Vector2.Distance(
                new Vector2(playerKarakter.transform.position.x, 0),
                new Vector2(posisiTarget.x, 0)) > 0.15f)
            {
                playerKarakter.transform.position = Vector3.MoveTowards(
                    playerKarakter.transform.position,
                    posisiTarget,
                    kecepatanJalan * Time.deltaTime);

                yield return null;
            }
        }

        yield return new WaitForSeconds(1.2f);
    }

    // ==========================================
    // NYAWA (Salah 3x → Menu_Pencapaian)
    // ==========================================
    private void PemainTerkenaDamage()
    {
        if (sedangProsesDamage) return;
        sedangProsesDamage = true;

        nyawaPemain--;
        Debug.Log($"<color=orange>[DAMAGE]</color> Sisa nyawa: {nyawaPemain}");

        if (slotGambarHati != null && nyawaPemain >= 0 && nyawaPemain < slotGambarHati.Length)
        {
            if (spriteHatiRetak != null)
                slotGambarHati[nyawaPemain].sprite = spriteHatiRetak;
            else
                slotGambarHati[nyawaPemain].gameObject.SetActive(false);
        }

        if (nyawaPemain <= 0)
        {
            // ===== SALAH 3X → LANGSUNG KE MENU_PENCAPAIAN =====
            currentState = GameState.GameOver;

            // Sembunyikan semua panel
            if (arrowPetunjuk != null) arrowPetunjuk.SetActive(false);
            if (tulisan3D != null) tulisan3D.SetActive(false); // <-- TAMBAHAN
            if (panelTantanganInput != null) panelTantanganInput.SetActive(false);
            if (panelDialog != null) panelDialog.SetActive(false);
            if (panelHeader != null) panelHeader.SetActive(false);
            if (panelEvaluasi != null) panelEvaluasi.SetActive(false);

            if (panelGameOver != null)
                panelGameOver.SetActive(true);

            PlayerPrefs.Save();
            Invoke(nameof(MuatLayarPencapaian), 3f);
        }
        else
        {
            StartCoroutine(ResetInputSetelahSalah());
        }
    }

    private IEnumerator ResetInputSetelahSalah()
    {
        if (inputFieldJawaban != null)
            inputFieldJawaban.interactable = false;

        yield return new WaitForSeconds(1.6f);

        sedangProsesDamage = false;

        if (nyawaPemain > 0)
        {
            ResetAreaInput();
            TampilkanSoalKeUI();
        }
    }

    // ==========================================
    // EVALUASI AKHIR
    // ==========================================
    private void TampilkanEvaluasiAkhir()
    {
        currentState = GameState.EvaluasiAkhir;

        // ===== SEMBUNYIKAN SEMUA PANEL LAIN =====
        if (panelTantanganInput != null) panelTantanganInput.SetActive(false);
        if (arrowPetunjuk != null) arrowPetunjuk.SetActive(false);
        if (tulisan3D != null) tulisan3D.SetActive(false);   // <-- TAMBAHAN
        if (panelHeader != null) panelHeader.SetActive(false);
        if (panelDialog != null) panelDialog.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);

        // ===== NYALAKAN WIN UI / PANEL EVALUASI =====
        if (panelEvaluasi != null)
            WinManager.TampilkanWin(panelEvaluasi);
        else
            WinManager.TampilkanWin();

        // Update total koin saja (teks ringkasan materi diisi MANUAL di Inspector)
        int totalKoin = GameStatisticsManager.GetTotalKoin();
        if (teksTotalKoin != null)
            teksTotalKoin.text = $"Total Koin Terkumpul: {totalKoin} Koin";

        UpdateTeksKoin();
    }

    // ==========================================
    // PINDAH SCENE
    // ==========================================
    private void PindahKeStage5()
    {
        Time.timeScale = 1f;
        WinManager.PindahKeMapCheckpoint();
    }

    private void MuatLayarPencapaian()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Pencapaian");
    }
}