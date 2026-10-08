using UnityEngine;

public class PrasastiPetunjuk : MonoBehaviour
{
    [Header("UI Interaksi")]
    public GameObject floatingPrompt; // Ikon [!] di atas prasasti

    private void Start()
    {
        // Pastikan prompt disembunyikan di awal
        if (floatingPrompt != null)
            floatingPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (floatingPrompt != null)
                floatingPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (floatingPrompt != null)
                floatingPrompt.SetActive(false);
        }
    }

    // Jika Anda ingin membuat interaksi dialog saat pemain menekan tombol, 
    // Anda bisa menambahkannya di dalam method Update() mengecek Input.GetKeyDown() 
    // saat OnTrigger-nya sedang aktif.
}