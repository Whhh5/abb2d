

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
}