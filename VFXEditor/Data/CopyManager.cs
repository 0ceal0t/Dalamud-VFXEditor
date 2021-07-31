using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.UI.VFX;

namespace VFXEditor.Data {
    public class CopyManager {
        private static List<Vector4> CurveKeys;

        public static void Initialize() {
            CurveKeys = new();
        }

        public static void ClearCurveKeys() {
            CurveKeys.Clear();
        }

        public static void AddCurveKey(float time, float x, float y, float z) {
            CurveKeys.Add( new Vector4( time, x, y, z ) );
        }

        public static List<Vector4> GetCurveKeys() {
            return CurveKeys;
        }

        public static bool HasCurveKeys() {
            return CurveKeys.Count > 0;
        }

        public static void Dispose() {
            CurveKeys.Clear();
        }
    }
}
