using AVFXLib.Models;
using System.Collections.Generic;
using System.Numerics;

namespace VFXEditor.Data {

    public class CopyManager {
        public static bool IsCopying { get; private set; }
        public static bool IsPasting { get; private set; }

        public static List<Vector4> CurveKeys { get; private set; }
        public static Dictionary<string, Base> Copied { get; private set;  }

        public static void Initialize() {
            CurveKeys = new();
            Copied = new();
        }

        public static void Dispose() {
            CurveKeys.Clear();
            Copied.Clear();
        }

        // ======== COPY ==============
        public static void PreDraw() {
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

        // ======== CURVE KEYS ===============

        public static void ClearCurveKeys() {
            CurveKeys.Clear();
        }

        public static void AddCurveKey(float time, float x, float y, float z) {
            CurveKeys.Add( new Vector4( time, x, y, z ) );
        }

        public static bool HasCurveKeys() {
            return CurveKeys.Count > 0;
        }
    }
}
