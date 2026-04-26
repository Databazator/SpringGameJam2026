using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    private const string ProductName = "SpringGameJam2026";
    private const string OutputRoot = "Builds";

    private static string[] EnabledScenes =>
        EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

    public static void BuildAll()
    {
        BuildWindows();
        BuildLinux();
        BuildMac();
        BuildWebGL();
    }

    public static void BuildWindows()
    {
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.IL2CPP);
        Run(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, OutPath("Windows", $"{ProductName}.exe"));
    }

    public static void BuildLinux()
    {
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.IL2CPP);
        Run(BuildTarget.StandaloneLinux64, BuildTargetGroup.Standalone, OutPath("Linux", $"{ProductName}.x86_64"));
    }

    public static void BuildMac()
    {
        // Cross-compile from non-mac host = Mono only. IL2CPP for OSX needs Xcode toolchain.
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        Run(BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone, OutPath("Mac", $"{ProductName}.app"));
    }

    public static void BuildWebGL()
    {
        Run(BuildTarget.WebGL, BuildTargetGroup.WebGL, OutPath("WebGL", string.Empty));
    }

    private static string OutPath(string platform, string artifact)
    {
        var dir = Path.GetFullPath(Path.Combine(OutputRoot, platform));
        Directory.CreateDirectory(dir);
        return string.IsNullOrEmpty(artifact) ? dir : Path.Combine(dir, artifact);
    }

    private static void Run(BuildTarget target, BuildTargetGroup group, string locationPath)
    {
        var scenes = EnabledScenes;
        if (scenes.Length == 0)
        {
            Debug.LogError("[BuildScript] No enabled scenes in EditorBuildSettings — abort.");
            EditorApplication.Exit(2);
            return;
        }

        var opts = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = locationPath,
            target = target,
            targetGroup = group,
            options = BuildOptions.None,
        };

        Debug.Log($"[BuildScript] Building {target} -> {locationPath}");
        var report = BuildPipeline.BuildPlayer(opts);
        var summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[BuildScript] {target} OK. Size {summary.totalSize} bytes. Time {summary.totalTime}.");
        }
        else
        {
            Debug.LogError($"[BuildScript] {target} FAILED: {summary.result}. Errors: {summary.totalErrors}.");
            EditorApplication.Exit(1);
        }
    }
}
