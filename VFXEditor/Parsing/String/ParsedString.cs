using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedStringIcon {
        public Func<FontAwesomeIcon> Icon;
        public bool Remove;
        public Action<string> Action;
    }

    public class ParsedString : ParsedSimpleBase<string> {
        public readonly List<ParsedStringIcon> Icons;
        public bool HasIcons => Icons?.Count > 0;
        private readonly bool ForceLowerCase;

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private string StateBeforeEdit = "";

        public ParsedString( string name, string value ) : base( name, value ) { }

        public ParsedString( string name, List<ParsedStringIcon> icons = null, bool forceLower = false ) : base( name ) {
            Value = "";
            Icons = icons ?? [];
            ForceLowerCase = forceLower;
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Value = FileUtils.ReadString( reader );
        }

        public override void Write( BinaryWriter writer ) => FileUtils.WriteString( writer, Value, writeNull: true );

        public override void Draw() => Draw( 255, Name, 0, ImGuiInputTextFlags.None );

        public void Draw( uint maxSize, string label, float offset, ImGuiInputTextFlags flags ) {
            DrawInput( maxSize, label, offset, flags );
            DrawIcons();
        }

        protected override void DrawBody() { }

        public void DrawInput( uint maxSize, string label, float offset, ImGuiInputTextFlags flags ) {
            CopyPaste();
            using var _ = ImRaii.PushId( Name );

            if( HasIcons || offset > 0 ) {
                var iconsSize = HasIcons ? Icons.Select( x => UiUtils.GetPaddedIconSize( x.Icon.Invoke() ) ).Sum() : 0;
                var inputSize = UiUtils.GetOffsetInputSize( offset + iconsSize );
                ImGui.SetNextItemWidth( inputSize );
            }

            var prevValue = Value;
            if( ImGui.InputText( HasIcons ? $"##{Name}" : label, ref Value, maxSize, flags ) ) {
                if( !Editing ) {
                    Editing = true;
                    StateBeforeEdit = prevValue;
                }
                LastEditTime = DateTime.Now;
            }
            else if( Editing && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                Editing = false;
                Update( StateBeforeEdit, ForceLowerCase ? Value.ToLower() : Value );
            }
        }

        public void DrawIcons() {
            if( !HasIcons ) return;
            using var _ = ImRaii.PushId( Name );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) );
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                foreach( var icon in Icons ) {
                    ImGui.SameLine();
                    if( icon.Remove ? UiUtils.RemoveButton( icon.Icon.Invoke().ToIconString() ) : ImGui.Button( icon.Icon.Invoke().ToIconString() ) ) icon.Action.Invoke( Value );
                }
            }

            ImGui.SameLine();
            ImGui.Text( Name );
        }
    }
}
