using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeTool
{
    public ComputeShader CommonShader { get; set; }
    public ComputeBuffer InputBuffer { get; set; }

    public static ComputeTool CreateComputeTool()
    {
        ComputeTool tool = new ComputeTool();

        return tool;
    }
    

}
