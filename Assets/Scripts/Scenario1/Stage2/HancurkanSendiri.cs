using UnityEngine;

public class HancurkanSendiri : MonoBehaviour
{
    [Header("Pengaturan Waktu Hancur")]
    public float waktuHidup = 1.5f; // Sesuaikan dengan durasi animasi ledakan

    void Awake()
    {
        // Pastikan objek ini selalu aktif saat di-instantiate
        gameObject.SetActive(true);
    }

    void Start()
    {
        // Hancurkan GameObject ini setelah 'waktuHidup' detik
        Destroy(gameObject, waktuHidup);
    }
}