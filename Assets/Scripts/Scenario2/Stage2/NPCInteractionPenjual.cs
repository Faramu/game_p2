using UnityEngine;

public class NPCInteractionGerbang : MonoBehaviour
{
    [Header("UI & Indikator")]
    [Tooltip("Tombol/Ikon bertuliskan 'E' yang muncul di atas NPC/Gerbang saat player dekat")]
    public GameObject tombolInteraksiVisual; 
    
    private bool playerDiDekat = false;
    private bool sudahTerbuka = false; // Mencegah interaksi berulang

    void Start()
    {
        if (tombolInteraksiVisual != null) tombolInteraksiVisual.SetActive(false);
    }

    void Update()
    {
        // Jika player di dekat gerbang dan menekan E
        if (playerDiDekat && !sudahTerbuka && Input.GetKeyDown(KeyCode.E))
        {
            TriggerInteraksi();
        }
    }

    public void TriggerInteraksi()
    {
        if (!playerDiDekat) return;

        Debug.Log("Pemain berinteraksi dengan Pedagang Gerbang!");
        sudahTerbuka = true; // Kunci agar tidak bisa ditekan E berkali-kali
        
        // Sembunyikan ikon "E" saat dialog dimulai
        if (tombolInteraksiVisual != null) tombolInteraksiVisual.SetActive(false);

        // Panggil SkenarioGerbangManager untuk memulai cerita
        if (SkenarioGerbangManager.Instance != null)
        {
            SkenarioGerbangManager.Instance.MulaiSkenarioGerbang();
        }
        else
        {
            Debug.LogError("SkenarioGerbangManager.Instance tidak ditemukan di Scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !sudahTerbuka)
        {
            playerDiDekat = true;
            if (tombolInteraksiVisual != null) tombolInteraksiVisual.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDiDekat = false;
            if (tombolInteraksiVisual != null) tombolInteraksiVisual.SetActive(false);
        }
    }
}