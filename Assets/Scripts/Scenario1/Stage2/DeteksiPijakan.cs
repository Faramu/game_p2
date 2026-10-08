using UnityEngine;

public class DeteksiPijakan : MonoBehaviour
{
    [Header("Hubungkan ke Manager")]
    public TebingKabutManager manager; 
    
    [Header("Pengaturan Tebing")]
    public bool isPositif; 

    void OnCollisionEnter2D(Collision2D col) 
    {
        // Mendeteksi apakah yang menyentuh/menginjak adalah Player
        if (col.gameObject.CompareTag("Player")) 
        {
            if (manager == null)
            {
                manager = Object.FindFirstObjectByType<TebingKabutManager>();
            }

            if (manager != null) {
                // Beri tahu manager jawaban pemain
                manager.CekJawaban(isPositif);
            } else {
                Debug.LogError("Manager belum dihubungkan di tebing: " + gameObject.name);
            }
        }
    }
}