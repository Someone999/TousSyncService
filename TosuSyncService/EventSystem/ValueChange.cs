namespace TosuSyncService.EventSystem;

public class ValueChange<T>
{
    public ValueChange(T? oldValue, T? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public T? OldValue { get; set; }
    public T? NewValue { get; set; }
}