using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleTextureReflection : AvfxParticleAttribute {
        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxBool UseScreenCopy = new( "Use Screen Copy", "bUSC" );
        public readonly AvfxEnum<TextureFilterType> TextureFilter = new( "Texture Filter", "TFT" );
        public readonly AvfxEnum<TextureCalculateColor> TextureCalculateColorType = new( "Calculate Color", "TCCT" );
        public readonly AvfxInt TextureIdx = new( "Texture Index", "TxNo" );
        public readonly AvfxCurve Rate = new( "Rate", "Rate" );
        public readonly AvfxCurve RPow = new( "Power", "RPow" );

        private readonly List<AvfxBase> Parsed;

        public AvfxParticleTextureReflection( AvfxParticle particle ) : base( "TR", particle ) {
            InitNodeSelects();

            Parsed = new() {
                Enabled,
                UseScreenCopy,
                TextureFilter,
                TextureCalculateColorType,
                TextureIdx,
                Rate,
                RPow
            };
            // Default
            TextureIdx.SetValue( -1 );

            Display.Add( Enabled );
            Display.Add( UseScreenCopy );
            Display.Add( TextureFilter );
            Display.Add( TextureCalculateColorType );

            DisplayTabs.Add( Rate );
            DisplayTabs.Add( RPow );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            ReadNested( reader, Parsed, size );
            EnableAllSelectors();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Parsed );

        public override void DrawUnassigned( string parentId ) {
            AssignedCopyPaste( this, GetDefaultText() );
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) ) Assign();
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/TR";

            AssignedCopyPaste( this, GetDefaultText() );
            IUiItem.DrawListTabs( DisplayTabs, id );
        }

        public override string GetDefaultText() => "Texture Reflection";

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<AvfxTexture>( Particle, "Texture", Particle.NodeGroups.Textures, TextureIdx )
        };
    }
}
