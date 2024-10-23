using UnityEngine;
using System.Collections;

public class LoadTargetData : GameUtil
{
	private Object m_Object;

	public Object GetObject()
	{
		return m_Object;
    }
	public void SetObject(Object obj)
	{
		m_Object = obj;
    }
}

