using UnityEngine;

public class PutriAnomali : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Nama Animasi")]
    public string idleAnim = "NPC_Putri";
    public string jalanAnim = "NPC_PutriJalan";

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        PlayIdle();
    }

    public void PlayIdle()
    {
        if (animator == null) return;
        animator.Play(idleAnim);
        Debug.Log("<color=magenta>Putri → Idle</color>");
    }

    public void PlayJalan()
    {
        if (animator == null) return;
        animator.Play(jalanAnim);
        Debug.Log("<color=magenta>Putri → Jalan</color>");
    }

    public void SampaiPortal()
    {
        PlayIdle();
        Debug.Log("<color=magenta>Putri sampai di portal</color>");
    }
}