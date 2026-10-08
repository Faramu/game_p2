using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Stage6Level3Manager : MonoBehaviour
{
    public static Stage6Level3Manager Instance;

    public enum GameState
    {
        Dialog,
        MenungguJawaban,
        Menembak,
        Transisi,
        BerjalanKePortal,
        Video,
        Evaluasi,
        Selesai
    }

    [Header("--- STATE ---")]
    public GameState currentState = GameState.Dialog;

    [Header("--- PENGATURAN SCENE ---")]
    public int jumlahSoalDiSceneIni = 3;

    [Header("--- UI UTAMA ---")]
    public GameObject panelDialog;
    public TextMeshProUGUI areaTeksKuis;
    public Button tombolLanjutDialog;
    public GameObject objekJawaban;
    public TMP_InputField inputJawaban;
    public Button tombolJawab;
    public Button tombolLanjutFeedback;

[Header("--- VIDEO PORTAL ---")]
public PortalVideoController portalVideo;

    [Header("--- PANEL LAIN ---")]
    public GameObject panelGameOver;
    public GameObject panelFlashMerah;
    public GameObject panelEvaluasi;
    public Button tombolLanjutEvaluasi;
    public GameObject panelVideo;
    public VideoPlayer videoPlayer;
    public float durasiVideoCadangan = 12f;

    [Header("--- SENAPAN & EFEK TEMBAK ---")]
    public Transform titikTembak;
    public Transform titikPutriTembak;
    public GameObject prefabPeluruTerbang;
    public GameObject prefabLedakan;
    public float durasiTerbangPeluru = 0.5f;

    [Header("--- AUDIO LEDAKAN ---")]
    public AudioClip suaraLedakan;
    [Range(0f, 1f)] public float volumeLedakan = 0.9f;

    [Header("--- ANIMASI SENAPAN ---")]
    public Animator animatorSenapan;
    public string namaAnimasiSenapan = "senapan";
    public float durasiAnimasiSenapan = 0.6f;

    [Header("--- EFEK JAWABAN SALAH ---")]
    public GameObject prefabLapisanSihir;
    public GameObject prefabPeluruPantul;
    public GameObject prefabLedakanPlayer;
    public float durasiLapisanSihir = 1.2f;
    public float durasiPeluruPantul = 0.6f;

    [Header("--- RAJA & PERISAI ---")]
    public RajaAnomali scriptRaja;
    public GameObject objekRaja;
    public TextMeshPro[] teksAngkaPerisai;
    public Slider sliderNyawaRaja;
    public int nyawaRajaMaksimal = 3;
    private int nyawaRajaSaatIni;

    [Header("--- PUTRI & PLAYER ---")]
    public GameObject objekPutri;
    public PutriAnomali scriptPutri;
    public Transform playerTransform;
    public MonoBehaviour scriptPergerakanPlayer;
    public float kecepatanJalan = 3f;

    [Header("--- PORTAL ---")]
    public Transform titikPortal;
    public GameObject objekPortal;

    [Header("--- NYAWA PEMAIN ---")]
    public int nyawaMaksimal = 3;
    private int nyawaSaatIni;
    public Image[] ikonHatiUI;
    public Sprite spriteHatiUtuh;
    public Sprite spriteHatiRetak;

    [Header("--- KOIN ---")]
    public int koinSaatIni = 0;
    public int koinPerJawabanBenar = 25;
    public TextMeshProUGUI teksKoinUI;

    [Header("--- PINDAH SCENE ---")]
    public string namaSceneGameOver = "Menu_Pencapaian";
    public string namaSceneSetelahEvaluasi = "Menu_Pencapaian";

    [Header("--- PENGATURAN WAKTU ---")]
    public float delaySetelahTembak = 1.3f;
    public float delaySetelahRajaHilang = 0.6f;
    public float durasiLedakan = 1.5f;

    private string[] dialogAktif;
    private int dialogIndex = 0;
    private List<DataSoalKPK> bankSoal = new List<DataSoalKPK>();
    private DataSoalKPK soalAktif;
    private int soalTerjawabBenar = 0;
    private int totalSoalTarget = 0;
    private bool sedangMemproses = false;
    private bool lanjutKeSelesaiLevel = false;
    private bool lanjutDariEvaluasi = false;

    private void MainkanSuaraLedakan(Vector3 posisi)
    {
        if (suaraLedakan == null) return;
        Vector3 posisiAudio = Camera.main != null ? Camera.main.transform.position : posisi;
        AudioSource.PlayClipAtPoint(suaraLedakan, posisiAudio, volumeLedakan);
    }

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
        if (objekJawaban != null) objekJawaban.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
        if (panelVideo != null) panelVideo.SetActive(false);
        if (objekPortal != null) objekPortal.SetActive(false);

        SembunyikanAngkaPerisai();

        if (tombolLanjutDialog != null)
        {
            tombolLanjutDialog.onClick.RemoveAllListeners();
            tombolLanjutDialog.onClick.AddListener(LanjutDialog);
        }

        if (tombolJawab != null)
        {
            tombolJawab.onClick.RemoveAllListeners();
            tombolJawab.onClick.AddListener(CekJawaban);
        }

        if (tombolLanjutFeedback != null)
        {
            tombolLanjutFeedback.onClick.RemoveAllListeners();
            tombolLanjutFeedback.onClick.AddListener(OnKlikLanjutFeedback);
            tombolLanjutFeedback.gameObject.SetActive(false);
        }

        // Tombol evaluasi: diikat saat evaluasi muncul (bukan di Start)
        if (tombolLanjutEvaluasi != null)
            tombolLanjutEvaluasi.gameObject.SetActive(false);

        if (inputJawaban != null)
            inputJawaban.onSubmit.AddListener((val) => CekJawaban());

        if (scriptRaja == null && objekRaja != null)
            scriptRaja = objekRaja.GetComponent<RajaAnomali>();

        if (scriptPutri == null && objekPutri != null)
            scriptPutri = objekPutri.GetComponent<PutriAnomali>();

        if (videoPlayer == null && panelVideo != null)
            videoPlayer = panelVideo.GetComponentInChildren<VideoPlayer>(true);

        if (scriptRaja == null && objekRaja != null)
            scriptRaja = objekRaja.GetComponent<RajaAnomali>();

        if (scriptPutri == null && objekPutri != null)
            scriptPutri = objekPutri.GetComponent<PutriAnomali>();

        if (videoPlayer == null && panelVideo != null)
            videoPlayer = panelVideo.GetComponentInChildren<VideoPlayer>(true);

        // Cari PortalVideoController otomatis jika belum di-assign
        if (portalVideo == null && panelVideo != null)
            portalVideo = panelVideo.GetComponentInChildren<PortalVideoController>(true);
        MulaiLevel();
    }

    public void MulaiLevel()
    {
        soalTerjawabBenar = 0;
        sedangMemproses = false;
        lanjutKeSelesaiLevel = false;
        lanjutDariEvaluasi = false;
        totalSoalTarget = Mathf.Max(1, jumlahSoalDiSceneIni);

        dialogAktif = Stage6Level3Database.DapatkanDialog();
        IsiUlangBankSoal();

        nyawaRajaSaatIni = nyawaRajaMaksimal;
        UpdateUINyawaRaja();
        SembunyikanAngkaPerisai();

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = false;

        if (scriptPutri != null)
            scriptPutri.PlayIdle();

        if (dialogAktif != null && dialogAktif.Length > 0)
        {
            dialogIndex = 0;
            currentState = GameState.Dialog;
            TampilkanModeDialog();
            TampilkanTeksDialog();
        }
        else
        {
            JalankanAnimasiPerisaiAtauLanjut();
        }
    }

    private void IsiUlangBankSoal()
    {
        List<DataSoalKPK> semua = Stage6Level3Database.DapatkanSoal();
        if (semua == null || semua.Count == 0)
        {
            Debug.LogError("Stage6Level3Database kosong!");
            bankSoal.Clear();
            return;
        }

        AcakList(semua);
        bankSoal.Clear();

        int butuh = Mathf.Max(1, totalSoalTarget - soalTerjawabBenar);
        butuh = Mathf.Min(butuh, semua.Count);

        for (int i = 0; i < butuh; i++)
            bankSoal.Add(semua[i]);

        Debug.Log($"Isi bank soal: {bankSoal.Count} | Benar: {soalTerjawabBenar}/{totalSoalTarget}");
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

    private void TampilkanModeDialog()
    {
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);

        if (panelDialog != null) panelDialog.SetActive(true);
        if (objekJawaban != null) objekJawaban.SetActive(false);

        if (tombolLanjutDialog != null)
        {
            tombolLanjutDialog.gameObject.SetActive(true);
            tombolLanjutDialog.interactable = true;
        }

        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.gameObject.SetActive(false);

        if (tombolLanjutEvaluasi != null)
            tombolLanjutEvaluasi.gameObject.SetActive(false);
    }

    private void TampilkanModeSoal()
    {
        if (panelDialog != null) panelDialog.SetActive(true);
        if (objekJawaban != null) objekJawaban.SetActive(true);
        if (tombolLanjutDialog != null) tombolLanjutDialog.gameObject.SetActive(false);
        if (tombolLanjutFeedback != null) tombolLanjutFeedback.gameObject.SetActive(false);
        if (tombolLanjutEvaluasi != null) tombolLanjutEvaluasi.gameObject.SetActive(false);

        if (inputJawaban != null)
        {
            inputJawaban.text = "";
            inputJawaban.ActivateInputField();
        }
    }

    private void SembunyikanSemuaPanelUI()
    {
        if (panelDialog != null) panelDialog.SetActive(false);
        if (objekJawaban != null) objekJawaban.SetActive(false);
        if (tombolLanjutFeedback != null) tombolLanjutFeedback.gameObject.SetActive(false);
        if (tombolLanjutEvaluasi != null) tombolLanjutEvaluasi.gameObject.SetActive(false);
    }

    private void TampilkanTeksDialog()
    {
        if (areaTeksKuis != null && dialogAktif != null && dialogIndex < dialogAktif.Length)
        {
            areaTeksKuis.text = dialogAktif[dialogIndex];
            areaTeksKuis.color = Color.brown;
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
            SembunyikanSemuaPanelUI();
            JalankanAnimasiPerisaiAtauLanjut();
        }
    }

    private void JalankanAnimasiPerisaiAtauLanjut()
    {
        if (scriptRaja != null)
        {
            scriptRaja.OnPerisaiSelesai = () => MulaiSoalPertama();
            scriptRaja.MulaiAnimasiPerisai();
        }
        else
        {
            MulaiSoalPertama();
        }
    }

    private void MulaiSoalPertama()
    {
        if (bankSoal.Count == 0)
            IsiUlangBankSoal();

        currentState = GameState.MenungguJawaban;
        TampilkanSoalBaru();
    }

    private void TampilkanSoalBaru()
    {
        if (soalTerjawabBenar >= totalSoalTarget)
        {
            Debug.Log("Target jawaban benar tercapai → selesai level");
            StartCoroutine(SelesaiLevelCoroutine());
            return;
        }

        if (bankSoal.Count == 0)
            IsiUlangBankSoal();

        if (bankSoal.Count == 0)
        {
            Debug.LogError("Tidak ada soal. Cek Stage6Level3Database.");
            return;
        }

        soalAktif = bankSoal[0];
        bankSoal.RemoveAt(0);

        if (teksAngkaPerisai != null && teksAngkaPerisai.Length >= 2)
        {
            if (teksAngkaPerisai[0] != null)
            {
                teksAngkaPerisai[0].gameObject.SetActive(true);
                teksAngkaPerisai[0].text = soalAktif.angkaPertama.ToString();
            }
            if (teksAngkaPerisai[1] != null)
            {
                teksAngkaPerisai[1].gameObject.SetActive(true);
                teksAngkaPerisai[1].text = soalAktif.angkaKedua.ToString();
            }
        }

        if (areaTeksKuis != null)
        {
            areaTeksKuis.text = $"Berapakah KPK dari {soalAktif.angkaPertama} dan {soalAktif.angkaKedua}?";
            areaTeksKuis.color = Color.brown;
        }

        TampilkanModeSoal();
        currentState = GameState.MenungguJawaban;
        sedangMemproses = false;
        lanjutKeSelesaiLevel = false;
    }

    private void SembunyikanAngkaPerisai()
    {
        if (teksAngkaPerisai == null) return;
        foreach (var t in teksAngkaPerisai)
            if (t != null) t.gameObject.SetActive(false);
    }

    public void CekJawaban()
    {
        if (sedangMemproses || currentState != GameState.MenungguJawaban) return;
        if (inputJawaban == null || soalAktif == null) return;

        string jawabanUser = inputJawaban.text.Trim();
        if (string.IsNullOrEmpty(jawabanUser))
        {
            Debug.Log("Jawaban kosong");
            return;
        }

        string jawabanBenar = AmbilJawabanBenar(soalAktif);
        bool benar = jawabanUser.Equals(jawabanBenar, System.StringComparison.OrdinalIgnoreCase);

        Debug.Log($"User: [{jawabanUser}] | Benar: [{jawabanBenar}] | Hasil: {(benar ? "BENAR" : "SALAH")}");

        if (benar)
            StartCoroutine(ProsesJawabanBenar());
        else
            StartCoroutine(ProsesJawabanSalah());
    }

    private string AmbilJawabanBenar(DataSoalKPK soal)
    {
        if (soal.indeksJawabanBenar == 0) return soal.pilihan1;
        if (soal.indeksJawabanBenar == 1) return soal.pilihan2;
        return soal.pilihan3;
    }

    private void TampilkanFeedback(string pesan, bool benar)
    {
        if (areaTeksKuis == null) return;
        areaTeksKuis.text = pesan;
        areaTeksKuis.color = benar ? Color.green : Color.red;
        if (panelDialog != null) panelDialog.SetActive(true);
    }

    private IEnumerator ProsesJawabanBenar()
    {
        sedangMemproses = true;
        currentState = GameState.Menembak;

        koinSaatIni += koinPerJawabanBenar;
        GameStatisticsManager.TambahKoin(koinPerJawabanBenar);
        GameStatisticsManager.CatatJawabanBenar(soalAktif != null ? $"S6L3-{soalAktif.instruksiTeks}" : "S6L3");
        UpdateUIKoin();
        soalTerjawabBenar++;

        TampilkanFeedback(soalAktif.feedbackBenar, true);
        if (objekJawaban != null) objekJawaban.SetActive(false);

        if (animatorSenapan != null)
        {
            animatorSenapan.Play(namaAnimasiSenapan, 0, 0f);
            yield return new WaitForSeconds(durasiAnimasiSenapan);
        }

        Transform titikMulai = titikPutriTembak != null ? titikPutriTembak : titikTembak;
        if (prefabPeluruTerbang != null && titikMulai != null && objekRaja != null)
        {
            GameObject peluru = Instantiate(prefabPeluruTerbang, titikMulai.position, Quaternion.identity);
            Vector3 a = titikMulai.position;
            Vector3 b = objekRaja.transform.position;
            float t = 0f;
            while (t < durasiTerbangPeluru)
            {
                t += Time.deltaTime;
                float p = t / durasiTerbangPeluru;
                if (peluru != null)
                {
                    peluru.transform.position = Vector3.Lerp(a, b, p);
                    Vector3 dir = (b - a).normalized;
                    peluru.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                }
                yield return null;
            }
            if (peluru != null) Destroy(peluru);
        }

        if (prefabLedakan != null && objekRaja != null)
        {
            GameObject e = Instantiate(prefabLedakan, objekRaja.transform.position, Quaternion.identity);
            Destroy(e, durasiLedakan);
            MainkanSuaraLedakan(objekRaja.transform.position);
        }

        if (scriptRaja != null) scriptRaja.KenaDamage();
        nyawaRajaSaatIni = Mathf.Max(0, nyawaRajaSaatIni - 1);
        UpdateUINyawaRaja();

        yield return new WaitForSeconds(delaySetelahTembak);

        lanjutKeSelesaiLevel = (soalTerjawabBenar >= totalSoalTarget);
        Debug.Log($"Benar! Progress {soalTerjawabBenar}/{totalSoalTarget} | Selesai? {lanjutKeSelesaiLevel}");

        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.gameObject.SetActive(true);
        else
            OnKlikLanjutFeedback();
    }

    private IEnumerator ProsesJawabanSalah()
    {
        sedangMemproses = true;
        currentState = GameState.Menembak;
        lanjutKeSelesaiLevel = false;

        GameStatisticsManager.CatatJawabanSalah(soalAktif != null ? $"S6L3-{soalAktif.instruksiTeks}" : "S6L3");

        TampilkanFeedback(soalAktif.feedbackSalah, false);
        if (objekJawaban != null) objekJawaban.SetActive(false);

        GameObject sihir = null;
        if (prefabLapisanSihir != null && objekRaja != null)
            sihir = Instantiate(prefabLapisanSihir, objekRaja.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.3f);

        if (prefabPeluruPantul != null && objekRaja != null && playerTransform != null)
        {
            GameObject peluru = Instantiate(prefabPeluruPantul, objekRaja.transform.position, Quaternion.identity);
            Vector3 a = objekRaja.transform.position;
            Vector3 b = playerTransform.position;
            float t = 0f;
            while (t < durasiPeluruPantul)
            {
                t += Time.deltaTime;
                float p = t / durasiPeluruPantul;
                if (peluru != null)
                {
                    peluru.transform.position = Vector3.Lerp(a, b, p);
                    Vector3 dir = (b - a).normalized;
                    peluru.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                }
                yield return null;
            }
            if (peluru != null) Destroy(peluru);
        }

        if (prefabLedakanPlayer != null && playerTransform != null)
        {
            GameObject e = Instantiate(prefabLedakanPlayer, playerTransform.position, Quaternion.identity);
            Destroy(e, 1.5f);
            MainkanSuaraLedakan(playerTransform.position);
        }

        nyawaSaatIni--;
        UpdateUIHati();
        StartCoroutine(EfekFlashMerah());

        if (sihir != null) Destroy(sihir);

        yield return new WaitForSeconds(durasiLapisanSihir);

        if (nyawaSaatIni <= 0)
        {
            ProsesGameOver();
            yield break;
        }

        lanjutKeSelesaiLevel = false;
        Debug.Log($"Salah. Nyawa player: {nyawaSaatIni}. Lanjut soal lain (bukan portal).");

        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.gameObject.SetActive(true);
        else
            OnKlikLanjutFeedback();
    }

    public void OnKlikLanjutFeedback()
    {
        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.gameObject.SetActive(false);

        if (lanjutKeSelesaiLevel)
            StartCoroutine(SelesaiLevelCoroutine());
        else
            TampilkanSoalBaru();
    }

