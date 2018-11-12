using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour{
    
    private void Awake()
    {
        GameObject[] music = GameObject.FindGameObjectsWithTag("Audio");
        if (music.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }
}