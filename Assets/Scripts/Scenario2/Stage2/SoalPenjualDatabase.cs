using System.Collections.Generic;
using UnityEngine;

public static class SoalPenjualDatabase
{
    [System.Serializable]
    public class SoalCerita
    {
        [TextArea(2, 4)] public string narasiSoal;
        public string notasiAngka;
        public int jawabanBenar;
        [TextArea(2, 3)] public string hintJikaSalah;
    }

    public static List<SoalCerita> MuatSoalBerdasarkanLevel(int level)
    {
        List<SoalCerita> daftar = new List<SoalCerita>();

        switch (level)
        {
            case 1: MuatSoalLevel1(daftar); break;
            case 2: MuatSoalLevel2(daftar); break;
            case 3: MuatSoalLevel3(daftar); break;
            default: MuatSoalLevel1(daftar); break;
        }

        return daftar;
    }

    static void Tambah(List<SoalCerita> list, string narasi, string notasi, int jawaban, string hint)
    {
        list.Add(new SoalCerita
        {
            narasiSoal = narasi,
            notasiAngka = notasi,
            jawabanBenar = jawaban,
            hintJikaSalah = hint
        });
    }

    // ====================== LEVEL 1 ======================
    static void MuatSoalLevel1(List<SoalCerita> daftar)
    {
        Tambah(daftar, "Membeli 10 kg apel segar, lalu terjual 4 kg. Berapa sisa apel di kios?", "10 - 4 = ?", 6, "Hitung: 10 - 4 = 6 kg.");
        Tambah(daftar, "Stok mangga kurang 5 kg, lalu datang pasokan 8 kg. Berapa stok mangga sekarang?", "-5 + 8 = ?", 3, "Hitung: -5 + 8 = 3 kg.");
        Tambah(daftar, "Kios punya 15 kg pisang, lalu membuang 5 kg karena busuk. Sisa pisang?", "15 - 5 = ?", 10, "Hitung: 15 - 5 = 10 kg.");
        Tambah(daftar, "Penjual rugi Rp 10.000, lalu rugi lagi Rp 5.000. Berapa total kerugiannya?", "-10.000 - 5.000 = ?", -15000, "Rugi bertambah rugi menjadi semakin minus: -15.000. (Gunakan tanda minus!)");
        Tambah(daftar, "Ada 20 buah jeruk, lalu mendapat bonus tambahan 5 buah. Total buah jeruk?", "20 + 5 = ?", 25, "Hitung: 20 + 5 = 25 buah.");
        Tambah(daftar, "Kekurangan 8 buah semangka, lalu dipasok 12 buah. Total stok sekarang?", "-8 + 12 = ?", 4, "Hitung: -8 + 12 = 4 buah.");
        Tambah(daftar, "Punya 14 kg salak, laku terjual 10 kg. Sisa salak di keranjang?", "14 - 10 = ?", 4, "Hitung: 14 - 10 = 4 kg.");
        Tambah(daftar, "Utang 15 kg pepaya kepada tengkulak, lalu dibayar lunas 15 kg. Sisa utang?", "-15 + 15 = ?", 0, "Hitung: -15 + 15 = 0 kg.");
        Tambah(daftar, "Rugi 12 kg pir yang rusak, tapi dikembalikan ke supplier 8 kg. Sisa kerugian?", "-12 + 8 = ?", -4, "Hitung: -12 + 8 = -4 kg. (Gunakan tanda minus!)");
        Tambah(daftar, "Stok awal 0, lalu mendapat kiriman 18 kg jambu manis. Berapa stoknya?", "0 + 18 = ?", 18, "Hitung: 0 + 18 = 18 kg.");
        Tambah(daftar, "Sisa 5 kg duku, lalu terjual pesanan 9 kg (berutang stok). Posisi stok sekarang?", "5 - 9 = ?", -4, "Hitung: 5 - 9 = -4 kg. Kamu kekurangan stok. (Gunakan tanda minus!)");
        Tambah(daftar, "Buah naga ada 22 kg, dibeli oleh pelanggan 12 kg. Berapa sisa buah naga?", "22 - 12 = ?", 10, "Hitung: 22 - 12 = 10 kg.");
        Tambah(daftar, "Kios defisit alpukat 20 kg, lalu dipasok 5 kg. Sisa defisit alpukat?", "-20 + 5 = ?", -15, "Hitung: -20 + 5 = -15 kg. (Gunakan tanda minus!)");
        Tambah(daftar, "Punya 30 kg manggis, membuang 10 kg yang kulitnya pecah. Sisa manggis?", "30 - 10 = ?", 20, "Hitung: 30 - 10 = 20 kg.");
        Tambah(daftar, "Toko untung Rp 25.000, dipakai bayar sewa meja Rp 15.000. Sisa keuntungan?", "25.000 - 15.000 = ?", 10000, "Hitung: 25000 - 15000 = 10000.");
    }

