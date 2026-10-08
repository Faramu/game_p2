using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Stage6Level2Manager : MonoBehaviour
{
    public static Stage6Level2Manager Instance;

    public enum GameState
    {
        Dialog,
        MemilihPeluru,
        Menembak,
        Transisi,
        Selesai
    }

    [Header("--- STATE ---")]
    public GameState currentState = GameState.Dialog;

    // =========================================================
    // PENGATURAN SCENE
    // =========================================================
    [Header("--- PENGATURAN SCENE INI ---")]
    [Range(1, 3)]
    public int subLevelSaatIni = 1;
    public int jumlahSoalDiSceneIni = 3;
    public bool otomatisPindahScene = false;
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
    public Button tombolLanjutFeedback;              // ← TOMBOL BARU (wajib diisi)

    // =========================================================
    // PELURU
    // =========================================================
    [Header("--- REFERENSI PELURU ---")]
    public PeluruKPK[] daftarPeluru;

    // =========================================================
    // SENAPAN & EFEK TEMBAK
    // =========================================================
    [Header("--- SENAPAN & EFEK TEMBAK ---")]
    public Transform titikTembak;
    public GameObject prefabPeluruTerbang;
    public GameObject prefabLedakan;
    public float durasiTerbangPeluru = 0.5f;

    [Header("--- AUDIO LEDAKAN ---")]
    public AudioClip suaraLedakan;
    [Range(0f, 1f)] public float volumeLedakan = 0.9f;

    [Header("--- ANIMASI SENAPAN ---")]
    public Animator animatorSenapan;
    public string namaAnimasiSenapan = "senapan_menembak";
    public float durasiAnimasiSenapan = 0.6f;

    // =========================================================
    // EFEK JAWABAN SALAH
    // =========================================================
    [Header("--- EFEK JAWABAN SALAH ---")]
    public GameObject prefabLapisanSihir;
    public GameObject prefabPeluruPantul;
    public GameObject prefabLedakanPlayer;
    public Transform playerTransform;
    public float durasiLapisanSihir = 1.2f;
    public float durasiPeluruPantul = 0.6f;

    // =========================================================
    // RAJA & PERISAI
    // =========================================================
    [Header("--- RAJA & PERISAI ---")]
    public RajaAnomali scriptRaja;
    public GameObject objekRaja;
    public TextMeshPro[] teksAngkaPerisai;
    public Slider sliderNyawaRaja;
    public int nyawaRajaMaksimal = 3;
    private int nyawaRajaSaatIni;

    // =========================================================
    // KURUNGAN PUTRI
    // =========================================================
    [Header("--- KURUNGAN PUTRI ---")]
    public GameObject[] daftarGembokKurungan;
    public GameObject objekKurunganTertutup;
    public GameObject objekKurunganTerbuka;
    public GameObject objekPutriTerkurung;
    public GameObject objekPutriBebas;
    public Transform posisiPutriDiSampingPlayer;

    // =========================================================
    // NYAWA PEMAIN
    // =========================================================
    [Header("--- SISTEM NYAWA PEMAIN ---")]
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
    public int koinPerJawabanBenar = 25;
    public TextMeshProUGUI teksKoinUI;

    // =========================================================
    // PLAYER
    // =========================================================
    [Header("--- PLAYER ---")]
    public MonoBehaviour scriptPergerakanPlayer;

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
    public float delaySetelahTembak = 1.3f;
    public float delaySetelahSelesai = 2f;
    public float durasiLedakan = 1.5f;

    // =========================================================
    // DATA INTERNAL
    // =========================================================
    private string[] dialogAktif;
    private int dialogIndex = 0;

    private List<DataSoalKPK> bankSoal = new List<DataSoalKPK>();
    private DataSoalKPK soalAktif;
    private int soalTerjawab = 0;
    private int totalSoal = 0;
    private int gembokYangSudahRusak = 0;
    private bool sedangMemproses = false;

    // Untuk membedakan apakah setelah klik Lanjut harus selesai level atau lanjut soal
    private bool setelahFeedbackHarusSelesai = false;

    private void MainkanSuaraLedakan(Vector3 posisi)
    {
        if (suaraLedakan == null) return;
        Vector3 posisiAudio = Camera.main != null ? Camera.main.transform.position : posisi;
        AudioSource.PlayClipAtPoint(suaraLedakan, posisiAudio, volumeLedakan);
    }

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
        nyawaRajaSaatIni = nyawaRajaMaksimal;
        koinSaatIni = GameStatisticsManager.GetTotalKoin();

        UpdateUIHati();
        UpdateUIKoin();
        UpdateUINyawaRaja();

        if (panelDialog != null) panelDialog.SetActive(false);
        if (panelFeedback != null) panelFeedback.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);
        if (objekPortalKeluar != null) objekPortalKeluar.SetActive(false);
        if (objekPutriBebas != null) objekPutriBebas.SetActive(false);
        if (objekKurunganTerbuka != null) objekKurunganTerbuka.SetActive(false);

        SembunyikanAngkaPerisai();
        MatikanSemuaPeluru();

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

        if (scriptRaja == null && objekRaja != null)
            scriptRaja = objekRaja.GetComponent<RajaAnomali>();

        MulaiLevel();
    }

    // =========================================================
    // MULAI LEVEL
    // =========================================================
    public void MulaiLevel()
    {
        soalTerjawab = 0;
        gembokYangSudahRusak = 0;
        sedangMemproses = false;
        setelahFeedbackHarusSelesai = false;

        dialogAktif = Stage6Level2Database.DapatkanDialog(subLevelSaatIni);
        List<DataSoalKPK> semuaSoal = Stage6Level2Database.DapatkanSoal(subLevelSaatIni);
        AcakList(semuaSoal);

        bankSoal.Clear();
        int jumlahAmbil = Mathf.Min(jumlahSoalDiSceneIni, semuaSoal.Count);
        for (int i = 0; i < jumlahAmbil; i++)
        {
            bankSoal.Add(semuaSoal[i]);
        }
        totalSoal = bankSoal.Count;

        if (daftarGembokKurungan != null)
        {
            foreach (var gembok in daftarGembokKurungan)
            {
                if (gembok != null) gembok.SetActive(true);
            }
        }

        if (objekKurunganTertutup != null) objekKurunganTertutup.SetActive(true);
        if (objekKurunganTerbuka != null) objekKurunganTerbuka.SetActive(false);
        if (objekPutriTerkurung != null) objekPutriTerkurung.SetActive(true);
        if (objekPutriBebas != null) objekPutriBebas.SetActive(false);

        nyawaRajaSaatIni = nyawaRajaMaksimal;
        UpdateUINyawaRaja();
        SembunyikanAngkaPerisai();

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = false;

        if (dialogAktif != null && dialogAktif.Length > 0)
        {
            dialogIndex = 0;
            currentState = GameState.Dialog;
            if (panelDialog != null) panelDialog.SetActive(true);
            TampilkanTeksDialog();
        }
        else
        {
            if (panelDialog != null) panelDialog.SetActive(false);
            JalankanAnimasiPerisaiAtauLanjut();
        }
    }

    private void AcakList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int r = Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = temp;
        }
    }

    // =========================================================
    // DIALOG
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
            JalankanAnimasiPerisaiAtauLanjut();
        }
    }

    private void JalankanAnimasiPerisaiAtauLanjut()
    {
        Debug.Log("<color=green>Dialog selesai, mencoba jalankan animasi perisai...</color>");

        if (scriptRaja != null)
        {
            scriptRaja.OnPerisaiSelesai = () =>
            {
                Debug.Log("<color=green>Callback OnPerisaiSelesai diterima, mulai soal</color>");
                MulaiSoalPertama();
            };

            scriptRaja.MulaiAnimasiPerisai();
        }
        else
        {
            Debug.LogError("scriptRaja masih NULL! Animasi tidak jalan. Melanjutkan ke soal...");
            MulaiSoalPertama();
        }
    }

    // =========================================================
    // SOAL & PELURU
    // =========================================================
    private void MulaiSoalPertama()
    {
        if (bankSoal.Count == 0)
        {
            SelesaiLevel();
            return;
        }

        currentState = GameState.MemilihPeluru;
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

        SetupPeluruDanPerisai(soalAktif);

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = true;

        currentState = GameState.MemilihPeluru;
        sedangMemproses = false;
    }

    private void SetupPeluruDanPerisai(DataSoalKPK soal)
    {
        if (teksAngkaPerisai != null && teksAngkaPerisai.Length >= 2)
        {
            if (teksAngkaPerisai[0] != null)
            {
                teksAngkaPerisai[0].gameObject.SetActive(true);
                teksAngkaPerisai[0].text = soal.angkaPertama.ToString();
            }

            if (teksAngkaPerisai[1] != null)
            {
                teksAngkaPerisai[1].gameObject.SetActive(true);
                teksAngkaPerisai[1].text = soal.angkaKedua.ToString();
            }
        }

        if (daftarPeluru == null || daftarPeluru.Length < 3)
        {
            Debug.LogError("daftarPeluru harus berisi 3 objek PeluruKPK!");
            return;
        }

        string[] pilihan = { soal.pilihan1, soal.pilihan2, soal.pilihan3 };
        bool[] benar = new bool[3];
        benar[soal.indeksJawabanBenar] = true;

        for (int i = 0; i < 3; i++)
        {
            if (daftarPeluru[i] != null)
            {
                daftarPeluru[i].SetupPeluru(pilihan[i], benar[i]);
            }
        }
    }

    private void SembunyikanAngkaPerisai()
    {
        if (teksAngkaPerisai == null) return;

        foreach (var teks in teksAngkaPerisai)
        {
            if (teks != null)
                teks.gameObject.SetActive(false);
        }
    }

    private void MatikanSemuaPeluru()
    {
        if (daftarPeluru == null) return;
        foreach (var p in daftarPeluru)
        {
            if (p != null) p.gameObject.SetActive(false);
        }
    }

    // =========================================================
    // DIPANGGIL DARI PELURU
    // =========================================================
    public void JawabanBenar()
    {
        if (sedangMemproses || currentState != GameState.MemilihPeluru) return;
        sedangMemproses = true;

        koinSaatIni += koinPerJawabanBenar;
        GameStatisticsManager.TambahKoin(koinPerJawabanBenar);
        GameStatisticsManager.CatatJawabanBenar(soalAktif != null ? $"S6L2-{soalAktif.instruksiTeks}" : "S6L2");
        UpdateUIKoin();

        TampilkanFeedback(soalAktif.feedbackBenar, true);
        soalTerjawab++;

        StartCoroutine(ProsesTembakan());
    }

    // =========================================================
    // JAWABAN SALAH → RAJA PASANG SIHIR + PANTULKAN PELURU
    // =========================================================
    public void PemainTerkenaDamage()
    {
        if (sedangMemproses || currentState != GameState.MemilihPeluru) return;
        sedangMemproses = true;

        GameStatisticsManager.CatatJawabanSalah(soalAktif != null ? $"S6L2-{soalAktif.instruksiTeks}" : "S6L2");

        if (soalAktif != null)
            TampilkanFeedback(soalAktif.feedbackSalah, false);

        StartCoroutine(ProsesJawabanSalah());
    }

    private IEnumerator ProsesJawabanSalah()
    {
        currentState = GameState.Menembak;
        MatikanSemuaPeluru();

        // 1. Munculkan lapisan sihir pelindung di raja
        GameObject sihir = null;
        if (prefabLapisanSihir != null && objekRaja != null)
        {
            sihir = Instantiate(prefabLapisanSihir, objekRaja.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.3f);

        // 2. Peluru pantul dari raja menuju player
        if (prefabPeluruPantul != null && objekRaja != null && playerTransform != null)
        {
            GameObject peluruPantul = Instantiate(prefabPeluruPantul, objekRaja.transform.position, Quaternion.identity);
            Vector3 startPos = objekRaja.transform.position;
            Vector3 targetPos = playerTransform.position;
            float waktu = 0f;

            while (waktu < durasiPeluruPantul)
            {
                waktu += Time.deltaTime;
                float t = waktu / durasiPeluruPantul;

                if (peluruPantul != null)
                {
                    peluruPantul.transform.position = Vector3.Lerp(startPos, targetPos, t);
                    Vector3 arah = (targetPos - startPos).normalized;
                    float sudut = Mathf.Atan2(arah.y, arah.x) * Mathf.Rad2Deg;
                    peluruPantul.transform.rotation = Quaternion.Euler(0, 0, sudut);
                }
                yield return null;
            }

            if (peluruPantul != null) Destroy(peluruPantul);
        }

        // 3. Ledakan di player
        if (prefabLedakanPlayer != null && playerTransform != null)
        {
            GameObject ledakanPlayer = Instantiate(prefabLedakanPlayer, playerTransform.position, Quaternion.identity);
            Destroy(ledakanPlayer, 1.5f);
            MainkanSuaraLedakan(playerTransform.position);
        }

        // 4. Player terkena damage
        nyawaSaatIni--;
        UpdateUIHati();
        StartCoroutine(EfekFlashMerah());

        // 5. Hilangkan lapisan sihir
        if (sihir != null)
            Destroy(sihir);

        yield return new WaitForSeconds(durasiLapisanSihir);

        // Cek game over
        if (nyawaSaatIni <= 0)
        {
            if (panelFeedback != null) panelFeedback.SetActive(false);
            ProsesGameOver();
        }
        else
        {
            // Masih hidup → tampilkan tombol Lanjut (tunggu player klik)
            setelahFeedbackHarusSelesai = false;
            if (tombolLanjutFeedback != null)
                tombolLanjutFeedback.gameObject.SetActive(true);
        }
    }

    // =========================================================
    // ANIMASI TEMBAKAN (JAWABAN BENAR)
    // =========================================================
    private IEnumerator ProsesTembakan()
    {
        currentState = GameState.Menembak;
        MatikanSemuaPeluru();

        // 1. Animasi Senapan
        if (animatorSenapan != null)
        {
            animatorSenapan.Play(namaAnimasiSenapan, 0, 0f);
            yield return new WaitForSeconds(durasiAnimasiSenapan);
        }
        else
        {
            Debug.LogWarning("Animator Senapan belum di-assign! Langsung lanjut ke peluru.");
        }

        // 2. Peluru terbang ke raja
        if (prefabPeluruTerbang != null && titikTembak != null && objekRaja != null)
        {
            GameObject peluru = Instantiate(prefabPeluruTerbang, titikTembak.position, Quaternion.identity);

            Vector3 startPos = titikTembak.position;
            Vector3 targetPos = objekRaja.transform.position;
            float waktu = 0f;

            while (waktu < durasiTerbangPeluru)
            {
                waktu += Time.deltaTime;
                float t = waktu / durasiTerbangPeluru;

                if (peluru != null)
                {
                    peluru.transform.position = Vector3.Lerp(startPos, targetPos, t);

                    Vector3 arah = (targetPos - startPos).normalized;
                    float sudut = Mathf.Atan2(arah.y, arah.x) * Mathf.Rad2Deg;
                    peluru.transform.rotation = Quaternion.Euler(0, 0, sudut);
                }

                yield return null;
            }

            if (peluru != null) Destroy(peluru);
        }

        // 3. Ledakan + Damage Raja
        if (prefabLedakan != null && objekRaja != null)
        {
            GameObject ledakan = Instantiate(prefabLedakan, objekRaja.transform.position, Quaternion.identity);
            Destroy(ledakan, durasiLedakan);
            MainkanSuaraLedakan(objekRaja.transform.position);
        }

        if (scriptRaja != null)
        {
            scriptRaja.KenaDamage();
        }

        nyawaRajaSaatIni--;
        UpdateUINyawaRaja();

        // Hilangkan 1 gembok
        if (daftarGembokKurungan != null && gembokYangSudahRusak < daftarGembokKurungan.Length)
        {
            if (daftarGembokKurungan[gembokYangSudahRusak] != null)
            {
                daftarGembokKurungan[gembokYangSudahRusak].SetActive(false);
            }
            gembokYangSudahRusak++;
        }

        yield return new WaitForSeconds(delaySetelahTembak);

        // Tentukan apakah ini soal terakhir
        setelahFeedbackHarusSelesai = (soalTerjawab >= totalSoal);

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

        if (setelahFeedbackHarusSelesai)
        {
            StartCoroutine(SelesaiLevelCoroutine());
        }
        else
        {
            TampilkanSoalBerikutnya();
        }
    }

    // =========================================================
    // SELESAI LEVEL
    // =========================================================
    private IEnumerator SelesaiLevelCoroutine()
    {
        currentState = GameState.Transisi;
        yield return new WaitForSeconds(delaySetelahSelesai);
        SelesaiLevel();
    }

    private void SelesaiLevel()
    {
        currentState = GameState.Selesai;

        if (daftarGembokKurungan != null)
        {
            foreach (var gembok in daftarGembokKurungan)
            {
                if (gembok != null) gembok.SetActive(false);
            }
        }

        if (objekKurunganTertutup != null) objekKurunganTertutup.SetActive(false);
        if (objekKurunganTerbuka != null) objekKurunganTerbuka.SetActive(true);

        if (objekPutriTerkurung != null) objekPutriTerkurung.SetActive(false);
        if (objekPutriBebas != null) objekPutriBebas.SetActive(true);

        SembunyikanAngkaPerisai();

        if (subLevelSaatIni == 3 && objekPutriBebas != null && posisiPutriDiSampingPlayer != null)
        {
            objekPutriBebas.transform.position = posisiPutriDiSampingPlayer.position;
            objekPutriBebas.transform.rotation = posisiPutriDiSampingPlayer.rotation;
        }

        if (scriptRaja != null)
            scriptRaja.Kalah();

        if (objekPortalKeluar != null) objekPortalKeluar.SetActive(true);

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = true;

        Debug.Log("<color=cyan>=== PUTRI ARUNA TERBEBAS! ===</color>");

        GameStatisticsManager.BerikanRewardLevel(6, 2, subLevelSaatIni, 25);
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

    private void UpdateUINyawaRaja()
    {
        if (sliderNyawaRaja != null)
        {
            sliderNyawaRaja.maxValue = nyawaRajaMaksimal;
            sliderNyawaRaja.value = nyawaRajaSaatIni;
        }
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
        if (scriptPergerakanPlayer != null) scriptPergerakanPlayer.enabled = false;
        MatikanSemuaPeluru();
        if (panelGameOver != null) panelGameOver.SetActive(true);

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