using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleUvSet : AvfxSelectableItem {
        public readonly AvfxEnum<TextureCalculateUV> CalculateUVType = new( "Calculate UV", "CUvT" );
        public readonly AvfxCurve2Axis Scale = new( "Scale", "Scl" );
        public readonly AvfxCurve2Axis Scroll = new( "Scroll", "Scr" );
        public readonly AvfxCurve Rot = new( "Rotation", "Rot" );
        public readonly AvfxCurve RotRandom = new( "Rotation Random", "RotR" );

        private readonly List<AvfxBase> Parsed;
        private readonly List<AvfxItem> Curves;
        private readonly List<IUiBase> Display;

        public AvfxParticleUvSet() : base( "UvSt" ) {
            Parsed = new() {
                CalculateUVType,
                Scale,
                Scroll,
                Rot,
                RotRandom
            };

            Display = new() {
                CalculateUVType
            };

            Curves = new() {
                Scale,
                Scroll,
                Rot,
                RotRandom
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Parsed, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        public override void Draw( string parentId ) {
            var id = parentId + "/UV";
            IUiBase.DrawList( Display, id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            IUiItem.DrawListTabs( Curves, parentId );
        }

        public override string GetDefaultText() => $"UV {GetIdx()}";
    }
}
