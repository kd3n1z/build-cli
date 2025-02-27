using BuildCLI.Internal;
using UnityEngine;

namespace BuildCLI {
    public static class Build {
        private const string Prefix = "__" + nameof(BuildCLI) + "_";
        internal const string TempDirectoryName = Prefix + "temp";
        internal const string BuildInfoFile = Prefix + "buildInfo";

        private static bool _initialized = false;
        private static SerializableDictionary _info;

        public static SerializableDictionary Info {
            get {
                if (!_initialized) {
                    _initialized = true;

                    _info = new SerializableDictionary();

                    try {
                        TextAsset resource = Resources.Load<TextAsset>(BuildInfoFile);
                        _info = JsonUtility.FromJson<SerializableDictionary>(resource.text);
                    }
                    catch {
                        // ignored
                    }
                }

                return _info;
            }
        }
    }
}