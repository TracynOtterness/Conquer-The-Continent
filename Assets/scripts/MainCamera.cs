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
    float cameraSize = 10f;
    Vector3 fixTransform;
    public float[] clampValueX;
    public float[] clampValueY;
    public int adjustValue;

    void Start()
    {
        this.GetComponent<Camera>().orthographicSize = 10.86f;
    }

    //so basically use the website on school computer to make functios for clamp 
    //values and instead of a switch for set things make it a continuous camera scroll between 0 and 10.48

    void Update()
    {
        clampValueX = ClampValueGetX(cameraSize);
        clampValueY = ClampValueGetY(cameraSize);
        fixTransform = transform.position;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && this.GetComponent<Camera>().orthographicSize > 1.48f)
        {
            this.GetComponent<Camera>().orthographicSize -= .4f;//needs changed
            cameraSize = this.GetComponent<Camera>().orthographicSize;
            poscoefficientx = AdjustDragCoefficientX(cameraSize);
            poscoefficienty = AdjustDragCoefficientY(cameraSize);
            adjustValue = 0;
            if (UIManager.showingShipInfo && UIManager.ship.invasionPossibility) {
                RectTransform invasionTransform = UIManager.invasionButton.GetComponent<RectTransform>();
                switch((int)cameraSize){
                    case 2: invasionTransform.localScale = new Vector3(2, 2, 1); adjustValue = 0; break;
                    case 3: invasionTransform.localScale = new Vector3(1, 1, 1); adjustValue = 20; break;
                    case 4: invasionTransform.localScale = new Vector3(.5f, .5f, 1); adjustValue = 25; break;
                    default: UIManager.HideInvasionButton(); break;
                }
                UIManager.AdjustInvasionButton(adjustValue);
                if (cameraSize <= 4 && !UIManager.ship.moored){
                    UIManager.invasionButton.GetComponent<UnityEngine.UI.Image>().enabled = true;
                    UIManager.invasionButton.GetComponentInChildren<UnityEngine.UI.Text>().enabled = true;
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && this.GetComponent<Camera>().orthographicSize < 10.48f)
        {
            this.GetComponent<Camera>().orthographicSize += .4f;
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
                RectTransform invasionTransform = UIManager.invasionButton.GetComponent<RectTransform>();
                switch ((int)cameraSize)
                {
                    case 2: invasionTransform.localScale = new Vector3(2, 2, 1); adjustValue = 0; break;
                    case 3: invasionTransform.localScale = new Vector3(1, 1, 1); adjustValue = 20; break;
                    case 4: invasionTransform.localScale = new Vector3(.5f, .5f, 1); adjustValue = 25; break;
                    default: UIManager.HideInvasionButton(); break;
                }
                UIManager.AdjustInvasionButton(adjustValue);
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
    public float[] ClampValueGetX(float cs) //needs reworking to be fine with floats
    {
        //function for clampvalueXmin: y = 1.773x + .138
        //function for clampvalueXmax: 
        float[] b = new float[2];
        b[0] = cs * 1.74f - .3f;
        b[1] = -cs * .85f + 27.86f;
        return b;
    }
    public float[] ClampValueGetY(float cs) //needs reworking to be fine with floats
    {
        //function for clampvalueYmin: y = x - .189f
        //function for clampvalueYmax: y = -x + 19.23
        float[] b = new float[2];
        b[0] = cs * .97f - .3f;
        b[1] = -cs * .97f + 20.72f;
        return b;
    }
    public void FixCameraOnZoomOutX()
    {
        print("FixCameraX");
        print("x transform: " + transform.position.x);
        print("y transform: " + transform.position.y);
        print(clampValueX[0]);
        print(clampValueX[1]);
        float fixX = 0;
        if (transform.position.x < clampValueX[0]) { fixX = clampValueX[0]; }
        if (transform.position.x > clampValueX[1]) { fixX = clampValueX[1]; }
        fixTransform = new Vector3(fixX, fixTransform.y, fixTransform.z);
        transform.position = fixTransform;
    }
    public void FixCameraOnZoomOutY()
    {
        print("FixCameraY");
        print("x transform: " + transform.position.x);
        print("y transform: " + transform.position.y);
        print(clampValueY[0]);
        print(clampValueY[1]);
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
}
