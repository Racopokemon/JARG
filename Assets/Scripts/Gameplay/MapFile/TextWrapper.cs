[System.Serializable]
public class TextWrapper : MoveWrapper
{
    public float start, end;
    public string text;

    public TextWrapper() { }

    public override Driver CreateDriver()
    {
        return new TextDriver(this);
    }
}