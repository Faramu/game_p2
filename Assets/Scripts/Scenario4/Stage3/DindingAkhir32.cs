using UnityEngine;
using System.Collections;

public class DindingAkhir32 : MonoBehaviour
{
    [Header("Visual Dinding Bertahap")]
    [Tooltip("Index 0 = Utuh, 1 = Retak 1, 2 = Retak 2, 3 = Retak 3, 4 = Retak Parah")]
    public GameObject[] tahapDinding;

    [Header("Efek")]
    public ParticleSystem efekDebu;
    public AudioSource audioBatuRuntuh;

    [Header("Spawn Koin")]
    public GameObject koinRahasiaPrefab;
    public int jumlahKoinSpawn = 5;

    private void Start()
    {
        // Pastikan hanya tahap 0 yang aktif di awal
        SetTahap(0);
    }

    public void ProsesKerusakanBertahap(int jumlahBenar)
    {
        Debug.Log($"<color=orange>[DINDING]</color> Dipanggil dengan jumlahBenar = {jumlahBenar}");

        if (tahapDinding == null || tahapDinding.Length == 0)
        {
            Debug.LogError("[DINDING] Array tahapDinding KOSONG! Drag objek dinding ke Inspector.");
            return;
        }

        // jumlahBenar 1 → index 1, 2 → index 2, dst. (index 0 = utuh)
        int index = Mathf.Clamp(jumlahBenar, 0, tahapDinding.Length - 1);

        SetTahap(index);

        if (efekDebu != null) efekDebu.Play();
        if (audioBatuRuntuh != null) audioBatuRuntuh.Play();
    }

    private void SetTahap(int indexAktif)
    {
        for (int i = 0; i < tahapDinding.Length; i++)
        {
            if (tahapDinding[i] != null)
            {
                bool aktif = (i == indexAktif);
                tahapDinding[i].SetActive(aktif);
            }
        }

        Debug.Log($"<color=green>[DINDING]</color> Menampilkan tahap index {indexAktif}");
    }

    public void HancurkanDindingRaksasa()
    {
        StartCoroutine(ProsesKehancuranFinal());
    }

    private IEnumerator ProsesKehancuranFinal()
    {
        if (efekDebu != null) efekDebu.Play();
        if (audioBatuRuntuh != null) audioBatuRuntuh.Play();

        // Matikan semua visual
        for (int i = 0; i < tahapDinding.Length; i++)
        {
            if (tahapDinding[i] != null)
                tahapDinding[i].SetActive(false);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (koinRahasiaPrefab != null)
        {
            for (int i = 0; i < jumlahKoinSpawn; i++)
            {
                Vector3 pos = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(1f, 2.5f), 0);
                Instantiate(koinRahasiaPrefab, pos, Quaternion.identity);
            }
        }

        Debug.Log("<color=cyan>[DINDING]</color> Dinding hancur total!");
        yield return null;
    }
}