using System;
using Fleck;

namespace OsuLoader.Api.Websocket;

public class WebsocketController
{
    private readonly IWebSocketConnection _websocket;
    
    public WebsocketController(IWebSocketConnection connection)
    {
        _websocket = connection;
        connection.OnOpen = OnClientOpen;
        connection.OnClose = OnClientClose;
        connection.OnMessage = OnClientMessage;
        connection.OnError = OnClientError;
    }
    
    private async void OnClientOpen()
    {
        // Console.WriteLine($"{_websocket.ConnectionInfo.Host} is open");
    }

    private async void OnClientClose()
    {
        // Console.WriteLine($"{_websocket.ConnectionInfo.Host} is closed");
    }

    private async void OnClientMessage(string message)
    {
        var command = WebsocketCommand.TryParse(message);
        var parseResult = command != null;
        var executeResult = command?.Execute() ?? false;
        if (parseResult && executeResult)
            await _websocket.Send($"command \"{command.Command}\" successfully executed.");
        else if (parseResult)
            await _websocket.Send($"command \"{command.Command}\" failed.");
        else
            await _websocket.Send($"parse command failed.");
    }

    private async void OnClientError(Exception ex)
    {
        Console.Error.WriteLine(ex);
    }
}