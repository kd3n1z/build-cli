#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildCLI {
    public static class Builder {
        private const string ArgumentPrefix = "--buildcli:";

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

            BuildReport report = BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes.Where(e => e.enabled).Select(e => e.path).ToArray(),
                outPath,
                (BuildTarget)Enum.Parse(typeof(BuildTarget), GetStringArgument("target", "NoTarget")),
                options
            );

            if (report.summary.result == BuildResult.Succeeded) {
                Debug.Log($"Build successful - written to {outPath}");
            }
            else {
                Debug.LogError($"Build not successful, status: {report.summary.result.ToString().ToLower()}");
                EditorApplication.Exit(1);
            }
        }
    }
}

#endif
