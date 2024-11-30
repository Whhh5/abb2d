using System;
public interface IABBEventExecute
{
    public void EventExecute(EnABBEvent enEvent, int sourceID, int type, object userData);
}

