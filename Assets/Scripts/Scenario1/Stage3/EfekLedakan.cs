using UnityEngine;

public class EfekLedakan : MonoBehaviour
{
    [Header("Pengaturan Ledakan")]
    public float waktuHancur = 1.5f; // Berapa lama ledakan terlihat sebelum hilang

    void Start()
    {
        // Otomatis menghancurkan diri sendiri setelah beberapa detik
        Destroy(gameObject, waktuHancur);
    }
}