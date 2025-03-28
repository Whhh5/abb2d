using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[EditorFieldName("禁止移动")]
public class EntityNoMoveBuffDataEditor : IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.NoMovement;
    public void InitParams(int[] arrParam)
    {
    }

    public void Draw()
    {
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}


[EditorFieldName("player buff")]
public class EntityPlayerBuffDataEditor : EntityPlayerBuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.PlayerBuff;
    public void InitParams(int[] arrParam)
    {
    }

    public void Draw()
    {
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}


public class EntityPoisonBuffDataEditor : EntityPoisonBuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.Poison;
    public void InitParams(int[] arrParam)
    {
    }

    public void Draw()
    {
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}
public class EntityPlayerSkill2DataEditor : EntityPoisonBuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.PlayerSkill2;
    public void InitParams(int[] arrParam)
    {
    }

    public void Draw()
    {
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}
public class EntityExpiosionDataEditor : EntityPoisonBuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.Expiosion;
    public void InitParams(int[] arrParam)
    {
    }

    public void Draw()
    {
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}
public class EntityExpiosion2DataEditor : EntityPoisonBuffData, IBuffDaraEditor
{
    public EnBuff Buff => EnBuff.Expiosion2;
    public void InitParams(int[] arrParam)
    {
    }

    public void Draw()
    {
    }

    public void GetStringData(ref List<int> data)
    {
        var index = data.Count;
        data.Insert(index, data.Count - index);
    }

    public void InitEditor()
    {
    }

}