using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.SklbFormat.Layers {
    public class SklbLayer : IUiItem {
        public readonly SklbFile File;

        public readonly ParsedInt Id = new( "Id" );
        public readonly List<SklbLayerBone> Bones = new();
        private readonly CollapsingHeaders<SklbLayerBone> BonesView;

        public int Size => 4 + 2 + 2 * Bones.Count; // Id + numBones + boneIndices

        public SklbLayer( SklbFile file ) {
            File = file;
            BonesView = new( "Bone", Bones, null, () => new( file ), () => CommandManager.Sklb );
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
            BonesView.Draw();
        }
    }

    public class SklbLayerBone : IUiItem {
        public readonly SklbFile File;
        public ParsedShort BoneIndex = new( "Bone Index" );

        public SklbLayerBone( SklbFile file ) {
            File = file;
        }

        public SklbLayerBone( SklbFile file, BinaryReader reader ) {
            File = file;
            BoneIndex.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            BoneIndex.Write( writer );
        }

        public void Draw() {
            var boneIdx = BoneIndex.Value;
            if( boneIdx >= File.Bones.Bones.Count ) {
                BoneIndex.Draw( CommandManager.Sklb );
                return;
            }

            var bone = boneIdx == -1 ? null : File.Bones.Bones[boneIdx];
            var text = bone == null ? "[NONE]" : bone.Name.Value;

            using var combo = ImRaii.Combo( "Bone", text );
            if( !combo ) return;

            if( ImGui.Selectable( "[NONE]", bone == null ) ) {
                CommandManager.Sklb.Add( new ParsedSimpleCommand<int>( BoneIndex, -1 ) );
            }

            var idx = 0;

            foreach( var item in File.Bones.Bones ) {
                using var _ = ImRaii.PushId( idx );
                var selected = bone == item;

                if( ImGui.Selectable( item.Name.Value, selected ) ) {
                    CommandManager.Sklb.Add( new ParsedSimpleCommand<int>( BoneIndex, idx ) );
                }

                if( selected ) ImGui.SetItemDefaultFocus();
                idx++;
            }
        }
    }
}
