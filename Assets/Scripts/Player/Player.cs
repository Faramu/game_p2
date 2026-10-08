using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("--- STATUS PLAYER ---")]
    public int health = 3;
    public int coins = 0;
    public bool canMove = true;

    [Header("--- UI HEALTH & COIN ---")]
    public TMP_Text textKoinUI;
    public Image[] hearts;
    public Sprite hatiPenuh;
    public Sprite hatiKosong;

    private static int nextHealth = 3;
    private static int nextCoins = 0;
    private static bool isRespawn = false;

    [Header("--- PERGERAKAN ---")]
    public float moveSpeed = 5f;
    public float jumpForce = 9f;               // Sudah diturunkan
    public int extraJumpValue = 1;
    private int extraJump;

    [Header("--- SENSOR TANAH & JURANG ---")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float fallThreshold = -10f;

    [Header("--- CHECKPOINT & RESPAWN ---")]
    [Tooltip("Daftar objek checkpoint di stage yang bisa di-assign manual lewat Inspector")]
    public Transform[] daftarCheckpoint;
    [Tooltip("Offset ketinggian saat respawn agar player berdiri aman di atas platform (contoh: Y=4 menjadi Y=5)")]
    public float respawnYOffset = 1.0f;
    [Tooltip("Nama scene Menu Pencapaian saat game over / nyawa habis")]
    public string namaSceneMenuPencapaian = "Menu_Pencapaian";
    private Vector3 startPosition;
    private Vector3 lastSafePosition;
    private bool hasCustomCheckpoint = false;

    // Input mobile (Maju, Mundur, Loncat)
    private float mobileMoveInput = 0f;

    [Header("--- AUDIO ---")]
    public AudioSource audioSource;
    public AudioClip suaraLompat;
    public AudioClip suaraJatuh;
    public AudioClip suaraKenaDamage;
    public AudioClip suaraAmbilKoin;
    public AudioClip suaraKuisBenar;

    // Komponen
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Status
    private bool isDead = false;
    private bool isInvincible = false;
    private bool misiJalanKananSelesai = false;
    private string currentAnim = "";
    private float lastGroundedTime;
private float groundedRememberTime = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        extraJump = extraJumpValue;

        // Respawn handling & reset status nyawa
        if (isRespawn && nextHealth > 0)
        {
            health = nextHealth;
            coins = nextCoins;
        }
        else
        {
            health = 3;
            coins = 0;
            nextHealth = 3;
            nextCoins = 0;
            isRespawn = false;
        }

        isDead = false;
        isInvincible = false;
        canMove = true;
        Time.timeScale = 1f;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        startPosition = transform.position;
        lastSafePosition = startPosition;

        SetupCheckpointsFromInspector();
        UpdateHealthUI();
        UpdateKoinUI();
        PlayAnim("player1_idle");
    }

    void Update()
    {
        if (isDead) return;

        // Cek jatuh ke jurang
        if (transform.position.y < fallThreshold)
        {
            PlaySound(suaraJatuh);
            ProsesJatuhKeJurang();
            return;
        }

        // Cek apakah player telah melewati checkpoint yang di-assign di Inspector
        PeriksaDaftarCheckpoint();

        if (!canMove)
        {
            if (rb != null)
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (isGrounded) PlayAnim("player1_idle");
            return;
        }

        // Reset extra jump
        if (isGrounded)
            extraJump = extraJumpValue;

        // Lompat (Keyboard)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Loncat();
        }

        // Gerakan horizontal (PC keyboard + Mobile input)
        float keyboardInput = Input.GetAxisRaw("Horizontal");
        float moveInput = Mathf.Abs(mobileMoveInput) > 0.01f ? mobileMoveInput : keyboardInput;
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        // Flip
        if (moveInput > 0.1f)
        {
            if (spriteRenderer != null) spriteRenderer.flipX = false;

            if (!misiJalanKananSelesai)
            {
                misiJalanKananSelesai = true;
                MisiManager managerMisi = Object.FindFirstObjectByType<MisiManager>();
                if (managerMisi != null)
                    managerMisi.MisiSelesai(0);
            }
        }
        else if (moveInput < -0.1f)
        {
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }

        SetAnimation(moveInput);
    }

    private void FixedUpdate()
    {
        if (groundCheck == null || isDead)
        {
            isGrounded = false;
            return;
        }

        bool groundedNow = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (groundedNow)
        {
            isGrounded = true;
            lastGroundedTime = Time.time;
        }
        else
        {
            // Buffer biar tidak mudah kehilangan grounded
            isGrounded = (Time.time - lastGroundedTime) <= groundedRememberTime;
        }
    }

    private void SetAnimation(float moveInput)
    {
        if (animator == null || isDead) return;

        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.3f)
                PlayAnim("player1_loncat");
            else
                PlayAnim("player1_jatuh");
        }
        else
        {
            if (Mathf.Abs(moveInput) > 0.1f)
                PlayAnim("player1_jalan");
            else
                PlayAnim("player1_idle");
        }
    }

    private void PlayAnim(string newAnim)
    {
        if (currentAnim == newAnim) return;
        currentAnim = newAnim;
        animator.Play(newAnim);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || isInvincible) return;

        if (collision.gameObject.CompareTag("Damage"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.6f);
            PlaySound(suaraKenaDamage);
            StartCoroutine(BlinkRed());
            KurangiNyawa();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Coin") || collision.gameObject.CompareTag("Reward"))
        {
            coins++;
            nextCoins = coins;
            UpdateKoinUI();
            PlaySound(suaraAmbilKoin);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Checkpoint"))
        {
            SetCheckpoint(collision.transform.position);
        }
    }

    // =========================================================
    // KONTROL TAMBAHAN UNTUK MOBILE
    // =========================================================
    public void MulaiMaju()
    {
        mobileMoveInput = 1f;
    }

    public void StopMaju()
    {
        if (mobileMoveInput > 0f) mobileMoveInput = 0f;
    }

    public void MulaiMundur()
    {
        mobileMoveInput = -1f;
    }

    public void StopMundur()
    {
        if (mobileMoveInput < 0f) mobileMoveInput = 0f;
    }

    public void Loncat()
    {
        if (!canMove || isDead) return;

        if (isGrounded || extraJump > 0)
        {
            if (rb != null)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (!isGrounded) extraJump--;
            PlaySound(suaraLompat);
        }
    }

    // =========================================================
    // SISTEM CHECKPOINT & RESPAWN
    // =========================================================
    private void SetupCheckpointsFromInspector()
    {
        if (daftarCheckpoint == null) return;

        foreach (Transform cp in daftarCheckpoint)
        {
            if (cp == null) continue;

            Collider2D col = cp.GetComponent<Collider2D>();
            if (col == null)
            {
                col = cp.gameObject.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
            }
            else
            {
                col.isTrigger = true;
            }

            if (cp.GetComponent<CheckpointTrigger>() == null)
            {
                cp.gameObject.AddComponent<CheckpointTrigger>();
            }
        }
    }

    private void PeriksaDaftarCheckpoint()
    {
        if (daftarCheckpoint == null || daftarCheckpoint.Length == 0) return;

        for (int i = 0; i < daftarCheckpoint.Length; i++)
        {
            Transform cp = daftarCheckpoint[i];
            if (cp == null) continue;

            // Jika player sudah melewati posisi X checkpoint atau berada dekat (radius 1.5 unit)
            float dist = Vector2.Distance(transform.position, cp.position);
            bool passedX = transform.position.x >= cp.position.x;
            if ((passedX || dist < 1.5f) && (!hasCustomCheckpoint || cp.position.x > lastSafePosition.x))
            {
                SetCheckpoint(cp.position);
            }
        }
    }

    public void SetCheckpoint(Vector3 pos)
    {
        lastSafePosition = pos;
        hasCustomCheckpoint = true;
        Debug.Log($"[PLAYER] Checkpoint disimpan di: {pos}");
    }

    public Vector3 GetLastSafePosition()
    {
        return lastSafePosition;
    }

    public void MatikanPlayer()
    {
        if (isDead) return;

        isDead = true;
        canMove = false;
        isInvincible = true;

        // Setiap player mati, kurangi nyawa 1
        health -= 1;
        if (health < 0) health = 0;
        nextHealth = health;
        UpdateHealthUI();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // Jika player sudah mati 3 kali / heart mencapai 0, jangan melakukan respawn lagi
        if (health <= 0)
        {
            Debug.Log($"[PLAYER] Nyawa habis (0 heart)! Langsung pindah ke scene {namaSceneMenuPencapaian}");
            StartCoroutine(PindahKeMenuPencapaian());
        }
        else
        {
            // Selama heart masih tersedia, player respawn ke checkpoint terakhir yang sudah dilewati
            Debug.Log($"[PLAYER] Player mati! Sisa hati: {health}. Respawn ke checkpoint.");
            StartCoroutine(RespawnPlayerCoroutine());
        }
    }

    private void ProsesJatuhKeJurang()
    {
        MatikanPlayer();
    }

    private IEnumerator RespawnPlayerCoroutine()
    {
        // Pastikan tidak melakukan respawn jika nyawa sudah habis
        if (health <= 0) yield break;

        yield return new WaitForSeconds(0.3f);

        if (health <= 0) yield break;

        // Kembalikan player ke checkpoint terakhir yang sudah dilewati (bukan posisi awal/spawn jika checkpoint sudah ada)
        Vector3 targetRespawn = hasCustomCheckpoint ? lastSafePosition : startPosition;
        targetRespawn.y += respawnYOffset;

        transform.position = targetRespawn;

        // Reset velocity Rigidbody2D saat respawn agar player tidak langsung jatuh atau bergerak sendiri
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        isDead = false;
        canMove = true;
        PlayAnim("player1_idle");

        Debug.Log($"[PLAYER] Player berhasil respawn di {targetRespawn}");

        // Efek kedip perlindungan sesaat setelah respawn (1.5 detik)
        yield return StartCoroutine(BlinkInvincible(1.5f));

        isInvincible = false;
    }

    private IEnumerator BlinkInvincible(float durasi)
    {
        if (spriteRenderer == null) yield break;

        float durasiHabis = Time.time + durasi;
        while (Time.time < durasiHabis)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.25f);
            yield return new WaitForSeconds(0.12f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.12f);
        }
        spriteRenderer.color = Color.white;
    }

    private void KurangiNyawa()
    {
        if (isDead || isInvincible) return;

        health -= 1;
        if (health < 0) health = 0;
        nextHealth = health;
        UpdateHealthUI();

        Debug.Log($"[PLAYER] Kena damage! Sisa hati: {health}");

        if (health <= 0)
        {
            // Nyawa habis: jangan respawn, langsung kunci player dan pindah ke Menu Pencapaian
            isDead = true;
            canMove = false;
            isInvincible = true;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;
            }
            Debug.Log($"[PLAYER] Nyawa habis (0 heart)! Langsung pindah ke {namaSceneMenuPencapaian}");
            StartCoroutine(PindahKeMenuPencapaian());
        }
        else
        {
            StartCoroutine(BlinkDamageInvincible());
        }
    }

    private IEnumerator BlinkDamageInvincible()
    {
        isInvincible = true;
        yield return StartCoroutine(BlinkInvincible(1.0f));
        isInvincible = false;
    }

    /// <summary>
    /// Mengurangi 1 Health/Heart Player saat menjawab soal kuis salah.
    /// Memperbarui UI Heart dan mengembalikan sisa HP terbaru.
    /// </summary>
    public int KurangiHealthSatu()
    {
        int sebelum = health;
        if (health > 0)
        {
            health -= 1;
            nextHealth = health;
            UpdateHealthUI();
        }
        int sesudah = health;
        Debug.Log($"[HEART]\nJawaban salah.\nHP: {sebelum} → {sesudah}");

        if (health <= 0)
        {
            isDead = true;
            canMove = false;
            isInvincible = true;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;
            }
        }

        return sesudah;
    }

    public void JawabanBenar()
    {
        PlaySound(suaraKuisBenar);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void UpdateKoinUI()
    {
        if (textKoinUI != null)
            textKoinUI.text = coins.ToString();
    }

    public void UpdateHealthUI()
    {
        if (hearts == null || hearts.Length == 0) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            hearts[i].enabled = true;
            hearts[i].color = Color.white;

            if (i < health)
                hearts[i].sprite = hatiPenuh;
            else
                hearts[i].sprite = hatiKosong;
        }
    }

    private IEnumerator BlinkRed()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator ReloadSceneWithDelay()
    {
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void ResetStatusPermainan()
    {
        nextHealth = 3;
        nextCoins = 0;
        isRespawn = false;
    }

    private IEnumerator PindahKeMenuPencapaian()
    {
        yield return new WaitForSeconds(0.5f);
        ResetStatusPermainan();
        SceneManager.LoadScene(namaSceneMenuPencapaian);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        if (rb != null)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.5f);

        PlaySound(suaraKenaDamage);
        StartCoroutine(BlinkRed());
        KurangiNyawa();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
