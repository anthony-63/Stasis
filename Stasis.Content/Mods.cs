public class Mods
{
    public float Speed = 1f;
    public float StartFrom = 0f;
    public bool NoFail = false;
    public bool VisualMap = false;


    public void Serialize(FileStream stream)
    {
        stream.Write(BitConverter.GetBytes(Speed));
        stream.Write(BitConverter.GetBytes(NoFail));
    }
}