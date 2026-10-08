using System.Collections.Generic;

public static class FPBQuestionBank
{
    public struct OpsiJawaban
    {
        public int angka;
        public bool isBenar;
        public string feedbackTeks;
    }

    public struct DataSoalFPB
    {
        public int angkaMeriam1;
        public int angkaMeriam2;
        public string teksFaktor1;
        public string teksFaktor2;
        public string teksFaktorSama;
        public List<OpsiJawaban> pilihanJawaban;
    }

    // ===================== SUB LEVEL 1 - MUDAH (Angka 6-27) =====================
    public static List<DataSoalFPB> GetSoalSubLevel1()
    {
        List<DataSoalFPB> list = new List<DataSoalFPB>();

        list.Add(BuatSoal(6, 8,
            "1, 2, 3, 6", "1, 2, 4, 8", "1, 2",
            2, "<color=green>Tepat!</color> 2 adalah FPB dari 6 dan 8.",
            3, "<color=red>Salah!</color> 3 tidak membagi 8.",
            4, "<color=red>Salah!</color> 4 tidak membagi 6."));

        list.Add(BuatSoal(8, 12,
            "1, 2, 4, 8", "1, 2, 3, 4, 6, 12", "1, 2, 4",
            4, "<color=green>Bagus!</color> 4 adalah FPB dari 8 dan 12.",
            2, "<color=red>Kurang kuat!</color> 2 bukan yang terbesar.",
            6, "<color=red>Salah!</color> 6 tidak membagi 8."));

        list.Add(BuatSoal(9, 15,
            "1, 3, 9", "1, 3, 5, 15", "1, 3",
            3, "<color=green>Benar!</color> 3 adalah FPB dari 9 dan 15.",
            5, "<color=red>Salah!</color> 5 tidak membagi 9.",
            9, "<color=red>Salah!</color> 9 tidak membagi 15."));

        list.Add(BuatSoal(10, 15,
            "1, 2, 5, 10", "1, 3, 5, 15", "1, 5",
            5, "<color=green>Hebat!</color> 5 adalah FPB dari 10 dan 15.",
            2, "<color=red>Salah!</color> 2 tidak membagi 15.",
            10, "<color=red>Salah!</color> 10 tidak membagi 15."));

        list.Add(BuatSoal(12, 16,
            "1, 2, 3, 4, 6, 12", "1, 2, 4, 8, 16", "1, 2, 4",
            4, "<color=green>Sempurna!</color> 4 adalah FPB dari 12 dan 16.",
            2, "<color=red>Terlalu kecil!</color> Masih ada faktor persekutuan yang lebih besar.",
            8, "<color=red>Salah!</color> 8 tidak membagi 12."));

        list.Add(BuatSoal(14, 21,
            "1, 2, 7, 14", "1, 3, 7, 21", "1, 7",
            7, "<color=green>Tepat!</color> 7 adalah FPB dari 14 dan 21.",
            3, "<color=red>Salah!</color> 3 tidak membagi 14.",
            14, "<color=red>Salah!</color> 14 tidak membagi 21."));

        list.Add(BuatSoal(15, 20,
            "1, 3, 5, 15", "1, 2, 4, 5, 10, 20", "1, 5",
            5, "<color=green>Benar!</color> 5 adalah FPB dari 15 dan 20.",
            3, "<color=red>Salah!</color> 3 tidak membagi 20.",
            10, "<color=red>Salah!</color> 10 tidak membagi 15."));

        list.Add(BuatSoal(6, 9,
            "1, 2, 3, 6", "1, 3, 9", "1, 3",
            3, "<color=green>Hebat!</color> 3 adalah FPB dari 6 dan 9.",
            2, "<color=red>Salah!</color> 2 tidak membagi 9.",
            6, "<color=red>Salah!</color> 6 tidak membagi 9."));

        list.Add(BuatSoal(10, 12,
            "1, 2, 5, 10", "1, 2, 3, 4, 6, 12", "1, 2",
            2, "<color=green>Tepat!</color> 2 adalah FPB dari 10 dan 12.",
            4, "<color=red>Salah!</color> 4 tidak membagi 10.",
            5, "<color=red>Salah!</color> 5 tidak membagi 12."));

        list.Add(BuatSoal(18, 27,
            "1, 2, 3, 6, 9, 18", "1, 3, 9, 27", "1, 3, 9",
            9, "<color=green>Sempurna!</color> 9 adalah FPB dari 18 dan 27.",
            3, "<color=red>Kurang kuat!</color> 3 bukan yang terbesar.",
            6, "<color=red>Salah!</color> 6 tidak membagi 27."));

        return list;
    }

