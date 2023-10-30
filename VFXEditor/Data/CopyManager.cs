using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AvfxFormat;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.Data {
    public class CopyManager {
        public static CopyManager Avfx => Plugin.AvfxManager?.GetCopyManager();

        public bool IsCopying { get; private set; }
        public bool IsPasting { get; private set; }
        public CompoundCommand PasteCommand { get; private set; } = new();

        public readonly Dictionary<(Type, string), bool> AvfxAssigned = new();
        public readonly Dictionary<(Type, string), object> Data = new();
        public readonly List<(KeyType, Vector4)> CurveKeys = new();

        public CopyManager() { }

        public bool HasCurveKeys() => CurveKeys.Count > 0;

        public void AddCurveKey( AvfxCurveKey key ) => CurveKeys.Add( key.CopyPasteData );

        public void ClearCurveKeys() => CurveKeys.Clear();

        public void SetValue<R>( object item, string name, R value ) {
            Data[(item.GetType(), name)] = value;
        }

        public bool GetValue<R>( object item, string name, out R value ) {
            value = default;
            if( !Data.TryGetValue( (item.GetType(), name), out var val ) ) return false;
            if( val is R v ) {
                value = v;
                return true;
            }
            return false;
        }

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
            manager.Add( PasteCommand );
            PasteCommand = new();
        }

        private void Clear() {
            AvfxAssigned.Clear();
            Data.Clear();
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

        public static void DrawDisabled() {
            ImGui.MenuItem( "Copy" );
            ImGui.MenuItem( "Paste" );
        }

        //==================

        public static void FinalizeAll() => Plugin.Managers.ForEach( x => x?.GetCopyManager()?.FinalizePaste( x?.GetCurrentCommandManager() ) );

        public static void ResetAll() => Plugin.Managers.ForEach( x => x?.GetCopyManager()?.Reset() );

        public static void DisposeAll() => Plugin.Managers.ForEach( x => x?.GetCopyManager()?.Dispose() );
    }
}
