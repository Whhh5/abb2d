using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extend
{
    public static T CreatePlayableAdapter<T>(this PlayableGraphAdapter graph)
        where T : PlayableAdapter, new()
    {
        var data = new T();
        data.Initialization(graph);
        return data;
    }
}
