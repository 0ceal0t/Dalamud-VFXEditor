using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.TargetConstraint {
    public class KdbNodeTargetOrientationConstraint : KdbNodeTargetConstraint {
        public override KdbNodeType Type => KdbNodeType.TargetOrientationConstraint;

        public readonly ParsedDouble3 Unknown1 = new( "Unknown 1" );

        public KdbNodeTargetOrientationConstraint() : base() { }

        public KdbNodeTargetOrientationConstraint( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        protected override void ReadTargetConstraintBody( BinaryReader reader ) {
            Unknown1.Read( reader );
        }

        protected override void WriteTargetConstraintBody( BinaryWriter writer ) {
            Unknown1.Write( writer );
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

                    Bone.Draw();
                    Unknown1.Draw();
                }
            }
        }
    }
}
