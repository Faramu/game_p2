using UnityEngine;
using TMPro;
using System.Collections;

public class PeluruKPK : MonoBehaviour
{
    [Header("Referensi Visual Peluru")]
    public TextMeshPro labelTeks;                 // angka di peluru

    [Header("Referensi Manager")]
    public Stage6Level2Manager stage6Level2Manager;

    [Header("Animasi (Opsional)")]
    public float durasiGetar = 0.2f;

    private bool isJawabanBenar;
    private bool sudahDipilih = false;
    private Vector3 posisiAwal;

    private void Awake()
    {
        posisiAwal = transform.position;

        if (stage6Level2Manager == null)
        {
            stage6Level2Manager = FindFirstObjectByType<Stage6Level2Manager>();
        }
    }

    // Dipanggil oleh Manager saat soal ditampilkan
    public void SetupPeluru(string teks, bool benar)
    {
        gameObject.SetActive(true);
        transform.position = posisiAwal;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        if (labelTeks != null)
        {
            labelTeks.gameObject.SetActive(true);
            labelTeks.text = teks;
        }

        isJawabanBenar = benar;
        sudahDipilih = false;
    }

    // ==========================================
    // PLAYER MENYENTUH PELURU
    // ==========================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") &&
            stage6Level2Manager != null &&
            stage6Level2Manager.currentState == Stage6Level2Manager.GameState.MemilihPeluru)
        {
            PilihPeluru();
        }
    }

    // ==========================================
    // PLAYER KLIK PELURU
    // ==========================================
    private void OnMouseDown()
    {
        if (stage6Level2Manager == null) return;

        if (stage6Level2Manager.currentState == Stage6Level2Manager.GameState.MemilihPeluru)
        {
            PilihPeluru();
        }
    }

    // ==========================================
    // PROSES PILIHAN
    // ==========================================
    private void PilihPeluru()
    {
        if (sudahDipilih) return;
        sudahDipilih = true;

        if (isJawabanBenar)
        {
            StartCoroutine(PeluruBenar());
        }
        else
        {
            StartCoroutine(PeluruSalah());
        }
    }

    // ==========================================
    // JAWABAN BENAR
    // ==========================================
    private IEnumerator PeluruBenar()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (labelTeks != null)
            labelTeks.gameObject.SetActive(false);

        // Beritahu Manager
        if (stage6Level2Manager != null)
        {
            stage6Level2Manager.JawabanBenar();
        }

        // Langsung sembunyikan peluru (efek tembak diurus Manager)
        gameObject.SetActive(false);
        yield return null;
    }

    // ==========================================
    // JAWABAN SALAH
    // ==========================================
    private IEnumerator PeluruSalah()
    {
        if (stage6Level2Manager != null)
        {
            stage6Level2Manager.PemainTerkenaDamage();
        }

        // Efek getar
        Vector3 posisiAsli = transform.position;

        transform.position = posisiAsli + new Vector3(0.15f, 0f, 0f);
        yield return new WaitForSeconds(0.07f);

        transform.position = posisiAsli - new Vector3(0.15f, 0f, 0f);
        yield return new WaitForSeconds(0.07f);

        transform.position = posisiAsli;

        // Bisa dipilih lagi
        sudahDipilih = false;
    }
}