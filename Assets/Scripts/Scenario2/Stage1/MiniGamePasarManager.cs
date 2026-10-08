using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MiniGamePasarManager : MonoBehaviour
{
    public enum FaseGame { Instruksi, Latihan, KuisAsli, Selesai }

    [Header("--- STATUS FASE SAAT INI ---")]
    public FaseGame faseSaatIni = FaseGame.Instruksi;

    [Header("--- PENGATURAN LEVEL ---")]
    [Range(1, 3)] public int levelSaatIni = 1;

    [Header("--- BANK SOAL ---")]
    public SoalPasarDatabase.SoalPasar soalContoh;
    public List<SoalPasarDatabase.SoalPasar> daftarSoal = new List<SoalPasarDatabase.SoalPasar>();

    // Sistem Anti Pengulangan Soal
    private List<int> soalBelumDimainkan = new List<int>();
    private int indeksSoalAktif = 0;
    private bool mengulangSoalYangSama = false;

    [Header("--- KOLEKSI GAMBAR BERDASARKAN TAG ---")]
    public Sprite[] gambarApel;
    public Sprite[] gambarPisang;
    public Sprite[] gambarJeruk;
    public Sprite[] gambarBuahBusuk;
    public Sprite[] gambarKoin;
    public Sprite[] gambarKios;
    public Sprite[] gambarBox;
    public Sprite[] gambarLainnya;

    [Header("--- SYARAT MENANG (STREAK) ---")]
    public int targetStreak = 10;
    private int streakBenar = 0;

    [Header("--- REFERENSI UI ---")]
    public GameObject panelMiniGame;
    public TextMeshProUGUI teksJudulSoal;
    public Image ilustrasiGambar;
    public TextMeshProUGUI teksCounter;
    public TextMeshProUGUI teksTombolAtas;
    public TextMeshProUGUI teksTombolBawah;
    public TextMeshProUGUI teksFeedback;
    public GameObject tombolAtasObjek;
    public GameObject tombolBawahObjek;

    [Header("--- SISTEM NYAWA & KOIN ---")]
    public int nyawa = 3;
    public GameObject[] ikonHati;
    public int totalKoin = 0;
    public TextMeshProUGUI teksKoin;
    public GameObject panelGameOver;
    public GameObject panelDialog;
    public GameObject panelWinUi;

    private bool sedangMenampilkanPenjelasan = false;

    void Awake()
    {
        InisialisasiBankSoal();
    }

    void Start()
    {
        if (panelMiniGame != null) panelMiniGame.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelWinUi != null) panelWinUi.SetActive(false);
    }

    public void InisialisasiBankSoal()
    {
        daftarSoal.Clear();

        soalContoh = SoalPasarDatabase.BuatSoalContoh();
        daftarSoal = SoalPasarDatabase.MuatSoalBerdasarkanLevel(levelSaatIni);

        IsiUlangKantongSoal();

        Debug.Log("<color=green>Berhasil memuat " + daftarSoal.Count + " soal untuk Level " + levelSaatIni + "!</color>");
    }

    void IsiUlangKantongSoal()
    {
        soalBelumDimainkan.Clear();
        for (int i = 0; i < daftarSoal.Count; i++)
        {
            soalBelumDimainkan.Add(i);
        }
    }

    Sprite AmbilGambarSesuaiTag(SoalPasarDatabase.TagGambar tag)
    {
        Sprite[] pilihanArray = null;

        switch (tag)
        {
            case SoalPasarDatabase.TagGambar.Apel:      pilihanArray = gambarApel; break;
            case SoalPasarDatabase.TagGambar.Pisang:    pilihanArray = gambarPisang; break;
            case SoalPasarDatabase.TagGambar.Jeruk:     pilihanArray = gambarJeruk; break;
            case SoalPasarDatabase.TagGambar.BuahBusuk: pilihanArray = gambarBuahBusuk; break;
            case SoalPasarDatabase.TagGambar.Koin:      pilihanArray = gambarKoin; break;
            case SoalPasarDatabase.TagGambar.Kios:      pilihanArray = gambarKios; break;
            case SoalPasarDatabase.TagGambar.Box:       pilihanArray = gambarBox; break;
            default:                                    pilihanArray = gambarLainnya; break;
        }

        if (pilihanArray != null && pilihanArray.Length > 0)
        {
            int indeksAcak = Random.Range(0, pilihanArray.Length);
            return pilihanArray[indeksAcak];
        }

        return null;
    }

    // =========================================================================
    // LOGIKA PERMAINAN
    // =========================================================================
    public void MulaiMiniGame()
    {
        if (panelDialog != null) panelDialog.SetActive(false);
        if (panelMiniGame != null) panelMiniGame.SetActive(true);
        if (panelWinUi != null) panelWinUi.SetActive(false);

        nyawa = 3;
        totalKoin = 0;
        streakBenar = 0;
        mengulangSoalYangSama = false;

        InisialisasiBankSoal();
        UpdateUIKoin();

        if (ikonHati != null)
        {
            foreach (GameObject hati in ikonHati)
            {
                if (hati != null) hati.SetActive(true);
            }
        }

        MasukFaseInstruksi();
    }

    void MasukFaseInstruksi()
    {
        faseSaatIni = FaseGame.Instruksi;
        sedangMenampilkanPenjelasan = false;

        if (teksCounter != null) teksCounter.text = "LV " + levelSaatIni;
        if (teksJudulSoal != null) teksJudulSoal.text = "CARA MENGERJAKAN (LEVEL " + levelSaatIni + ")";
        if (ilustrasiGambar != null) ilustrasiGambar.enabled = false;

        if (teksFeedback != null)
        {
            teksFeedback.text = "<size=11><b><color=yellow>ATURAN PASAR BILANGAN:</color></b>\n• Beli/Untung/Barang Bagus = Positif (+)\n• Jual/Rugi/Utang/Barang Busuk = Negatif (-)\n• Menyingkirkan/Membatalkan = Dikurang (-)\n<i>Tekan tombol hijau untuk mencoba Contoh Soal!</i></size>";
            teksFeedback.color = Color.white;
        }

        if (tombolAtasObjek != null) tombolAtasObjek.SetActive(true);
        if (tombolBawahObjek != null) tombolBawahObjek.SetActive(false);
        if (teksTombolAtas != null) teksTombolAtas.text = "COBA >>";
    }

    void MasukFaseLatihan()
    {
        faseSaatIni = FaseGame.Latihan;
        sedangMenampilkanPenjelasan = false;

        if (tombolAtasObjek != null) tombolAtasObjek.SetActive(true);
        if (tombolBawahObjek != null) tombolBawahObjek.SetActive(true);

        if (teksCounter != null) teksCounter.text = "<size=11>CONTOH</size>";
        if (teksJudulSoal != null) teksJudulSoal.text = "[CONTOH] " + soalContoh.templateNarasi;

        if (ilustrasiGambar != null)
        {
            Sprite gambarDipilih = AmbilGambarSesuaiTag(soalContoh.tag);
            if (gambarDipilih != null)
            {
                ilustrasiGambar.sprite = gambarDipilih;
                ilustrasiGambar.enabled = true;
            }
            else
            {
                ilustrasiGambar.enabled = false;
            }
        }

        if (teksTombolAtas != null) teksTombolAtas.text = soalContoh.templateOpsiAtas;
        if (teksTombolBawah != null) teksTombolBawah.text = soalContoh.templateOpsiBawah;

        if (teksFeedback != null)
        {
            teksFeedback.text = "<color=yellow>[MODE CONTOH]</color> Pilih ekspresi matematika yang paling tepat! (Aman jika salah)";
            teksFeedback.color = Color.white;
        }
    }

    void MasukFaseKuisAsli()
    {
        faseSaatIni = FaseGame.KuisAsli;
        streakBenar = 0;
        mengulangSoalYangSama = false;
        AmbilSoalKuis();
    }

    void AmbilSoalKuis()
    {
        if (daftarSoal == null || daftarSoal.Count == 0)
        {
            SelesaikanMiniGame();
            return;
        }

        sedangMenampilkanPenjelasan = false;

        if (!mengulangSoalYangSama)
        {
            if (soalBelumDimainkan.Count == 0)
            {
                IsiUlangKantongSoal();
            }

            int indeksDalamKantong = Random.Range(0, soalBelumDimainkan.Count);
            indeksSoalAktif = soalBelumDimainkan[indeksDalamKantong];
            soalBelumDimainkan.RemoveAt(indeksDalamKantong);
        }

        TampilkanSoalKuisKeUI();
    }

    void TampilkanSoalKuisKeUI()
    {
        SoalPasarDatabase.SoalPasar soal = daftarSoal[indeksSoalAktif];

        if (tombolAtasObjek != null) tombolAtasObjek.SetActive(true);
        if (tombolBawahObjek != null) tombolBawahObjek.SetActive(true);

        if (teksCounter != null) teksCounter.text = "Streak: <color=yellow>" + streakBenar + "</color> / " + targetStreak;
        if (teksJudulSoal != null) teksJudulSoal.text = soal.templateNarasi;

        if (ilustrasiGambar != null)
        {
            Sprite gambarDipilih = AmbilGambarSesuaiTag(soal.tag);
            if (gambarDipilih != null)
            {
                ilustrasiGambar.sprite = gambarDipilih;
                ilustrasiGambar.enabled = true;
            }
            else
            {
                ilustrasiGambar.enabled = false;
            }
        }

        if (teksTombolAtas != null) teksTombolAtas.text = soal.templateOpsiAtas;
        if (teksTombolBawah != null) teksTombolBawah.text = soal.templateOpsiBawah;

        if (teksFeedback != null)
        {
            teksFeedback.text = "Manakah kalimat matematika yang benar?";
            teksFeedback.color = Color.white;
        }
    }

    public void CekJawaban(bool pilihAtas)
    {
        if (sedangMenampilkanPenjelasan) return;

        // --- FASE INSTRUKSI ---
        if (faseSaatIni == FaseGame.Instruksi)
        {
            MasukFaseLatihan();
            return;
        }

        // --- FASE LATIHAN (Aman jika salah) ---
        if (faseSaatIni == FaseGame.Latihan)
        {
            if (pilihAtas == soalContoh.apakahOpsiAtasBenar)
            {
                sedangMenampilkanPenjelasan = true;
                if (teksFeedback != null)
                {
                    teksFeedback.text = "<color=green>BENAR SEKALI!</color>\n" + soalContoh.hintJikaSalah +
                                        "\n<b><color=yellow>Bersiaplah, Kuis Level " + levelSaatIni + " akan dimulai!</color></b>";
                }
                Invoke("MasukFaseKuisAsli", 3.5f);
            }
            else
            {
                // Salah di latihan → TIDAK mengurangi nyawa
                if (teksFeedback != null)
                {
                    teksFeedback.text = "<color=orange>KURANG TEPAT!</color> " + soalContoh.hintJikaSalah + 
                                        ". Coba klik tombol yang satunya!";
                }
            }
            return;
        }

        // --- FASE KUIS ASLI ---
        if (faseSaatIni == FaseGame.KuisAsli)
        {
            SoalPasarDatabase.SoalPasar soal = daftarSoal[indeksSoalAktif];
            bool isBenar = (pilihAtas == soal.apakahOpsiAtasBenar);
            string qId = $"S2-L1-S{levelSaatIni}-Q{streakBenar + 1:D2}";
            Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {streakBenar + 1}/{targetStreak}");

            if (isBenar)
            {
                Debug.Log($"[ANSWER]\nCorrect");
                GameStatisticsManager.CatatJawabanBenar(qId);

                // ========== BENAR ==========
                sedangMenampilkanPenjelasan = true;
                streakBenar++;
                totalKoin += 5;

                GameStatisticsManager.TambahKoin(5, 2, 1, levelSaatIni);

                mengulangSoalYangSama = false;
                UpdateUIKoin();

                if (teksFeedback != null)
                {
                    teksFeedback.text = "<color=green>BENAR! (+5 Koin)</color>\nPertahankan rentetan jawaban benarmu!";
                }

                if (streakBenar >= targetStreak)
                {
                    Invoke("SelesaikanMiniGame", 1.5f);
                }
                else
                {
                    Invoke("AmbilSoalKuis", 1.5f);
                }
            }
            else
            {
                Debug.Log($"[ANSWER]\nWrong");
                GameStatisticsManager.CatatJawabanSalah(qId);

                // ========== SALAH ==========
                nyawa--;
                mengulangSoalYangSama = false; // soal tidak diulang

                if (ikonHati != null && nyawa >= 0 && nyawa < ikonHati.Length)
                {
                    if (ikonHati[nyawa] != null) ikonHati[nyawa].SetActive(false);
                }

                if (nyawa <= 0)
                {
                    // Sudah 3x salah → pindah ke Menu_Pencapaian
                    sedangMenampilkanPenjelasan = true;
                    if (teksFeedback != null)
                        teksFeedback.text = "<color=red>NYAWA HABIS!</color> Kamu gagal mematahkan kutukan pasar!";
                    Invoke("KalahGame", 2f);
                }
                else
                {
                    sedangMenampilkanPenjelasan = true;
                    if (teksFeedback != null)
                    {
                        teksFeedback.text = "<color=red>SALAH!</color>\n<b>Hint:</b> " + soal.hintJikaSalah +
                                            "\n<i><color=yellow>Streak tetap " + streakBenar + ". Pahami hint-nya lalu lanjut soal berikutnya...</color></i>";
                    }
                    Invoke("AmbilSoalKuis", 4f);
                }
            }
        }
    }

    void UpdateUIKoin()
    {
        if (teksKoin != null) teksKoin.text = totalKoin.ToString();
    }

    void SelesaikanMiniGame()
    {
        faseSaatIni = FaseGame.Selesai;
        GameStatisticsManager.BerikanRewardLevel(2, 1, levelSaatIni, 25);

        if (panelMiniGame != null) panelMiniGame.SetActive(false);
        WinManager.TampilkanWin(panelWinUi);
    }

    void KalahGame()
    {
        if (panelMiniGame != null) panelMiniGame.SetActive(false);

        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Pencapaian");
    }
}