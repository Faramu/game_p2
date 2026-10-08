using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MisiManager : MonoBehaviour
{
    [System.Serializable]
    public class DataMisi {
        public string namaMisiUntukCatatan; // Hanya penanda nama di Inspector
        public GameObject ikonCentang;      // Masukkan gambar centang
        public TextMeshProUGUI teksMisi;    // Masukkan objek TextMeshPro
    }

    [Header("--- PANEL MISI UTAMA ---")]
    [Tooltip("Panel Misi utama yang akan diaktifkan/dinonaktifkan")]
    public GameObject panelMisi;

    [Header("Daftar UI Misi (Slot 0, 1, 2)")]
    public DataMisi[] daftarMisi;

    private AudioSource audioSource;
    private int currentSubLevel = 1;
    private bool sudahInisialisasiMisi = false;

    private void Awake()
    {
        if (panelMisi == null)
        {
            // 1. Cari berdasarkan nama GameObject di scene
            GameObject bg = GameObject.Find("Panel_Misi_BG");
            if (bg != null)
            {
                panelMisi = bg;
            }
            // 2. Jika tidak ditemukan lewat nama, telusuri parent dari teks misi
            else if (daftarMisi != null && daftarMisi.Length > 0 && daftarMisi[0].teksMisi != null)
            {
                Transform t = daftarMisi[0].teksMisi.transform;
                while (t.parent != null && t.parent.GetComponent<Canvas>() == null)
                {
                    t = t.parent;
                }
                panelMisi = t.gameObject;
            }
            else if (transform.parent != null && (transform.parent.name.Contains("Misi") || transform.parent.name.Contains("Panel") || transform.parent.name.Contains("Container")))
            {
                panelMisi = transform.parent.gameObject;
            }
            else
            {
                panelMisi = gameObject;
            }
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InisialisasiMisiDariSceneAktif();
    }

    public void InisialisasiMisiDariSceneAktif()
    {
        if (sudahInisialisasiMisi) return;
        sudahInisialisasiMisi = true;

        string sceneName = SceneManager.GetActiveScene().name;
        int stage = 1;
        int level = 1;
        int subLevel = 1;

        if (CheckpointManager.TryGetInfoScene(sceneName, out int stg, out int lvl, out int subLvl, out _))
        {
            stage = stg;
            level = lvl;
            subLevel = subLvl;
        }
        else
        {
            if (sceneName == "Stage1_Eksplorasi") { stage = 1; level = 1; subLevel = 1; }
            else if (sceneName == "Stage1_Eksplorasi_1") { stage = 1; level = 1; subLevel = 2; }
            else if (sceneName == "Stage1_Eksplorasi_2") { stage = 1; level = 1; subLevel = 3; }
        }

        currentSubLevel = subLevel;

        Debug.Log($"[MISSION] Scene aktif: {sceneName}");
        Debug.Log($"[MISSION] Stage: {stage}");
        Debug.Log($"[MISSION] Level: {level}");
        Debug.Log($"[MISSION] Sublevel: {subLevel}");
        Debug.Log($"[MISSION] Memuat misi untuk Sublevel {subLevel}");

        SetTeksMisiLevelInternal(subLevel);
    }

    // Sembunyikan panel misi (SetActive false) setelah misi selesai atau Win UI muncul
    public void SembunyikanPanelMisi()
    {
        if (panelMisi != null)
        {
            panelMisi.SetActive(false);
            Debug.Log("[MISI] Panel Misi dinonaktifkan (SetActive false)");
        }

        GameObject bg = GameObject.Find("Panel_Misi_BG");
        if (bg != null && bg != panelMisi)
        {
            bg.SetActive(false);
        }
    }

    // Tampilkan panel misi untuk misi berikutnya jika diperlukan
    public void TampilkanPanelMisi()
    {
        if (panelMisi != null)
        {
            panelMisi.SetActive(true);
            Debug.Log("[MISI] Panel Misi diaktifkan (SetActive true)");
        }
        else
        {
            GameObject bg = GameObject.Find("Panel_Misi_BG");
            if (bg != null)
            {
                bg.SetActive(true);
            }
        }
    }

    // Cek apakah seluruh misi di daftar ini sudah selesai
    public bool ApakahSemuaMisiSelesai()
    {
        // 1. Cek dari daftarMisi jika sudah diatur
        if (daftarMisi != null && daftarMisi.Length > 0)
        {
            foreach (var misi in daftarMisi)
            {
                if (misi.ikonCentang != null && !misi.ikonCentang.activeSelf)
                    return false;
            }
            return true;
        }

        // 2. Fallback: cari ikon centang pada objek panelMisi / child
        GameObject targetPanel = panelMisi;
        if (targetPanel == null) targetPanel = GameObject.Find("Panel_Misi_BG");
        if (targetPanel == null) targetPanel = GameObject.Find("Container_Misi");

        if (targetPanel != null)
        {
            Transform[] allChildren = targetPanel.GetComponentsInChildren<Transform>(true);
            int countCentang = 0;
            foreach (Transform t in allChildren)
            {
                string lower = t.name.ToLower();
                if (lower.Contains("centang") || lower.Contains("check"))
                {
                    countCentang++;
                    if (!t.gameObject.activeSelf)
                        return false;
                }
            }
            if (countCentang > 0)
                return true;
        }

        return true;
    }

    // Reset visual centang & warna teks ke kondisi awal
    public void ResetTampilanMisi()
    {
        foreach (var misi in daftarMisi) {
            if (misi.ikonCentang != null) misi.ikonCentang.SetActive(false);
            if (misi.teksMisi != null) {
                misi.teksMisi.fontStyle = FontStyles.Normal;
                misi.teksMisi.color = Color.white;
            }
        }
    }

    // Fungsi untuk menceklis misi berdasarkan indeks (0 = Misi 1, 1 = Misi 2, 2 = Misi 3)
    public void MisiSelesai(int indeksMisi)
    {
        if (indeksMisi >= 0 && indeksMisi < daftarMisi.Length) 
        {
            // Jika belum terceklis, berikan efek coret dan centang
            if (daftarMisi[indeksMisi].ikonCentang != null && !daftarMisi[indeksMisi].ikonCentang.activeSelf) 
            {
                daftarMisi[indeksMisi].ikonCentang.SetActive(true);

                if (daftarMisi[indeksMisi].teksMisi != null) {
                    daftarMisi[indeksMisi].teksMisi.fontStyle = FontStyles.Strikethrough;
                    daftarMisi[indeksMisi].teksMisi.color = new Color(0.7f, 0.7f, 0.7f, 1f); 
                }

                if (audioSource != null) audioSource.Play();
                Debug.Log("Misi ke-" + (indeksMisi + 1) + " Selesai!");
            }
        }
    }

    // Memperbarui isi teks misi secara otomatis untuk Level 1, 2, dan 3
    public void SetTeksMisiLevel(int level)
    {
        // Pastikan hanya memuat misi untuk sublevel yang sesuai dengan scene yang sedang aktif!
        string sceneName = SceneManager.GetActiveScene().name;
        int activeSubLevel = currentSubLevel;
        if (CheckpointManager.TryGetInfoScene(sceneName, out _, out _, out int subLvl, out _))
        {
            activeSubLevel = subLvl;
        }
        else if (sceneName == "Stage1_Eksplorasi") activeSubLevel = 1;
        else if (sceneName == "Stage1_Eksplorasi_1") activeSubLevel = 2;
        else if (sceneName == "Stage1_Eksplorasi_2") activeSubLevel = 3;

        if (level != activeSubLevel)
        {
            Debug.LogWarning($"[MISSION] Pergantian ke Sublevel {level} diabaikan karena scene masih '{sceneName}' (Sublevel {activeSubLevel})!");
            level = activeSubLevel;
        }

        SetTeksMisiLevelInternal(level);
    }

    private void SetTeksMisiLevelInternal(int level)
    {
        ResetTampilanMisi(); // Bersihkan centang lama saat inisialisasi level

        if (level == 1) // --- AREA POSITIF ---
        {
            if (daftarMisi.Length > 0 && daftarMisi[0].teksMisi != null) 
                daftarMisi[0].teksMisi.text = "Berjalan ke kanan memulai penelusuran";
            if (daftarMisi.Length > 1 && daftarMisi[1].teksMisi != null) 
                daftarMisi[1].teksMisi.text = "Ambil seluruh kunang-kunang positif";
            if (daftarMisi.Length > 2 && daftarMisi[2].teksMisi != null) 
                daftarMisi[2].teksMisi.text = "Ambil seluruh kunang-kunang untung";
        }
        else if (level == 2) // --- AREA NEGATIF ---
        {
            if (daftarMisi.Length > 0 && daftarMisi[0].teksMisi != null) 
                daftarMisi[0].teksMisi.text = "Melanjutkan penelusuran ke area negatif";
            if (daftarMisi.Length > 1 && daftarMisi[1].teksMisi != null) 
                daftarMisi[1].teksMisi.text = "Ambil seluruh kunang-kunang negatif";
            if (daftarMisi.Length > 2 && daftarMisi[2].teksMisi != null) 
                daftarMisi[2].teksMisi.text = "Ambil seluruh kunang-kunang rugi";
        }
        else if (level == 3) // --- EVALUASI MAKNA ---
        {
            if (daftarMisi.Length > 0 && daftarMisi[0].teksMisi != null) 
                daftarMisi[0].teksMisi.text = "Melanjutkan penelusuran ke gerbang akhir";
            if (daftarMisi.Length > 1 && daftarMisi[1].teksMisi != null) 
                daftarMisi[1].teksMisi.text = "Fahami kembali seluruh makna kunang-kunang";
            if (daftarMisi.Length > 2 && daftarMisi[2].teksMisi != null) 
                daftarMisi[2].teksMisi.text = "Selesaikan eksplorasi Hutan Aruna";
        }
    }
}