    // ====================== LEVEL 2 ======================
    static void MuatSoalLevel2(List<SoalCerita> daftar)
    {
        Tambah(daftar, "Pedagang kehilangan 5 kg apel, menerima pasokan 8 kg, dan mendapat tambahan 5 kg lagi. Berapa total perubahan stok?", "-5 + 8 + 5 = ?", 8, "Hitung berurutan: -5 + 8 + 5 = 8 kg.");
        Tambah(daftar, "Ibu membeli 12 apel, memberikan 7 ke tetangga, dan membeli lagi 5 mangga. Total buah Ibu?", "12 - 7 + 5 = ?", 10, "Hitung berurutan: 12 - 7 + 5 = 10 buah.");
        Tambah(daftar, "Kios rugi 12 kg mangga busuk, membuang 6 kg rusak, lalu menyisihkan 3 kg jelek. Total penyusutan mangga?", "-12 - 6 - 3 = ?", -21, "Penyusutan terus bertambah: -12 - 6 - 3 = -21 kg. (Ketik tanda minus!)");
        Tambah(daftar, "Kios punya 15 kg jeruk, mendapat bonus 5 kg, lalu menjual 10 kg. Sisa jeruk?", "15 + 5 - 10 = ?", 10, "Hitung berurutan: 15 + 5 - 10 = 10 kg.");
        Tambah(daftar, "Toko rugi 8 kg semangka, dipasok 12 kg, dan retur dibatalkan 4 kg (+). Perubahan stok semangka?", "-8 + 12 + 4 = ?", 8, "Hitung berurutan: -8 + 12 + 4 = 8 kg.");
        Tambah(daftar, "Stok melon berkurang 20 buah, berkurang lagi 5 buah, lalu terjual 10 buah. Perubahan persediaan melon?", "-20 - 5 - 10 = ?", -25, "Hitung: -20 - 5 - 10 = -25 buah. (Ketik tanda minus!)");
        Tambah(daftar, "Ani membeli 7 kg pisang, memberikan 9 kg ke panti, lalu mendapat 3 kg. Sisa pisang?", "7 - 9 + 3 = ?", 1, "Hitung berurutan: 7 - 9 + 3 = 1 kg.");
        Tambah(daftar, "Stok anggur susut 15 kg, panen 10 kg, dan sisa susut berkurang 5 kg (+). Total persediaan?", "-15 + 10 + 5 = ?", 0, "Hitung: -15 + 10 + 5 = 0 kg.");
        Tambah(daftar, "Ada 18 kg mangga, terjual 12 kg, lalu pembeli mengembalikan 5 kg. Mangga sekarang?", "18 - 12 + 5 = ?", 11, "Hitung berurutan: 18 - 12 + 5 = 11 kg.");
        Tambah(daftar, "Catatan kekurangan stok apel -6 kg, bertambah kurang -4 kg, diisi kembali 8 kg. Posisi persediaan?", "-6 - 4 + 8 = ?", -2, "Hitung: -6 - 4 + 8 = -2 kg. (Ketik tanda minus!)");
        Tambah(daftar, "Penjual punya 25 kg jeruk, membuang 15 kg busuk, menjual 10 kg. Sisa jeruk?", "25 - 15 - 10 = ?", 0, "Hitung berurutan: 25 - 15 - 10 = 0 kg.");
        Tambah(daftar, "Awalnya 30 kg buah naga, rusak 15 kg, panen lagi 10 kg. Buah naga sekarang?", "30 - 15 + 10 = ?", 25, "Hitung berurutan: 30 - 15 + 10 = 25 kg.");
        Tambah(daftar, "Kios kehilangan 9 kg salak, kehilangan lagi 3 kg, mendapat kiriman 12 kg. Perubahan salak?", "-9 - 3 + 12 = ?", 0, "Hitung: -9 - 3 + 12 = 0 kg.");
        Tambah(daftar, "Ada 50 kg rambutan, dijual 25 kg, dijual lagi 35 kg. Posisi stok rambutan?", "50 - 25 - 35 = ?", -10, "Hitung berurutan: 50 - 25 - 35 = -10 kg.");
        Tambah(daftar, "Stok awal kurang 15 kg, dipasok 25 kg, dipasok lagi 10 kg. Total stok sekarang?", "-15 + 25 + 10 = ?", 20, "Hitung berurutan: -15 + 25 + 10 = 20 kg.");
    }