// =========================================================
// SELESAI: Portal → VIDEO → EVALUASI → Menu_Pencapaian
// =========================================================
private IEnumerator SelesaiLevelCoroutine()
{
    currentState = GameState.Transisi;
    SembunyikanSemuaPanelUI();
    SembunyikanAngkaPerisai();

    Debug.Log("<color=cyan>SELESAI LEVEL: raja → portal → video → evaluasi</color>");

    if (scriptRaja != null)
        scriptRaja.Kalah();

    GameStatisticsManager.CatatBossKalah("Raja Anomali");
    GameStatisticsManager.BerikanRewardLevel(6, 3, 1, 50);

    yield return new WaitForSeconds(Mathf.Min(delaySetelahRajaHilang, 0.3f));

    if (objekRaja != null)
        objekRaja.SetActive(false);

    if (objekPortal != null)
        objekPortal.SetActive(true);

    currentState = GameState.BerjalanKePortal;
    yield return StartCoroutine(JalanMenujuPortal());

    // Video langsung
    yield return StartCoroutine(CobaPutarVideoDiPortal());

    currentState = GameState.Selesai;
    Debug.Log("<color=cyan>Level Selesai!</color>");
    if (panelEvaluasi != null)
    {
        WinManager.TampilkanWin(panelEvaluasi);
    }
    else
    {
        WinManager.TampilkanWin();
    }
}

