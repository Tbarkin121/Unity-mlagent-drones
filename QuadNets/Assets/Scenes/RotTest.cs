using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cube;
    public Rigidbody rb { get; private set; }
    void Start()
    {
        rb = cube.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddRelativeTorque(cube.transform.InverseTransformVector(cube.transform.up) * 0.1f, ForceMode.Force);
        rb.AddRelativeTorque(cube.transform.InverseTransformVector(cube.transform.right) * 0.1f, ForceMode.Force);
        rb.AddRelativeTorque(cube.transform.InverseTransformVector(cube.transform.forward) * 0.1f, ForceMode.Force);
        print(cube.transform.InverseTransformVector(rb.angularVelocity));
    }
}
