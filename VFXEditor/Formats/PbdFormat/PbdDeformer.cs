using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components.SplitViews;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdDeformer : IUiItem {
        public readonly ParsedShort SkeletonId = new( "Skeleton Id" );
        public readonly ParsedFloat UnknownScale = new( "Unknown Scale" );

        public readonly ParsedBool Enabled = new( "Enabled", true );
        public readonly List<PbdBone> Bones = [];
        private readonly CommandSplitView<PbdBone> BoneView;

        public PbdDeformer() {
            BoneView = new( "Bone", Bones, false, ( PbdBone bone, int idx ) => bone.Name.Value, () => new() );
        }

        public PbdDeformer( BinaryReader reader ) : this() {
            SkeletonId.Read( reader );
            reader.ReadUInt16(); // connection index
            var offset = reader.ReadInt32();
            UnknownScale.Read( reader );

            if( offset == 0 ) {
                Enabled.Value = false;
                return;
            }

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;

            var boneCount = reader.ReadInt32();

            var stringOffsets = new List<short>();
            for( var i = 0; i < boneCount; i++ ) stringOffsets.Add( reader.ReadInt16() );
            FileUtils.PadTo( reader, 4 );

            for( var i = 0; i < boneCount; i++ ) Bones.Add( new( reader ) );
            for( var i = 0; i < boneCount; i++ ) {
                reader.BaseStream.Position = offset + stringOffsets[i];
                Bones[i].Name.Read( reader );
            }

            reader.BaseStream.Position = savePos;
        }

        public void Draw() {
            SkeletonId.Draw();
            UnknownScale.Draw();

            Enabled.Draw();
            if( Enabled.Value ) {
                ImGui.Separator();
                BoneView.Draw();
            }
        }
    }
}
