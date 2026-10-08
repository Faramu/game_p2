using System.Collections.Generic;
using UnityEngine;

public static class Level2_PohonFaktorDatabase
{
    public struct OpsiJawaban
    {
        public int angka;
        public bool isBenar;
        public string feedbackTeks;
    }

    public struct DataSoalPohonFaktor
    {
        public int angkaApi;
        public int angkaEs;
        public string pohonFaktorApi;
        public string pohonFaktorEs;
        public List<OpsiJawaban> pilihanJawaban;
    }

    // ===================== LEVEL 1 - MUDAH (Angka 6 - 25) =====================
    public static List<DataSoalPohonFaktor> GetSoalLevel1()
    {
        List<DataSoalPohonFaktor> list = new List<DataSoalPohonFaktor>();

        list.Add(BuatSoal(6, 8, "2 x 3", "2 x 2 x 2", 
            2, "<color=green>Tepat!</color> 2 menahan ledakan awal.", 
            3, "<color=red>Jebakan!</color> 3 tidak membagi 8.", 
            4, "<color=red>Hancur!</color> 4 tidak membagi 6."));

        list.Add(BuatSoal(8, 12, "2 x 2 x 2", "2 x 2 x 3", 
            4, "<color=green>Tepat!</color> FPB dari 8 dan 12 adalah 4.", 
            2, "<color=red>Jebakan!</color> 2 bukan angka terbesar.", 
            8, "<color=red>Hancur!</color> 8 tidak bisa membagi 12."));

        list.Add(BuatSoal(9, 15, "3 x 3", "3 x 5", 
            3, "<color=green>Tepat!</color> 3 adalah FPB dari 9 dan 15.", 
            5, "<color=red>Jebakan!</color> 5 tidak membagi 9.", 
            9, "<color=red>Hancur!</color> 9 tidak membagi 15."));

        list.Add(BuatSoal(10, 15, "2 x 5", "3 x 5", 
            5, "<color=green>Benar!</color> FPB dari 10 dan 15 adalah 5.", 
            2, "<color=red>Jebakan!</color> 2 tidak membagi 15.", 
            10, "<color=red>Hancur!</color> 10 tidak membagi 15."));

        list.Add(BuatSoal(12, 16, "2 x 2 x 3", "2 x 2 x 2 x 2", 
            4, "<color=green>Sempurna!</color> FPB dari 12 dan 16 adalah 4.", 
            2, "<color=red>Terlalu lemah!</color> 2 bukan yang terbesar.", 
            8, "<color=red>Gagal!</color> 8 tidak membagi 12."));

        list.Add(BuatSoal(14, 21, "2 x 7", "3 x 7", 
            7, "<color=green>Hebat!</color> 7 adalah perisai yang tepat.", 
            3, "<color=red>Gagal!</color> 3 tidak membagi 14.", 
            14, "<color=red>Gagal!</color> 14 tidak membagi 21."));

        list.Add(BuatSoal(15, 20, "3 x 5", "2 x 2 x 5", 
            5, "<color=green>Kuat!</color> 5 adalah FPB 15 dan 20.", 
            3, "<color=red>Jebakan!</color> 3 tidak membagi 20.", 
            10, "<color=red>Hancur!</color> 10 tidak membagi 15."));

        list.Add(BuatSoal(18, 24, "2 x 3 x 3", "2 x 2 x 2 x 3", 
            6, "<color=green>Kuat!</color> 6 adalah FPB dari 18 dan 24.", 
            3, "<color=red>Jebakan!</color> Pilih angka yang lebih besar.", 
            9, "<color=red>Hancur!</color> 9 tidak membagi 24."));

        list.Add(BuatSoal(20, 25, "2 x 2 x 5", "5 x 5", 
            5, "<color=green>Hebat!</color> 5 membagi 20 dan 25.", 
            2, "<color=red>Gagal!</color> 2 tidak membagi 25.", 
            10, "<color=red>Gagal!</color> 10 tidak membagi 25."));

        list.Add(BuatSoal(16, 24, "2 x 2 x 2 x 2", "2 x 2 x 2 x 3", 
            8, "<color=green>Bagus!</color> 8 adalah FPB dari 16 dan 24.", 
            4, "<color=red>Jebakan!</color> Masih ada yang lebih besar.", 
            12, "<color=red>Hancur!</color> 12 tidak membagi 16."));

        return list;
    }

