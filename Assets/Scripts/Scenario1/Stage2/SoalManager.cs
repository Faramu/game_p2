using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SoalManager : MonoBehaviour
{
    [System.Serializable]
    public struct Soal {
        [TextArea(2, 4)] public string teksSoal;
        public bool jawabanPositif; // true = Hijau, false = Ungu
    }

    [Header("Pengaturan Soal")]
    public List<Soal> bankSoal;
    private Soal soalSaatIni;
    private int streakBenar = 0;

    [Header("UI Dialog Awal")]
    public GameObject panelDialog;
    public TextMeshProUGUI textDialog;

    [Header("Referensi Objek (TANPA PREFAB)")]
    public Player playerScript; 
    public GameObject meteorDiLayar; // Kolom untuk objek Batu_Soal
    public TMP_Text teksPadaMeteor;  // Kolom untuk Teks yang ada di dalam Batu_Soal
    public Transform spawnPoint;
    public GameObject jembatanTali;

    private bool kuisMulai = false;

    void Start() {
        if (jembatanTali != null) jembatanTali.SetActive(false);
        
        // Sembunyikan meteor di detik pertama game mulai
        if (meteorDiLayar != null) meteorDiLayar.SetActive(false);
        
        if (panelDialog != null) panelDialog.SetActive(true);
        if (playerScript != null) playerScript.canMove = false; 

        if (textDialog != null) {
            textDialog.text = "Tebing Kabut ada di depanmu!\nBerjalanlah ke TENGAH (antara platform Hijau dan Ungu) untuk memanggil batu ujian.\n(Klik PAHAM jika sudah siap!)";
        }
    }

    public void TombolPaham() {
        if (panelDialog != null) panelDialog.SetActive(false);
        if (playerScript != null) playerScript.canMove = true; 
    }

    public void PemicuTengahDiinjak() {
        kuisMulai = true; 
        MunculkanSoal();  
    }

    public void MunculkanSoal() {
        if (!kuisMulai) return; 
        if (bankSoal == null || bankSoal.Count == 0 || meteorDiLayar == null || teksPadaMeteor == null) return;

        // 1. Pilih soal acak
        soalSaatIni = bankSoal[Random.Range(0, bankSoal.Count)];
        
        // 2. Langsung timpa teksnya (tanpa perlu mencari komponen lagi)
        teksPadaMeteor.text = soalSaatIni.teksSoal;

        // 3. Pindahkan batu tersebut ke titik atas (Spawn Point)
        meteorDiLayar.transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);

        // 4. Munculkan (nyalakan) kembali batunya
        meteorDiLayar.SetActive(true);
    }

    public void CekJawaban(bool pemainMemilihPositif) {
        if (!kuisMulai) return; 

        if (pemainMemilihPositif == soalSaatIni.jawabanPositif) {
            Debug.Log("JAWABAN BENAR!");
            streakBenar++;
            
            if (streakBenar >= 3) {
                if (jembatanTali != null) jembatanTali.SetActive(true);
                meteorDiLayar.SetActive(false); // Sembunyikan batunya setelah menang
                kuisMulai = false; 
            } else {
                // Sembunyikan sejenak, lalu munculkan soal baru
                meteorDiLayar.SetActive(false);
                MunculkanSoal(); 
            }
        } else {
            Debug.Log("JAWABAN SALAH!");
            Player player = playerScript != null ? playerScript : Object.FindFirstObjectByType<Player>();
            if (player != null)
            {
                player.KurangiHealthSatu();
            }
            streakBenar = 0; 
            meteorDiLayar.SetActive(false);
            MunculkanSoal();
        }
    }
}