// Instant ke portal (hilangkan delay jalan panjang)
private IEnumerator JalanMenujuPortal()
{
    if (titikPortal == null) yield break;

    if (scriptPergerakanPlayer != null)
        scriptPergerakanPlayer.enabled = false;

    if (playerTransform != null)
        playerTransform.position = titikPortal.position;

    if (objekPutri != null)
        objekPutri.transform.position = titikPortal.position;

    if (scriptPutri != null)
        scriptPutri.SampaiPortal();

    Debug.Log("<color=cyan>Sampai portal (instant)</color>");
    yield return null;
}

private IEnumerator CobaPutarVideoDiPortal()
{
    currentState = GameState.Video;
    SembunyikanSemuaPanelUI();

    if (panelEvaluasi != null)
        panelEvaluasi.SetActive(false);

    float tPortal = Time.realtimeSinceStartup;
    Debug.Log($"<color=yellow>PORTAL SIAP @ {tPortal:F2} → panggil video</color>");

    if (portalVideo == null)
    {
        Debug.LogWarning("portalVideo NULL → langsung evaluasi");
        yield break;
    }

    bool selesai = false;
    portalVideo.OnVideoSelesai = () => { selesai = true; };

    portalVideo.PlayDariPortal();
    Debug.Log($"<color=yellow>PlayDariPortal selesai dipanggil +{(Time.realtimeSinceStartup - tPortal):F2}s</color>");

    float t = 0f;
    while (!selesai && t < 120f)
    {
        t += Time.deltaTime;
        yield return null;
    }

    if (!selesai)
    {
        Debug.LogWarning("Video timeout → Skip");
        portalVideo.Skip();
    }
    else
    {
        Debug.Log("Video selesai → evaluasi");
    }
}