    // ===================== LEVEL 2 - SEDANG (Angka 24 - 60) =====================
    public static List<DataSoalPohonFaktor> GetSoalLevel2()
    {
        List<DataSoalPohonFaktor> list = new List<DataSoalPohonFaktor>();

        list.Add(BuatSoal(24, 36, "2 x 2 x 2 x 3", "2 x 2 x 3 x 3", 
            12, "<color=green>Luar biasa!</color> 12 adalah FPB dari 24 dan 36.", 
            6, "<color=red>Jebakan!</color> 6 bukan yang terbesar.", 
            18, "<color=red>Gagal!</color> 18 tidak membagi 24."));

        list.Add(BuatSoal(30, 45, "2 x 3 x 5", "3 x 3 x 5", 
            15, "<color=green>Tepat!</color> 15 menahan ledakan 30 dan 45.", 
            5, "<color=red>Terlalu kecil!</color> Masih ada 15.", 
            10, "<color=red>Gagal!</color> 10 tidak membagi 45."));

        list.Add(BuatSoal(28, 42, "2 x 2 x 7", "2 x 3 x 7", 
            14, "<color=green>Kuat!</color> 14 adalah FPB dari 28 dan 42.", 
            7, "<color=red>Jebakan!</color> 7 bukan yang terbesar.", 
            21, "<color=red>Hancur!</color> 21 tidak membagi 28."));

        list.Add(BuatSoal(32, 48, "2 x 2 x 2 x 2 x 2", "2 x 2 x 2 x 2 x 3", 
            16, "<color=green>Sempurna!</color> 16 adalah FPB dari 32 dan 48.", 
            8, "<color=red>Lemah!</color> Masih ada faktor persekutuan lebih besar.", 
            24, "<color=red>Gagal!</color> 24 tidak membagi 32."));

        list.Add(BuatSoal(20, 50, "2 x 2 x 5", "2 x 5 x 5", 
            10, "<color=green>Benar!</color> 10 menahan sihir 20 dan 50.", 
            5, "<color=red>Jebakan!</color> 5 bukan yang terbesar.", 
            20, "<color=red>Gagal!</color> 20 tidak membagi 50."));

        list.Add(BuatSoal(36, 48, "2 x 2 x 3 x 3", "2 x 2 x 2 x 2 x 3", 
            12, "<color=green>Hebat!</color> 12 adalah FPB dari 36 dan 48.", 
            6, "<color=red>Terlalu kecil!</color>", 
            18, "<color=red>Hancur!</color> 18 tidak membagi 48."));

        list.Add(BuatSoal(24, 40, "2 x 2 x 2 x 3", "2 x 2 x 2 x 5", 
            8, "<color=green>Bagus!</color> 8 memecah serangan 24 dan 40.", 
            4, "<color=red>Jebakan!</color> 4 bukan yang terbesar.", 
            12, "<color=red>Gagal!</color> 12 tidak membagi 40."));

        list.Add(BuatSoal(40, 60, "2 x 2 x 2 x 5", "2 x 2 x 3 x 5", 
            20, "<color=green>Tepat!</color> 20 menahan serangan api dan es.", 
            10, "<color=red>Lemah!</color> Ada faktor yang lebih tebal.", 
            30, "<color=red>Gagal!</color> 30 tidak membagi 40."));

        list.Add(BuatSoal(35, 56, "5 x 7", "2 x 2 x 2 x 7", 
            7, "<color=green>Kuat!</color> 7 adalah FPB dari 35 dan 56.", 
            5, "<color=red>Gagal!</color> 5 tidak membagi 56.", 
            14, "<color=red>Hancur!</color> 14 tidak membagi 35."));

        list.Add(BuatSoal(27, 45, "3 x 3 x 3", "3 x 3 x 5", 
            9, "<color=green>Sempurna!</color> 9 adalah FPB 27 dan 45.", 
            3, "<color=red>Jebakan!</color> Terlalu kecil.", 
            15, "<color=red>Gagal!</color> 15 tidak membagi 27."));

        return list;
    }

