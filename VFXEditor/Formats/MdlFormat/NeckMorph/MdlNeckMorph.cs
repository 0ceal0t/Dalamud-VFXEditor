using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.NeckMorph {
    // Present on face models since 7.1
    // https://github.com/TexTools/xivModdingFramework/blob/master/xivModdingFramework/Models/DataContainers/NeckMorphEntry.cs
    public class MdlNeckMorph : IUiItem {
        private readonly MdlFile File;

        public readonly ParsedFloat3 PositionAdjust = new( "Position Adjust" );
        public readonly ParsedUInt Unknown = new( "Unknown" );
        public readonly ParsedFloat3 NormalAdjust = new( "Normal Adjust" );

        // Indices into bone table 0
        public readonly ParsedByte[] Bones = [
            new( "Bone 1" ),
            new( "Bone 2" ),
            new( "Bone 3" ),
            new( "Bone 4" ),
        ];

        public MdlNeckMorph( MdlFile file ) {
            File = file;
        }

        public MdlNeckMorph( MdlFile file, BinaryReader reader ) : this( file ) {
            PositionAdjust.Read( reader );
            Unknown.Read( reader );
            NormalAdjust.Read( reader );
            foreach( var bone in Bones ) bone.Read( reader );
        }

        public void Draw() {
            PositionAdjust.Draw();
            Unknown.Draw();
            NormalAdjust.Draw();

            ImGui.TextDisabled( "Bone indices refer to bone table 0. Slots 3 and 4 use 0 as \"none\"" );
            for( var i = 0; i < Bones.Length; i++ ) {
                Bones[i].Draw();
                ImGui.SameLine();
                ImGui.TextDisabled( GetBoneName( i ) );
            }
        }

        private string GetBoneName( int slot ) {
            var index = Bones[slot].Value;
            if( slot >= 2 && index == 0 ) return "[NONE]";

            var tables = File.BoneTables.Tables;
            if( tables.Count == 0 || index < 0 || index >= tables[0].Bones.Count ) return "[INVALID]";
            return tables[0].Bones[index].Value;
        }

        public void Write( BinaryWriter writer ) {
            PositionAdjust.Write( writer );
            Unknown.Write( writer );
            NormalAdjust.Write( writer );
            foreach( var bone in Bones ) bone.Write( writer );
        }
    }
}