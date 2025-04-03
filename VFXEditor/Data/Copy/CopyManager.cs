using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.AvfxFormat;

namespace VfxEditor.Data.Copy {
    public class CopyManager {
        public static CopyManager? Current => Stack.Count == 0 ? null : Stack.Peek();
        private static readonly Stack<CopyManager> Stack = new();

        public static bool IsCopying => Current != null && Current.Copying;

        public static bool IsPasting => Current != null && Current.Pasting;

        public static void Push( CopyManager current ) {
            Stack.Push( current );
            if( current != null ) {
                current.Copying = false;
                current.Pasting = false;
            }
        }

        public static void Pop() => Stack.Pop();

        public static void Draw() => Current?.DrawInternal();

        public static void Copy() => Current?.CopyInternal();

        public static void Paste() => Current?.PasteInternal();

        public static void TrySetValue<R>( object item, string name, R value ) {
            if( !IsCopying ) return;
            Current.SetValue( item, name, value );
        }

        public static bool TryGetValue<R>( object item, string name, out R value ) {
            value = default;
            if( !IsPasting ) return false;
            return Current.GetValue( item, name, out value );
        }

        public static void TrySetAssigned<R>( R item, string name ) where R : AvfxBase {
            if( !IsCopying ) return;
            Current.AvfxAssigned[(item.GetType(), name)] = item.IsAssigned();
        }

        public static bool TryGetAssigned<R>( R item, string name, out bool value ) {
            value = false;
            if( !IsPasting ) return false;
            if( !Current.AvfxAssigned.TryGetValue( (item.GetType(), name), out value ) ) return false;
            return true;
        }

        // ==================

        protected bool Copying = false;
        protected bool Pasting = false;

        protected readonly Dictionary<(Type, string), bool> AvfxAssigned = [];
        protected readonly Dictionary<(Type, string), object> Data = [];

        public CopyManager() { }

        protected void SetValue<R>( object item, string name, R value ) {
            Data[(item.GetType(), name)] = value;
        }

        protected bool GetValue<R>( object item, string name, out R value ) {
            value = default;
            if( !Data.TryGetValue( (item.GetType(), name), out var val ) ) return false;
            if( val is R v ) {
                value = v;
                return true;
            }
            return false;
        }

        protected void DrawInternal() {
            if( ImGui.MenuItem( "Copy" ) ) CopyInternal();
            if( ImGui.MenuItem( "Paste" ) ) PasteInternal();
        }

        private void CopyInternal() {
            Copying = true;
            ClearData();
        }

        private void ClearData() {
            AvfxAssigned.Clear();
            Data.Clear();
        }

        private void PasteInternal() { Pasting = true; }
    }
}
