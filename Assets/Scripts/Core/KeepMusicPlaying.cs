using UnityEngine;

public class KeepMusicPlaying : MonoBehaviour
{
    private static KeepMusicPlaying instance;

    void Awake()
    {
        // Mengecek apakah sudah ada objek musik yang berjalan
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Hancurkan objek baru yang duplikat saat scene di-reload
            return;
        }

        instance = this;
        // Membuat objek ini tidak hancur saat scene di-load ulang
        DontDestroyOnLoad(gameObject); 
    }
}