using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MapDataSO", menuName = "SO/MapData", order = 0)]
public class MapDataSO : ScriptableObject
{
    public int weight; // x
    public int height; // z
    public Vector3 startPoint;
    public Vector3 chunkSize;

    // 起点终点位置
    public float startPosZ;
    public float endPosZ;

    // 障碍物
    public int barrierColCount; // 障碍物列数
    public int barrierRowCount; // 障碍物行数
    public float barrierStartDis; // 第一个障碍物距离起点多少距离
    public float barrierEndDis; // 最后一个障碍物距离终点多少距离
    public Vector3 barrierSize; // 障碍物大小

    public int GetRowCount()
    {
        var result = Mathf.FloorToInt(height / chunkSize.z);
        return result;
    }
    public int GetColCount()
    {
        var result = Mathf.FloorToInt(weight / chunkSize.x);
        return result;
    }
    public Vector3 GetMapPos(int row, int col)
    {
        var x = (col + 0.5f) * chunkSize.x;
        var z = (row + 0.5f) * chunkSize.y;
        var y = chunkSize.y * -0.5f;
        return startPoint + new Vector3(x, y, z);
    }
    public float GetBarrierIntervalZ()
    {
        var barLength = Mathf.Abs(height - endPosZ - startPosZ - barrierEndDis - barrierStartDis);
        var interval = barLength / Mathf.Max(1, barrierRowCount - 1);
        return interval;
    }
    public float GetBarrierIntervalX()
    {
        var barWeight = weight - barrierSize.x;
        var intervalX = barWeight / Mathf.Max(1, barrierColCount - 1);
        return intervalX;
    }
    public Vector3 GetBarrierPos(int row, int col)
    {
        var intercalZ = GetBarrierIntervalZ();
        var intercalX = GetBarrierIntervalX();
        var posZ = intercalZ * row + startPoint.z + startPosZ + barrierStartDis;
        var posX = (col - (barrierColCount - 1) * 0.5f) * intercalX + (weight * 0.5f + startPoint.x);
        return new Vector3(posX, startPoint.y, posZ);
    }
    public Vector3 GetPlayerStartPos()
    {
        return startPoint + new Vector3(weight * 0.5f, 0, startPosZ - 0.3f);
    }
}