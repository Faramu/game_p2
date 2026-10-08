using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    public enum TipeLevel
    {
        Level1,
        Level2,
        Level3
    }

    [Header("--- PILIH LEVEL ---")]
    [Tooltip("Pilih level mana yang akan dipanggil oleh trigger ini")]
    public TipeLevel levelYangDipanggil = TipeLevel.Level1;

    [Header("--- REFERENSI MANAGER ---")]
    [Tooltip("Tarik GameObject yang memiliki script Level1_FPBManager")]
    public Level1_FPBManager level1Manager;

    [Tooltip("Tarik GameObject yang memiliki script Level2_FPBManager")]
    public Level2_FPBManager level2Manager;

    [Tooltip("Tarik GameObject yang memiliki script Level3_FPBManager")]
    public Level3_FPBManager level3Manager;

    [Header("--- PENGATURAN TRIGGER ---")]
    [Tooltip("Tag dari objek player (biasanya 'Player')")]
    public string tagPlayer = "Player";

    [Tooltip("Apakah trigger ini hanya bisa digunakan sekali?")]
    public bool hanyaSekali = true;

    private bool sudahDisentuh = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sudahDisentuh && hanyaSekali) return;

        if (!collision.CompareTag(tagPlayer)) return;

        bool berhasilDipanggil = false;

        switch (levelYangDipanggil)
        {
            case TipeLevel.Level1:
                if (level1Manager != null)
                {
                    level1Manager.MulaiTutorial();
                    berhasilDipanggil = true;
                    Debug.Log("[LevelTrigger] Memanggil Level1_FPBManager");
                }
                else
                {
                    Debug.LogWarning("[LevelTrigger] Level1_FPBManager belum di-assign di Inspector!");
                }
                break;

            case TipeLevel.Level2:
                if (level2Manager != null)
                {
                    level2Manager.MulaiTutorial();
                    berhasilDipanggil = true;
                    Debug.Log("[LevelTrigger] Memanggil Level2_FPBManager");
                }
                else
                {
                    Debug.LogWarning("[LevelTrigger] Level2_FPBManager belum di-assign di Inspector!");
                }
                break;

            case TipeLevel.Level3:
                // Nanti jika sudah ada Level3_FPBManager, aktifkan baris di bawah:
                if (level3Manager != null)
                {
                    level3Manager.MulaiTutorial();
                    berhasilDipanggil = true;
                }
                else
                {
                    Debug.LogWarning("[LevelTrigger] Level3_FPBManager belum di-assign di Inspector!");
                }
                Debug.LogWarning("[LevelTrigger] Level 3 belum diimplementasikan!");
                break;
        }

        if (berhasilDipanggil)
        {
            sudahDisentuh = true;

            // Nonaktifkan collider agar tidak memicu ulang
            if (hanyaSekali)
            {
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }
        }
    }
}