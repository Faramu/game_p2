using UnityEngine;

public class Jurang : MonoBehaviour
{
    private NagaBattleGarisBilanganManager manager;

    void Start()
    {
        // PERBAIKAN WARNING: Menggunakan FindFirstObjectByType
        manager = Object.FindFirstObjectByType<NagaBattleGarisBilanganManager>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (manager != null)
            {
                // Akan memanggil fungsi RespawnPlayer yang baru saja kita buat di manager
                manager.RespawnPlayer();
            }
        }
    }
}