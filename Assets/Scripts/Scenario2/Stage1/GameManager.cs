using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level State")]
    public bool isTutorialCleared = false;
    public bool isNPCFruitCleared = false;
    public int scorePemain = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelesaikanTutorial()
    {
        isTutorialCleared = true;
        Debug.Log("Level 1 Clear! Gerbang menuju Level 2 Terbuka.");
    }
}