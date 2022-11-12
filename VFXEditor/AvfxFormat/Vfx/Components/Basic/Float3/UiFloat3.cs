using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiFloat3 : IUiBase {
        public readonly string Name;
        public readonly AVFXFloat Literal1;
        public readonly AVFXFloat Literal2;
        public readonly AVFXFloat Literal3;
        private readonly List<AVFXBase> Literals = new();

        public UiFloat3( string name, AVFXFloat literal1, AVFXFloat literal2, AVFXFloat literal3 ) {
            Name = name;
            Literals.Add( Literal1 = literal1 );
            Literals.Add( Literal2 = literal2 );
            Literals.Add( Literal3 = literal3 );
        }

        public void DrawInline( string id ) {
            // Copy/Paste
            if( CopyManager.IsCopying ) {
                CopyManager.Assigned[$"{Name}_1"] = Literal1.IsAssigned();
                CopyManager.Assigned[$"{Name}_2"] = Literal2.IsAssigned();
                CopyManager.Assigned[$"{Name}_3"] = Literal3.IsAssigned();
                CopyManager.Floats[$"{Name}_1"] = Literal1.GetValue();
                CopyManager.Floats[$"{Name}_2"] = Literal2.GetValue();
                CopyManager.Floats[$"{Name}_3"] = Literal3.GetValue();
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Assigned.TryGetValue( $"{Name}_1", out var a ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal1, a ) );
                if( CopyManager.Assigned.TryGetValue( $"{Name}_2", out var a2 ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal2, a2 ) );
                if( CopyManager.Assigned.TryGetValue( $"{Name}_3", out var a3 ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal3, a3 ) );

                Vector3 val = new( Literal1.GetValue(), Literal2.GetValue(), Literal3.GetValue() );
                if( CopyManager.Floats.TryGetValue( $"{Name}_1", out var l ) ) val.X = l;
                if( CopyManager.Floats.TryGetValue( $"{Name}_2", out var l2 ) ) val.Y = l2;
                if( CopyManager.Floats.TryGetValue( $"{Name}_3", out var l3 ) ) val.Y = l3;
                CopyManager.PasteCommand.Add( new UiFloat3Command( Literal1, Literal2, Literal3, val ) );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literals, Name, id ) ) return;

            var value = new Vector3( Literal1.GetValue(), Literal2.GetValue(), Literal3.GetValue() );
            if( ImGui.InputFloat3( Name + id, ref value ) ) {
                CommandManager.Avfx.Add( new UiFloat3Command( Literal1, Literal2, Literal3, value ) );
            }

            IUiBase.DrawRemoveContextMenu( Literals, Name, id );
        }
    }
}
