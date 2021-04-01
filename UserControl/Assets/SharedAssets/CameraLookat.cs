using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookat : MonoBehaviour
{
    public GameObject target;
    public float cameraDist = 5;
    private Vector3 offset;
    // Update is called once per frame
    void Start()
    {
        UpdateCameraOffset();
    }
    void Update()
    {
        UpdateCameraOffset();
        gameObject.transform.SetPositionAndRotation(target.transform.position + offset, Quaternion.identity);
        gameObject.transform.LookAt(target.transform);
    }

    private void UpdateCameraOffset()
    {
        Vector3 vector = new Vector3(0.0f, cameraDist, -cameraDist);
        offset = Quaternion.AngleAxis(target.transform.eulerAngles.y, Vector3.up) * vector;
    }
}
