using UnityEngine;
using TMPro;

// Extension method untuk debug
public static class TransformExtensions
{
    public static string GetPath(this Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}

public class BatuMantraController : MonoBehaviour
{
    [Header("Referensi UI")]
    public TMP_Text teksNarasi;
    public TMP_Text teksNotasi;
    public GameObject efekLedakanApi;
    public AudioClip suaraLedakan;

    [Header("Pengaturan")]
    public float batasWaktu = 10f;
    public float kecepatanMengejar = 12f;
    public float tinggiMengambang = 0.5f; // Efek naik-turun saat diam (tetap di posisi)
    public float kecepatanMengambang = 3f; // Kecepatan naik-turun
    public int damageMantra = 1;

    private float timer;
    private Transform playerTarget;
    private TebingKabutManager manager;
    private bool sedangMengejar = false;
    private bool isReady = false; 
    private bool sudahSelesai = false; 
    private Vector3 posisiAwal; // Menyimpan posisi asal agar tidak drifting

    void Start()
    {
        Debug.Log("<color=yellow>Meteor Spawned! Menunggu siap...</color>");
        Invoke("SetReady", 0.5f); 
    }

    void SetReady() { isReady = true; }

    public void SetupBatuSoal(string narasi, string notasi, Transform player, TebingKabutManager mgr)
    {
        manager = mgr;
        playerTarget = player;
        timer = batasWaktu;
        sedangMengejar = false;
        sudahSelesai = false; 
        
        // Simpan posisi awal saat meteor baru muncul
        posisiAwal = transform.position;
        
        // Debug meteor structure
        Debug.Log($"<color=yellow>===== METEOR STRUCTURE DEBUG =====\nMeteor: {name}, Active: {gameObject.activeSelf}, Position: {transform.position}</color>");

        // Cari text components dengan berbagai kemungkinan path
        if (teksNarasi == null)
        {
            // Coba path 1: Canvas/Narasi
            Transform narasiTransform = transform.Find("Canvas/Narasi");
            if (narasiTransform != null) {
                teksNarasi = narasiTransform.GetComponent<TMP_Text>();
                Debug.Log("<color=green>Narasi ditemukan di: Canvas/Narasi</color>");
            }
            
            // Coba path 2: Canvas (jika Narasi langsung child)
            if (teksNarasi == null) {
                narasiTransform = transform.Find("Canvas").Find("Narasi");
                if (narasiTransform != null) {
                    teksNarasi = narasiTransform.GetComponent<TMP_Text>();
                    Debug.Log("<color=green>Narasi ditemukan di: Canvas->Narasi</color>");
                }
            }
            
            // Coba path 3: Cari di seluruh child
            if (teksNarasi == null) {
                TMP_Text[] allTexts = GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text txt in allTexts) {
                    if (txt.name == "Narasi" || txt.name.Contains("Narasi")) {
                        teksNarasi = txt;
                        Debug.Log("<color=green>Narasi ditemukan di: " + txt.transform.GetPath() + "</color>");
                        break;
                    }
                }
            }
            
            if (teksNarasi == null) {
                Debug.LogError("<color=red>GAGAL menemukan Narasi component!</color>");
            }
        }

        if (teksNotasi == null)
        {
            // Coba path 1: Canvas/Notasi
            Transform notasiTransform = transform.Find("Canvas/Notasi");
            if (notasiTransform != null) {
                teksNotasi = notasiTransform.GetComponent<TMP_Text>();
                Debug.Log("<color=green>Notasi ditemukan di: Canvas/Notasi</color>");
            }
            
            // Coba path 2: Canvas (jika Notasi langsung child)
            if (teksNotasi == null) {
                notasiTransform = transform.Find("Canvas").Find("Notasi");
                if (notasiTransform != null) {
                    teksNotasi = notasiTransform.GetComponent<TMP_Text>();
                    Debug.Log("<color=green>Notasi ditemukan di: Canvas->Notasi</color>");
                }
            }
            
            // Coba path 3: Cari di seluruh child
            if (teksNotasi == null) {
                TMP_Text[] allTexts = GetComponentsInChildren<TMP_Text>();
                foreach (TMP_Text txt in allTexts) {
                    if (txt.name == "Notasi" || txt.name.Contains("Notasi")) {
                        teksNotasi = txt;
                        Debug.Log("<color=green>Notasi ditemukan di: " + txt.transform.GetPath() + "</color>");
                        break;
                    }
                }
            }
            
            if (teksNotasi == null) {
                Debug.LogError("<color=red>GAGAL menemukan Notasi component!</color>");
            }
        }

