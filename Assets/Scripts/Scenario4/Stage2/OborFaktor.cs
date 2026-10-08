using UnityEngine;
using TMPro;

public class OborFaktor : MonoBehaviour
{
    [Header("Pengaturan Obor")]
    public int nilaiAngkaObor;
    public bool isBagianDariUrutanBenar;

    [Header("Referensi Komponen")]
    public TextMeshPro textLabelAngka;
    public GameObject efekCahayaObor;
    public AudioSource audioObor;
    public SpriteRenderer spriteRenderer;

    [Header("Warna Visual")]
    public Color warnaNormal = Color.white;
    public Color warnaTerpilih = new Color(0.2f, 1f, 0.4f);

    private bool sudahDipilih = false;
    private bool bisaDiklik = true;

    private void Awake()
    {
        if (textLabelAngka == null)
            textLabelAngka = GetComponentInChildren<TextMeshPro>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetupObor(int angkaBaru, bool termasukUrutanBenar)
    {
        gameObject.SetActive(true);

        nilaiAngkaObor = angkaBaru;
        isBagianDariUrutanBenar = termasukUrutanBenar;
        sudahDipilih = false;
        bisaDiklik = true;

        if (textLabelAngka != null)
            textLabelAngka.text = nilaiAngkaObor.ToString();

        if (efekCahayaObor != null)
            efekCahayaObor.SetActive(false);

        UpdateVisual();
    }

    public void ResetObor()
    {
        sudahDipilih = false;
        bisaDiklik = true;

        if (efekCahayaObor != null)
            efekCahayaObor.SetActive(false);

        UpdateVisual();
    }

private void OnMouseDown()
{
    if (!bisaDiklik) return;
    if (GoaFaktorManager2.Instance == null) return;
    if (GoaFaktorManager2.Instance.currentState != GoaFaktorManager2.GameState.PilihObor) return;

    // Toggle: kalau sudah dipilih → batalkan
    if (sudahDipilih)
    {
        sudahDipilih = false;
        UpdateVisual();

        if (efekCahayaObor != null)
            efekCahayaObor.SetActive(false);

        GoaFaktorManager2.Instance.HapusPilihanDariUrutan(this);
        Debug.Log($"<color=orange>[BATAL]</color> {gameObject.name} | Nilai: {nilaiAngkaObor}");
        return;
    }

    // Belum dipilih → pilih
    sudahDipilih = true;
    UpdateVisual();

    if (efekCahayaObor != null)
        efekCahayaObor.SetActive(true);

    if (audioObor != null)
        audioObor.Play();

    GoaFaktorManager2.Instance.TambahPilihanKeUrutan(this);
    Debug.Log($"<color=cyan>[KLIK OBOR]</color> {gameObject.name} | Nilai: {nilaiAngkaObor}");
}


    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = sudahDipilih ? warnaTerpilih : warnaNormal;
    }

    public void KunciObor()
    {
        bisaDiklik = false;
    }

    public void NyalakanOborBenar()
    {
        sudahDipilih = true;
        if (efekCahayaObor != null) efekCahayaObor.SetActive(true);
        if (audioObor != null) audioObor.Play();
        UpdateVisual();
    }

    public void PadamkanOborSalah()
    {
        if (audioObor != null) audioObor.Play();
        ResetObor();
    }
}