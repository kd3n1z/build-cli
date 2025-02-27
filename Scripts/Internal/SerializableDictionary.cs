using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace BuildCLI.Internal {
    [Serializable]
    public class SerializableDictionary {
        [SerializeField] private List<SerializableKeyValuePair> data = new List<SerializableKeyValuePair>();

        [CanBeNull]
        public string this[string key] {
            get {
                foreach (SerializableKeyValuePair kvp in data) {
                    if (kvp.k == key) {
                        return kvp.v;
                    }
                }

                return null;
            }
            internal set {
                foreach (SerializableKeyValuePair kvp in data) {
                    if (kvp.k == key) {
                        kvp.v = value;
                        return;
                    }
                }

                data.Add(new SerializableKeyValuePair() {
                    k = key,
                    v = value
                });
            }
        }

        [Serializable]
        private class SerializableKeyValuePair {
            [SerializeField] internal string k;
            [SerializeField] internal string v;
        }
    }
}