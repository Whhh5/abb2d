

public enum EnAnimLayerStatus
{
    None,
    Entering,
    Playing,
    Exiting,
    Nothing,
}
public class LayerMixerConnectInfo : IGamePool
{
    public int port;
    public EnAnimLayer layer;
    public EnEntityCmd cmd;

    public void OnPoolDestroy()
    {
        port = -1;
        layer = EnAnimLayer.None;
        cmd = EnEntityCmd.None;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(CustomPoolData userData)
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}