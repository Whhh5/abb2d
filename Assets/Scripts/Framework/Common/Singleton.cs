using UnityEngine;
using System.Collections;

public abstract class Singleton<T>
	where T: class, new()
{
	public static T Instance = new();

	public virtual void Awake()
	{

	}
	public virtual void OnEnable()
	{

	}
	public virtual void Start()
	{

    }
    public virtual void OnDisable()
    {

    }
	public virtual void Destroy()
	{

	}
}

