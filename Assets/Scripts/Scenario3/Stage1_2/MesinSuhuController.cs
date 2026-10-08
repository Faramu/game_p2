using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MesinSuhuController : MonoBehaviour
{
    [Header("Tingkat Kesulitan Mesin")]
    public int subLevelMesin = 1;

    [Header("Pengaturan Jumlah Soal")]
    public int jumlahSoalDibutuhkan = 3;
    public Color warnaTemaPanel = new Color(0.1f, 0.2f, 0.4f, 0.9f);

    [Header("Referensi Objek Fisik (Pintu)")]
    public GameObject objekPintuEs;
    public GameObject objekPintuKayu;

    public TMP_Text teksTekanE;

    [Header("Perubahan Background (Opsional)")]
    public GameObject bgSalju;
    public GameObject bgHangat;

    [Header("Referensi UI Panel Soal (Pilihan Ganda)")]
    public GameObject panelSoalUI;
    public Image backgroundPanelUI;
    public TextMeshProUGUI UIKonteksSoal;
    public TextMeshProUGUI UIPertanyaanSoal;
    public TextMeshProUGUI UITeksTombolA;
    public TextMeshProUGUI UITeksTombolB;
    public Button tombolUI_A;
    public Button tombolUI_B;
    public Button tombolUI_Lanjut;

    [Header("--- PENGATURAN PINDAH LEVEL (CUTSCENE) ---")]
    public bool iniMesinTerakhir = false;
    public string namaSceneBerikutnya;
    public GameObject playerKarakter;
    public Transform titikPintuKeluar;
    public float kecepatanJalanCutscene = 3f;

    [Header("--- SISTEM NYAWA & KOIN ---")]
    public int nyawaMaksimal = 3;
    private int nyawaSaatIni;

    public int koinSaatIni = 0;
    public int hadiahKoinPerSoal = 10;
    public TextMeshProUGUI teksKoinUI;

    public Image[] ikonHatiUI = new Image[3];
    public Sprite spriteHatiUtuh;
    public Sprite spriteHatiRetak;

    private DataSoalSuhu dataSoal;
    private bool playerDiDekat = false;
    private bool mesinSudahSelesai = false;
    private bool isJawabanTerakhirBenar = false;
    private int nilaiJawabanTerakhir = 0;
    private int soalTerjawabBenar = 0;

    private void Start()
    {
        nyawaSaatIni = nyawaMaksimal;
        UpdateUIHati();
        UpdateUIKoin();

        if (teksTekanE != null) teksTekanE.gameObject.SetActive(false);
        if (panelSoalUI != null) panelSoalUI.SetActive(false);
        if (bgSalju != null) bgSalju.SetActive(true);
        if (bgHangat != null) bgHangat.SetActive(false);

        Invoke(nameof(AmbilSoalBaruDariDatabase), 0.2f);
    }

    private void UpdateUIHati()
    {
        if (ikonHatiUI == null || ikonHatiUI.Length == 0) return;

        for (int i = 0; i < ikonHatiUI.Length; i++)
        {
            if (ikonHatiUI[i] == null) continue;

            if (i < nyawaSaatIni)
            {
                ikonHatiUI[i].gameObject.SetActive(true);
                if (spriteHatiUtuh != null)
                    ikonHatiUI[i].sprite = spriteHatiUtuh;
            }
            else
            {
                if (spriteHatiRetak != null)
                {
                    ikonHatiUI[i].gameObject.SetActive(true);
                    ikonHatiUI[i].sprite = spriteHatiRetak;
                }
                else
                {
                    ikonHatiUI[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void UpdateUIKoin()
    {
        if (teksKoinUI != null)
            teksKoinUI.text = koinSaatIni.ToString();
    }

    private void AmbilSoalBaruDariDatabase()
    {
        if (Stage3Manager.Instance != null)
            dataSoal = Stage3Manager.Instance.AmbilSoalAcak(subLevelMesin);
    }

    private void Update()
    {
        if (playerDiDekat && !mesinSudahSelesai && Input.GetKeyDown(KeyCode.E))
        {
            if (dataSoal == null) AmbilSoalBaruDariDatabase();
            if (dataSoal != null) BukaPanelSoal();
        }
    }

    private void BukaPanelSoal()
    {
        if (teksTekanE != null) teksTekanE.gameObject.SetActive(false);

        // Aktifkan indikator suhu
        if (Stage3Manager.Instance != null)
            Stage3Manager.Instance.AktifkanPanelSuhu();

        if (panelSoalUI != null)
        {
            panelSoalUI.SetActive(true);
            if (backgroundPanelUI != null)
                backgroundPanelUI.color = warnaTemaPanel;

            if (UIKonteksSoal)
                UIKonteksSoal.text = dataSoal.narasi;

            if (UIPertanyaanSoal)
            {
                UIPertanyaanSoal.gameObject.SetActive(true);
                UIPertanyaanSoal.text = $"[ Soal {soalTerjawabBenar + 1} dari {jumlahSoalDibutuhkan} ]\n{dataSoal.pertanyaan}";
                UIPertanyaanSoal.color = Color.white;
            }

            bool tukarPosisi = (Random.value > 0.5f);

            if (tukarPosisi)
            {
                if (UITeksTombolA) UITeksTombolA.text = dataSoal.teksTombolB;
                if (UITeksTombolB) UITeksTombolB.text = dataSoal.teksTombolA;

                if (tombolUI_A)
                {
                    tombolUI_A.gameObject.SetActive(true);
                    tombolUI_A.onClick.RemoveAllListeners();
                    tombolUI_A.onClick.AddListener(() => TampilkanPenjelasan(dataSoal.nilaiB));
                }
                if (tombolUI_B)
                {
                    tombolUI_B.gameObject.SetActive(true);
                    tombolUI_B.onClick.RemoveAllListeners();
                    tombolUI_B.onClick.AddListener(() => TampilkanPenjelasan(dataSoal.nilaiA));
                }
            }
            else
            {
                if (UITeksTombolA) UITeksTombolA.text = dataSoal.teksTombolA;
                if (UITeksTombolB) UITeksTombolB.text = dataSoal.teksTombolB;

                if (tombolUI_A)
                {
                    tombolUI_A.gameObject.SetActive(true);
                    tombolUI_A.onClick.RemoveAllListeners();
                    tombolUI_A.onClick.AddListener(() => TampilkanPenjelasan(dataSoal.nilaiA));
                }
                if (tombolUI_B)
                {
                    tombolUI_B.gameObject.SetActive(true);
                    tombolUI_B.onClick.RemoveAllListeners();
                    tombolUI_B.onClick.AddListener(() => TampilkanPenjelasan(dataSoal.nilaiB));
                }
            }

            if (tombolUI_Lanjut)
                tombolUI_Lanjut.gameObject.SetActive(false);
        }
    }

    private void TampilkanPenjelasan(int nilaiOpsiDipilih)
    {
        nilaiJawabanTerakhir = nilaiOpsiDipilih;
        isJawabanTerakhirBenar = (nilaiOpsiDipilih == dataSoal.jawabanBenar);

        if (tombolUI_A) tombolUI_A.gameObject.SetActive(false);
        if (tombolUI_B) tombolUI_B.gameObject.SetActive(false);

        if (UIPertanyaanSoal)
        {
            UIPertanyaanSoal.gameObject.SetActive(true);
            int lvl = Stage3Manager.Instance != null ? ((int)Stage3Manager.Instance.levelAktif + 1) : 1;
            string qId = $"S3-L{lvl}-Q{dataSoal.idSoal:D2}";

            if (isJawabanTerakhirBenar)
            {
                Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {dataSoal.idSoal}");
                Debug.Log($"[ANSWER]\nCorrect");
                GameStatisticsManager.CatatJawabanBenar(qId);

                UIPertanyaanSoal.text = $"JAWABAN BENAR! (+{hadiahKoinPerSoal} Koin)";
                UIPertanyaanSoal.color = Color.green;
            }
            else
            {
                Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {dataSoal.idSoal}");
                Debug.Log($"[ANSWER]\nWrong");
                GameStatisticsManager.CatatJawabanSalah(qId);

                UIPertanyaanSoal.text = "JAWABAN SALAH!";
                UIPertanyaanSoal.color = Color.red;
            }
        }

        if (UIKonteksSoal)
            UIKonteksSoal.text = isJawabanTerakhirBenar ? dataSoal.penjelasanBenar : dataSoal.penjelasanSalah;

        if (tombolUI_Lanjut != null)
        {
            tombolUI_Lanjut.gameObject.SetActive(true);
            tombolUI_Lanjut.onClick.RemoveAllListeners();
            tombolUI_Lanjut.onClick.AddListener(TutupPanelDanEksekusi);
        }
    }

    private void TutupPanelDanEksekusi()
    {
        if (isJawabanTerakhirBenar)
        {
            soalTerjawabBenar++;

            // Tambah koin
            koinSaatIni += hadiahKoinPerSoal;
            UpdateUIKoin();
            int stgLvl = Stage3Manager.Instance != null ? ((int)Stage3Manager.Instance.levelAktif + 1) : 1;
            GameStatisticsManager.TambahKoin(hadiahKoinPerSoal, 3, stgLvl, 1);

            // ===== SUHU NAIK KARENA BENAR =====
            if (Stage3Manager.Instance != null)
                Stage3Manager.Instance.NaikkanSuhu();

            if (soalTerjawabBenar < jumlahSoalDibutuhkan)
            {
                AmbilSoalBaruDariDatabase();
                BukaPanelSoal();
            }
            else
            {
                if (panelSoalUI != null) panelSoalUI.SetActive(false);
                EksekusiBerhasil();
                if (iniMesinTerakhir)
                    StartCoroutine(CutsceneJalanKePintu());
            }
        }
        else
        {
            // ===== SUHU TURUN KARENA SALAH =====
            if (Stage3Manager.Instance != null)
                Stage3Manager.Instance.TurunkanSuhu();

            if (panelSoalUI != null) panelSoalUI.SetActive(false);
            EksekusiGagal(nilaiJawabanTerakhir);
        }
    }

    private void EksekusiBerhasil()
    {
        mesinSudahSelesai = true;

        if (objekPintuEs != null) objekPintuEs.SetActive(false);
        if (objekPintuKayu != null) objekPintuKayu.SetActive(true);

        if (bgSalju != null) bgSalju.SetActive(false);
        if (bgHangat != null) bgHangat.SetActive(true);

        if (teksTekanE != null && !iniMesinTerakhir)
        {
            teksTekanE.gameObject.SetActive(true);
            teksTekanE.text = "Berhasil! Suhu kembali normal.";
            teksTekanE.color = Color.green;
        }
    }

    private void EksekusiGagal(int nilaiOperasi)
    {
        nyawaSaatIni--;
        UpdateUIHati();

        if (nyawaSaatIni > 0)
        {
            if (teksTekanE != null)
            {
                teksTekanE.gameObject.SetActive(true);
                teksTekanE.text = "Suhu Kacau! Hati retak 1. Tekan 'E' untuk coba lagi.";
                teksTekanE.color = Color.red;
            }
            AmbilSoalBaruDariDatabase();
        }
        else
        {
            if (teksTekanE != null)
            {
                teksTekanE.gameObject.SetActive(true);
                teksTekanE.text = "GAME OVER! Nyawamu habis.";
                teksTekanE.color = Color.red;
            }
            mesinSudahSelesai = true;
            StartCoroutine(GameOverPindahScene());
        }
    }

    private IEnumerator GameOverPindahScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Menu_Pencapaian");
    }

    private IEnumerator CutsceneJalanKePintu()
    {
        if (playerKarakter == null || titikPintuKeluar == null) yield break;

        Rigidbody2D rb = playerKarakter.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D col = playerKarakter.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        MonoBehaviour[] scripts = playerKarakter.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null && script != this &&
                !script.GetType().Name.Contains("Animator") &&
                !script.GetType().Name.Contains("Transform"))
            {
                script.enabled = false;
            }
        }

        Vector3 posisiTarget = titikPintuKeluar.position;
        posisiTarget.z = playerKarakter.transform.position.z;

        while (Vector2.Distance(
            new Vector2(playerKarakter.transform.position.x, 0),
            new Vector2(posisiTarget.x, 0)) > 0.5f)
        {
            playerKarakter.transform.position = Vector3.MoveTowards(
                playerKarakter.transform.position,
                posisiTarget,
                kecepatanJalanCutscene * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Tampilkan Win UI melalui WinManager
        WinManager.TampilkanWin();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !mesinSudahSelesai)
        {
            playerDiDekat = true;
            if (teksTekanE != null)
            {
                teksTekanE.gameObject.SetActive(true);
                teksTekanE.text = "Tekan 'E' untuk aktifkan mesin";
                teksTekanE.color = Color.white;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerDiDekat = false;
            if (teksTekanE != null && !mesinSudahSelesai)
                teksTekanE.gameObject.SetActive(false);

            if (panelSoalUI != null)
                panelSoalUI.SetActive(false);
        }
    }
}