using UnityEngine;
using UnityEngine.SceneManagement; // Tambahan wajib untuk pindah scene
using System.Collections;

public class GerbangKeluar : MonoBehaviour
{
    [Header("Referensi Gerbang")]
    public GameObject gerbangTertutup;
    public GameObject gerbangTerbuka;

    [Header("Pengaturan Pindah Scene")]
    public string tagPlayer = "Player";
    [Tooltip("Ketik nama scene stage berikutnya persis seperti di Build Settings")]
    public string namaSceneBerikutnya = "Stage3_Level1"; 
    public float jedaWaktuPindah = 1.5f; // Waktu jeda agar pemain bisa melihat gerbang terbuka dulu
    public bool sudahTerbuka = false;

    private void Start()
    {
        // Pastikan kondisi awal: tertutup dan belum terbuka
        sudahTerbuka = false;
        if (gerbangTertutup != null) gerbangTertutup.SetActive(true);
        if (gerbangTerbuka != null) gerbangTerbuka.SetActive(false);
    }

private void OnTriggerEnter2D(Collider2D other)
    {
        // Radar untuk memastikan sistem fisika Unity mendeteksi tabrakan
        Debug.Log("<color=yellow>[CEK FISIKA]</color> Gerbang disentuh oleh objek: " + other.name + " | Tag: " + other.tag);

        if (sudahTerbuka) return;

        if (other.CompareTag(tagPlayer))
        {
            BukaGerbang();
            StartCoroutine(ProsesPindahScene());
        }
        else
        {
            Debug.LogWarning("[GERBANG] Objek yang menyentuh bukan Player. Tag yang diminta: " + tagPlayer);
        }
    }
    public void BukaGerbang()
    {
        if (sudahTerbuka) return;

        sudahTerbuka = true;

        if (gerbangTertutup != null)
            gerbangTertutup.SetActive(false);

        if (gerbangTerbuka != null)
            gerbangTerbuka.SetActive(true);

        Debug.Log("<color=green>[GERBANG]</color> Gerbang berhasil dibuka!");
    }

    private IEnumerator ProsesPindahScene()
    {
        // Tunggu sejenak agar animasi/visual gerbang terbuka terlihat
        yield return new WaitForSeconds(jedaWaktuPindah);
        
        // Tampilkan Win UI melalui WinManager
        WinManager.TampilkanWin();
    }

    // Dipanggil dari Manager saat level selesai (semua soal benar)
    public void AktifkanGerbang()
    {
        // Munculkan gerbang dalam keadaan tertutup
        gameObject.SetActive(true);

        if (gerbangTertutup != null) gerbangTertutup.SetActive(true);
        if (gerbangTerbuka != null) gerbangTerbuka.SetActive(false);

        sudahTerbuka = false;
    }
}