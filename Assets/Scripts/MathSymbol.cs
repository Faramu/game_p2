using UnityEngine;
using TMPro;

public class MathSymbol : MonoBehaviour
{
    [Header("Pengaturan Edukasi")]
    [TextArea(2, 4)]
    public string pesanMatematika; // Isi penjelasan materi di Inspector nanti

    [Header("Referensi UI Canvas")]
    public GameObject panelEdukasi;
    public TextMeshProUGUI textEdukasi;

    // Fungsi otomatis Unity saat mendeteksi tabrakan Is Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Memastikan yang menabrak objek ini adalah Player
        if (collision.CompareTag("Player"))
        {
            // Tampilkan panel dan ubah teksnya sesuai materi
            panelEdukasi.SetActive(true);
            textEdukasi.text = pesanMatematika;

            // Hancurkan objek simbol ini agar tidak bisa diambil dua kali
            Destroy(gameObject);
        }
    }
}