using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckpointTrigger : MonoBehaviour
{
    [Tooltip("Offset respawn di checkpoint ini")]
    public Vector3 respawnOffset = Vector3.zero;

    private bool sudahAktif = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !sudahAktif)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.SetCheckpoint(transform.position + respawnOffset);
                sudahAktif = true;
                Debug.Log($"[CHECKPOINT] Trigger diaktifkan di posisi {transform.position}");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = sudahAktif ? Color.green : Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 2f, 0.1f));
        Gizmos.DrawSphere(transform.position + respawnOffset, 0.2f);
    }
}
