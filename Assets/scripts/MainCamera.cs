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

    void Start()
    {
        this.GetComponent<Camera>().orthographicSize = 10f;
    }

    void Update()
    {
        clampValueX = ClampValueGetX((int)cameraSize);
        clampValueY = ClampValueGetY((int)cameraSize);
        fixTransform = transform.position;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && this.GetComponent<Camera>().orthographicSize > 2f)
        {
            this.GetComponent<Camera>().orthographicSize -= 1f;
            cameraSize = this.GetComponent<Camera>().orthographicSize;
            poscoefficientx = AdjustDragCoefficientX((int)cameraSize);
            poscoefficienty = AdjustDragCoefficientY((int)cameraSize);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && this.GetComponent<Camera>().orthographicSize < 10f)
        {
            this.GetComponent<Camera>().orthographicSize += 1f;
            cameraSize = this.GetComponent<Camera>().orthographicSize;
            poscoefficientx = AdjustDragCoefficientX((int)cameraSize);
            poscoefficienty = AdjustDragCoefficientY((int)cameraSize);
            clampValueX = ClampValueGetX((int)cameraSize);
            clampValueY = ClampValueGetY((int)cameraSize);
            if (transform.position.x < clampValueX[0] || transform.position.x > clampValueX[1]) { FixCameraOnZoomOutX(); }
            if (transform.position.y < clampValueY[0] || transform.position.y > clampValueY[1]) { FixCameraOnZoomOutY(); }
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
        }
    }
    public float[] ClampValueGetX(int cs)
    {
        float[] b = new float[2];
        switch (cs)
        {
            case 10: b[0] = 15.73f; b[1] = 15.73f; break;
            case 9: b[0] = 14.44f; b[1] = 16.14f; break;
            case 8: b[0] = 12.88f; b[1] = 17.06f; break;
            case 7: b[0] = 11.29f; b[1] = 18.06f; break;
            case 6: b[0] = 9.70f; b[1] = 19.02f; break;
            case 5: b[0] = 8.11f; b[1] = 19.98f; break;
            case 4: b[0] = 6.52f; b[1] = 20.98f; break;
            case 3: b[0] = 4.93f; b[1] = 21.94f; break;
            case 2: b[0] = 3.30f; b[1] = 22.90f; break;
        }
        return b;
    }
    public float[] ClampValueGetY(int cs)
    {
        float[] b = new float[2];
        switch (cs)
        {
            case 10: b[0] = 9.61f; b[1] = 9.61f; break;
            case 9: b[0] = 8.74f; b[1] = 10.40f; break;
            case 8: b[0] = 7.76f; b[1] = 11.42f; break;
            case 7: b[0] = 6.77f; b[1] = 12.41f; break;
            case 6: b[0] = 5.78f; b[1] = 13.40f; break;
            case 5: b[0] = 4.80f; b[1] = 14.42f; break;
            case 4: b[0] = 3.81f; b[1] = 15.41f; break;
            case 3: b[0] = 2.79f; b[1] = 16.40f; break;
            case 2: b[0] = 1.80f; b[1] = 17.38f; break;
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
    float AdjustDragCoefficientX(int cs)
    {
        float returnValue = (2 * cs) / .625f;
        return returnValue;
    }
    float AdjustDragCoefficientY(int cs)
    {
        float returnValue = cs * 2;
        return returnValue;
    }
}