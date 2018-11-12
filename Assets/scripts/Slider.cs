using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slider : MonoBehaviour {

    public bool snap;
    public string setting;
    public Vector3 adjustedMousePos;
    static int playerCount;
    public static float volume = 1;
    public AudioSource audioSource;

	private void Start()
	{
        audioSource = GameObject.FindWithTag("Audio").GetComponent<AudioSource>();
	}
	private void OnMouseDrag()
	{
        if(snap){
            adjustedMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (adjustedMousePos.x < -365) { SnapTo(0); }
            if (-365 <= adjustedMousePos.x && adjustedMousePos.x < -219) { SnapTo(1); }
            if (-219 <= adjustedMousePos.x && adjustedMousePos.x < -73) { SnapTo(2); }
            if (-73 <= adjustedMousePos.x && adjustedMousePos.x < 73) { SnapTo(3); }
            if (73 <= adjustedMousePos.x && adjustedMousePos.x < 219) { SnapTo(4); }
            if (219 <= adjustedMousePos.x && adjustedMousePos.x < 365) { SnapTo(5); }
            if (365 <= adjustedMousePos.x) { SnapTo(6); }
        }
        else{
            adjustedMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(Mathf.Clamp(adjustedMousePos.x, -438f, 438f), this.transform.position.y, this.transform.position.z); 
        }
        AdjustValue(setting);
	}
    void SnapTo(int i){
        switch(i){
            case 0: this.transform.position = new Vector3(-438, this.transform.position.y, this.transform.position.z); playerCount = i + 2; break;
            case 1: this.transform.position = new Vector3(-292, this.transform.position.y, this.transform.position.z); playerCount = i + 2; break;
            case 2: this.transform.position = new Vector3(-146, this.transform.position.y, this.transform.position.z); playerCount = i + 2; break;
            case 3: this.transform.position = new Vector3(0, this.transform.position.y, this.transform.position.z); playerCount = i + 2; break;
            case 4: this.transform.position = new Vector3(146, this.transform.position.y, this.transform.position.z); playerCount = i + 2; break;
            case 5: this.transform.position = new Vector3(292, this.transform.position.y, this.transform.position.z); playerCount = i + 2; break;
            case 6: this.transform.position = new Vector3(438, this.transform.position.y, this.transform.position.z); playerCount = i + 2; break;
        }
    }
    void AdjustValue(string s){
        switch(s){
            case "players": UIManager.playerCount = playerCount; GameObject.Find("PlayerCount").GetComponent<UnityEngine.UI.Text>().text = System.Convert.ToString(playerCount); break;
            case "music": audioSource.volume = (this.transform.position.x + 438) / 876; volume = audioSource.volume; break;
        }
    }
}
