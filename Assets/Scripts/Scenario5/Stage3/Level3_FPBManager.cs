using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Level3_FPBManager : MonoBehaviour
{
    [Header("--- PENGATURAN LEVEL SCENE ---")]
    public int levelSaatIni = 3;
    public int targetJumlahSoal = 10;

    [Header("--- REFERENSI UI DIALOG ---")]
    public GameObject panelDialogMisi;
    public TextMeshProUGUI teksIsiDialog;
    public GameObject tombolLanjut;

    [Header("--- REFERENSI MERIAM ---")]
    public GameObject objekMeriam1;
    public GameObject objekMeriam2;
    public TextMeshPro teksMeriam1;
    public TextMeshPro teksMeriam2;
    public GameObject prefabLedakanMeriam;
    public GameObject prefabPeluruApi;
    public Transform titikMunculPeluru1;
    public Transform titikMunculPeluru2;

    [Header("--- AUDIO LEDAKAN ---")]
    public AudioClip suaraLedakan;
    [Range(0f, 1f)] public float volumeLedakan = 0.9f;

    [Header("--- INPUT JAWABAN ---")]
    public GameObject grupInputJawaban;
    public TMP_InputField inputJawaban;
    public Button tombolSubmit;
    public GameObject panelFlashMerah;

    [Header("--- SISTEM NYAWA ---")]
    public int nyawaMaksimal = 3;
    private int nyawaSaatIni;
    public Image[] ikonHatiUI;
    public Sprite spriteHatiUtuh;
    public Sprite spriteHatiRetak;

    [Header("--- SISTEM KOIN ---")]
    public int koinSaatIni = 0;
    public int koinPerJawabanBenar = 20;
    public TextMeshProUGUI teksKoinUI;

    [Header("--- GAMEPLAY ---")]
    public MonoBehaviour scriptPergerakanPlayer;
    public Transform posisiAwalPlayer;

    [Header("--- BENTENG ---")]
    public BentengController[] daftarBenteng;
    public TMP_Text[] teksAngkaBenteng;

    [Header("--- PREFAB TEMBOK PELINDUNG ---")]
    public GameObject prefabTembokPelindung;
    private GameObject tembokInstan;

    [Header("--- PENGATURAN ANIMASI ---")]
    public float delaySetelahTeleport = 0.45f;
    public float delaySetelahBukaBenteng = 0.6f;
    public float kecepatanPeluru = 12f;

    [Header("--- PINDAH SCENE ---")]
    [Tooltip("Scene stage berikutnya setelah menang (dari panel evaluasi)")]
    public string namaSceneBerikutnya;

    [Tooltip("Scene saat nyawa habis")]
    public string namaSceneGameOver = "Menu_Pencapaian";

    public GameObject objekPortalKeluar;

    [Header("--- PANEL EVALUASI & UI LAIN ---")]
    public GameObject panelEvaluasi;
    public Button tombolLanjutEvaluasi;
    public GameObject panelHeader;
    public GameObject[] uiLainYangDimatikan;

    // ================= INTERNAL =================
    private List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor> bankSoalAktif =
        new List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor>();
    private Level2_PohonFaktorDatabase.DataSoalPohonFaktor soalAktif;

    private List<Level2_PohonFaktorDatabase.OpsiJawaban> pilihanSaatIni =
        new List<Level2_PohonFaktorDatabase.OpsiJawaban>();

    private int soalTerjawab = 0;
    private struct BarisDialog { public string isiTeks; }
    private BarisDialog[] alurDialog;

    private int dialogIndex = 0;
    private bool isDialogActive = false;
    private bool isIntroSudahTampil = false;
    private bool sedangModeIntro = false;
    private bool sedangProsesJawaban = false;

    private void MainkanSuaraLedakan(Vector3 posisi)
    {
        if (suaraLedakan == null) return;
        Vector3 posisiAudio = Camera.main != null ? Camera.main.transform.position : posisi;
        AudioSource.PlayClipAtPoint(suaraLedakan, posisiAudio, volumeLedakan);
    }

    void Awake()
    {
        if (grupInputJawaban != null) grupInputJawaban.SetActive(false);
        if (panelDialogMisi != null) panelDialogMisi.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
    }

    void Start()
    {
        nyawaSaatIni = nyawaMaksimal;
        UpdateUIHati();
        UpdateUIKoin();
        SiapkanBankSoal();

        if (panelDialogMisi != null) panelDialogMisi.SetActive(false);
        if (grupInputJawaban != null) grupInputJawaban.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);
        if (objekPortalKeluar != null) objekPortalKeluar.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);

        if (teksMeriam1 != null) teksMeriam1.text = "";
        if (teksMeriam2 != null) teksMeriam2.text = "";

        ResetSemuaBenteng();

        if (tombolLanjut != null)
        {
            Button btn = tombolLanjut.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(LanjutDialog);
            }
        }

        if (tombolSubmit != null)
        {
            tombolSubmit.onClick.RemoveAllListeners();
            tombolSubmit.onClick.AddListener(SubmitJawaban);
        }

        if (inputJawaban != null)
        {
            inputJawaban.onSubmit.RemoveAllListeners();
            inputJawaban.onSubmit.AddListener((_) => SubmitJawaban());
        }

        // Tombol Lanjut di panel evaluasi → stage berikutnya
        if (tombolLanjutEvaluasi != null)
        {
            tombolLanjutEvaluasi.onClick.RemoveAllListeners();
            tombolLanjutEvaluasi.onClick.AddListener(LanjutKeStageBerikutnya);
        }
    }

    void Update() { }

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

    public void TambahKoin(int jumlah)
    {
        koinSaatIni += jumlah;
        UpdateUIKoin();
        GameStatisticsManager.TambahKoin(jumlah, 5, 3, 1);
    }

    private void SiapkanBankSoal()
    {
        List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor> sumberSoal =
            Level2_PohonFaktorDatabase.GetSoalLevel3();

        bankSoalAktif = AmbilSoal(sumberSoal, targetJumlahSoal);
        Debug.Log($"[SOAL] Level 3 → {bankSoalAktif.Count} soal");
    }

    private List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor> AmbilSoal(
        List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor> sumber, int jumlah)
    {
        List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor> hasil =
            new List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor>(sumber);

        for (int i = 0; i < hasil.Count; i++)
        {
            var temp = hasil[i];
            int r = Random.Range(i, hasil.Count);
            hasil[i] = hasil[r];
            hasil[r] = temp;
        }

        if (jumlah < hasil.Count)
            hasil = hasil.GetRange(0, jumlah);

        return hasil;
    }

    private void TampilkanModeDialog()
    {
        if (grupInputJawaban != null) grupInputJawaban.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(true);
    }

    private void TampilkanModeInput()
    {
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (grupInputJawaban != null) grupInputJawaban.SetActive(true);
    }

    private void SembunyikanSemuaKontrol()
    {
        if (grupInputJawaban != null) grupInputJawaban.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
    }

    public void MulaiTutorial()
    {
        KembalikanPlayerKePosisiAwal();
        sedangProsesJawaban = false;

        if (scriptPergerakanPlayer != null)
        {
            scriptPergerakanPlayer.enabled = false;
            var rb = scriptPergerakanPlayer.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        panelDialogMisi.SetActive(true);
        TampilkanModeDialog();

        isDialogActive = true;
        dialogIndex = 0;

        if (!isIntroSudahTampil)
        {
            sedangModeIntro = true;
            SiapkanIntroMateri();
        }
        else
        {
            sedangModeIntro = false;
            SiapkanSoalBerikutnya();
        }

        TampilkanTeks();
    }

    private void SiapkanIntroMateri()
    {
        alurDialog = new BarisDialog[]
        {
            new BarisDialog { isiTeks = "Ksatria, ini adalah <color=red>Level 3</color>!\nTantangan terakhir: kamu harus mengetik sendiri jawaban FPB-nya." },
            new BarisDialog { isiTeks = "Lihat faktor di dialog, lalu ketik angka FPB yang <color=red>PALING BESAR</color>." },
            new BarisDialog { isiTeks = "Ketik angka yang ada di benteng, lalu tekan Submit!" }
        };
    }

    private void SiapkanSoalBerikutnya()
    {
        KembalikanPlayerKePosisiAwal();
        ResetSemuaBenteng();
        sedangProsesJawaban = false;

        if (bankSoalAktif == null || bankSoalAktif.Count == 0)
        {
            StartCoroutine(TampilkanPanelEvaluasi());
            return;
        }

        soalAktif = bankSoalAktif[0];
        bankSoalAktif.RemoveAt(0);

        if (teksMeriam1 != null) teksMeriam1.text = soalAktif.angkaApi.ToString();
        if (teksMeriam2 != null) teksMeriam2.text = soalAktif.angkaEs.ToString();
        if (objekMeriam1 != null) objekMeriam1.SetActive(true);
        if (objekMeriam2 != null) objekMeriam2.SetActive(true);

        alurDialog = new BarisDialog[]
        {
            new BarisDialog { isiTeks =
                $"<b>[Soal {soalTerjawab + 1}/{targetJumlahSoal}]</b>\n" +
                $"Meriam menyerang dengan angka:\n" +
                $"<color=red>{soalAktif.angkaApi}</color>   &   <color=red>{soalAktif.angkaEs}</color>\n" +
                $"Tentukan <color=red>FPB</color>-nya!" },

            new BarisDialog { isiTeks =
                $"Faktor <color=red>{soalAktif.angkaApi}</color>:\n<color=black>{soalAktif.pohonFaktorApi}</color>\n" +
                $"Faktor <color=red>{soalAktif.angkaEs}</color>:\n<color=black>{soalAktif.pohonFaktorEs}</color>" },

            new BarisDialog { isiTeks =
                $"Ketik angka FPB yang <color=red>PALING BESAR</color>, lalu tekan Submit!" }
        };

        pilihanSaatIni.Clear();
        foreach (var o in soalAktif.pilihanJawaban)
        {
            pilihanSaatIni.Add(new Level2_PohonFaktorDatabase.OpsiJawaban
            {
                angka = o.angka,
                isBenar = o.isBenar,
                feedbackTeks = o.feedbackTeks
            });
        }

        for (int i = 0; i < pilihanSaatIni.Count; i++)
        {
            var temp = pilihanSaatIni[i];
            int r = Random.Range(i, pilihanSaatIni.Count);
            pilihanSaatIni[i] = pilihanSaatIni[r];
            pilihanSaatIni[r] = temp;
        }

        if (teksAngkaBenteng != null)
        {
            for (int i = 0; i < teksAngkaBenteng.Length && i < pilihanSaatIni.Count; i++)
            {
                if (teksAngkaBenteng[i] != null)
                {
                    teksAngkaBenteng[i].gameObject.SetActive(true);
                    teksAngkaBenteng[i].text = pilihanSaatIni[i].angka.ToString();
                }
            }
        }

        if (inputJawaban != null)
        {
            inputJawaban.text = "";
            inputJawaban.interactable = true;
        }
    }

    public void LanjutDialog()
    {
        if (!isDialogActive) return;

        dialogIndex++;

        if (dialogIndex < alurDialog.Length)
        {
            TampilkanModeDialog();
            TampilkanTeks();
        }
        else
        {
            if (sedangModeIntro)
            {
                isIntroSudahTampil = true;
                sedangModeIntro = false;
                SiapkanSoalBerikutnya();
                dialogIndex = 0;
                TampilkanModeDialog();
                TampilkanTeks();
            }
            else
            {
                TampilkanInputJawaban();
            }
        }
    }

    private void TampilkanTeks()
    {
        if (alurDialog == null || dialogIndex >= alurDialog.Length) return;
        teksIsiDialog.text = alurDialog[dialogIndex].isiTeks;
    }

    private void TampilkanInputJawaban()
    {
        TampilkanModeInput();

        teksIsiDialog.text =
            $"Meriam: <color=red>{soalAktif.angkaApi}</color> & <color=red>{soalAktif.angkaEs}</color>\n" +
            $"Ketik <color=red>FPB</color> yang paling besar, lalu Submit!";

        if (inputJawaban != null)
        {
            inputJawaban.text = "";
            inputJawaban.interactable = true;
            inputJawaban.Select();
            inputJawaban.ActivateInputField();
        }
    }

    public void SubmitJawaban()
    {
        if (sedangProsesJawaban) return;
        if (grupInputJawaban == null || !grupInputJawaban.activeSelf) return;

        string teks = inputJawaban != null ? inputJawaban.text.Trim() : "";

        if (string.IsNullOrEmpty(teks) || !int.TryParse(teks, out int jawabanPlayer))
        {
            teksIsiDialog.text = "<color=red>Masukkan angka yang valid!</color>";
            return;
        }

        int indexBenteng = -1;
        Level2_PohonFaktorDatabase.OpsiJawaban opsiDitemukan = default;
        bool ketemu = false;

        for (int i = 0; i < pilihanSaatIni.Count; i++)
        {
            if (pilihanSaatIni[i].angka == jawabanPlayer)
            {
                indexBenteng = i;
                opsiDitemukan = pilihanSaatIni[i];
                ketemu = true;
                break;
            }
        }

        if (!ketemu)
        {
            teksIsiDialog.text = $"<color=red>{jawabanPlayer}</color> bukan pilihan benteng!\nKetik angka yang tertera di benteng.";
            return;
        }

        if (daftarBenteng == null || indexBenteng >= daftarBenteng.Length || daftarBenteng[indexBenteng] == null)
            return;

        sedangProsesJawaban = true;
        SembunyikanSemuaKontrol();

        if (inputJawaban != null) inputJawaban.interactable = false;

        StartCoroutine(AlurBenteng(daftarBenteng[indexBenteng], opsiDitemukan.isBenar, opsiDitemukan.feedbackTeks));
    }

    private IEnumerator AlurBenteng(BentengController bentengDipilih, bool isBenar, string feedback)
    {
        if (scriptPergerakanPlayer != null)
        {
            scriptPergerakanPlayer.enabled = false;
            var rb = scriptPergerakanPlayer.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.transform.position = bentengDipilih.GetPlayerPosition();

        yield return new WaitForSeconds(delaySetelahTeleport);

        bentengDipilih.Open(isBenar);

        if (isBenar)
        {
            if (prefabTembokPelindung != null)
            {
                if (tembokInstan != null) Destroy(tembokInstan);
                Vector3 posTembok = bentengDipilih.GetTitikMunculTembok();
                tembokInstan = Instantiate(prefabTembokPelindung, posTembok, Quaternion.identity);
                tembokInstan.SetActive(true);
            }
        }
        else
        {
            if (tembokInstan != null)
            {
                Destroy(tembokInstan);
                tembokInstan = null;
            }
        }

        yield return new WaitForSeconds(delaySetelahBukaBenteng);

        if (daftarBenteng != null)
        {
            for (int i = 0; i < daftarBenteng.Length; i++)
            {
                var b = daftarBenteng[i];
                if (b != null && b != bentengDipilih)
                {
                    b.KenaPeluruDanMeledak(prefabLedakanMeriam);
                    MainkanSuaraLedakan(b.transform.position);
                    if (teksAngkaBenteng != null && i < teksAngkaBenteng.Length && teksAngkaBenteng[i] != null)
                        teksAngkaBenteng[i].gameObject.SetActive(false);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 targetPos;
        if (isBenar && tembokInstan != null)
        {
            targetPos = tembokInstan.transform.position;
        }
        else
        {
            targetPos = scriptPergerakanPlayer != null
                ? scriptPergerakanPlayer.transform.position
                : bentengDipilih.GetPlayerPosition();
        }

        Vector3 awal1 = titikMunculPeluru1 != null ? titikMunculPeluru1.position :
                        (objekMeriam1 != null ? objekMeriam1.transform.position : Vector3.zero);
        Vector3 awal2 = titikMunculPeluru2 != null ? titikMunculPeluru2.position :
                        (objekMeriam2 != null ? objekMeriam2.transform.position : Vector3.zero);

        if (prefabPeluruApi != null)
        {
            if (objekMeriam1 != null && objekMeriam1.activeInHierarchy)
                yield return StartCoroutine(TembakPeluruManual(awal1, targetPos));
            if (objekMeriam2 != null && objekMeriam2.activeInHierarchy)
                yield return StartCoroutine(TembakPeluruManual(awal2, targetPos));
        }

        yield return new WaitForSeconds(0.3f);

        if (isBenar)
            yield return StartCoroutine(ProsesSukses(bentengDipilih, feedback));
        else
            yield return StartCoroutine(ProsesDamageDanRetry(feedback));
    }

    private IEnumerator TembakPeluruManual(Vector3 awal, Vector3 target)
    {
        if (prefabPeluruApi == null) yield break;

        GameObject peluru = Instantiate(prefabPeluruApi, awal, Quaternion.identity);
        peluru.name = "Peluru_Temp";
        peluru.SetActive(true);

        var proj = peluru.GetComponent<ProjectileMeriam>();
        if (proj != null) proj.enabled = false;

        Vector3 arah = (target - awal).normalized;
        if (arah.sqrMagnitude > 0.001f)
        {
            float sudut = Mathf.Atan2(arah.y, arah.x) * Mathf.Rad2Deg;
            peluru.transform.rotation = Quaternion.Euler(0, 0, sudut);
        }

        float jarak = Vector3.Distance(awal, target);
        float durasi = Mathf.Clamp(jarak / Mathf.Max(kecepatanPeluru, 1f), 0.35f, 1.2f);
        float t = 0f;

        while (t < 1f)
        {
            if (peluru == null) yield break;
            t += Time.deltaTime / durasi;
            peluru.transform.position = Vector3.Lerp(awal, target, Mathf.Clamp01(t));
            yield return null;
        }

        if (peluru != null) peluru.transform.position = target;

        if (prefabLedakanMeriam != null)
        {
            Destroy(Instantiate(prefabLedakanMeriam, target, Quaternion.identity), 1.6f);
            MainkanSuaraLedakan(target);
        }

        if (peluru != null) Destroy(peluru);
    }

    private IEnumerator ProsesSukses(BentengController benteng, string pesan)
    {
        string qId = $"S5-L3-S1-Q{soalTerjawab + 1:D2}";
        Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {soalTerjawab + 1}/{targetJumlahSoal}");
        Debug.Log($"[ANSWER]\nCorrect");
        GameStatisticsManager.CatatJawabanBenar(qId);

        TambahKoin(koinPerJawabanBenar);
        soalTerjawab++;

        teksIsiDialog.text = pesan + $"\n<color=red>+{koinPerJawabanBenar} Koin!</color>";
        yield return new WaitForSeconds(1.4f);

        if (prefabLedakanMeriam != null)
        {
            if (objekMeriam1 != null && objekMeriam1.activeSelf)
            {
                Vector3 posisiLedakan = objekMeriam1.transform.position;
                Destroy(Instantiate(prefabLedakanMeriam, posisiLedakan, Quaternion.identity), 1.5f);
                MainkanSuaraLedakan(posisiLedakan);
                objekMeriam1.SetActive(false);
            }
            if (objekMeriam2 != null && objekMeriam2.activeSelf)
            {
                Vector3 posisiLedakan = objekMeriam2.transform.position;
                Destroy(Instantiate(prefabLedakanMeriam, posisiLedakan, Quaternion.identity), 1.5f);
                MainkanSuaraLedakan(posisiLedakan);
                objekMeriam2.SetActive(false);
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (bankSoalAktif == null || bankSoalAktif.Count == 0)
        {
            // Semua soal selesai → panel evaluasi
            yield return StartCoroutine(TampilkanPanelEvaluasi());
        }
        else
        {
            SiapkanSoalBerikutnya();
            dialogIndex = 0;
            TampilkanTeks();
            panelDialogMisi.SetActive(true);
            TampilkanModeDialog();
        }
    }

    private IEnumerator ProsesDamageDanRetry(string pesan)
    {
        string qId = $"S5-L3-S1-Q{soalTerjawab + 1:D2}";
        Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {soalTerjawab + 1}/{targetJumlahSoal}");
        Debug.Log($"[ANSWER]\nWrong");
        GameStatisticsManager.CatatJawabanSalah(qId);

        if (panelFlashMerah != null) panelFlashMerah.SetActive(true);
        nyawaSaatIni--;
        UpdateUIHati();
        yield return new WaitForSeconds(0.3f);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);

        if (nyawaSaatIni <= 0)
        {
            teksIsiDialog.text = "<color=red>Ksatria gugur!</color>";
            yield return new WaitForSeconds(2f);

            // Nyawa habis → Menu_Pencapaian
            if (!string.IsNullOrEmpty(namaSceneGameOver))
                SceneManager.LoadScene(namaSceneGameOver);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            yield break;
        }

        teksIsiDialog.text = pesan;
        yield return new WaitForSeconds(2f);

        ResetSemuaBenteng();
        KembalikanPlayerKePosisiAwal();
        sedangProsesJawaban = false;

        dialogIndex = 0;
        TampilkanTeks();
        panelDialogMisi.SetActive(true);
        TampilkanModeDialog();

        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = true;
    }

    // =========================================================
    // PANEL EVALUASI
    // =========================================================
    private IEnumerator TampilkanPanelEvaluasi()
    {
        GameStatisticsManager.BerikanRewardLevel(5, 3, 1, 50);

        if (panelDialogMisi != null) panelDialogMisi.SetActive(false);
        SembunyikanSemuaKontrol();
        isDialogActive = false;

        // Matikan header
        if (panelHeader != null)
            panelHeader.SetActive(false);

        // Matikan UI lain
        if (uiLainYangDimatikan != null)
        {
            foreach (var ui in uiLainYangDimatikan)
            {
                if (ui != null) ui.SetActive(false);
            }
        }

        if (objekMeriam1 != null) objekMeriam1.SetActive(false);
        if (objekMeriam2 != null) objekMeriam2.SetActive(false);

        yield return new WaitForSeconds(0.35f);

        if (panelEvaluasi != null)
        {
            WinManager.TampilkanWin(panelEvaluasi);
            Debug.Log("<color=black>[EVALUASI]</color> Panel evaluasi ditampilkan");
        }
        else
        {
            WinManager.TampilkanWin();
        }
    }

    /// <summary>
    /// Dipanggil saat tombol Lanjut di panel evaluasi diklik
    /// </summary>
    public void LanjutKeStageBerikutnya()
    {
        WinManager.PindahKeMapCheckpoint();
    }

    private void KembalikanPlayerKePosisiAwal()
    {
        if (scriptPergerakanPlayer != null && posisiAwalPlayer != null)
            scriptPergerakanPlayer.transform.position = posisiAwalPlayer.position;
    }

    private void ResetSemuaBenteng()
    {
        if (tembokInstan != null)
        {
            Destroy(tembokInstan);
            tembokInstan = null;
        }

        if (daftarBenteng == null) return;

        for (int i = 0; i < daftarBenteng.Length; i++)
        {
            var b = daftarBenteng[i];
            if (b == null) continue;

            b.SetClosed();
            b.gameObject.SetActive(true);

            if (teksAngkaBenteng != null && i < teksAngkaBenteng.Length && teksAngkaBenteng[i] != null)
                teksAngkaBenteng[i].gameObject.SetActive(true);
        }
    }

    public void OnProjectileBlocked(ProjectileMeriam proj, BentengController benteng) { }
    public void OnProjectileFailed(ProjectileMeriam proj, BentengController benteng) { }
    public void OnProjectileHitPlayer(ProjectileMeriam proj) { }
}