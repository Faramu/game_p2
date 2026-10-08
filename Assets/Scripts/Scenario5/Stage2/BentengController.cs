using UnityEngine;
using System.Collections;
using TMPro;

public class BentengController : MonoBehaviour
{
    [Header("Visual")]
    public GameObject visualClosed;
    public GameObject visualOpen;

    [Header("Pertahanan")]
    public Collider2D colliderPertahanan;
    public Transform playerStandPoint;

    [Header("Titik Muncul Tembok")]
    [Tooltip("Drag child Titik_Muncul_Benteng milik benteng ini")]
    public Transform titikMunculTembok;

    [Header("Efek Hancur / Ledakan")]
    [Tooltip("Prefab ledakan saat benteng hancur")]
    public GameObject prefabLedakanBenteng;

    [Header("Status")]
    public bool isCorrectBenteng = false;

    private bool isOpen = false;
    private bool sudahHancur = false;

    void Start()
    {
        SetClosed();
    }

    public void SetClosed()
    {
        isOpen = false;
        sudahHancur = false;
        isCorrectBenteng = false;

        if (visualClosed != null) visualClosed.SetActive(true);
        if (visualOpen != null) visualOpen.SetActive(false);
        if (colliderPertahanan != null) colliderPertahanan.enabled = false;

        // Aktifkan kembali semua teks di dalam benteng
        var semuaTeks = GetComponentsInChildren<TMP_Text>(true);
        foreach (var teks in semuaTeks)
        {
            if (teks != null) teks.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
    }

    public void Open(bool asCorrect)
    {
        if (sudahHancur) return;

        isCorrectBenteng = asCorrect;
        isOpen = true;

        if (visualClosed != null) visualClosed.SetActive(false);
        if (visualOpen != null) visualOpen.SetActive(true);
        if (colliderPertahanan != null) colliderPertahanan.enabled = true;

        Debug.Log($"[Benteng] {gameObject.name} OPEN | Correct = {asCorrect}");
    }

    public Vector3 GetPlayerPosition()
    {
        return playerStandPoint != null ? playerStandPoint.position : transform.position;
    }

    public Vector3 GetTitikMunculTembok()
    {
        return titikMunculTembok != null ? titikMunculTembok.position : transform.position;
    }

    public void OnHitByProjectile(ProjectileMeriam projectile)
    {
        if (!isOpen || sudahHancur) return;

        if (isCorrectBenteng)
        {
            Debug.Log($"[Benteng] {gameObject.name} MENAHAN serangan!");
            projectile.OnBlockedByBenteng(this);
        }
        else
        {
            Debug.Log($"[Benteng] {gameObject.name} GAGAL menahan!");
            projectile.OnFailedByBenteng(this);
            HancurkanBenteng();
        }
    }

    public void HancurkanBenteng()
    {
        KenaPeluruDanMeledak(null);
    }

    public void KenaPeluruDanMeledak()
    {
        KenaPeluruDanMeledak(null);
    }

    /// <summary>
    /// Hancurkan benteng. Bisa diberi prefab ledakan cadangan dari Manager.
    /// </summary>
    public void KenaPeluruDanMeledak(GameObject prefabLedakanFallback)
    {
        if (sudahHancur) return;
        sudahHancur = true;

        if (colliderPertahanan != null)
            colliderPertahanan.enabled = false;

        StartCoroutine(AnimasiHancur(prefabLedakanFallback));
    }

    private IEnumerator AnimasiHancur(GameObject prefabLedakanFallback)
    {
        GameObject prefabLedakan = prefabLedakanBenteng != null
            ? prefabLedakanBenteng
            : prefabLedakanFallback;

        if (prefabLedakan != null)
        {
            GameObject ledakan = Instantiate(prefabLedakan, transform.position, Quaternion.identity);
            ledakan.SetActive(true);
            Destroy(ledakan, 2f);
            Debug.Log($"<color=yellow>[LEDAKAN]</color> {gameObject.name} meledak di {transform.position}");
        }
        else
        {
            Debug.LogError($"[Benteng] {gameObject.name} TIDAK ADA PREFAB LEDAKAN!");
        }

        // Tunggu agar ledakan sempat terlihat
        yield return new WaitForSeconds(0.45f);

        if (visualClosed != null) visualClosed.SetActive(false);
        if (visualOpen != null) visualOpen.SetActive(false);

        var semuaTeks = GetComponentsInChildren<TMP_Text>(true);
        foreach (var teks in semuaTeks)
        {
            if (teks != null) teks.gameObject.SetActive(false);
        }

        Debug.Log($"[Benteng] {gameObject.name} MELEDAK lalu menghilang");
    }
}