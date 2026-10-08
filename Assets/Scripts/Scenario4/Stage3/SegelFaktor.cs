using UnityEngine;
using System.Collections;

public class SegelFaktor : MonoBehaviour
{
    [Header("Referensi Visual Altar")]
    public GameObject batuAltar;
    public GameObject lingkaranSihir;
    public GameObject cahayaSegel;
    public ParticleSystem particleEnergy;

    [Header("UI Indikator")]
    public GameObject indikatorTekanE; 

    [Header("Referensi Eksternal")]
    public DindingAkhir32 dindingAkhir; 

    private bool sudahSelesai = false;
    private bool playerDiDekat = false;

    private void Start()
    {
        if (lingkaranSihir) lingkaranSihir.SetActive(false);
        if (cahayaSegel) cahayaSegel.SetActive(false);
        if (particleEnergy) particleEnergy.Stop();
        
        if (indikatorTekanE) indikatorTekanE.SetActive(false);
    }

    private void Update()
    {
        if (playerDiDekat && !sudahSelesai && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("<color=cyan>[SEGEL]</color> Tombol E ditekan!");
            
            if (indikatorTekanE) indikatorTekanE.SetActive(false);
            
            if (Stage4Level3Manager.Instance != null)
            {
                Stage4Level3Manager.Instance.BukaPanelKuis();
            }
            else
            {
                Debug.LogWarning("Stage4Level3Manager tidak ditemukan di Scene!");
            }
        }
    }

    public void AktifkanSihirSegel()
    {
        sudahSelesai = true;
        StartCoroutine(ProsesHancurkanDinding());
    }

    public void BatuMemantulKePlayer()
    {
        Debug.Log("<color=orange>[SEGEL]</color> Energi gagal! Batu memantul ke arah Player!");
    }

    private IEnumerator ProsesHancurkanDinding()
    {
        if (lingkaranSihir) lingkaranSihir.SetActive(true);
        if (cahayaSegel) cahayaSegel.SetActive(true);
        if (particleEnergy) particleEnergy.Play();

        yield return new WaitForSeconds(1.5f);
        if (batuAltar) batuAltar.SetActive(false);
        
        yield return new WaitForSeconds(0.5f);
        if (dindingAkhir) dindingAkhir.HancurkanDindingRaksasa();
    }

    // --- RADAR FISIKA ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("<color=yellow>[RADAR]</color> Ada objek menyentuh segel: " + other.name + " | Tag: " + other.tag);
        
        if (other.CompareTag("Player") && !sudahSelesai) 
        {
            playerDiDekat = true;
            if (indikatorTekanE) indikatorTekanE.SetActive(true);
            Debug.Log("<color=green>[SEGEL]</color> Ksatria siap memecahkan segel!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            playerDiDekat = false;
            if (indikatorTekanE) indikatorTekanE.SetActive(false);
        }
    }
}