        // Pastikan Canvas visible
        Transform canvasTransform = transform.Find("Canvas");
        if (canvasTransform != null) {
            Canvas canvas = canvasTransform.GetComponent<Canvas>();
            if (canvas != null) {
                canvas.enabled = true;
                Debug.Log($"<color=green>Canvas enabled. RenderMode: {canvas.renderMode}, SortingOrder: {canvas.sortingOrder}</color>");
            }
            canvasTransform.gameObject.SetActive(true);
            
            // Log Canvas position dan scale
            RectTransform canvasRect = canvasTransform.GetComponent<RectTransform>();
            if (canvasRect != null) {
                Debug.Log($"<color=green>Canvas RectTransform - LocalScale: {canvasRect.localScale}, AnchoredPos: {canvasRect.anchoredPosition}</color>");
            }
            
            // Log semua child objects
            int childCount = 0;
            foreach (Transform child in canvasTransform) {
                childCount++;
                Debug.Log($"<color=yellow>Canvas Child #{childCount}: {child.name}, Active: {child.gameObject.activeSelf}, Position: {child.localPosition}</color>");
                
                // Log grandchildren
                foreach (Transform grandchild in child) {
                    Debug.Log($"  └─ Grandchild: {grandchild.name}, Active: {grandchild.gameObject.activeSelf}");
                }
            }
            
            Debug.Log("<color=green>Canvas diaktifkan untuk meteor!</color>");
        } else {
            Debug.LogError("<color=red>Canvas tidak ditemukan!</color>");
        }

        // Set teks
        if (teksNarasi != null) {
            teksNarasi.text = narasi;
            teksNarasi.color = Color.white;
            teksNarasi.enabled = true;
            
            // Pastikan parent gameobjects juga active
            Transform current = teksNarasi.transform.parent;
            while (current != null) {
                if (!current.gameObject.activeSelf) {
                    Debug.LogWarning($"<color=orange>Parent inactive ditemukan: {current.name}, mengaktifkan...</color>");
                    current.gameObject.SetActive(true);
                }
                current = current.parent;
            }
            
            // FIX SCALE - Kurangi scale agar teks terlihat
            RectTransform rectNarasi = teksNarasi.GetComponent<RectTransform>();
            if (rectNarasi != null) {
                // Kurangi scale menjadi 1/5 dari original agar sesuai dengan Canvas scale
                rectNarasi.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                Debug.Log($"<color=cyan>✓ Narasi diset: {narasi}\n" +
                    $"  Position: {rectNarasi.anchoredPosition}, Scale: {rectNarasi.localScale}, Active: {teksNarasi.gameObject.activeSelf}</color>");
            }
        } else {
            Debug.LogError("<color=red>✗ teksNarasi masih NULL!</color>");
        }

        if (teksNotasi != null) {
            teksNotasi.text = notasi;
            teksNotasi.color = Color.white;
            teksNotasi.enabled = true;
            
            // Pastikan parent gameobjects juga active
            Transform current = teksNotasi.transform.parent;
            while (current != null) {
                if (!current.gameObject.activeSelf) {
                    Debug.LogWarning($"<color=orange>Parent inactive ditemukan: {current.name}, mengaktifkan...</color>");
                    current.gameObject.SetActive(true);
                }
                current = current.parent;
            }
            
            // FIX SCALE - Kurangi scale agar teks terlihat
            RectTransform rectNotasi = teksNotasi.GetComponent<RectTransform>();
            if (rectNotasi != null) {
                // Kurangi scale menjadi 1/5 dari original agar sesuai dengan Canvas scale
                rectNotasi.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                Debug.Log($"<color=cyan>✓ Notasi diset: {notasi}\n" +
                    $"  Position: {rectNotasi.anchoredPosition}, Scale: {rectNotasi.localScale}, Active: {teksNotasi.gameObject.activeSelf}</color>");
            }
        } else {
            Debug.LogError("<color=red>✗ teksNotasi masih NULL!</color>");
        }
        
        Debug.Log("<color=yellow>===== END DEBUG =====</color>");
    }

    void Update()
    {
        if (sudahSelesai) return;

        if (!sedangMengejar)
        {
            // Efek mengambang (bobbing) saat diam
            transform.position = posisiAwal + new Vector3(0, Mathf.Sin(Time.time * kecepatanMengambang) * tinggiMengambang, 0);
            
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                TriggerKejarPlayer();
                if (manager != null) manager.WaktuHabis();
            }
        }
        else
        {
            // KODE BARU: Meteor terbang mengejar player!
            if (playerTarget != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, kecepatanMengejar * Time.deltaTime);
            }
        }
    }

    public void TriggerKejarPlayer()
    {
        sedangMengejar = true;
        
        // Ubah warna teks menjadi merah untuk memberi tanda bahaya
        if (teksNarasi != null) teksNarasi.color = Color.red;
        if (teksNotasi != null) teksNotasi.color = Color.red;
    }

    public void HancurkanKarenaBenar()
    {
        sudahSelesai = true;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isReady || sudahSelesai || !sedangMengejar) return;

    if (collision.CompareTag("Player"))
    {
        sudahSelesai = true; // Langsung kunci di sini agar tidak looping trigger!

        Player p = collision.GetComponent<Player>();
        if (p != null) p.TakeDamage(damageMantra);

        if (efekLedakanApi != null) Instantiate(efekLedakanApi, transform.position, Quaternion.identity);
        if (suaraLedakan != null) AudioSource.PlayClipAtPoint(suaraLedakan, Camera.main.transform.position, 1f);

        Destroy(gameObject);
    }
    }
}