using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.Parsing {
    public class ParsedStringIcon {
        public Func<FontAwesomeIcon> Icon;
        public bool Remove;
        public Action<string> Action;
    }

    public class ParsedString : ParsedSimpleBase<string, string> {
        public readonly List<ParsedStringIcon> Icons = new();
        public bool HasIcons => Icons?.Count > 0;

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private string StateBeforeEdit = "";

        public ParsedString( string name, string value ) : this( name ) {
            Value = value;
        }

        public ParsedString( string name, List<ParsedStringIcon> icons ) : this( name ) {
            Icons = icons;
        }

        public ParsedString( string name ) : base( name ) {
            Value = "";
        }

        public override void Read( BinaryReader reader ) => Read( reader, 0 );

        public override void Read( BinaryReader reader, int size ) {
            Value = FileUtils.ReadString( reader );
        }

        public override void Write( BinaryWriter writer ) => FileUtils.WriteString( writer, Value, writeNull: true );

        public override void Draw( CommandManager manager ) => Draw( manager, 255, Name, 0, ImGuiInputTextFlags.None );

        public void Draw( CommandManager manager, uint maxSize, string label, float offset, ImGuiInputTextFlags flags ) {
            DrawInput( manager, maxSize, label, offset, flags );
            DrawIcons();
        }

        public void DrawInput( CommandManager manager, uint maxSize, string label, float offset, ImGuiInputTextFlags flags ) {
            Copy( manager );
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
                manager.Add( new ParsedSimpleCommand<string>( this, StateBeforeEdit, Value ) );
            }
        }

        public void DrawIcons() {
            if( !HasIcons ) return;
            using var _ = ImRaii.PushId( Name );

            var imguiStyle = ImGui.GetStyle();
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                foreach( var icon in Icons ) {
                    ImGui.SameLine();
                    if( icon.Remove ? UiUtils.RemoveButton( icon.Icon.Invoke().ToIconString() ) : ImGui.Button( icon.Icon.Invoke().ToIconString() ) ) icon.Action.Invoke( Value );
                }
            }

            ImGui.SameLine();
            ImGui.Text( Name );
        }

        protected override Dictionary<string, string> GetCopyMap( CopyManager manager ) => manager.Strings;

        protected override string ToCopy() => Value;

        protected override string FromCopy( string val ) => val;
    }
}
