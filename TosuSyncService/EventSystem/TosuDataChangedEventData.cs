using TosuSyncService.Model;

namespace TosuSyncService.EventSystem;

public class TosuDataChangedEventData<TData>
{
    public TosuDataChangedEventData(ValueChange<TData> dataValueChange, ValueChange<TosuData> gosuDataValueChange)
    {
        EventValueChange = dataValueChange;
        GosuDataChange = gosuDataValueChange;
    }

    public ValueChange<TData> EventValueChange { get; }

    public ValueChange<TosuData> GosuDataChange { get; }
    
    public DateTime CurrentTime { get; } = DateTime.Now;
}