using System.Collections.Generic;
using UnityEngine;

public static class GoaFaktorDatabase
{
    // =========================================================
    // DIALOG (Lebih jelas & edukatif)
    // =========================================================
    public static string[] DapatkanDialog(int subLevel)
    {
        switch (subLevel)
        {
            case 1:
                return new string[]
                {
                    "Ksatria, selamat datang di Goa Faktor!\nDinding di depan hanya bisa dihancurkan oleh energi FAKTOR.",
                    "Apa itu FAKTOR?\nFaktor adalah bilangan yang dapat membagi habis suatu bilangan tanpa sisa.",
                    "Contoh: Faktor dari 12 adalah 1, 2, 3, 4, 6, dan 12. Karena 12 ÷ 3 = 4 (tidak bersisa).",
                    "Cara bermain:\n1. Lihat angka di dinding (itu targetmu).",
                    "2. Pilih SEMUA batu yang merupakan faktor dari angka tersebut.",
                    "3. Tekan tombol KONFIRMASI.\nHati-hati! Memilih angka yang BUKAN faktor akan membuatmu kehilangan nyawa."
                };

            case 2:
                return new string[]
                {
                    "Bagus! Kamu sudah masuk lebih dalam.\nDi area ini, kamu hanya boleh memilih FAKTOR PRIMA.",
                    "Apa itu bilangan PRIMA?\nBilangan prima adalah bilangan yang HANYA memiliki dua faktor: 1 dan dirinya sendiri.",
                    "Contoh bilangan prima: 2, 3, 5, 7, 11, 13...\nCatatan penting: Angka 1 BUKAN bilangan prima.",
                    "Contoh:\nFaktor dari 30 = 1, 2, 3, 5, 6, 10, 15, 30\nFaktor PRIMAnya hanya: 2, 3, dan 5.",
                    "Jadi di level ini, pilih HANYA yang prima, lalu tekan Konfirmasi."
                };

            case 3:
                return new string[]
                {
                    "Ini area terdalam Goa Faktor.\nSekarang kamu harus memilih faktor POSITIF dan NEGATIF.",
                    "Tahukah kamu?\nSetiap faktor positif selalu punya pasangan negatifnya.",
                    "Contoh: Karena 3 adalah faktor dari 12 (12 ÷ 3 = 4),\nmaka -3 juga faktor dari 12 (12 ÷ (-3) = -4).",
                    "Jadi faktor lengkap dari 12 adalah:\n±1, ±2, ±3, ±4, ±6, ±12",
                    "Di level ini, pilih SEMUA faktor (positif dan negatif), lalu tekan Konfirmasi. Jangan sampai ada yang terlewat!"
                };

            default:
                return new string[]
                {
                    "Selamat datang di Goa Faktor. Pilih semua faktor yang benar, lalu tekan Konfirmasi."
                };
        }
    }

