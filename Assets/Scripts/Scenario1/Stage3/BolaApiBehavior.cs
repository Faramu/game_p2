using UnityEngine;
using TMPro;
using System.Collections;

public class BolaApiBehavior : MonoBehaviour
{
    [Header("Pengaturan Gerakan")]
    public float kecepatanTerbang = 12f;
    public float tinggiLengkungan = 1.5f;

    [Header("Teks Lompatan")]
    public TextMeshProUGUI teksLompatanPrefab;
    public Vector3 offsetTeks = new Vector3(0f, 1.2f, 0f);
    public float ukuranTeks = 3.5f;
    public Color warnaTeksPositif = Color.green;
    public Color warnaTeksNegatif = Color.red;

    [Header("Efek & Audio")]
    public GameObject efekLedakanPrefab;
    public AudioClip suaraLedakan;
    [Range(0f, 1f)] public float volumeLedakan = 1f;

    private Transform target;
    private System.Action onSelesai;
    private string teksLompatan;

    private Vector3 posisiAwal;
    private Vector3 posisiTarget;
    private float progress = 0f;
    private bool sudahMengenai = false;
    private TextMeshProUGUI teksInstance;

    public void Tembak(Transform targetTransform, System.Action callback, string tag, string teksLompat = "")
    {
        target = targetTransform;
        onSelesai = callback;
        teksLompatan = teksLompat;

        posisiAwal = transform.position;
        posisiTarget = target != null ? target.position : transform.position + Vector3.right * 3f;

        progress = 0f;
        sudahMengenai = false;

        BuatTeksLompatan();
        StartCoroutine(TerbangKeTarget());
    }

    void BuatTeksLompatan()
    {
        if (string.IsNullOrEmpty(teksLompatan)) return;

        if (teksLompatanPrefab != null)
        {
            teksInstance = Instantiate(teksLompatanPrefab, transform.position + offsetTeks, Quaternion.identity);
        }
        else
        {
            GameObject go = new GameObject("TeksLompatan");
            teksInstance = go.AddComponent<TextMeshProUGUI>();
            teksInstance.alignment = TextAlignmentOptions.Center;
            teksInstance.fontSize = ukuranTeks;
            teksInstance.transform.position = transform.position + offsetTeks;
        }

        if (teksInstance != null)
        {
            teksInstance.text = teksLompatan;
            teksInstance.color = teksLompatan.StartsWith("-") ? warnaTeksNegatif : warnaTeksPositif;
        }
    }

    IEnumerator TerbangKeTarget()
    {
        float jarak = Vector3.Distance(posisiAwal, posisiTarget);
        if (jarak < 0.5f) jarak = 0.5f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * (kecepatanTerbang / jarak);
            progress = Mathf.Clamp01(progress);

            // Gerakan lurus + sedikit lengkungan
            Vector3 pos = Vector3.Lerp(posisiAwal, posisiTarget, progress);
            pos.y += Mathf.Sin(progress * Mathf.PI) * tinggiLengkungan;

            transform.position = pos;

            if (teksInstance != null)
                teksInstance.transform.position = transform.position + offsetTeks;

            yield return null;
        }

        // Pastikan posisi akhir tepat
        transform.position = posisiTarget;
        MengenaiTarget();
    }

    void MengenaiTarget()
    {
        if (sudahMengenai) return;
        sudahMengenai = true;

        // Suara
        if (suaraLedakan != null)
        {
            AudioSource.PlayClipAtPoint(suaraLedakan, 
                Camera.main != null ? Camera.main.transform.position : transform.position, 
                volumeLedakan);
        }

        // Efek ledakan
        if (efekLedakanPrefab != null)
        {
            Instantiate(efekLedakanPrefab, transform.position, Quaternion.identity);
        }

        // Hapus teks
        if (teksInstance != null)
            Destroy(teksInstance.gameObject);

        // Callback
        onSelesai?.Invoke();

        Destroy(gameObject);
    }
}