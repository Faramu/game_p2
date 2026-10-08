using System.Collections.Generic;

public static class Stage6Level3Database
{
    // =====================================================
    // DIALOG LEVEL 3
    // =====================================================
    public static string[] DapatkanDialog()
    {
        return new string[]
        {
            "Ksatria dan Putri Aruna, ini adalah pertarungan terakhir melawan Raja Anomali!",
            "Raja sudah sangat lemah, namun masih menyisakan sihir kuat di perisainya.",
            "Kali ini kamu harus mengetik sendiri jawaban KPK-nya.",
            "Hitung dengan teliti. Setiap jawaban benar akan melemahkan raja semakin parah.",
            "Jika berhasil, Raja Anomali akan kalah total dan jalan pulang akan terbuka!",
            "Siap? Mari kita akhiri ini!"
        };
    }

    // =====================================================
    // SOAL LEVEL 3 (10 soal, feedback lebih detail)
    // =====================================================
    public static List<DataSoalKPK> DapatkanSoal()
    {
        List<DataSoalKPK> soal = new List<DataSoalKPK>();

        soal.Add(BuatSoal(
            12, 18,
            "36", "24", "72",
            0,
            "Benar! KPK dari 12 dan 18 adalah 36.\nPenjelasan:\n• Kelipatan 12: 12, 24, 36, 48, 60, 72...\n• Kelipatan 18: 18, 36, 54, 72...\n• Angka sama yang paling kecil adalah 36.",
            "Belum tepat.\nCara mencari KPK 12 dan 18:\n1. Tulis kelipatan 12: 12, 24, 36, 48...\n2. Tulis kelipatan 18: 18, 36, 54...\n3. Cari angka yang sama dan paling kecil → 36.\nCoba lagi!"
        ));

        soal.Add(BuatSoal(
            15, 20,
            "60", "30", "120",
            0,
            "Tepat! KPK dari 15 dan 20 adalah 60.\nPenjelasan:\n• 15 = 3 × 5\n• 20 = 2² × 5\n• KPK diambil dari faktor dengan pangkat tertinggi → 2² × 3 × 5 = 60.",
            "Jawaban masih kurang tepat.\nIngat:\n• 30 bukan kelipatan 20.\n• 120 adalah kelipatan persekutuan, tapi bukan yang terkecil.\n• KPK yang benar adalah 60.\nHitung ulang ya!"
        ));

        soal.Add(BuatSoal(
            14, 21,
            "42", "28", "84",
            0,
            "Hebat! KPK dari 14 dan 21 adalah 42.\nPenjelasan:\n• 14 = 2 × 7\n• 21 = 3 × 7\n• KPK = 2 × 3 × 7 = 42.\n42 adalah bilangan terkecil yang habis dibagi 14 dan 21.",
            "Belum benar.\nCoba ingat:\n• 28 bukan kelipatan 21.\n• 84 terlalu besar (kelipatan persekutuan tapi bukan terkecil).\n• Jawaban yang tepat adalah 42."
        ));

        soal.Add(BuatSoal(
            16, 24,
            "48", "32", "96",
            0,
            "Benar sekali! KPK dari 16 dan 24 adalah 48.\nPenjelasan:\n• 16 = 2⁴\n• 24 = 2³ × 3\n• Ambil pangkat tertinggi: 2⁴ × 3 = 16 × 3 = 48.",
            "Masih kurang tepat.\nPerhatikan:\n• 32 bukan kelipatan 24.\n• 96 adalah kelipatan keduanya, tapi bukan yang terkecil.\n• KPK yang benar = 48."
        ));

        soal.Add(BuatSoal(
            18, 24,
            "72", "36", "144",
            0,
            "Bagus! KPK dari 18 dan 24 adalah 72.\nPenjelasan:\n• 18 = 2 × 3²\n• 24 = 2³ × 3\n• KPK = 2³ × 3² = 8 × 9 = 72.",
            "Belum tepat.\nCatatan penting:\n• 36 bukan kelipatan 24 (24 × 1,5 = 36, bukan bilangan bulat kali).\n• 144 terlalu besar.\n• Yang paling kecil dan benar adalah 72."
        ));

        soal.Add(BuatSoal(
            15, 25,
            "75", "50", "150",
            0,
            "Sempurna! KPK dari 15 dan 25 adalah 75.\nPenjelasan:\n• 15 = 3 × 5\n• 25 = 5²\n• KPK = 3 × 5² = 3 × 25 = 75.",
            "Jawaban belum benar.\nPenjelasan:\n• 50 bukan kelipatan 15.\n• 150 adalah kelipatan persekutuan, tetapi bukan yang terkecil.\n• KPK yang benar adalah 75."
        ));

        soal.Add(BuatSoal(
            20, 30,
            "60", "40", "120",
            0,
            "Tepat! KPK dari 20 dan 30 adalah 60.\nPenjelasan:\n• 20 = 2² × 5\n• 30 = 2 × 3 × 5\n• KPK = 2² × 3 × 5 = 4 × 3 × 5 = 60.",
            "Masih salah.\nIngat:\n• 40 bukan kelipatan 30.\n• 120 terlalu besar.\n• Bilangan terkecil yang habis dibagi 20 dan 30 adalah 60."
        ));

        soal.Add(BuatSoal(
            21, 28,
            "84", "42", "168",
            0,
            "Benar! KPK dari 21 dan 28 adalah 84.\nPenjelasan:\n• 21 = 3 × 7\n• 28 = 2² × 7\n• KPK = 2² × 3 × 7 = 4 × 3 × 7 = 84.",
            "Belum tepat.\nPerhatikan:\n• 42 bukan kelipatan 28.\n• 168 adalah kelipatan persekutuan yang lebih besar.\n• KPK yang benar = 84."
        ));

        soal.Add(BuatSoal(
            16, 20,
            "80", "40", "160",
            0,
            "Hebat! KPK dari 16 dan 20 adalah 80.\nPenjelasan:\n• 16 = 2⁴\n• 20 = 2² × 5\n• KPK = 2⁴ × 5 = 16 × 5 = 80.",
            "Jawaban kurang tepat.\nCatatan:\n• 40 bukan kelipatan 16.\n• 160 terlalu besar.\n• Yang paling kecil adalah 80."
        ));

        soal.Add(BuatSoal(
            24, 36,
            "72", "48", "144",
            0,
            "Sempurna! KPK dari 24 dan 36 adalah 72.\nPenjelasan:\n• 24 = 2³ × 3\n• 36 = 2² × 3²\n• KPK = 2³ × 3² = 8 × 9 = 72.\n72 adalah bilangan terkecil yang habis dibagi 24 dan 36.",
            "Belum benar.\nIngat:\n• 48 bukan kelipatan 36.\n• 144 adalah kelipatan persekutuan yang lebih besar.\n• KPK yang tepat adalah 72."
        ));

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
            instruksiTeks = "Ketik KPK yang tepat!",
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