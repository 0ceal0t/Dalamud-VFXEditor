using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Source {
    public enum LinkType {
        None = 0,
        Bone = 2,
    }

    public class KdbNodeSourceRotate : KdbNodeSource {
        public override KdbNodeType Type => KdbNodeType.SourceRotate;

        public readonly ParsedQuat SourceQuat = new( "Source", size: 8 );
        public readonly ParsedQuat TargetQuat = new( "Target", size: 8 );
        public readonly ParsedDouble3 Aim = new( "Aim Vector" );
        public readonly ParsedDouble3 Up = new( "Up Vector" );

        public readonly ParsedDouble4 Unknown1 = new( "Unknown 1" );
        public readonly ParsedEnum<LinkType> Link = new( "Link Type" );
        public readonly ParsedFnvHash LinkHash = new( "Link" );
        public readonly ParsedBool Unknown2 = new( "Unknown 2", size: 2 );
        public readonly ParsedBool Unknown3 = new( "Unknown 3", size: 2 );

        public KdbNodeSourceRotate() : base() { }

        public KdbNodeSourceRotate( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            var boneCount = reader.ReadUInt32();
            var bonePosition = reader.BaseStream.Position + reader.ReadUInt32();
            _ = reader.ReadUInt32(); // weight count, same as bone count
            var weightPosition = reader.BaseStream.Position + reader.ReadUInt32();

            SourceQuat.Read( reader );
            TargetQuat.Read( reader );
            Aim.Read( reader );
            Up.Read( reader );

            Unknown1.Read( reader );
            Link.Read( reader );
            LinkHash.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );

            for( var i = 0; i < boneCount; i++ ) Bones.Add( new() );
            reader.BaseStream.Position = bonePosition;
            foreach( var bone in Bones ) bone.Name.Read( reader );
            reader.BaseStream.Position = weightPosition;
            foreach( var bone in Bones ) bone.Weight.Read( reader );
        }

        public override void UpdateBones( List<string> boneList ) {
            base.UpdateBones( boneList );
            LinkHash.Guess( boneList );
        }

        protected override void DrawBody( List<string> bones ) {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Bones" ) ) {
                if( tab ) { BoneTable.Draw(); }
            }

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) {
                    using var _ = ImRaii.PushId( "Parameters" );
                    using var child = ImRaii.Child( "Child" );

                    SourceQuat.Draw();
                    TargetQuat.Draw();
                    Aim.Draw();
                    Up.Draw();

                    Unknown1.Draw();
                    Link.Draw();
                    using( var disabled = ImRaii.Disabled( Link.Value == LinkType.None ) ) {
                        LinkHash.Draw();
                    }
                    Unknown2.Draw();
                    Unknown3.Draw();
                }
            }
        }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.RotateAngle ),
            new( ConnectionType.BendingAngle ),
            new( ConnectionType.RollBend ),
            new( ConnectionType.Roll ),
            new( ConnectionType.BendS ),
            new( ConnectionType.BendT ),
            new( ConnectionType.Expmap ),
            new( ConnectionType.ExpmapX ),
            new( ConnectionType.ExpmapY ),
            new( ConnectionType.ExpmapZ ),
            new( ConnectionType.RotateQuat ),
            new( ConnectionType.BendingQuat ),
        ];
    }
}
