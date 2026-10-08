using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistem Deck / Bag untuk menjamin:
/// 1. Tidak ada soal duplikat dalam satu sesi gameplay (misal 10 soal).
/// 2. Soal yang sudah digunakan tidak muncul kembali sampai seluruh isi bank habis (deck exhausted).
/// 3. Ketika deck habis, otomatis di-reshuffle (Fisher-Yates) secara adil.
/// </summary>
public class QuestionDeck<T>
{
    private readonly List<T> poolMaster = new List<T>();
    private readonly List<T> sisaDeck = new List<T>();
    private readonly HashSet<T> sesiAktif = new HashSet<T>();

    public QuestionDeck() { }

    public QuestionDeck(IEnumerable<T> kumpulanSoal)
    {
        Inisialisasi(kumpulanSoal);
    }

    public void Inisialisasi(IEnumerable<T> kumpulanSoal)
    {
        poolMaster.Clear();
        if (kumpulanSoal != null)
        {
            poolMaster.AddRange(kumpulanSoal);
        }
        ResetDeck();
    }

    public int TotalSoalMaster => poolMaster.Count;
    public int SisaDeckCount => sisaDeck.Count;

    public void ResetDeck()
    {
        sisaDeck.Clear();
        sisaDeck.AddRange(poolMaster);
        KocokDeck(sisaDeck);
        sesiAktif.Clear();
    }

    public void MulaiSesiGameplayBaru()
    {
        sesiAktif.Clear();
    }

    /// <summary>
    /// Mengambil 1 soal berikutnya dari deck.
    /// Menjamin tidak duplikat dengan soal yang sudah ditarik di sesi saat ini.
    /// </summary>
    public T AmbilSoalBerikutnya()
    {
        if (poolMaster.Count == 0)
        {
            Debug.LogWarning("[DECK] Bank soal kosong!");
            return default;
        }

        // Jika sisa deck habis, reset dan kocok ulang seluruh deck master
        if (sisaDeck.Count == 0)
        {
            Debug.Log("[DECK] Seluruh soal di bank telah dimainkan. Mereset dan mengocok ulang deck!");
            sisaDeck.AddRange(poolMaster);
            KocokDeck(sisaDeck);
        }

        // Cari soal dari sisaDeck yang belum ditarik pada sesi gameplay saat ini
        int indexTerpilih = -1;
        for (int i = 0; i < sisaDeck.Count; i++)
        {
            if (!sesiAktif.Contains(sisaDeck[i]))
            {
                indexTerpilih = i;
                break;
            }
        }

        // Jika semua kartu di sisaDeck sudah ada di sesiAktif (misal bank < 10 soal),
        // fallback pilih kartu pertama
        if (indexTerpilih < 0)
        {
            indexTerpilih = 0;
        }

        T soal = sisaDeck[indexTerpilih];
        sisaDeck.RemoveAt(indexTerpilih);
        sesiAktif.Add(soal);

        return soal;
    }

    /// <summary>
    /// Mengambil sejumlah 'jumlahTarget' soal unik sekaligus untuk 1 sesi gameplay (misal 10 soal).
    /// </summary>
    public List<T> AmbilSesiSoal(int jumlahTarget = 10)
    {
        MulaiSesiGameplayBaru();
        List<T> hasil = new List<T>();

        int target = Mathf.Min(jumlahTarget, poolMaster.Count);
        for (int i = 0; i < target; i++)
        {
            hasil.Add(AmbilSoalBerikutnya());
        }

        return hasil;
    }

    private static void KocokDeck(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }
}
