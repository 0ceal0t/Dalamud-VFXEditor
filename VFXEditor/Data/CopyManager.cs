using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Data {
    public class CopyManager {
        public static CopyManager Avfx => Plugin.AvfxManager?.GetCopyManager();

        public bool IsCopying { get; private set; }
        public bool IsPasting { get; private set; }

        public readonly Dictionary<string, bool> Assigned = new();
        public readonly Dictionary<string, bool> Bools = new();
        public readonly Dictionary<string, int> Ints = new();
        public readonly Dictionary<string, float> Floats = new();
        public readonly Dictionary<string, Vector2> Vector2s = new();
        public readonly Dictionary<string, Vector3> Vector3s = new();
        public readonly Dictionary<string, Vector4> Vector4s = new();
        public readonly Dictionary<string, string> Strings = new();
        public readonly Dictionary<string, string> Enums = new();
        public readonly List<Vector4> CurveKeys = new();
        public CompoundCommand PasteCommand { get; private set; } = new( false, true );

        public CopyManager() { }

        public void Reset() {
            IsCopying = false;
            IsPasting = false;
        }

        public void Copy() {
            Clear();
            IsCopying = true;
        }

        public void Paste() {
            IsPasting = true;
        }

        public void FinalizePaste( CommandManager manager ) {
            if( manager == null ) return;
            if( !IsPasting ) return;
            Clear();
            manager.Add( PasteCommand ); // execute
            PasteCommand = new( false, true ); // reset
        }

        public void ClearCurveKeys() => CurveKeys.Clear();

        public void AddCurveKey( float time, float x, float y, float z ) => CurveKeys.Add( new Vector4( time, x, y, z ) );

        public bool HasCurveKeys() => CurveKeys.Count > 0;

        private void Clear() {
            Assigned.Clear();
            Bools.Clear();
            Ints.Clear();
            Floats.Clear();
            Vector2s.Clear();
            Vector3s.Clear();
            Vector4s.Clear();
            Strings.Clear();
            Enums.Clear();
        }

        public void Dispose() {
            PasteCommand.Clear();
            CurveKeys.Clear();
            Clear();
        }

        public void Draw() {
            if( ImGui.MenuItem( "Copy" ) ) Copy();
            if( ImGui.MenuItem( "Paste" ) ) Paste();
        }

        //==================

        public static void FinalizeAll() => Plugin.Managers.ForEach( x => x?.GetCopyManager()?.FinalizePaste( x?.GetCommandManager() ) );

        public static void ResetAll() => Plugin.Managers.ForEach( x => x?.GetCopyManager()?.Reset() );

        public static void DisposeAll() => Plugin.Managers.ForEach( x => x?.GetCopyManager()?.Dispose() );
    }
}
