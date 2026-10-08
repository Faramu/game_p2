using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NagaBattleGarisBilanganManager : MonoBehaviour
{
    public enum GameState { DialogAwal, Bertempur, Menang, Kalah }
    public GameState currentState = GameState.DialogAwal;

    [System.Serializable]
    public class Soal
    {
        [TextArea(2, 3)] public string narasi;
        public int posisiPlayer;
        public int posisiMusuhAwal;
        public int posisiMusuh;
        public int angkaTarget;
        [TextArea(2, 3)] public string penjelasan;
    }

    [Header("--- SISTEM LEVEL & SOAL ---")]
    public int levelSaatIni = 1;
    private List<Soal> bankSoal = new List<Soal>();
    private List<Soal> sisaSoal = new List<Soal>();
    private Soal soalAktif;

    [Header("--- UI YANG HARUS DIMATIKAN SAAT MENANG ---")]
    public GameObject[] uiUntukDimatikan;

    [Header("--- PENGATURAN DIALOG PER LEVEL ---")]
    public GameObject panelDialog;
    public TextMeshProUGUI textDialog;

    [TextArea(2, 3)]
    public string[] dialogLevel1 = {
        "Ksatria! Boss Naga telah menghadang di Garis Bilangan!",
        "Posisi awalmu adalah Titik 0. Ingat: Kanan bernilai Positif (+), Kiri bernilai Negatif (-).",
        "Ketik berapa langkah bola api harus melesat untuk mengenai target koordinatnya!"
    };

    [TextArea(2, 3)]
    public string[] dialogLevel2 = {
        "Pertarungan semakin sengit! Sekarang kamu tidak selalu mulai dari Titik 0.",
        "Perhatikan di titik mana kamu berpijak, lalu hitung JARAK menuju Boss Naga!",
        "Rumus Jarak: Titik Musuh dikurangi Titik Ksatria. Jangan sampai salah arah (+/-)!"
    };

    [TextArea(2, 3)]
    public string[] dialogLevel3 = {
        "Peringatan! Boss Naga menggunakan sihir manipulasi posisi!",
        "Naga itu akan berpindah-pindah sebelum diam di satu titik. Cari tahu posisi akhir naga dulu!",
        "Setelah tahu posisi akhir naga, hitung JARAK tembakan dari posisimu saat ini!"
    };

    [TextArea(2, 3)]
    public string[] dialogMenang = {
        "Hebat! Boss Naga telah berhasil dikalahkan!",
        "Sekarang berjalanlah ke arah <b>KIRI</b> untuk melanjutkan misi berikutnya!"
    };

    private string[] dialogAktif;
    private int currentPage = 0;
    private bool isDialogMenang = false;

    [Header("--- SISTEM SKOR & NYAWA ---")]
    public int targetSkorUntukMenang = 10;
    private int skorSaatIni = 0;
    public int nyawaMaksimal = 3;
    private int nyawaSaatIni;
    private bool sudahCatatBosKalah = false;

    [Header("--- PENGATURAN UI HATI ---")]
    public Image[] ikonHatiUI;
    public Sprite spriteHatiUtuh;
    public Sprite spriteHatiRetak;

    [Header("--- REFERENSI OBJEK (Wajib Diisi) ---")]
    public Transform playerTransform;
    public Transform bossNagaTransform;
    public GameObject bayanganNaga;
    public Transform titikTembakPlayer;
    public Transform titikTembakNaga;
    public GameObject bolaApiMaster;

    [Header("--- REFERENSI UI HUD ---")]
    public TextMeshProUGUI teksMisi;
    public TextMeshProUGUI teksFeedback;
    public TextMeshProUGUI teksSkor;
    public TMP_InputField inputField;
    public GameObject tombolSerang;

    [Header("--- REFERENSI UI EVALUASI AKHIR (LEVEL 3) ---")]
    public GameObject panelEvaluasi;

    private PlankBehavior[] semuaPapan;
    private bool sedangProsesSerangan = false;
    private Coroutine animasiNagaCoroutine;

    void Start()
    {
        nyawaSaatIni = nyawaMaksimal;
        skorSaatIni = 0;

        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
        UpdateUIHati();

        semuaPapan = Object.FindObjectsByType<PlankBehavior>(FindObjectsSortMode.None);
        if (bolaApiMaster != null) bolaApiMaster.SetActive(false);
        if (teksFeedback != null) teksFeedback.text = "";

        MuatBankSoal(levelSaatIni);
        StartDialog();
    }

    void StartDialog()
    {
        currentState = GameState.DialogAwal;
        currentPage = 0;
        isDialogMenang = false;

        if (teksMisi != null && teksMisi != textDialog) teksMisi.gameObject.SetActive(false);
        if (teksFeedback != null) teksFeedback.gameObject.SetActive(false);
        if (teksSkor != null) teksSkor.gameObject.SetActive(false);
        if (inputField != null) inputField.gameObject.SetActive(false);
        if (tombolSerang != null) tombolSerang.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);

        if (levelSaatIni == 1) dialogAktif = dialogLevel1;
        else if (levelSaatIni == 2) dialogAktif = dialogLevel2;
        else if (levelSaatIni == 3) dialogAktif = dialogLevel3;
        else dialogAktif = dialogLevel1;

        if (panelDialog != null) panelDialog.SetActive(true);
        if (textDialog != null)
        {
            textDialog.gameObject.SetActive(true);
            if (dialogAktif != null && dialogAktif.Length > 0)
                textDialog.text = dialogAktif[0];
        }
    }

    void StartDialogMenang()
    {
        currentState = GameState.Menang;
        currentPage = 0;
        isDialogMenang = true;
        dialogAktif = dialogMenang;

        if (teksMisi != null) teksMisi.gameObject.SetActive(false);
        if (teksFeedback != null) teksFeedback.gameObject.SetActive(false);
        if (teksSkor != null) teksSkor.gameObject.SetActive(false);
        if (inputField != null) inputField.gameObject.SetActive(false);
        if (tombolSerang != null) tombolSerang.SetActive(false);

        if (uiUntukDimatikan != null)
        {
            foreach (var ui in uiUntukDimatikan)
                if (ui) ui.SetActive(false);
        }

        if (bossNagaTransform) bossNagaTransform.gameObject.SetActive(false);
        if (bayanganNaga) bayanganNaga.SetActive(false);

        if (panelDialog != null) panelDialog.SetActive(true);
        if (textDialog != null)
        {
            textDialog.gameObject.SetActive(true);
            if (dialogAktif != null && dialogAktif.Length > 0)
                textDialog.text = dialogAktif[0];
        }
    }

    public void KlikTombolLanjut()
    {
        currentPage++;

        if (dialogAktif != null && currentPage < dialogAktif.Length)
        {
            if (textDialog != null)
            {
                textDialog.gameObject.SetActive(true);
                textDialog.text = dialogAktif[currentPage];
            }
        }
        else
        {
            if (panelDialog != null) panelDialog.SetActive(false);

            if (isDialogMenang)
            {
                if (teksMisi != null)
                {
                    teksMisi.gameObject.SetActive(true);
                    teksMisi.text = "<color=green><b>BOSS NAGA KALAH!</b></color>\nBerjalanlah ke arah <b>KIRI</b> untuk melanjutkan misi!";
                }
            }
            else
            {
                currentState = GameState.Bertempur;

                if (teksMisi != null) teksMisi.gameObject.SetActive(true);
                if (teksFeedback != null) teksFeedback.gameObject.SetActive(true);
                if (teksSkor != null) teksSkor.gameObject.SetActive(true);
                if (inputField != null) inputField.gameObject.SetActive(true);
                if (tombolSerang != null) tombolSerang.SetActive(true);

                MulaiRondeBaru();
            }
        }
    }

    void SetHUD(bool aktif)
    {
        if (teksMisi != null && teksMisi != textDialog)
            teksMisi.gameObject.SetActive(aktif);
        if (teksFeedback != null)
            teksFeedback.gameObject.SetActive(aktif);
        if (teksSkor != null)
            teksSkor.gameObject.SetActive(aktif);
        if (inputField != null)
            inputField.gameObject.SetActive(aktif);
        if (tombolSerang != null)
            tombolSerang.SetActive(aktif);

        if (panelDialog != null)
            panelDialog.SetActive(!aktif);
    }

    void MulaiRondeBaru()
    {
        if (currentState != GameState.Bertempur) return;

        sedangProsesSerangan = false;
        if (tombolSerang) tombolSerang.SetActive(true);
        if (inputField)
        {
            inputField.text = "";
            inputField.interactable = true;
        }
        if (teksFeedback) teksFeedback.text = "Ketik jawabanmu dan tekan Serang!";

        GenerateSoalBaru();
        UpdateUISkor();
    }

    void GenerateSoalBaru()
    {
        if (sisaSoal == null || sisaSoal.Count == 0)
        {
            sisaSoal = new List<Soal>(bankSoal);
        }

        if (semuaPapan == null || semuaPapan.Length == 0)
            semuaPapan = Object.FindObjectsByType<PlankBehavior>(FindObjectsSortMode.None);

        int indexAcak = Random.Range(0, sisaSoal.Count);
        soalAktif = sisaSoal[indexAcak];
        sisaSoal.RemoveAt(indexAcak);

        if (teksMisi) teksMisi.text = soalAktif.narasi;

        ResetSemuaWarnaPapan();
        PindahkanTepatKePapan(playerTransform, soalAktif.posisiPlayer, 0.5f);

        if (animasiNagaCoroutine != null)
            StopCoroutine(animasiNagaCoroutine);

        if (levelSaatIni == 3 && bayanganNaga != null)
        {
            bayanganNaga.SetActive(true);
            PindahkanTepatKePapan(bayanganNaga.transform, soalAktif.posisiMusuhAwal, 0.5f);

            if (bossNagaTransform != null)
            {
                bossNagaTransform.gameObject.SetActive(false);
                animasiNagaCoroutine = StartCoroutine(AnimasikanPerpindahanNaga(soalAktif.posisiMusuhAwal, soalAktif.posisiMusuh, 0.5f));
            }
        }
        else
        {
            if (bayanganNaga != null) bayanganNaga.SetActive(false);
            if (bossNagaTransform != null)
            {
                bossNagaTransform.gameObject.SetActive(true);
                PindahkanTepatKePapan(bossNagaTransform, soalAktif.posisiMusuh, 0.5f);
            }
        }

        if (playerTransform && bossNagaTransform)
        {
            AturArahHadap(playerTransform, bossNagaTransform.position);
            AturArahHadap(bossNagaTransform, playerTransform.position);
        }

        UpdateTitikTembak();
    }

    IEnumerator AnimasikanPerpindahanNaga(int awal, int tujuan, float tinggiY)
    {
        if (bayanganNaga != null) bayanganNaga.SetActive(true);
        yield return new WaitForSeconds(0.8f);

        if (bossNagaTransform != null)
        {
            bossNagaTransform.gameObject.SetActive(true);
            PindahkanTepatKePapan(bossNagaTransform, awal, tinggiY);
        }

        yield return new WaitForSeconds(0.4f);
        PindahkanTepatKePapan(bossNagaTransform, tujuan, tinggiY);

        if (playerTransform && bossNagaTransform)
            AturArahHadap(bossNagaTransform, playerTransform.position);

        UpdateTitikTembak();
    }

    void PindahkanTepatKePapan(Transform obj, int targetKoordinat, float tinggiY)
    {
        if (obj == null) return;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        Collider2D col = obj.GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
        }

        PlankBehavior papanKetemu = null;

        if (semuaPapan != null)
        {
            foreach (var papan in semuaPapan)
            {
                if (papan != null && papan.koordinatPapan == targetKoordinat)
                {
                    float posY = papan.transform.position.y + tinggiY;

                    if (col != null && papan.GetComponent<Collider2D>() != null)
                    {
                        float setengahPapan = papan.GetComponent<Collider2D>().bounds.size.y / 2f;
                        float setengahObj = col.bounds.size.y / 2f;
                        posY = papan.transform.position.y + setengahPapan + setengahObj;
                    }

                    obj.position = new Vector3(papan.transform.position.x, posY, 0f);
                    papanKetemu = papan;
                    break;
                }
            }
        }

        if (rb != null) rb.simulated = true;

        if (papanKetemu != null)
            StartCoroutine(PaksaUpdatePapan(papanKetemu));
    }

    IEnumerator PaksaUpdatePapan(PlankBehavior papan)
    {
        yield return null;
        yield return null;
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.05f);

        if (papan != null)
            papan.CekObjekDiAtas();
    }

    void ResetSemuaWarnaPapan()
    {
        if (semuaPapan == null) return;
        foreach (var p in semuaPapan)
            if (p != null) p.ResetPapan();
    }

    void AturArahHadap(Transform karakter, Vector3 targetPos)
    {
        if (karakter == null) return;
        float arah = targetPos.x < karakter.position.x ? -1f : 1f;
        karakter.localScale = new Vector3(arah * Mathf.Abs(karakter.localScale.x), karakter.localScale.y, 1f);
    }

    void UpdateTitikTembak()
    {
        if (titikTembakPlayer && playerTransform)
            titikTembakPlayer.position = playerTransform.position + new Vector3(playerTransform.localScale.x > 0 ? 1f : -1f, 0.5f, 0f);

        if (titikTembakNaga && bossNagaTransform)
            titikTembakNaga.position = bossNagaTransform.position + new Vector3(bossNagaTransform.localScale.x > 0 ? 1.5f : -1.5f, 1f, 0f);
    }

    public void EksekusiSerangan()
    {
        if (currentState != GameState.Bertempur || sedangProsesSerangan) return;
        if (inputField == null || string.IsNullOrEmpty(inputField.text)) return;

        if (!int.TryParse(inputField.text, out int jawaban))
        {
            if (teksFeedback) teksFeedback.text = "<color=yellow>Masukkan angka bilangan bulat!</color>";
            return;
        }

        ProsesCekJawaban(jawaban);
    }

 void ProsesCekJawaban(int jawabanSiswa)
{
    if (soalAktif == null) return;

    sedangProsesSerangan = true;
    if (tombolSerang) tombolSerang.SetActive(false);
    if (inputField) inputField.interactable = false;

    string teksLompat = jawabanSiswa > 0 ? "+" + jawabanSiswa : jawabanSiswa.ToString();
    bool benar = jawabanSiswa == soalAktif.angkaTarget;

    if (benar)
    {
        if (teksFeedback)
            teksFeedback.text = $"<color=green>BENAR!</color> {soalAktif.penjelasan}\n<b>Bola api menuju Boss Naga!</b>";

        TembakBolaApi("Enemy", () => StartCoroutine(ProsesMenangRonde()), teksLompat);
    }
    else
    {
        // Langsung tampilkan feedback
        if (teksFeedback)
            teksFeedback.text = $"<color=red>SALAH!</color> Jawaban benar: {soalAktif.angkaTarget}\n<b>Boss Naga menyerang!</b>";

        // Tembak bola api (visual saja)
        TembakBolaApi("Player", null, teksLompat);   // callback dikosongkan

        // Paksa lanjut setelah 1.6 detik (meski bola api bermasalah)
        StartCoroutine(ProsesHukumanSalah());
    }
}

 void TembakBolaApi(string tagTarget, System.Action callback, string teksLompatan = "")
{
    if (bolaApiMaster == null)
    {
        callback?.Invoke();
        return;
    }

    Transform asal = null;
    Transform target = null;

    if (tagTarget == "Enemy")
    {
        asal = titikTembakPlayer;
        target = bossNagaTransform;
    }
    else
    {
        asal = titikTembakNaga;
        target = playerTransform;
    }

    if (asal == null || target == null)
    {
        Debug.LogError($"Gagal tembak! asal/target null (Tag: {tagTarget})");
        callback?.Invoke();
        return;
    }

    GameObject api = Instantiate(bolaApiMaster, asal.position, Quaternion.identity);
    api.SetActive(true);

    BolaApiBehavior skrip = api.GetComponent<BolaApiBehavior>();
    if (skrip != null)
    {
        skrip.Tembak(target, callback, tagTarget, teksLompatan);
    }
    else
    {
        Debug.LogError("Prefab Bola Api tidak punya BolaApiBehavior!");
        Destroy(api);
        callback?.Invoke();
    }
}
    IEnumerator ProsesMenangRonde()
    {
        string questionId = $"S1-L3-S{levelSaatIni}-Q{(skorSaatIni + 1):D2}";
        Debug.Log($"[QUESTION]\nQuestionID={questionId}\nQuestion {(skorSaatIni + 1)}/{targetSkorUntukMenang}");
        Debug.Log($"[ANSWER]\nCorrect");
        GameStatisticsManager.CatatJawabanBenar(questionId);

        skorSaatIni++;
        UpdateUISkor();

        yield return new WaitForSeconds(1.8f);

        if (skorSaatIni >= targetSkorUntukMenang)
        {
            if (!sudahCatatBosKalah)
            {
                sudahCatatBosKalah = true;
                GameStatisticsManager.CatatBossKalah("Boss Naga");
                GameStatisticsManager.BerikanRewardLevel(1, 3, levelSaatIni, 35);
            }
            StartDialogMenang();
        }
        else
        {
            MulaiRondeBaru();
        }
    }

    IEnumerator ProsesHukumanSalah()
    {
        string questionId = $"S1-L3-S{levelSaatIni}-Q{(skorSaatIni + 1):D2}";
        Debug.Log($"[QUESTION]\nQuestionID={questionId}\nQuestion {(skorSaatIni + 1)}/{targetSkorUntukMenang}");
        Debug.Log($"[ANSWER]\nWrong");
        GameStatisticsManager.CatatJawabanSalah(questionId);

        nyawaSaatIni--;
        UpdateUIHati();

        yield return new WaitForSeconds(1.8f);

        if (nyawaSaatIni > 0)
        {
            MulaiRondeBaru();
        }
        else
        {
            currentState = GameState.Kalah;
            SetHUD(false);

            if (teksMisi)
            {
                teksMisi.gameObject.SetActive(true);
                teksMisi.text = "<color=red><b>GAME OVER!</b></color>\nKamu kehabisan nyawa!";
            }

            Invoke(nameof(PindahKeScenePencapaian), 2.5f);
        }
    }

    public void PlayerMencapaiGarisFinish()
    {
        if (currentState != GameState.Menang) return;

        if (teksMisi) teksMisi.gameObject.SetActive(false);

        if (panelEvaluasi != null)
            WinManager.TampilkanWin(panelEvaluasi);
        else
            WinManager.TampilkanWin();
    }

    public void PindahKeScenePencapaian()
    {
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Pencapaian");
    }

    void UpdateUIHati()
    {
        if (ikonHatiUI == null) return;

        for (int i = 0; i < ikonHatiUI.Length; i++)
        {
            if (ikonHatiUI[i] == null) continue;

            if (i < nyawaSaatIni)
            {
                ikonHatiUI[i].gameObject.SetActive(true);
                if (spriteHatiUtuh) ikonHatiUI[i].sprite = spriteHatiUtuh;
            }
            else
            {
                if (spriteHatiRetak)
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

    void UpdateUISkor()
    {
        if (teksSkor) teksSkor.text = $"{skorSaatIni} / {targetSkorUntukMenang}";
    }

    public void RespawnPlayer()
    {
        nyawaSaatIni--;
        UpdateUIHati();

        if (nyawaSaatIni > 0)
            MulaiRondeBaru();
        else
        {
            currentState = GameState.Kalah;
            SetHUD(false);
            if (teksMisi)
            {
                teksMisi.gameObject.SetActive(true);
                teksMisi.text = "<color=red><b>GAME OVER!</b></color>\nKamu jatuh ke jurang!";
            }
            Invoke(nameof(PindahKeScenePencapaian), 2.5f);
        }
    }

    void MuatBankSoal(int level)
    {
        bankSoal.Clear();
        sisaSoal.Clear();

        if (level == 1)
        {
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 4 langkah di kanan. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = 4, angkaTarget = 4, penjelasan = "4 langkah kanan dari 0 = +4" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 3 langkah di kiri. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = -3, angkaTarget = -3, penjelasan = "3 langkah kiri dari 0 = -3" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 6 langkah di kanan. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = 6, angkaTarget = 6, penjelasan = "6 langkah kanan = +6" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 5 langkah di kiri. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = -5, angkaTarget = -5, penjelasan = "5 langkah kiri = -5" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 8 langkah di kanan. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = 8, angkaTarget = 8, penjelasan = "8 langkah kanan = +8" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 7 langkah di kiri. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = -7, angkaTarget = -7, penjelasan = "7 langkah kiri = -7" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 2 langkah di kanan. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = 2, angkaTarget = 2, penjelasan = "2 langkah kanan = +2" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 10 langkah di kiri. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = -10, angkaTarget = -10, penjelasan = "10 langkah kiri = -10" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 1 langkah di kiri. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = -1, angkaTarget = -1, penjelasan = "1 langkah kiri = -1" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Boss Naga ada 9 langkah di kanan. Berapa koordinat Boss Naga?", posisiPlayer = 0, posisiMusuh = 9, angkaTarget = 9, penjelasan = "9 langkah kanan = +9" });
        }
        else if (level == 2)
        {
            bankSoal.Add(new Soal { narasi = "Ksatria di (-2). Boss Naga di (3). Berapa langkah tembakan?", posisiPlayer = -2, posisiMusuh = 3, angkaTarget = 5, penjelasan = "3 - (-2) = 5 (Kanan)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (3). Boss Naga di (-1). Berapa langkah tembakan?", posisiPlayer = 3, posisiMusuh = -1, angkaTarget = -4, penjelasan = "-1 - 3 = -4 (Kiri)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (-4). Boss Naga di (2). Berapa langkah tembakan?", posisiPlayer = -4, posisiMusuh = 2, angkaTarget = 6, penjelasan = "2 - (-4) = 6 (Kanan)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (5). Boss Naga di (-3). Berapa langkah tembakan?", posisiPlayer = 5, posisiMusuh = -3, angkaTarget = -8, penjelasan = "-3 - 5 = -8 (Kiri)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (-1). Boss Naga di (6). Berapa langkah tembakan?", posisiPlayer = -1, posisiMusuh = 6, angkaTarget = 7, penjelasan = "6 - (-1) = 7 (Kanan)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (2). Boss Naga di (-6). Berapa langkah tembakan?", posisiPlayer = 2, posisiMusuh = -6, angkaTarget = -8, penjelasan = "-6 - 2 = -8 (Kiri)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (-5). Boss Naga di (0). Berapa langkah tembakan?", posisiPlayer = -5, posisiMusuh = 0, angkaTarget = 5, penjelasan = "0 - (-5) = 5 (Kanan)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (4). Boss Naga di (0). Berapa langkah tembakan?", posisiPlayer = 4, posisiMusuh = 0, angkaTarget = -4, penjelasan = "0 - 4 = -4 (Kiri)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (-3). Boss Naga di (-7). Berapa langkah tembakan?", posisiPlayer = -3, posisiMusuh = -7, angkaTarget = -4, penjelasan = "-7 - (-3) = -4 (Kiri)" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (1). Boss Naga di (8). Berapa langkah tembakan?", posisiPlayer = 1, posisiMusuh = 8, angkaTarget = 7, penjelasan = "8 - 1 = 7 (Kanan)" });
        }
        else if (level == 3)
        {
            bankSoal.Add(new Soal { narasi = "Ksatria di (-5). Naga dari 0 mundur 3 langkah kiri. Berapa jarak tembakan?", posisiPlayer = -5, posisiMusuhAwal = 0, posisiMusuh = -3, angkaTarget = 2, penjelasan = "Posisi Naga = -3. Jarak = -3 - (-5) = 2" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (4). Naga dari -4 maju 2 langkah kanan. Berapa jarak tembakan?", posisiPlayer = 4, posisiMusuhAwal = -4, posisiMusuh = -2, angkaTarget = -6, penjelasan = "Posisi Naga = -2. Jarak = -2 - 4 = -6" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (-2). Naga dari -8 teleport 12 langkah kanan. Berapa jarak tembakan?", posisiPlayer = -2, posisiMusuhAwal = -8, posisiMusuh = 4, angkaTarget = 6, penjelasan = "Posisi Naga = 4. Jarak = 4 - (-2) = 6" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (6). Naga dari 2 bergeser 10 langkah kiri. Berapa jarak tembakan?", posisiPlayer = 6, posisiMusuhAwal = 2, posisiMusuh = -8, angkaTarget = -14, penjelasan = "Posisi Naga = -8. Jarak = -8 - 6 = -14" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (0). Naga dari 8 mundur 12 langkah kiri. Berapa jarak tembakan?", posisiPlayer = 0, posisiMusuhAwal = 8, posisiMusuh = -4, angkaTarget = -4, penjelasan = "Posisi Naga = -4. Jarak = -4 - 0 = -4" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (-9). Naga dari -3 maju 5 langkah kanan. Berapa jarak tembakan?", posisiPlayer = -9, posisiMusuhAwal = -3, posisiMusuh = 2, angkaTarget = 11, penjelasan = "Posisi Naga = 2. Jarak = 2 - (-9) = 11" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (2). Naga dari 5 mundur 8 langkah kiri. Berapa jarak tembakan?", posisiPlayer = 2, posisiMusuhAwal = 5, posisiMusuh = -3, angkaTarget = -5, penjelasan = "Posisi Naga = -3. Jarak = -3 - 2 = -5" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (-4). Naga dari -8 bergerak 11 langkah kanan. Berapa jarak tembakan?", posisiPlayer = -4, posisiMusuhAwal = -8, posisiMusuh = 3, angkaTarget = 7, penjelasan = "Posisi Naga = 3. Jarak = 3 - (-4) = 7" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (5). Naga dari 5 sihir 11 langkah kiri. Berapa jarak tembakan?", posisiPlayer = 5, posisiMusuhAwal = 5, posisiMusuh = -6, angkaTarget = -11, penjelasan = "Posisi Naga = -6. Jarak = -6 - 5 = -11" });
            bankSoal.Add(new Soal { narasi = "Ksatria di (-2). Naga dari -9 terbang 10 langkah kanan. Berapa jarak tembakan?", posisiPlayer = -2, posisiMusuhAwal = -9, posisiMusuh = 1, angkaTarget = 3, penjelasan = "Posisi Naga = 1. Jarak = 1 - (-2) = 3" });
        }

        sisaSoal = new List<Soal>(bankSoal);
    }

    public void CekJawabanSiswa(int angkaPlatform)
    {
        if (currentState != GameState.Bertempur || sedangProsesSerangan) return;
        ProsesCekJawaban(angkaPlatform);
    }
}
