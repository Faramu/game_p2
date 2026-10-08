using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TebingKabutManager : MonoBehaviour
{
    public enum GameState
    {
        DialogAwal,
        MenungguTrigger,
        TutorialPanah,
        TutorialContoh,
        Gameplay,
        Menjelaskan,
        Kalah,
        Selesai
    }

    public GameState currentState = GameState.DialogAwal;

    [System.Serializable]
    public class Soal
    {
        [TextArea(2, 3)] public string narasi;
        public bool jawabanBenarBool;
        public string jawabanBenarString;
        [TextArea(2, 3)] public string penjelasan;
    }

    [Header("--- SISTEM LEVEL & PROGRES ---")]
    public int levelSaatIni = 1;
    public int targetSoal = 10;
    private int jumlahSoalTerjawab = 0;

    [Header("--- SISTEM NYAWA ---")]
    public int nyawaMaksimal = 3;
    private int nyawaSaatIni;

    [Header("Pengaturan UI Hati")]
    public Image[] ikonHatiUI = new Image[3];
    public Sprite spriteHatiUtuh;
    public Sprite spriteHatiRetak;

    [Header("Pengaturan Ledakan")]
    public GameObject prefabLedakanMantra;

    public GameObject jembatanFinish;

    [Header("--- UI DIALOG ---")]
    public GameObject panelDialog;
    public TextMeshProUGUI textDialog;
    public TextMeshProUGUI teksPadaTombol;

    [Header("--- UI KOTAK SOAL ---")]
    public TextMeshProUGUI teksKotakUngu;

    [Header("--- PENGATURAN METEOR SOAL ---")]
    public GameObject prefabMeteor;
    public Transform titikSpawnMeteor;

    [Header("--- REFERENSI VISUAL PANAH (SPRITE) ---")]
    public GameObject panahKanan;
    public Animator animPanahKanan;
    public GameObject panahKiri;
    public Animator animPanahKiri;

    private List<Soal> bankSoal = new List<Soal>();
    private List<int> sisaIndeksSoal = new List<int>(); // agar soal tidak berulang
    private int idSoalAktif;
    private GameObject meteorAktif;
#pragma warning disable 0414
    private int tahapTutorial = 0;
    private string teksIntro, teksTengah, teksTutorialKiri, teksTutorialContoh;
#pragma warning restore 0414
    private bool sedangMemprosesJawaban = false;

    void Start()
    {
        Player player = Object.FindFirstObjectByType<Player>();
        if (player != null)
        {
            nyawaSaatIni = player.health;
        }
        else
        {
            nyawaSaatIni = nyawaMaksimal;
        }
        UpdateUIHati();

        if (jembatanFinish != null)
            jembatanFinish.SetActive(false);

        TampilkanPanah(false, false);

        MuatBankSoal(levelSaatIni);
        ResetSisaSoal();
        SiapkanTeksDialogLevel(levelSaatIni);

        currentState = GameState.DialogAwal;
        if (panelDialog != null)
        {
            panelDialog.SetActive(true);
            if (textDialog != null) textDialog.text = teksIntro;
            if (teksPadaTombol != null) teksPadaTombol.text = "LANJUT";
        }

        SetPlayerMovement(false);
    }

    private void Update()
    {
        // Pastikan player tidak dapat bergerak bebas saat dialog/penjelasan aktif
        if (currentState == GameState.DialogAwal ||
            currentState == GameState.Menjelaskan ||
            currentState == GameState.Kalah)
        {
            SetPlayerMovement(false);
        }
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

    private void SiapkanTeksDialogLevel(int level)
    {
        if (level == 1)
        {
            teksIntro = "Ksatria! Selamat datang di Tebing Kabut! Energi di sini terbagi menjadi dua kutub. Jawab tantangan untuk membuka jembatan. Majulah ke tengah!";
            teksTengah = "Lihat panah Hijau di kananmu. Melompatlah ke sana untuk kejadian yang bermakna BERTAMBAH, NAIK, atau POSITIF (+).";
            teksTutorialKiri = "Sebaliknya, panah Ungu di kirimu adalah area NEGATIF (-). Untuk kejadian yang BERKURANG atau TURUN.";
            teksTutorialContoh = "CONTOH: 'Penyusutan'. Penyusutan berarti ukurannya berkurang (Negatif).\nLOMPAT KE KIRI (Area Negatif) untuk berlatih!";
        }
        else if (level == 2)
        {
            teksIntro = "Luar biasa! Di Level 2 ini, kamu harus menerjemahkan narasi menjadi ANGKA. Majulah ke tengah tebing!";
            teksTengah = "Panah Hijau (Kanan) tetap untuk arah Positif (+). Misal: 'Air bertambah 20 liter' berarti +20.";
            teksTutorialKiri = "Panah Ungu (Kiri) untuk Negatif (-).";
            teksTutorialContoh = "CONTOH: 'Musuh jatuh ke jurang 8 meter'. Kedalaman berarti minus (-8).\nAyo, LOMPAT KE KIRI (Area Negatif) sekarang!";
        }
        else
        {
            teksIntro = "Ujian terakhir di Level 3! Kejadian alam kini terjadi beruntun. Hitung hasil akhirnya! Majulah ke tengah.";
            teksTengah = "Kanan (+) untuk kejadian yang pada akhirnya membawa keuntungan/bertambah.";
            teksTutorialKiri = "Kiri (-) untuk kejadian yang hasil akhirnya merugikan/berkurang.";
            teksTutorialContoh = "CONTOH: 'Naik 5 tangga, lalu turun 7 tangga'. Hasilnya adalah (5 - 7) = -2.\nKarena hasilnya Negatif (-2), LOMPAT KE KIRI untuk latihan!";
        }
    }

    private void ResetSisaSoal()
    {
        sisaIndeksSoal.Clear();
        for (int i = 0; i < bankSoal.Count; i++)
            sisaIndeksSoal.Add(i);
    }

    public void KlikTombol ()
    {
        if (currentState == GameState.Selesai)
        {
            if (panelDialog != null) panelDialog.SetActive(false);
            SetPlayerMovement(true);
            return;
        }

        if (currentState == GameState.Kalah)
        {
            PlayerPrefs.Save();
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu_Pencapaian");
            return;
        }

        if (currentState == GameState.DialogAwal)
        {
            if (panelDialog != null) panelDialog.SetActive(false);
            SetPlayerMovement(true);
            currentState = GameState.MenungguTrigger;
            return;
        }

        if (currentState == GameState.TutorialPanah || currentState == GameState.TutorialContoh)
        {
            if (panelDialog != null) panelDialog.SetActive(false);
            SetPlayerMovement(true);
            currentState = GameState.MenungguTrigger;
            MunculkanSoalBaru();
            return;
        }

        if (currentState == GameState.Menjelaskan)
        {
            if (panelDialog != null) panelDialog.SetActive(false);
            SetPlayerMovement(true);

            if (jumlahSoalTerjawab >= targetSoal)
            {
                currentState = GameState.Selesai;
                if (jembatanFinish != null) jembatanFinish.SetActive(true);
            }
            else
            {
                currentState = GameState.MenungguTrigger;
                // Sembunyikan panah sejenak saat menunggu player kembali ke arena trigger
                TampilkanPanah(false, false);

                // Reset trigger agar player bisa memicu soal berikutnya saat masuk arena
                TriggerAreaTengah trigger = Object.FindFirstObjectByType<TriggerAreaTengah>();
                if (trigger != null)
                {
                    trigger.ResetTrigger();
                }
            }
            return;
        }
    }

    public bool PemicuTengahDiinjak()
    {
        if (currentState == GameState.DialogAwal)
        {
            // Jika player langsung melangkah ke trigger saat dialog awal terbuka
            if (panelDialog != null) panelDialog.SetActive(false);
            currentState = GameState.MenungguTrigger;
        }

        if (currentState != GameState.MenungguTrigger)
        {
            Debug.Log($"[STAGE1-LV2 BATU]\nPEMANGGILAN DIBLOKIR.\nState = {currentState}");
            return false;
        }

        // Tampilkan panah petunjuk
        TampilkanPanah(true, true);
        Debug.Log("[STAGE1-LV2 TRIGGER]\nPanah ditampilkan.");

        // Panggil / munculkan Batu Mantra (Soal)
        Debug.Log("[STAGE1-LV2 BATU]\nMemanggil Batu Mantra.");
        MunculkanSoalBaru();

        return true;
    }

    public void MunculkanSoalBaru()
    {
        sedangMemprosesJawaban = false;
        currentState = GameState.Gameplay;
        TampilkanPanah(true, true);
        SetPlayerMovement(true);

        int noSoal = jumlahSoalTerjawab + 1;
        Debug.Log($"[QUESTION]\nSoal {noSoal} ditampilkan.");

        if (meteorAktif != null)
            Destroy(meteorAktif);

        if (bankSoal.Count == 0)
        {
            Debug.LogError("Bank soal kosong!");
            return;
        }

        // Soal tidak berulang
        if (sisaIndeksSoal.Count == 0)
            ResetSisaSoal();

        int indexAcak = Random.Range(0, sisaIndeksSoal.Count);
        idSoalAktif = sisaIndeksSoal[indexAcak];
        sisaIndeksSoal.RemoveAt(indexAcak);

        Soal soalSaatIni = bankSoal[idSoalAktif];

        if (teksKotakUngu != null)
            teksKotakUngu.text = soalSaatIni.narasi;

        if (prefabMeteor == null)
        {
            Debug.LogError("[STAGE1-LV2 BATU]\nERROR: Reference Batu Mantra NULL.");
            return;
        }
        if (titikSpawnMeteor == null)
        {
            Debug.LogError("[STAGE1-LV2 BATU]\nERROR: Reference titikSpawnMeteor NULL.");
            return;
        }

        Vector3 posisiSpawn = titikSpawnMeteor.position;
        posisiSpawn.y -= 4f;
        meteorAktif = Instantiate(prefabMeteor, posisiSpawn, Quaternion.identity);

        if (meteorAktif != null)
        {
            Debug.Log("[STAGE1-LV2 BATU]\nBatu Mantra berhasil diaktifkan/spawn.");
            Debug.Log("[STAGE1-LV2 BATU]\nBatu Mantra ACTIVE.");
            Debug.Log("[STAGE1-LV2 SOAL]\nBatu Mantra siap memunculkan soal.");

            BatuMantraController batuScript = meteorAktif.GetComponent<BatuMantraController>();
            if (batuScript != null)
            {
                Player player = Object.FindFirstObjectByType<Player>();
                string notasiTampil = (levelSaatIni == 3) ? "(Pilih Jawaban)" : "";
                batuScript.SetupBatuSoal(
                    soalSaatIni.narasi,
                    notasiTampil,
                    player != null ? player.transform : null,
                    this
                );
                Debug.Log("[STAGE1-LV2 SOAL]\nDialog/Soal dipanggil.");
            }
            else
            {
                Debug.LogError("[STAGE1-LV2 SOAL]\nERROR: Batu aktif tetapi soal tidak dipanggil.");
            }
        }
        else
        {
            Debug.LogError("[STAGE1-LV2 BATU]\nERROR: Reference Batu Mantra NULL.");
        }
    }

    public void CekJawaban(bool pilihanPemain)
    {
        if (currentState == GameState.TutorialContoh)
        {
            if (pilihanPemain == false)
            {
                SetPlayerMovement(false);
                if (panelDialog != null) panelDialog.SetActive(true);
                if (textDialog != null)
                    textDialog.text = "Tepat sekali! Kamu sudah paham arahnya. Bersiaplah, meteor kuis sesungguhnya akan berjatuhan!";
                if (teksPadaTombol != null) teksPadaTombol.text = "";
                currentState = GameState.Menjelaskan;
            }
            return;
        }

        if (currentState != GameState.Gameplay) return;

        ProsesHasilJawaban(pilihanPemain == bankSoal[idSoalAktif].jawabanBenarBool);
    }

    public void CekJawabanInputLevel3(string inputSiswa)
    {
        if (currentState != GameState.Gameplay || levelSaatIni != 3) return;

        bool isBenar = (inputSiswa.Trim() == bankSoal[idSoalAktif].jawabanBenarString);
        ProsesHasilJawaban(isBenar);
    }

    private void ProsesHasilJawaban(bool isBenar)
    {
        if (bankSoal.Count == 0 || idSoalAktif < 0 || idSoalAktif >= bankSoal.Count)
            return;

        if (sedangMemprosesJawaban) return;
        sedangMemprosesJawaban = true;

        Soal soalSaatIni = bankSoal[idSoalAktif];
        SetPlayerMovement(false);

        int noSoal = jumlahSoalTerjawab + 1;
        string questionId = $"S1-L2-S{levelSaatIni}-Q{noSoal:D2}";

        if (isBenar)
        {
            Debug.Log($"[QUESTION]\nQuestionID={questionId}\nQuestion {noSoal}/{targetSoal}");
            Debug.Log($"[ANSWER]\nCorrect");
            GameStatisticsManager.CatatJawabanBenar(questionId);

            Player player = Object.FindFirstObjectByType<Player>();
            int currentHP = player != null ? player.health : nyawaSaatIni;
            Debug.Log($"[QUESTION]\nJawaban benar.\nHP tetap = {currentHP}");

            if (meteorAktif != null)
            {
                BatuMantraController batuScript = meteorAktif.GetComponent<BatuMantraController>();
                if (batuScript != null)
                    batuScript.HancurkanKarenaBenar();
                else
                    Destroy(meteorAktif);
                meteorAktif = null;
            }

            jumlahSoalTerjawab++;
            currentState = GameState.Menjelaskan;

            if (panelDialog != null) panelDialog.SetActive(true);

            if (jumlahSoalTerjawab >= targetSoal)
            {
                if (textDialog != null)
                    textDialog.text = "<color=green>Luar biasa!</color> Kamu berhasil menjawab " + targetSoal + " soal. Jembatan terbuka!";
                if (teksPadaTombol != null) teksPadaTombol.text = "";
                if (jembatanFinish != null) jembatanFinish.SetActive(true);
                currentState = GameState.Selesai;

                // Berikan reward penyelesaian level jika belum pernah diklaim
                GameStatisticsManager.BerikanRewardLevel(1, 2, levelSaatIni, 25);
            }
            else
            {
                if (textDialog != null)
                    textDialog.text = $"<color=green>BENAR!</color>\n{soalSaatIni.penjelasan}\n(Soal ke-{jumlahSoalTerjawab}/{targetSoal})";
                if (teksPadaTombol != null) teksPadaTombol.text = "LANJUT";
            }
        }
        else
        {
            Debug.Log($"[QUESTION]\nQuestionID={questionId}\nQuestion {noSoal}/{targetSoal}");
            Debug.Log($"[ANSWER]\nWrong");
            GameStatisticsManager.CatatJawabanSalah(questionId);

            if (meteorAktif != null)
            {
                BatuMantraController batuScript = meteorAktif.GetComponent<BatuMantraController>();
                if (batuScript != null)
                    batuScript.HancurkanKarenaBenar();
                else
                    Destroy(meteorAktif);
                meteorAktif = null;
            }

            // Kurangi HP/Heart Player 1 secara resmi
            Player player = Object.FindFirstObjectByType<Player>();
            int hpSebelum = player != null ? player.health : nyawaSaatIni;
            int hpSesudah = hpSebelum;

            if (player != null)
            {
                hpSesudah = player.KurangiHealthSatu();
            }
            else
            {
                nyawaSaatIni--;
                if (nyawaSaatIni < 0) nyawaSaatIni = 0;
                hpSesudah = nyawaSaatIni;
                Debug.Log($"[HEART]\nJawaban salah.\nHP: {hpSebelum} → {hpSesudah}");
            }

            nyawaSaatIni = hpSesudah;
            UpdateUIHati();

            if (panelDialog != null) panelDialog.SetActive(true);

            if (nyawaSaatIni > 0)
            {
                currentState = GameState.Menjelaskan;
                if (textDialog != null)
                    textDialog.text = $"<color=red>SALAH!</color>\n{soalSaatIni.penjelasan}\nHati tersisa: {nyawaSaatIni}\nCoba soal berikutnya!";
                if (teksPadaTombol != null) teksPadaTombol.text = "LANJUT";
            }
            else
            {
                currentState = GameState.Kalah;
                if (textDialog != null)
                    textDialog.text = "<color=red>GAME OVER!</color>\nSeluruh hatimu telah hancur. Kamu harus mengulangi area ini!";
                if (teksPadaTombol != null) teksPadaTombol.text = "LIHAT PENCAPAIAN";
            }
        }
    }

    public void WaktuHabis()
    {
        if (currentState != GameState.Gameplay) return;
        if (sedangMemprosesJawaban) return;
        sedangMemprosesJawaban = true;

        int noSoal = jumlahSoalTerjawab + 1;
        Debug.Log($"[QUESTION]\nSoal {noSoal}\nJawaban = SALAH");

        if (meteorAktif != null)
        {
            Destroy(meteorAktif);
            meteorAktif = null;
        }

        Player player = Object.FindFirstObjectByType<Player>();
        int hpSebelum = player != null ? player.health : nyawaSaatIni;
        int hpSesudah = hpSebelum;

        if (player != null)
        {
            hpSesudah = player.KurangiHealthSatu();
        }
        else
        {
            nyawaSaatIni--;
            if (nyawaSaatIni < 0) nyawaSaatIni = 0;
            hpSesudah = nyawaSaatIni;
            Debug.Log($"[HEART]\nJawaban salah.\nHP: {hpSebelum} → {hpSesudah}");
        }

        nyawaSaatIni = hpSesudah;
        UpdateUIHati();

        SetPlayerMovement(false);
        if (panelDialog != null) panelDialog.SetActive(true);

        if (nyawaSaatIni > 0)
        {
            currentState = GameState.Menjelaskan;
            if (textDialog != null)
                textDialog.text = $"<color=orange>WAKTU HABIS!</color>\nHati tersisa: {nyawaSaatIni}\nKamu harus lebih cepat!\nCoba soal berikutnya.";
            if (teksPadaTombol != null) teksPadaTombol.text = "LANJUT";
        }
        else
        {
            currentState = GameState.Kalah;
            if (textDialog != null)
                textDialog.text = "<color=red>GAME OVER!</color>\nSeluruh hatimu telah hancur karena kehabisan waktu.";
            if (teksPadaTombol != null) teksPadaTombol.text = "LIHAT PENCAPAIAN";
        }
    }

    public int GetNomorSoalSaatIni()
    {
        return jumlahSoalTerjawab + 1;
    }

private void TampilkanPanah(bool kanan, bool kiri)
{
    if (animPanahKanan != null)
    {
        animPanahKanan.gameObject.SetActive(kanan);
        if (kanan && animPanahKanan.enabled)
            animPanahKanan.Play("PetunjukPositif", 0, 0f);  // nama state di Animator
    }
    else if (panahKanan != null)
    {
        panahKanan.SetActive(kanan);
    }

    if (animPanahKiri != null)
    {
        animPanahKiri.gameObject.SetActive(kiri);
        if (kiri && animPanahKiri.enabled)
            animPanahKiri.Play("PetunjukNegatif", 0, 0f);   // nama state di Animator
    }
    else if (panahKiri != null)
    {
        panahKiri.SetActive(kiri);
    }
}

    private void SetPlayerMovement(bool canMove)
    {
        Player player = Object.FindFirstObjectByType<Player>();
        if (player == null) return;
        player.canMove = canMove;
    }

    private void MuatBankSoal(int level)
    {
        bankSoal.Clear();

        if (level == 1)
        {
            bankSoal.Add(new Soal { narasi = "Keuntungan", jawabanBenarBool = true, penjelasan = "Keuntungan berarti uang bertambah, jadi bernilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Kenaikan", jawabanBenarBool = true, penjelasan = "Segala bentuk kenaikan melambangkan arah Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Maju", jawabanBenarBool = true, penjelasan = "Langkah maju ke depan adalah pergerakan Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Penambahan", jawabanBenarBool = true, penjelasan = "Penambahan sudah pasti membuat nilai menjadi Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Laba", jawabanBenarBool = true, penjelasan = "Laba sama dengan untung, nilainya adalah Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Menabung", jawabanBenarBool = true, penjelasan = "Menabung menambah jumlah aset, bernilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Di atas laut", jawabanBenarBool = true, penjelasan = "Ketinggian di atas permukaan laut bernilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Mendapat", jawabanBenarBool = true, penjelasan = "Mendapatkan sesuatu berarti bertambah (Positif)." });
            bankSoal.Add(new Soal { narasi = "Bonus", jawabanBenarBool = true, penjelasan = "Bonus adalah tambahan ekstra, sehingga bernilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Pemasukan", jawabanBenarBool = true, penjelasan = "Uang yang masuk ke kas adalah nilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Penyusutan", jawabanBenarBool = false, penjelasan = "Penyusutan berarti ukurannya berkurang atau Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Kerugian", jawabanBenarBool = false, penjelasan = "Rugi artinya harta atau nilai berkurang (Negatif)." });
            bankSoal.Add(new Soal { narasi = "Penurunan", jawabanBenarBool = false, penjelasan = "Penurunan melambangkan pergerakan ke bawah atau Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Mundur", jawabanBenarBool = false, penjelasan = "Langkah mundur berlawanan arah, jadi bernilai Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Di bawah laut", jawabanBenarBool = false, penjelasan = "Kedalaman di bawah laut selalu bernilai Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Utang", jawabanBenarBool = false, penjelasan = "Memiliki utang memotong nilai kekayaan, berarti Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Pengeluaran", jawabanBenarBool = false, penjelasan = "Mengeluarkan uang membuat kas berkurang (Negatif)." });
            bankSoal.Add(new Soal { narasi = "Denda", jawabanBenarBool = false, penjelasan = "Denda adalah bentuk hukuman pemotongan nilai (Negatif)." });
            bankSoal.Add(new Soal { narasi = "Kehilangan", jawabanBenarBool = false, penjelasan = "Kehilangan sesuatu membuat jumlahnya berkurang (Negatif)." });
            bankSoal.Add(new Soal { narasi = "Menyelam", jawabanBenarBool = false, penjelasan = "Bergerak ke bawah permukaan air bernilai Negatif (-)." });
        }
        else if (level == 2)
        {
            bankSoal.Add(new Soal { narasi = "Kabut tebal menyebabkan suhu turun 5 derajat celcius", jawabanBenarBool = false, penjelasan = "Suhu turun diwakili arah Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Pemain maju melompati rintangan sejauh 3 meter", jawabanBenarBool = true, penjelasan = "Maju bernilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Burung hantu magis melayang naik 12 meter", jawabanBenarBool = true, penjelasan = "Terbang naik berarti arah Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Penyelam mencari mutiara 15 meter di bawah laut", jawabanBenarBool = false, penjelasan = "Di bawah laut berarti kedalaman Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Pasukan goblin mundur ketakutan sejauh 8 langkah", jawabanBenarBool = false, penjelasan = "Mundur bernilai Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Toko obat desa mengalami laba 50 koin tembaga", jawabanBenarBool = true, penjelasan = "Laba/Untung menambah saldo bernilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Bunga bank dari tabungan desa bertambah 15 koin", jawabanBenarBool = true, penjelasan = "Bertambahnya aset bernilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Keranjang buah busuk menyebabkan rugi 30 koin", jawabanBenarBool = false, penjelasan = "Kerugian membuat saldo minus atau Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Menggali tanah sedalam 4 meter untuk jebakan", jawabanBenarBool = false, penjelasan = "Di bawah tanah selalu bermakna Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Helikopter penyelamat berada di ketinggian 200 meter", jawabanBenarBool = true, penjelasan = "Ketinggian di atas tanah bernilai Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Suhu di kawah gunung berapi meningkat tajam 45 derajat", jawabanBenarBool = true, penjelasan = "Peningkatan/naik selalu Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Prajurit berhutang 10 koin untuk makan malam", jawabanBenarBool = false, penjelasan = "Utang identik dengan nilai Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Penduduk memberikan donasi sumbangan sebanyak 25 koin", jawabanBenarBool = true, penjelasan = "Sumbangan menambah kas, jadi Positif (+)." });
            bankSoal.Add(new Soal { narasi = "Kekuatan segel gerbang melemah dan menyusut 8 poin", jawabanBenarBool = false, penjelasan = "Melemah atau menyusut berarti Negatif (-)." });
            bankSoal.Add(new Soal { narasi = "Menarik tali jangkar kapal naik 10 meter", jawabanBenarBool = true, penjelasan = "Menarik naik ke atas bernilai Positif (+)." });
        }
else if (level == 3)
{
    // ==========================================
    // LEVEL 3 - EVALUASI POSITIF / NEGATIF
    // Dua perubahan sederhana dalam satu soal
    // ==========================================

    bankSoal.Add(new Soal
    {
        narasi = "Ksatria maju 5 langkah, kemudian mundur 2 langkah. Apakah posisi akhirnya Positif atau Negatif?",
        jawabanBenarBool = true,
        penjelasan = "Maju +5 lalu mundur -2. Jadi 5 - 2 = +3. Hasil akhirnya Positif (+)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Suhu berada di 4°C, kemudian turun 7°C. Apakah suhu akhirnya Positif atau Negatif?",
        jawabanBenarBool = false,
        penjelasan = "Suhu 4°C turun 7°C. Jadi 4 - 7 = -3°C. Hasilnya Negatif (-)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Pemain memiliki 8 koin, kemudian mendapatkan 4 koin lagi. Apakah jumlah koin akhirnya Positif atau Negatif?",
        jawabanBenarBool = true,
        penjelasan = "8 + 4 = +12. Jumlah koin bertambah, sehingga Positif (+)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Ksatria berada 3 meter di bawah tanah, kemudian turun lagi 4 meter. Apakah posisinya akhirnya Positif atau Negatif?",
        jawabanBenarBool = false,
        penjelasan = "Posisi awal -3 meter dan turun 4 meter lagi. Jadi -3 - 4 = -7 meter. Hasilnya Negatif (-)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Suhu awal -2°C, kemudian naik 5°C. Apakah suhu akhirnya Positif atau Negatif?",
        jawabanBenarBool = true,
        penjelasan = "-2 + 5 = +3°C. Suhu akhirnya Positif (+)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Pedagang mendapatkan 10 koin, kemudian kehilangan 15 koin. Apakah perubahan akhirnya Positif atau Negatif?",
        jawabanBenarBool = false,
        penjelasan = "10 - 15 = -5. Karena hasilnya -5, maka Negatif (-)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Pemain mundur 6 langkah, kemudian maju 10 langkah. Apakah posisi akhirnya Positif atau Negatif?",
        jawabanBenarBool = true,
        penjelasan = "-6 + 10 = +4. Posisi akhirnya Positif (+)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Suhu awal 6°C, kemudian turun 10°C. Apakah suhu akhirnya Positif atau Negatif?",
        jawabanBenarBool = false,
        penjelasan = "6 - 10 = -4°C. Suhu akhirnya Negatif (-)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Ksatria kehilangan 5 koin, kemudian mendapatkan 8 koin. Apakah perubahan jumlah koin akhirnya Positif atau Negatif?",
        jawabanBenarBool = true,
        penjelasan = "-5 + 8 = +3. Perubahan akhirnya Positif (+)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Pemain berada di posisi -4, kemudian maju 2 langkah. Apakah posisi akhirnya Positif atau Negatif?",
        jawabanBenarBool = false,
        penjelasan = "-4 + 2 = -2. Karena hasilnya masih di bawah 0, maka Negatif (-)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Desa mendapat 7 koin dari hadiah, kemudian mendapat 3 koin lagi. Apakah perubahan akhirnya Positif atau Negatif?",
        jawabanBenarBool = true,
        penjelasan = "7 + 3 = +10. Jumlahnya bertambah, sehingga Positif (+)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Suhu berada di -5°C, kemudian turun 3°C lagi. Apakah suhu akhirnya Positif atau Negatif?",
        jawabanBenarBool = false,
        penjelasan = "-5 - 3 = -8°C. Suhu semakin turun, sehingga Negatif (-)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Ksatria maju 7 langkah, kemudian maju lagi 2 langkah. Apakah posisi akhirnya Positif atau Negatif?",
        jawabanBenarBool = true,
        penjelasan = "7 + 2 = +9. Posisi akhirnya Positif (+)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Pemain memiliki 12 koin, kemudian kehilangan 5 koin. Apakah perubahan akhirnya Positif atau Negatif?",
        jawabanBenarBool = true,
        penjelasan = "12 - 5 = +7. Jumlah koin masih bertambah dari titik awal, sehingga Positif (+)."
    });

    bankSoal.Add(new Soal
    {
        narasi = "Pemain berada di posisi 2, kemudian mundur 6 langkah. Apakah posisi akhirnya Positif atau Negatif?",
        jawabanBenarBool = false,
        penjelasan = "2 - 6 = -4. Posisi akhirnya Negatif (-)."
    });
}
    }
}