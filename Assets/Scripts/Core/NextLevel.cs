using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [Tooltip("Nama scene berikutnya (opsional/legacy, alur perpindahan level sekarang ditentukan dinamis oleh WinManager)")]
    public string nextLevelName;

    private bool sudahDiklik = false;

    private void OnEnable()
    {
        sudahDiklik = false;
        // Saat tombol/panel WinUI aktif, nonaktifkan seluruh panel lain di layar
        Transform winUIRoot = transform.parent != null ? transform.parent : transform;
        Flag.NonaktifkanSemuaPanelLain(winUIRoot.gameObject);
    }

    public void LoadNextLevel()
    {
        if (sudahDiklik) return;
        sudahDiklik = true;

        // Alur perpindahan dinamis sesuai aturan:
        // Jika masih ada sublevel -> sublevel berikutnya
        // Jika sublevel terakhir / tanpa sublevel -> Map_Checkpoint
        WinManager.LanjutSetelahWin();
    }
}
