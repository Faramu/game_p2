using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                // Menambah koin sementara untuk keperluan di dalam Stage saat itu
                player.coins += 1; 

                // Menambah dan menyimpan koin secara permanen untuk Menu Pencapaian
                UpdatePencapaian.TambahKoin(1); 

                // Menghilangkan koin dari arena
                Destroy(gameObject);
            }
        }
    }
}