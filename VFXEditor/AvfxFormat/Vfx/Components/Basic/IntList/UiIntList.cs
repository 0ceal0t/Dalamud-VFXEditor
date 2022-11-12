using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiIntList : IUiBase {
        public readonly string Name;
        public readonly AVFXIntList Literal;

        public UiIntList( string name, AVFXIntList literal ) {
            Name = name;
            Literal = literal;
        }

        public void DrawInline( string id ) {
            // Copy/Paste
            if( CopyManager.IsCopying ) {
                CopyManager.Assigned[Name] = Literal.IsAssigned();
                CopyManager.Ints[Name] = Literal.GetValue()[0];
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Assigned.TryGetValue( Name, out var a ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal, a ) );
                if( CopyManager.Ints.TryGetValue( Name, out var l ) ) CopyManager.PasteCommand.Add( new UiIntListCommand( Literal, l ) );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            var value = Literal.GetValue()[0];
            if( ImGui.InputInt( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new UiIntListCommand( Literal, value ) );
            }

            IUiBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}
