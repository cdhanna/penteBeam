using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Preview3dBehaviour : MonoBehaviour
{

    public RawImage Destination;
    public GameObject Prefab;

    public Transform RenderSpace;
    public Camera RenderCamera;

    public RenderTexture RenderTexture;
    public GameObject preview;

    // Start is called before the first frame update
    void Start()
    {
        // instatiate a camera and a prefab somewhere in hell...
        SetPreview(Prefab);
    }

    public void SetPreview(GameObject prefab)
    {
        Prefab = prefab;
        if (preview || preview != null)
        {
            Destroy(preview);
        }
        if (prefab == null) return;

        preview = Instantiate(Prefab, RenderSpace);

        RenderCamera.targetTexture = RenderTexture;
    }

    // Update is called once per frame
    void Update()
    {
        Destination.texture = RenderTexture;
    }
}
