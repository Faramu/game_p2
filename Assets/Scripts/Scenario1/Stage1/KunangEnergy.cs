using UnityEngine;

public class KunangEnergy : MonoBehaviour
{
    public enum TipeSimbol { Naik, Turun, Untung, Rugi }
    
    [Header("Set Tipe Dasar Kunang-Kunang")]
    public TipeSimbol tipeKunangKunang;

    [Header("Atur Level Kunang-Kunang ini (1, 2, atau 3)")]
    [Range(1, 3)]
    public int levelStage = 1; 

    [Header("Efek Suara")]
    public AudioClip suaraAmbilKunang;

    private string[][] teksNaik = new string[][] {
        new string[] { "Keren! Kunang-Kunang NAIK.\nArah vertikal ke atas selalu dilambangkan dengan tanda positif (+).", "Contoh: Pesawat terbang naik setinggi 500 meter = +500" },
        new string[] { "Bagus! Kunang-Kunang NAIK.\nPertambahan suhu akibat pemanasan bernilai positif (+).", "Contoh: Suhu ruangan laboratorium naik 4 derajat = +4" },
        new string[] { "Pintar! Kunang-Kunang NAIK.\nBergerak maju ke depan dari titik awal bernilai positif (+).", "Contoh: Ksatria melangkah maju 10 langkah = +10" }
    };

    private string[][] teksTurun = new string[][] {
        new string[] { "Hati-hati! Kunang-Kunang TURUN.\nArah vertikal ke bawah dilambangkan dengan tanda negatif (-).", "Contoh: Suhu dingin malam hari turun 4 derajat = -4" },
        new string[] { "Awas! Kunang-Kunang TURUN.\nPosisi di bawah permukaan air laut dinilai negatif (-).", "Contoh: Kapal selam menyelam 15 meter = -15" },
        new string[] { "Perhatikan! Kunang-Kunang TURUN.\nBergerak mundur ke belakang bernilai negatif (-).", "Contoh: Mundur menjauhi rintangan 5 langkah = -5" }
    };

    private string[][] teksUntung = new string[][] {
        new string[] { "Hebat! Kunang-Kunang UNTUNG.\nDalam perdagangan, laba dicatat sebagai bilangan positif (+).", "Contoh: Keuntungan jualan pulsa Rp10.000 = +10.000" },
        new string[] { "Mantap! Kunang-Kunang UNTUNG.\nMenerima pemberian uang saku bernilai positif (+).", "Contoh: Diberi uang jajan tambahan Rp5.000 = +5.000" },
        new string[] { "Luar biasa! Kunang-Kunang UNTUNG.\nMenabung uang di celengan menambah jumlah aset (+).", "Contoh: Menabung uang saku Rp8.000 = +8.000" }
    };

    private string[][] teksRugi = new string[][] {
        new string[] { "Waduh! Kunang-Kunang RUGI.\nKerugian finansial atau berutang dinyatakan sebagai bilangan negatif (-).", "Contoh: Berutang ke warung Rp10.000 = -10.000" },
        new string[] { "Sayang sekali! Kunang-Kunang RUGI.\nBarang dagangan yang rusak atau busuk mengurangi nilai (-).", "Contoh: 5 buah apel busuk dibuang = -5" },
        new string[] { "Aduh! Kunang-Kunang RUGI.\nMengeluarkan uang untuk membayar denda bernilai negatif (-).", "Contoh: Membayar denda perpustakaan Rp2.000 = -2.000" }
    };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (suaraAmbilKunang != null)
            {
                AudioSource.PlayClipAtPoint(suaraAmbilKunang, Camera.main.transform.position, 1f);
            }

            string[] pesan = new string[2];

            switch (tipeKunangKunang)
            {
                case TipeSimbol.Naik:
                    pesan = teksNaik[Random.Range(0, teksNaik.Length)];
                    break;
                case TipeSimbol.Turun:
                    pesan = teksTurun[Random.Range(0, teksTurun.Length)];
                    break;
                case TipeSimbol.Untung:
                    pesan = teksUntung[Random.Range(0, teksUntung.Length)];
                    break;
                case TipeSimbol.Rugi:
                    pesan = teksRugi[Random.Range(0, teksRugi.Length)];
                    break;
            }

            Skenario1Manager gm = Object.FindFirstObjectByType<Skenario1Manager>();
            if (gm != null)
            {
                gm.TambahKunangKunang(pesan, tipeKunangKunang); 
            }

            Destroy(gameObject); 
        }
    }
}