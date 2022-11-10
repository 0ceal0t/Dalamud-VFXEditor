using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticle : UiNode {
        public readonly AVFXParticle Particle;
        public readonly UiNodeGroupSet NodeGroups;
        public readonly UiCombo<ParticleType> Type;
        public readonly List<UiParticleUvSet> UvSets;

        public UiData Data;

        public readonly List<UiItem> Animation;
        public readonly UiTextureColor1 TC1;
        public readonly UiTextureColor2 TC2;
        public readonly UiTextureColor2 TC3;
        public readonly UiTextureColor2 TC4;
        public readonly UiTextureNormal TN;
        public readonly UiTextureDistortion TD;
        public readonly UiTexturePalette TP;
        public readonly UiTextureReflection TR;
        public readonly List<UiItem> Tex;
        public readonly UiItemSplitView<UiItem> AnimationSplit;
        public readonly UiItemSplitView<UiItem> TexSplit;
        public readonly UiUvSetSplitView UvSet;
        public readonly UiNodeGraphView NodeView;
        private readonly List<IUiBase> Parameters;

        public UiParticle( AVFXParticle particle, UiNodeGroupSet nodeGroups, bool hasDependencies = false ) : base( UiNodeGroup.ParticleColor, hasDependencies ) {
            Particle = particle;
            NodeGroups = nodeGroups;
            NodeView = new UiNodeGraphView( this );

            Animation = new List<UiItem>();
            Tex = new List<UiItem>();
            UvSets = new List<UiParticleUvSet>();

            Type = new UiCombo<ParticleType>( "Type", Particle.ParticleVariety, extraCommand: () => {
                //Particle.SetType( Particle.ParticleVariety.GetValue() );
                //SetType();
                // TODO
                return null;
            } );
            Parameters = new List<IUiBase> {
                new UiInt( "Loop Start", Particle.LoopStart ),
                new UiInt( "Loop End", Particle.LoopEnd ),
                new UiCheckbox( "Use Simple Animation", Particle.SimpleAnimEnable ),
                new UiCombo<RotationDirectionBase>( "Rotation Direction Base", Particle.RotationDirectionBaseType ),
                new UiCombo<RotationOrder>( "Rotation Compute Order", Particle.RotationOrderType ),
                new UiCombo<CoordComputeOrder>( "Coord Compute Order", Particle.CoordComputeOrderType ),
                new UiCombo<DrawMode>( "Draw Mode", Particle.DrawModeType ),
                new UiCombo<CullingType>( "Culling Type", Particle.CullingTypeType ),
                new UiCombo<EnvLight>( "Enviornmental Light", Particle.EnvLightType ),
                new UiCombo<DirLight>( "Directional Light", Particle.DirLightType ),
                new UiCombo<UVPrecision>( "UV Precision", Particle.UvPrecisionType ),
                new UiInt( "Draw Priority", Particle.DrawPriority ),
                new UiCheckbox( "Depth Test", Particle.IsDepthTest ),
                new UiCheckbox( "Depth Write", Particle.IsDepthWrite ),
                new UiCheckbox( "Soft Particle", Particle.IsSoftParticle ),
                new UiInt( "Collision Type", Particle.CollisionType ),
                new UiCheckbox( "BS11", Particle.Bs11 ),
                new UiCheckbox( "Apply Tone Map", Particle.IsApplyToneMap ),
                new UiCheckbox( "Apply Fog", Particle.IsApplyFog ),
                new UiCheckbox( "Enable Clip Near", Particle.ClipNearEnable ),
                new UiCheckbox( "Enable Clip Far", Particle.ClipFarEnable ),
                new UiFloat2( "Clip Near", Particle.ClipNearStart, Particle.ClipNearEnd ),
                new UiFloat2( "Clip Far", Particle.ClipFarStart, Particle.ClipFarEnd ),
                new UiCombo<ClipBasePoint>( "Clip Base Point", Particle.ClipBasePointType ),
                new UiInt( "Apply Rate Environment", Particle.ApplyRateEnvironment ),
                new UiInt( "Apply Rate Directional", Particle.ApplyRateDirectional ),
                new UiInt( "Apply Rate Light Buffer", Particle.ApplyRateLightBuffer ),
                new UiCheckbox( "DOTy", Particle.DOTy ),
                new UiFloat( "Depth Offset", Particle.DepthOffset )
            };

            Animation.Add( new UiLife( Particle.Life ) );
            Animation.Add( new UiParticleSimple( Particle.Simple, this ) );
            Animation.Add( new UiCurve( Particle.Gravity, "Gravity" ) );
            Animation.Add( new UiCurve( Particle.GravityRandom, "Gravity Random" ) );
            Animation.Add( new UiCurve( Particle.AirResistance, "Air Resistance", locked: true ) );
            Animation.Add( new UiCurve( Particle.AirResistanceRandom, "Air Resistance Random" ) );
            Animation.Add( new UiCurve3Axis( Particle.Scale, "Scale", locked: true ) );
            Animation.Add( new UiCurve3Axis( Particle.Rotation, "Rotation", locked: true ) );
            Animation.Add( new UiCurve3Axis( Particle.Position, "Position", locked: true ) );
            Animation.Add( new UiCurve( Particle.RotVelX, "Rotation Velocity X" ) );
            Animation.Add( new UiCurve( Particle.RotVelY, "Rotation Velocity Y" ) );
            Animation.Add( new UiCurve( Particle.RotVelZ, "Rotation Velocity Z" ) );
            Animation.Add( new UiCurve( Particle.RotVelXRandom, "Rotation Velocity X Random" ) );
            Animation.Add( new UiCurve( Particle.RotVelYRandom, "Rotation Velocity Y Random" ) );
            Animation.Add( new UiCurve( Particle.RotVelZRandom, "Rotation Velocity Z Random" ) );
            Animation.Add( new UiCurveColor( Particle.Color, "Color", locked: true ) );

            foreach( var uvSet in Particle.UvSets ) {
                UvSets.Add( new UiParticleUvSet( uvSet, this ) );
            }

            SetType();

            Tex.Add( TC1 = new UiTextureColor1( Particle.TC1, this ) );
            Tex.Add( TC2 = new UiTextureColor2( Particle.TC2, "Texture Color 2", this ) );
            Tex.Add( TC3 = new UiTextureColor2( Particle.TC3, "Texture Color 3", this ) );
            Tex.Add( TC4 = new UiTextureColor2( Particle.TC4, "Texture Color 4", this ) );
            Tex.Add( TN = new UiTextureNormal( Particle.TN, this ) );
            Tex.Add( TR = new UiTextureReflection( Particle.TR, this ) );
            Tex.Add( TD = new UiTextureDistortion( Particle.TD, this ) );
            Tex.Add( TP = new UiTexturePalette( Particle.TP, this ) );

            AnimationSplit = new UiItemSplitView<UiItem>( Animation );
            TexSplit = new UiItemSplitView<UiItem>( Tex );
            UvSet = new UiUvSetSplitView( UvSets, this );
            HasDependencies = false; // if imported, all set now
        }

        public void SetType() {
            Data?.Disable();
            Data = Particle.ParticleVariety.GetValue() switch {
                ParticleType.Model => new UiParticleDataModel( ( AVFXParticleDataModel )Particle.Data, this ),
                ParticleType.LightModel => new UiParticleDataLightModel( ( AVFXParticleDataLightModel )Particle.Data, this ),
                ParticleType.Powder => new UiParticleDataPowder( ( AVFXParticleDataPowder )Particle.Data ),
                ParticleType.Decal => new UiParticleDataDecal( ( AVFXParticleDataDecal )Particle.Data ),
                ParticleType.DecalRing => new UiParticleDataDecalRing( ( AVFXParticleDataDecalRing )Particle.Data ),
                ParticleType.Disc => new UiParticleDataDisc( ( AVFXParticleDataDisc )Particle.Data ),
                ParticleType.Laser => new UiParticleDataLaser( ( AVFXParticleDataLaser )Particle.Data ),
                ParticleType.Polygon => new UiParticleDataPolygon( ( AVFXParticleDataPolygon )Particle.Data ),
                ParticleType.Polyline => new UiParticleDataPolyline( ( AVFXParticleDataPolyline )Particle.Data ),
                ParticleType.Windmill => new UiParticleDataWindmill( ( AVFXParticleDataWindmill )Particle.Data ),
                ParticleType.Line => new UiParticleDataLine( ( AVFXParticleDataLine )Particle.Data ),
                _ => null
            };
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Ptcl";
            DrawRename( id );
            Type.DrawInline( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters" + id ) ) {
                    DrawParameters( id + "/Param" );
                    ImGui.EndTabItem();
                }
                if( Data != null && ImGui.BeginTabItem( "Data" + id ) ) {
                    DrawData( id + "/Data" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Animation" + id ) ) {
                    AnimationSplit.DrawInline( id + "/Animation" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "UV Sets" + id ) ) {
                    UvSet.DrawInline( id + "/UVSets" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Textures" + id ) ) {
                    TexSplit.DrawInline( id + "/Tex" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            NodeView.DrawInline( id );
            IUiBase.DrawList( Parameters, id );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.DrawInline( id );
            ImGui.EndChild();
        }

        public override string GetDefaultText() => $"Particle {Idx}({Particle.ParticleVariety.GetValue()})";

        public override string GetWorkspaceId() => $"Ptcl{Idx}";

        public override void Write( BinaryWriter writer ) => Particle.Write( writer );
    }
}
