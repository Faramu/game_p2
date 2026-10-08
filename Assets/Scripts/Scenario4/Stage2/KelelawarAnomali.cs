using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KelelawarAnomali : MonoBehaviour
{
    [Header("Daftar Objek Kelelawar di Scene")]
    [Tooltip("Masukkan semua kelelawar yang ingin dihilangkan satu per satu (bisa 2, 3, 5, dst)")]
    public GameObject[] daftarKelelawar;

    [Header("Pengaturan Serangan")]
    public float kecepatanTerbang = 15f;

    [Tooltip("Nama State animasi menyerang di Animator (contoh: KelelawarMenyerang)")]
    public string namaAnimasiMenyerang = "KelelawarMenyerang";

    [Header("Debug")]
    public bool tampilkanLog = true;

    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    private void Start()
    {
        // Pastikan semua kelelawar aktif di awal
        if (daftarKelelawar != null)
        {
            foreach (var kel in daftarKelelawar)
            {
                if (kel != null)
                    kel.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Dipanggil saat jawaban BENAR → menghilangkan 1 kelelawar yang masih aktif
    /// </summary>
    public void HilangkanSatuKelelawar()
    {
        if (daftarKelelawar == null || daftarKelelawar.Length == 0)
        {
            if (tampilkanLog) Debug.LogWarning("[KELELAWAR] daftarKelelawar kosong!");
            return;
        }

        // Cari kelelawar pertama yang masih aktif
        for (int i = 0; i < daftarKelelawar.Length; i++)
        {
            if (daftarKelelawar[i] != null && daftarKelelawar[i].activeInHierarchy)
            {
                daftarKelelawar[i].SetActive(false);

                if (tampilkanLog)
                    Debug.Log($"<color=yellow>[KELELAWAR]</color> Kelelawar ke-{i + 1} berhasil dihilangkan!");

                return; // Hanya hilangkan 1 saja
            }
        }

        if (tampilkanLog)
            Debug.Log("<color=yellow>[KELELAWAR]</color> Semua kelelawar sudah hilang.");
    }

    /// <summary>
    /// Dipanggil saat jawaban SALAH → semua kelelawar yang masih hidup menyerang
    /// </summary>
    public void KelelawarMenyerang()
    {
        if (daftarKelelawar == null) return;

        // Hentikan serangan sebelumnya agar tidak menumpuk
        StopAllCoroutines();
        activeCoroutines.Clear();

        foreach (GameObject kel in daftarKelelawar)
        {
            if (kel != null && kel.activeInHierarchy)
            {
                Coroutine co = StartCoroutine(ProsesTerbangMenyerang(kel));
                activeCoroutines.Add(co);
            }
        }
    }

    private IEnumerator ProsesTerbangMenyerang(GameObject kelelawar)
    {
        if (kelelawar == null) yield break;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[KELELAWAR] Player tidak ditemukan! Pastikan objek pemain punya Tag 'Player'.");
            yield break;
        }

        Vector3 posisiAwal = kelelawar.transform.position;
        Vector3 posisiTarget = player.transform.position;

        // Mainkan animasi menyerang
        Animator anim = kelelawar.GetComponent<Animator>();
        if (anim != null && !string.IsNullOrEmpty(namaAnimasiMenyerang))
        {
            anim.Play(namaAnimasiMenyerang, 0, 0f);
        }

        // Terbang menuju player
        while (kelelawar != null && kelelawar.activeInHierarchy &&
               Vector3.Distance(kelelawar.transform.position, posisiTarget) > 0.5f)
        {
            kelelawar.transform.position = Vector3.MoveTowards(
                kelelawar.transform.position,
                posisiTarget,
                kecepatanTerbang * Time.deltaTime
            );
            yield return null;
        }

        // Sedikit jeda di depan player
        yield return new WaitForSeconds(0.25f);

        // Kembali ke posisi awal
        while (kelelawar != null && kelelawar.activeInHierarchy &&
               Vector3.Distance(kelelawar.transform.position, posisiAwal) > 0.1f)
        {
            kelelawar.transform.position = Vector3.MoveTowards(
                kelelawar.transform.position,
                posisiAwal,
                kecepatanTerbang * Time.deltaTime
            );
            yield return null;
        }

        if (kelelawar != null)
            kelelawar.transform.position = posisiAwal;
    }

    /// <summary>
    /// Dipanggil saat level selesai (semua soal benar) → semua kelelawar kabur
    /// </summary>
    public void KelelawarKaburSemua()
    {
        StopAllCoroutines();
        activeCoroutines.Clear();

        if (daftarKelelawar == null) return;

        foreach (var kel in daftarKelelawar)
        {
            if (kel != null)
                kel.SetActive(false);
        }

        if (tampilkanLog)
            Debug.Log("<color=green>[KELELAWAR]</color> Semua kelelawar kabur!");
    }

    /// <summary>
    /// Opsional: reset semua kelelawar (bisa dipanggil saat restart level)
    /// </summary>
    public void ResetKelelawar()
    {
        StopAllCoroutines();
        activeCoroutines.Clear();

        if (daftarKelelawar == null) return;

        foreach (var kel in daftarKelelawar)
        {
            if (kel != null)
                kel.SetActive(true);
        }
    }
}