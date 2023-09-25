using Dalamud.Interface;
using ImGuiNET;
using Lumina.Misc;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ShpkFormat.Utils {
    public class ParsedCrc : ParsedUInt {
        private bool ManualEntry = false;
        private string ManualValue = "";

        public ParsedCrc( string name ) : base( name, 4 ) { }

        public ParsedCrc( string name, uint value ) : base( name, value, 4 ) { }

        public void Draw( CommandManager manager, Dictionary<uint, string> crcMap ) {
            Copy( manager );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) );
            using var _ = ImRaii.PushId( Name );

            if( !ManualEntry ) {
                var iconSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.PencilAlt );
                var inputSize = UiUtils.GetOffsetInputSize( iconSize );

                var text = crcMap.TryGetValue( Value, out var id ) ? id : $"0x{Value:X4}";
                ImGui.SetNextItemWidth( inputSize );
                using( var combo = ImRaii.Combo( $"##{Name}", text ) ) {
                    if( combo ) {
                        var idx = 0;
                        foreach( var entry in crcMap ) {
                            using var __ = ImRaii.PushId( idx );
                            if( ImGui.Selectable( entry.Value, entry.Key == Value ) ) manager.Add( new ParsedSimpleCommand<uint>( this, entry.Key ) );
                            idx++;
                        }
                    }
                }

                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( FontAwesomeIcon.PencilAlt.ToIconString() ) ) {
                        ManualEntry = true;
                        ManualValue = "";
                    }
                }

                UiUtils.Tooltip( "Manually enter a string value to be converted to Crc32" );

                if( !Name.StartsWith( "##" ) ) {
                    ImGui.SameLine();
                    ImGui.Text( Name );
                }
            }
            else {
                var checkSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Check );
                var cancelSize = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Times );
                var inputSize = UiUtils.GetOffsetInputSize( checkSize + cancelSize );

                ImGui.SetNextItemWidth( inputSize );
                ImGui.InputTextWithHint( $"##{Name}", "String to be converted", ref ManualValue, 255 );

                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) {
                        Value = Crc32.Get( ManualValue, 0xFFFFFFFFu );
                        ManualEntry = false;
                    }

                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Times.ToIconString() ) ) ManualEntry = false;
                }

                if( !Name.StartsWith( "##" ) ) {
                    ImGui.SameLine();
                    ImGui.Text( Name );
                }
            }
        }
    }
}
