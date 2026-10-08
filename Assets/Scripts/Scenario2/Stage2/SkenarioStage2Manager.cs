using UnityEngine;

public class SkenarioStage2Manager : MonoBehaviour
{
    public DialogManager dialogManager;
    public MisiManager misiManager; 

    public static SkenarioStage2Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start() 
    {
        Debug.Log("Skenario Stage 2: Siap Menerima Interaksi Penjual Buah.");
    }

    // Dipanggil saat pemain menekan tombol interaksi di dekat NPC Penjual Buah
    public void MulaiSkenarioPenjual()
    {
        // Pastikan MiniGameManager ada untuk mengecek Level
        if (MiniGamePenjualManager.Instance == null)
        {
            Debug.LogError("MiniGamePenjualManager tidak ditemukan di Scene!");
            return;
        }

        int levelGame = MiniGamePenjualManager.Instance.levelSaatIni;
        BarisDialog[] percakapanPenjual;

        // --- DIALOG DINAMIS BERDASARKAN LEVEL ---
        if (levelGame == 1)
        {
            percakapanPenjual = new BarisDialog[] {
                new BarisDialog { namaPembicara = "Penjual Buah", kalimat = "Nak Ksatria! Syukurlah kamu datang. Catatan buah masuk dan keluarku kacau gara-gara kutukan." },
                new BarisDialog { namaPembicara = "Player", kalimat = "Tenang Pak, saya bantu. Kita hitung yang sederhana dulu ya." },
                new BarisDialog { namaPembicara = "Penjual Buah", kalimat = "Tolong bantu aku. Ketikkan hasil akhirnya di bukuku ya!" }
            };
        }
        else if (levelGame == 2)
        {
            percakapanPenjual = new BarisDialog[] {
                new BarisDialog { namaPembicara = "Penjual Buah", kalimat = "Aduh, pusing sekali! Sekarang hitungannya makin rumit, banyak penyusutan dan retur." },
                new BarisDialog { namaPembicara = "Player", kalimat = "Tidak masalah, Pak. Kalau hasilnya berkurang atau rugi, saya akan beri tanda minus (-)." },
                new BarisDialog { namaPembicara = "Penjual Buah", kalimat = "Terima kasih! Jangan sampai salah ketik tanda minusnya ya, nanti aku bangkrut!" }
            };
        }
        else // LEVEL 3
        {
            percakapanPenjual = new BarisDialog[] {
                new BarisDialog { namaPembicara = "Penjual Buah", kalimat = "Ini gawat! Angka kerugian dan keuntungannya membesar! Kutukannya makin kuat!" },
                new BarisDialog { namaPembicara = "Player", kalimat = "Fokus saja pada aturannya, Pak. Untung/Barang Masuk itu Positif, Rugi/Barang Keluar itu Negatif." },
                new BarisDialog { namaPembicara = "Penjual Buah", kalimat = "Baiklah, tolong selesaikan perhitungan tingkat lanjut ini untukku. Ini harapan terakhir Kiosku!" }
            };
        }

        // Tampilkan Dialog
        if (dialogManager != null)
        {
            dialogManager.TampilkanDialog(percakapanPenjual, () => {
                Debug.Log("Dialog Selesai. Membuka Kuis Ketik Angka (Level " + levelGame + ")");
                MiniGamePenjualManager.Instance.MulaiMiniGame();
            });
        }
        else
        {
            MiniGamePenjualManager.Instance.MulaiMiniGame();
        }
    }

    public void UpdateProgresPenjual(bool berhasil)
    {
        if (berhasil)
        {
            Debug.Log("Tantangan Penjual Buah Selesai! Misi diperbarui.");
            // Panggil MisiManager di sini nanti
        }
    }
}