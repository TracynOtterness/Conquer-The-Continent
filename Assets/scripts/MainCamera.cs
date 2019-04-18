using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    new Component camera;
    bool dragging = false;
    Vector3 originalPos;
    Vector3 originalTransform;
    public float poscoefficientx = 9.35f;
    public float poscoefficienty = 5.85f;
    float maxSize;
    float cameraSize = 10f;
    Vector3 fixTransform;
    public float[] clampValueX;
    public float[] clampValueY;
    public int adjustValue;

	//so basically use the website on school computer to make functios for clamp 
	//values and instead of a switch for set things make it a continuous camera scroll between 0 and 10.48
	private void Start()
	{
        SetCameraStartingPosition();
	}

	void Update()
    {
        clampValueX = ClampValueGetX(cameraSize);
        clampValueY = ClampValueGetY(cameraSize);
        fixTransform = transform.position;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && this.GetComponent<Camera>().orthographicSize > 1.48f)
        {
            this.GetComponent<Camera>().orthographicSize -= .4f;
            if (this.GetComponent<Camera>().orthographicSize < 1.48f) this.GetComponent<Camera>().orthographicSize = 1.48f;
            cameraSize = this.GetComponent<Camera>().orthographicSize;
            poscoefficientx = AdjustDragCoefficientX(cameraSize);
            poscoefficienty = AdjustDragCoefficientY(cameraSize);
            adjustValue = 0;
            if (UIManager.showingShipInfo && UIManager.ship.invasionPossibility) {
                AdjustInvasionButtonPosition();
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && this.GetComponent<Camera>().orthographicSize < maxSize)
        {
            this.GetComponent<Camera>().orthographicSize += .4f;
            if (this.GetComponent<Camera>().orthographicSize > maxSize) this.GetComponent<Camera>().orthographicSize = maxSize;
            cameraSize = this.GetComponent<Camera>().orthographicSize;
            poscoefficientx = AdjustDragCoefficientX(cameraSize);
            poscoefficienty = AdjustDragCoefficientY(cameraSize);
          
            clampValueX = ClampValueGetX(cameraSize);
            clampValueY = ClampValueGetY(cameraSize);
            if (transform.position.x < clampValueX[0] || transform.position.x > clampValueX[1]) { FixCameraOnZoomOutX(); }
            if (transform.position.y < clampValueY[0] || transform.position.y > clampValueY[1]) { FixCameraOnZoomOutY(); }
            adjustValue = 0;
            if (UIManager.showingShipInfo && UIManager.ship.invasionPossibility)
            {
                AdjustInvasionButtonPosition();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            originalTransform = transform.position;
            originalPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            dragging = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            dragging = false;
        }
        if (dragging)
        {
            Vector3 currentMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector3 pos = (currentMousePos - originalPos);
            pos.x *= poscoefficientx;
            pos.y *= poscoefficienty;
            transform.position = new Vector3(Mathf.Clamp(originalTransform.x - pos.x, clampValueX[0], clampValueX[1]), Mathf.Clamp(originalTransform.y - pos.y, clampValueY[0], clampValueY[1]), originalTransform.z);
            if (UIManager.showingShipInfo && UIManager.ship.invasionPossibility){ UIManager.AdjustInvasionButton(adjustValue); }
        }
    }
    public float[] ClampValueGetX(float cs) 
    {
        //function for clampvalueXmin: y = 1.773x + .138
        //function for clampvalueXmax: 
        float[] b = new float[2];
        b[0] = cs * 1.74f - .314f;
        switch(Instantiator.size){
            case 1: b[1] = -cs * .854f + 11.336f; break;
            case 2: b[1] = -cs * .837f + 18.653f; break;
            case 3: b[1] = -cs * .8525f + 27.854f; break;
        }
        return b;
    }
    public float[] ClampValueGetY(float cs)
    {
        //function for clampvalueYmin: y = x - .189f
        //function for clampvalueYmax: y = -x + 19.23
        float[] b = new float[2];
        b[0] = cs * .966f - .278f;
        switch(Instantiator.size){
            case 1: b[1] = -cs * .967f + 8.392f; break;
            case 2: b[1] = -cs * .974f + 13.997f; break;
            case 3: b[1] = -cs * .964f + 20.68f; break;
        }
        return b;
    }
    public void FixCameraOnZoomOutX()
    {
        print("FixCameraX");
        float fixX = 0;
        if (transform.position.x < clampValueX[0]) { fixX = clampValueX[0]; }
        if (transform.position.x > clampValueX[1]) { fixX = clampValueX[1]; }
        fixTransform = new Vector3(fixX, fixTransform.y, fixTransform.z);
        transform.position = fixTransform;
    }
    public void FixCameraOnZoomOutY()
    {
        print("FixCameraY");
        float fixY = 0;
        if (transform.position.y < clampValueY[0]) { fixY = clampValueY[0]; }
        if (transform.position.y > clampValueY[1]) { fixY = clampValueY[1]; }
        fixTransform = new Vector3(fixTransform.x, fixY, fixTransform.z);
        transform.position = fixTransform;
    }
    float AdjustDragCoefficientX(float cs)
    {
        float returnValue = (2 * cs) / .625f;
        return returnValue;
    }
    float AdjustDragCoefficientY(float cs)
    {
        float returnValue = cs * 2;
        return returnValue;
    }
    void SetCameraStartingPosition(){
        switch (Instantiator.size)
        {
            case 1: maxSize = 4.49f; this.GetComponent<Camera>().orthographicSize = maxSize; transform.position = new Vector3(7.5f, 4.05f, -10); GameObject.Find("MiniMap").transform.position = new Vector3(5.51f, 4.01f, -12); GameObject.Find("MiniMap").GetComponent<Camera>().orthographicSize = 4.42f; break;
            case 2: maxSize = 7.36f; this.GetComponent<Camera>().orthographicSize = maxSize; transform.position = new Vector3(12.49f, 6.83f, -10); GameObject.Find("MiniMap").transform.position = new Vector3(9.17f, 6.83f, -12); GameObject.Find("MiniMap").GetComponent<Camera>().orthographicSize = 7.15f; break;
            case 3: maxSize = 10.86f; this.GetComponent<Camera>().orthographicSize = maxSize; break;
        }
    }
    public void AdjustInvasionButtonPosition(){
        
        RectTransform invasionTransform = UIManager.invasionButton.GetComponent<RectTransform>();

        float invasionTransformValue = cameraSize * -.625f + 2.925f;
        invasionTransform.localScale = new Vector3(invasionTransformValue, invasionTransformValue, 1);
        adjustValue = (int)(cameraSize * 14.583f - 31.583f);

        UIManager.AdjustInvasionButton(adjustValue);

        if (cameraSize <= 4 && !UIManager.ship.moored)
        {
            UIManager.invasionButton.GetComponent<UnityEngine.UI.Image>().enabled = true;
            UIManager.invasionButton.GetComponentInChildren<UnityEngine.UI.Text>().enabled = true;
        }
        else{
            UIManager.HideInvasionButton();
        }
    }
}
