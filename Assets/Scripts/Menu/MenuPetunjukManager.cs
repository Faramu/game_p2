using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using System;

public class MenuPetunjukManager : MonoBehaviour
{
    public static MenuPetunjukManager Instance;

    [Header("--- REFERENSI UI ---")]
    public GameObject panelBlur; 
    
    [Tooltip("Slot Image di Canvas untuk menampilkan ilustrasi stage")]
    public Image slotIlustrasiStage; 

    [Tooltip("Semua objek arrow yang berada di belakang gambar stage")]
    public GameObject[] arrowStage;

    [Header("--- PENGATURAN SCENE ---")]
    [Tooltip("Isi dengan nama Scene Main Menu sesuai yang ada di Build Settings")]
    public string namaSceneMainMenu = "MainMenuScene"; 

    [Tooltip("Isi dengan nama Scene Map Checkpoint sesuai yang ada di Build Settings")]
    public string namaSceneMapCheckpoint = "Map_Checkpoint";

    private int activeStageID = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        TutupSemuaPanelPetunjuk();
    }

    public void FokusKeStage(int stageID, string[] dialogMentah, Sprite gambarIlustrasi, string namaNPC)
    {
        if (DialogManager.Instance == null)
        {
            Debug.LogError("DialogManager.Instance tidak ditemukan!");
            return;
        }

        activeStageID = stageID;

        // Konversi array string menjadi struktur BarisDialog
        BarisDialog[] dialogTerkonversi = new BarisDialog[dialogMentah.Length];
        for (int i = 0; i < dialogMentah.Length; i++)
        {
            dialogTerkonversi[i] = new BarisDialog 
            { 
                namaPembicara = namaNPC, 
                kalimat = dialogMentah[i] 
            };
        }

        if (panelBlur != null) panelBlur.SetActive(true);
        AturArrowStage(true);

        if (slotIlustrasiStage != null && gambarIlustrasi != null)
        {
            slotIlustrasiStage.sprite = gambarIlustrasi; 
            slotIlustrasiStage.gameObject.SetActive(true); 
        }

        // Jalankan dialog. Tombol Lanjut digunakan antar kalimat; callback dipanggil setelah dialog terakhir selesai.
        DialogManager.Instance.TampilkanDialog(dialogTerkonversi, SaatDialogSelesai);
    }

    // Overload untuk kompatibilitas jika ada pemanggilan lama
    public void FokusKeStage(string[] dialogMentah, Sprite gambarIlustrasi, string namaNPC)
    {
        FokusKeStage(1, dialogMentah, gambarIlustrasi, namaNPC);
    }

    private void SaatDialogSelesai()
    {
        Debug.Log($"[PETUNJUK] Dialog Stage {activeStageID} selesai");
        Debug.Log($"[PETUNJUK] Target Map = Stage {activeStageID}");
        Debug.Log("[PETUNJUK] Membuka Map_Checkpoint");

        TutupSemuaPanelPetunjuk();

        // Simpan target ke PlayerMarker dan PlayerPrefs sebelum pindah scene
        PlayerMarker.StagePetunjukTujuan = activeStageID;
        PlayerPrefs.SetInt("StagePetunjukTujuan", activeStageID);
        PlayerPrefs.Save();
        PlayerMarker.hasLoggedMapTarget = false;

        if (!string.IsNullOrEmpty(namaSceneMapCheckpoint))
        {
            SceneManager.LoadScene(namaSceneMapCheckpoint);
        }
        else
        {
            Debug.LogError("[PETUNJUK] Nama Scene Map Checkpoint belum diisi di Inspector!");
        }
    }

    public void TutupSemuaPanelPetunjuk()
    {
        if (panelBlur != null) panelBlur.SetActive(false);
        if (slotIlustrasiStage != null) slotIlustrasiStage.gameObject.SetActive(false);
        AturArrowStage(false);
    }

    private void AturArrowStage(bool aktif)
    {
        if (arrowStage == null) return;
        foreach (GameObject arrow in arrowStage)
        {
            if (arrow != null) arrow.SetActive(aktif);
        }
    }

    public void KembaliKeMainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(namaSceneMainMenu))
        {
            SceneManager.LoadScene(namaSceneMainMenu);
        }
        else
        {
            Debug.LogError("Nama Scene Main Menu belum diisi di Inspector!");
        }
    }
}