    // ===================== SUB LEVEL 2 - SEDANG (Angka 16-48) =====================
    public static List<DataSoalFPB> GetSoalSubLevel2()
    {
        List<DataSoalFPB> list = new List<DataSoalFPB>();

        list.Add(BuatSoal(16, 24,
            "1, 2, 4, 8, 16", "1, 2, 3, 4, 6, 8, 12, 24", "1, 2, 4, 8",
            8, "<color=green>Hebat!</color> 8 adalah FPB dari 16 dan 24.",
            4, "<color=red>Masih ada yang lebih besar!</color>",
            12, "<color=red>Salah!</color> 12 tidak membagi 16."));

        list.Add(BuatSoal(20, 30,
            "1, 2, 4, 5, 10, 20", "1, 2, 3, 5, 6, 10, 15, 30", "1, 2, 5, 10",
            10, "<color=green>Benar!</color> 10 adalah FPB dari 20 dan 30.",
            5, "<color=red>Kurang besar!</color>",
            15, "<color=red>Salah!</color> 15 tidak membagi 20."));

        list.Add(BuatSoal(24, 32,
            "1, 2, 3, 4, 6, 8, 12, 24", "1, 2, 4, 8, 16, 32", "1, 2, 4, 8",
            8, "<color=green>Tepat!</color> 8 adalah FPB dari 24 dan 32.",
            4, "<color=red>Terlalu kecil!</color>",
            6, "<color=red>Salah!</color> 6 tidak membagi 32."));

        list.Add(BuatSoal(18, 24,
            "1, 2, 3, 6, 9, 18", "1, 2, 3, 4, 6, 8, 12, 24", "1, 2, 3, 6",
            6, "<color=green>Bagus!</color> 6 adalah FPB dari 18 dan 24.",
            3, "<color=red>Masih ada yang lebih besar!</color>",
            9, "<color=red>Salah!</color> 9 tidak membagi 24."));

        list.Add(BuatSoal(28, 42,
            "1, 2, 4, 7, 14, 28", "1, 2, 3, 6, 7, 14, 21, 42", "1, 2, 7, 14",
            14, "<color=green>Sempurna!</color> 14 adalah FPB dari 28 dan 42.",
            7, "<color=red>Terlalu kecil!</color>",
            21, "<color=red>Salah!</color> 21 tidak membagi 28."));

        list.Add(BuatSoal(25, 35,
            "1, 5, 25", "1, 5, 7, 35", "1, 5",
            5, "<color=green>Hebat!</color> 5 adalah FPB dari 25 dan 35.",
            10, "<color=red>Salah!</color> 10 bukan faktor dari keduanya.",
            15, "<color=red>Salah!</color> 15 bukan faktor dari keduanya."));

        list.Add(BuatSoal(30, 45,
            "1, 2, 3, 5, 6, 10, 15, 30", "1, 3, 5, 9, 15, 45", "1, 3, 5, 15",
            15, "<color=green>Tepat!</color> 15 adalah FPB dari 30 dan 45.",
            5, "<color=red>Kurang besar!</color>",
            10, "<color=red>Salah!</color> 10 tidak membagi 45."));

        list.Add(BuatSoal(24, 36,
            "1, 2, 3, 4, 6, 8, 12, 24", "1, 2, 3, 4, 6, 9, 12, 18, 36", "1, 2, 3, 4, 6, 12",
            12, "<color=green>Benar!</color> 12 membagi habis 24 dan 36.",
            6, "<color=red>Masih ada yang lebih besar!</color>",
            18, "<color=red>Salah!</color> 18 tidak membagi 24."));

        list.Add(BuatSoal(32, 48,
            "1, 2, 4, 8, 16, 32", "1, 2, 3, 4, 6, 8, 12, 16, 24, 48", "1, 2, 4, 8, 16",
            16, "<color=green>Luar biasa!</color> 16 adalah FPB dari 32 dan 48.",
            8, "<color=red>Kurang besar!</color>",
            12, "<color=red>Salah!</color> 12 tidak membagi 32."));

        list.Add(BuatSoal(21, 35,
            "1, 3, 7, 21", "1, 5, 7, 35", "1, 7",
            7, "<color=green>Sempurna!</color> 7 adalah FPB dari 21 dan 35.",
            3, "<color=red>Salah!</color> 3 tidak membagi 35.",
            14, "<color=red>Salah!</color> 14 bukan faktor dari keduanya."));

        return list;
    }

