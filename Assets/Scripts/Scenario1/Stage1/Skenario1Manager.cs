using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Skenario1Manager : MonoBehaviour
{
    public static Skenario1Manager Instance;

    [Header("--- REFERENSI OBJEK UTAMA ---")]
    public BoxCollider2D gerbangCollider;
    public MisiManager misiManager;
    [Tooltip("Panel Misi jika ingin di-assign langsung di Inspector")]
    public GameObject panelMisi;

    [Header("--- SISTEM LEVEL & EKSPLORASI ---")]
    public int levelSaatIni = 1;
    public int totalKoin = 0;
    public int kunangTerkumpulDiLevelIni = 0;
    public int targetKunangPerLevel = 6;

    // Hitungan Misi Spesifik
    private int kunangPositifCount = 0;
    private int kunangUntungCount = 0;
    private int kunangNegatifCount = 0;
    private int kunangRugiCount = 0;

    private const string KEY_VIDEO_WATCHED = "IntroVideoWatched_Skenario1";

    private bool misiLevelSelesai = false;
    private bool sudahJalan = false;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        // 1. Tentukan sublevel berdasarkan scene aktif
        string sceneName = SceneManager.GetActiveScene().name;
        if (CheckpointManager.TryGetInfoScene(sceneName, out int stg, out int lvl, out int subLvl, out _))
        {
            levelSaatIni = subLvl;
        }
        else if (sceneName == "Stage1_Eksplorasi") levelSaatIni = 1;
        else if (sceneName == "Stage1_Eksplorasi_1") levelSaatIni = 2;
        else if (sceneName == "Stage1_Eksplorasi_2") levelSaatIni = 3;

        // Sesuaikan target kunang per sublevel
        if (levelSaatIni == 1) targetKunangPerLevel = 6;
        else if (levelSaatIni == 2) targetKunangPerLevel = 6;
        else if (levelSaatIni == 3) targetKunangPerLevel = 4;

        // Temukan panelMisi jika belum di-assign di Inspector
        if (panelMisi == null)
        {
            panelMisi = GameObject.Find("Panel_Misi_BG");
        }

        // Hubungkan panelMisi ke misiManager jika belum terhubung
        if (panelMisi != null && misiManager != null && (misiManager.panelMisi == null || misiManager.panelMisi == misiManager.gameObject))
        {
            misiManager.panelMisi = panelMisi;
        }

        // Sinkronkan tampilan Misi UI berdasarkan scene aktif
        if (misiManager != null)
        {
            misiManager.InisialisasiMisiDariSceneAktif();
        }

        IntroVideoManager introMgr = FindObjectOfType<IntroVideoManager>();
        if (introMgr != null && introMgr.gameObject.activeInHierarchy)
        {
            // Biarkan IntroVideoManager yang play/skip, nanti dia akan panggil LanjutkanSetelahVideo()
            return;
        }

        // Kalau tidak ada IntroVideoManager, langsung lanjut
        LanjutkanSetelahVideo();
    }

    void Update()
    {
        if (!sudahJalan && misiManager != null)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
            {
                sudahJalan = true;
                PlayerMulaiJalan();
            }
        }
    }

    // Dipanggil oleh IntroVideoManager setelah video selesai
    public void LanjutkanSetelahVideo()
    {
        TampilkanDialogAwalLevel(levelSaatIni);
    }

    public void TampilkanDialogAwalLevel(int level, System.Action onSelesai = null)
    {
        BarisDialog[] dialogLevel = null;

        if (level == 1)
        {
            dialogLevel = new BarisDialog[] {
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Halo petualang muda! Selamat datang di Hutan Aruna yang legendaris ini." },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Di hutan ini, semua energi diatur oleh hukum Bilangan Bulat. Kita mulai dari area pertama: Bilangan Positif (+)!" },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Bilangan positif menggambarkan segala halsifatnya NAIK, MAJU, atau UNTUNG." },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Misi pertamamu: Berjalanlah ke kanan, temukan kunang-kunang bertanda Naik dan Untung, lalu pahami maknanya!" }
            };
        }
        else if (level == 2)
        {
            dialogLevel = new BarisDialog[] {
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Luar biasa! Kamu sudah menguasai konsep bilangan positif." },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Sekarang, mari kita melangkah lebih dalam ke area berikutnya: Bilangan Negatif (-)!" },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Berbeda dengan sebelumnya, bilangan negatif melambangkan kejadian yang TURUN, MUNDUR, atau RUGI." },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Lanjutkan perjalananmu ke kanan dan kumpulkan kunang-kunang bernilai Turun dan Rugi!" }
            };
        }
        else if (level == 3)
        {
            dialogLevel = new BarisDialog[] {
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Hebat sekali! Kamu sudah memahami perbedaan antara Positif (+) dan Negatif (-)." },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Ini adalah wilayah terakhir di Hutan Aruna sebelum gerbang tebing terbuka!" },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Di area ini, kamu akan menemui campuran dari seluruh jenis energi kunang-kunang." },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Fahami kembali seluruh makna yang telah kamu pelajari untuk menyelesaikan eksplorasi hutan ini!" }
            };
        }

        if (DialogManager.Instance != null && dialogLevel != null)
        {
            DialogManager.Instance.TampilkanDialog(dialogLevel, onSelesai);
        }
        else
        {
            onSelesai?.Invoke();
        }
    }

    public void PlayerMulaiJalan()
    {
        if (misiManager != null)
        {
            misiManager.MisiSelesai(0);
        }
    }

    public void TambahKunangKunang(string[] pesanEdu, KunangEnergy.TipeSimbol tipe)
    {
        totalKoin += 10;
        kunangTerkumpulDiLevelIni++;

        UpdatePencapaian.TambahKoin(10);

        // Pastikan misi jalan (misi 0) selesai jika player mengumpulkan kunang
        if (misiManager != null)
        {
            misiManager.MisiSelesai(0);
        }

        if (tipe == KunangEnergy.TipeSimbol.Naik) kunangPositifCount++;
        if (tipe == KunangEnergy.TipeSimbol.Untung) kunangUntungCount++;
        if (tipe == KunangEnergy.TipeSimbol.Turun) kunangNegatifCount++;
        if (tipe == KunangEnergy.TipeSimbol.Rugi) kunangRugiCount++;

        BarisDialog[] dialogKunang = new BarisDialog[pesanEdu.Length];
        for (int i = 0; i < pesanEdu.Length; i++)
        {
            dialogKunang[i] = new BarisDialog { namaPembicara = "Aki Aruna", kalimat = pesanEdu[i] };
        }

        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.TampilkanDialog(dialogKunang, () => {
                CekProsesLevel();
            });
        }
    }

    private void CekProsesLevel()
    {
        if (misiManager != null)
        {
            if (levelSaatIni == 1)
            {
                if (kunangPositifCount >= 3) misiManager.MisiSelesai(1);
                if (kunangUntungCount >= 3)  misiManager.MisiSelesai(2);
            }
            else if (levelSaatIni == 2)
            {
                if (kunangNegatifCount >= 3) misiManager.MisiSelesai(1);
                if (kunangRugiCount >= 3)    misiManager.MisiSelesai(2);
            }
            else if (levelSaatIni == 3)
            {
                if (kunangTerkumpulDiLevelIni >= 3) misiManager.MisiSelesai(1);
                if (kunangTerkumpulDiLevelIni >= targetKunangPerLevel) misiManager.MisiSelesai(2);
            }
        }

        bool siapNaikLevel = false;

        if (levelSaatIni == 1)
        {
            if (kunangPositifCount >= 3 && kunangUntungCount >= 3 && kunangTerkumpulDiLevelIni >= targetKunangPerLevel)
                siapNaikLevel = true;
        }
        else if (levelSaatIni == 2)
        {
            if (kunangNegatifCount >= 3 && kunangRugiCount >= 3 && kunangTerkumpulDiLevelIni >= targetKunangPerLevel)
                siapNaikLevel = true;
        }
        else if (levelSaatIni == 3)
        {
            if (kunangTerkumpulDiLevelIni >= targetKunangPerLevel)
                siapNaikLevel = true;
        }

        if (siapNaikLevel && !misiLevelSelesai)
        {
            misiLevelSelesai = true;

            // Pastikan seluruh misi pada sublevel ini ditandai selesai
            if (misiManager != null)
            {
                misiManager.MisiSelesai(0);
                misiManager.MisiSelesai(1);
                misiManager.MisiSelesai(2);
            }

            // Tampilkan debug log yang diminta
            Debug.Log($"[MISSION] Misi Sublevel {levelSaatIni} selesai");
            Debug.Log("[MISSION] Tidak mengganti ke Sublevel berikutnya karena scene masih sama");

            // Buka gerbang agar pemain bisa melangkah ke garis finish
            BukaGerbangTebingKabut();

            BarisDialog[] dialogSelesaiArea = new BarisDialog[] {
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Bagus, Ksatria! Kamu berhasil menyelesaikan seluruh misi di area ini." },
                new BarisDialog { namaPembicara = "Aki Aruna", kalimat = "Gerbang telah terbuka! Silakan melangkah ke garis finish untuk melanjutkan petualanganmu." }
            };

            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.TampilkanDialog(dialogSelesaiArea, () => {
                    SembunyikanUIMisi();
                });
            }
            else
            {
                SembunyikanUIMisi();
            }
        }
    }

    private void SelesaikanEksplorasiTanpaEvaluasi()
    {
        BukaGerbangTebingKabut();
    }

    void BukaGerbangTebingKabut()
    {
        if (gerbangCollider != null)
        {
            gerbangCollider.isTrigger = true;
            Debug.Log("Selamat! Seluruh eksplorasi 3 level selesai dan gerbang terbuka!");
        }
    }

    // Dipanggil saat player menyelesaikan misi atau menyentuh Finish
    public void SembunyikanUIMisi()
    {
        if (panelMisi != null)
        {
            panelMisi.SetActive(false);
            Debug.Log("[MISI] panelMisi dinonaktifkan (SetActive false)");
        }

        GameObject bg = GameObject.Find("Panel_Misi_BG");
        if (bg != null && bg != panelMisi)
        {
            bg.SetActive(false);
        }

        if (misiManager != null)
        {
            misiManager.SembunyikanPanelMisi();
        }
    }

    public void PindahKeStage2()
    {
        CheckpointManager.CatatProgressSceneSelesai(SceneManager.GetActiveScene().name);
        UpdatePencapaian.SelesaikanStage(1);
        WinManager.PindahKeMapCheckpoint();
    }
}