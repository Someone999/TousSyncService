using TosuSyncService.EventSystem;
using WebSocket4Net;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;

namespace TosuSyncService.Websockets;

public abstract class TosuWebsocketBase
{
    protected WebSocket Socket;
    protected TosuEventUpdater EventUpdater { get; }
    public TosuEventManager EventManager { get; }
    public string ClientId { get; set; } = "";
    protected TosuWebsocketBase(string url, 
        TosuEventUpdater? eventUpdater = null,
        TosuEventManager? eventManager = null,
        bool registerDefaultEvents = true)
    {
        Socket = new WebSocket(url);
        EventManager = eventManager ?? new TosuEventManager();
        EventUpdater = eventUpdater ?? new TosuEventUpdater(EventManager);
        if (!registerDefaultEvents)
        {
            return;
        }
        
        Socket.Closed += OnDisconnected;
        Socket.Opened += OnConnected;
        Socket.Error += OnError;
        Socket.DataReceived += OnDataReceived;
        Socket.MessageReceived += OnMessageReceived;
    }

    public void Connect()
    {
        if (Socket.State == WebSocketState.Open)
        {
            return;
        }
        
        Socket.Open();
    }

    public void Disconnect()
    {
        if (Socket.State == WebSocketState.Closed)
        {
            return;
        }
        
        Socket.Close();
    }

    protected virtual void OnConnected(object? sender, EventArgs args)
    {
    }

    protected virtual void OnDisconnected(object? sender, EventArgs args)
    {
    }

    protected virtual void OnMessageReceived(object? sender, MessageReceivedEventArgs args)
    {
    }
    
    protected virtual void OnDataReceived(object? sender, DataReceivedEventArgs args)
    {
    }

    protected virtual void OnError(object? sender, ErrorEventArgs args)
    {
    }
}