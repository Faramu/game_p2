using UnityEngine;

public class RestartGame : MonoBehaviour
{
  public void LoadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1f;
    }
}
