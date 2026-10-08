using UnityEngine;
using System.Collections;

public class ProjectileMeriam : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 5f;

    private Vector3 targetPos;
    private bool isMoving = false;
    
    // UBAHAN 1: Ganti Level1_FPBManager menjadi MonoBehaviour agar fleksibel
    private MonoBehaviour managerAktif; 
    private bool sudahHit = false;

    // UBAHAN 2: Parameter diganti menjadi MonoBehaviour
    public void Init(Vector3 target, MonoBehaviour mgr)
    {
        targetPos = target;
        managerAktif = mgr;
        isMoving = true;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!isMoving || sudahHit) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.15f)
        {
            // Sudah sampai target tapi belum kena collider → anggap miss / langsung proses
            isMoving = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (sudahHit) return;

        // Cek apakah kena Benteng
        BentengController benteng = other.GetComponent<BentengController>();
        if (benteng == null)
            benteng = other.GetComponentInParent<BentengController>();

        if (benteng != null)
        {
            sudahHit = true;
            isMoving = false;
            benteng.OnHitByProjectile(this);
            return;
        }

        // Cek apakah kena Player
        if (other.CompareTag("Player"))
        {
            sudahHit = true;
            isMoving = false;
            
            // UBAHAN 3: Cek tipe manager saat terkena player
            if (managerAktif is Level1_FPBManager lvl1)
            {
                lvl1.OnProjectileHitPlayer(this);
            }
            else if (managerAktif is Level2_FPBManager lvl2)
            {
                lvl2.OnProjectileHitPlayer(this);
            }
            // ---> TAMBAHAN LEVEL 3
            else if (managerAktif is Level3_FPBManager lvl3)
            {
                lvl3.OnProjectileHitPlayer(this);
            }
            
            Destroy(gameObject);
        }
    }

    public void OnBlockedByBenteng(BentengController benteng)
    {
        // Efek benturan (tambah VFX di sini jika ada)
        Debug.Log("[Projectile] Diblokir benteng kuat");
        
        // Cek tipe manager saat diblokir benteng
        if (managerAktif is Level1_FPBManager lvl1)
        {
            lvl1.OnProjectileBlocked(this, benteng);
        }
        else if (managerAktif is Level2_FPBManager lvl2)
        {
            lvl2.OnProjectileBlocked(this, benteng);
        }
        // ---> TAMBAHAN LEVEL 3
        else if (managerAktif is Level3_FPBManager lvl3)
        {
            lvl3.OnProjectileBlocked(this, benteng);
        }

        Destroy(gameObject);
    }

    public void OnFailedByBenteng(BentengController benteng)
    {
        // Benteng gagal → biarkan peluru lanjut ke player atau langsung damage
        Debug.Log("[Projectile] Benteng gagal, lanjut ke player");
        
        // Cek tipe manager saat benteng gagal
        if (managerAktif is Level1_FPBManager lvl1)
        {
            lvl1.OnProjectileFailed(this, benteng);
            lvl1.OnProjectileHitPlayer(this); // Opsi 1: langsung damage
        }
        else if (managerAktif is Level2_FPBManager lvl2)
        {
            lvl2.OnProjectileFailed(this, benteng);
            lvl2.OnProjectileHitPlayer(this); // Opsi 1: langsung damage
        }
        // ---> TAMBAHAN LEVEL 3
        else if (managerAktif is Level3_FPBManager lvl3)
        {
            // Level 3 langsung memberikan damage ke player jika gagal
            lvl3.OnProjectileHitPlayer(this); 
        }

        Destroy(gameObject);
    }
}