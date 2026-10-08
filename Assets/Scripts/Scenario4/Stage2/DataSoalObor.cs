using UnityEngine;

[System.Serializable]
public class DataSoalObor
{
    [TextArea(2, 3)]
    public string narasiMisi;

    public int angkaTarget;                 // angka target / kelelawar
    public int[] pilihanAngkaObor;          // semua angka di obor (termasuk pengecoh)
    public int[] urutanBenar;               // faktor diurutkan dari kecil → besar

    [Header("Feedback Edukasi Matematika")]
    [TextArea(2, 3)] public string penjelasanBenar;
    [TextArea(2, 3)] public string penjelasanSalah;

    // Constructor untuk soal URUTAN (Level 2)
    public DataSoalObor(string misi, int target, int[] pilihan, int[] urutan, string feedbackB, string feedbackS)
    {
        narasiMisi = misi;
        angkaTarget = target;
        pilihanAngkaObor = pilihan;
        urutanBenar = urutan;
        penjelasanBenar = feedbackB;
        penjelasanSalah = feedbackS;
    }
}