using UnityEngine;

public class DindingGoa : MonoBehaviour
{
    [Header("Komponen Utama")]
    public Collider2D hitboxCollider;
    
    [Header("Efek Saat Dihantam Batu")]
    public ParticleSystem vfxLedakan;
    public AudioSource audioLedakan;

    [Header("Objek Dinding Bertahap (Dari Utuh ke Parah)")]
    [Tooltip("Masukkan 4 objek dari Hierarchy: 0=Utuh, 1=Retak Dikit, 2=Retak Sedang, 3=Parah")]
    public GameObject[] objekTahapKerusakan; 

    [Header("Pengaturan Hit")]
    public int totalHitDibutuhkan = 5; 
    private int hitSaatIni = 0;

    private void Start()
    {
        // Pastikan hanya objek dinding utuh (index 0) yang menyala di awal permainan
        UpdateVisualDinding(0);
    }

    public void TerimaHantamanBatu()
    {
        hitSaatIni++;

        // 1. Paksa Nyalakan Ledakan
        if (vfxLedakan != null) 
        {
            vfxLedakan.gameObject.SetActive(true);
            vfxLedakan.Play();
        }
        if (audioLedakan != null) audioLedakan.Play();

        // 2. Ganti Objek Dinding (Efek Retak)
        int indeksObjek = hitSaatIni - 1; 
        UpdateVisualDinding(indeksObjek);

        // 3. Jika sudah 5 kali kena hit (Hancur Total)
        if (hitSaatIni >= totalHitDibutuhkan)
        {
            if (hitboxCollider != null) hitboxCollider.enabled = false;
            
            // Matikan SEMUA visual dinding (karena sudah hancur)
            UpdateVisualDinding(-1); 
            
            // Matikan juga angka 32 di dinding saat hancur
            if (GoaFaktorManager.Instance != null && GoaFaktorManager.Instance.teksAngkaDindingUtama != null)
            {
                GoaFaktorManager.Instance.teksAngkaDindingUtama.gameObject.SetActive(false);
            }
            
            Debug.Log("Dinding 32 Berhasil Dihancurkan Sepenuhnya!");
        }
    }

    // Fungsi otomatis untuk menyalakan 1 objek dan mematikan sisanya
    private void UpdateVisualDinding(int indeksAktif)
    {
        for (int i = 0; i < objekTahapKerusakan.Length; i++)
        {
            if (objekTahapKerusakan[i] != null)
            {
                // SetActive(true) jika urutannya pas, SetActive(false) jika bukan gilirannya
                objekTahapKerusakan[i].SetActive(i == indeksAktif);
            }
        }
    }

    // Fungsi pembersih debu saat soal baru muncul
    public void HentikanEfekDebu()
    {
        if (vfxLedakan != null)
        {
            vfxLedakan.Stop(); 
            vfxLedakan.Clear(); 
            vfxLedakan.gameObject.SetActive(false); 
        }
    }
}