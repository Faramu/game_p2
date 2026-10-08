using System.Collections.Generic;
using UnityEngine;

public static class SoalPasarDatabase
{
    public enum TagGambar { Apel, Pisang, Jeruk, BuahBusuk, Koin, Kios, Box, Lainnya }

    [System.Serializable]
    public class SoalPasar
    {
        [TextArea(2, 3)] public string templateNarasi;
        public string templateOpsiAtas;
        public string templateOpsiBawah;
        public bool apakahOpsiAtasBenar;
        [TextArea(2, 3)] public string hintJikaSalah;
        public TagGambar tag;
    }

    public static SoalPasar BuatSoalContoh()
    {
        return new SoalPasar
        {
            templateNarasi = "Membeli 5 kg buah apel segar untuk stok toko",
            templateOpsiAtas = "+5",
            templateOpsiBawah = "-5",
            apakahOpsiAtasBenar = true,
            hintJikaSalah = "Membeli barang dagangan berarti menambah stok di toko (Positif).",
            tag = TagGambar.Apel
        };
    }

    public static List<SoalPasar> MuatSoalBerdasarkanLevel(int level)
    {
        List<SoalPasar> daftar = new List<SoalPasar>();

        switch (level)
        {
            case 1: MuatSoalLevel1(daftar); break;
            case 2: MuatSoalLevel2(daftar); break;
            case 3: MuatSoalLevel3(daftar); break;
            default: MuatSoalLevel1(daftar); break;
        }

        return daftar;
    }

    static void Tambah(List<SoalPasar> list, string narasi, string opsiAtas, string opsiBawah, bool atasBenar, string hint, TagGambar tag)
    {
        list.Add(new SoalPasar
        {
            templateNarasi = narasi,
            templateOpsiAtas = opsiAtas,
            templateOpsiBawah = opsiBawah,
            apakahOpsiAtasBenar = atasBenar,
            hintJikaSalah = hint,
            tag = tag
        });
    }

    // ====================== LEVEL 1 ======================
    static void MuatSoalLevel1(List<SoalPasar> daftar)
    {
        Tambah(daftar, "Membeli 5 kg apel segar untuk stok toko", "+5", "-5", true, "Membeli barang berarti menambah stok (+).", TagGambar.Apel);
        Tambah(daftar, "Menjual 8 kg jeruk kepada pelanggan", "+8", "-8", false, "Menjual berarti mengurangi stok dagangan (-).", TagGambar.Jeruk);
        Tambah(daftar, "Mendapat keuntungan dagang Rp 15.000", "+15.000", "-15.000", true, "Keuntungan menambah uang modal (+).", TagGambar.Koin);
        Tambah(daftar, "Mengalami kerugian karena buah busuk Rp 20.000", "+20.000", "-20.000", false, "Kerugian mengurangi uang dagang (-).", TagGambar.BuahBusuk);
        Tambah(daftar, "Menerima kiriman 10 box apel manis", "+10", "-10", true, "Menerima kiriman barang menambah stok (+).", TagGambar.Box);
        Tambah(daftar, "Membuang 4 kg pisang yang hancur", "+4", "-4", false, "Membuang barang mengurangi stok toko (-).", TagGambar.BuahBusuk);
        Tambah(daftar, "Mendapat bonus kemitraan 6 kotak jeruk", "+6", "-6", true, "Bonus adalah penambahan barang gratis (+).", TagGambar.Box);
        Tambah(daftar, "Membayar retribusi kebersihan Kios Rp 5.000", "+5.000", "-5.000", false, "Membayar iuran mengurangi uang kas (-).", TagGambar.Kios);
        Tambah(daftar, "Menemukan uang tercecer Rp 10.000 di area pasar", "+10.000", "-10.000", true, "Menemukan rezeki menambah uang kas (+).", TagGambar.Koin);
        Tambah(daftar, "Membayar upah kurir angkut Rp 25.000", "+25.000", "-25.000", false, "Upah adalah biaya pengeluaran (-).", TagGambar.Lainnya);
        Tambah(daftar, "Memanen 12 keranjang apel dari kebun sendiri", "+12", "-12", true, "Hasil panen menambah stok dagangan (+).", TagGambar.Apel);
        Tambah(daftar, "Timbangan rusak menyebabkan rugi Rp 30.000", "+30.000", "-30.000", false, "Kerusakan alat menambah kerugian (-).", TagGambar.Lainnya);
        Tambah(daftar, "Ada 7 pembeli baru mendaftar member Kios", "+7", "-7", true, "Bertambahnya pelanggan bernilai positif (+).", TagGambar.Kios);
        Tambah(daftar, "Mengembalikan 3 dus pisang busuk ke pabrik", "+3", "-3", false, "Mengembalikan barang mengurangi stok di gudang (-).", TagGambar.BuahBusuk);
        Tambah(daftar, "Menerima subsidi modal dari desa Rp 50.000", "+50.000", "-50.000", true, "Subsidi modal menambah saldo kas (+).", TagGambar.Koin);
        Tambah(daftar, "Terkena denda keterlambatan buka Kios Rp 15.000", "+15.000", "-15.000", false, "Denda mengurangi uang pendapatan (-).", TagGambar.Kios);
        Tambah(daftar, "Stok pisang matang bertambah 9 kg", "+9", "-9", true, "Penambahan stok barang adalah positif (+).", TagGambar.Pisang);
        Tambah(daftar, "Stok jeruk sunkist ludes terjual 11 kg", "+11", "-11", false, "Stok yang terjual dan berkurang bernilai negatif (-).", TagGambar.Jeruk);
        Tambah(daftar, "Memperoleh hadiah lomba Kios paling bersih Rp 100.000", "+100.000", "-100.000", true, "Hadiah perlombaan menambah uang kas (+).", TagGambar.Kios);
        Tambah(daftar, "Kehilangan 5 kg apel karena dicuri", "+5", "-5", false, "Kehilangan barang dagangan bernilai negatif (-).", TagGambar.Apel);
    }

