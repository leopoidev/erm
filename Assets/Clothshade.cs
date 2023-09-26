using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using System.IO;
using System.Runtime.InteropServices;

public class Clothshade : MonoBehaviour
{
    public ComputeShader moveShader;
    public ComputeBuffer InputBuffer;
    public ComputeBuffer ClothBuffer;
    public ComputeBuffer MovementBuffer;
    public ComputeBuffer PointBuffer;
    public ComputeBuffer ColorBuffer;
    public ComputeBuffer OutputDebugBuffer;
    public ComputeBuffer ReuseBuffer;
    public Mesh mesh;
    public Material material;
  
    public struct pointconnection
    {
    int2 pointid;
    float length;
    }  
    public float dtmodifier = 1;

   
    private int sqrvertexcount;
    private int ConnectKernel;
    private int MoveKernel;
    private int applyKernel;
    private int clearKernel;
    private int CompressKernel;
    public Vector3 pointselected;
    private ComputeBufferType buftype = ComputeBufferType.Structured;
    public float timestep;
    private void OnEnable()
    {
        Destroybuffers();
        ConnectKernel = moveShader.FindKernel("Connect");
        MoveKernel = moveShader.FindKernel("Move");
        clearKernel = moveShader.FindKernel("Clear");
        CompressKernel = moveShader.FindKernel("Compress");
        applyKernel = moveShader.FindKernel("Apply");

        //get the mesh from the mesh filter
        mesh = this.GetComponent<MeshFilter>().mesh;
        //sort the vertices by their x position and then their y position
        sortvertices();
        //get the positions of the vertices
        Vector3[] vertices = mesh.vertices;
        Vector3[] clear = new Vector3[vertices.Length];
        //set the size of the buffer to the number of vertices

        CreateBuffers(vertices);
        SetupBuffers(ConnectKernel);
        SetupBuffers(clearKernel);
        //square root of the number of vertices rounded up
        sqrvertexcount = Mathf.CeilToInt(Mathf.Sqrt(vertices.Length));
        moveShader.SetVector("size", new Vector2(sqrvertexcount, sqrvertexcount));
        Debug.Log(sqrvertexcount);
        moveShader.Dispatch(clearKernel, sqrvertexcount, sqrvertexcount, 1);
        moveShader.Dispatch(clearKernel, sqrvertexcount, sqrvertexcount, 1);
        moveShader.Dispatch(clearKernel, sqrvertexcount, sqrvertexcount, 1);
        moveShader.Dispatch(ConnectKernel, sqrvertexcount, sqrvertexcount, 1);
        var CommandBuffer = new CommandBuffer();
    }
    private void sortvertices()
    {
          // Get the vertices
        Vector3[] vertices = mesh.vertices;

        // Create a custom class to hold the original position and index
        VertexWithIndex[] verticesWithIndices = new VertexWithIndex[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            verticesWithIndices[i] = new VertexWithIndex(vertices[i], i);
        }

        // Sort the vertices by X and then Y positions
        System.Array.Sort(verticesWithIndices, (a, b) =>
        {
            if (a.Position.x != b.Position.x)
                return a.Position.x.CompareTo(b.Position.x);
            return a.Position.y.CompareTo(b.Position.y);
        });

        // Update the vertices array with the sorted positions
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = verticesWithIndices[i].Position;
        }

        // Apply the updated vertices to the mesh
        mesh.vertices = vertices;

