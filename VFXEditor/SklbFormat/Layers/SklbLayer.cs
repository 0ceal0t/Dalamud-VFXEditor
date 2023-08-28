using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Parsing;
using VfxEditor.SklbFormat.Bones;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.SklbFormat.Layers {
    public class SklbLayer : IUiItem {
        public readonly SklbFile File;

        public readonly ParsedInt Id = new( "Id" );
        public readonly List<SklbLayerBone> Bones = new();

        public int Size => 4 + 2 + 2 * Bones.Count; // Id + numBones + boneIndices

        public SklbLayer( SklbFile file ) {
            File = file;
        }

        public SklbLayer( SklbFile file, BinaryReader reader ) : this( file ) {
            Id.Read( reader );
            var numBones = reader.ReadInt16();
            for( var i = 0; i < numBones; i++ ) {
                Bones.Add( new( file, reader ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            writer.Write( ( short )Bones.Count );
            Bones.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            Id.Draw( CommandManager.Sklb );

            for( var idx = 0; idx < Bones.Count; idx++ ) {
                var bone = Bones[idx];
                using var _ = ImRaii.PushId( idx );

                bone.Draw();

                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.SameLine();
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                    CommandManager.Sklb.Add( new GenericRemoveCommand<SklbLayerBone>( Bones, bone ) );
                    break;
                }
            }

            if( ImGui.Button( "+ New" ) ) { // NEW
                CommandManager.Sklb.Add( new GenericAddCommand<SklbLayerBone>( Bones, new( File ) ) );
            }
        }
    }

    public unsafe class SklbLayerBone : IUiItem {
        public readonly SklbFile File;
        public ParsedBoneIndex Bone = new( "##Bone", -1 ); // Don't want the label to actually appear

        public SklbLayerBone( SklbFile file ) {
            File = file;
        }

        public SklbLayerBone( SklbFile file, BinaryReader reader ) {
            File = file;
            Bone.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Bone.Write( writer );
        }

        public void Draw() {
            Bone.Draw( File.Bones.Bones );
        }
    }
}
