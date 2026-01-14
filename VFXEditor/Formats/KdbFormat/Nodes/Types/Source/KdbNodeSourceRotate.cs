using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Source {
    public class KdbNodeSourceRotate : KdbNodeSource {
        public override KdbNodeType Type => KdbNodeType.SourceRotate;

        public readonly ParsedDouble4 SourceQuat = new( "Source" );
        public readonly ParsedDouble4 TargetQuat = new( "Target" );
        public readonly ParsedDouble3 Aim = new( "Aim Vector" );
        public readonly ParsedDouble3 Up = new( "Up Vector" );

        public readonly ParsedDouble4 Unknown3 = new( "Unknown 3" );
        public readonly ParsedEnum<LinkType> Link = new( "Link Type" );
        public readonly ParsedFnvHash LinkHash = new( "Link" );
        public readonly ParsedBool Unknown4 = new( "Unknown 4", size: 2 );
        public readonly ParsedBool Unknown5 = new( "Unknown 5", size: 2 );

        public KdbNodeSourceRotate() : base() { }

        public KdbNodeSourceRotate( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        protected override void ReadSourceBody( BinaryReader reader ) {
            SourceQuat.Read( reader );
            TargetQuat.Read( reader );
            Aim.Read( reader );
            Up.Read( reader );

            Unknown3.Read( reader );
            Link.Read( reader );
            LinkHash.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
        }

        protected override void WriteSourceBody( BinaryWriter writer ) {
            SourceQuat.Write( writer );
            TargetQuat.Write( writer );
            Aim.Write( writer );
            Up.Write( writer );

            Unknown3.Write( writer );
            Link.Write( writer );
            LinkHash.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
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

                    Unknown3.Draw();
                    Link.Draw();
                    using( var disabled = ImRaii.Disabled( Link.Value == LinkType.None ) ) {
                        LinkHash.Draw();
                    }
                    Unknown4.Draw();
                    Unknown5.Draw();
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
