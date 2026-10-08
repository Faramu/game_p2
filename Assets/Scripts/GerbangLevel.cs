using UnityEngine;
using UnityEngine.SceneManagement;

public class GerbangLevel : MonoBehaviour
{
    [Header("--- VALIDASI MISI ---")]
    [Tooltip("Wajib menyelesaikan semua misi sebelum gerbang mengizinkan pindah level?")]
    public bool wajibSelesaikanMisi = true;

    [Tooltip("Referensi MisiManager di scene (opsional, jika kosong dicari otomatis)")]
    public MisiManager misiManager;

    [Tooltip("Referensi DialogManager di scene (opsional, memakai DialogManager.Instance jika kosong)")]
    public DialogManager dialogManager;

    [Header("--- PESAN DIALOG MISI BELUM SELESAI ---")]
    public string namaPembicara = "Aki Aruna";
    [TextArea(2, 4)]
    public string pesanMisiBelumSelesai = "Kamu belum menyelesaikan semua target misi pada level ini! Selesaikan terlebih dahulu sebelum membuka gerbang berikutnya.";


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Berpindah scene hanya jika gerbang sudah terbuka (menjadi Trigger) dan disentuh player
        if (collision.CompareTag("Player"))
        {
            WinManager.TampilkanWin();
        }
    }
}