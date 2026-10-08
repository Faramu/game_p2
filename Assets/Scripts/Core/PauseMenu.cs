using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;

    void Update()
    {
        // Tetap bisa pakai ESC di keyboard
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton(); // Memanggil fungsi PauseButton di bawah
        }
    }

    // FUNGSI BARU: Agar bisa dipanggil oleh tombol gambar Pause di layar
public void PauseButton() 
    {
        Debug.Log("HORE! Tombol berhasil ditekan!"); // TAMBAHKAN BARIS INI
        container.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1; // Mengembalikan waktu normal sebelum pindah scene
        SceneManager.LoadScene("MainMenuScene"); // Pastikan nama scene-nya sesuai
    }
}