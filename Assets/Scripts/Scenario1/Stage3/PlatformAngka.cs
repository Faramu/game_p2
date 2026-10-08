using UnityEngine;

public class PlatformAngka : MonoBehaviour
{
    public int nilaiAngkaPlatform; // Isi angka ini di Inspector sesuai platformnya (-3, -2, 1, dll)
    private NagaBattleGarisBilanganManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<NagaBattleGarisBilanganManager>();
    }

    // Deteksi jika Player mendarat di atas platform ini
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            manager.CekJawabanSiswa(nilaiAngkaPlatform);
        }
    }
}