    // ===================== LEVEL 3 - SULIT (Angka 45 - 120) =====================
    public static List<DataSoalPohonFaktor> GetSoalLevel3()
    {
        List<DataSoalPohonFaktor> list = new List<DataSoalPohonFaktor>();

        list.Add(BuatSoal(48, 72, "2 x 2 x 2 x 2 x 3", "2 x 2 x 2 x 3 x 3", 
            24, "<color=green>Luar Biasa!</color> 24 menahan serangan masif.", 
            12, "<color=red>Lemah!</color> 12 bukan yang terbesar.", 
            36, "<color=red>Hancur!</color> 36 tidak membagi 48."));

        list.Add(BuatSoal(54, 90, "2 x 3 x 3 x 3", "2 x 3 x 3 x 5", 
            18, "<color=green>Sempurna!</color> 18 adalah FPB 54 dan 90.", 
            9, "<color=red>Jebakan!</color> 9 terlalu kecil.", 
            27, "<color=red>Gagal!</color> 27 tidak membagi 90."));

        list.Add(BuatSoal(60, 84, "2 x 2 x 3 x 5", "2 x 2 x 3 x 7", 
            12, "<color=green>Kuat!</color> 12 adalah perisai yang tepat.", 
            6, "<color=red>Jebakan!</color> Masih ada angka 12.", 
            24, "<color=red>Hancur!</color> 24 tidak membagi 60."));

        list.Add(BuatSoal(50, 75, "2 x 5 x 5", "3 x 5 x 5", 
            25, "<color=green>Benar!</color> 25 membagi 50 dan 75.", 
            5, "<color=red>Lemah!</color> Terlalu kecil.", 
            50, "<color=red>Gagal!</color> 50 tidak membagi 75."));

        list.Add(BuatSoal(64, 96, "2 x 2 x 2 x 2 x 2 x 2", "2 x 2 x 2 x 2 x 2 x 3", 
            32, "<color=green>Epik!</color> 32 memantulkan serangan 64 dan 96.", 
            16, "<color=red>Jebakan!</color> Bukan yang terbesar.", 
            48, "<color=red>Gagal!</color> 48 tidak membagi 64."));

        list.Add(BuatSoal(70, 105, "2 x 5 x 7", "3 x 5 x 7", 
            35, "<color=green>Tepat!</color> 35 adalah FPB dari 70 dan 105.", 
            7, "<color=red>Lemah!</color> 7 terlalu kecil.", 
            70, "<color=red>Hancur!</color> 70 tidak membagi 105."));

        list.Add(BuatSoal(72, 108, "2 x 2 x 2 x 3 x 3", "2 x 2 x 3 x 3 x 3", 
            36, "<color=green>Sempurna!</color> 36 menetralkan serangan.", 
            18, "<color=red>Jebakan!</color> Bukan faktor terbesar.", 
            54, "<color=red>Gagal!</color> 54 tidak membagi 72."));

        list.Add(BuatSoal(80, 120, "2 x 2 x 2 x 2 x 5", "2 x 2 x 2 x 3 x 5", 
            40, "<color=green>Kemenangan!</color> 40 adalah FPB 80 dan 120.", 
            20, "<color=red>Lemah!</color> Masih ada faktor persekutuan lain.", 
            60, "<color=red>Hancur!</color> 60 tidak membagi 80."));

        list.Add(BuatSoal(45, 105, "3 x 3 x 5", "3 x 5 x 7", 
            15, "<color=green>Benar!</color> 15 adalah FPB 45 dan 105.", 
            5, "<color=red>Jebakan!</color> Terlalu kecil.", 
            45, "<color=red>Gagal!</color> 45 tidak membagi 105."));

        list.Add(BuatSoal(56, 98, "2 x 2 x 2 x 7", "2 x 7 x 7", 
            14, "<color=green>Hebat!</color> 14 menahan serangan 56 dan 98.", 
            7, "<color=red>Lemah!</color> Bukan yang terbesar.", 
            28, "<color=red>Hancur!</color> 28 tidak membagi 98."));

        return list;
    }

    private static DataSoalPohonFaktor BuatSoal(int mApi, int mEs, string fApi, string fEs, int bnr, string fbBnr, int slh1, string fbSlh1, int slh2, string fbSlh2)
    {
        return new DataSoalPohonFaktor
        {
            angkaApi = mApi,
            angkaEs = mEs,
            pohonFaktorApi = fApi,
            pohonFaktorEs = fEs,
            pilihanJawaban = new List<OpsiJawaban>
            {
                new OpsiJawaban { angka = bnr, isBenar = true,  feedbackTeks = fbBnr },
                new OpsiJawaban { angka = slh1, isBenar = false, feedbackTeks = fbSlh1 },
                new OpsiJawaban { angka = slh2, isBenar = false, feedbackTeks = fbSlh2 }
            }
        };
    }
}