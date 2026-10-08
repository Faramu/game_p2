using UnityEngine;

public class TriggerAreaTengah : MonoBehaviour
{
    [Header("Referensi Manager")]
    public TebingKabutManager manager;

    private bool sudahDipicu = false;

    private void Awake()
    {
        sudahDipicu = false;
        if (manager == null)
        {
            manager = Object.FindFirstObjectByType<TebingKabutManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CobaPicuTrigger(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!sudahDipicu)
        {
            CobaPicuTrigger(collision);
        }
    }

    private void CobaPicuTrigger(Collider2D collision)
    {
        if (sudahDipicu) return;
        if (!collision.CompareTag("Player")) return;

        if (manager == null)
        {
            manager = Object.FindFirstObjectByType<TebingKabutManager>();
        }

        if (manager != null)
        {
            Debug.Log("[STAGE1-LV2 TRIGGER]\nPlayer masuk area soal.");

            bool berhasil = manager.PemicuTengahDiinjak();
            if (berhasil)
            {
                sudahDipicu = true;
            }
        }
        else
        {
            Debug.LogWarning("<color=red>[STAGE1-LV2 TRIGGER] TebingKabutManager belum dihubungkan pada TriggerAreaTengah!</color>");
        }
    }

    public void ResetTrigger()
    {
        sudahDipicu = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.enabled)
        {
            col.enabled = true;
        }
    }
}