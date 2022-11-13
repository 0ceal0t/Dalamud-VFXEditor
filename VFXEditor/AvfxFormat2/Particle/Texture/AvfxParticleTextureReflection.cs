using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleTextureReflection : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxBool UseScreenCopy = new( "Use Screen Copy", "bUSC" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureCalculateColor> TextureCalculateColorType = new( "Calculate Color", "TCCT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo" );
        public readonly AvfxCurve Rate = new( "Rate", "Rate" );
        public readonly AvfxCurve RPow = new( "Power", "RPow" );

        private readonly List<AvfxBase> Children;

        public AvfxParticleTextureReflection( AvfxParticle particle ) : base( "TR", particle ) {
            InitNodeSelects();

            Children = new() {
                Enabled,
                UseScreenCopy,
                TextureFilter,
                TextureCalculateColorType,
                TextureIdx,
                Rate,
                RPow
            };
            TextureIdx.SetValue( -1 );

            Parameters.Add( Enabled );
            Parameters.Add( UseScreenCopy );
            Parameters.Add( TextureFilter );
            Parameters.Add( TextureCalculateColorType );

            Tabs.Add( Rate );
            Tabs.Add( RPow );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Children, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) ) Assign();
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TR";
            IUiItem.DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Texture Reflection";

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        };
    }
}
