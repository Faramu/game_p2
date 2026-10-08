using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class GoaFaktorManager2 : MonoBehaviour
{
    public static GoaFaktorManager2 Instance;

    public enum GameState { DialogAwal, PilihObor, Feedback, GameOver, Selesai }

    [Header("State Permainan")]
    public GameState currentState = GameState.DialogAwal;

    [Header("--- PENGATURAN LEVEL ---")]
    [Tooltip("1 = faktor biasa, 2 = urutan faktor kecil→besar, 3 = negatif (nanti)")]
    public int subLevelSaatIni = 2;
    public int targetSoalBerhasil = 5;

    [Header("UI & Dialog Pengantar / Feedback")]
    public GameObject panelDialogNPC;
    public TextMeshProUGUI teksDialog;
    public GameObject panelInstruksi;
    public TextMeshProUGUI teksInstruksi;
    public GameObject tombolLanjutFeedback;

    [Header("UI Urutan Pilihan")]
    public TextMeshProUGUI teksUrutanPilihan;   // tampil: "Urutan: 1 → 2 → 4"

    [Header("HUD Koin")]
    public TextMeshProUGUI teksJumlahKoin;

    [Header("Sistem Nyawa & HUD Hati")]
    public int nyawaPemain = 3;
    public Image[] slotGambarHati;
    public Sprite spriteHatiRetak;
    public GameObject panelGameOver;

    [Header("Referensi Objek di Scene")]
    public OborFaktor[] arrayObjekObor;
    public GerbangKeluar gerbangKeluar;

    [Header("=== KELELAWAR (Kontrol per Index) ===")]
    public GameObject[] daftarKelelawar;

    [Header("Pengaturan Serangan Kelelawar")]
    public float kecepatanTerbang = 15f;
    public string namaAnimasiMenyerang = "KelelawarMenyerang";

    [Header("Dialog Pages Pengantar")]
    [TextArea(3, 5)]
    public string[] halamanDialog;

    // ====================== PRIVATE ======================
    private int indeksHalaman = 0;
    private List<DataSoalObor> bankSoalLevel = new List<DataSoalObor>();
    private int indeksSoalAktif = 0;
    private bool jawabanTerakhirBenar = false;
    private int indeksKelelawarBerikutnya = 0;

    // Urutan klik player
    private List<int> urutanPilihanPlayer = new List<int>();

    private void Awake()
    {
        Instance = this;
        bankSoalLevel = GoaFaktorDatabase2.MuatSoal(subLevelSaatIni);
    }

    private void Start()
    {
        if (panelInstruksi != null) panelInstruksi.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (tombolLanjutFeedback != null) tombolLanjutFeedback.SetActive(false);

        if (gerbangKeluar != null)
            gerbangKeluar.gameObject.SetActive(false);

        // Sembunyikan teks urutan di awal
        if (teksUrutanPilihan != null)
        {
            teksUrutanPilihan.gameObject.SetActive(false);
            teksUrutanPilihan.text = "Urutan: -";
        }

        ResetSemuaKelelawar();
        UpdateTeksKoin();
        MulaiDialogAwal();
    }

    // =========================================================
    //  DIALOG
    // =========================================================
    void MulaiDialogAwal()
    {
        currentState = GameState.DialogAwal;

        if (panelDialogNPC != null)
            panelDialogNPC.SetActive(true);

        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.SetActive(true);

        if (halamanDialog == null || halamanDialog.Length == 0)
        {
            halamanDialog = new string[]
            {
                "Ksatria, kelelawar di area ini lebih licik dari sebelumnya!",
                "Sekarang tidak cukup hanya memilih faktor. Kamu harus menyalakan obor dalam URUTAN dari yang TERKECIL ke yang TERBESAR.",
                "Contoh: jika target 4, faktornya 1, 2, dan 4.\nNyalakan berurutan: 1 → 2 → 4.\nSalah urutan atau memilih angka bukan faktor = kelelawar menyerang!"
            };
        }

        indeksHalaman = 0;
        if (teksDialog != null)
            teksDialog.text = halamanDialog[indeksHalaman];
    }

    public void KlikTombolLanjut()
    {
        if (currentState == GameState.DialogAwal)
        {
            indeksHalaman++;
            if (indeksHalaman < halamanDialog.Length)
            {
                if (teksDialog != null)
                    teksDialog.text = halamanDialog[indeksHalaman];
            }
            else
            {
                SelesaiDialogDanMulaiGame();
            }
        }
        else if (currentState == GameState.Feedback)
        {
            TutupFeedbackDanLanjutkan();
        }
    }

    void SelesaiDialogDanMulaiGame()
    {
        if (panelDialogNPC != null)
            panelDialogNPC.SetActive(false);

        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.SetActive(false);

        currentState = GameState.PilihObor;

        if (panelInstruksi != null)
            panelInstruksi.SetActive(true);

        TampilkanSoalAktif();
    }

    // =========================================================
    //  SOAL
    // =========================================================
    public void TampilkanSoalAktif()
    {
        if (indeksSoalAktif >= targetSoalBerhasil || indeksSoalAktif >= bankSoalLevel.Count)
            return;

        // Reset urutan setiap soal baru
        urutanPilihanPlayer.Clear();
        UpdateTeksUrutan();

        // Tampilkan teks urutan saat soal muncul
        if (teksUrutanPilihan != null)
            teksUrutanPilihan.gameObject.SetActive(true);

        DataSoalObor soal = bankSoalLevel[indeksSoalAktif];

        if (teksInstruksi != null)
        {
            teksInstruksi.text =
                $"[Soal {indeksSoalAktif + 1}/{targetSoalBerhasil}] {soal.narasiMisi}\n" +
                $"Target: <b>{soal.angkaTarget}</b> | Urutkan faktor dari kecil → besar!";
        }

        for (int i = 0; i < arrayObjekObor.Length; i++)
        {
            if (arrayObjekObor[i] == null) continue;

            if (i < soal.pilihanAngkaObor.Length)
            {
                int nilai = soal.pilihanAngkaObor[i];
                bool termasukUrutan = soal.urutanBenar != null && System.Linq.Enumerable.Contains(soal.urutanBenar, nilai);

                arrayObjekObor[i].gameObject.SetActive(true);
                arrayObjekObor[i].SetupObor(nilai, termasukUrutan);
            }
            else
            {
                arrayObjekObor[i].gameObject.SetActive(false);
            }
        }
    }

    // =========================================================
    //  SISTEM URUTAN
    // =========================================================
    public void TambahPilihanKeUrutan(OborFaktor obor)
    {
        if (currentState != GameState.PilihObor) return;
        if (obor == null) return;

        urutanPilihanPlayer.Add(obor.nilaiAngkaObor);
        UpdateTeksUrutan();

        Debug.Log($"<color=cyan>[URUTAN]</color> Player pilih: {string.Join(" → ", urutanPilihanPlayer)}");

        DataSoalObor soal = bankSoalLevel[indeksSoalAktif];

        // Validasi otomatis saat jumlah klik = jumlah faktor yang harus dipilih
        if (soal.urutanBenar != null && urutanPilihanPlayer.Count >= soal.urutanBenar.Length)
        {
            ValidasiUrutan();
        }
    }

    void UpdateTeksUrutan()
    {
        if (teksUrutanPilihan == null) return;

        if (urutanPilihanPlayer.Count == 0)
            teksUrutanPilihan.text = "Urutan: -";
        else
            teksUrutanPilihan.text = "Urutan: " + string.Join(" → ", urutanPilihanPlayer);
    }

    public void HapusPilihanDariUrutan(OborFaktor obor)
    {
        if (currentState != GameState.PilihObor) return;
        if (obor == null) return;

        // Hapus 1 kemunculan nilai ini dari urutan (dari belakang biar yang terakhir diklik yang hilang)
        for (int i = urutanPilihanPlayer.Count - 1; i >= 0; i--)
        {
            if (urutanPilihanPlayer[i] == obor.nilaiAngkaObor)
            {
                urutanPilihanPlayer.RemoveAt(i);
                break;
            }
        }

        UpdateTeksUrutan();
        Debug.Log($"<color=orange>[URUTAN]</color> Setelah batal: {string.Join(" → ", urutanPilihanPlayer)}");
    }

    void ValidasiUrutan()
    {
        DataSoalObor soal = bankSoalLevel[indeksSoalAktif];

        // Kunci semua obor
        foreach (var obor in arrayObjekObor)
        {
            if (obor != null)
                obor.KunciObor();
        }

        bool sama = false;
        if (soal.urutanBenar != null && urutanPilihanPlayer.Count == soal.urutanBenar.Length)
        {
            sama = true;
            for (int i = 0; i < urutanPilihanPlayer.Count; i++)
            {
                if (urutanPilihanPlayer[i] != soal.urutanBenar[i])
                {
                    sama = false;
                    break;
                }
            }
        }

        if (sama)
        {
            // ========== BENAR ==========
            UpdatePencapaian.CatatJawaban(true);
            UpdatePencapaian.TambahKoin(10);
            UpdateTeksKoin();
            jawabanTerakhirBenar = true;

            HilangkanKelelawarBerikutnya();
            TampilkanFeedbackEdukasi(true, soal.penjelasanBenar);
        }
        else
        {
            // ========== SALAH ==========
            UpdatePencapaian.CatatJawaban(false);
            jawabanTerakhirBenar = false;

            KelelawarMenyerangSemuaYangAktif();
            PemainTerkenaDamage();
            TampilkanFeedbackEdukasi(false, soal.penjelasanSalah);
        }
    }

    // Method lama tetap ada agar tidak error jika masih dipanggil di tempat lain
    public void ValidasiPilihanObor(OborFaktor oborDipilih)
    {
        // Dialihkan ke sistem urutan baru
        TambahPilihanKeUrutan(oborDipilih);
    }

    void TampilkanFeedbackEdukasi(bool isBenar, string penjelasan)
    {
        currentState = GameState.Feedback;

        // Sembunyikan teks urutan saat feedback
        if (teksUrutanPilihan != null)
            teksUrutanPilihan.gameObject.SetActive(false);

        if (panelInstruksi != null)
            panelInstruksi.SetActive(true);

        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.SetActive(true);

        if (isBenar)
        {
            if (teksInstruksi != null)
                teksInstruksi.text = "<color=green><b>URUTAN BENAR! (+10 Koin)</b></color>\n" + penjelasan;
        }
        else
        {
            if (teksInstruksi != null)
                teksInstruksi.text = "<color=red><b>URUTAN SALAH!</b></color>\n" + penjelasan + "\n(Nyawa berkurang 1)";
        }
    }

    void TutupFeedbackDanLanjutkan()
    {
        if (tombolLanjutFeedback != null)
            tombolLanjutFeedback.SetActive(false);

        if (jawabanTerakhirBenar)
        {
            indeksSoalAktif++;

            if (indeksSoalAktif >= targetSoalBerhasil)
            {
                // ========== SEMUA SOAL SELESAI ==========
                currentState = GameState.Selesai;
                KaburkanSemuaKelelawar();

                // Sembunyikan teks urutan
                if (teksUrutanPilihan != null)
                    teksUrutanPilihan.gameObject.SetActive(false);

                // Instruksi baru: minta player berjalan lurus ke depan mencari pintu goa
                if (teksInstruksi != null)
                {
                    teksInstruksi.text =
                        "<color=red><b>SEMUA TANTANGAN SELESAI!</b></color>\n" +
                        "Jalan keluar telah terbuka.\n" +
                        "Berjalanlah <b>lurus ke depan</b> untuk menemukan pintu goa dan bukalah!";
                }

                if (gerbangKeluar != null)
                    gerbangKeluar.AktifkanGerbang();

                GameStatisticsManager.BerikanRewardLevel(4, 2, subLevelSaatIni, 25);
            }
            else
            {
                currentState = GameState.PilihObor;
                TampilkanSoalAktif();
            }
        }
        else
        {
            // Ulangi soal yang sama
            currentState = GameState.PilihObor;
            TampilkanSoalAktif();
        }
    }

    // =========================================================
    //  KELELAWAR
    // =========================================================
    void HilangkanKelelawarBerikutnya()
    {
        if (daftarKelelawar == null || daftarKelelawar.Length == 0)
        {
            Debug.LogWarning("[KELELAWAR] daftarKelelawar masih kosong di Inspector!");
            return;
        }

        while (indeksKelelawarBerikutnya < daftarKelelawar.Length)
        {
            GameObject kel = daftarKelelawar[indeksKelelawarBerikutnya];
            if (kel != null && kel.activeInHierarchy)
            {
                kel.SetActive(false);
                Debug.Log($"<color=yellow>[KELELAWAR]</color> Kelelawar index {indeksKelelawarBerikutnya} dihilangkan!");
                indeksKelelawarBerikutnya++;
                return;
            }
            indeksKelelawarBerikutnya++;
        }
    }

    void KelelawarMenyerangSemuaYangAktif()
    {
        if (daftarKelelawar == null) return;

        StopAllCoroutines();

        for (int i = 0; i < daftarKelelawar.Length; i++)
        {
            GameObject kel = daftarKelelawar[i];
            if (kel != null && kel.activeInHierarchy)
                StartCoroutine(ProsesTerbangMenyerang(kel));
        }
    }

    IEnumerator ProsesTerbangMenyerang(GameObject kelelawar)
    {
        if (kelelawar == null) yield break;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[KELELAWAR] Tidak menemukan objek dengan Tag 'Player'!");
            yield break;
        }

        Vector3 posisiAwal = kelelawar.transform.position;
        Vector3 posisiTarget = player.transform.position;

        Animator anim = kelelawar.GetComponent<Animator>();
        if (anim != null && !string.IsNullOrEmpty(namaAnimasiMenyerang))
            anim.Play(namaAnimasiMenyerang, 0, 0f);

        while (kelelawar != null && kelelawar.activeInHierarchy &&
               Vector3.Distance(kelelawar.transform.position, posisiTarget) > 0.6f)
        {
            kelelawar.transform.position = Vector3.MoveTowards(
                kelelawar.transform.position, posisiTarget, kecepatanTerbang * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (kelelawar != null && kelelawar.activeInHierarchy &&
               Vector3.Distance(kelelawar.transform.position, posisiAwal) > 0.1f)
        {
            kelelawar.transform.position = Vector3.MoveTowards(
                kelelawar.transform.position, posisiAwal, kecepatanTerbang * Time.deltaTime);
            yield return null;
        }

        if (kelelawar != null)
            kelelawar.transform.position = posisiAwal;
    }

    void KaburkanSemuaKelelawar()
    {
        if (daftarKelelawar == null) return;

        StopAllCoroutines();

        foreach (var kel in daftarKelelawar)
        {
            if (kel != null)
                kel.SetActive(false);
        }
    }

    void ResetSemuaKelelawar()
    {
        indeksKelelawarBerikutnya = 0;

        if (daftarKelelawar == null) return;

        foreach (var kel in daftarKelelawar)
        {
            if (kel != null)
                kel.SetActive(true);
        }
    }

    // =========================================================
    //  NYAWA & GAME OVER
    // =========================================================
    void PemainTerkenaDamage()
    {
        nyawaPemain--;
        Debug.Log($"<color=orange>[DAMAGE]</color> Sisa nyawa: {nyawaPemain}");

        if (slotGambarHati != null && nyawaPemain >= 0 && nyawaPemain < slotGambarHati.Length)
        {
            if (spriteHatiRetak != null)
                slotGambarHati[nyawaPemain].sprite = spriteHatiRetak;
            else
                slotGambarHati[nyawaPemain].gameObject.SetActive(false);
        }

        if (nyawaPemain <= 0)
            ProsesGameOver();
    }

    void ProsesGameOver()
    {
        currentState = GameState.GameOver;

        if (panelInstruksi != null)
            panelInstruksi.SetActive(false);

        // Pastikan teks urutan juga disembunyikan
        if (teksUrutanPilihan != null)
            teksUrutanPilihan.gameObject.SetActive(false);

        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        PlayerPrefs.Save();
        Invoke(nameof(PindahKeMenuPencapaian), 3f);
    }

    public void RestartLevel()
    {
        PindahKeMenuPencapaian();
    }

    private void PindahKeMenuPencapaian()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu_Pencapaian");
    }

    // =========================================================
    //  KOIN
    // =========================================================
    void UpdateTeksKoin()
    {
        if (teksJumlahKoin == null) return;
        teksJumlahKoin.text = GameStatisticsManager.GetTotalKoin().ToString();
    }
}