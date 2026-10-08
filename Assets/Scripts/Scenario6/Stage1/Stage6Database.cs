using System.Collections.Generic;

public static class Stage6Database
{
    // =====================================================
    // DIALOG PER SUB-LEVEL
    // =====================================================

    public static string[] DapatkanDialog(int subLevel)
    {
        if (subLevel == 1)
        {
            return new string[]
            {
                "Ksatria, kita akhirnya tiba di gerbang menuju Ruang Takhta!",
                "Lihat dua gembok besar di depanmu. Masing-masing memiliki angka 3 dan 4.",
                "Kita membutuhkan satu angka yang dapat digunakan untuk membuka kedua gembok sekaligus.",
                "Angka tersebut harus merupakan kelipatan dari 3 dan juga kelipatan dari 4.",
                "Ingat! KPK adalah Kelipatan Persekutuan Terkecil.",
                "Kelipatan 3 adalah: 3, 6, 9, 12, 15...",
                "Kelipatan 4 adalah: 4, 8, 12, 16...",
                "Angka yang sama dan paling kecil adalah 12.",
                "Sekarang pilih kunci dengan angka KPK yang tepat!"
            };
        }

        if (subLevel == 2)
        {
            return new string[]
            {
                "Bagus, Ksatria! Kamu berhasil memahami cara kerja KPK.",
                "Namun, segel berikutnya membutuhkan perhitungan yang lebih sulit.",
                "Kali ini kamu harus menemukan KPK dengan lebih mandiri.",
                "Ingat, cari kelipatan yang sama dari kedua angka.",
                "Kemudian pilih kelipatan persekutuan yang paling kecil.",
                "Hati-hati, beberapa pilihan mungkin merupakan kelipatan persekutuan tetapi bukan KPK!",
                "Ayo pilih kunci yang tepat!"
            };
        }

        return new string[]
        {
            "Ini adalah segel terakhir pada gerbang luar.",
            "Sekarang kamu harus menentukan KPK dengan lebih mandiri.",
            "Tidak ada lagi daftar kelipatan yang diberikan.",
            "Hitung dengan teliti dan pilih angka yang merupakan KPK.",
            "Ingat, kita mencari kelipatan persekutuan yang PALING KECIL.",
            "Jika berhasil, gerbang menuju Ruang Takhta akan terbuka!"
        };
    }


    // =====================================================
    // SOAL PER SUB-LEVEL
    // =====================================================

    public static List<DataSoalKPK> DapatkanSoal(int subLevel)
    {
        List<DataSoalKPK> soal = new List<DataSoalKPK>();

        // -------------------------------------------------
        // SUB LEVEL 1
        // -------------------------------------------------

        if (subLevel == 1)
        {
            soal.Add(
                BuatSoal(
                    3, 4,
                    "12", "7", "24",
                    0,
                    "Benar! KPK dari 3 dan 4 adalah 12.",
                    "Belum tepat. Cari kelipatan 3 dan 4 yang sama dan paling kecil."
                )
            );

            soal.Add(
                BuatSoal(
                    2, 3,
                    "5", "6", "12",
                    1,
                    "Tepat! KPK dari 2 dan 3 adalah 6.",
                    "Coba cari kelipatan yang sama dari 2 dan 3."
                )
            );

            soal.Add(
                BuatSoal(
                    2, 4,
                    "4", "8", "12",
                    0,
                    "Benar! KPK dari 2 dan 4 adalah 4.",
                    "Ingat, kita mencari kelipatan persekutuan yang paling kecil."
                )
            );
        }

        // -------------------------------------------------
        // SUB LEVEL 2
        // -------------------------------------------------

        else if (subLevel == 2)
        {
            soal.Add(
                BuatSoal(
                    4, 6,
                    "10", "12", "24",
                    1,
                    "Hebat! KPK dari 4 dan 6 adalah 12.",
                    "Coba cari kelipatan 4 dan 6."
                )
            );

            soal.Add(
                BuatSoal(
                    3, 5,
                    "8", "15", "30",
                    1,
                    "Benar! KPK dari 3 dan 5 adalah 15.",
                    "Cari kelipatan yang dimiliki oleh 3 dan 5."
                )
            );

            soal.Add(
                BuatSoal(
                    4, 8,
                    "8", "16", "24",
                    0,
                    "Tepat! KPK dari 4 dan 8 adalah 8.",
                    "Jangan memilih angka yang lebih besar jika ada yang lebih kecil."
                )
            );

            soal.Add(
                BuatSoal(
                    5, 6,
                    "11", "30", "60",
                    1,
                    "Bagus! KPK dari 5 dan 6 adalah 30.",
                    "Cari kelipatan 5 dan 6 yang sama."
                )
            );
        }

        // -------------------------------------------------
        // SUB LEVEL 3
        // -------------------------------------------------

        else
        {
            soal.Add(
                BuatSoal(
                    6, 8,
                    "24", "48", "18",
                    0,
                    "Sempurna! KPK dari 6 dan 8 adalah 24.",
                    "48 memang kelipatan keduanya, tetapi bukan yang terkecil."
                )
            );

            soal.Add(
                BuatSoal(
                    6, 9,
                    "18", "36", "54",
                    0,
                    "Benar! KPK dari 6 dan 9 adalah 18.",
                    "Cari kelipatan pertama yang sama dari 6 dan 9."
                )
            );

            soal.Add(
                BuatSoal(
                    8, 12,
                    "24", "48", "96",
                    0,
                    "Tepat! KPK dari 8 dan 12 adalah 24.",
                    "48 dan 96 adalah kelipatan persekutuan, tetapi bukan yang terkecil."
                )
            );

            soal.Add(
                BuatSoal(
                    9, 12,
                    "18", "36", "72",
                    1,
                    "Hebat! KPK dari 9 dan 12 adalah 36.",
                    "Jangan memilih 72. Cari kelipatan persekutuan yang lebih kecil."
                )
            );
        }

        return soal;
    }


    // =====================================================
    // PEMBUAT SOAL
    // =====================================================

    private static DataSoalKPK BuatSoal(
        int angka1,
        int angka2,
        string pilihan1,
        string pilihan2,
        string pilihan3,
        int jawabanBenar,
        string feedbackBenar,
        string feedbackSalah)
    {
        return new DataSoalKPK
        {
            instruksiTeks = "Pilih kunci dengan KPK yang tepat!",

            angkaPertama = angka1,
            angkaKedua = angka2,

            pilihan1 = pilihan1,
            pilihan2 = pilihan2,
            pilihan3 = pilihan3,

            indeksJawabanBenar = jawabanBenar,

            feedbackBenar = feedbackBenar,
            feedbackSalah = feedbackSalah
        };
    }
}


// =========================================================
// DATA MODEL
// =========================================================

[System.Serializable]
public class DataSoalKPK
{
    public string instruksiTeks;

    public int angkaPertama;
    public int angkaKedua;

    public string pilihan1;
    public string pilihan2;
    public string pilihan3;

    public int indeksJawabanBenar;

    public string feedbackBenar;
    public string feedbackSalah;
}