        // Redo the triangles based on the new vertex order
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i++)
        {
            for (int j = 0; j < verticesWithIndices.Length; j++)
            {
                if (triangles[i] == verticesWithIndices[j].Index)
                {
                    triangles[i] = j;
                    break;
                }
            }
        }

        // Apply the updated triangles to the mesh
        mesh.triangles = triangles;

        // Recalculate normals and other mesh properties if necessary
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
       private class VertexWithIndex
    {
        public Vector3 Position;
        public int Index;

        public VertexWithIndex(Vector3 position, int index)
        {
            Position = position;
            Index = index;
        }
    }
    private void SetupBuffers(int kernel)
    {
        moveShader.SetBuffer(kernel, nameof(InputBuffer), InputBuffer);
        moveShader.SetBuffer(kernel, nameof(MovementBuffer), MovementBuffer);
        moveShader.SetBuffer(kernel, nameof(PointBuffer), PointBuffer);
        moveShader.SetBuffer(kernel, nameof(ClothBuffer), ClothBuffer);
        moveShader.SetBuffer(kernel, nameof(ColorBuffer), ColorBuffer);
        moveShader.SetBuffer(kernel, nameof(OutputDebugBuffer), OutputDebugBuffer);
        moveShader.SetBuffer(kernel, nameof(ReuseBuffer), ReuseBuffer);
    }
    struct erm
    {
        public float3 pos;
        public float3 vel;
    }
    public float adjustvalue;
    private void CreateBuffers(Vector3[] vertices)
    {
        InputBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3, buftype);
        MovementBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 6, buftype);
        PointBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 6, buftype);

        ClothBuffer = new ComputeBuffer(vertices.Length, ((sizeof(int) *2) + (sizeof(float) *3)) *12, buftype);
        ColorBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 4,buftype);
        OutputDebugBuffer = new ComputeBuffer(12, sizeof(float) * 3, buftype);
        ReuseBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3, buftype);
        InputBuffer.SetData(vertices);
        Bounds bounds = mesh.bounds;
        bounds.Expand(1000);
        mesh.bounds = bounds;

        //create empty arrays for the buffers
        erm[] movement = new erm[vertices.Length];
        erm[] points = new erm[vertices.Length];
        pointconnection[] cloth = new pointconnection[vertices.Length];
        Vector4[] colors = new Vector4[vertices.Length];
        //set the buffers to the arrays
        MovementBuffer.SetData(movement);
    
       
        ColorBuffer.SetData(colors);
        //set the buffers of the material
        
    }
    public int itvalue;
    float dtprev;
    private void Update()
    {
     
        
           //set the shader to the compute shader
        SetupBuffers(MoveKernel);
        SetupBuffers(CompressKernel);
        
        //set the number of threads to the number of vertices
        moveShader.SetFloat("mod",dtmodifier);
        moveShader.SetFloat("dtprev", dtprev);
        dtprev = Time.deltaTime / dtmodifier;
        moveShader.SetFloat("adjustvalue", adjustvalue);
        moveShader.SetVector("pointselected", new Vector3(pointselected.x, pointselected.y, pointselected.z));
        moveShader.Dispatch(MoveKernel, sqrvertexcount, sqrvertexcount, 1); 
        SetupBuffers(applyKernel);
        moveShader.Dispatch(applyKernel, sqrvertexcount, sqrvertexcount, 1);
        for(int i = 0; i < itvalue; i++)
        {
        moveShader.Dispatch(CompressKernel, sqrvertexcount, sqrvertexcount, 1);
        }
       
        

        //set the buffers of the material 
        material.SetBuffer("PointBuffer", PointBuffer);
        material.SetBuffer("ColorBuffer", ColorBuffer);
        //get the data from the output buffer
        Vector3[] output = new Vector3[12];
        OutputDebugBuffer.GetData(output);
        Debug.Log(output[1]);

    }
    private void OnDestroy()
    {
        Destroybuffers();
    }
    private void OnApplicationQuit()
    {
        Destroybuffers();
    }
    private void OnDisable()
    {
        Destroybuffers();
    }
    private void Destroybuffers()
    {
        //release the buffers
        if (InputBuffer != null)
        InputBuffer.Release();
        if (MovementBuffer != null)
        MovementBuffer.Release();
        if (PointBuffer != null)
        PointBuffer.Release();
        if (ClothBuffer != null)
        ClothBuffer.Release();
        if (ColorBuffer != null)
        ColorBuffer.Release();
        if (mesh != null)
        mesh = null;
    }

}
