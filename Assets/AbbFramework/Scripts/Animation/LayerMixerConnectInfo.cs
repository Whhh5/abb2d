

public enum EnAnimLayerStatus
{
    None,
    Entering,
    Playing,
    Exiting,
    Nothing,
}
public class LayerMixerConnectInfo : IClassPool
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

    public void OnPoolInit<T>(ref T userData) where T : IClassPoolUserData
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}