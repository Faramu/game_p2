using System.Collections.Generic;

public static class BankSoal_Stage3_Lv1
{
    public static void MuatSoal(Dictionary<int, DataSoalSuhu> database)
    {
        // 🟢 SUB-LEVEL 1.1 (+ × + / + ÷ +)
        TambahSoal(database, 1, "Kali", 3,
            "Dikali 3", 3,
            "Dikali (-3)", -3,
            "Suhu mesin sekarang 4°C. Agar pintu kayu mencair, suhu harus menjadi 12°C.",
            "Suhu 4°C harus dikali berapa agar hasilnya 12°C?",
            "Benar!\n\n4 × 3 = 12°C.\nAngka positif dikali positif hasilnya tetap positif.",
            "Salah!\n\n4 × (-3) = -12°C.\nAngka positif dikali negatif hasilnya menjadi negatif (makin dingin).");

        TambahSoal(database, 2, "Kali", 2,
            "Dikali 2", 2,
            "Dikali 3", 3,
            "Suhu tungku sekarang 5°C. Kita butuh suhu 10°C agar mesin uap bisa bekerja.",
            "Suhu 5°C harus dikali berapa agar menjadi 10°C?",
            "Benar!\n\n5 × 2 = 10°C.",
            "Salah!\n\n5 × 3 = 15°C (terlalu panas).");

        TambahSoal(database, 3, "Kali", 4,
            "Dikali 4", 4,
            "Dikali (-4)", -4,
            "Suhu awal 6°C. Balok baja hanya bisa meleleh jika suhunya mencapai 24°C.",
            "Suhu 6°C harus dikali berapa agar menjadi 24°C?",
            "Benar!\n\n6 × 4 = 24°C.",
            "Salah!\n\n6 × (-4) = -24°C (makin dingin).");

        TambahSoal(database, 4, "Kali", 8,
            "Dikali 8", 8,
            "Dikali (-8)", -8,
            "Kristal memiliki suhu 2°C. Agar bisa menerangi ruangan, suhunya harus menjadi 16°C.",
            "Suhu 2°C harus dikali berapa agar menjadi 16°C?",
            "Benar!\n\n2 × 8 = 16°C.",
            "Salah!\n\n2 × (-8) = -16°C.");

        TambahSoal(database, 5, "Kali", 3,
            "Dikali 3", 3,
            "Dikali (-3)", -3,
            "Mesin air hangat bersuhu 7°C. Desa membutuhkan suhu 21°C.",
            "Suhu 7°C harus dikali berapa agar menjadi 21°C?",
            "Benar!\n\n7 × 3 = 21°C.",
            "Salah!\n\n7 × (-3) = -21°C.");

        TambahSoal(database, 6, "Bagi", 3,
            "Dibagi 3", 3,
            "Dibagi (-3)", -3,
            "Mesin kepanasan di suhu 15°C. Suhu harus diturunkan menjadi 5°C agar aman.",
            "Suhu 15°C harus dibagi berapa agar menjadi 5°C?",
            "Benar!\n\n15 ÷ 3 = 5°C.",
            "Salah!\n\n15 ÷ (-3) = -5°C (makin dingin).");

        TambahSoal(database, 7, "Bagi", 4,
            "Dibagi 4", 4,
            "Dibagi (-4)", -4,
            "Pipa uap bersuhu 20°C. Ruangan membutuhkan suhu nyaman 5°C.",
            "Suhu 20°C harus dibagi berapa agar menjadi 5°C?",
            "Benar!\n\n20 ÷ 4 = 5°C.",
            "Salah!\n\n20 ÷ (-4) = -5°C.");

        TambahSoal(database, 8, "Bagi", 6,
            "Dibagi 6", 6,
            "Dibagi (-6)", -6,
            "Suhu lava adalah 24°C. Pandai besi membutuhkan suhu 4°C untuk membuat pedang.",
            "Suhu 24°C harus dibagi berapa agar menjadi 4°C?",
            "Benar!\n\n24 ÷ 6 = 4°C.",
            "Salah!\n\n24 ÷ (-6) = -4°C.");

        TambahSoal(database, 9, "Bagi", 5,
            "Dibagi 5", 5,
            "Dibagi 2", 2,
            "Suhu udara 10°C. Kebun botani hanya bisa tumbuh di suhu 2°C.",
            "Suhu 10°C harus dibagi berapa agar menjadi 2°C?",
            "Benar!\n\n10 ÷ 5 = 2°C.",
            "Salah!\n\n10 ÷ 2 = 5°C (masih terlalu panas).");

        TambahSoal(database, 10, "Bagi", 5,
            "Dibagi 5", 5,
            "Dibagi (-5)", -5,
            "Radiasi panas mencapai 30°C. Segera turunkan suhu menjadi 6°C!",
            "Suhu 30°C harus dibagi berapa agar menjadi 6°C?",
            "Benar!\n\n30 ÷ 5 = 6°C.",
            "Salah!\n\n30 ÷ (-5) = -6°C.");

        // 🟡 SUB-LEVEL 1.2 (- × + = -) dan (- ÷ + = -)
        TambahSoal(database, 11, "Kali", 4,
            "Dikali 4", 4,
            "Dikali (-4)", -4,
            "Lantai es bersuhu -3°C. Agar bisa dilewati, suhu harus menjadi -12°C (lebih dingin).",
            "Suhu -3°C harus dikali berapa agar menjadi -12°C?",
            "Benar!\n\n(-3) × 4 = -12°C.\nNegatif dikali positif hasilnya negatif.",
            "Salah!\n\n(-3) × (-4) = 12°C (menjadi panas).");

        TambahSoal(database, 12, "Kali", 3,
            "Dikali 3", 3,
            "Dikali (-3)", -3,
            "Air danau bersuhu -5°C. Agar monster tidak muncul, suhu harus menjadi -15°C.",
            "Suhu -5°C harus dikali berapa agar menjadi -15°C?",
            "Benar!\n\n(-5) × 3 = -15°C.",
            "Salah!\n\n(-5) × (-3) = 15°C.");

        TambahSoal(database, 13, "Kali", 6,
            "Dikali 6", 6,
            "Dikali (-6)", -6,
            "Jebakan bersuhu -2°C. Agar lorong terkunci, suhu harus menjadi -12°C.",
            "Suhu -2°C harus dikali berapa agar menjadi -12°C?",
            "Benar!\n\n(-2) × 6 = -12°C.",
            "Salah!\n\n(-2) × (-6) = 12°C.");

        TambahSoal(database, 14, "Kali", 5,
            "Dikali 5", 5,
            "Dikali 4", 4,
            "Dinding pertahanan bersuhu -4°C. Agar semakin kuat, suhu harus menjadi -20°C.",
            "Suhu -4°C harus dikali berapa agar menjadi -20°C?",
            "Benar!\n\n(-4) × 5 = -20°C.",
            "Salah!\n\n(-4) × 4 = -16°C.");

        TambahSoal(database, 15, "Kali", 2,
            "Dikali 2", 2,
            "Dikali (-2)", -2,
            "Ramuan harus disimpan di suhu -14°C. Suhu sekarang baru -7°C.",
            "Suhu -7°C harus dikali berapa agar menjadi -14°C?",
            "Benar!\n\n(-7) × 2 = -14°C.",
            "Salah!\n\n(-7) × (-2) = 14°C.");

        TambahSoal(database, 16, "Bagi", 4,
            "Dibagi 4", 4,
            "Dibagi (-4)", -4,
            "Ruangan terlalu dingin (-16°C). Ksatria butuh suhu -4°C agar bisa bernapas.",
            "Suhu -16°C harus dibagi berapa agar menjadi -4°C?",
            "Benar!\n\n(-16) ÷ 4 = -4°C.",
            "Salah!\n\n(-16) ÷ (-4) = 4°C (menjadi panas).");

        TambahSoal(database, 17, "Bagi", 5,
            "Dibagi 5", 5,
            "Dibagi (-5)", -5,
            "Badai salju bersuhu -25°C. Turunkan intensitasnya menjadi -5°C.",
            "Suhu -25°C harus dibagi berapa agar menjadi -5°C?",
            "Benar!\n\n(-25) ÷ 5 = -5°C.",
            "Salah!\n\n(-25) ÷ (-5) = 5°C.");

        TambahSoal(database, 18, "Bagi", 3,
            "Dibagi 3", 3,
            "Dibagi (-3)", -3,
            "Batu es besar bersuhu -18°C. Kurangi kebekuannya menjadi -6°C agar bisa dipecah.",
            "Suhu -18°C harus dibagi berapa agar menjadi -6°C?",
            "Benar!\n\n(-18) ÷ 3 = -6°C.",
            "Salah!\n\n(-18) ÷ (-3) = 6°C.");

        TambahSoal(database, 19, "Bagi", 6,
            "Dibagi 6", 6,
            "Dibagi 5", 5,
            "Kutukan es bersuhu -30°C. Pecahkan kutukan tersebut menjadi -5°C.",
            "Suhu -30°C harus dibagi berapa agar menjadi -5°C?",
            "Benar!\n\n(-30) ÷ 6 = -5°C.",
            "Salah!\n\n(-30) ÷ 5 = -6°C.");

        TambahSoal(database, 20, "Bagi", 2,
            "Dibagi 2", 2,
            "Dibagi (-2)", -2,
            "Gembok es bersuhu -12°C. Agar terbuka, suhu harus menjadi -6°C.",
            "Suhu -12°C harus dibagi berapa agar menjadi -6°C?",
            "Benar!\n\n(-12) ÷ 2 = -6°C.",
            "Salah!\n\n(-12) ÷ (-2) = 6°C.");

        // 🔴 SUB-LEVEL 1.3 (- × - = +) dan (+ ÷ - = -) / (- ÷ - = +)
        TambahSoal(database, 21, "Kali", -3,
            "Dikali (-3)", -3,
            "Dikali 3", 3,
            "Gunung es bersuhu -4°C. Hanya bisa hancur jika suhunya menjadi 12°C (panas).",
            "Suhu -4°C harus dikali berapa agar menjadi 12°C?",
            "Benar!\n\n(-4) × (-3) = 12°C.\nNegatif dikali negatif hasilnya positif.",
            "Salah!\n\n(-4) × 3 = -12°C (makin dingin).");

        TambahSoal(database, 22, "Kali", -4,
            "Dikali (-4)", -4,
            "Dikali 4", 4,
            "Pohon raksasa hampir mati di suhu -5°C. Berikan panas 20°C untuk menghidupkannya.",
            "Suhu -5°C harus dikali berapa agar menjadi 20°C?",
            "Benar!\n\n(-5) × (-4) = 20°C.",
            "Salah!\n\n(-5) × 4 = -20°C.");

        TambahSoal(database, 23, "Kali", -2,
            "Dikali (-2)", -2,
            "Dikali 2", 2,
            "Ruang mesin membeku di -6°C. Ubah menjadi hangat 12°C.",
            "Suhu -6°C harus dikali berapa agar menjadi 12°C?",
            "Benar!\n\n(-6) × (-2) = 12°C.",
            "Salah!\n\n(-6) × 2 = -12°C.");

        TambahSoal(database, 24, "Kali", -3,
            "Dikali (-3)", -3,
            "Dikali 3", 3,
            "Kristal gelap memancarkan suhu -8°C. Ubah menjadi cahaya dengan suhu 24°C.",
            "Suhu -8°C harus dikali berapa agar menjadi 24°C?",
            "Benar!\n\n(-8) × (-3) = 24°C.",
            "Salah!\n\n(-8) × 3 = -24°C.");

        TambahSoal(database, 25, "Kali", -5,
            "Dikali (-5)", -5,
            "Dikali (-4)", -4,
            "Gerbang raja bersuhu -7°C. Harus dilelehkan di suhu 35°C.",
            "Suhu -7°C harus dikali berapa agar menjadi 35°C?",
            "Benar!\n\n(-7) × (-5) = 35°C.",
            "Salah!\n\n(-7) × (-4) = 28°C.");

        TambahSoal(database, 26, "Bagi", -4,
            "Dibagi (-4)", -4,
            "Dibagi 4", 4,
            "Jebakan api bersuhu 20°C. Ubah menjadi es beku -5°C agar bisa dilewati.",
            "Suhu 20°C harus dibagi berapa agar menjadi -5°C?",
            "Benar!\n\n20 ÷ (-4) = -5°C.\nPositif dibagi negatif hasilnya negatif.",
            "Salah!\n\n20 ÷ 4 = 5°C.");

        TambahSoal(database, 27, "Bagi", -3,
            "Dibagi (-3)", -3,
            "Dibagi (-5)", -5,
            "Reaktor kelebihan muatan di 15°C. Bekukan menjadi -5°C.",
            "Suhu 15°C harus dibagi berapa agar menjadi -5°C?",
            "Benar!\n\n15 ÷ (-3) = -5°C.",
            "Salah!\n\n15 ÷ (-5) = -3°C.");

        TambahSoal(database, 28, "Bagi", -6,
            "Dibagi (-6)", -6,
            "Dibagi 6", 6,
            "Lahar mengalir di suhu 24°C. Ubah menjadi pijakan es di suhu -4°C.",
            "Suhu 24°C harus dibagi berapa agar menjadi -4°C?",
            "Benar!\n\n24 ÷ (-6) = -4°C.",
            "Salah!\n\n24 ÷ 6 = 4°C.");

        TambahSoal(database, 29, "Bagi", -2,
            "Dibagi (-2)", -2,
            "Dibagi 2", 2,
            "Sihir membekukan ruangan di -18°C. Ubah kembali menjadi hangat 9°C.",
            "Suhu -18°C harus dibagi berapa agar menjadi 9°C?",
            "Benar!\n\n(-18) ÷ (-2) = 9°C.\nNegatif dibagi negatif hasilnya positif.",
            "Salah!\n\n(-18) ÷ 2 = -9°C.");

        TambahSoal(database, 30, "Bagi", -5,
            "Dibagi (-5)", -5,
            "Dibagi (-6)", -6,
            "Monster salju terbentuk dari suhu -30°C. Lemahkan dengan mengubah suhunya menjadi 6°C.",
            "Suhu -30°C harus dibagi berapa agar menjadi 6°C?",
            "Benar!\n\n(-30) ÷ (-5) = 6°C.",
            "Salah!\n\n(-30) ÷ (-6) = 5°C.");
    }

    private static void TambahSoal(Dictionary<int, DataSoalSuhu> db, int id, string op, int jwb,
        string tA, int vA, string tB, int vB,
        string nar, string tny, string pBenar, string pSalah)
    {
        db.Add(id, new DataSoalSuhu
        {
            idSoal = id,
            jenisOperasi = op,
            jawabanBenar = jwb,
            teksTombolA = tA,
            nilaiA = vA,
            teksTombolB = tB,
            nilaiB = vB,
            narasi = nar,
            pertanyaan = tny,
            penjelasanBenar = pBenar,
            penjelasanSalah = pSalah
        });
    }
}