    // ===================== SUB LEVEL 3 - SULIT (Angka 36-120) =====================
    public static List<DataSoalFPB> GetSoalSubLevel3()
    {
        List<DataSoalFPB> list = new List<DataSoalFPB>();

        list.Add(BuatSoal(36, 54,
            "1, 2, 3, 4, 6, 9, 12, 18, 36", "1, 2, 3, 6, 9, 18, 27, 54", "1, 2, 3, 6, 9, 18",
            18, "<color=green>Sempurna!</color> 18 adalah FPB dari 36 dan 54.",
            9, "<color=red>Masih ada yang lebih besar!</color>",
            12, "<color=red>Salah!</color> 12 tidak membagi 54."));

        list.Add(BuatSoal(40, 60,
            "1, 2, 4, 5, 8, 10, 20, 40", "1, 2, 3, 4, 5, 6, 10, 12, 15, 20, 30, 60", "1, 2, 4, 5, 10, 20",
            20, "<color=green>Hebat!</color> 20 adalah FPB dari 40 dan 60.",
            10, "<color=red>Kurang besar!</color>",
            30, "<color=red>Salah!</color> 30 tidak membagi 40."));

        list.Add(BuatSoal(45, 75,
            "1, 3, 5, 9, 15, 45", "1, 3, 5, 15, 25, 75", "1, 3, 5, 15",
            15, "<color=green>Benar!</color> 15 adalah FPB dari 45 dan 75.",
            5, "<color=red>Terlalu kecil!</color>",
            25, "<color=red>Salah!</color> 25 tidak membagi 45."));

        list.Add(BuatSoal(48, 72,
            "1, 2, 3, 4, 6, 8, 12, 16, 24, 48", "1, 2, 3, 4, 6, 8, 9, 12, 18, 24, 36, 72", "1, 2, 3, 4, 6, 8, 12, 24",
            24, "<color=green>Fantastis!</color> 24 adalah FPB dari 48 dan 72.",
            12, "<color=red>Masih ada yang lebih besar!</color>",
            16, "<color=red>Salah!</color> 16 tidak membagi 72."));

        list.Add(BuatSoal(50, 75,
            "1, 2, 5, 10, 25, 50", "1, 3, 5, 15, 25, 75", "1, 5, 25",
            25, "<color=green>Tepat!</color> 25 membagi habis 50 dan 75.",
            15, "<color=red>Salah!</color> 15 tidak membagi 50.",
            50, "<color=red>Salah!</color> 50 tidak membagi 75."));

        list.Add(BuatSoal(64, 96,
            "1, 2, 4, 8, 16, 32, 64", "1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 96", "1, 2, 4, 8, 16, 32",
            32, "<color=green>Kuat sekali!</color> 32 adalah FPB dari 64 dan 96.",
            16, "<color=red>Kurang besar!</color>",
            24, "<color=red>Salah!</color> 24 tidak membagi 64."));

        list.Add(BuatSoal(42, 56,
            "1, 2, 3, 6, 7, 14, 21, 42", "1, 2, 4, 7, 8, 14, 28, 56", "1, 2, 7, 14",
            14, "<color=green>Sempurna!</color> 14 adalah FPB dari 42 dan 56.",
            7, "<color=red>Terlalu kecil!</color>",
            21, "<color=red>Salah!</color> 21 tidak membagi 56."));

        list.Add(BuatSoal(60, 90,
            "1, 2, 3, 4, 5, 6, 10, 12, 15, 20, 30, 60", "1, 2, 3, 5, 6, 9, 10, 15, 18, 30, 45, 90", "1, 2, 3, 5, 6, 10, 15, 30",
            30, "<color=green>Luar biasa!</color> 30 adalah FPB dari 60 dan 90.",
            15, "<color=red>Masih ada yang lebih besar!</color>",
            20, "<color=red>Salah!</color> 20 tidak membagi 90."));

        list.Add(BuatSoal(36, 48,
            "1, 2, 3, 4, 6, 9, 12, 18, 36", "1, 2, 3, 4, 6, 8, 12, 16, 24, 48", "1, 2, 3, 4, 6, 12",
            12, "<color=green>Bagus!</color> 12 adalah FPB dari 36 dan 48.",
            6, "<color=red>Kurang besar!</color>",
            18, "<color=red>Salah!</color> 18 tidak membagi 48."));

        list.Add(BuatSoal(80, 120,
            "1, 2, 4, 5, 8, 10, 16, 20, 40, 80", "1, 2, 3, 4, 5, 6, 8, 10, 12, 15, 20, 24, 30, 40, 60, 120", "1, 2, 4, 5, 8, 10, 20, 40",
            40, "<color=green>Kemenangan!</color> 40 adalah FPB dari 80 dan 120.",
            20, "<color=red>Terlalu kecil!</color>",
            60, "<color=red>Salah!</color> 60 tidak membagi 80."));

        return list;
    }

    // Helper pembuat soal
    private static DataSoalFPB BuatSoal(int m1, int m2, string f1, string f2, string fs,
                                       int bnr, string fbBnr, int slh1, string fbSlh1, int slh2, string fbSlh2)
    {
        return new DataSoalFPB
        {
            angkaMeriam1 = m1,
            angkaMeriam2 = m2,
            teksFaktor1 = f1,
            teksFaktor2 = f2,
            teksFaktorSama = fs,
            pilihanJawaban = new List<OpsiJawaban>
            {
                new OpsiJawaban { angka = bnr, isBenar = true,  feedbackTeks = fbBnr },
                new OpsiJawaban { angka = slh1, isBenar = false, feedbackTeks = fbSlh1 },
                new OpsiJawaban { angka = slh2, isBenar = false, feedbackTeks = fbSlh2 }
            }
        };
    }
}