using System.Collections.Generic;

public static class BankSoal_Stage3_Lv3
{
    public static void MuatSoal(Dictionary<int, DataSoalSuhu> database)
    {
        // 🏰 LEVEL 3: EVALUASI AKHIR (INPUT MANUAL / BOSS)
        // Tombol A dan B dikosongkan ("") karena menggunakan Input Field.
        
        // --- Kumpulan 1-5 ---
        TambahSoal(database, 61, "Campuran", -6, "", 0, "", 0, "Sistem reaktor kritis! Masukkan suhu stabilisasi akhir:", "(-12 × 3) ÷ (-4) - 15 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 62, "Campuran", 12, "", 0, "", 0, "Bahaya overload! Hitung suhu akhir untuk mendinginkan inti:", "(20 ÷ (-5)) × (-2) + 4 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 63, "Campuran", -20, "", 0, "", 0, "Sistem pertahanan suhu meleleh! Kalibrasi ulang ke:", "(-8 + 2) × 5 - (-10) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 64, "Campuran", 8, "", 0, "", 0, "Kode darurat! Ketikkan suhu akhir dari persamaan berikut:", "(-24 ÷ 6) × (-2) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 65, "Campuran", -15, "", 0, "", 0, "Mesin pendingin bermasalah! Hitung suhu pemulihan:", "10 × (-3) ÷ 2 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");

        // --- Kumpulan 6-10 ---
        TambahSoal(database, 66, "Campuran", -13, "", 0, "", 0, "Ancaman anomali es! Masukkan suhu akhir untuk memecahkan es:", "(-15 ÷ 3) + (4 × (-2)) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 67, "Campuran", -19, "", 0, "", 0, "Inti magma tidak stabil! Turunkan suhu dengan memecahkan sandi:", "(8 × (-3)) - (-10 ÷ 2) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 68, "Campuran", 20, "", 0, "", 0, "Tekanan suhu ruang melonjak! Kalibrasi sistem sekarang:", "(-20 + 5) ÷ (-3) × 4 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 69, "Campuran", 25, "", 0, "", 0, "Sistem keamanan mesin terkunci! Masukkan kode suhu yang tepat:", "(-5) × (-4) - (30 ÷ (-6)) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 70, "Campuran", -25, "", 0, "", 0, "Suhu ekstrem terdeteksi! Netralkan mesin pada titik:", "40 ÷ (-4) + (-3 × 5) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");

        // --- Kumpulan 11-15 ---
        TambahSoal(database, 71, "Campuran", -17, "", 0, "", 0, "Peringatan inti beku! Hitung suhu peleleh dari persamaan:", "(-18 ÷ (-2)) × (-3) + 10 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 72, "Campuran", -8, "", 0, "", 0, "Kebocoran uap panas! Cari suhu akhir yang aman:", "(12 - (-8)) ÷ (-5) × 2 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 73, "Campuran", -18, "", 0, "", 0, "Radiasi suhu reaktor bergejolak! Tentukan titik stabilnya:", "(-7 × 3) - (24 ÷ (-8)) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 74, "Campuran", -5, "", 0, "", 0, "Sistem darurat aktif! Berapa target suhu penyelamatan?", "50 ÷ (-5) × 2 - (-15) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 75, "Campuran", 28, "", 0, "", 0, "Sensor pemanas rusak! Masukkan ulang suhu kalibrasi manual:", "(-4) × (15 ÷ (-3)) + 8 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");

        // --- Kumpulan 16-20 ---
        TambahSoal(database, 76, "Campuran", -10, "", 0, "", 0, "Mesin bos melebihi batas batas ambang! Paksa turunkan suhu:", "(-30 ÷ 6) × (-4) - 30 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 77, "Campuran", 6, "", 0, "", 0, "Kegagalan stabilisator tingkat lanjut! Perbaiki pada titik suhu:", "14 + (-6 × 4) ÷ 3 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 78, "Campuran", -5, "", 0, "", 0, "Sistem inti akan meledak! Masukkan kode penjinak bersuhu:", "(-40 ÷ (-8)) × (-5) + 20 = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 79, "Campuran", -9, "", 0, "", 0, "Gelombang api mendekat! Hitung penangkal suhu seketika:", "(9 × (-2)) - (-36 ÷ 4) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
        TambahSoal(database, 80, "Campuran", -20, "", 0, "", 0, "Evaluasi akhir reaktor! Masukkan suhu pamungkas untuk menang:", "(-100 ÷ 10) × 3 - (-10) = ... °C", "JAWABAN TEPAT!\nSuhu berhasil distabilkan!", "JAWABAN SALAH!\nSistem Overload!");
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