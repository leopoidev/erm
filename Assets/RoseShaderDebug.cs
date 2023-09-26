using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseShaderDebug : MonoBehaviour
{
    private RenderTexture renderTexture;
    public Vector2Int size;
    public ComputeShader RenderDebugShader;
    public Material material;
    public GameObject RenderPlane;
    // Start is called before the first frame update
    void Start()
    {
        //get the buffer from the Clothshade script
        ComputeBuffer buffer = this.GetComponent<Clothshade>().PointBuffer;
        //create a render texture with the same size as the buffer
        renderTexture = new RenderTexture(size.x, size.y, 24);
        //set the format of the render texture to float
        renderTexture.format = RenderTextureFormat.ARGBFloat;
        //enable random write
        renderTexture.enableRandomWrite = true;
        //create the render texture
        renderTexture.Create();
        material.SetTexture("_BaseMap", renderTexture);
        //set the render texture to the ui raw image
        RenderPlane.GetComponent<MeshRenderer>().material = material;
        RenderPlane.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", renderTexture);
    }

    // Update is called once per frame
    void Update()
    {
        //set the shader to the compute shader
        RenderDebugShader.SetBuffer(0, "buffer", this.GetComponent<Clothshade>().PointBuffer);
        //set the render texture
        RenderDebugShader.SetTexture(0, "Result", renderTexture);
        //set the size of the render texture
        RenderDebugShader.SetInt("sizex", size.x);
        RenderDebugShader.SetInt("sizey", size.y);
        //set the number of threads
        RenderDebugShader.Dispatch(0, size.x / 8, size.y / 8, 1);
        
    }
}
