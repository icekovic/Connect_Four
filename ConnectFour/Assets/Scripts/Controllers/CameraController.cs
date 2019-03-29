using ConnectFour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Camera cam;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        mainCamera.orthographic = true;

        //cam = GetComponent<Camera>();
        //cam.orthographic = true;
    }

    void LateUpdate()
    {
        float maxY = (GameObject.Find("GameController").GetComponent<GameController>().GetNumberOfRows()) + 2;

        //cam.orthographicSize = maxY / 2f;
    }
}
