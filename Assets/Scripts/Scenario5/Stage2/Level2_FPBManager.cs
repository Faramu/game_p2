using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Level2_FPBManager : MonoBehaviour
{
    [Header("--- PENGATURAN LEVEL SCENE ---")]
    public int levelSaatIni = 1;
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

    [Header("--- TOMBOL JAWABAN ---")]
    public GameObject grupTombolJawaban;
    public Button tombol1;
    public TextMeshProUGUI teksTombol1;
    public Button tombol2;
    public TextMeshProUGUI teksTombol2;
    public Button tombol3;
    public TextMeshProUGUI teksTombol3;
    public GameObject panelFlashMerah;

    [Header("--- SISTEM NYAWA ---")]
    public int nyawaMaksimal = 3;
    private int nyawaSaatIni;
    public Image[] ikonHatiUI;
    public Sprite spriteHatiUtuh;
    public Sprite spriteHatiRetak;

    [Header("--- SISTEM KOIN ---")]
    public int koinSaatIni = 0;
    public int koinPerJawabanBenar = 15;
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
    public string namaSceneBerikutnya;
    [Tooltip("Scene saat nyawa habis")]
    public string namaSceneGameOver = "Menu_Pencapaian";
    public GameObject objekPortalKeluar;

    // ================= INTERNAL =================
    private List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor> bankSoalAktif =
        new List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor>();
    private Level2_PohonFaktorDatabase.DataSoalPohonFaktor soalAktif;

    private int soalTerjawab = 0;
    private struct BarisDialog { public string isiTeks; }
    private BarisDialog[] alurDialog;

    private int dialogIndex = 0;
    private bool isDialogActive = false;
    private bool isIntroSudahTampil = false;
    private bool sedangModeIntro = false;

    private void MainkanSuaraLedakan(Vector3 posisi)
    {
        if (suaraLedakan == null) return;
        Vector3 posisiAudio = Camera.main != null ? Camera.main.transform.position : posisi;
        AudioSource.PlayClipAtPoint(suaraLedakan, posisiAudio, volumeLedakan);
    }

    void Awake()
    {
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);
        if (panelDialogMisi != null) panelDialogMisi.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);
    }

    void Start()
    {
        nyawaSaatIni = nyawaMaksimal;
        UpdateUIHati();
        UpdateUIKoin();
        SiapkanBankSoal();

        if (panelDialogMisi != null) panelDialogMisi.SetActive(false);
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);
        if (objekPortalKeluar != null) objekPortalKeluar.SetActive(false);

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
        GameStatisticsManager.TambahKoin(jumlah, 5, 2, levelSaatIni);
    }

    private void SiapkanBankSoal()
    {
        List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor> sumberSoal =
            new List<Level2_PohonFaktorDatabase.DataSoalPohonFaktor>();

        if (levelSaatIni == 1) sumberSoal = Level2_PohonFaktorDatabase.GetSoalLevel1();
        else if (levelSaatIni == 2) sumberSoal = Level2_PohonFaktorDatabase.GetSoalLevel2();
        else if (levelSaatIni == 3) sumberSoal = Level2_PohonFaktorDatabase.GetSoalLevel3();

        bankSoalAktif = AmbilSoal(sumberSoal, targetJumlahSoal);
        Debug.Log($"[SOAL] Level {levelSaatIni} → {bankSoalAktif.Count} soal");
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

    public void MulaiTutorial()
    {
        KembalikanPlayerKePosisiAwal();

        if (scriptPergerakanPlayer != null)
        {
            scriptPergerakanPlayer.enabled = false;
            var rb = scriptPergerakanPlayer.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        panelDialogMisi.SetActive(true);
        grupTombolJawaban.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(true);

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
            new BarisDialog { isiTeks = $"Ksatria, selamat datang di Level {levelSaatIni}!\nAwas, ada serangan meriam dari pasukan anomali.\nKita butuh benteng <color=red>FPB</color> untuk menahannya!" },
            new BarisDialog { isiTeks = "Faktor adalah bilangan yang dapat membagi habis bilangan lain tanpa sisa." },
            new BarisDialog { isiTeks = "FPB adalah angka persekutuan yang <color=red>PALING BESAR</color> di antara kedua faktor meriam." },
            new BarisDialog { isiTeks = "Pilih batu dengan angka FPB yang benar untuk membuka benteng pelindung!" }
        };
    }

    private void SiapkanSoalBerikutnya()
    {
        KembalikanPlayerKePosisiAwal();
        ResetSemuaBenteng();

        // Kunci tombol jawaban saat menyiapkan soal baru
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);

        if (bankSoalAktif == null || bankSoalAktif.Count == 0)
        {
            if (daftarBenteng != null && daftarBenteng.Length > 0)
                StartCoroutine(MunculkanPortalDanPindah(daftarBenteng[0]));
            return;
        }

        soalAktif = bankSoalAktif[0];
        bankSoalAktif.RemoveAt(0);

        if (teksMeriam1 != null) teksMeriam1.text = soalAktif.angkaApi.ToString();
        if (teksMeriam2 != null) teksMeriam2.text = soalAktif.angkaEs.ToString();
        if (objekMeriam1 != null) objekMeriam1.SetActive(true);
        if (objekMeriam2 != null) objekMeriam2.SetActive(true);

        List<Level2_PohonFaktorDatabase.OpsiJawaban> pilihanAcak =
            new List<Level2_PohonFaktorDatabase.OpsiJawaban>();

        foreach (var o in soalAktif.pilihanJawaban)
        {
            pilihanAcak.Add(new Level2_PohonFaktorDatabase.OpsiJawaban
            {
                angka = o.angka,
                isBenar = o.isBenar,
                feedbackTeks = o.feedbackTeks
            });
        }

        for (int i = 0; i < pilihanAcak.Count; i++)
        {
            var temp = pilihanAcak[i];
            int r = Random.Range(i, pilihanAcak.Count);
            pilihanAcak[i] = pilihanAcak[r];
            pilihanAcak[r] = temp;
        }

        string teksPilihanSoal = $"Pilihan: <b>[A] {pilihanAcak[0].angka}   [B] {pilihanAcak[1].angka}   [C] {pilihanAcak[2].angka}</b>";

        alurDialog = new BarisDialog[]
        {
            new BarisDialog { isiTeks =
                $"<b>[Soal {soalTerjawab + 1}/{targetJumlahSoal}]</b>\n" +
                $"Meriam menyerang dengan angka:\n" +
                $"<color=red>{soalAktif.angkaApi}</color>   &   <color=red>{soalAktif.angkaEs}</color>\n" +
                $"Tentukan <color=red>FPB</color>-nya!\n\n{teksPilihanSoal}" },

            new BarisDialog { isiTeks =
                $"Faktor <color=red>{soalAktif.angkaApi}</color>:\n<color=black>{soalAktif.pohonFaktorApi}</color>\n" +
                $"Faktor <color=red>{soalAktif.angkaEs}</color>:\n<color=black>{soalAktif.pohonFaktorEs}</color>\n\n{teksPilihanSoal}" },

            new BarisDialog { isiTeks =
                $"Faktor yang sama:\n<color=black>(Lihat faktor di atas)</color>\n" +
                $"Pilih yang <color=red>PALING BESAR</color>!\n\n{teksPilihanSoal}" }
        };

        teksTombol1.text = pilihanAcak[0].angka.ToString();
        teksTombol2.text = pilihanAcak[1].angka.ToString();
        teksTombol3.text = pilihanAcak[2].angka.ToString();

        if (teksAngkaBenteng != null && teksAngkaBenteng.Length >= 3)
        {
            if (teksAngkaBenteng[0] != null)
            {
                teksAngkaBenteng[0].gameObject.SetActive(true);
                teksAngkaBenteng[0].text = pilihanAcak[0].angka.ToString();
            }
            if (teksAngkaBenteng[1] != null)
            {
                teksAngkaBenteng[1].gameObject.SetActive(true);
                teksAngkaBenteng[1].text = pilihanAcak[1].angka.ToString();
            }
            if (teksAngkaBenteng[2] != null)
            {
                teksAngkaBenteng[2].gameObject.SetActive(true);
                teksAngkaBenteng[2].text = pilihanAcak[2].angka.ToString();
            }
        }

        tombol1.onClick.RemoveAllListeners();
        tombol1.onClick.AddListener(() => ProsesPilihJawaban(pilihanAcak[0], 0));
        tombol2.onClick.RemoveAllListeners();
        tombol2.onClick.AddListener(() => ProsesPilihJawaban(pilihanAcak[1], 1));
        tombol3.onClick.RemoveAllListeners();
        tombol3.onClick.AddListener(() => ProsesPilihJawaban(pilihanAcak[2], 2));
    }

    public void LanjutDialog()
    {
        if (!isDialogActive) return;

        dialogIndex++;

        if (dialogIndex < alurDialog.Length)
        {
            TampilkanTeks();
            // Pastikan tombol jawaban tetap OFF selama dialog masih berjalan
            if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);
        }
        else
        {
            if (sedangModeIntro)
            {
                isIntroSudahTampil = true;
                sedangModeIntro = false;
                SiapkanSoalBerikutnya();
                dialogIndex = 0;
                TampilkanTeks();
                if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);
            }
            else
            {
                TampilkanPilihanJawaban();
            }
        }
    }

    private void TampilkanTeks()
    {
        if (alurDialog == null || dialogIndex >= alurDialog.Length) return;
        teksIsiDialog.text = alurDialog[dialogIndex].isiTeks;
    }

    private void TampilkanPilihanJawaban()
    {
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(true);

        teksIsiDialog.text =
            $"Meriam: <color=red>{soalAktif.angkaApi}</color> & <color=red>{soalAktif.angkaEs}</color>\n" +
            $"Pilih <color=red>FPB</color> yang paling besar!";
    }

    private void ProsesPilihJawaban(Level2_PohonFaktorDatabase.OpsiJawaban jawaban, int indexTombol)
    {
        if (daftarBenteng == null || indexTombol >= daftarBenteng.Length || daftarBenteng[indexTombol] == null)
            return;

        grupTombolJawaban.SetActive(false);
        StartCoroutine(AlurBenteng(daftarBenteng[indexTombol], jawaban.isBenar, jawaban.feedbackTeks));
    }

    // =========================================================
    // ALUR UTAMA
    // =========================================================
    private IEnumerator AlurBenteng(BentengController bentengDipilih, bool isBenar, string feedback)
    {
        // 1. Kunci player
        if (scriptPergerakanPlayer != null)
        {
            scriptPergerakanPlayer.enabled = false;
            var rb = scriptPergerakanPlayer.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        // 2. Pindah ke benteng
        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.transform.position = bentengDipilih.GetPlayerPosition();

        yield return new WaitForSeconds(delaySetelahTeleport);

        // 3. Buka benteng yang dipilih
        bentengDipilih.Open(isBenar);

        // 4. Munculkan TEMBOK hanya jika BENAR
        if (isBenar)
        {
            if (prefabTembokPelindung != null)
            {
                if (tembokInstan != null) Destroy(tembokInstan);

                Vector3 posTembok = bentengDipilih.GetTitikMunculTembok();
                tembokInstan = Instantiate(prefabTembokPelindung, posTembok, Quaternion.identity);
                tembokInstan.name = "TembokPelindung_Temp";
                tembokInstan.SetActive(true);

                Debug.Log($"<color=black>[TEMBOK]</color> Muncul di {posTembok}");
            }
            else
            {
                Debug.LogError("[TEMBOK] prefabTembokPelindung = NULL!");
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

        // 5. Ledakan benteng yang TIDAK dipilih
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

        // 6. Target tembakan
        Vector3 targetPos;
        if (isBenar && tembokInstan != null)
        {
            targetPos = tembokInstan.transform.position;
            Debug.Log($"<color=black>[TEMBAK]</color> Target = TEMBOK di {targetPos}");
        }
        else
        {
            targetPos = scriptPergerakanPlayer != null
                ? scriptPergerakanPlayer.transform.position
                : bentengDipilih.transform.position;
            Debug.Log($"<color=red>[TEMBAK]</color> Jawaban salah, target = PLAYER di {targetPos}");
        }

        Vector3 awal1 = titikMunculPeluru1 != null ? titikMunculPeluru1.position :
                        (objekMeriam1 != null ? objekMeriam1.transform.position : Vector3.zero);
        Vector3 awal2 = titikMunculPeluru2 != null ? titikMunculPeluru2.position :
                        (objekMeriam2 != null ? objekMeriam2.transform.position : Vector3.zero);

        Debug.Log($"[TEMBAK] awal1={awal1} | awal2={awal2}");
        Debug.Log($"[TEMBAK] prefabPeluruApi = {(prefabPeluruApi != null ? prefabPeluruApi.name : "NULL!")}");

        // 7. Tembak peluru
        if (prefabPeluruApi == null)
        {
            Debug.LogError("[TEMBAK] prefabPeluruApi masih NULL!");
        }
        else
        {
            if (objekMeriam1 != null && objekMeriam1.activeInHierarchy)
                yield return StartCoroutine(TembakPeluruManual(awal1, targetPos));
            else
                Debug.LogWarning("[TEMBAK] objekMeriam1 tidak aktif");

            if (objekMeriam2 != null && objekMeriam2.activeInHierarchy)
                yield return StartCoroutine(TembakPeluruManual(awal2, targetPos));
            else
                Debug.LogWarning("[TEMBAK] objekMeriam2 tidak aktif");
        }

        yield return new WaitForSeconds(0.3f);

        // 8. Hasil
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

        Debug.Log($"[TEMBAK] Peluru terbang {awal} → {target} (durasi {durasi:F2}s)");

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
            GameObject ledakan = Instantiate(prefabLedakanMeriam, target, Quaternion.identity);
            Destroy(ledakan, 1.6f);
            MainkanSuaraLedakan(target);
            Debug.Log($"<color=red>[LEDAKAN]</color> di target {target}");
        }

        if (peluru != null) Destroy(peluru);
    }

    private IEnumerator ProsesSukses(BentengController benteng, string pesan)
    {
        string qId = $"S5-L2-S{levelSaatIni}-Q{soalTerjawab + 1:D2}";
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
            GameStatisticsManager.BerikanRewardLevel(5, 2, levelSaatIni, 25);
            panelDialogMisi.SetActive(false);
            isDialogActive = false;
            StartCoroutine(MunculkanPortalDanPindah(benteng));
        }
        else
        {
            SiapkanSoalBerikutnya();
            dialogIndex = 0;
            TampilkanTeks();
            panelDialogMisi.SetActive(true);
            if (tombolLanjut != null) tombolLanjut.SetActive(true);
        }
    }

    private IEnumerator ProsesDamageDanRetry(string pesan)
    {
        string qId = $"S5-L2-S{levelSaatIni}-Q{soalTerjawab + 1:D2}";
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
            if (!string.IsNullOrEmpty(namaSceneGameOver))
                SceneManager.LoadScene(namaSceneGameOver);
            else
                SceneManager.LoadScene("Menu_Pencapaian");
            yield break;
        }

        teksIsiDialog.text = pesan;
        yield return new WaitForSeconds(2f);

        ResetSemuaBenteng();
        KembalikanPlayerKePosisiAwal();

        dialogIndex = 0;
        TampilkanTeks();
        panelDialogMisi.SetActive(true);
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);

        if (tombolLanjut != null) tombolLanjut.SetActive(true);
        if (scriptPergerakanPlayer != null)
            scriptPergerakanPlayer.enabled = true;
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

    private IEnumerator MunculkanPortalDanPindah(BentengController bentengTerakhir)
    {
        if (objekPortalKeluar != null)
        {
            objekPortalKeluar.transform.position = bentengTerakhir.GetPlayerPosition();
            objekPortalKeluar.SetActive(true);
        }

        yield return new WaitForSeconds(0.8f);

        // Tampilkan Win UI melalui WinManager
        WinManager.TampilkanWin();
    }

    // Stub agar tidak error jika masih ada referensi lama
    public void OnProjectileBlocked(ProjectileMeriam proj, BentengController benteng) { }
    public void OnProjectileFailed(ProjectileMeriam proj, BentengController benteng) { }
    public void OnProjectileHitPlayer(ProjectileMeriam proj) { }
}