#nullable enable

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using Mono.Cecil;
using Mono.Cecil.Cil;
using OsuLoader.Api;

namespace OsuLoader
{
    public static class LoaderMain
    {
        internal static string OsuOriginalPath =>
            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}" +
            @"\osu!\osu!-backup.exe";
        
        private static string OsuModifiedPath =>
            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}" +
            @"\osu!\osu!-modified.exe";
        
        internal static string OsuExePath =>
            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}" +
            @"\osu!\osu!.exe";

        private static string OsuDirectory =>
            Path.GetDirectoryName(OsuExePath)!;
        
        private static readonly DefaultAssemblyResolver Resolver = new();
        
        private static readonly ReaderParameters Parameters = new()
        {
            AssemblyResolver = Resolver
        };
        
        internal static Assembly OsuAssembly =>
            Assembly.LoadFrom(OsuExePath);
        
        private static AssemblyDefinition OsuAssemblyDefinition =>
            AssemblyDefinition.ReadAssembly(OsuExePath, Parameters);
        
        // private static AssemblyDefinition CurrentAssembly =>
        //     AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location);
        
        private static Harmony? Harmony { get; set; }

        public static void Main(string[] args)
        {
            Resolver.AddSearchDirectory(OsuDirectory);
            CheckOsuFiles();
            ModifyTargetAssembly();
            StartTargetAssembly();
        }

        public static void OsuMain()
        {
            Console.WriteLine("Initializing...");
            ApiManager.Run();
            BlockUpdate();
            PatchAll();
            
            var thread = new Thread(OsuManager.Start);
            thread.IsBackground = true;
            thread.Start();
        }

        private static void BlockUpdate()
        {
            if (Directory.Exists(Path.Combine(OsuDirectory, "_pending")))
                Directory.Delete(Path.Combine(OsuDirectory, "_pending"), true);
        }

        private static void CheckOsuFiles()
        {
            if (File.Exists(Path.Combine(OsuDirectory, "_pending", "osu!.exe")))
                File.Copy(Path.Combine(OsuDirectory, "_pending", "osu!.exe"), OsuExePath, true);
            if (!File.Exists(OsuOriginalPath))
                File.Copy(OsuExePath, OsuOriginalPath, true);
            File.Copy(OsuOriginalPath, OsuExePath, true);
            File.Copy(Assembly.GetExecutingAssembly().Location, OsuDirectory + @"\OsuLoader.exe", true);
        }

        private static void ModifyTargetAssembly()
        {
            using (AssemblyDefinition assembly = OsuAssemblyDefinition)
            {
                var mainClass = assembly.MainModule.GetType("\u0023\u003DzYxzv\u00242KB0p4kxDf8GucDjzIfehSk");
                var mainMethod = mainClass.Methods.FirstOrDefault(m => m.Name == "\u0023\u003Dz6TB7efI\u003D");
                
                if (mainMethod is null)
                    Environment.Exit(1);
                
                var processor = mainMethod!.Body.GetILProcessor();
                var patchMethod = assembly.MainModule.ImportReference(
                    typeof(LoaderMain).GetMethod("OsuMain", BindingFlags.Public | BindingFlags.Static));
                
                processor.InsertBefore(mainMethod.Body.Instructions[0], Instruction.Create(OpCodes.Call, patchMethod));

                assembly.MainModule.Kind = ModuleKind.Console;
                assembly.Write(OsuModifiedPath);
            }

            File.Copy(OsuModifiedPath, OsuExePath, true);
        }

        private static void StartTargetAssembly()
        {
            var info = new ProcessStartInfo(OsuExePath)
            {
                UseShellExecute = false,
            };
            var process = Process.Start(info)!;
            process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
            process.ErrorDataReceived += (sender, args) => Console.Error.WriteLine(args.Data);
            process.WaitForExit();
            var code = process.ExitCode;
            Environment.Exit(code);
        }

        private static void PatchAll()
        {
            File.WriteAllText(Path.Combine(OsuDirectory, "debug.txt"), 
                $"PatchAll executed at {DateTime.Now}");
            Harmony ??= new Harmony("OsuLoader");
            Harmony.PatchAll();
        }
    }
}