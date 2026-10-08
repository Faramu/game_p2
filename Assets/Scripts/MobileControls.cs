using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileControls : MonoBehaviour
{
    public static MobileControls Instance;

    [Header("Panel Tombol Mobile")]
    public GameObject mobileControlsPanel;

    [Header("Pengaturan Platform")]
    [Tooltip("Jika true, tombol mobile dipaksa muncul di Editor untuk simulasi. Default false agar di Editor/PC tidak muncul.")]
    public bool forceShowInEditor = false;

    [Header("Referensi Tombol (Opsional di Inspector)")]
    public Button buttonMaju;
    public Button buttonMundur;
    public Button buttonLoncat;

    private Player cachedPlayer;

    private void Awake()
    {
        Instance = this;
        if (mobileControlsPanel == null)
        {
            mobileControlsPanel = gameObject;
        }
    }

    private void Start()
    {
        SetupPlatformVisibility();
        FindPlayerAndSetupButtons();
    }

    public void SetupPlatformVisibility()
    {
        bool showMobileControls = false;

#if UNITY_EDITOR
        // Di Unity Editor saat testing sebagai PC, disembunyikan
        showMobileControls = forceShowInEditor;
#else
        // Di Android / Mobile HP asli, aktifkan
        showMobileControls = Application.isMobilePlatform;
#endif

        if (mobileControlsPanel != null)
        {
            mobileControlsPanel.SetActive(showMobileControls);
            Debug.Log($"[MOBILE CONTROLS] Tampilan mobile controls: {showMobileControls} (isMobilePlatform: {Application.isMobilePlatform})");
        }
    }

    private void FindPlayerAndSetupButtons()
    {
        if (cachedPlayer == null)
        {
            cachedPlayer = Object.FindFirstObjectByType<Player>();
        }

        // Cari otomatis tombol jika belum di-assign di Inspector
        if (buttonMaju == null)
        {
            Transform t = transform.Find("Button_Maju");
            if (t != null) buttonMaju = t.GetComponent<Button>();
        }
        if (buttonMundur == null)
        {
            Transform t = transform.Find("Button_Mundur");
            if (t != null) buttonMundur = t.GetComponent<Button>();
        }
        if (buttonLoncat == null)
        {
            Transform t = transform.Find("Button_Loncat");
            if (t != null) buttonLoncat = t.GetComponent<Button>();
        }

        // Setup event trigger untuk Maju
        SetupEventTrigger(buttonMaju != null ? buttonMaju.gameObject : null, MulaiMaju, StopMaju);

        // Setup event trigger untuk Mundur
        SetupEventTrigger(buttonMundur != null ? buttonMundur.gameObject : null, MulaiMundur, StopMundur);

        // Setup event trigger untuk Loncat
        if (buttonLoncat != null)
        {
            EventTrigger trigger = buttonLoncat.GetComponent<EventTrigger>();
            if (trigger == null) trigger = buttonLoncat.gameObject.AddComponent<EventTrigger>();

            AddTriggerEntry(trigger, EventTriggerType.PointerDown, Loncat);
            buttonLoncat.onClick.RemoveListener(Loncat);
            buttonLoncat.onClick.AddListener(Loncat);
        }
    }

    private void SetupEventTrigger(GameObject btnObj, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
    {
        if (btnObj == null) return;

        EventTrigger trigger = btnObj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = btnObj.AddComponent<EventTrigger>();

        AddTriggerEntry(trigger, EventTriggerType.PointerDown, onDown);
        AddTriggerEntry(trigger, EventTriggerType.PointerUp, onUp);
        AddTriggerEntry(trigger, EventTriggerType.PointerExit, onUp);
    }

    private void AddTriggerEntry(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((_) => action?.Invoke());
        trigger.triggers.Add(entry);
    }

    // =========================================================
    // FUNGSI KONTROL YANG TERHUBUNG KE PLAYER
    // =========================================================

    public void MulaiMaju()
    {
        if (cachedPlayer == null) cachedPlayer = Object.FindFirstObjectByType<Player>();
        if (cachedPlayer != null) cachedPlayer.MulaiMaju();
    }

    public void StopMaju()
    {
        if (cachedPlayer == null) cachedPlayer = Object.FindFirstObjectByType<Player>();
        if (cachedPlayer != null) cachedPlayer.StopMaju();
    }

    public void MulaiMundur()
    {
        if (cachedPlayer == null) cachedPlayer = Object.FindFirstObjectByType<Player>();
        if (cachedPlayer != null) cachedPlayer.MulaiMundur();
    }

    public void StopMundur()
    {
        if (cachedPlayer == null) cachedPlayer = Object.FindFirstObjectByType<Player>();
        if (cachedPlayer != null) cachedPlayer.StopMundur();
    }

    public void Loncat()
    {
        if (cachedPlayer == null) cachedPlayer = Object.FindFirstObjectByType<Player>();
        if (cachedPlayer != null) cachedPlayer.Loncat();
    }
}