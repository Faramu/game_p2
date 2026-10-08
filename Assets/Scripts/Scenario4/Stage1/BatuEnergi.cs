using UnityEngine;
using TMPro;

public class BatuEnergi : MonoBehaviour
{
    [Header("Referensi")]
    public TextMeshPro labelTeks;
    public SpriteRenderer spriteRenderer;

    [Header("Warna Visual")]
    public Color warnaNormal = new Color(0.75f, 0.75f, 0.8f);   // abu kebiruan
    public Color warnaTerpilih = new Color(0.1f, 1f, 0.3f);      // hijau neon
    public Color warnaTerkunci = new Color(0.4f, 0.4f, 0.4f);    // abu gelap

    [Header("Efek Saat Dipilih")]
    public bool gunakanScale = true;
    public float scaleTerpilih = 1.25f;

    // Data runtime
    private int nilaiBatu;
    private bool isFaktorBenar;
    private bool isSelected = false;
    private bool bisaDiklik = true;
    private Vector3 posisiAwal;
    private Vector3 scaleAwal;

    private void Awake()
    {
        posisiAwal = transform.position;
        scaleAwal = transform.localScale;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetupBatu(int nilai, bool merupakanFaktor)
    {
        gameObject.SetActive(true);
        transform.position = posisiAwal;
        transform.localScale = scaleAwal;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        nilaiBatu = nilai;
        isFaktorBenar = merupakanFaktor;
        isSelected = false;
        bisaDiklik = true;

        if (labelTeks != null)
        {
            labelTeks.gameObject.SetActive(true);
            labelTeks.text = nilai.ToString();
        }

        UpdateVisual();
    }

    public void ResetBatu()
    {
        isSelected = false;
        bisaDiklik = true;
        transform.localScale = scaleAwal;
        UpdateVisual();
    }

    private void OnMouseDown()
    {
        if (!bisaDiklik) return;
        if (GoaFaktorManager.Instance == null) return;
        if (GoaFaktorManager.Instance.currentState != GoaFaktorManager.GameState.Bertempur) return;

        TogglePilih();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!bisaDiklik) return;
        if (!collision.CompareTag("Player")) return;
        if (GoaFaktorManager.Instance == null) return;
        if (GoaFaktorManager.Instance.currentState != GoaFaktorManager.GameState.Bertempur) return;

        TogglePilih();
    }

    private void TogglePilih()
    {
        isSelected = !isSelected;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Warna
        if (spriteRenderer != null)
        {
            if (!bisaDiklik)
                spriteRenderer.color = warnaTerkunci;
            else if (isSelected)
                spriteRenderer.color = warnaTerpilih;
            else
                spriteRenderer.color = warnaNormal;
        }

        // Scale
        if (gunakanScale)
            transform.localScale = isSelected ? scaleAwal * scaleTerpilih : scaleAwal;

        // Warna teks biar kontras
        if (labelTeks != null)
            labelTeks.color = isSelected ? Color.black : Color.white;
    }

    public bool IsSelected => isSelected;
    public int NilaiBatu => nilaiBatu;
    public bool IsFaktorBenar => isFaktorBenar;

    public void KunciBatu()
    {
        bisaDiklik = false;
        UpdateVisual();
    }

    public void SembunyikanBatu()
    {
        gameObject.SetActive(false);
    }
}