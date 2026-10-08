using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class DataSoalFaktor
{
    public string instruksiTeks;
    public int angkaTarget;
    public int[] semuaPilihan;
    public int[] faktorBenar;

    [TextArea(2, 4)] public string feedbackBenar;
    [TextArea(2, 4)] public string feedbackSalah;
}

public class GoaFaktorManager : MonoBehaviour
{
    public static GoaFaktorManager Instance;

    public enum GameState { DialogAwal, Bertempur, Feedback, GameOver, Menang }

    [Header("State Permainan")]
    public GameState currentState = GameState.DialogAwal;

    [Header("Pengaturan Sub Level")]
    [Range(1, 3)]
    public int subLevelSaatIni = 1;

    [Header("UI Panel")]
    public GameObject panelUtama;
    public TextMeshProUGUI teksUtama;
    public GameObject tombolLanjut;
    public Button tombolKonfirmasi;

    [Header("Referensi Dinding")]
    public TMP_Text teksAngkaDindingUtama;
    public DindingGoa dindingGoaUtama;

    [Header("Referensi Batu (Array)")]
    public GameObject grupBatuEnergi;
    public BatuEnergi[] daftarBatu;

    [Header("Sistem Nyawa & Koin")]
    public int nyawaPemain = 3;
    public Image[] slotGambarHati;
    public Sprite spriteHatiRetak;
    public int koinSaatIni = 0;
    public int hadiahKoinPerSoal = 10;
    public TextMeshProUGUI teksKoinUI;

    [Header("UI Game Over")]
    public GameObject panelGameOver;

    [Header("Transisi")]
    public GameObject playerKsatria;
    public Transform titikPintuGoa;
    public float kecepatanJalan = 3f;
    public string namaSceneBerikutnya = "Stage4_Level2";

    private int soalTerjawab = 0;
    private int totalSoalDibutuhkan = 5;
    private bool jawabanTerakhirBenar = false;

    private List<DataSoalFaktor> bankSoal = new List<DataSoalFaktor>();
    private string[] halamanDialog;
    private int indeksHalaman = 0;

    private void Awake()
    {
        Instance = this;
        SiapkanDataDariDatabase();
    }

    private void Start()
    {
        koinSaatIni = GameStatisticsManager.GetTotalKoin();
        UpdateUIKoin();

        if (grupBatuEnergi != null) grupBatuEnergi.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);

        if (tombolKonfirmasi != null)
        {
            tombolKonfirmasi.gameObject.SetActive(false);
            tombolKonfirmasi.onClick.RemoveAllListeners();
            tombolKonfirmasi.onClick.AddListener(KonfirmasiJawaban);
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
        MulaiDialogAwal();
    }

    // =========================================================
    // AMBIL DATA DARI DATABASE
    // =========================================================
    private void SiapkanDataDariDatabase()
    {
        halamanDialog = GoaFaktorDatabase.DapatkanDialog(subLevelSaatIni);
        bankSoal = GoaFaktorDatabase.DapatkanSoal(subLevelSaatIni);
        AcakBankSoal();

        Debug.Log($"[GoaFaktor] SubLevel {subLevelSaatIni} | Dialog: {halamanDialog.Length} | Soal: {bankSoal.Count}");
    }

    private void AcakBankSoal()
    {
        for (int i = 0; i < bankSoal.Count; i++)
        {
            DataSoalFaktor temp = bankSoal[i];
            int r = Random.Range(i, bankSoal.Count);
            bankSoal[i] = bankSoal[r];
            bankSoal[r] = temp;
        }
    }

    private void UpdateUIKoin()
    {
        if (teksKoinUI != null)
            teksKoinUI.text = koinSaatIni.ToString();
    }

    // =========================================================
    // DIALOG
    // =========================================================
    public void MulaiDialogAwal()
    {
        currentState = GameState.DialogAwal;
        indeksHalaman = 0;

        if (panelUtama != null) panelUtama.SetActive(true);
        if (tombolLanjut != null) tombolLanjut.SetActive(true);
        if (tombolKonfirmasi != null) tombolKonfirmasi.gameObject.SetActive(false);

        if (halamanDialog != null && halamanDialog.Length > 0 && teksUtama != null)
            teksUtama.text = halamanDialog[indeksHalaman];
    }

