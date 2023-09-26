using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get the scene view camera
        Camera sceneviewcamera = UnityEditor.SceneView.lastActiveSceneView.camera;
        //get the position of the scene view camera
        Vector3 cameraposition = sceneviewcamera.transform.position;
        //get the rotation of the scene view camera
        Quaternion camerarotation = sceneviewcamera.transform.rotation;
        //set the position of the game camera to the position of the scene view camera
        this.transform.position = cameraposition;
        //set the rotation of the game camera to the rotation of the scene view camera
        this.transform.rotation = camerarotation;
        
    }
}
