using ImGuiNET;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiFloat : IUiBase {
        public readonly string Name;
        public readonly AVFXFloat Literal;

        public UiFloat( string name, AVFXFloat literal ) {
            Name = name;
            Literal = literal;
        }

        public void DrawInline( string id ) {
            // Copy/Paste
            if( CopyManager.IsCopying ) {
                CopyManager.Assigned[Name] = Literal.IsAssigned();
                CopyManager.Floats[Name] = Literal.GetValue();
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Assigned.TryGetValue( Name, out var a ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal, a ) );
                if( CopyManager.Floats.TryGetValue( Name, out var l ) ) CopyManager.PasteCommand.Add( new UiFloatCommand( Literal, l ) );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            var value = Literal.GetValue();
            if( ImGui.InputFloat( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new UiFloatCommand( Literal, value ) );
            }

            IUiBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}
