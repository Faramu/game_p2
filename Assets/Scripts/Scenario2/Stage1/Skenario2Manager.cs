using UnityEngine;

public class Skenario2Manager : MonoBehaviour
{
    public DialogManager dialogManager;
    public MisiManager misiManager; 

    public static Skenario2Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start() 
    {
        IntroVideoManager introMgr = FindObjectOfType<IntroVideoManager>();
        if (introMgr != null && introMgr.gameObject.activeInHierarchy)
        {
            return;
        }
        LanjutkanSetelahVideo();
    }

    public void LanjutkanSetelahVideo()
    {
        // Jalankan dialog yang disesuaikan dengan level yang sedang aktif
        MulaiDialogSesuaiLevel();
    }

    public void MulaiDialogSesuaiLevel()
    {
        // 1. Cek level berapa yang sedang aktif di MiniGamePasarManager
        int levelAktif = 1;
        MiniGamePasarManager miniGame = Object.FindFirstObjectByType<MiniGamePasarManager>();
        
        if (miniGame != null)
        {
            levelAktif = miniGame.levelSaatIni;
        }

        // 2. Ambil daftar dialog sesuai level tersebut
        BarisDialog[] percakapanPasar = GetPercakapanSesuaiLevel(levelAktif);

        // 3. Tampilkan dialog ke UI
        if (dialogManager != null)
        {
            dialogManager.TampilkanDialog(percakapanPasar, () => {
                Debug.Log("Dialog Level " + levelAktif + " Selesai. Membuka Tutorial terlebih dahulu!");
                BukaPanelInformasiTutorial();
            });
        }
    }

    // =========================================================================
    // BANK DIALOG MENYESUAIKAN DENGAN TINGKAT KESULITAN LEVEL
    // =========================================================================
    private BarisDialog[] GetPercakapanSesuaiLevel(int level)
    {
        switch (level)
        {
            case 1: // LEVEL 1: KONSEP DASAR (MUDAH)
                return new BarisDialog[] {
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Tolong! Kutukan Kerugian ini membuat catatan transaksi dasarku kacau semua!" },
                    new BarisDialog { namaPembicara = "Player", kalimat = "Tenang, Pak Pedagang. Apa yang sebenarnya terjadi di pasar ini?" },
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Aku bingung menentukan transaksi mana yang termasuk untung/tambah (+) dan rugi/kurang (-)!" },
                    new BarisDialog { namaPembicara = "Player", kalimat = "Ingat Pak, membeli atau mendapat barang itu Positif (+), sedangkan menjual atau membuang itu Negatif (-)!" },
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Benar! Tolong bantu aku mengelompokkan transaksi dasar ini agar pasar bisa normal kembali!" }
                };

            case 2: // LEVEL 2: KOMBINASI TANDA & DOUBLE NEGATIVE (SEDANG)
                return new BarisDialog[] {
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Aduh, kutukannya makin parah! Catatanku makin rumit dengan transaksi barang busuk dan utang!" },
                    new BarisDialog { namaPembicara = "Player", kalimat = "Jangan panik Pak. Kali ini kita harus memperhatikan kombinasi dua tanda plus dan minus." },
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Misalnya kalau aku membuang barang busuk atau membatalkan utang, hasilnya jadi untung atau rugi ya?" },
                    new BarisDialog { namaPembicara = "Player", kalimat = "Membuang (-) barang busuk (-) sama saja dengan menguntungkan toko (+)! Mari kita hitung bersama!" },
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Bagus sekali! Tolong bantu aku membenahi transaksi gabungan ini!" }
                };

            case 3: // LEVEL 3: PERKALIAN, PEMBAGIAN, & SULIT
                return new BarisDialog[] {
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Gawat! Kutukan Kerugian ini memicu kerugian berantai selama berhari-hari!" },
                    new BarisDialog { namaPembicara = "Player", kalimat = "Tampaknya kita harus menghitung perkalian, pembagian, dan operasi gabungan bilangan bulat!" },
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Iya! Ada rugi berturut-turut dan utang pasar yang harus dibagi rata ke beberapa mitra!" },
                    new BarisDialog { namaPembicara = "Player", kalimat = "Siap Pak, ini tantangan matematika tingkat lanjut. Saya pasti bisa membantu mematahkan kutukan ini!" },
                    new BarisDialog { namaPembicara = "Pedagang", kalimat = "Terima kasih banyak! Ini adalah tahap akhir untuk menyelamatkan Putri Aruna dari gudang!" }
                };

            default:
                return GetPercakapanSesuaiLevel(1);
        }
    }

    public void UpdateProgresPasar(bool jawabanBenar)
    {
        if (jawabanBenar)
        {
            Debug.Log("Satu langkah lagi mematahkan kutukan!");
        }
    }

    private void BukaPanelInformasiTutorial()
    {
        Debug.Log("Mengarahkan player ke Papan Informasi / Tutorial...");
        
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.StartTutorial();
        }
        else
        {
            Debug.LogError("TutorialManager tidak ditemukan di Scene Stage 2!");
        }
    }

    public void MulaiSkenarioPasar()
    {
        Debug.Log("Mulai Skenario Pasar dipanggil oleh Interaksi NPC!");
        MulaiDialogSesuaiLevel();
    }
}