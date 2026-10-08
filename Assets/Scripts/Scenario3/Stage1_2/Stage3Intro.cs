using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Stage3Intro : MonoBehaviour
{
    public enum SubLevel { Level_1_1, Level_1_2, Level_1_3, Level_2_1, Level_2_2, Level_2_3, Level_3_Boss }
    
    [Header("Pengaturan Sub-Level")]
    public SubLevel subLevelSaatIni = SubLevel.Level_1_1;

    [Header("Referensi UI Dialog")]
    public GameObject panelDialog;
    public TextMeshProUGUI teksDialog;
    
    [Tooltip("Tarik tombol Lanjut Dialog ke sini")]
    public Button tombolLanjutDialog;          // ← WAJIB diisi di Inspector

    private string[] kalimat;
    private int indexDialog = 0;

    private void Start()
    {
        // Pastikan panel dialog mati di awal
        if (panelDialog != null) panelDialog.SetActive(false);

        SiapkanDialog();

        IntroVideoManager introMgr = FindObjectOfType<IntroVideoManager>();
        if (introMgr != null && introMgr.gameObject.activeInHierarchy)
        {
            return;
        }
        LanjutkanSetelahVideo();
    }

    public void LanjutkanSetelahVideo()
    {
        Invoke(nameof(MulaiIntro), 0.5f);
    }

    private void SiapkanDialog()
    {
        if (subLevelSaatIni == SubLevel.Level_1_1)
        {
            kalimat = new string[] {
                "Penjaga Pohon: Ksatria! Syukurlah kau tiba. Raja Anomali telah mengutuk Rumah Pohon ini menjadi bongkahan es raksasa!",
                "Penjaga Pohon: Satu-satunya cara menyelamatkan tempat ini adalah dengan menstabilkan suhunya. Pakailah 'Gelang Suhu' ini.",
                "Penjaga Pohon: Di setiap pintu, ada Mesin Suhu Sihir. Kau harus menggunakan ilmu Perkalian dan Pembagian untuk mencairkan esnya.",
                "Penjaga Pohon: Ingat aturan dasarnya! Jika suhu sudah HANGAT (+), lalu digandakan dengan tuas POSITIF (+), maka suhunya makin PANAS (+).",
                "Penjaga Pohon: Contoh: Suhu 2°C diputar tuas [× 3], maka 2 × 3 = 6°C! Ayo, berlatihlah pada pintu es di depan sana!"
            };
        }
        else if (subLevelSaatIni == SubLevel.Level_1_2)
        {
            kalimat = new string[] {
                "Penjaga Pohon: Kerja bagus di lantai pertama! Tapi berhati-hatilah, lorong ini jauh lebih berbahaya.",
                "Penjaga Pohon: Suhu di sini sudah berada di bawah nol, alias bersuhu NEGATIF (-). Tembok esnya sangat tebal!",
                "Penjaga Pohon: Jika kau melipatgandakan (+) suhu yang beku (-), maka ruangan ini akan SEMAKIN BEKU (-)! Hati-hati!",
                "Penjaga Pohon: Ingat aturannya: Negatif dikali Positif hasilnya Negatif (- × + = -). Pilihlah tuasmu dengan bijak!"
            };
        }
        else if (subLevelSaatIni == SubLevel.Level_1_3)
        {
            kalimat = new string[] {
                "Penjaga Pohon: Ksatria, kita berada di area dengan anomali terkuat. Suhu di sini membeku ekstrem!",
                "Penjaga Pohon: Namun, ada rahasia sihir kuno: 'Sihir Pembalik Realita'. Tuas dengan lambang NEGATIF (-) bisa membalikkan keadaan!",
                "Penjaga Pohon: Jika suhu sudah BEKU (-), lalu kau kalikan dengan tuas KUTUKAN (-), es itu akan hancur dan berbalik menjadi PANAS (+)!",
                "Penjaga Pohon: Rapalkan mantra ini di kepalamu: Negatif dikali Negatif hasilnya Positif (- × - = +). Gunakan sihir ini untuk membuka jalan!"
            };
        }
        else if (subLevelSaatIni == SubLevel.Level_2_1)
        {
            kalimat = new string[] {
                "Penjaga Pohon: Luar biasa! Kau berhasil mencapai Lantai Dua Rumah Pohon. Tapi tantangannya baru saja dimulai.",
                "Penjaga Pohon: Di lantai ini, Raja Anomali memasang 'Sistem Combo Berantai'. Kau tidak bisa hanya memutar satu tuas.",
                "Penjaga Pohon: Kau harus memecahkan 3 kode suhu secara berturut-turut! Jika salah satu saja, kau harus mengulang dari awal.",
                "Penjaga Pohon: Konsentrasilah pada hasil suhu sebelumnya untuk memecahkan tuas berikutnya. Jangan terburu-buru!"
            };
        }
        else if (subLevelSaatIni == SubLevel.Level_2_2)
        {
            kalimat = new string[] {
                "Penjaga Pohon: Gawat, Ksatria! Ruangan ini mengalami fluktuasi parah. Angka suhunya melambung sangat tinggi dan tidak stabil!",
                "Penjaga Pohon: Satu-satunya cara menjinakkannya adalah dengan memecah energinya. Gunakan tuas PEMBAGIAN (÷).",
                "Penjaga Pohon: Aturan pembagian sama persis dengan perkalian. Positif dibagi Negatif, hasilnya akan berbalik jadi Negatif (+ ÷ - = -).",
                "Penjaga Pohon: Pecah energinya 3 kali berturut-turut untuk menstabilkan ruangan ini!"
            };
        }
        else if (subLevelSaatIni == SubLevel.Level_2_3)
        {
            kalimat = new string[] {
                "Penjaga Pohon: Kita hampir sampai di Puncak! Ruangan ini adalah garis pertahanan terakhir sebelum Ruang Inti.",
                "Penjaga Pohon: Sistem pertahanannya sangat acak! Kadang kau harus mengalikan, kadang harus membaginya.",
                "Penjaga Pohon: Ingat kembali semua latihanmu. Jangan biarkan angka-angka ekstrem itu mengecohmu!"
            };
        }
        else if (subLevelSaatIni == SubLevel.Level_3_Boss)
        {
            kalimat = new string[] {
                "Penjaga Pohon: Ini dia... Ruang Inti Rumah Pohon! Panel ini menyegel portal menuju markas musuh.",
                "Penjaga Pohon: Mesin ini tidak memiliki pilihan ganda. Kamu harus memecahkan Operasi Campuran dan mengetik jawabannya sendiri!",
                "Penjaga Pohon: Sebelum mulai, dengarkan baik-baik Aturan Prioritas Sihir (Ka-Ba-Ta-Ku):",
                "Penjaga Pohon: PERTAMA: Kerjakan angka yang berada di dalam TANDA KURUNG ( ) terlebih dahulu.",
                "Penjaga Pohon: KEDUA: Kerjakan sihir PERKALIAN (×) atau PEMBAGIAN (÷).",
                "Penjaga Pohon: KETIGA: Terakhir, barulah kerjakan PENJUMLAHAN (+) atau PENGURANGAN (-).",
                "Penjaga Pohon: Mari kita ambil contoh rumus ini: (-12 × 3) ÷ (-4) - 15 = ...",
                "Penjaga Pohon: Langkah 1 (Dalam Kurung): Kerjakan (-12 × 3) hasilnya adalah -36.",
                "Penjaga Pohon: Langkah 2 (Pembagian): Kerjakan -36 ÷ (-4). Ingat, Negatif bagi Negatif jadi Positif, hasilnya 9.",
                "Penjaga Pohon: Langkah 3 (Pengurangan): Terakhir, 9 - 15 hasilnya adalah -6! Maka sandinya adalah -6.",
                "Penjaga Pohon: Paham kan? Jika hasilnya negatif, jangan lupa ketik tanda strip (-) ya. Sekarang, buktikan kemampuanmu!"
            };
        }
        else
        {
            kalimat = new string[] { "Penjaga Pohon: Lanjutkan perjalananmu, Ksatria!" };
        }
    }

    private void MulaiIntro()
    {
        if (Stage3Manager.Instance != null && Stage3Manager.Instance.panelIndikatorSuhu != null)
            Stage3Manager.Instance.panelIndikatorSuhu.SetActive(false);

        if (panelDialog != null)
        {
            panelDialog.SetActive(true);
        }

        // Pastikan tombol selalu aktif & terhubung
        SetupTombolLanjut();

        indexDialog = 0;
        TampilkanKalimat();
    }

    private void SetupTombolLanjut()
    {
        if (tombolLanjutDialog == null) return;

        // Paksa tombol aktif
        tombolLanjutDialog.gameObject.SetActive(true);
        tombolLanjutDialog.interactable = true;

        // Bersihkan listener lama lalu pasang ulang (supaya tidak hilang)
        tombolLanjutDialog.onClick.RemoveAllListeners();
        tombolLanjutDialog.onClick.AddListener(LanjutDialog);
    }

    public void LanjutDialog()
    {
        indexDialog++;
        
        if (indexDialog < kalimat.Length)
        {
            TampilkanKalimat();
        }
        else
        {
            SelesaiDialog();
        }
    }

    private void TampilkanKalimat()
    {
        if (teksDialog != null && kalimat != null && indexDialog < kalimat.Length)
        {
            teksDialog.text = kalimat[indexDialog];
        }

        // Pastikan tombol tetap muncul setiap kalimat baru
        if (tombolLanjutDialog != null)
        {
            tombolLanjutDialog.gameObject.SetActive(true);
            tombolLanjutDialog.interactable = true;
        }
    }

    private void SelesaiDialog()
    {
        if (panelDialog != null) 
            panelDialog.SetActive(false);

        if (Stage3Manager.Instance != null) 
            Stage3Manager.Instance.AktifkanPanelSuhu();
    }
}