using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI Components")]
    [SerializeField] private GameObject panelTutorial;
    [SerializeField] private Image visualDisplay; 

    [Header("Tutorial Data")]
    [SerializeField] private List<TutorialSlide> tutorialSlides;

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelTutorial != null) panelTutorial.SetActive(false);
    }

    public void StartTutorial()
    {
        if (tutorialSlides.Count == 0) return;
        
        currentIndex = 0;
        panelTutorial.SetActive(true);
        Time.timeScale = 0f; 
        DisplaySlide(currentIndex);
    }

    public void NextSlide()
    {
        currentIndex++;
        if (currentIndex < tutorialSlides.Count)
        {
            DisplaySlide(currentIndex);
        }
        else
        {
            CloseTutorial();
        }
    }

    public void PrevSlide()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            DisplaySlide(currentIndex);
        }
    }

    private void DisplaySlide(int index)
    {
        TutorialSlide slide = tutorialSlides[index];
        
        if (visualDisplay != null && slide.visualAsset != null)
        {
            visualDisplay.sprite = slide.visualAsset;
        }
    }

    // =========================================================================
    // PERBAIKAN UTAMA: DETEKSI OTOMATIS STAGE 1 DAN STAGE 2
    // =========================================================================
    public void CloseTutorial()
    {
        Debug.Log("<color=yellow>1. Fungsi CloseTutorial berhasil terpanggil!</color>");
        
        panelTutorial.SetActive(false);
        Time.timeScale = 1f; // Kembalikan waktu game ke normal

        Debug.Log("<color=yellow>2. Mencoba mendeteksi MiniGame di Scene ini...</color>");
        
        // --- CEK STAGE 1 ---
        MiniGamePasarManager miniGameStage1 = Object.FindFirstObjectByType<MiniGamePasarManager>();
        if (miniGameStage1 != null)
        {
            Debug.Log("<color=green>3. [STAGE 1] MiniGamePasarManager ditemukan! Memulai Minigame...</color>");
            miniGameStage1.MulaiMiniGame();
            return; // Hentikan fungsi di sini agar tidak lanjut ke bawah
        }

        // --- CEK STAGE 2 ---
        MiniGamePenjualManager miniGameStage2 = Object.FindFirstObjectByType<MiniGamePenjualManager>();
        if (miniGameStage2 != null)
        {
            Debug.Log("<color=green>3. [STAGE 2] MiniGamePenjualManager ditemukan! Memulai Kuis Ketik Angka...</color>");
            miniGameStage2.MulaiMiniGame();
            return; // Hentikan fungsi di sini
        }

        // --- JIKA TIDAK ADA MINIGAME ---
        Debug.Log("<color=orange>Tutorial ditutup. (Tidak ada MiniGame yang terhubung di Scene ini).</color>");
    }
}