    // ====================== LEVEL 2 ======================
    static void MuatSoalLevel2(List<SoalPasar> daftar)
    {
        Tambah(daftar, "Menerima kiriman 10 kg apel busuk dari tengkulak", "+(+10) = +10", "+(-10) = -10", false, "Menerima (+) barang busuk (-) menghasilkan nilai negatif.", TagGambar.BuahBusuk);
        Tambah(daftar, "Membuang 10 kg pisang busuk ke tempat sampah", "-(-10) = +10", "-(+10) = -10", true, "Membuang (-) barang busuk (-) sama dengan menyelamatkan toko (+).", TagGambar.BuahBusuk);
        Tambah(daftar, "Membatalkan pesanan masuk 10 kg jeruk segar", "-(-10) = +10", "-(+10) = -10", false, "Membatalkan (-) barang bagus (+) mengurangi prospek untung (-).", TagGambar.Jeruk);
        Tambah(daftar, "Utang toko sebesar Rp 10.000 dibebaskan oleh bank", "-(-10.000) = +10.000", "+(-10.000) = -10.000", true, "Menghapus (-) sebuah utang (-) adalah sebuah keuntungan (+).", TagGambar.Koin);
        Tambah(daftar, "Menambahkan catatan utang baru Rp 10.000 ke buku kas", "-(+10.000) = -10.000", "+(-10.000) = -10.000", false, "Menambah (+) utang (-) membuat kas bernilai negatif.", TagGambar.Koin);
        Tambah(daftar, "Menarik kembali 10 apel cacat dari tangan pembeli", "-(-10) = +10", "+(-10) = -10", true, "Mengambil/menarik (-) barang jelek (-) memulihkan reputasi (+).", TagGambar.BuahBusuk);
        Tambah(daftar, "Memasukkan 10 keranjang buah berulat ke gudang Kios", "+(+10) = +10", "+(-10) = -10", false, "Menaruh (+) buah berulat (-) merugikan stok gudang (-).", TagGambar.Kios);
        Tambah(daftar, "Menolak pembayaran denda keterlambatan Kios Rp 10.000", "-(-10.000) = +10.000", "-(+10.000) = -10.000", true, "Menolak/batal (-) membayar denda (-) menyelamatkan uang kas (+).", TagGambar.Kios);
        Tambah(daftar, "Memberikan diskon rugi Rp 10.000 kepada pembeli", "-(+10.000) = -10.000", "+(-10.000) = -10.000", false, "Memberikan (+) potongan harga/rugi (-) mengurangi pendapatan (-).", TagGambar.Koin);
        Tambah(daftar, "Mencegah kerugian buah busuk senilai Rp 10.000", "-(-10.000) = +10.000", "+(+10.000) = +10.000", true, "Mencegah (-) kerugian (-) bernilai positif bagi toko (+).", TagGambar.BuahBusuk);
        Tambah(daftar, "Menerima tagihan listrik Kios sebesar Rp 10.000", "-(+10.000) = -10.000", "+(-10.000) = -10.000", false, "Menerima (+) tagihan pengeluaran (-) menambah beban kas (-).", TagGambar.Kios);
        Tambah(daftar, "Membatalkan denda kebersihan pasar Rp 10.000", "-(-10.000) = +10.000", "+(-10.000) = -10.000", true, "Pembatalan (-) denda (-) mengembalikan saldo kas (+).", TagGambar.Koin);
        Tambah(daftar, "Menambah pengeluaran darurat Kios Rp 10.000", "-(-10.000) = +10.000", "+(-10.000) = -10.000", false, "Menambah (+) pengeluaran (-) mengurangi laba bersih (-).", TagGambar.Kios);
        Tambah(daftar, "Menghapus 10 keranjang jeruk asam dari menu display", "-(-10) = +10", "-(+10) = -10", true, "Menghapus (-) produk jelek (-) meningkatkan kualitas toko (+).", TagGambar.Jeruk);
        Tambah(daftar, "Menanggung kerugian mitra dagang sebesar Rp 10.000", "+(+10.000) = +10.000", "+(-10.000) = -10.000", false, "Menanggung (+) kerugian (-) mengurangi uang modalmu (-).", TagGambar.Koin);
        Tambah(daftar, "Mengabaikan 10 Box pesanan fiktif yang merugikan", "-(-10) = +10", "+(-10) = -10", true, "Menghindari (-) hal merugikan (-) menyelamatkan usaha (+).", TagGambar.Box);
        Tambah(daftar, "Mencatat penurunan nilai buah pisang sebesar Rp 10.000", "-(+10.000) = -10.000", "+(-10.000) = -10.000", false, "Mencatat (+) penurunan nilai (-) adalah kerugian (-).", TagGambar.Pisang);
        Tambah(daftar, "Membebaskan mitra dari denda keterlambatan Rp 10.000", "-(-10.000) = +10.000", "+(+10.000) = +10.000", true, "Menghapus (-) denda mitra (-) mempererat kerjasama (+).", TagGambar.Koin);
        Tambah(daftar, "Mengambil alih 10 peti jeruk rusak dari cabang toko", "-(-10) = +10", "+(-10) = -10", false, "Mengambil (+) barang rusak (-) menambah beban toko (-).", TagGambar.Box);
        Tambah(daftar, "Membuang catatan tagihan palsu senilai Rp 10.000", "-(-10.000) = +10.000", "+(+10.000) = -10.000", true, "Membuang (-) tagihan palsu (-) mengamankan keuangan (+).", TagGambar.Lainnya);
    }

