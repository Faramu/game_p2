using System.Collections.Generic;

public static class Stage6Level2Database
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
                "Ksatria, kita sudah masuk ke Ruang Takhta!",
                "Raja Anomali menahan Putri Aruna di balik kurungan besi.",
                "Gunakan Meriam KPK untuk menembak dan merusak besi kurungan!",
                "Setiap jawaban benar akan melemahkan raja dan merusak satu lapisan besi.",
                "Ingat, pilih angka yang merupakan Kelipatan Persekutuan Terkecil (KPK).",
                "Ayo mulai tembakan pertama!"
            };
        }

        if (subLevel == 2)
        {
            return new string[]
            {
                "Bagus! Besi kurungan mulai retak.",
                "Tapi Raja Anomali semakin marah. Soal kali ini lebih menantang.",
                "Tetap fokus mencari KPK dari kedua angka.",
                "Jangan terkecoh dengan kelipatan yang lebih besar.",
                "Hancurkan sisa besi kurungan!"
            };
        }

        // Sub-level 3
        return new string[]
        {
            "Ini adalah tembakan terakhir!",
            "Putri Aruna hampir bebas. Jangan sampai gagal.",
            "Hitung KPK dengan teliti.",
            "Satu kesalahan bisa membuat raja pulih kembali.",
            "Lepaskan Putri Aruna sekarang juga!"
        };
    }

    // =====================================================
    // SOAL PER SUB-LEVEL (masing-masing 10 soal)
    // =====================================================
    public static List<DataSoalKPK> DapatkanSoal(int subLevel)
    {
        List<DataSoalKPK> soal = new List<DataSoalKPK>();

        // -------------------------------------------------
        // SUB LEVEL 1 (Lebih mudah)
        // -------------------------------------------------
        if (subLevel == 1)
        {
            soal.Add(BuatSoal(3, 4, "12", "7", "24", 0, "Benar! KPK 3 dan 4 adalah 12.", "Cari kelipatan terkecil yang sama."));
            soal.Add(BuatSoal(2, 5, "10", "7", "20", 0, "Tepat! KPK 2 dan 5 adalah 10.", "Kelipatan 2 dan 5 yang sama."));
            soal.Add(BuatSoal(4, 6, "12", "10", "24", 0, "Hebat! KPK 4 dan 6 adalah 12.", "Jangan pilih 24, itu bukan yang terkecil."));
            soal.Add(BuatSoal(3, 5, "15", "8", "30", 0, "Benar! KPK 3 dan 5 adalah 15.", "Cari kelipatan persekutuan terkecil."));
            soal.Add(BuatSoal(2, 4, "4", "6", "8", 0, "Tepat! KPK 2 dan 4 adalah 4.", "4 adalah kelipatan terkecil yang sama."));
            soal.Add(BuatSoal(3, 6, "6", "9", "12", 0, "Benar! KPK 3 dan 6 adalah 6.", "6 sudah habis dibagi 3 dan 6."));
            soal.Add(BuatSoal(5, 10, "10", "15", "20", 0, "Bagus! KPK 5 dan 10 adalah 10.", "Pilih yang paling kecil."));
            soal.Add(BuatSoal(4, 8, "8", "12", "16", 0, "Tepat! KPK 4 dan 8 adalah 8.", "8 adalah KPK-nya."));
            soal.Add(BuatSoal(2, 3, "6", "5", "12", 0, "Benar! KPK 2 dan 3 adalah 6.", "Kelipatan 2 dan 3 yang sama."));
            soal.Add(BuatSoal(5, 6, "30", "11", "60", 0, "Hebat! KPK 5 dan 6 adalah 30.", "30 adalah yang terkecil."));
        }

        // -------------------------------------------------
        // SUB LEVEL 2 (Sedang)
        // -------------------------------------------------
        else if (subLevel == 2)
        {
            soal.Add(BuatSoal(6, 8, "24", "16", "48", 0, "Benar! KPK 6 dan 8 adalah 24.", "48 terlalu besar."));
            soal.Add(BuatSoal(6, 9, "18", "12", "36", 0, "Tepat! KPK 6 dan 9 adalah 18.", "Cari yang paling kecil."));
            soal.Add(BuatSoal(8, 12, "24", "16", "36", 0, "Hebat! KPK 8 dan 12 adalah 24.", "24 adalah jawaban yang benar."));
            soal.Add(BuatSoal(9, 12, "36", "18", "72", 0, "Benar! KPK 9 dan 12 adalah 36.", "18 bukan kelipatan 12."));
            soal.Add(BuatSoal(7, 5, "35", "12", "70", 0, "Bagus! KPK 7 dan 5 adalah 35.", "Kelipatan 5 dan 7."));
            soal.Add(BuatSoal(8, 10, "40", "20", "80", 0, "Tepat! KPK 8 dan 10 adalah 40.", "20 bukan kelipatan 8."));
            soal.Add(BuatSoal(9, 6, "18", "15", "36", 0, "Benar! KPK 9 dan 6 adalah 18.", "18 adalah yang terkecil."));
            soal.Add(BuatSoal(10, 15, "30", "20", "60", 0, "Hebat! KPK 10 dan 15 adalah 30.", "Pilih yang paling kecil."));
            soal.Add(BuatSoal(7, 14, "14", "21", "28", 0, "Benar! KPK 7 dan 14 adalah 14.", "14 sudah habis dibagi keduanya."));
            soal.Add(BuatSoal(8, 6, "24", "16", "48", 0, "Tepat! KPK 8 dan 6 adalah 24.", "Sama seperti 6 dan 8."));
        }

        // -------------------------------------------------
        // SUB LEVEL 3 (Lebih sulit)
        // -------------------------------------------------
        else
        {
            soal.Add(BuatSoal(12, 18, "36", "24", "72", 0, "Sempurna! KPK 12 dan 18 adalah 36.", "24 bukan kelipatan 18."));
            soal.Add(BuatSoal(15, 20, "60", "30", "120", 0, "Benar! KPK 15 dan 20 adalah 60.", "30 bukan kelipatan 20."));
            soal.Add(BuatSoal(14, 21, "42", "28", "84", 0, "Hebat! KPK 14 dan 21 adalah 42.", "Cari kelipatan persekutuan terkecil."));
            soal.Add(BuatSoal(16, 24, "48", "32", "96", 0, "Tepat! KPK 16 dan 24 adalah 48.", "32 bukan kelipatan 24."));
            soal.Add(BuatSoal(18, 24, "72", "36", "144", 0, "Bagus! KPK 18 dan 24 adalah 72.", "36 bukan kelipatan 24."));
            soal.Add(BuatSoal(15, 25, "75", "50", "150", 0, "Benar! KPK 15 dan 25 adalah 75.", "50 bukan kelipatan 15."));
            soal.Add(BuatSoal(20, 30, "60", "40", "120", 0, "Tepat! KPK 20 dan 30 adalah 60.", "60 adalah yang terkecil."));
            soal.Add(BuatSoal(21, 28, "84", "42", "168", 0, "Hebat! KPK 21 dan 28 adalah 84.", "42 bukan kelipatan 28."));
            soal.Add(BuatSoal(16, 20, "80", "40", "160", 0, "Benar! KPK 16 dan 20 adalah 80.", "40 bukan kelipatan 16."));
            soal.Add(BuatSoal(24, 36, "72", "48", "144", 0, "Sempurna! KPK 24 dan 36 adalah 72.", "48 bukan kelipatan 36."));
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
            instruksiTeks = "Pilih peluru dengan KPK yang tepat!",
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