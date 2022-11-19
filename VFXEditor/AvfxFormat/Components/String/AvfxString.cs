using Dalamud.Interface;
using ImGuiNET;
using System;
using System.IO;
using System.Text;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxString : AvfxDrawable {
        public readonly string Name;
        private readonly int FixedSize;
        private readonly bool ShowRemoveButton;
        private string Value = "";
        private string InputString = "";

        public AvfxString( string name, string avfxName, bool showRemoveButton = false, int fixedSize = -1 ) : base( avfxName ) {
            Name = name;
            FixedSize = fixedSize;
            ShowRemoveButton = showRemoveButton;
        }

        public string GetValue() => Value;

        public void SetValue( string value ) {
            SetAssigned( true );
            Value = value;
            InputString = Value.Trim( '\0' );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Value = Encoding.ASCII.GetString( reader.ReadBytes( size ) );
            InputString = (Value ?? "").Trim( '\0' );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            var bytes = Encoding.ASCII.GetBytes( Value );
            writer.Write( bytes );
            if( FixedSize != -1 ) WritePad( writer, FixedSize - bytes.Length );
        }

        public override void Draw( string id ) {
            // Unassigned
            AssignedCopyPaste( this, Name );
            if( DrawAddButton( this, Name, id ) ) return;

            // Copy/Paste
            var manager = CopyManager.Avfx;
            if( manager.IsCopying ) manager.Strings[Name] = Value;
            if( manager.IsPasting && manager.Strings.TryGetValue( Name, out var val ) ) manager.PasteCommand.Add( new AvfxStringCommand( this, val, IsAssigned() ) );

            var style = ImGui.GetStyle();
            ImGui.PushFont( UiBuilder.IconFont );
            var checkSize = ImGui.CalcTextSize( $"{( char )FontAwesomeIcon.Check}" ).X + style.FramePadding.X * 2 + 2;
            var removeSize = ImGui.CalcTextSize( $"{( char )FontAwesomeIcon.Trash}" ).X + style.FramePadding.X * 2 + 2;
            ImGui.PopFont();

            // Input
            var inputSize = ImGui.GetContentRegionAvail().X * 0.65f - checkSize - ( ShowRemoveButton ? removeSize : 0 );
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( $"{id}-MainInput", ref InputString, 256 );

            DrawRemoveContextMenu( this, Name, id );

            ImGui.PushFont( UiBuilder.IconFont );

            // Check - update value
            ImGui.SameLine( inputSize + 2 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Check}" + id ) ) {
                var newValue = InputString.Trim().Trim( '\0' ) + '\u0000';
                CommandManager.Avfx.Add( new AvfxStringCommand( this, newValue, ShowRemoveButton && newValue.Trim( '\0' ).Length == 0 ) );
            }
            // Remove - unassign
            if( ShowRemoveButton ) {
                ImGui.SameLine( inputSize + checkSize + 2 );
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + id ) ) CommandManager.Avfx.Add( new AvfxStringCommand( this, "", true ) );
            }

            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.Text( Name );
        }
    }
}
