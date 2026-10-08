using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MesinSuhuInputController : MonoBehaviour
{
    [Header("Tingkat Kesulitan Boss")]
    public int subLevelMesin = 7; 
    public Color warnaTemaPanel = new Color(0.6f, 0.1f, 0.1f, 0.9f); 

    [Header("Pengaturan Jumlah Soal")]
    public int jumlahSoalDibutuhkan = 3; 
    private int soalTerjawabBenar = 0;

    [Header("Referensi Objek Fisik")]
    public GameObject objekPintuEs;
    public GameObject objekPortalKeluar;
    
    [Tooltip("Tarik objek Tulisan E (Bisa 3D Text atau UI) ke sini")]
    public TMP_Text teksTekanE; 
    
    public GameObject bgSalju;
    public GameObject bgHangat;

    [Header("Referensi UI Panel Boss (Input Field)")]
    public GameObject panelInputUI;
    public Image backgroundPanelUI;
    public TextMeshProUGUI UIKonteksSoal;    
    public TextMeshProUGUI UIPertanyaanRumus; 
    
    [Tooltip("Tarik InputField dari Canvas")]
    public TMP_InputField inputFieldJawaban; 
    
    [Tooltip("Tarik Tombol Submit (TombolOpsIA) ke sini")]
    public Button tombolSubmit; 
    
    [Tooltip("Tarik Tombol Lanjut ke sini")]
    public Button tombolUI_Lanjut;

    [Header("--- UI EVALUASI AKHIR (RAPOR) ---")]
    public GameObject panelEvaluasi;
    public TextMeshProUGUI teksHasilKoin;
    // Variabel teksHasilMateri masih dipertahankan jika Anda sewaktu-waktu butuh mengaturnya kembali, 
    // namun pengisian teks otomatisnya di bawah sudah dimatikan.
    public TextMeshProUGUI teksHasilMateri;

    [Header("--- CUTSCENE MENANG ---")]
    public GameObject playerKarakter;
    public Transform titikPortalKeluar;
    public string namaSceneBerikutnya = "Stage4_PemecahDinding";

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

    private void Start()
    {
        nyawaSaatIni = nyawaMaksimal;
        UpdateUIHati();
        UpdateUIKoin();

        if (teksTekanE != null) teksTekanE.gameObject.SetActive(false);
        if (panelInputUI != null) panelInputUI.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
        
        if (bgSalju != null) bgSalju.SetActive(true);
        if (bgHangat != null) bgHangat.SetActive(false);

        // Matikan Raycast Target pada teks supaya tidak menutupi tombol
        MatikanRaycastTeks();

        Invoke(nameof(AmbilSoalBaruDariDatabase), 0.2f);
    }

    private void MatikanRaycastTeks()
    {
        if (UIKonteksSoal != null) UIKonteksSoal.raycastTarget = false;
        if (UIPertanyaanRumus != null) UIPertanyaanRumus.raycastTarget = false;
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
        if (teksKoinUI != null) teksKoinUI.text = koinSaatIni.ToString();
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

        if (panelInputUI != null)
        {
            panelInputUI.SetActive(true);
            if (backgroundPanelUI != null) backgroundPanelUI.color = warnaTemaPanel;

            // Pastikan teks tidak menutupi tombol
            MatikanRaycastTeks();

            if (UIKonteksSoal != null) UIKonteksSoal.text = dataSoal.narasi;
            
            if (UIPertanyaanRumus != null) 
            { 
                UIPertanyaanRumus.gameObject.SetActive(true); 
                UIPertanyaanRumus.text = $"[ Soal {soalTerjawabBenar + 1} dari {jumlahSoalDibutuhkan} ]\n{dataSoal.pertanyaan}"; 
                UIPertanyaanRumus.color = Color.white; 
            }

            // --- INPUT FIELD ---
            if (inputFieldJawaban != null) 
            {
                inputFieldJawaban.gameObject.SetActive(true);
                inputFieldJawaban.interactable = true;
                inputFieldJawaban.text = ""; 
                StartCoroutine(FokusKeInputField());
            }

            // --- TOMBOL SUBMIT (TombolOpsIA) ---
            SetupTombolSubmit();

            // Tombol Lanjut disembunyikan dulu
            if (tombolUI_Lanjut != null) 
                tombolUI_Lanjut.gameObject.SetActive(false);
        }
    }

    private void SetupTombolSubmit()
    {
        if (tombolSubmit == null) return;

        tombolSubmit.gameObject.SetActive(true);
        tombolSubmit.interactable = true;

        // Bersihkan listener lama & pasang ulang
        tombolSubmit.onClick.RemoveAllListeners();
        tombolSubmit.onClick.AddListener(CekJawabanKetik);

        // Pindahkan tombol ke paling atas supaya tidak tertutup
        tombolSubmit.transform.SetAsLastSibling();
    }

    private IEnumerator FokusKeInputField()
    {
        yield return null;
        if (inputFieldJawaban != null && inputFieldJawaban.gameObject.activeInHierarchy)
        {
            inputFieldJawaban.Select();
            inputFieldJawaban.ActivateInputField();
        }
    }

    private void CekJawabanKetik()
    {
        if (inputFieldJawaban == null) return;

        string jawabanKetik = inputFieldJawaban.text.Trim();
        string jawabanBenarString = dataSoal.jawabanBenar.ToString();

        isJawabanTerakhirBenar = (jawabanKetik == jawabanBenarString);

        TampilkanPenjelasan();
    }

    private void TampilkanPenjelasan()
    {
        if (inputFieldJawaban != null) inputFieldJawaban.gameObject.SetActive(false);
        if (tombolSubmit != null) tombolSubmit.gameObject.SetActive(false);

        if (UIPertanyaanRumus != null) 
        {
            string qId = $"S3-L3-S1-Q{dataSoal.idSoal:D2}";
            if (isJawabanTerakhirBenar)
            {
                Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {dataSoal.idSoal}");
                Debug.Log($"[ANSWER]\nCorrect");
                GameStatisticsManager.CatatJawabanBenar(qId);

                UIPertanyaanRumus.text = $"JAWABAN BENAR! (+{hadiahKoinPerSoal} Koin)";
                UIPertanyaanRumus.color = Color.green;
            }
            else
            {
                Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {dataSoal.idSoal}");
                Debug.Log($"[ANSWER]\nWrong");
                GameStatisticsManager.CatatJawabanSalah(qId);

                UIPertanyaanRumus.text = "JAWABAN SALAH!";
                UIPertanyaanRumus.color = Color.red;
            }
        }

        if (UIKonteksSoal != null) 
            UIKonteksSoal.text = isJawabanTerakhirBenar ? dataSoal.penjelasanBenar : dataSoal.penjelasanSalah;

        // Setup tombol Lanjut
        if (tombolUI_Lanjut != null)
        {
            tombolUI_Lanjut.gameObject.SetActive(true);
            tombolUI_Lanjut.interactable = true;
            tombolUI_Lanjut.onClick.RemoveAllListeners();
            tombolUI_Lanjut.onClick.AddListener(TutupPanelDanEksekusi);
            tombolUI_Lanjut.transform.SetAsLastSibling();
        }
    }

    private void TutupPanelDanEksekusi()
    {
        if (panelInputUI != null) panelInputUI.SetActive(false);

        if (isJawabanTerakhirBenar) 
        {
            koinSaatIni += hadiahKoinPerSoal;
            UpdateUIKoin();
            GameStatisticsManager.TambahKoin(hadiahKoinPerSoal, 3, 3, 1);
            soalTerjawabBenar++;

            if (soalTerjawabBenar < jumlahSoalDibutuhkan)
            {
                AmbilSoalBaruDariDatabase();
                BukaPanelSoal();
            }
            else
            {
                mesinSudahSelesai = true;
                if (objekPintuEs != null) objekPintuEs.SetActive(false);
                if (objekPortalKeluar != null) objekPortalKeluar.SetActive(true);

                if (bgSalju != null) bgSalju.SetActive(false);
                if (bgHangat != null) bgHangat.SetActive(true);

                TampilkanMenuEvaluasiFinal();
            }
        }
        else 
        {
            nyawaSaatIni--;
            UpdateUIHati();

            if (nyawaSaatIni > 0)
            {
                if (teksTekanE != null)
                {
                    teksTekanE.gameObject.SetActive(true);
                    teksTekanE.text = "Sistem Overload! Hati retak 1. Rumus Diacak Ulang. Tekan 'E'.";
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
    }

    private void TampilkanMenuEvaluasiFinal()
    {
        if (playerKarakter != null)
        {
            Rigidbody2D rb = playerKarakter.GetComponent<Rigidbody2D>();
            if (rb != null) 
            { 
                rb.linearVelocity = Vector2.zero; 
                rb.bodyType = RigidbodyType2D.Kinematic; 
            }
        }

        if (panelEvaluasi != null) panelEvaluasi.SetActive(true);

        if (teksHasilKoin != null)
            teksHasilKoin.text = "Total Koin Didapat: " + koinSaatIni + " Koin";

        // Teks materi dihapus dari script agar tidak menimpa tulisan bawaan pada UI Inspector
    }

    public void TombolLanjutPindahScene()
    {
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
        StartCoroutine(CutsceneJalanKePortal());
    }

    private IEnumerator GameOverPindahScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Menu_Pencapaian"); 
    }

    private IEnumerator CutsceneJalanKePortal()
    {
        if (playerKarakter == null || titikPortalKeluar == null) yield break;

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

        Vector3 posisiTarget = titikPortalKeluar.position;
        posisiTarget.z = playerKarakter.transform.position.z; 

        while (Vector2.Distance(new Vector2(playerKarakter.transform.position.x, 0), new Vector2(posisiTarget.x, 0)) > 0.5f)
        {
            playerKarakter.transform.position = Vector3.MoveTowards(playerKarakter.transform.position, posisiTarget, 3f * Time.deltaTime);
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
                teksTekanE.text = "Tekan 'E' untuk masuk sistem"; 
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
            if (panelInputUI != null) 
                panelInputUI.SetActive(false);
        }
    }
}