    // ====================== LEVEL 3 ======================
    static void MuatSoalLevel3(List<SoalCerita> daftar)
    {
        Tambah(daftar, "Kios punya 50 kg apel, laku 30 kg, lalu mendapat kiriman dadakan 40 kg. Berapa total akhir?", "50 - 30 + 40 = ?", 60, "Hitung berurutan: 50 - 30 + 40 = 60 kg.");
        Tambah(daftar, "Kurang 45 kg mangga, lalu dipasok 25 kg, dan rusak digigit hama 10 kg. Berapa sisa stok?", "-45 + 25 - 10 = ?", -30, "Hitung: -45 + 25 - 10 = -30 kg. (Gunakan tanda minus!)");
        Tambah(daftar, "Ada 100 kg pisang, terjual besar 60 kg, dibuang karena busuk 15 kg. Berapa sisa pisang?", "100 - 60 - 15 = ?", 25, "Hitung berurutan: 100 - 60 - 15 = 25 kg.");
        Tambah(daftar, "Utang 80 kg jeruk, dibayar 50 kg, lalu karena kehabisan, berutang lagi 20 kg. Total utang jeruk?", "-80 + 50 - 20 = ?", -50, "Hitung: -80 + 50 - 20 = -50 kg. (Gunakan tanda minus!)");
        Tambah(daftar, "Stok awal 75 kg semangka, rusak di jalan 25 kg, lalu laku terjual 40 kg. Berapa sisa semangka?", "75 - 25 - 40 = ?", 10, "Hitung berurutan: 75 - 25 - 40 = 10 kg.");
        Tambah(daftar, "Defisit melon 60 kg, dipasok dari desa 100 kg, lalu laku dibeli pejabat 30 kg. Total akhir melon?", "-60 + 100 - 30 = ?", 10, "Hitung: -60 + 100 - 30 = 10 kg.");
        Tambah(daftar, "Punya 120 kg salak, retur dari pelanggan 20 kg (+), lalu terpaksa dibuang 40 kg. Sisa salak?", "120 + 20 - 40 = ?", 100, "Hitung berurutan: 120 + 20 - 40 = 100 kg.");
        Tambah(daftar, "Rugi 55 kg pepaya, rugi lagi 15 kg, lalu diganti oleh supplier 40 kg. Sisa kerugian?", "-55 - 15 + 40 = ?", -30, "Hitung berurutan: -55 - 15 + 40 = -30 kg. (Gunakan tanda minus!)");
        Tambah(daftar, "Ada 85 kg pir, terjual 45 kg, lalu datang truk membawa 50 kg. Total pir di Kios?", "85 - 45 + 50 = ?", 90, "Hitung berurutan: 85 - 45 + 50 = 90 kg.");
        Tambah(daftar, "Utang 150 kg jambu air, dipasok 100 kg, lalu ternyata rusak 25 kg. Total persediaan?", "-150 + 100 - 25 = ?", -75, "Hitung: -150 + 100 - 25 = -75 kg. (Gunakan tanda minus!)");
        Tambah(daftar, "Kurang 70 kg duku, kurang lagi pesanan 30 kg, lalu dipasok panen 80 kg. Sisa persediaan?", "-70 - 30 + 80 = ?", -20, "Hitung berurutan: -70 - 30 + 80 = -20 kg. (Gunakan tanda minus!)");
        Tambah(daftar, "Stok 130 kg kelengkeng, laku 80 kg, lalu pembeli membatalkan 20 kg (+). Sisa kelengkeng?", "130 - 80 + 20 = ?", 70, "Hitung berurutan: 130 - 80 + 20 = 70 kg.");
        Tambah(daftar, "Punya 200 kg rambutan, terjual 150 kg, lalu busuk 30 kg. Berapa sisa rambutan siap jual?", "200 - 150 - 30 = ?", 20, "Hitung berurutan: 200 - 150 - 30 = 20 kg.");
        Tambah(daftar, "Kas Kios minus Rp 50.000, dibayar pelanggan Rp 30.000, denda retribusi Rp 10.000. Posisi uang?", "-50000 + 30000 - 10000 = ?", -30000, "Hitung: -50000 + 30000 - 10000 = -30000.");
        Tambah(daftar, "Kios untung Rp 100.000, dipakai modal Rp 60.000, bayar keamanan Rp 20.000. Sisa laba?", "100000 - 60000 - 20000 = ?", 20000, "Hitung: 100000 - 60000 - 20000 = 20000.");
    }
}