    // =========================================================
    // SOAL
    // =========================================================
    public static List<DataSoalFaktor> DapatkanSoal(int subLevel)
    {
        List<DataSoalFaktor> bank = new List<DataSoalFaktor>();

        // =====================================================
        // LEVEL 1 — FAKTOR POSITIF BIASA
        // =====================================================
        if (subLevel == 1)
        {
            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor dari 12, lalu tekan Konfirmasi!",
                angkaTarget = 12,
                semuaPilihan = new int[] { 1, 2, 3, 4, 5, 6, 7, 12 },
                faktorBenar = new int[] { 1, 2, 3, 4, 6, 12 },
                feedbackBenar = "Benar! Faktor dari 12 adalah 1, 2, 3, 4, 6, dan 12.\n(5 dan 7 bukan faktor karena 12 tidak habis dibagi keduanya.)",
                feedbackSalah = "Kurang tepat.\nFaktor 12: 1, 2, 3, 4, 6, 12.\nJangan pilih angka yang tidak membagi habis 12."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor dari 18!",
                angkaTarget = 18,
                semuaPilihan = new int[] { 1, 2, 3, 5, 6, 9, 11, 18 },
                faktorBenar = new int[] { 1, 2, 3, 6, 9, 18 },
                feedbackBenar = "Tepat! Faktor 18: 1, 2, 3, 6, 9, 18.",
                feedbackSalah = "Masih ada yang kurang atau ada angka yang bukan faktor 18.\nFaktor benar: 1, 2, 3, 6, 9, 18."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor dari 20!",
                angkaTarget = 20,
                semuaPilihan = new int[] { 1, 2, 4, 5, 7, 10, 15, 20 },
                faktorBenar = new int[] { 1, 2, 4, 5, 10, 20 },
                feedbackBenar = "Hebat! Faktor 20: 1, 2, 4, 5, 10, 20.",
                feedbackSalah = "Cek lagi. Faktor 20: 1, 2, 4, 5, 10, 20.\n7 dan 15 bukan faktor."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor dari 16!",
                angkaTarget = 16,
                semuaPilihan = new int[] { 1, 2, 3, 4, 6, 8, 12, 16 },
                faktorBenar = new int[] { 1, 2, 4, 8, 16 },
                feedbackBenar = "Bagus! Faktor 16: 1, 2, 4, 8, 16.",
                feedbackSalah = "Belum tepat. Faktor 16 hanya: 1, 2, 4, 8, 16."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor dari 24!",
                angkaTarget = 24,
                semuaPilihan = new int[] { 1, 2, 3, 4, 6, 7, 8, 12, 24 },
                faktorBenar = new int[] { 1, 2, 3, 4, 6, 8, 12, 24 },
                feedbackBenar = "Luar biasa! Semua faktor 24 berhasil dikumpulkan.",
                feedbackSalah = "Masih ada yang kurang atau salah.\nFaktor 24: 1, 2, 3, 4, 6, 8, 12, 24."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor dari 15!",
                angkaTarget = 15,
                semuaPilihan = new int[] { 1, 2, 3, 5, 6, 10, 15 },
                faktorBenar = new int[] { 1, 3, 5, 15 },
                feedbackBenar = "Benar! Faktor 15: 1, 3, 5, 15.",
                feedbackSalah = "15 hanya habis dibagi 1, 3, 5, dan 15."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor dari 10!",
                angkaTarget = 10,
                semuaPilihan = new int[] { 1, 2, 3, 4, 5, 8, 10 },
                faktorBenar = new int[] { 1, 2, 5, 10 },
                feedbackBenar = "Tepat! Faktor 10: 1, 2, 5, 10.",
                feedbackSalah = "Faktor 10 hanya: 1, 2, 5, 10."
            });
        }

        // =====================================================
        // LEVEL 2 — HANYA FAKTOR PRIMA
        // =====================================================
        else if (subLevel == 2)
        {
            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih HANYA faktor PRIMA dari 30!",
                angkaTarget = 30,
                semuaPilihan = new int[] { 1, 2, 3, 5, 6, 10, 15, 30 },
                faktorBenar = new int[] { 2, 3, 5 },
                feedbackBenar = "Benar! Faktor prima dari 30 adalah 2, 3, dan 5.\n(1 bukan prima. 6, 10, 15, 30 bukan prima.)",
                feedbackSalah = "Ingat: bilangan prima hanya habis dibagi 1 dan dirinya sendiri.\nFaktor prima 30: 2, 3, 5. Angka 1 BUKAN prima."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih HANYA faktor PRIMA dari 42!",
                angkaTarget = 42,
                semuaPilihan = new int[] { 1, 2, 3, 6, 7, 14, 21, 42 },
                faktorBenar = new int[] { 2, 3, 7 },
                feedbackBenar = "Tepat! Faktor prima 42: 2, 3, 7.",
                feedbackSalah = "6, 14, 21, 42 bukan prima. 1 juga bukan prima.\nJawaban benar: 2, 3, 7."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih HANYA faktor PRIMA dari 28!",
                angkaTarget = 28,
                semuaPilihan = new int[] { 1, 2, 4, 7, 14, 28 },
                faktorBenar = new int[] { 2, 7 },
                feedbackBenar = "Bagus! Faktor prima 28: 2 dan 7.",
                feedbackSalah = "Faktor prima 28 hanya 2 dan 7.\n4, 14, 28, dan 1 bukan prima."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih HANYA faktor PRIMA dari 45!",
                angkaTarget = 45,
                semuaPilihan = new int[] { 1, 3, 5, 9, 15, 45 },
                faktorBenar = new int[] { 3, 5 },
                feedbackBenar = "Benar! Faktor prima 45: 3 dan 5.",
                feedbackSalah = "9, 15, 45, dan 1 bukan prima.\nJawaban benar: 3 dan 5."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih HANYA faktor PRIMA dari 60!",
                angkaTarget = 60,
                semuaPilihan = new int[] { 1, 2, 3, 4, 5, 6, 10, 15, 30, 60 },
                faktorBenar = new int[] { 2, 3, 5 },
                feedbackBenar = "Hebat! Faktor prima 60: 2, 3, 5.",
                feedbackSalah = "Hanya 2, 3, dan 5 yang prima. Sisanya bukan."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih HANYA faktor PRIMA dari 66!",
                angkaTarget = 66,
                semuaPilihan = new int[] { 1, 2, 3, 6, 11, 22, 33, 66 },
                faktorBenar = new int[] { 2, 3, 11 },
                feedbackBenar = "Luar biasa! Faktor prima 66: 2, 3, 11.",
                feedbackSalah = "Jawaban benar: 2, 3, 11. Yang lain bukan prima."
            });
        }

        // =====================================================
        // LEVEL 3 — FAKTOR POSITIF + NEGATIF
        // =====================================================
        else if (subLevel == 3)
        {
            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor (positif & negatif) dari 12!",
                angkaTarget = 12,
                semuaPilihan = new int[] { -6, -4, -3, -2, -1, 1, 2, 3, 4, 6 },
                faktorBenar = new int[] { -6, -4, -3, -2, -1, 1, 2, 3, 4, 6 },
                feedbackBenar = "Hebat! Faktor 12 meliputi positif dan negatif:\n±1, ±2, ±3, ±4, ±6.\n(12 juga faktor, tapi tidak dimunculkan di pilihan ini.)",
                feedbackSalah = "Setiap faktor positif punya pasangan negatif.\nPastikan ±1, ±2, ±3, ±4, ±6 semuanya terpilih."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor (positif & negatif) dari 18!",
                angkaTarget = 18,
                semuaPilihan = new int[] { -9, -6, -3, -2, -1, 1, 2, 3, 6, 9 },
                faktorBenar = new int[] { -9, -6, -3, -2, -1, 1, 2, 3, 6, 9 },
                feedbackBenar = "Benar! Faktor 18: ±1, ±2, ±3, ±6, ±9.",
                feedbackSalah = "Jangan lewatkan pasangan negatifnya.\nFaktor lengkap: ±1, ±2, ±3, ±6, ±9."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor (positif & negatif) dari 10!",
                angkaTarget = 10,
                semuaPilihan = new int[] { -10, -5, -2, -1, 1, 2, 5, 10 },
                faktorBenar = new int[] { -10, -5, -2, -1, 1, 2, 5, 10 },
                feedbackBenar = "Tepat! Faktor 10: ±1, ±2, ±5, ±10.",
                feedbackSalah = "Faktor 10 lengkap: ±1, ±2, ±5, ±10."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor (positif & negatif) dari 16!",
                angkaTarget = 16,
                semuaPilihan = new int[] { -8, -4, -2, -1, 1, 2, 4, 8 },
                faktorBenar = new int[] { -8, -4, -2, -1, 1, 2, 4, 8 },
                feedbackBenar = "Bagus! Faktor 16: ±1, ±2, ±4, ±8.",
                feedbackSalah = "Pastikan pasangan positif dan negatifnya lengkap."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor (positif & negatif) dari 20!",
                angkaTarget = 20,
                semuaPilihan = new int[] { -10, -5, -4, -2, -1, 1, 2, 4, 5, 10 },
                faktorBenar = new int[] { -10, -5, -4, -2, -1, 1, 2, 4, 5, 10 },
                feedbackBenar = "Luar biasa! Faktor 20: ±1, ±2, ±4, ±5, ±10.",
                feedbackSalah = "Masih ada yang kurang. Cek pasangan negatif dan positifnya."
            });

            bank.Add(new DataSoalFaktor
            {
                instruksiTeks = "Pilih SEMUA faktor (positif & negatif) dari 8!",
                angkaTarget = 8,
                semuaPilihan = new int[] { -8, -4, -2, -1, 1, 2, 4, 8 },
                faktorBenar = new int[] { -8, -4, -2, -1, 1, 2, 4, 8 },
                feedbackBenar = "Sempurna! Faktor 8: ±1, ±2, ±4, ±8.",
                feedbackSalah = "Faktor 8: ±1, ±2, ±4, ±8. Jangan ada yang terlewat."
            });
        }

        return bank;
    }
}