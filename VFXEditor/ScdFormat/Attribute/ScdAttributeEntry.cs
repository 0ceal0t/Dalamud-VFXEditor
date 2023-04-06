using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.ScdFormat
{
    public class ScdAttributeEntry : ScdEntry, ISimpleUiBase {
        public readonly ParsedByte Version = new( "Version" );
        public readonly ParsedReserve Reserved = new( 1 );
        public readonly ParsedShort AttributeId = new( "Attribute Id" );
        public readonly ParsedShort SearchAttributeId = new( "Search Attribute Id" );
        public readonly ParsedByte ConditionFirst = new( "First Condition" );
        public readonly ParsedByte ArgumentCount = new( "Argument Count" );
        public readonly ParsedInt SoundLabelLow = new( "Sound Label Low" );
        public readonly ParsedInt SoundLabelHigh = new( "Sound Label High" );

        public readonly AttributeResultCommand ResultFirst = new();
        public readonly AttributeExtendData Extend1 = new();
        public readonly AttributeExtendData Extend2 = new();
        public readonly AttributeExtendData Extend3 = new();
        public readonly AttributeExtendData Extend4 = new();

        private readonly List<ParsedBase> Parsed;

        public ScdAttributeEntry() {
            Parsed = new() {
                Version,
                Reserved,
                AttributeId,
                SearchAttributeId,
                ConditionFirst,
                ArgumentCount,
                SoundLabelLow,
                SoundLabelHigh,
            };
        }

        public override void Read( BinaryReader reader ) {
            Parsed.ForEach( x => x.Read( reader ) );

            ResultFirst.Read( reader );
            Extend1.Read( reader );
            Extend2.Read( reader );
            Extend3.Read( reader );
            Extend4.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Parsed.ForEach( x => x.Write( writer ) );

            ResultFirst.Write( writer );
            Extend1.Write( writer );
            Extend2.Write( writer );
            Extend3.Write( writer );
            Extend4.Write( writer );
        }

        public void Draw( string id ) {
            Parsed.ForEach( x => x.Draw( id, CommandManager.Scd ) );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( ImGui.BeginTabBar( $"{id}/Tabs" ) ) {
                if( ImGui.BeginTabItem( $"Result{id}" ) ) {
                    ResultFirst.Draw( $"{id}/Result" );
                    ImGui.EndTabItem();
                }

                if( ImGui.BeginTabItem( $"Extend 1{id}" ) ) {
                    Extend1.Draw( $"{id}/Extend1" );
                    ImGui.EndTabItem();
                }

                if( ImGui.BeginTabItem( $"Extend 2{id}" ) ) {
                    Extend2.Draw( $"{id}/Extend2" );
                    ImGui.EndTabItem();
                }

                if( ImGui.BeginTabItem( $"Extend 3{id}" ) ) {
                    Extend3.Draw( $"{id}/Extend3" );
                    ImGui.EndTabItem();
                }

                if( ImGui.BeginTabItem( $"Extend 4{id}" ) ) {
                    Extend4.Draw( $"{id}/Extend4" );
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
    }
}
