using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerDetector : MonoBehaviour
{
    public GameObject SketchBoard;
    [SerializeField] Texture RightCamera;
    [SerializeField] Texture LeftCamera;
    private Texture2D texture2d;
    private MeshRenderer _meshRenderer;
    bool whichCamera = false;

    void Start()
    {
         _meshRenderer = SketchBoard.GetComponent<MeshRenderer>();
        _meshRenderer.material.mainTexture = RightCamera;
    }
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            
            whichCamera = !whichCamera;
            if ( whichCamera )
            {
                Texture2D tex2d = new Texture2D(RightCamera.width, RightCamera.height, TextureFormat.RGB24, false);
                // RenderTexture.active = RightCamera;
                tex2d.ReadPixels(new Rect(0, 0, RightCamera.width, RightCamera.height), 0, 0);
                tex2d.Apply();
                _meshRenderer.material.mainTexture = tex2d;
            }
            else
            {
                texture2d = LeftCamera as Texture2D;
                _meshRenderer.material.mainTexture = texture2d;
                
            }
            

        }

    }

}
