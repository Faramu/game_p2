using UnityEngine;

public class TriggerGerbang : MonoBehaviour
{
    [Header("--- UI INDIKATOR (opsional) ---")]
    public GameObject tombolInteraksiVisual;

    private bool sudahDipicu = false;

    void Start()
    {
        if (tombolInteraksiVisual != null)
            tombolInteraksiVisual.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hanya sekali, saat player masuk
        if (other.CompareTag("Player") && !sudahDipicu)
        {
            sudahDipicu = true;

            if (tombolInteraksiVisual != null)
                tombolInteraksiVisual.SetActive(false);

            // Langsung mulai dialog skenario (tanpa tekan E)
            if (SkenarioGerbangManager.Instance != null)
            {
                SkenarioGerbangManager.Instance.MulaiSkenarioGerbang();
            }
            else
            {
                Debug.LogError("SkenarioGerbangManager tidak ditemukan di Scene!");
            }
        }
    }
}