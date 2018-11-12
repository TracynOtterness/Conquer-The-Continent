using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {
    bool playGame;
    int volume = 1;
    AudioSource audioSource;
    public AudioClip audioClip;
    string scene;

    public void SwitchScene(string s){
        if (s == "Scene1"){
            playGame = true;
            audioSource = GameObject.FindWithTag("Audio").GetComponent<AudioSource>();
            print(audioSource);
            scene = s;
        }
        else{
            SceneManager.LoadScene(s);
        }
    }

	private void Update()
	{
        if (playGame){
            if (audioSource.volume - .5f * Time.deltaTime <= 0){
                audioSource.volume = 0;
                audioSource.clip = audioClip;
                SceneManager.LoadScene(scene);
            }
            else{
                audioSource.volume -= .5f * Time.deltaTime;
            }
        }
	}
}