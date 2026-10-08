using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GerbangUjungManager : MonoBehaviour
{
    public static GerbangUjungManager Instance { get; private set; }

    [System.Serializable]
    public class SoalGerbang
    {
        [TextArea(2, 4)] public string narasiSoal;
        public string notasiAngka;
        public int jawabanBenar;
        [TextArea(2, 3)] public string hintJikaSalah;
    }

    [Header("--- PENGATURAN EVALUASI AKHIR ---")]
    public int targetBenarTotal = 10;
    private int jumlahBenarSaatIni = 0;
    private int jumlahSalahSaatIni = 0;

    [Header("--- BANK SOAL GERBANG ---")]
    public List<SoalGerbang> daftarSoalUtama = new List<SoalGerbang>();
    private List<SoalGerbang> daftarSoalSisa = new List<SoalGerbang>();
    private SoalGerbang soalAktif;

    [Header("--- REFERENSI UI GERBANG ---")]
    public GameObject panelGerbang;
    public TextMeshProUGUI teksSoalGerbang;
    public TextMeshProUGUI teksProgresGerbang;
    public TextMeshProUGUI teksFeedbackGerbang;
    public TMP_InputField inputJawabanGerbang;
    public GameObject tombolSubmitGerbang;

    [Header("--- REFERENSI UI EVALUASI ---")]
    [Tooltip("Isi teks panel ini manual di Inspector")]
    public GameObject panelEvaluasi;

    [Header("--- PENGATURAN PINDAH SCENE ---")]
    public string namaSceneStage3 = "Stage3_RumahPohon";
    public GameObject panelFadeToBlack;

    private bool sedangMenampilkanPenjelasan = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (panelGerbang != null) panelGerbang.SetActive(false);
        if (panelFadeToBlack != null) panelFadeToBlack.SetActive(false);
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);

        InisialisasiSoalGerbang();
    }

    // =========================================================================
    // BANK SOAL
    // =========================================================================
    void InisialisasiSoalGerbang()
    {
        daftarSoalUtama.Clear();

        TambahSoal("Pedagang membawa 5 kg apel dan 3 kg mangga. Di perjalanan, 8 kg buah gabungan tersebut busuk dan dibuang. Berapa sisa buah yang bisa dijual?", "(5 + 3) - 8 = ?", 0, "Hitung: 5 + 3 = 8, lalu 8 - 8 = 0 kg.");
        TambahSoal("Toko punya persediaan 10 kg apel dan 5 kg jeruk. Hari ini terjual 15 kg dari total buah tersebut. Berapa sisa persediaan buah di toko?", "(10 + 5) - 15 = ?", 0, "Hitung: 10 + 5 = 15, lalu 15 - 15 = 0 kg.");
        TambahSoal("Pak Budi rugi 8 kg pisang karena busuk, lalu menjual 5 kg mangga, namun kehilangan lagi 3 kg jeruk. Berapa total perubahan persediaan buah Pak Budi?", "-8 + 5 - 3 = ?", -6, "Hitung berurutan: -8 + 5 = -3, lalu -3 - 3 = -6 kg.");
        TambahSoal("Kios menerima 4 kg anggur dan 7 kg apel, tetapi 13 kg gabungan buah di kios rusak akibat banjir. Berapa posisi persediaan buah kios?", "(4 + 7) - 13 = ?", -2, "Hitung: 4 + 7 = 11, lalu 11 - 13 = -2 kg.");
        TambahSoal("Toko buah untung dari penjualan 12 kg mangga, rugi 15 kg jeruk yang busuk, lalu untung lagi dari penjualan 5 kg apel. Berapa total perubahan persediaan/keuangan buah?", "12 - 15 + 5 = ?", 2, "Hitung berurutan: 12 - 15 = -3, lalu -3 + 5 = 2.");
        TambahSoal("Kios kekurangan stok 10 kg semangka, mendapat pasokan 15 kg melon, lalu membuang 10 kg jeruk busuk. Berapa posisi neraca stok buah kios?", "-10 + 15 - 10 = ?", -5, "Hitung berurutan: -10 + 15 = 5, lalu 5 - 10 = -5 kg.");
        TambahSoal("Ani membeli 15 kg jeruk (dicatat -15), lalu menjual 20 kg mangga (+20), dan membuang 3 kg apel busuk (-3). Berapa total perubahan persediaan buah?", "-15 + 20 - 3 = ?", 2, "Hitung berurutan: -15 + 20 = 5, lalu 5 - 3 = 2 kg.");
        TambahSoal("Keranjang buah mengalami penyusutan 4 kg pisang dan 2 kg durian, lalu mendapat tambahan 5 kg salak segar. Berapa total perubahan bobot buah di keranjang?", "-4 - 2 + 5 = ?", -1, "Hitung berurutan: -4 - 2 = -6, lalu -6 + 5 = -1 kg.");
        TambahSoal("Pedagang membawa 10 kg apel. Di pasar, 3 kg apel busuk (-3) dan 5 kg mangga terjual (-5). Berapa total sisa persediaan buah?", "10 - 3 - 5 = ?", 2, "Hitung berurutan: 10 - 3 = 7, lalu 7 - 5 = 2 kg.");
        TambahSoal("Minggu lalu toko untung 7 kg stok pir, minggu ini rugi 10 kg nangka busuk, dan minggu depan diprediksi rugi 4 kg buah naga. Berapa akumulasi perubahan persediaan?", "7 - 10 - 4 = ?", -7, "Hitung berurutan: 7 - 10 = -3, lalu -3 - 4 = -7 kg.");
        TambahSoal("Kios kekurangan 5 kg alpukat, menerima pasokan 5 kg kelengkeng, lalu mendapat tambahan 3 kg jambu. Berapa total persediaan buah sekarang?", "-5 + 5 + 3 = ?", 3, "Hitung berurutan: -5 + 5 = 0, lalu 0 + 3 = 3 kg.");
        TambahSoal("Persediaan stroberi susut 6 kg, dipasok 9 kg kiwi, lalu pepaya busuk 4 kg. Berapa total perubahan stok buah?", "-6 + 9 - 4 = ?", -1, "Hitung berurutan: -6 + 9 = 3, lalu 3 - 4 = -1 kg.");
        TambahSoal("Kasir mencatat untung penjualan 15 kg rambutan, rugi 20 kg duku busuk, dan dapat pasokan 10 kg sirsak. Berapa posisi akhir stok buah?", "15 - 20 + 10 = ?", 5, "Hitung berurutan: 15 - 20 = -5, lalu -5 + 10 = 5 kg.");
        TambahSoal("Pembeli mengutang 15 kg kedondong, mencicil dengan menyerahkan 10 kg sawo, lalu mengutang lagi 6 kg markisa. Berapa posisi neraca buah pembeli?", "-15 + 10 - 6 = ?", -11, "Hitung berurutan: -15 + 10 = -5, lalu -5 - 6 = -11 kg.");
        TambahSoal("Kualitas gelombang 1 manggis -3, gelombang 2 jambu air +8, dan gelombang 3 lengkeng -7. Berapa total skor kualitas ketiga buah tersebut?", "-3 + 8 - 7 = ?", -2, "Hitung berurutan: -3 + 8 = 5, lalu 5 - 7 = -2.");
        TambahSoal("Pemasukan stok 5 kg apel, pengeluaran 7 kg jeruk, dan kiriman tambahan 2 kg mangga. Berapa perubahan persediaan buah di toko?", "5 - 7 + 2 = ?", 0, "Hitung berurutan: 5 - 7 = -2, lalu -2 + 2 = 0 kg.");
        TambahSoal("Truk membawa stok buah: berkurang 10 kg pisang, bertambah 12 kg melon, dan berkurang lagi 6 kg semangka. Berapa perubahan total stok di truk?", "-10 + 12 - 6 = ?", -4, "Hitung berurutan: -10 + 12 = 2, lalu 2 - 6 = -4 kg.");
        TambahSoal("Toko rugi 4 kg anggur, rugi lagi 5 kg durian, lalu mendapat bantuan pasokan 8 kg nangka. Berapa posisi persediaan buah toko?", "-4 - 5 + 8 = ?", -1, "Hitung berurutan: -4 - 5 = -9, lalu -9 + 8 = -1 kg.");
        TambahSoal("Kios punya 6 kg salak, kehilangan 9 kg buah naga, lalu mendapat pasokan 4 kg alpukat. Berapa sisa total persediaan buah?", "6 - 9 + 4 = ?", 1, "Hitung berurutan: 6 - 9 = -3, lalu -3 + 4 = 1 kg.");
        TambahSoal("Koperasi rugi 20 kg kelengkeng, mendapat ganti rugi 15 kg jambu, lalu rugi lagi 3 kg sirsak. Berapa total perubahan stok buah?", "-20 + 15 - 3 = ?", -8, "Hitung berurutan: -20 + 15 = -5, lalu -5 - 3 = -8 kg.");
        TambahSoal("Pasar buah mencatat: susut 2 kg rambutan, bertambah 8 kg duku, dan susut lagi 10 kg kedondong. Berapa total perubahan persediaan buah?", "-2 + 8 - 10 = ?", -4, "Hitung berurutan: -2 + 8 = 6, lalu 6 - 10 = -4 kg.");
        TambahSoal("Stok awal 10 kg sawo, dijual 14 kg markisa, lalu ada pasokan darurat 4 kg manggis. Berapa sisa neraca persediaan buah?", "10 - 14 + 4 = ?", 0, "Hitung berurutan: 10 - 14 = -4, lalu -4 + 4 = 0 kg.");
        TambahSoal("Kerugian gudang: 12 kg jambu air busuk, masuk pasokan 10 kg lengkeng, lalu 2 kg mangga rusak. Berapa total perubahan stok buah?", "-12 + 10 - 2 = ?", -4, "Hitung berurutan: -12 + 10 = -2, lalu -2 - 2 = -4 kg.");
        TambahSoal("Reputasi stan: minus 5 dari pisang busuk, minus 8 dari durian mentah, lalu plus 12 dari apel segar. Berapa poin reputasi stan?", "-5 - 8 + 12 = ?", -1, "Hitung berurutan: -5 - 8 = -13, lalu -13 + 12 = -1.");
        TambahSoal("Perubahan volume persediaan: berkurang 7 kg jeruk, bertambah 12 kg pisang, dan berkurang lagi 8 kg anggur. Berapa perubahan total persediaan?", "-7 + 12 - 8 = ?", -3, "Hitung berurutan: -7 + 12 = 5, lalu 5 - 8 = -3 kg.");
        TambahSoal("Keuntungan penjualan 15 kg manggis, dikurangi kerugian 18 kg mangga busuk, ditambah keuntungan 5 kg jeruk. Berapa total perubahan neraca buah?", "15 - 18 + 5 = ?", 2, "Hitung berurutan: 15 - 18 = -3, lalu -3 + 5 = 2 kg.");
        TambahSoal("Kekurangan stok dari supplier A adalah 5 kg apel dan supplier B 7 kg mangga. Pemilik menerima pasokan 10 kg jeruk. Berapa sisa kekurangan stok buah?", "-5 - 7 + 10 = ?", -2, "Hitung berurutan: -5 - 7 = -12, lalu -12 + 10 = -2 kg.");
        TambahSoal("Penyusutan gudang pendingin: -15 kg strawberi, bertambah 10 kg kiwi, dan bertambah lagi 3 kg pepaya. Berapa total perubahan stok buah?", "-15 + 10 + 3 = ?", -2, "Hitung berurutan: -15 + 10 = -5, lalu -5 + 3 = -2 kg.");
        TambahSoal("Persediaan awal 8 kg melon, dijual 12 kg semangka, lalu panen 3 kg nangka. Berapa posisi persediaan buah?", "8 - 12 + 3 = ?", -1, "Hitung berurutan: 8 - 12 = -4, lalu -4 + 3 = -1 kg.");
        TambahSoal("Posisi persediaan stan: kurang 6 kg sirsak, bertambah 9 kg duku, lalu dijual 5 kg rambutan. Berapa posisi neraca buah stan sekarang?", "-6 + 9 - 5 = ?", -2, "Hitung berurutan: -6 + 9 = 3, lalu 3 - 5 = -2 kg.");
    }

    void TambahSoal(string narasi, string notasi, int jawaban, string hint)
    {
        daftarSoalUtama.Add(new SoalGerbang
        {
            narasiSoal = narasi,
            notasiAngka = notasi,
            jawabanBenar = jawaban,
            hintJikaSalah = hint
        });
    }

    // =========================================================================
    // GAME GERBANG (dipanggil dari SkenarioGerbangManager setelah dialog selesai)
    // =========================================================================
    public void BukaGerbangUjung()
    {
        if (panelGerbang != null) panelGerbang.SetActive(true);

        if (tombolSubmitGerbang != null) tombolSubmitGerbang.SetActive(true);
        if (inputJawabanGerbang != null) inputJawabanGerbang.gameObject.SetActive(true);
        if (teksSoalGerbang != null) teksSoalGerbang.gameObject.SetActive(true);

        jumlahBenarSaatIni = 0;
        jumlahSalahSaatIni = 0;

        ResetPoolSoal();
        AmbilSoalGerbangBaru();
    }

    void ResetPoolSoal()
    {
        daftarSoalSisa = new List<SoalGerbang>(daftarSoalUtama);
    }

    void AmbilSoalGerbangBaru()
    {
        if (daftarSoalSisa.Count == 0) ResetPoolSoal();

        sedangMenampilkanPenjelasan = false;

        int indeksAcak = Random.Range(0, daftarSoalSisa.Count);
        soalAktif = daftarSoalSisa[indeksAcak];
        daftarSoalSisa.RemoveAt(indeksAcak);

        if (inputJawabanGerbang != null)
        {
            inputJawabanGerbang.text = "";
            inputJawabanGerbang.ActivateInputField();
        }

        TampilkanSoalGerbangKeUI();
    }

    void TampilkanSoalGerbangKeUI()
    {
        if (teksProgresGerbang != null)
            teksProgresGerbang.text = "Sandi Terbuka: <color=yellow>" + jumlahBenarSaatIni + " / " + targetBenarTotal + "</color>\n<b>Kesalahan: <color=red>" + jumlahSalahSaatIni + "/2</color></b>";

        if (teksSoalGerbang != null)
            teksSoalGerbang.text = soalAktif.narasiSoal + "\n<b><color=red>Hitung: " + soalAktif.notasiAngka + "</color></b>";

        if (teksFeedbackGerbang != null)
            teksFeedbackGerbang.text = "Masukkan angka sandi (ketik '-' jika negatif):";
    }

    public void CekJawabanGerbang()
    {
        if (sedangMenampilkanPenjelasan || inputJawabanGerbang == null) return;

        string ketikan = inputJawabanGerbang.text.Trim();

        if (int.TryParse(ketikan, out int hasilKetik))
        {
            string qId = $"S2-L3-S1-Q{jumlahBenarSaatIni + 1:D2}";
            Debug.Log($"[QUESTION]\nQuestionID={qId}\nQuestion {jumlahBenarSaatIni + 1}/{targetBenarTotal}");

            if (hasilKetik == soalAktif.jawabanBenar)
            {
                Debug.Log($"[ANSWER]\nCorrect");
                GameStatisticsManager.CatatJawabanBenar(qId);
                GameStatisticsManager.TambahKoin(10, 2, 3, 1);

                sedangMenampilkanPenjelasan = true;
                jumlahBenarSaatIni++;

                if (teksFeedbackGerbang != null)
                    teksFeedbackGerbang.text = "<color=green>SANDI DITERIMA! (+10 Koin)</color> Kunci berputar...";

                if (jumlahBenarSaatIni >= targetBenarTotal)
                {
                    if (tombolSubmitGerbang != null) tombolSubmitGerbang.SetActive(false);
                    if (inputJawabanGerbang != null) inputJawabanGerbang.gameObject.SetActive(false);
                    Invoke("MunculkanPanelEvaluasi", 1.5f);
                }
                else
                {
                    Invoke("AmbilSoalGerbangBaru", 1.5f);
                }
            }
            else
            {
                ProsesSalahGerbang();
            }
        }
        else
        {
            if (teksFeedbackGerbang != null)
                teksFeedbackGerbang.text = "<color=orange>Format sandi tidak valid!</color> Masukkan angka bulat.";
        }
    }

    void ProsesSalahGerbang()
    {
        string qId = $"S2-L3-S1-Q{jumlahBenarSaatIni + 1:D2}";
        Debug.Log($"[ANSWER]\nWrong");
        GameStatisticsManager.CatatJawabanSalah(qId);

        jumlahSalahSaatIni++;
        sedangMenampilkanPenjelasan = true;

        if (jumlahSalahSaatIni > 2)
        {
            if (teksFeedbackGerbang != null)
                teksFeedbackGerbang.text = "<color=red>SISTEM TERKUNCI! (>2 Kesalahan)</color>\nProgres pembukaan di-reset dari awal!";

            jumlahBenarSaatIni = 0;
            jumlahSalahSaatIni = 0;
            ResetPoolSoal();
            Invoke("AmbilSoalGerbangBaru", 4f);
        }
        else
        {
            if (teksFeedbackGerbang != null)
                teksFeedbackGerbang.text = "<color=red>SANDI DITOLAK!</color>\n" + soalAktif.hintJikaSalah;

            Invoke("AmbilSoalGerbangBaru", 4f);
        }
    }

    // =========================================================================
    // PANEL EVALUASI & PINDAH SCENE
    // =========================================================================
    void MunculkanPanelEvaluasi()
    {
        GameStatisticsManager.BerikanRewardLevel(2, 3, 1, 50);

        if (panelGerbang != null) panelGerbang.SetActive(false);

        if (teksSoalGerbang != null) teksSoalGerbang.gameObject.SetActive(false);
        if (teksProgresGerbang != null) teksProgresGerbang.gameObject.SetActive(false);
        if (teksFeedbackGerbang != null) teksFeedbackGerbang.gameObject.SetActive(false);
        if (inputJawabanGerbang != null) inputJawabanGerbang.gameObject.SetActive(false);
        if (tombolSubmitGerbang != null) tombolSubmitGerbang.SetActive(false);

        // Tampilkan Win UI melalui WinManager
        if (panelEvaluasi != null)
        {
            WinManager.TampilkanWin(panelEvaluasi);
        }
        else
        {
            WinManager.TampilkanWin();
        }
    }

    // Sambungkan ke tombol "Lanjut" di Panel Evaluasi
    public void TombolLanjutPindahScene()
    {
        if (panelEvaluasi != null) panelEvaluasi.SetActive(false);
        if (panelFadeToBlack != null) panelFadeToBlack.SetActive(true);

        Invoke("PindahScene", 2f);
    }

    void PindahScene()
    {
        WinManager.PindahKeMapCheckpoint();
    }
}