using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {
    bool playGame;
    int volume = 1;
    AudioSource audioSource;
    public AudioClip audioClip;
    string scene;

    public void SwitchScene(string s){
        SceneManager.LoadScene(s);
    }
}
