using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoalKetikLevel3
{
    [TextArea(2, 4)]
    public string narasiSoal;
    public string kunciJawaban;
    [TextArea(2, 3)]
    public string penjelasanBenar;
    [TextArea(2, 3)]
    public string penjelasanSalah;
}

public class BankSoalKetikLevel3 : MonoBehaviour
{
    [Header("Daftar Soal (otomatis diisi saat Play)")]
    public List<SoalKetikLevel3> daftarSoal = new List<SoalKetikLevel3>();

    private int indeksSoalSaatIni = 0;

    private void Awake()
    {
        // SELALU isi ulang soal agar tidak hilang
        InisialisasiSoal();
        indeksSoalSaatIni = 0;
    }

    private void InisialisasiSoal()
    {
        daftarSoal = new List<SoalKetikLevel3>();

        // Soal 1
        daftarSoal.Add(new SoalKetikLevel3
        {
            narasiSoal = "Ketik semua angka faktor dari 32 (pisahkan dengan koma).",
            kunciJawaban = "1,2,4,8,16,32",
            penjelasanBenar = "Hebat! Angka-angka tersebut adalah faktor dari 32.",
            penjelasanSalah = "Salah! Ada angka yang bukan faktor dari 32 atau ada yang kurang."
        });

        // Soal 2
        daftarSoal.Add(new SoalKetikLevel3
        {
            narasiSoal = "Ketik semua angka faktor dari 24 (pisahkan dengan koma).",
            kunciJawaban = "1,2,3,4,6,8,12,24",
            penjelasanBenar = "Benar! Angka-angka tersebut dapat membagi habis 24.",
            penjelasanSalah = "Kurang tepat! Coba ingat kembali faktor dari 24."
        });

        // Soal 3
        daftarSoal.Add(new SoalKetikLevel3
        {
            narasiSoal = "Ketik semua angka faktor dari 18 (pisahkan dengan koma).",
            kunciJawaban = "1,2,3,6,9,18",
            penjelasanBenar = "Bagus! Faktor-faktor tersebut benar untuk 18.",
            penjelasanSalah = "Belum tepat. Periksa kembali pembagi dari 18."
        });

        // Soal 4
        daftarSoal.Add(new SoalKetikLevel3
        {
            narasiSoal = "Ketik semua angka faktor dari 20 (pisahkan dengan koma).",
            kunciJawaban = "1,2,4,5,10,20",
            penjelasanBenar = "Tepat sekali! 20 habis dibagi oleh angka-angka ini.",
            penjelasanSalah = "Salah! Perhatikan kembali faktor dari 20."
        });

        // Soal 5
        daftarSoal.Add(new SoalKetikLevel3
        {
            narasiSoal = "Ketik semua angka faktor dari 36 (pisahkan dengan koma).",
            kunciJawaban = "1,2,3,4,6,9,12,18,36",
            penjelasanBenar = "Luar biasa! Kamu menemukan semua faktor dari 36.",
            penjelasanSalah = "Gagal! Ada faktor dari 36 yang belum disebut atau salah ketik."
        });

        Debug.Log($"<color=cyan>[BANK SOAL]</color> Berhasil mengisi {daftarSoal.Count} soal.");
    }

    public SoalKetikLevel3 AmbilSoalBerikutnya()
    {
        if (daftarSoal == null || daftarSoal.Count == 0)
        {
            Debug.LogError("[BANK SOAL] daftarSoal kosong!");
            return null;
        }

        if (indeksSoalSaatIni >= daftarSoal.Count)
        {
            Debug.LogWarning("[BANK SOAL] Soal sudah habis.");
            return null;
        }

        SoalKetikLevel3 soal = daftarSoal[indeksSoalSaatIni];
        indeksSoalSaatIni++;

        Debug.Log($"<color=cyan>[BANK SOAL]</color> Mengambil soal ke-{indeksSoalSaatIni}");
        return soal;
    }

    public void ResetBank()
    {
        indeksSoalSaatIni = 0;
    }
}