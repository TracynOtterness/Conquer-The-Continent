using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Audio : MonoBehaviour{
    static Audio staticAudio;
    public AudioClip[] audioClips;
    private void Awake()
    {
        if(staticAudio == null){
            staticAudio = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }
	private void OnEnable()
	{
        SceneManager.sceneLoaded += OnSceneLoaded;
	}
	private void OnDisable()
	{
        SceneManager.sceneLoaded -= OnSceneLoaded;
	}
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Main Game")
        {
            this.GetComponent<AudioSource>().clip = audioClips[1];
        }
    }
}
