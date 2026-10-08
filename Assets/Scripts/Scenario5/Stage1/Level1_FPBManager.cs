using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Level1_FPBManager : MonoBehaviour
{
    [Header("--- PENGATURAN LEVEL SCENE ---")]
    [Tooltip("Tentukan Sub Level saat ini: 1 (Mudah), 2 (Sedang), 3 (Sulit)")]
    public int subLevelSaatIni = 1;
    [Tooltip("Berapa kali pemain harus menjawab / jumlah benteng di scene ini?")]
    public int targetJumlahSoal = 10;

    [Header("--- PINDAH SCENE & PORTAL (2 MODE) ---")]
    [Tooltip("Ketikkan nama Scene berikutnya yang akan dimuat saat level selesai")]
    public string namaSceneBerikutnya;
    [Tooltip("Scene saat nyawa habis")]
    public string namaSceneGameOver = "Menu_Pencapaian";
    [Tooltip("Tarik objek portal KELUAR TERTUTUP ke sini (aktif selama soal belum selesai)")]
    public GameObject objekPortalTertutup;
    [Tooltip("Tarik objek portal KELUAR TERBUKA ke sini (aktif saat soal sudah selesai)")]
    public GameObject objekPortalTerbuka;
    [Tooltip("Titik posisi / Transform dari Portal Keluar")]
    public Transform titikPortalKeluar;

    [Header("--- REFERENSI UI DIALOG ---")]
    public GameObject panelDialogMisi;
    public TextMeshProUGUI teksIsiDialog;
    public GameObject tombolLanjut;

    [Header("--- REFERENSI MERIAM & LEDAKAN ---")]
    public GameObject objekMeriam1;
    public GameObject objekMeriam2;
    public TextMeshPro teksMeriam1;
    public TextMeshPro teksMeriam2;
    public Transform titikTembakMeriam1;
    public Transform titikTembakMeriam2;
    public GameObject prefabLedakanMeriam;
    public GameObject prefabPeluruApi;

    [Header("--- AUDIO LEDAKAN ---")]
    public AudioClip suaraLedakan;
    [Range(0f, 1f)] public float volumeLedakan = 0.9f;

    [Header("--- REFERENSI TOMBOL JAWABAN ---")]
    public GameObject grupTombolJawaban;
    public Button tombol1;
    public TextMeshProUGUI teksTombol1;
    public Button tombol2;
    public TextMeshProUGUI teksTombol2;
    public Button tombol3;
    public TextMeshProUGUI teksTombol3;
    public GameObject panelFlashMerah;

    [Header("--- SISTEM NYAWA & HATI ---")]
    public int nyawaMaksimal = 3;
    private int nyawaSaatIni;
    public Image[] ikonHatiUI;
    public Sprite spriteHatiUtuh;
    public Sprite spriteHatiRetak;

    [Header("--- SISTEM KOIN ---")]
    public int koinSaatIni = 0;
    public int koinPerJawabanBenar = 15;
    public TextMeshProUGUI teksKoinUI;

    [Header("--- REFERENSI GAMEPLAY ---")]
    public MonoBehaviour scriptPergerakanPlayer;
    public GameObject prefabBenteng;
    public Transform titikMunculBenteng;
    private GameObject bentengInstan;

    [Header("--- PENGATURAN ANIMASI TEMBAKAN ---")]
    public float kecepatanPeluru = 12f;
    public float delayAntarTembakan = 0.18f;
    public int sortingOrderPeluru = 50;

    // ================= INTERNAL =================
    private List<FPBQuestionBank.DataSoalFPB> bankSoalAktif = new List<FPBQuestionBank.DataSoalFPB>();
    private int soalTerjawab = 0;
    private FPBQuestionBank.DataSoalFPB soalAktif;

    private struct BarisDialog { public string isiTeks; }
    private BarisDialog[] alurDialog;

    private int dialogIndex = 0;
    private bool isDialogActive = false;
    private bool isIntroSudahTampil = false;
    private bool sedangModeIntro = false;
    private bool menungguTombolLanjutSelesai = false;

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

        // Amankan UI di awal permainan
        if (panelDialogMisi != null) panelDialogMisi.SetActive(false);
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);

        // Atur posisi dan status Portal
        if (objekPortalTertutup != null)
        {
            if (titikPortalKeluar != null) objekPortalTertutup.transform.position = titikPortalKeluar.position;
            objekPortalTertutup.SetActive(true);
        }
        if (objekPortalTerbuka != null)
        {
            if (titikPortalKeluar != null) objekPortalTerbuka.transform.position = titikPortalKeluar.position;
            objekPortalTerbuka.SetActive(false); // Terbuka nanti saat menang
        }

        if (teksMeriam1 != null) teksMeriam1.text = "";
        if (teksMeriam2 != null) teksMeriam2.text = "";

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

    public void LanjutkanSetelahVideo()
    {
        // Level 1 FPB dimulai melalui interaksi tutorial/NPC,
        // jadi kita tidak memaksa mulai dialog di sini.
        Debug.Log("Video selesai, menunggu interaksi player.");
    }

    private void UpdateUIHati()
    {
        if (ikonHatiUI == null || ikonHatiUI.Length == 0) return;
        for (int i = 0; i < ikonHatiUI.Length; i++)
        {
            if (ikonHatiUI[i] == null) continue;
            ikonHatiUI[i].sprite = (i < nyawaSaatIni) ? spriteHatiUtuh : spriteHatiRetak;
        }
    }

    private void UpdateUIKoin()
    {
        if (teksKoinUI != null) teksKoinUI.text = koinSaatIni.ToString();
    }

    public void TambahKoin(int jumlah)
    {
        koinSaatIni += jumlah;
        UpdateUIKoin();
        GameStatisticsManager.TambahKoin(jumlah, 5, 1, subLevelSaatIni);
    }

    private void MainkanSuaraLedakan(Vector3 posisi)
    {
        if (suaraLedakan == null) return;
        AudioSource.PlayClipAtPoint(
            suaraLedakan,
            Camera.main != null ? Camera.main.transform.position : posisi,
            volumeLedakan
        );
    }

    private void SiapkanBankSoal()
    {
        List<FPBQuestionBank.DataSoalFPB> sumberSoal = new List<FPBQuestionBank.DataSoalFPB>();

        if (subLevelSaatIni == 1) sumberSoal = FPBQuestionBank.GetSoalSubLevel1();
        else if (subLevelSaatIni == 2) sumberSoal = FPBQuestionBank.GetSoalSubLevel2();
        else if (subLevelSaatIni == 3) sumberSoal = FPBQuestionBank.GetSoalSubLevel3();
        else sumberSoal = FPBQuestionBank.GetSoalSubLevel1(); 

        AcakList(sumberSoal);
        if (targetJumlahSoal < sumberSoal.Count)
            sumberSoal = sumberSoal.GetRange(0, targetJumlahSoal);

        bankSoalAktif.AddRange(sumberSoal);
        Debug.Log($"<color=green>[Level 1 Manager]</color> {bankSoalAktif.Count} soal berhasil dimuat dari Sub-Level {subLevelSaatIni}.");
    }

    private void AcakList(List<FPBQuestionBank.DataSoalFPB> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var temp = list[i];
            int r = Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = temp;
        }
    }

    public void MulaiTutorial()
    {
        if (soalTerjawab >= targetJumlahSoal) return;

        if (scriptPergerakanPlayer != null)
        {
            scriptPergerakanPlayer.enabled = false;
            Rigidbody2D rb = scriptPergerakanPlayer.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        panelDialogMisi.SetActive(true);
        if (tombolLanjut != null) tombolLanjut.SetActive(true);
        
        // PASTIKAN TOMBOL MATI SAAT TUTORIAL DIMULAI
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);

        isDialogActive = true;
        dialogIndex = 0;
        menungguTombolLanjutSelesai = false;

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
        // PASTIKAN TOMBOL JAWABAN MATI SAAT INTRO
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);

        if (subLevelSaatIni == 1)
        {
            alurDialog = new BarisDialog[]
            {
                new BarisDialog { isiTeks = "Ksatria, kamu telah keluar dari goa! Namun awas, kita sudah memasuki Jalur Markas musuh." },
                new BarisDialog { isiTeks = "Pasukan Raja Anomali menembakkan meriam ganda. Kita butuh benteng dari kekuatan <color=red>FPB (Faktor Persekutuan Terbesar)</color>!" },
                new BarisDialog { isiTeks = "Pilih batu dengan angka FPB yang benar untuk memantulkan peluru!" }
            };
        }
        else if (subLevelSaatIni == 2)
        {
            alurDialog = new BarisDialog[]
            {
                new BarisDialog { isiTeks = "Ksatria, Sub-Level Sedang! Gempuran meriam semakin kuat." },
                new BarisDialog { isiTeks = "Gunakan <color=red>FPB</color> terbesar untuk memantulkan serangan mereka!" }
            };
        }
        else
        {
            alurDialog = new BarisDialog[]
            {
                new BarisDialog { isiTeks = "Sub-Level Sulit! Pertahanan terakhir sebelum masuk lebih dalam." },
                new BarisDialog { isiTeks = "Fokus cari persekutuan terbesarnya!" }
            };
        }
    }

    private void SiapkanSoalBerikutnya()
    {
        if (bankSoalAktif.Count == 0) return;

        if (bentengInstan != null) Destroy(bentengInstan);

        if (objekMeriam1 != null) objekMeriam1.SetActive(true);
        if (objekMeriam2 != null) objekMeriam2.SetActive(true);

        // KUNCI TOMBOL JAWABAN SAAT MENYIAPKAN SOAL BARU
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);

        soalAktif = bankSoalAktif[0];
        bankSoalAktif.RemoveAt(0);

        if (teksMeriam1 != null) teksMeriam1.text = soalAktif.angkaMeriam1.ToString();
        if (teksMeriam2 != null) teksMeriam2.text = soalAktif.angkaMeriam2.ToString();

        List<FPBQuestionBank.OpsiJawaban> pilihanAcak = new List<FPBQuestionBank.OpsiJawaban>(soalAktif.pilihanJawaban);
        for (int i = 0; i < pilihanAcak.Count; i++)
        {
            var temp = pilihanAcak[i];
            int r = Random.Range(i, pilihanAcak.Count);
            pilihanAcak[i] = pilihanAcak[r];
            pilihanAcak[r] = temp;
        }

        teksTombol1.text = pilihanAcak[0].angka.ToString();
        teksTombol2.text = pilihanAcak[1].angka.ToString();
        teksTombol3.text = pilihanAcak[2].angka.ToString();

        string teksPilihanSoal = $"Pilihan: <b>[A] {pilihanAcak[0].angka}   [B] {pilihanAcak[1].angka}   [C] {pilihanAcak[2].angka}</b>";

        alurDialog = new BarisDialog[]
        {
            new BarisDialog { isiTeks = $"<b>[Soal {soalTerjawab + 1}/{targetJumlahSoal}]</b>\nMeriam berangka <color=red>{soalAktif.angkaMeriam1}</color> dan <color=red>{soalAktif.angkaMeriam2}</color> menembak!\n\n{teksPilihanSoal}" },
            new BarisDialog { isiTeks = $"Faktor {soalAktif.angkaMeriam1}: <color=black>{soalAktif.teksFaktor1}</color>\nFaktor {soalAktif.angkaMeriam2}: <color=black>{soalAktif.teksFaktor2}</color>\n\n{teksPilihanSoal}" },
            new BarisDialog { isiTeks = $"Angka yang sama: <color=black>{soalAktif.teksFaktorSama}</color>\nPilih yang <color=red>PALING BESAR</color>!\n\n{teksPilihanSoal}" }
        };

        tombol1.onClick.RemoveAllListeners();
        tombol1.onClick.AddListener(() => CekJawaban(pilihanAcak[0]));
        tombol2.onClick.RemoveAllListeners();
        tombol2.onClick.AddListener(() => CekJawaban(pilihanAcak[1]));
        tombol3.onClick.RemoveAllListeners();
        tombol3.onClick.AddListener(() => CekJawaban(pilihanAcak[2]));
    }

    public void LanjutDialog()
    {
        if (!isDialogActive) return;

        if (menungguTombolLanjutSelesai)
        {
            StartCoroutine(ProsesAkhirKePortal());
            return;
        }

        dialogIndex++;

        if (dialogIndex < alurDialog.Length)
        {
            TampilkanTeks();
            // PASTIKAN TOMBOL TETAP MATI SELAMA DIALOG MASIH BERJALAN
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
                // PASTIKAN TOMBOL TETAP MATI SAAT BERALIH DARI INTRO KE SOAL PERTAMA
                if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);
            }
            else
            {
                // DIALOG SELESAI / MASUK TAHAP MENJAWAB:
                // TOMBOL LANJUT MATI, KELOMPOK TOMBOL JAWABAN BARU DITAMPILKAN!
                if (tombolLanjut != null) tombolLanjut.SetActive(false);
                if (grupTombolJawaban != null) grupTombolJawaban.SetActive(true);
                teksIsiDialog.text = "Pilih Batu FPB yang <color=red>PALING BESAR</color>!";
            }
        }
    }

    private void TampilkanTeks()
    {
        if (alurDialog == null || dialogIndex >= alurDialog.Length) return;
        teksIsiDialog.text = alurDialog[dialogIndex].isiTeks;
    }

    private void CekJawaban(FPBQuestionBank.OpsiJawaban jawaban)
    {
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);

        if (jawaban.isBenar)
            StartCoroutine(ProsesJawabanBenar(jawaban.feedbackTeks));
        else
            StartCoroutine(ProsesJawabanSalah(jawaban.feedbackTeks));
    }

    private IEnumerator TembakPeluru(Vector3 awal, Vector3 target, bool ledakanDiAkhir)
    {
        if (prefabPeluruApi == null) yield break;

        GameObject peluru = Instantiate(prefabPeluruApi, awal, Quaternion.identity);
        
        SpriteRenderer sr = peluru.GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = sortingOrderPeluru;
        else
        {
            sr = peluru.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = sortingOrderPeluru;
        }

        if (peluru.transform.localScale.x < 0.3f) peluru.transform.localScale = Vector3.one;

        Vector3 arah = (target - awal).normalized;
        float sudut = Mathf.Atan2(arah.y, arah.x) * Mathf.Rad2Deg;
        peluru.transform.rotation = Quaternion.Euler(0, 0, sudut);

        float jarak = Vector3.Distance(awal, target);
        float durasi = Mathf.Max(jarak / kecepatanPeluru, 0.35f);
        float waktu = 0f;

        while (waktu < durasi)
        {
            if (peluru == null) yield break;
            waktu += Time.deltaTime;
            peluru.transform.position = Vector3.Lerp(awal, target, waktu / durasi);
            yield return null;
        }

        if (peluru != null) peluru.transform.position = target;

        if (ledakanDiAkhir && prefabLedakanMeriam != null)
        {
            GameObject ledakan = Instantiate(prefabLedakanMeriam, target, Quaternion.identity);
            var srLedakan = ledakan.GetComponent<SpriteRenderer>();
            if (srLedakan != null) srLedakan.sortingOrder = sortingOrderPeluru + 1;
            var ps = ledakan.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(ledakan, 1.8f);
        }

        MainkanSuaraLedakan(target);

        if (peluru != null) Destroy(peluru, 0.05f);
    }

    private IEnumerator ProsesJawabanBenar(string pesan)
    {
        string qId = $"S5-L1-S{subLevelSaatIni}-Q{soalTerjawab + 1:D2}";
        Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {soalTerjawab + 1}/{targetJumlahSoal}");
        Debug.Log($"[ANSWER]\nCorrect");
        GameStatisticsManager.CatatJawabanBenar(qId);

        TambahKoin(koinPerJawabanBenar);
        teksIsiDialog.text = pesan + $"\n<color=red>+{koinPerJawabanBenar} Koin!</color>";

        Vector3 posisiBenteng = titikMunculBenteng != null ? titikMunculBenteng.position : Vector3.zero;

        if (prefabBenteng != null)
            bentengInstan = Instantiate(prefabBenteng, posisiBenteng, Quaternion.identity);

        yield return new WaitForSeconds(0.4f);

        Vector3 awal1 = titikTembakMeriam1 != null ? titikTembakMeriam1.position :
                        (objekMeriam1 != null ? objekMeriam1.transform.position : Vector3.zero);
        Vector3 awal2 = titikTembakMeriam2 != null ? titikTembakMeriam2.position :
                        (objekMeriam2 != null ? objekMeriam2.transform.position : Vector3.zero);

        if (objekMeriam1 != null && objekMeriam1.activeSelf)
            StartCoroutine(TembakPeluru(awal1, posisiBenteng, true));

        yield return new WaitForSeconds(delayAntarTembakan);

        if (objekMeriam2 != null && objekMeriam2.activeSelf)
            yield return StartCoroutine(TembakPeluru(awal2, posisiBenteng, true));
        else
            yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(0.35f);

        // Hancurkan musuh, munculkan efek ledakan, mainkan suara, lalu hancurkan efeknya sebelum soal reset
        if (prefabLedakanMeriam != null)
        {
            if (objekMeriam1 != null && objekMeriam1.activeSelf)
            {
                GameObject efek1 = Instantiate(prefabLedakanMeriam, objekMeriam1.transform.position, Quaternion.identity);
                MainkanSuaraLedakan(objekMeriam1.transform.position);
                Destroy(efek1, 0.9f); 
                objekMeriam1.SetActive(false);
            }
            if (objekMeriam2 != null && objekMeriam2.activeSelf)
            {
                GameObject efek2 = Instantiate(prefabLedakanMeriam, objekMeriam2.transform.position, Quaternion.identity);
                MainkanSuaraLedakan(objekMeriam2.transform.position);
                Destroy(efek2, 0.9f); 
                objekMeriam2.SetActive(false);
            }
        }

        yield return new WaitForSeconds(1.0f);

        panelDialogMisi.SetActive(false);
        isDialogActive = false;
        soalTerjawab++;

        if (soalTerjawab >= targetJumlahSoal)
        {
            GameStatisticsManager.BerikanRewardLevel(5, 1, subLevelSaatIni, 25);
            panelDialogMisi.SetActive(true);
            if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);
            teksIsiDialog.text = "<color=black>Luar Biasa!</color> Semua meriam berhasil dihancurkan.\nTekan <b>Lanjut</b> untuk membuka portal.";
            isDialogActive = true;
            menungguTombolLanjutSelesai = true;
            if (tombolLanjut != null) tombolLanjut.SetActive(true);
        }
        else
        {
            isDialogActive = true;
            SiapkanSoalBerikutnya(); 
            dialogIndex = 0;
            TampilkanTeks();
            panelDialogMisi.SetActive(true);
            if (tombolLanjut != null) tombolLanjut.SetActive(true);
            if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);
        }
    }

    private IEnumerator ProsesJawabanSalah(string pesan)
    {
        string qId = $"S5-L1-S{subLevelSaatIni}-Q{soalTerjawab + 1:D2}";
        Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {soalTerjawab + 1}/{targetJumlahSoal}");
        Debug.Log($"[ANSWER]\nWrong");
        GameStatisticsManager.CatatJawabanSalah(qId);

        Vector3 posisiPlayer = scriptPergerakanPlayer != null ? scriptPergerakanPlayer.transform.position : Vector3.zero;

        Vector3 awal1 = titikTembakMeriam1 != null ? titikTembakMeriam1.position :
                        (objekMeriam1 != null ? objekMeriam1.transform.position : Vector3.zero);
        Vector3 awal2 = titikTembakMeriam2 != null ? titikTembakMeriam2.position :
                        (objekMeriam2 != null ? objekMeriam2.transform.position : Vector3.zero);

        if (objekMeriam1 != null && objekMeriam1.activeSelf)
            StartCoroutine(TembakPeluru(awal1, posisiPlayer, true));

        yield return new WaitForSeconds(delayAntarTembakan);

        if (objekMeriam2 != null && objekMeriam2.activeSelf)
            yield return StartCoroutine(TembakPeluru(awal2, posisiPlayer, true));
        else
            yield return new WaitForSeconds(0.45f);

        if (panelFlashMerah != null) panelFlashMerah.SetActive(true);
        nyawaSaatIni--;
        UpdateUIHati();
        yield return new WaitForSeconds(0.3f);
        if (panelFlashMerah != null) panelFlashMerah.SetActive(false);

        if (nyawaSaatIni <= 0)
        {
            teksIsiDialog.text = "<color=red>Ksatria telah gugur!</color>";
            yield return new WaitForSeconds(2.0f);
            if (!string.IsNullOrEmpty(namaSceneGameOver))
                SceneManager.LoadScene(namaSceneGameOver);
            else
                SceneManager.LoadScene("Menu_Pencapaian");
            yield break;
        }

        teksIsiDialog.text = pesan;
        yield return new WaitForSeconds(2.3f);

        sedangModeIntro = false;
        isDialogActive = true;
        SiapkanSoalBerikutnya();
        dialogIndex = 0;
        TampilkanTeks();
        if (tombolLanjut != null) tombolLanjut.SetActive(true);
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false); 
    }

    private IEnumerator ProsesAkhirKePortal()
    {
        isDialogActive = false;
        if (panelDialogMisi != null) panelDialogMisi.SetActive(false);
        if (tombolLanjut != null) tombolLanjut.SetActive(false);
        if (grupTombolJawaban != null) grupTombolJawaban.SetActive(false);

        if (bentengInstan != null)
        {
            BentengController ctrl = bentengInstan.GetComponent<BentengController>();
            if (ctrl != null) ctrl.Open(true);
        }

        if (objekPortalTertutup != null) objekPortalTertutup.SetActive(false);
        if (objekPortalTerbuka != null)
        {
            if (titikPortalKeluar != null)
                objekPortalTerbuka.transform.position = titikPortalKeluar.position;
            objekPortalTerbuka.SetActive(true);
        }

        Transform playerTr = scriptPergerakanPlayer != null ? scriptPergerakanPlayer.transform : null;
        if (scriptPergerakanPlayer != null) scriptPergerakanPlayer.enabled = false;

        if (playerTr != null && titikPortalKeluar != null)
        {
            float durasi = 1.1f;
            float t = 0f;
            Vector3 awal = playerTr.position;
            Vector3 tujuan = titikPortalKeluar.position;

            while (t < 1f)
            {
                t += Time.deltaTime / durasi;
                playerTr.position = Vector3.Lerp(awal, tujuan, t);
                yield return null;
            }
            playerTr.position = tujuan;
        }

        yield return new WaitForSeconds(0.5f);

        // Tampilkan Win UI melalui WinManager
        WinManager.TampilkanWin();
    }

    public void OnProjectileHitPlayer(ProjectileMeriam proj) { }
    public void OnProjectileBlocked(ProjectileMeriam proj, BentengController benteng) { }
    public void OnProjectileFailed(ProjectileMeriam proj, BentengController benteng) { }
}