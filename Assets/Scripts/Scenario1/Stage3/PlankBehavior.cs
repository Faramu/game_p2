using UnityEngine;
using TMPro;

public class PlankBehavior : MonoBehaviour
{
    public int koordinatPapan;

    [Header("Teks Angka")]
    public TextMeshPro teksAngka;
    public Color warnaNormal = Color.white;
    public Color warnaTerinjak = Color.red;

    [Header("Tag")]
    public string tagPlayer = "Player";
    public string tagMusuh = "Enemy";

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector3 posisiAwal;
    private int jumlahDiAtas;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        posisiAwal = transform.position;

        if (rb) rb.bodyType = RigidbodyType2D.Kinematic;
        if (teksAngka == null) teksAngka = GetComponentInChildren<TextMeshPro>();
        if (teksAngka != null) warnaNormal = teksAngka.color;
    }

    void Start() => CekObjekDiAtas();

    void OnCollisionEnter2D(Collision2D other)
    {
        if (IsValid(other.gameObject))
        {
            jumlahDiAtas++;
            SetWarna(warnaTerinjak);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (IsValid(other.gameObject))
        {
            jumlahDiAtas = Mathf.Max(0, jumlahDiAtas - 1);
            if (jumlahDiAtas == 0) SetWarna(warnaNormal);
        }
    }

public void CekObjekDiAtas()
{
    if (col == null) col = GetComponent<Collider2D>();
    if (col == null) return;

    // Perbesar sedikit area pengecekan
    Vector2 point = col.bounds.center;
    Vector2 size = col.bounds.size * 1.1f;

    Collider2D[] hasil = Physics2D.OverlapBoxAll(point, size, 0f);

    bool adaTarget = false;

    foreach (var hit in hasil)
    {
        if (hit != null && (hit.CompareTag(tagPlayer) || hit.CompareTag(tagMusuh)))
        {
            adaTarget = true;
            break;
        }
    }

    jumlahDiAtas = adaTarget ? 1 : 0;
    SetWarna(adaTarget ? warnaTerinjak : warnaNormal);

    if (adaTarget)
        Debug.Log($"<color=green>Papan {koordinatPapan} MENYALA</color>");
}

    bool IsValid(GameObject obj) => obj.CompareTag(tagPlayer) || obj.CompareTag(tagMusuh);

    void SetWarna(Color c)
    {
        if (teksAngka != null) teksAngka.color = c;
    }

    public void ResetPapan()
    {
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
        transform.position = posisiAwal;
        jumlahDiAtas = 0;
        SetWarna(warnaNormal);
    }

    public void JatuhkanPapan()
    {
        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 3f;
        }
    }
}