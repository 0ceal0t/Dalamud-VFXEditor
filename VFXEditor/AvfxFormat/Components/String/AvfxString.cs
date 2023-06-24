using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using System.Numerics;
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
            InputString = ( Value ?? "" ).Trim( '\0' );
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            var bytes = Encoding.ASCII.GetBytes( Value );
            writer.Write( bytes );
            if( FixedSize != -1 ) WritePad( writer, FixedSize - bytes.Length );
        }

        public override void Draw() => Draw( 0 );

        public void Draw( float preOffset ) {
            // Unassigned
            AssignedCopyPaste( this, Name );
            if( DrawAddButton( this, Name ) ) return;

            // Copy/Paste
            var manager = CopyManager.Avfx;
            if( manager.IsCopying ) manager.Strings[Name] = Value;
            if( manager.IsPasting && manager.Strings.TryGetValue( Name, out var val ) ) manager.PasteCommand.Add( new AvfxStringCommand( this, val, IsAssigned() ) );

            var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Check );
            var removeSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Trash );

            using var _ = ImRaii.PushId( Name );

            // Input
            var inputSize = UiUtils.GetOffsetInputSize( preOffset + checkSize + ( ShowRemoveButton ? removeSize : 0 ) );
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( "##Input", ref InputString, 256 );

            DrawRemoveContextMenu( this, Name );

            var imguiStyle = ImGui.GetStyle();

            // Check - update value
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) {
                    var newValue = InputString.Trim().Trim( '\0' ) + '\u0000';
                    CommandManager.Avfx.Add( new AvfxStringCommand( this, newValue, ShowRemoveButton && newValue.Trim( '\0' ).Length == 0 ) );
                }
            }

            UiUtils.Tooltip( "Update field value" );

            // Remove - unassign
            if( ShowRemoveButton ) {
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) CommandManager.Avfx.Add( new AvfxStringCommand( this, "", true ) );
                }

                UiUtils.Tooltip( "Unassign field" );
            }

            ImGui.SameLine();
            ImGui.Text( Name );
        }
    }
}
