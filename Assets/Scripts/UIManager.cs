using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public void MenuBtnOnClick()
    {
        SceneManager.LoadScene(0);
    }
    public void RestartBtnOnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void StartGameBtnClick()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitBtnOnClick()
    {
        Application.Quit();
    }
}
