using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib;

namespace VfxEditor.Data {
    public class CopyManager {
        public static bool IsCopying { get; private set; }
        public static bool IsPasting { get; private set; }

        public static readonly List<Vector4> CurveKeys = new();
        public static readonly Dictionary<string, AVFXBase> Copied = new();

        public static void Dispose() {
            CurveKeys.Clear();
            Copied.Clear();
        }

        public static void Reset() {
            IsCopying = false;
            IsPasting = false;
        }

        public static void Copy() {
            Copied.Clear();
            IsCopying = true;
        }

        public static void Paste() {
            IsPasting = true;
        }

        public static void ClearCurveKeys() {
            CurveKeys.Clear();
        }

        public static void AddCurveKey( float time, float x, float y, float z ) {
            CurveKeys.Add( new Vector4( time, x, y, z ) );
        }

        public static bool HasCurveKeys() => CurveKeys.Count > 0;
    }
}
