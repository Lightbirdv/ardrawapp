using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LineData 
{
    public Vector3[] positions;
    public int positionCount;

    public LineData(LineRenderer lr)
    {
        positionCount = lr.positionCount;
        positions = new Vector3[positionCount];
        lr.GetPositions(positions);
    }
}
