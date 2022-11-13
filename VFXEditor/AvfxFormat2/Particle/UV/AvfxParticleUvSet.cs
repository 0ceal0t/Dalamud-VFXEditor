using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleUvSet : AvfxSelectableItem {
        public readonly AvfxEnum<TextureCalculateUV> CalculateUVType = new( "Calculate UV", "CUvT" );
        public readonly AvfxCurve2Axis Scale = new( "Scale", "Scl" );
        public readonly AvfxCurve2Axis Scroll = new( "Scroll", "Scr" );
        public readonly AvfxCurve Rot = new( "Rotation", "Rot" );
        public readonly AvfxCurve RotRandom = new( "Rotation Random", "RotR" );

        private readonly List<AvfxBase> Children;
        private readonly List<AvfxItem> Curves;
        private readonly List<IUiBase> Parameters;

        public AvfxParticleUvSet() : base( "UvSt" ) {
            Children = new() {
                CalculateUVType,
                Scale,
                Scroll,
                Rot,
                RotRandom
            };

            Parameters = new() {
                CalculateUVType
            };

            Curves = new() {
                Scale,
                Scroll,
                Rot,
                RotRandom
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );

        public override void Draw( string parentId ) {
            var id = parentId + "/UV";
            IUiBase.DrawList( Parameters, id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            IUiItem.DrawListTabs( Curves, parentId );
        }

        public override string GetDefaultText() => $"UV {GetIdx()}";
    }
}
