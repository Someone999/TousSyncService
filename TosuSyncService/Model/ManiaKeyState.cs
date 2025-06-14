namespace TosuSyncService.Model;

public class ManiaKeyState
{
    public int KeyCode { get; set; }
    public bool IsPressed { get; set; }
    public override string ToString()
    {
        return $"{KeyCode}({IsPressed})";
    }
}