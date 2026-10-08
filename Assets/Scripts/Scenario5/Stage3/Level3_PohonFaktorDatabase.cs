using System.Collections.Generic;
using UnityEngine;

public static class Level3_PohonFaktorDatabase
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

    // =========================================================================
    // LEVEL 1 - MUDAH (10 SOAL)
    // =========================================================================
    public static List<DataSoalPohonFaktor> GetSoalLevel1()
    {
        List<DataSoalPohonFaktor> list = new List<DataSoalPohonFaktor>();

        list.Add(BuatSoal(8, 12, "2 x 2 x 2", "2 x 2 x 3", 4, "<color=green>Tepat!</color> FPB dari 8 dan 12 adalah 4."));
        list.Add(BuatSoal(12, 18, "2 x 2 x 3", "2 x 3 x 3", 6, "<color=green>Kuat!</color> 6 adalah FPB dari 12 dan 18."));
        list.Add(BuatSoal(16, 20, "2 x 2 x 2 x 2", "2 x 2 x 5", 4, "<color=green>Sempurna!</color> FPB dari 16 dan 20 adalah 4."));
        list.Add(BuatSoal(10, 15, "2 x 5", "3 x 5", 5, "<color=green>Benar!</color> FPB dari 10 dan 15 adalah 5."));
        list.Add(BuatSoal(14, 21, "2 x 7", "3 x 7", 7, "<color=green>Hebat!</color> 7 adalah perisai yang tepat."));
        list.Add(BuatSoal(18, 24, "2 x 3 x 3", "2 x 2 x 2 x 3", 6, "<color=green>Kuat!</color> 6 adalah FPB dari 18 dan 24."));
        list.Add(BuatSoal(9, 15, "3 x 3", "3 x 5", 3, "<color=green>Tepat!</color> 3 adalah FPB dari 9 dan 15."));
        list.Add(BuatSoal(20, 24, "2 x 2 x 5", "2 x 2 x 2 x 3", 4, "<color=green>Sempurna!</color> 4 menahan kedua serangan."));
        list.Add(BuatSoal(16, 24, "2 x 2 x 2 x 2", "2 x 2 x 2 x 3", 8, "<color=green>Bagus!</color> 8 adalah FPB dari 16 dan 24."));
        list.Add(BuatSoal(15, 25, "3 x 5", "5 x 5", 5, "<color=green>Hebat!</color> 5 membagi 15 dan 25."));

        return list;
    }

    // =========================================================================
    // LEVEL 2 - SEDANG (10 SOAL)
    // =========================================================================
    public static List<DataSoalPohonFaktor> GetSoalLevel2()
    {
        List<DataSoalPohonFaktor> list = new List<DataSoalPohonFaktor>();

        list.Add(BuatSoal(24, 36, "2 x 2 x 2 x 3", "2 x 2 x 3 x 3", 12, "<color=green>Luar biasa!</color> 12 adalah FPB dari 24 dan 36."));
        list.Add(BuatSoal(30, 45, "2 x 3 x 5", "3 x 3 x 5", 15, "<color=green>Tepat!</color> 15 menahan ledakan 30 dan 45."));
        list.Add(BuatSoal(28, 42, "2 x 2 x 7", "2 x 3 x 7", 14, "<color=green>Kuat!</color> 14 adalah FPB dari 28 dan 42."));
        list.Add(BuatSoal(32, 48, "2 x 2 x 2 x 2 x 2", "2 x 2 x 2 x 2 x 3", 16, "<color=green>Sempurna!</color> 16 adalah FPB dari 32 dan 48."));
        list.Add(BuatSoal(20, 50, "2 x 2 x 5", "2 x 5 x 5", 10, "<color=green>Benar!</color> 10 menahan sihir 20 dan 50."));
        list.Add(BuatSoal(36, 48, "2 x 2 x 3 x 3", "2 x 2 x 2 x 2 x 3", 12, "<color=green>Hebat!</color> 12 adalah FPB dari 36 dan 48."));
        list.Add(BuatSoal(24, 40, "2 x 2 x 2 x 3", "2 x 2 x 2 x 5", 8, "<color=green>Bagus!</color> 8 memecah serangan 24 dan 40."));
        list.Add(BuatSoal(40, 60, "2 x 2 x 2 x 5", "2 x 2 x 3 x 5", 20, "<color=green>Tepat!</color> 20 menahan serangan api dan es."));
        list.Add(BuatSoal(35, 56, "5 x 7", "2 x 2 x 2 x 7", 7, "<color=green>Kuat!</color> 7 adalah FPB dari 35 dan 56."));
        list.Add(BuatSoal(27, 45, "3 x 3 x 3", "3 x 3 x 5", 9, "<color=green>Sempurna!</color> 9 adalah FPB 27 dan 45."));

        return list;
    }

    // =========================================================================
    // LEVEL 3 - SULIT (10 SOAL)
    // =========================================================================
    public static List<DataSoalPohonFaktor> GetSoalLevel3()
    {
        List<DataSoalPohonFaktor> list = new List<DataSoalPohonFaktor>();

        // Soal Utama Sesuai Skema Cerita (Kereta 24 dan 36)
        list.Add(BuatSoal(24, 36, "2 x 2 x 2 x 3", "2 x 2 x 3 x 3", 12, "<color=green>Luar Biasa!</color> Benteng kokoh menahan 24 dan 36!"));
        list.Add(BuatSoal(48, 72, "2 x 2 x 2 x 2 x 3", "2 x 2 x 2 x 3 x 3", 24, "<color=green>Kuat!</color> 24 menahan serangan masif."));
        list.Add(BuatSoal(54, 90, "2 x 3 x 3 x 3", "2 x 3 x 3 x 5", 18, "<color=green>Sempurna!</color> 18 adalah FPB 54 dan 90."));
        list.Add(BuatSoal(60, 84, "2 x 2 x 3 x 5", "2 x 2 x 3 x 7", 12, "<color=green>Kuat!</color> 12 adalah perisai yang tepat."));
        list.Add(BuatSoal(50, 75, "2 x 5 x 5", "3 x 5 x 5", 25, "<color=green>Benar!</color> 25 membagi 50 dan 75."));
        list.Add(BuatSoal(64, 96, "2 x 2 x 2 x 2 x 2 x 2", "2 x 2 x 2 x 2 x 2 x 3", 32, "<color=green>Epik!</color> 32 memantulkan serangan 64 dan 96."));
        list.Add(BuatSoal(70, 105, "2 x 5 x 7", "3 x 5 x 7", 35, "<color=green>Tepat!</color> 35 adalah FPB dari 70 dan 105."));
        list.Add(BuatSoal(72, 108, "2 x 2 x 2 x 3 x 3", "2 x 2 x 3 x 3 x 3", 36, "<color=green>Sempurna!</color> 36 menetralkan serangan."));
        list.Add(BuatSoal(80, 120, "2 x 2 x 2 x 2 x 5", "2 x 2 x 2 x 3 x 5", 40, "<color=green>Kemenangan!</color> 40 adalah FPB 80 dan 120."));
        list.Add(BuatSoal(56, 98, "2 x 2 x 2 x 7", "2 x 7 x 7", 14, "<color=green>Hebat!</color> 14 menahan serangan 56 dan 98."));

        return list;
    }

    private static DataSoalPohonFaktor BuatSoal(int mApi, int mEs, string fApi, string fEs, int bnr, string fbBnr)
    {
        return new DataSoalPohonFaktor
        {
            angkaApi = mApi,
            angkaEs = mEs,
            pohonFaktorApi = fApi,
            pohonFaktorEs = fEs,
            pilihanJawaban = new List<OpsiJawaban>
            {
                new OpsiJawaban { angka = bnr, isBenar = true, feedbackTeks = fbBnr }
            }
        };
    }
}