using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookat : MonoBehaviour
{
    public GameObject target;
    public GameObject cameraGameObject;

    // Update is called once per frame
    void Update()
    {
        cameraGameObject.transform.LookAt(target.transform);
    }
}
