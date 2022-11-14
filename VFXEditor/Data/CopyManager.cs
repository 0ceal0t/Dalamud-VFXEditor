using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Data {
    public class CopyManager {
        public static bool IsCopying { get; private set; }
        public static bool IsPasting { get; private set; }

        public static readonly List<Vector4> CurveKeys = new();

        public static readonly Dictionary<string, bool> Assigned = new();
        public static readonly Dictionary<string, bool> Bools = new();
        public static readonly Dictionary<string, int> Ints = new();
        public static readonly Dictionary<string, float> Floats = new();
        public static readonly Dictionary<string, string> Strings = new();
        public static readonly Dictionary<string, string> Enums = new();

        public static CompoundCommand PasteCommand { get; private set; } = new(false, true);

        public static void Reset() {
            IsCopying = false;
            IsPasting = false;
        }

        public static void Copy() {
            Clear();
            IsCopying = true;
        }

        public static void Paste() {
            IsPasting = true;
        }

        public static void FinalizePaste() {
            if( !IsPasting ) return;
            Clear();
            CommandManager.Avfx?.Add( PasteCommand );
            PasteCommand = new( false, true );
        }

        public static void ClearCurveKeys() {
            CurveKeys.Clear();
        }

        public static void AddCurveKey( float time, float x, float y, float z ) {
            CurveKeys.Add( new Vector4( time, x, y, z ) );
        }

        public static bool HasCurveKeys() => CurveKeys.Count > 0;

        public static void Dispose() {
            PasteCommand.Clear();
            CurveKeys.Clear();
            Clear();
        }

        private static void Clear() {
            Assigned.Clear();
            Bools.Clear();
            Ints.Clear();
            Floats.Clear();
            Strings.Clear();
            Enums.Clear();
        }
    }
}
