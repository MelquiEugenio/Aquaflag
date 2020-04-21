using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    // Start is called before the first frame update
    public void PlayGame ()
    {
        Debug.Log("Loading");
        SceneManager.LoadScene(1);
    }

    public void QuitGame ()
    {
        Debug.Log("Quited");
        Application.Quit();
    }
}
