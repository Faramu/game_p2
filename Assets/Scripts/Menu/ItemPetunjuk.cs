using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemPetunjuk : MonoBehaviour
{
    [Header("--- IDENTITAS STAGE ---")]
    [Tooltip("Isi 1 untuk Stage 1, 2 untuk Stage 2, dst.")]
    public int stageID = 1;

    [Tooltip("Gambar ilustrasi stage ini")]
    public Sprite gambarIlustrasi;

    private Button tombol;

    private void Awake()
    {
        tombol = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (tombol == null)
            tombol = GetComponent<Button>();

        if (tombol != null)
        {
            tombol.onClick.RemoveAllListeners();
            tombol.onClick.AddListener(SaatStageDiklik);
        }
    }

    private void OnDisable()
    {
        if (tombol != null)
            tombol.onClick.RemoveAllListeners();
    }

    private void SaatStageDiklik()
    {
        Debug.Log($"[PETUNJUK] Stage {stageID} dipilih");

        if (MenuPetunjukManager.Instance == null)
        {
            Debug.LogError("[PETUNJUK] MenuPetunjukManager.Instance tidak ditemukan!");
            return;
        }

        // Ambil data string murni berdasarkan stage ID
        string[] dialogMentah = BuatDialogHardcode(stageID);

        if (dialogMentah == null || dialogMentah.Length == 0)
        {
            Debug.LogWarning($"[PETUNJUK] Tidak ada dialog hardcode untuk Stage {stageID}");
            return;
        }

        Debug.Log($"[PETUNJUK] Memuat data Stage {stageID}");
        
        // Kirim array string dan gambar langsung ke Manager Petunjuk
        MenuPetunjukManager.Instance.FokusKeStage(stageID, dialogMentah, gambarIlustrasi, "Aki Aruna");
    }

    // =====================================================
    // HARDCODE DIALOG PER STAGE (Menggunakan Array String)
    // =====================================================
private string[] BuatDialogHardcode(int id)
{
    switch (id)
    {
        case 1:
            return new string[]
            {
                "Selamat datang di Stage 1 - Hutan Bilangan!",
                "Jelajahi hutan dan pelajari konsep bilangan bulat.",
                "Temukan bilangan positif dan negatif untuk melanjutkan petualangan."
            };

        case 2:
            return new string[]
            {
                "Selamat datang di Stage 2 - Pasar Anomali!",
                "Bantu para pedagang menghadapi kekacauan di pasar.",
                "Gunakan operasi penjumlahan dan pengurangan untuk menyelesaikan setiap tantangan."
            };

        case 3:
            return new string[]
            {
                "Selamat datang di Stage 3 - Rumah Pohon!",
                "Naiklah menuju rumah pohon dan hadapi tantangan perubahan suhu.",
                "Gunakan operasi perkalian dan pembagian untuk menjaga keseimbangan suhu."
            };

        case 4:
            return new string[]
            {
                "Selamat datang di Stage 4 - Goa Faktor!",
                "Masuki goa misterius dan temukan faktor dari setiap bilangan.",
                "Pecahkan teka-teki faktor untuk membuka jalan menuju tahap berikutnya."
            };

        case 5:
            return new string[]
            {
                "Selamat datang di Stage 5 - Jalur Markas!",
                "Kamu semakin dekat dengan markas Kerajaan Anomali.",
                "Hadapi barikade dan gunakan pemahaman tentang FPB untuk membuka jalan."
            };

        case 6:
            return new string[]
            {
                "Selamat datang di Stage 6 - Kerajaan!",
                "Inilah wilayah terakhir dalam perjalananmu, Ksatria!",
                "Hadapi tantangan terakhir dan buktikan kemampuanmu dalam memahami KPK."
            };

        default:
            return new string[]
            {
                "Selamat datang di dunia Petualangan Bilangan Bulan!"
            };
        }
    }
}