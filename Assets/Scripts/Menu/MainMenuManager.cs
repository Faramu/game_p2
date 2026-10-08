using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Fungsi untuk tombol MULAI
    public void MulaiGame()
    {
        SceneManager.LoadScene("Map_Checkpoint");
    }

    // Fungsi untuk tombol PETUNJUK
    public void PetunjukGame()
    {
        // Berpindah ke Scene khusus panduan kontrol & mekanisme matematika
        SceneManager.LoadScene("Menu_Petunjuk");
    }

    // Fungsi untuk tombol PENCAPAIAN
    public void PencapaianGame()
    {
        // Berpindah ke Scene rekam jejak skor, koin, dan retensi belajar siswa
        SceneManager.LoadScene("Menu_Pencapaian");
    }

    // Fungsi untuk tombol KELUAR
    public void KeluarGame()
    {
        Debug.Log("Game Keluar"); // Hanya muncul di editor saat testing
        Application.Quit(); // Fungsi untuk menutup aplikasi asli (saat di-build)
    }
}