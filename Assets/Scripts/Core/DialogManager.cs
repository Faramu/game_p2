using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("--- UI PANEL & AVATAR ---")]
    public GameObject panelDialog;
    public GameObject avatarAki;          // Hanya gambar/avatar Aki di UI
    public GameObject avatarPlayerUI;     // Hanya gambar/avatar Player di UI (BUKAN objek Player di scene!)
    public TextMeshProUGUI teksNama, teksKalimat, teksTombol;
    
    [Header("--- REFERENSI PLAYER ---")]
    public Player playerScript;           // Script Player (bukan GameObject-nya)

    [Header("--- SETTINGS ---")]
    public float kecepatanTeks = 0.03f;

    private BarisDialog[] antreanDialog;
    private int index = 0;
    private Action onSelesai;
    private Coroutine teksCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void TampilkanDialog(BarisDialog[] percakapan, Action callback = null)
    {
        if (percakapan == null || percakapan.Length == 0) return;

        KunciPlayer(true);
        antreanDialog = percakapan;
        onSelesai = callback;
        index = 0;

        if (panelDialog != null)
            panelDialog.SetActive(true);

        TampilTeks();
    }

    private void TampilTeks()
    {
        if (antreanDialog == null || index >= antreanDialog.Length) return;

        teksNama.text = antreanDialog[index].namaPembicara;
        teksTombol.text = (index < antreanDialog.Length - 1) ? "" : "";

        // Cek apakah yang bicara adalah Player
        bool isPlayerSpeaking = antreanDialog[index].namaPembicara == "Player";

        // Hanya mengatur avatar UI, JANGAN pernah SetActive objek Player di scene
        if (avatarAki != null)
            avatarAki.SetActive(!isPlayerSpeaking);

        if (avatarPlayerUI != null)
            avatarPlayerUI.SetActive(isPlayerSpeaking);

        // Efek ketik
        if (teksCoroutine != null)
            StopCoroutine(teksCoroutine);

        teksCoroutine = StartCoroutine(EfekKetik(antreanDialog[index].kalimat));
    }

    private IEnumerator EfekKetik(string kalimat)
    {
        teksKalimat.text = "";
        foreach (char huruf in kalimat)
        {
            teksKalimat.text += huruf;
            yield return new WaitForSeconds(kecepatanTeks);
        }
    }

    public void TombolKlik()
    {
        if (antreanDialog == null) return;

        // Skip efek ketik
        if (teksKalimat.text.Length < antreanDialog[index].kalimat.Length)
        {
            if (teksCoroutine != null)
                StopCoroutine(teksCoroutine);

            teksKalimat.text = antreanDialog[index].kalimat;
            return;
        }

        index++;

        if (index < antreanDialog.Length)
        {
            TampilTeks();
        }
        else
        {
            TutupDialog();
        }
    }

    private void TutupDialog()
    {
        if (panelDialog != null)
            panelDialog.SetActive(false);

        if (avatarAki != null) avatarAki.SetActive(false);
        if (avatarPlayerUI != null) avatarPlayerUI.SetActive(false);

        KunciPlayer(false);
        onSelesai?.Invoke();
    }

    private void KunciPlayer(bool kunci)
    {
        if (playerScript != null)
        {
            playerScript.canMove = !kunci;
        }
    }
}