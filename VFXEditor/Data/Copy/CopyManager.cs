using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.Data.Copy {
    public class CopyManager {
        public static CopyManager Current => Stack.Count == 0 ? null : Stack.Peek();
        private static readonly Stack<CopyManager> Stack = new();

        public static void Push( CopyManager current ) => Stack.Push( current );

        public static void Pop() => Stack.Pop();

        public static void Draw() => Current.DrawInternal();

        // ==================

        public bool IsCopying { get; private set; }
        public bool IsPasting { get; private set; }
        public CompoundCommand PasteCommand { get; private set; } = new();

        public readonly Dictionary<(Type, string), bool> AvfxAssigned = new();
        public readonly Dictionary<(Type, string), object> Data = new();
        public readonly List<(KeyType, Vector4)> CurveKeys = new();

        public CopyManager() {

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

        protected void DrawInternal() {
            if( ImGui.MenuItem( "Copy" ) ) Copy();
            if( ImGui.MenuItem( "Paste" ) ) Paste();
        }
    }
}
