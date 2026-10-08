using UnityEngine;
using TMPro;
using System.Collections;

public class KunciKPK : MonoBehaviour
{
    [Header("Referensi Visual Kunci")]
    public TextMeshPro labelTeks;

    [Header("Referensi Stage 6")]
    public Stage6Manager stage6Manager;

    [Header("Animasi Kunci")]
    public Transform targetGembok;
    public float durasiGerak = 0.5f;

    private bool isJawabanBenar;
    private bool sudahDipilih = false;
    private Vector3 posisiAwal;

    private void Awake()
    {
        posisiAwal = transform.position;

        if (stage6Manager == null)
        {
            // Gunakan API baru (menghilangkan warning obsolete)
            stage6Manager = FindFirstObjectByType<Stage6Manager>();
        }
    }

    // Dipanggil oleh Stage6Manager saat soal ditampilkan
    public void SetupKunci(string teks, bool benar)
    {
        gameObject.SetActive(true);
        transform.position = posisiAwal;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        if (labelTeks != null)
        {
            labelTeks.gameObject.SetActive(true);
            labelTeks.text = teks;
        }

        isJawabanBenar = benar;
        sudahDipilih = false;
    }

    // ==========================================
    // PLAYER MENYENTUH KUNCI
    // ==========================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && 
            stage6Manager != null &&
            stage6Manager.currentState == Stage6Manager.GameState.MemilihKunci)
        {
            PilihKunci();
        }
    }

    // ==========================================
    // PLAYER KLIK KUNCI
    // ==========================================
    private void OnMouseDown()
    {
        if (stage6Manager == null)
            return;

        if (stage6Manager.currentState == Stage6Manager.GameState.MemilihKunci)
        {
            PilihKunci();
        }
    }

    // ==========================================
    // PROSES JAWABAN
    // ==========================================
    private void PilihKunci()
    {
        if (sudahDipilih)
            return;

        sudahDipilih = true;

        if (isJawabanBenar)
        {
            StartCoroutine(KunciBenar());
        }
        else
        {
            StartCoroutine(KunciSalah());
        }
    }

    // ==========================================
    // JAWABAN BENAR
    // ==========================================
    private IEnumerator KunciBenar()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (labelTeks != null)
            labelTeks.gameObject.SetActive(false);

        // Beritahu Manager bahwa jawaban benar
        if (stage6Manager != null)
        {
            stage6Manager.JawabanBenar();
        }

        // Animasi kunci bergerak menuju gembok
        if (targetGembok != null)
        {
            Vector3 posisiAwalGerak = transform.position;
            float waktu = 0f;

            while (waktu < durasiGerak)
            {
                waktu += Time.deltaTime;
                transform.position = Vector3.Lerp(
                    posisiAwalGerak,
                    targetGembok.position,
                    waktu / durasiGerak
                );
                yield return null;
            }

            transform.position = targetGembok.position;
        }

        // Sembunyikan kunci setelah digunakan
        gameObject.SetActive(false);
    }

    // ==========================================
    // JAWABAN SALAH
    // ==========================================
    private IEnumerator KunciSalah()
    {
        if (stage6Manager != null)
        {
            stage6Manager.PemainTerkenaDamage();
        }

        Vector3 posisiAsli = transform.position;

        // Efek kunci bergetar
        transform.position = posisiAsli + new Vector3(0.2f, 0f, 0f);
        yield return new WaitForSeconds(0.1f);

        transform.position = posisiAsli - new Vector3(0.2f, 0f, 0f);
        yield return new WaitForSeconds(0.1f);

        transform.position = posisiAsli;

        // Bisa dipilih kembali
        sudahDipilih = false;
    }
}