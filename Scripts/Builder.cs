#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using BuildCLI.Internal;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildCLI {
    public static class Builder {
        private const string ArgumentPrefix = "--buildcli:";
        private const string InfoPrefix = ArgumentPrefix + "info:";

        private static string[] _args;

        private static string GetStringArgument(string key, string defaultValue) {
            for (int i = 0; i < _args.Length; i++) {
                if (_args[i] == ArgumentPrefix + key) {
                    return _args[i + 1];
                }
            }

            return defaultValue;
        }

        private static bool GetBoolArgument(string key, bool defaultValue) {
            for (int i = 0; i < _args.Length; i++) {
                if (_args[i] == ArgumentPrefix + key) {
                    int nextIndex = i + 1;

                    if (nextIndex < _args.Length && _args[nextIndex] == "false") {
                        return false;
                    }

                    return true;
                }
            }

            return defaultValue;
        }

        public static void Build() {
            _args = Environment.GetCommandLineArgs();

            // get main arguments
            BuildOptions options = BuildOptions.None;

            if (GetBoolArgument("development", false)) {
                options |= BuildOptions.Development;
            }

            if (GetBoolArgument("cleanBuildCache", false)) {
                options |= BuildOptions.CleanBuildCache;
            }

            if (GetBoolArgument("strictMode", true)) {
                options |= BuildOptions.StrictMode;
            }

            string outPath = GetStringArgument("outputPath", "./Build/");

            SerializableDictionary info = new SerializableDictionary();

            // get info:
            for (int i = 0; i < _args.Length; i++) {
                string arg = _args[i];

                if (arg.StartsWith(InfoPrefix)) {
                    info[arg.Remove(0, InfoPrefix.Length)] = _args[++i].Trim();
                }
            }

            string relativeResourcesPath = Path.Combine("Assets", BuildCLI.Build.TempDirectoryName, "Resources");
            string absoluteResourcesPath = ToAbsolutePath(relativeResourcesPath);

            if (!Directory.Exists(absoluteResourcesPath)) {
                Directory.CreateDirectory(absoluteResourcesPath);
            }

            string relativeFilePath = Path.Combine(relativeResourcesPath, BuildCLI.Build.BuildInfoFile + ".txt");

            File.WriteAllText(ToAbsolutePath(relativeFilePath), JsonUtility.ToJson(info));
            AssetDatabase.ImportAsset(relativeFilePath, ImportAssetOptions.ImportRecursive);

            BuildReport report = BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes.Where(e => e.enabled).Select(e => e.path).ToArray(),
                outPath,
                (BuildTarget)Enum.Parse(typeof(BuildTarget), GetStringArgument("target", "NoTarget")),
                options
            );

            AssetDatabase.DeleteAsset(Path.Combine("Assets", BuildCLI.Build.TempDirectoryName));

            if (report.summary.result == BuildResult.Succeeded) {
                Debug.Log($"Build successful - written to {outPath}");
            }
            else {
                Debug.LogError($"Build not successful, status: {report.summary.result.ToString().ToLower()}");
                EditorApplication.Exit(1);
            }
        }

        private static string ToAbsolutePath(string relativePath) {
            return Path.Combine(Application.dataPath, "..", relativePath);
        }
    }
}

#endif