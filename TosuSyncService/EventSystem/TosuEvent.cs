using HsManEventSystem.Events;

namespace TosuSyncService.EventSystem;

public class TosuEvent<T> : GenericEvent<TosuDataChangedEventData<T>>
{
}