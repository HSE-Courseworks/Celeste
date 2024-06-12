using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    public void Play() {
        SceneManager.LoadScene(2);
    }
    public void Exit() {
        Application.Quit();
    }

}