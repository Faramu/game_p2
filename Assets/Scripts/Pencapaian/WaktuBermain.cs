using UnityEngine;

public class WaktuBermain : MonoBehaviour
{
    private float waktuBerjalan;
    private float timerSimpan = 0f;

    void Awake()
    {
        // 1. Mencegah Objek Ganda saat pemain bolak-balik antar Scene
        // Versi baru (tidak obsolete)
        if (Object.FindObjectsByType<WaktuBermain>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Membuat objek ini abadi menembus semua scene
        DontDestroyOnLoad(gameObject);

        // 2. Muat waktu sebelumnya dari memori
        waktuBerjalan = PlayerPrefs.GetFloat("WaktuBermain", 0f);
    }

    void Update()
    {
        // 3. Tambahkan waktu terus menerus
        waktuBerjalan += Time.deltaTime;
        timerSimpan += Time.deltaTime;

        // 4. AUTO-SAVE setiap 1 detik
        if (timerSimpan >= 1f)
        {
            PlayerPrefs.SetFloat("WaktuBermain", waktuBerjalan);
            PlayerPrefs.Save();

            timerSimpan = 0f;
        }
    }
}