#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using osu_common;
using osu;
using OsuLoader.Api;
using OsuLoader.Patches;

namespace OsuLoader;

public static class OsuManager
{
    internal static object mainForm;

    private static PlayModes _currentPlayMode;
    private static int _currentScore;
    private static float _currentAccuracy;
    private static int _currentCombo;
    private static ushort _current300S;
    private static ushort _current100S;
    private static ushort _current50S;
    private static ushort _currentPerfect;
    private static ushort _currentGood;
    private static ushort _currentMisses;
    
    internal static List<Keys> _keepPressedKeys = new();

    private static object StateManger
    {
        get
        {
            var type = LoaderMain.OsuAssembly.GetType("#=zP$TXuW9OveBuc$n8B_U9kWYQBsfm");
            var instance = Reflector.GetStaticField<object>(type, "#=zt8U7bcQ=");
            return instance;
        }
    }

    private static object PlayManager
    {
        get
        {
            var type = LoaderMain.OsuAssembly.GetType("#=zdEqWFlrNfRD97Tqz0Q==");
            var instance = Reflector.GetStaticField<object>(type, "#=zpwvpAUg$TP_so3ltiA==");
            return instance;
        }
    }

    private static Vector2 TransformPosition(Vector2 position)
    {
        var type = LoaderMain.OsuAssembly.GetType("#=zdEqWFlrNfRD97Tqz0Q==");
        var field = Reflector.GetStaticField<object>(type, "#=zwYyy9Y2uYl$5");
        var result = Reflector.CallMethod<Vector2>(field, "#=zxNc4ZyAbDHNb", position);
        return result;
    }

    public static OsuModes CurrentPlayMode =>
        Reflector.GetStaticField<OsuModes>(mainForm.GetType(), "#=zNhmtVIU=");

    public class KeyBindings
    {
        public static KeyBindings Instance = new KeyBindings();
        
        public object this[string key]
        {
            get
            {
                var type = LoaderMain.OsuAssembly.GetType("osu.Input.Bindings");
                var field = Reflector.GetStaticField<object>(type, key);
                return field;
            }
        }
    }

    public static void MoveMouse(Vector2 position)
    {
        var type = LoaderMain.OsuAssembly.GetType("#=zd67Vp1YN0U1ubMsGhQ==");
        Reflector.SetStaticField(type, "#=zUInTg9oi8M61", position);
        Reflector.SetStaticField(type, "#=znb$YZO4mzuMMNqNOJE4nVus=", TransformPosition(position));
    }

    public static void KeepingPressKeyBoard(Keys key)
    {
        _keepPressedKeys.Add(key);
    }

    public static void ReleasePressingKeyBoard(Keys key)
    {
        _keepPressedKeys.Remove(key);
    }

    public static void PressKeyBoard(Keys key)
    {
        MainPatch.KeysQueue.Add(key);
    }

    public static void KeepingPressKeyBinding(object binding)
    {
        var type = LoaderMain.OsuAssembly.GetType("#=zNywshvqVt1sGrY2HKQ==");
        var result = Reflector.CallStaticMethod<Keys>(type, "#=zQKXvVUE=", binding);
        KeepingPressKeyBoard(result);
    }

    public static void ReleasePressingKeyBinding(object binding)
    {
        var type = LoaderMain.OsuAssembly.GetType("#=zNywshvqVt1sGrY2HKQ==");
        var result = Reflector.CallStaticMethod<Keys>(type, "#=zQKXvVUE=", binding);
        ReleasePressingKeyBoard(result);
    }

    public static void PressKeyBinding(object binding)
    {
        var type = LoaderMain.OsuAssembly.GetType("#=zNywshvqVt1sGrY2HKQ==");
        var result = Reflector.CallStaticMethod<Keys>(type, "#=zQKXvVUE=", binding);
        PressKeyBoard(result);
    }

    public static void Start()
    {
        var type = LoaderMain.OsuAssembly.GetType("#=zdEqWFlrNfRD97Tqz0Q==");
        var field = type.GetField("#=zt8U7bcQ=", BindingFlags.Static | BindingFlags.NonPublic);
        while (true)
        {
            Thread.Sleep(100);
            if (field!.GetValue(null) != null)
                break;
        }
        
        mainForm = field.GetValue(null);
        Console.WriteLine($"Initialized.");

        while (true)
        {
            if (_keepPressedKeys.Count > 0 && CurrentPlayMode != OsuModes.Play)
                _keepPressedKeys.Clear();
            
            Thread.Sleep(10);
        }
    }

    public static ScoreData GetScoreData()
    {
        return new ScoreData(
            _currentScore, _currentAccuracy, _currentCombo,
            _current300S, _current100S, _current50S,
            _currentPerfect, _currentGood, _currentMisses);
    }

    internal static bool UpdateMode(PlayModes mode)
    {
        if (mode == _currentPlayMode)
            return false;
        _currentPlayMode = mode;
        return true;
    }

    internal static bool Update(
        int score, float accuracy, int combo,
        ushort __300, ushort __100, ushort __50,
        ushort perfect, ushort good, ushort misses)
    {
        if (score != _currentScore || accuracy != _currentAccuracy || combo != _currentCombo ||
            __300 != _current300S || __100 != _current100S || __50 != _current50S ||
            perfect != _currentPerfect || good != _currentGood || misses != _currentMisses)
        {
            _currentScore = score;
            _currentAccuracy = accuracy;
            _currentCombo = combo;
            _current300S = __300;
            _current100S = __100;
            _current50S = __50;
            _currentPerfect = perfect;
            _currentGood = good;
            _currentMisses = misses;
            return true;
        }
        return false;
    }

    internal static void Reset()
    {
        _currentScore = 0;
        _currentAccuracy = 100;
        _currentCombo = 0;
        _current300S = 0;
        _current100S = 0;
        _current50S = 0;
        _currentPerfect = 0;
        _currentGood = 0;
        _currentMisses = 0;
    }
}