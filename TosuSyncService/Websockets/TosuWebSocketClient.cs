using HsManCommonLibrary.ValueHolders;
using TosuSyncService.EventSystem;
using TosuSyncService.Model;
using WebSocket4Net;

namespace TosuSyncService.Websockets;

public class TosuWebSocketClient(string url = "ws://127.0.0.1:24050/websocket/v2"
    , TosuEventUpdater? eventUpdater = null,
    TosuEventManager? eventManager = null)
    : TosuWebsocketBase(url, eventUpdater, eventManager)
{
    private TosuData _last = new TosuData();
    private string _lastMessage = "";
    public ValueHolder<TosuData> Data { get; private set; } = new ValueHolder<TosuData>(new TosuData());
    protected override void OnMessageReceived(object? sender, MessageReceivedEventArgs args)
    {
        var message = args.Message;
        if (_lastMessage == message)
        {
            return;
        }

        var r = JsonObjectConverter<TosuData>.Instance.TryCreate(message, out var data);
        if (!r || data == null)
        {
            return;
        }

        _lastMessage = message;

        try
        {
            Data.SetValue(data);
            EventUpdater.UpdateEventAsync(_last, data).Wait();
            _last = data;
        }
        catch (Exception e)
        {
            // Ignored
        }
       
    }
}