    public void KlikTombolLanjut()
    {
        if (currentState == GameState.DialogAwal)
        {
            indeksHalaman++;
            if (indeksHalaman < halamanDialog.Length)
            {
                if (teksUtama != null)
                    teksUtama.text = halamanDialog[indeksHalaman];
            }
            else
            {
                SelesaiDialogDanMulaiGame();
            }
        }
        else if (currentState == GameState.Feedback)
        {
            TutupFeedbackDanLanjutkan();
        }
        else if (currentState == GameState.Menang)
        {
            MulaiJalanKePintu();
        }
    }

    private void SelesaiDialogDanMulaiGame()
    {
        currentState = GameState.Bertempur;

        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (grupBatuEnergi != null) grupBatuEnergi.SetActive(true);
        if (tombolKonfirmasi != null) tombolKonfirmasi.gameObject.SetActive(true);

        TampilkanSoalSaatIni();
    }

    // =========================================================
    // SOAL
    // =========================================================
    public void TampilkanSoalSaatIni()
    {
        if (dindingGoaUtama != null)
            dindingGoaUtama.HentikanEfekDebu();

        if (soalTerjawab < totalSoalDibutuhkan && soalTerjawab < bankSoal.Count)
        {
            currentState = GameState.Bertempur;

            if (panelUtama != null) panelUtama.SetActive(true);
            if (grupBatuEnergi != null) grupBatuEnergi.SetActive(true);
            if (tombolKonfirmasi != null) tombolKonfirmasi.gameObject.SetActive(true);
            if (tombolLanjut != null) tombolLanjut.SetActive(false);

            DataSoalFaktor soal = bankSoal[soalTerjawab];

            if (teksUtama != null)
                teksUtama.text = soal.instruksiTeks;

            if (teksAngkaDindingUtama != null)
                teksAngkaDindingUtama.text = soal.angkaTarget.ToString();

            // Setup batu sesuai pilihan soal
            for (int i = 0; i < daftarBatu.Length; i++)
            {
                if (daftarBatu[i] == null) continue;

                if (i < soal.semuaPilihan.Length)
                {
                    int nilai = soal.semuaPilihan[i];
                    bool isFaktor = soal.faktorBenar.Contains(nilai);
                    daftarBatu[i].SetupBatu(nilai, isFaktor);
                }
                else
                {
                    daftarBatu[i].SembunyikanBatu();
                }
            }
        }
        else
        {
            // MENANG
            currentState = GameState.Menang;

            if (grupBatuEnergi != null) grupBatuEnergi.SetActive(false);
            if (tombolKonfirmasi != null) tombolKonfirmasi.gameObject.SetActive(false);
            if (panelUtama != null) panelUtama.SetActive(true);
            if (tombolLanjut != null) tombolLanjut.SetActive(true);

            if (teksUtama != null)
                teksUtama.text = "Luar biasa! Dinding segel Goa Faktor telah hancur sepenuhnya. Lanjutkan perjalananmu!";
        }
    }

    // =========================================================
    // VALIDASI MULTI-SELECT
    // =========================================================
    public void KonfirmasiJawaban()
    {
        if (currentState != GameState.Bertempur) return;
        if (soalTerjawab >= bankSoal.Count) return;

        DataSoalFaktor soal = bankSoal[soalTerjawab];

        // Kumpulkan angka yang sedang dipilih player
        List<int> yangDipilih = new List<int>();
        foreach (var batu in daftarBatu)
        {
            if (batu != null && batu.gameObject.activeInHierarchy && batu.IsSelected)
            {
                yangDipilih.Add(batu.NilaiBatu);
            }
        }

        // Bandingkan dengan faktor benar (urutan tidak penting)
        List<int> faktorBenarList = new List<int>(soal.faktorBenar);
        yangDipilih.Sort();
        faktorBenarList.Sort();

        bool sama = yangDipilih.Count == faktorBenarList.Count;
        if (sama)
        {
            for (int i = 0; i < yangDipilih.Count; i++)
            {
                if (yangDipilih[i] != faktorBenarList[i])
                {
                    sama = false;
                    break;
                }
            }
        }

        // Kunci semua batu
        foreach (var batu in daftarBatu)
        {
            if (batu != null) batu.KunciBatu();
        }

        if (sama)
        {
            // Benar → gerbang retak
            if (dindingGoaUtama != null)
                dindingGoaUtama.TerimaHantamanBatu();

            TampilkanFeedback(true);
        }
        else
        {
            TampilkanFeedback(false);
        }
    }

