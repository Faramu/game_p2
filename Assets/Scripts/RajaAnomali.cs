using UnityEngine;
using System.Collections;
using System;

public class RajaAnomali : MonoBehaviour
{
    [Header("--- ANIMATOR ---")]
    public Animator animator;

    [Header("--- NAMA ANIMASI ---")]
    public string animMembuatPerisai = "Raja_Membuat_Perisai";
    public string animIdle = "Raja_Idle";
    public string animPerisaiPecah = "Raja_Perisai_Pecah";

    [Header("--- PENGATURAN ---")]
    public float durasiAnimasiMembuatPerisai = 1.2f;
    public float durasiAnimasiPerisaiPecah = 1.0f;

    private bool sedangAnimasi = false;
    private bool sudahMulaiPerisai = false;

    // Callback yang dipanggil setelah animasi membuat perisai selesai
    public Action OnPerisaiSelesai;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Saat baru masuk game / dialog masih berjalan → matikan animator (tidak ada animasi)
        if (animator != null)
            animator.enabled = false;
    }

    // =========================================================
    // DIPANGGIL SETELAH DIALOG SELESAI
    // =========================================================
    public void MulaiAnimasiPerisai()
    {
        Debug.Log("<color=yellow>MulaiAnimasiPerisai() dipanggil</color>");

        if (sudahMulaiPerisai)
        {
            Debug.LogWarning("Animasi membuat perisai sudah pernah dijalankan");
            OnPerisaiSelesai?.Invoke();
            return;
        }
        sudahMulaiPerisai = true;

        if (animator == null)
        {
            Debug.LogError("Animator masih kosong di RajaAnomali!");
            OnPerisaiSelesai?.Invoke();
            return;
        }

        StartCoroutine(JalankanAnimasiMembuatPerisai());
    }

    private IEnumerator JalankanAnimasiMembuatPerisai()
    {
        sedangAnimasi = true;
        Debug.Log("<color=cyan>Menyalakan Animator & memainkan Raja_Membuat_Perisai</color>");

        // 1. Nyalakan Animator
        animator.enabled = true;

        // 2. Tunggu 1 frame agar Animator siap
        yield return null;

        // 3. Mainkan animasi membuat perisai
        animator.Play(animMembuatPerisai, 0, 0f);

        // 4. Tunggu sampai animasi selesai
        yield return new WaitForSeconds(durasiAnimasiMembuatPerisai);

        Debug.Log("<color=cyan>Animasi Membuat Perisai selesai → pindah ke Idle</color>");

        // 5. Setelah selesai, masuk ke Idle (selama soal berjalan)
        animator.Play(animIdle, 0, 0f);
        sedangAnimasi = false;

        // 6. Beritahu Manager bahwa perisai sudah siap, boleh mulai soal
        OnPerisaiSelesai?.Invoke();
    }

    // =========================================================
    // KENA DAMAGE (tembakan biasa selama soal masih berjalan)
    // =========================================================
    public void KenaDamage()
    {
        // Selama soal masih berjalan, cukup tetap di Idle
        // (kalau nanti kamu punya animasi kena hit biasa, bisa ditambahkan di sini)
        if (animator != null && animator.enabled && !sedangAnimasi)
        {
            animator.Play(animIdle, 0, 0f);
        }
    }

    // =========================================================
    // KALAH (tembakan terakhir setelah semua soal selesai)
    // =========================================================
    public void Kalah()
    {
        StopAllCoroutines();
        sedangAnimasi = true;

        if (animator != null)
        {
            animator.enabled = true;
            Debug.Log("<color=red>Raja kalah → memainkan Raja_Perisai_Pecah</color>");
            animator.Play(animPerisaiPecah, 0, 0f);
        }
    }

    // =========================================================
    // OPSIONAL: Kalau ingin animasi pecah juga bisa dipanggil manual
    // =========================================================
    public void MainkanPerisaiPecah()
    {
        if (animator == null) return;

        StopAllCoroutines();
        sedangAnimasi = true;
        animator.enabled = true;
        animator.Play(animPerisaiPecah, 0, 0f);
    }
}