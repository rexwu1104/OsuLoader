#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using osu;

namespace OsuLoader.Api.Websocket;

public class WebsocketCommand
{
    private static List<string> _availableCommands =
    [
        "random",
        "play",
        "move",
        "press",
        "release",
        "restart"
    ];

    public readonly string Command;
    private readonly List<Json.JsonElement> _parameters;

    private WebsocketCommand(string command, params Json.JsonElement[] parameters)
    {
        Command = command;
        _parameters = [..parameters];
    }

    public bool Execute()
    {
        switch (Command)
        {
            case "random":
                if (OsuManager.CurrentPlayMode != OsuModes.SelectPlay)
                    return false;
                OsuManager.PressKeyBoard(Keys.F2);
                break;
            case "play":
                if (OsuManager.CurrentPlayMode != OsuModes.SelectPlay)
                    return false;
                OsuManager.PressKeyBoard(Keys.Enter);
                break;
            case "move":
                break;
            case "press":
                break;
            case "release":
                break;
            case "restart":
                break;
        }
        
        return true;
    }
    
    public static WebsocketCommand? TryParse(string rawCommand)
    {
        var json = Json.Parse(rawCommand);
        var command = json["command"];
        if (command is not null && _availableCommands.Contains(command.AsString().Value))
        {
            return new WebsocketCommand(json["command"].ToString(), json["params"]?.AsArray()?.Values ?? []);
        }
        
        return null;
    }
}