private IEnumerator TampilkanEvaluasiDanTungguKlik()
{
    currentState = GameState.Evaluasi;
    SembunyikanSemuaPanelUI();
    lanjutDariEvaluasi = false;

    if (panelEvaluasi != null)
    {
        panelEvaluasi.SetActive(true);
        Debug.Log("<color=green>Panel evaluasi ON</color>");
    }
    else
    {
        lanjutDariEvaluasi = true;
        yield break;
    }

    Button btn = tombolLanjutEvaluasi != null ? tombolLanjutEvaluasi : tombolLanjutFeedback;

    if (btn != null)
    {
        btn.gameObject.SetActive(true);
        btn.interactable = true;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnKlikLanjutDariEvaluasi);
    }
    else
    {
        yield return new WaitForSeconds(2f);
        lanjutDariEvaluasi = true;
    }

    while (!lanjutDariEvaluasi)
        yield return null;

    if (btn != null)
    {
        btn.onClick.RemoveListener(OnKlikLanjutDariEvaluasi);
        btn.gameObject.SetActive(false);
    }

    if (panelEvaluasi != null)
        panelEvaluasi.SetActive(false);
}

    public void OnKlikLanjutDariEvaluasi()
    {
        lanjutDariEvaluasi = true;
        Debug.Log("<color=green>Lanjut evaluasi → Map_Checkpoint</color>");
        WinManager.PindahKeMapCheckpoint();
    }

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

    private IEnumerator EfekFlashMerah()
    {
        if (panelFlashMerah == null) yield break;
        panelFlashMerah.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        panelFlashMerah.SetActive(false);
    }

    private void ProsesGameOver()
    {
        currentState = GameState.Selesai;
        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = false;
        SembunyikanSemuaPanelUI();
        if (panelGameOver != null)
            panelGameOver.SetActive(true);
        Invoke(nameof(PindahKeMenuPencapaian), 2f);
    }

    private void PindahKeMenuPencapaian()
    {
        SceneManager.LoadScene(namaSceneGameOver);
    }
}