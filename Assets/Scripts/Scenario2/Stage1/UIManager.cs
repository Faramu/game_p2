using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD Panels")]
    [SerializeField] private GameObject panelHUDUtama;
    [SerializeField] private GameObject panelPauseMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TogglePauseMenu(bool isiActive)
    {
        panelPauseMenu.SetActive(isiActive);
        Time.timeScale = isiActive ? 0f : 1f;
    }
}