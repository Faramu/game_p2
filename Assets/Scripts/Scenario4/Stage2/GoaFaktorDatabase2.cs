using System.Collections.Generic;

public static class GoaFaktorDatabase2
{
    public static List<DataSoalObor> MuatSoal(int level)
    {
        List<DataSoalObor> bankSoal = new List<DataSoalObor>();

        if (level == 1)
        {
            // Level 1: masih bisa pakai format urutan dengan 1 faktor saja
            // (atau nanti diganti ke sistem pilih 1 jika dibutuhkan)
            bankSoal.Add(new DataSoalObor(
                "Kelelawar takut pada faktor dari 12. Nyalakan obor faktor dari kecil ke besar!",
                12,
                new int[] { 1, 2, 3, 4, 5, 6 },
                new int[] { 1, 2, 3, 4, 6 },
                "Benar! Faktor 12 dari kecil ke besar: 1 → 2 → 3 → 4 → 6.",
                "Salah. Urutan faktor 12: 1 → 2 → 3 → 4 → 6."
            ));

            bankSoal.Add(new DataSoalObor(
                "Cari dan urutkan faktor dari 15!",
                15,
                new int[] { 1, 2, 3, 5, 6, 15 },
                new int[] { 1, 3, 5, 15 },
                "Bagus! Faktor 15: 1 → 3 → 5 → 15.",
                "Seharusnya: 1 → 3 → 5 → 15."
            ));

            bankSoal.Add(new DataSoalObor(
                "Urutkan faktor dari 10 dari kecil ke besar!",
                10,
                new int[] { 1, 2, 3, 5, 10 },
                new int[] { 1, 2, 5, 10 },
                "Tepat! Faktor 10: 1 → 2 → 5 → 10.",
                "Seharusnya: 1 → 2 → 5 → 10."
            ));

            bankSoal.Add(new DataSoalObor(
                "Urutkan faktor dari 16!",
                16,
                new int[] { 1, 2, 3, 4, 8, 16 },
                new int[] { 1, 2, 4, 8, 16 },
                "Hebat! Faktor 16: 1 → 2 → 4 → 8 → 16.",
                "Seharusnya: 1 → 2 → 4 → 8 → 16."
            ));

            bankSoal.Add(new DataSoalObor(
                "Urutkan faktor dari 9!",
                9,
                new int[] { 1, 2, 3, 6, 9 },
                new int[] { 1, 3, 9 },
                "Benar! Faktor 9: 1 → 3 → 9.",
                "Seharusnya: 1 → 3 → 9."
            ));
        }
        else if (level == 2)
        {
            // --- LEVEL 2: URUTAN FAKTOR (inti konsep baru) ---
            bankSoal.Add(new DataSoalObor(
                "Nyalakan obor faktor dari 4 berurutan dari kecil ke besar!",
                4,
                new int[] { 1, 2, 3, 4 },
                new int[] { 1, 2, 4 },
                "Benar! Faktor 4: 1 → 2 → 4. (3 bukan faktor)",
                "Salah. Urutan benar: 1 → 2 → 4."
            ));

            bankSoal.Add(new DataSoalObor(
                "Nyalakan obor faktor dari 6 berurutan dari kecil ke besar!",
                6,
                new int[] { 1, 2, 3, 4, 6 },
                new int[] { 1, 2, 3, 6 },
                "Benar! Faktor 6: 1 → 2 → 3 → 6.",
                "Seharusnya: 1 → 2 → 3 → 6."
            ));

            bankSoal.Add(new DataSoalObor(
                "Nyalakan obor faktor dari 8 berurutan dari kecil ke besar!",
                8,
                new int[] { 1, 2, 3, 4, 8 },
                new int[] { 1, 2, 4, 8 },
                "Bagus! Faktor 8: 1 → 2 → 4 → 8.",
                "Seharusnya: 1 → 2 → 4 → 8."
            ));

            bankSoal.Add(new DataSoalObor(
                "Nyalakan obor faktor dari 12 berurutan dari kecil ke besar!",
                12,
                new int[] { 1, 2, 3, 4, 5, 6, 12 },
                new int[] { 1, 2, 3, 4, 6, 12 },
                "Hebat! Faktor 12: 1 → 2 → 3 → 4 → 6 → 12.",
                "Seharusnya: 1 → 2 → 3 → 4 → 6 → 12."
            ));

            bankSoal.Add(new DataSoalObor(
                "Nyalakan obor faktor dari 18 berurutan dari kecil ke besar!",
                18,
                new int[] { 1, 2, 3, 5, 6, 9, 18 },
                new int[] { 1, 2, 3, 6, 9, 18 },
                "Luar biasa! Faktor 18: 1 → 2 → 3 → 6 → 9 → 18.",
                "Seharusnya: 1 → 2 → 3 → 6 → 9 → 18."
            ));

            bankSoal.Add(new DataSoalObor(
                "Nyalakan obor faktor dari 20 berurutan dari kecil ke besar!",
                20,
                new int[] { 1, 2, 4, 5, 7, 10, 20 },
                new int[] { 1, 2, 4, 5, 10, 20 },
                "Benar! Faktor 20: 1 → 2 → 4 → 5 → 10 → 20.",
                "Seharusnya: 1 → 2 → 4 → 5 → 10 → 20."
            ));

            bankSoal.Add(new DataSoalObor(
                "Nyalakan obor faktor dari 16 berurutan dari kecil ke besar!",
                16,
                new int[] { 1, 2, 3, 4, 8, 16 },
                new int[] { 1, 2, 4, 8, 16 },
                "Tepat! Faktor 16: 1 → 2 → 4 → 8 → 16.",
                "Seharusnya: 1 → 2 → 4 → 8 → 16."
            ));

            bankSoal.Add(new DataSoalObor(
                "Nyalakan obor faktor dari 24 berurutan dari kecil ke besar!",
                24,
                new int[] { 1, 2, 3, 4, 6, 8, 12, 24 },
                new int[] { 1, 2, 3, 4, 6, 8, 12, 24 },
                "Sempurna! Semua faktor 24 berurutan dengan benar.",
                "Seharusnya: 1 → 2 → 3 → 4 → 6 → 8 → 12 → 24."
            ));
        }
        else if (level == 3)
        {
            // Level 3: urutan faktor negatif (contoh sederhana)
            bankSoal.Add(new DataSoalObor(
                "Urutkan faktor negatif dari 12 (dari paling kecil/negatif ke mendekati nol)!",
                12,
                new int[] { -6, -4, -3, -2, -1, 1 },
                new int[] { -6, -4, -3, -2, -1 },
                "Benar! Faktor negatif 12 dari kecil ke besar: -6 → -4 → -3 → -2 → -1.",
                "Seharusnya: -6 → -4 → -3 → -2 → -1."
            ));

            bankSoal.Add(new DataSoalObor(
                "Urutkan faktor negatif dari 10!",
                10,
                new int[] { -10, -5, -2, -1, 2 },
                new int[] { -10, -5, -2, -1 },
                "Benar! -10 → -5 → -2 → -1.",
                "Seharusnya: -10 → -5 → -2 → -1."
            ));

            bankSoal.Add(new DataSoalObor(
                "Urutkan faktor negatif dari 8!",
                8,
                new int[] { -8, -4, -2, -1, 1, 2 },
                new int[] { -8, -4, -2, -1 },
                "Bagus! -8 → -4 → -2 → -1.",
                "Seharusnya: -8 → -4 → -2 → -1."
            ));

            bankSoal.Add(new DataSoalObor(
                "Urutkan faktor negatif dari 18!",
                18,
                new int[] { -9, -6, -3, -2, -1, 3 },
                new int[] { -9, -6, -3, -2, -1 },
                "Hebat! -9 → -6 → -3 → -2 → -1.",
                "Seharusnya: -9 → -6 → -3 → -2 → -1."
            ));

            bankSoal.Add(new DataSoalObor(
                "Urutkan faktor negatif dari 16!",
                16,
                new int[] { -8, -4, -2, -1, 4 },
                new int[] { -8, -4, -2, -1 },
                "Sempurna! -8 → -4 → -2 → -1.",
                "Seharusnya: -8 → -4 → -2 → -1."
            ));
        }

        return bankSoal;
    }
}