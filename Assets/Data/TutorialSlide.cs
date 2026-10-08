using UnityEngine;

// Baris di bawah inilah yang menciptakan menu "Tutorial/Slide" saat Anda klik kanan
[CreateAssetMenu(fileName = "NewTutorialSlide", menuName = "Tutorial/Slide")]
public class TutorialSlide : ScriptableObject
{
    public string judul;
    [TextArea] public string isiPenjelasan;
    public Sprite visualAsset; 
}