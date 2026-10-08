using System.Collections.Generic;

public static class BankSoal_Stage3_Lv2
{
    public static void MuatSoal(Dictionary<int, DataSoalSuhu> database)
    {
        // ⚔️ LEVEL 2.1: PERKALIAN BERANTAI (Sub Level = 4)
        TambahSoal(database, 31, "Kali", 4, "[ × 4 ]", 4, "[ × (-4) ]", -4, "Suhu saat ini adalah 6°C. Target suhu berikutnya adalah 24°C.", "Pilih operasi yang tepat:", "Tepat!\n\n6 × 4 = 24°C.", "Salah!\n\n6 × (-4) = -24°C. Malah membeku!");
        TambahSoal(database, 32, "Kali", 5, "[ × 5 ]", 5, "[ × (-5) ]", -5, "Suhu saat ini adalah -6°C. Target suhu berikutnya adalah ekstrem beku -30°C.", "Pilih operasi yang tepat:", "Tepat!\n\n(-6) × 5 = -30°C.", "Salah!\n\n(-6) × (-5) = 30°C. Suhu berbalik!");
        TambahSoal(database, 33, "Kali", -2, "[ × (-2) ]", -2, "[ × 2 ]", 2, "Suhu saat ini adalah -8°C. Suhu harus diubah menjadi hangat 16°C.", "Pilih operasi yang tepat:", "Brilian!\n\n(-8) × (-2) = 16°C.", "Salah!\n\n(-8) × 2 = -16°C. Masih beku!");
        TambahSoal(database, 34, "Kali", 3, "[ × 3 ]", 3, "[ × (-3) ]", -3, "Suhu saat ini adalah 15°C. Target suhu berikutnya adalah 45°C.", "Pilih operasi yang tepat:", "Bagus!\n\n15 × 3 = 45°C.", "Salah!\n\n15 × (-3) = -45°C.");
        TambahSoal(database, 35, "Kali", -4, "[ × (-4) ]", -4, "[ × 4 ]", 4, "Suhu saat ini adalah 10°C. Ruangan harus dibekukan menjadi -40°C.", "Pilih operasi yang tepat:", "Tepat!\n\n10 × (-4) = -40°C.", "Salah!\n\n10 × 4 = 40°C.");
        TambahSoal(database, 36, "Kali", -4, "[ × (-4) ]", -4, "[ × 4 ]", 4, "Suhu saat ini adalah -8°C. Suhu harus dibalikkan menjadi 32°C.", "Pilih operasi yang tepat:", "Sempurna!\n\n(-8) × (-4) = 32°C.", "Salah!\n\n(-8) × 4 = -32°C.");
        TambahSoal(database, 37, "Kali", -2, "[ × (-2) ]", -2, "[ × 2 ]", 2, "Suhu saat ini adalah 8°C. Suhu harus dikembalikan menjadi beku -16°C.", "Pilih operasi yang tepat:", "Benar!\n\n8 × (-2) = -16°C.", "Salah!\n\n8 × 2 = 16°C.");
        TambahSoal(database, 38, "Kali", 2, "[ × 2 ]", 2, "[ × (-2) ]", -2, "Suhu saat ini adalah -18°C. Target suhu berikutnya adalah ekstrem -36°C.", "Pilih operasi yang tepat:", "Tepat!\n\n(-18) × 2 = -36°C.", "Salah!\n\n(-18) × (-2) = 36°C.");
        TambahSoal(database, 39, "Kali", -5, "[ × (-5) ]", -5, "[ × 5 ]", 5, "Suhu saat ini adalah -5°C. Suhu harus dibalikkan menjadi 25°C.", "Pilih operasi yang tepat:", "Luar biasa!\n\n(-5) × (-5) = 25°C.", "Salah!\n\n(-5) × 5 = -25°C.");
        TambahSoal(database, 40, "Kali", 4, "[ × 4 ]", 4, "[ × (-4) ]", -4, "Suhu saat ini adalah 12°C. Target suhu berikutnya adalah 48°C.", "Pilih operasi yang tepat:", "Tepat!\n\n12 × 4 = 48°C.", "Salah!\n\n12 × (-4) = -48°C.");

        // ⚔️ LEVEL 2.2: PEMBAGIAN BERANTAI (Sub Level = 5)
        TambahSoal(database, 41, "Bagi", 4, "[ ÷ 4 ]", 4, "[ ÷ (-4) ]", -4, "Suhu ekstrem saat ini adalah 40°C. Target suhu normal adalah 10°C.", "Pilih operasi pembagi yang tepat:", "Benar!\n\n40 ÷ 4 = 10°C.", "Salah!\n\n40 ÷ (-4) = -10°C.");
        TambahSoal(database, 42, "Bagi", 4, "[ ÷ 4 ]", 4, "[ ÷ (-4) ]", -4, "Suhu saat ini adalah -20°C. Suhu harus dilemahkan menjadi -5°C.", "Pilih operasi pembagi yang tepat:", "Bagus!\n\n(-20) ÷ 4 = -5°C.", "Salah!\n\n(-20) ÷ (-4) = 5°C.");
        TambahSoal(database, 43, "Bagi", -4, "[ ÷ (-4) ]", -4, "[ ÷ 4 ]", 4, "Suhu es saat ini adalah -24°C. Suhu harus dibalikkan menjadi hangat 6°C.", "Pilih operasi pembagi yang tepat:", "Brilian!\n\n(-24) ÷ (-4) = 6°C.", "Salah!\n\n(-24) ÷ 4 = -6°C.");
        TambahSoal(database, 44, "Bagi", 10, "[ ÷ 10 ]", 10, "[ ÷ (-10) ]", -10, "Suhu badai saat ini adalah 50°C. Suhu harus dilemahkan menjadi 5°C.", "Pilih operasi pembagi yang tepat:", "Tepat!\n\n50 ÷ 10 = 5°C.", "Salah!\n\n50 ÷ (-10) = -5°C.");
        TambahSoal(database, 45, "Bagi", -4, "[ ÷ (-4) ]", -4, "[ ÷ 4 ]", 4, "Suhu radiasi saat ini adalah 24°C. Suhu harus dibalikkan secara instan menjadi -6°C.", "Pilih operasi pembagi yang tepat:", "Sempurna!\n\n24 ÷ (-4) = -6°C.", "Salah!\n\n24 ÷ 4 = 6°C.");
        TambahSoal(database, 46, "Bagi", -2, "[ ÷ (-2) ]", -2, "[ ÷ 2 ]", 2, "Suhu racun es saat ini adalah -16°C. Suhu harus dibalikkan menjadi 8°C.", "Pilih operasi pembagi yang tepat:", "Benar!\n\n(-16) ÷ (-2) = 8°C.", "Salah!\n\n(-16) ÷ 2 = -8°C.");
        TambahSoal(database, 47, "Bagi", -5, "[ ÷ (-5) ]", -5, "[ ÷ 5 ]", 5, "Suhu anomali saat ini adalah 30°C. Suhu harus diubah kembali menjadi es -6°C.", "Pilih operasi pembagi yang tepat:", "Tepat!\n\n30 ÷ (-5) = -6°C.", "Salah!\n\n30 ÷ 5 = 6°C.");
        TambahSoal(database, 48, "Bagi", 3, "[ ÷ 3 ]", 3, "[ ÷ (-3) ]", -3, "Suhu batu es saat ini adalah -30°C. Suhu harus dilemahkan menjadi -10°C.", "Pilih operasi pembagi yang tepat:", "Bagus!\n\n(-30) ÷ 3 = -10°C.", "Salah!\n\n(-30) ÷ (-3) = 10°C.");
        TambahSoal(database, 49, "Bagi", -5, "[ ÷ (-5) ]", -5, "[ ÷ 5 ]", 5, "Suhu es abadi saat ini adalah -10°C. Suhu harus dibalikkan menjadi air hangat 2°C.", "Pilih operasi pembagi yang tepat:", "Sempurna!\n\n(-10) ÷ (-5) = 2°C.", "Salah!\n\n(-10) ÷ 5 = -2°C.");
        TambahSoal(database, 50, "Bagi", 2, "[ ÷ 2 ]", 2, "[ ÷ (-2) ]", -2, "Suhu uap panas saat ini adalah 30°C. Target suhu ruang adalah 15°C.", "Pilih operasi pembagi yang tepat:", "Tepat!\n\n30 ÷ 2 = 15°C.", "Salah!\n\n30 ÷ (-2) = -15°C.");

        // ⚔️ LEVEL 2.3: EVALUASI CAMPURAN (Sub Level = 6)
        TambahSoal(database, 51, "Kali", -3, "[ × (-3) ]", -3, "[ × 3 ]", 3, "Suhu saat ini adalah -20°C. Hancurkan racun es ini dengan mengubahnya menjadi panas 60°C!", "Pilih operasi yang tepat:", "Sempurna!\n\n(-20) × (-3) = 60°C.", "Salah!\n\n(-20) × 3 = -60°C.");
        TambahSoal(database, 52, "Bagi", 10, "[ ÷ 10 ]", 10, "[ ÷ (-10) ]", -10, "Suhu badai raksasa saat ini adalah -100°C. Lemahkan badai ini menjadi -10°C!", "Pilih operasi yang tepat:", "Luar Biasa!\n\n(-100) ÷ 10 = -10°C.", "Salah!\n\n(-100) ÷ (-10) = 10°C.");
        TambahSoal(database, 53, "Bagi", -4, "[ ÷ (-4) ]", -4, "[ ÷ 4 ]", 4, "Suhu saat ini adalah 32°C. Balikkan kembali menjadi es -8°C!", "Pilih operasi yang tepat:", "Tepat!\n\n32 ÷ (-4) = -8°C.", "Salah!\n\n32 ÷ 4 = 8°C.");
        TambahSoal(database, 54, "Kali", 4, "[ × 4 ]", 4, "[ × (-4) ]", -4, "Suhu es saat ini adalah -12°C. Bekukan lebih tebal lagi menjadi -48°C!", "Pilih operasi yang tepat:", "Bagus!\n\n(-12) × 4 = -48°C.", "Salah!\n\n(-12) × (-4) = 48°C.");
        TambahSoal(database, 55, "Kali", -7, "[ × (-7) ]", -7, "[ × 7 ]", 7, "Suhu es magis saat ini adalah -5°C. Hancurkan dengan membalikkan suhu seketika ke 35°C!", "Pilih operasi yang tepat:", "Brilian!\n\n(-5) × (-7) = 35°C.", "Salah!\n\n(-5) × 7 = -35°C.");
        TambahSoal(database, 56, "Bagi", -8, "[ ÷ (-8) ]", -8, "[ ÷ 8 ]", 8, "Suhu ruangan terkutuk saat ini adalah -48°C. Pecahkan kutukan dan balikkan kembali ke hangat 6°C!", "Pilih operasi yang tepat:", "Sempurna!\n\n(-48) ÷ (-8) = 6°C.", "Salah!\n\n(-48) ÷ 8 = -6°C.");
        TambahSoal(database, 57, "Kali", -5, "[ × (-5) ]", -5, "[ × 5 ]", 5, "Suhu air panas saat ini adalah 20°C. Bekukan kembali secara ekstrem ke -100°C!", "Pilih operasi yang tepat:", "Benar!\n\n20 × (-5) = -100°C.", "Salah!\n\n20 × 5 = 100°C.");
        TambahSoal(database, 58, "Bagi", -4, "[ ÷ (-4) ]", -4, "[ ÷ 4 ]", 4, "Suhu beku absolut saat ini adalah -100°C. Netralkan menjadi hangat 25°C!", "Pilih operasi yang tepat:", "Tepat!\n\n(-100) ÷ (-4) = 25°C.", "Salah!\n\n(-100) ÷ 4 = -25°C.");
        TambahSoal(database, 59, "Kali", 9, "[ × 9 ]", 9, "[ × (-9) ]", -9, "Suhu ruangan saat ini adalah hangat 9°C. Bakar ruangan ini hingga mencapai 81°C!", "Pilih operasi yang tepat:", "Luar Biasa!\n\n9 × 9 = 81°C.", "Salah!\n\n9 × (-9) = -81°C.");
        TambahSoal(database, 60, "Kali", -5, "[ × (-5) ]", -5, "[ × 5 ]", 5, "Suhu es reaktor saat ini adalah -20°C. Panaskan kembali sistem ke 100°C!", "Pilih operasi yang tepat:", "Master Suhu!\n\n(-20) × (-5) = 100°C.", "Salah!\n\n(-20) × 5 = -100°C.");
    }

    private static void TambahSoal(Dictionary<int, DataSoalSuhu> db, int id, string op, int jwb, string tA, int vA, string tB, int vB, string nar, string tny, string pBenar, string pSalah)
    {
        db.Add(id, new DataSoalSuhu {
            idSoal = id, jenisOperasi = op, jawabanBenar = jwb,
            teksTombolA = tA, nilaiA = vA, teksTombolB = tB, nilaiB = vB,
            narasi = nar, pertanyaan = tny, penjelasanBenar = pBenar, penjelasanSalah = pSalah
        });
    }
}