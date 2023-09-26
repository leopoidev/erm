using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothGen : MonoBehaviour
{
    public Mesh mesh;
    public Vector2Int PlaneSize = new Vector2Int(10, 10);

    void Start()
    {
        GenerateMesh();
    }
    static Vector3[] To1DArray(Vector3[,] array2D)
    {
        Vector3[] array1D = new Vector3[array2D.GetLength(0) * array2D.GetLength(1)];
        int index = 0;
        for (int x = 0; x < array2D.GetLength(0); x++)
        {
            for (int y = 0; y < array2D.GetLength(1); y++)
            {
                array1D[index++] = array2D[x, y];
            }
        }
        return array1D;
    }
    void GenerateMesh()
    {
        //generate a 10x10 vertex array
        Vector3[,] vertices = new Vector3[PlaneSize.x, PlaneSize.y];
        //populate the vertex array
        for (int x = 0; x < PlaneSize.x; x++)
        {
            for (int y = 0; y < PlaneSize.y; y++)
            {
                vertices[x, y] = new Vector3(x, y, 0);
            }
        }
        //generate the triangles 
        int[] triangles = new int[(PlaneSize.x - 1) * (PlaneSize.y - 1) * 6];
        int t = 0;
        //loop trough triangles
        for (int x = 0; x < PlaneSize.x - 1; x++)
        {
            for (int y = 0; y < PlaneSize.y - 1; y++)
            {
                //every other triangle
                if ((x + y) % 2 == 0)
                {
                    //triangle 1
                    triangles[t++] = x + y * PlaneSize.x;
                    triangles[t++] = x + (y + 1) * PlaneSize.x;
                    triangles[t++] = (x + 1) + y * PlaneSize.x;
                    //triangle 2
                    triangles[t++] = x + (y + 1) * PlaneSize.x;
                    triangles[t++] = (x + 1) + (y + 1) * PlaneSize.x;
                    triangles[t++] = (x + 1) + y * PlaneSize.x;
                }
                else
                {
                    //triangle 1
                    triangles[t++] = x + y * PlaneSize.x;
                    triangles[t++] = x + (y + 1) * PlaneSize.x;
                    triangles[t++] = (x + 1) + (y + 1) * PlaneSize.x;
                    //triangle 2
                    triangles[t++] = x + y * PlaneSize.x;
                    triangles[t++] = (x + 1) + (y + 1) * PlaneSize.x;
                    triangles[t++] = (x + 1) + y * PlaneSize.x;
                }
            }
        }
        //create a new mesh
        mesh = new Mesh();
        //set the vertices and triangles
        mesh.vertices = To1DArray(vertices);
        mesh.triangles = triangles;
        //set the mesh to the mesh filter
        GetComponent<MeshFilter>().mesh = mesh;
    }

}
