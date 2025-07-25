﻿#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
                if (_parameters.Count != 2)
                    return false;
                OsuManager.MoveMouse(new Vector2(_parameters[0].AsNumber().Float, _parameters[1].AsNumber().Float));
                break;
            case "press":
                if (OsuManager.CurrentPlayMode != OsuModes.Play)
                    return false;
                OsuManager.KeepingPressKeyBinding(OsuManager.KeyBindings.Instance[_parameters[0].AsString().Value]);
                break;
            case "release":
                if (OsuManager.CurrentPlayMode != OsuModes.Play)
                    return false;
                OsuManager.ReleasePressingKeyBinding(OsuManager.KeyBindings.Instance[_parameters[0].AsString().Value]);
                break;
            case "restart":
                if (OsuManager.CurrentPlayMode != OsuModes.Play)
                    return false;
                OsuManager.PressKeyBinding(OsuManager.KeyBindings.Instance["QuickRetry"]);
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