    // ====================== LEVEL 3 ======================
    static void MuatSoalLevel3(List<SoalPasar> daftar)
    {
        Tambah(daftar, "Punya modal Rp 10.000 lalu rugi Rp 5.000", "10.000 - 5.000 = +5.000", "10.000 - 5.000 = -5.000", true, "Modal 10 dikurangi rugi 5 menyisakan saldo positif (+5).", TagGambar.Koin);
        Tambah(daftar, "Kios rugi Rp 5.000 lalu rugi lagi Rp 5.000", "(-5.000) - 5.000 = 0", "(-5.000) - 5.000 = -10.000", false, "Rugi bertambah rugi membuat total kerugian semakin besar (-10).", TagGambar.Kios);
        Tambah(daftar, "Toko rugi Rp 10.000 lalu mendapat untung Rp 15.000", "(-10.000) + 15.000 = +5.000", "(-10.000) + 15.000 = -5.000", true, "Keuntungan (15) lebih besar dari kerugian (10), sisa modal positif (+5).", TagGambar.Koin);
        Tambah(daftar, "Punya uang Rp 5.000 untuk membayar utang Rp 10.000", "5.000 - 10.000 = +5.000", "5.000 - 10.000 = -5.000", false, "Uang pembayaran kurang dari utang, sehingga masih tersisa utang (-5).", TagGambar.Koin);
        Tambah(daftar, "Mendapat untung Rp 10.000 lalu untung lagi Rp 10.000", "(+10.000) + 10.000 = +20.000", "(+10.000) + 10.000 = -20.000", true, "Untung ditambah untung menghasilkan keuntungan lebih besar (+20).", TagGambar.Koin);
        Tambah(daftar, "Suhu gudang Kios -5 derajat C lalu naik 10 derajat C", "(-5) + 10 = -15", "(-5) + 10 = +5", false, "Suhu dingin naik 10 derajat melewati nol menjadi hangat (+5).", TagGambar.Kios);
        Tambah(daftar, "Suhu ruang pendingin 5 derajat C lalu turun 10 derajat C", "5 - 10 = -5", "5 - 10 = +5", true, "Suhu turun 10 derajat dari 5 melewati nol menjadi dingin (-5).", TagGambar.Lainnya);
        Tambah(daftar, "Pedagang di titik -3 lalu maju 5 langkah ke Kios", "(-3) + 5 = -8", "(-3) + 5 = +2", false, "Bergerak ke kanan dari -3 sejauh 5 langkah sampai di titik +2.", TagGambar.Kios);
        Tambah(daftar, "Pedagang di titik 2 lalu mundur 6 langkah ke gudang", "2 - 6 = -4", "2 - 6 = +4", true, "Mundur ke kiri 6 langkah dari 2 melewati nol menuju -4.", TagGambar.Lainnya);
        Tambah(daftar, "Utang Rp 10.000 dibayar lunas Rp 10.000", "(-10.000) + 10.000 = -20.000", "(-10.000) + 10.000 = 0", false, "Utang dibayar dengan jumlah yang sama membuat saldo menjadi nol (0).", TagGambar.Koin);
        Tambah(daftar, "Membeli Box buah Rp 15.000 dengan modal Rp 20.000", "20.000 - 15.000 = +5.000", "20.000 - 15.000 = -5.000", true, "Uang modal tersisa Rp 5.000 (+5).", TagGambar.Box);
        Tambah(daftar, "Ada stok 10 kg apel lalu terjual 8 kg", "10 - 8 = -2", "10 - 8 = +2", false, "Stok awal 10 dikurangi 8 tersisa stok positif (+2).", TagGambar.Apel);
        Tambah(daftar, "Rugi Rp 20.000 lalu mendapat sumbangan Rp 10.000", "(-20.000) + 10.000 = -10.000", "(-20.000) + 10.000 = +10.000", true, "Sumbangan mengurangi beban rugi, tetapi toko masih rugi (-10).", TagGambar.Koin);
        Tambah(daftar, "Punya saldo Rp 0 lalu terkena denda Kios Rp 5.000", "0 - 5.000 = +5.000", "0 - 5.000 = -5.000", false, "Pengeluaran saat saldo nol membuat saldo menjadi minus (-5).", TagGambar.Kios);
        Tambah(daftar, "Maju 4 langkah (+4) lalu mundur 4 langkah (-4)", "4 - 4 = 0", "4 - 4 = -8", true, "Maju dan mundur dengan jarak sama mengembalikan ke titik awal (0).", TagGambar.Lainnya);
        Tambah(daftar, "Stok pisang -2 kg (kurang) lalu ditambah kiriman 5 kg", "(-2) + 5 = -7", "(-2) + 5 = +3", false, "Kekurangan stok tertutupi dan tersisa stok bersih 3 kg (+3).", TagGambar.Pisang);
        Tambah(daftar, "Suhu pasar -2 derajat C lalu turun lagi 3 derajat C", "(-2) - 3 = -5", "(-2) - 3 = +1", true, "Suhu semakin dingin sehingga nilainya makin negatif (-5).", TagGambar.Lainnya);
        Tambah(daftar, "Mendapat bonus Rp 5.000 lalu dipakai bayar parkir Rp 2.000", "5.000 - 2.000 = -3.000", "5.000 - 2.000 = +3.000", false, "Bonus dikurangi ongkos parkir menyisakan uang Rp 3.000 (+3).", TagGambar.Koin);
        Tambah(daftar, "Uang kas Rp 15.000 dipakai bayar tagihan Kios Rp 20.000", "15.000 - 20.000 = -5.000", "15.000 - 20.000 = +5.000", true, "Pembayaran melebihi kas menyebabkan defisit/utang (-5).", TagGambar.Kios);
        Tambah(daftar, "Pedagang melompat dari titik -8 naik 8 langkah ke atas", "(-8) + 8 = -16", "(-8) + 8 = 0", false, "Melompat naik 8 langkah dari -8 tepat mendarat di titik nol (0).", TagGambar.Lainnya);
    }
}