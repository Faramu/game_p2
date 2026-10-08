using UnityEngine;

public class NPCInteractionStage2 : MonoBehaviour
{
    private bool playerInRange = false;

    private void Update()
    {
        // Jika player berada di area NPC dan menekan tombol E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Jalankan alur skenario pasar
            Skenario2Manager.Instance.MulaiSkenarioPasar();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerInRange = false;
    }
}