    public void TampilkanFeedback(bool isBenar)
    {
        currentState = GameState.Feedback;
        jawabanTerakhirBenar = isBenar;

        if (grupBatuEnergi != null) grupBatuEnergi.SetActive(false);
        if (tombolKonfirmasi != null) tombolKonfirmasi.gameObject.SetActive(false);
        if (panelUtama != null) panelUtama.SetActive(true);
        if (tombolLanjut != null) tombolLanjut.SetActive(true);

        DataSoalFaktor soal = bankSoal[soalTerjawab];

        if (isBenar)
        {
            koinSaatIni += hadiahKoinPerSoal;
            UpdateUIKoin();

            UpdatePencapaian.CatatJawaban(true);
            UpdatePencapaian.TambahKoin(hadiahKoinPerSoal);

            if (teksUtama != null)
                teksUtama.text = $"<color=green><b>JAWABAN BENAR! (+{hadiahKoinPerSoal} Koin)</b></color>\n" + soal.feedbackBenar;
        }
        else
        {
            UpdatePencapaian.CatatJawaban(false);
            nyawaPemain--;
            UpdateUIHati();

            if (nyawaPemain > 0)
            {
                if (teksUtama != null)
                    teksUtama.text = "<color=red><b>KURANG TEPAT! (-1 Hati)</b></color>\n" + soal.feedbackSalah;
            }
            else
            {
                ProsesGameOver();
            }
        }
    }

    private void TutupFeedbackDanLanjutkan()
    {
        if (panelUtama != null) panelUtama.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);

        if (jawabanTerakhirBenar)
        {
            soalTerjawab++;
            TampilkanSoalSaatIni();
        }
        else
        {
            // Ulangi soal yang sama
            currentState = GameState.Bertempur;

            if (panelUtama != null) panelUtama.SetActive(true);
            if (teksUtama != null) teksUtama.text = bankSoal[soalTerjawab].instruksiTeks;
            if (grupBatuEnergi != null) grupBatuEnergi.SetActive(true);
            if (tombolKonfirmasi != null) tombolKonfirmasi.gameObject.SetActive(true);

            // Reset pilihan batu
            foreach (var batu in daftarBatu)
            {
                if (batu != null && batu.gameObject.activeInHierarchy)
                    batu.ResetBatu();
            }
        }
    }

    // =========================================================
    // NYAWA & GAME OVER
    // =========================================================
    private void UpdateUIHati()
    {
        if (slotGambarHati != null && nyawaPemain >= 0 && nyawaPemain < slotGambarHati.Length)
        {
            if (spriteHatiRetak != null)
                slotGambarHati[nyawaPemain].sprite = spriteHatiRetak;
            else
                slotGambarHati[nyawaPemain].gameObject.SetActive(false);
        }
    }

    private void ProsesGameOver()
    {
        currentState = GameState.GameOver;

        if (grupBatuEnergi != null) grupBatuEnergi.SetActive(false);
        if (tombolKonfirmasi != null) tombolKonfirmasi.gameObject.SetActive(false);
        if (panelUtama != null) panelUtama.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(true);

        PlayerPrefs.Save();
        Invoke(nameof(PindahKeMenuPencapaian), 3f);
    }

    public void RestartLevel()
    {
        PindahKeMenuPencapaian();
    }

    private void PindahKeMenuPencapaian()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Pencapaian");
    }

    // =========================================================
    // TRANSISI
    // =========================================================
    private void MulaiJalanKePintu()
    {
        if (panelUtama != null) panelUtama.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);

        if (playerKsatria != null && titikPintuGoa != null)
            StartCoroutine(JalanOtomatisLaluPindah());
        else
            MuatSceneBerikutnya();
    }

    private void MuatSceneBerikutnya()
    {
        GameStatisticsManager.BerikanRewardLevel(4, 1, subLevelSaatIni, 25);
        WinManager.TampilkanWin();
    }

    private System.Collections.IEnumerator JalanOtomatisLaluPindah()
    {
        Collider2D col = playerKsatria.GetComponent<Collider2D>();
        Rigidbody2D rb = playerKsatria.GetComponent<Rigidbody2D>();

        if (col != null) col.enabled = false;
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        Vector3 target = titikPintuGoa.position;
        target.z = playerKsatria.transform.position.z;

        while (Vector3.Distance(playerKsatria.transform.position, target) > 0.1f)
        {
            playerKsatria.transform.position = Vector3.MoveTowards(
                playerKsatria.transform.position,
                target,
                kecepatanJalan * Time.deltaTime
            );
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        MuatSceneBerikutnya();
    }
}