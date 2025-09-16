using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Source {
    public class KdbNodeSourceTranslate : KdbNodeSource {
        public override KdbNodeType Type => KdbNodeType.SourceTranslate;

        public readonly ParsedDouble3 Unknown1 = new( "Unknown 1" );
        public readonly ParsedDouble3 Unknown2 = new( "Unknown 2" );
        public readonly ParsedEnum<LinkType> Link = new( "Link Type" );
        public readonly ParsedFnvHash LinkHash = new( "Link" );

        public KdbNodeSourceTranslate() : base() { }

        public KdbNodeSourceTranslate( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        protected override void ReadSourceBody( BinaryReader reader ) {
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            Link.Read( reader );
            LinkHash.Read( reader );
            reader.ReadBytes( 4 ); // padding
        }

        protected override void WriteSourceBody( BinaryWriter writer ) {
            Unknown1.Write( writer );
            Unknown2.Write( writer );
            Link.Write( writer );
            LinkHash.Write( writer );
            FileUtils.Pad( writer, 4 ); // padding
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

                    Unknown1.Draw();
                    Unknown2.Draw();

                    Link.Draw();
                    using var disabled = ImRaii.Disabled( Link.Value == LinkType.None );
                    LinkHash.Draw();
                }
            }
        }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.TranslateX ),
            new( ConnectionType.TranslateY ),
            new( ConnectionType.TranslateZ ),
        ];
    }
}
