using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticle : UINode {
        public AVFXParticle Particle;
        public UIParticleView View;
        // =======================
        public UICombo<ParticleType> Type;
        public List<UIParticleUVSet> UVSets;
        //==========================
        public UIData Data;
        //========================
        List<UIItem> Animation;
        public UITextureColor1 TC1;
        public UITextureColor2 TC2;
        public UITextureColor2 TC3;
        public UITextureColor2 TC4;
        public UITextureNormal TN;
        public UITextureDistortion TD;
        public UITexturePalette TP;
        public UITextureReflection TR;
        List<UIItem> Tex;
        // ==================
        UIItemSplitView<UIItem> AnimationSplit;
        UIItemSplitView<UIItem> TexSplit;
        public UIUVSetSplitView UVSplit;
        public UINodeGraphView NodeView;

        public UIParticle( AVFXParticle particle, UIParticleView view, bool has_dependencies = false ) : base( ParticleColor, has_dependencies ) {
            Particle = particle;
            View = view;
            NodeView = new UINodeGraphView( this );
            // =======================
            Animation = new List<UIItem>();
            Tex = new List<UIItem>();
            UVSets = new List<UIParticleUVSet>();
            //==========================
            Type = new UICombo<ParticleType>( "Type", Particle.ParticleVariety, changeFunction: ChangeType );
            Attributes.Add( new UIInt( "Loop Start", Particle.LoopStart ) );
            Attributes.Add( new UIInt( "Loop End", Particle.LoopEnd ) );
            Attributes.Add( new UICheckbox( "Use Simple Animation", Particle.SimpleAnimEnable ) );
            Attributes.Add( new UICombo<RotationDirectionBase>( "Rotation Direction Base", Particle.RotationDirectionBaseType ) );
            Attributes.Add( new UICombo<RotationOrder>( "Rotation Compute Order", Particle.RotationOrderType ) );
            Attributes.Add( new UICombo<CoordComputeOrder>( "Coord Compute Order", Particle.CoordComputeOrderType ) );
            Attributes.Add( new UICombo<DrawMode>( "Draw Mode", Particle.DrawModeType ) );
            Attributes.Add( new UICombo<CullingType>( "Culling Type", Particle.CullingTypeType ) );
            Attributes.Add( new UICombo<EnvLight>( "Enviornmental Light", Particle.EnvLightType ) );
            Attributes.Add( new UICombo<DirLight>( "Directional Light", Particle.DirLightType ) );
            Attributes.Add( new UICombo<UVPrecision>( "UV Precision", Particle.UvPrecisionType ) );
            Attributes.Add( new UIInt( "Draw Priority", Particle.DrawPriority ) );
            Attributes.Add( new UICheckbox( "Depth Test", Particle.IsDepthTest ) );
            Attributes.Add( new UICheckbox( "Depth Write", Particle.IsDepthWrite ) );
            Attributes.Add( new UICheckbox( "Soft Particle", Particle.IsSoftParticle ) );
            Attributes.Add( new UIInt( "Collision Type", Particle.CollisionType ) );
            Attributes.Add( new UICheckbox( "BS11", Particle.Bs11 ) );
            Attributes.Add( new UICheckbox( "Apply Tone Map", Particle.IsApplyToneMap ) );
            Attributes.Add( new UICheckbox( "Apply Fog", Particle.IsApplyFog ) );
            Attributes.Add( new UICheckbox( "Enable Clip Near", Particle.ClipNearEnable ) );
            Attributes.Add( new UICheckbox( "Enable Clip Far", Particle.ClipFarEnable ) );
            Attributes.Add( new UIFloat2( "Clip Near", Particle.ClipNearStart, Particle.ClipNearEnd ) );
            Attributes.Add( new UIFloat2( "Clip Far", Particle.ClipFarStart, Particle.ClipFarEnd ) );
            Attributes.Add( new UICombo<ClipBasePoint>( "Clip Base Point", Particle.ClipBasePointType ) );
            Attributes.Add( new UIInt( "Apply Rate Environment", Particle.ApplyRateEnvironment ) );
            Attributes.Add( new UIInt( "Apply Rate Directional", Particle.ApplyRateDirectional ) );
            Attributes.Add( new UIInt( "Apply Rate Light Buffer", Particle.ApplyRateLightBuffer ) );
            Attributes.Add( new UICheckbox( "DOTy", Particle.DOTy ) );
            Attributes.Add( new UIFloat( "Depth Offset", Particle.DepthOffset ) );
            //==============================
            Animation.Add( new UILife( Particle.Life ) );
            Animation.Add( new UIParticleSimple( Particle.Simple ) );
            Animation.Add( new UICurve( Particle.Gravity, "Gravity" ) );
            Animation.Add( new UICurve( Particle.GravityRandom, "Gravity Random" ) );
            Animation.Add( new UICurve( Particle.AirResistance, "Air Resistance", locked: true ) );
            Animation.Add( new UICurve( Particle.AirResistanceRandom, "Air Resistance Random" ) );
            Animation.Add( new UICurve3Axis( Particle.Scale, "Scale", locked: true ) );
            Animation.Add( new UICurve3Axis( Particle.Rotation, "Rotation", locked: true ) );
            Animation.Add( new UICurve3Axis( Particle.Position, "Position", locked: true ) );
            Animation.Add( new UICurve( Particle.RotVelX, "Rotation Velocity X" ) );
            Animation.Add( new UICurve( Particle.RotVelY, "Rotation Velocity Y" ) );
            Animation.Add( new UICurve( Particle.RotVelZ, "Rotation Velocity Z" ) );
            Animation.Add( new UICurve( Particle.RotVelXRandom, "Rotation Velocity X Random" ) );
            Animation.Add( new UICurve( Particle.RotVelYRandom, "Rotation Velocity Y Random" ) );
            Animation.Add( new UICurve( Particle.RotVelZRandom, "Rotation Velocity Z Random" ) );
            Animation.Add( new UICurveColor( Particle.Color, "Color", locked: true ) );
            //===============================
            foreach( var uvSet in Particle.UVSets ) {
                UVSets.Add( new UIParticleUVSet( uvSet, this ) );
            }
            //===============================
            SetType();
            //============================
            TC1 = new UITextureColor1( Particle.TC1, this );
            TC2 = new UITextureColor2( Particle.TC2, "Texture Color 2", this );
            TC3 = new UITextureColor2( Particle.TC3, "Texture Color 3", this );
            TC4 = new UITextureColor2( Particle.TC4, "Texture Color 4", this );
            TN = new UITextureNormal( Particle.TN, this );
            TR = new UITextureReflection( Particle.TR, this );
            TD = new UITextureDistortion( Particle.TD, this );
            TP = new UITexturePalette( Particle.TP, this );
            Tex.Add( TC1 );
            Tex.Add( TC2 );
            Tex.Add( TC3 );
            Tex.Add( TC4 );
            Tex.Add( TN );
            Tex.Add( TR );
            Tex.Add( TD );
            Tex.Add( TP );
            //=============================
            AnimationSplit = new UIItemSplitView<UIItem>( Animation );
            TexSplit = new UIItemSplitView<UIItem>( Tex );
            UVSplit = new UIUVSetSplitView( UVSets, this );
        }
        public void SetType() {
            Data?.Cleanup();

            switch( Particle.ParticleVariety.Value ) {
                case ParticleType.Model:
                    Data = new UIParticleDataModel( ( AVFXParticleDataModel )Particle.Data, this );
                    break;
                case ParticleType.LightModel:
                    Data = new UIParticleDataLightModel( ( AVFXParticleDataLightModel )Particle.Data, this );
                    break;
                case ParticleType.Powder:
                    Data = new UIParticleDataPowder( ( AVFXParticleDataPowder )Particle.Data );
                    break;
                case ParticleType.Decal:
                    Data = new UIParticleDataDecal( ( AVFXParticleDataDecal )Particle.Data );
                    break;
                case ParticleType.DecalRing:
                    Data = new UIParticleDataDecalRing( ( AVFXParticleDataDecalRing )Particle.Data );
                    break;
                case ParticleType.Disc:
                    Data = new UIParticleDataDisc( ( AVFXParticleDataDisc )Particle.Data );
                    break;
                case ParticleType.Laser:
                    Data = new UIParticleDataLaser( ( AVFXParticleDataLaser )Particle.Data );
                    break;
                case ParticleType.Polygon:
                    Data = new UIParticleDataPolygon( ( AVFXParticleDataPolygon )Particle.Data );
                    break;
                case ParticleType.Polyline:
                    Data = new UIParticleDataPolyline( ( AVFXParticleDataPolyline )Particle.Data );
                    break;
                case ParticleType.Windmill:
                    Data = new UIParticleDataWindmill( ( AVFXParticleDataWindmill )Particle.Data );
                    break;
                case ParticleType.Line:
                    Data = new UIParticleDataLine( ( AVFXParticleDataLine )Particle.Data );
                    break;
                default:
                    Data = null;
                    break;
            }
        }
        public void ChangeType( LiteralEnum<ParticleType> literal ) {
            Particle.SetVariety( literal.Value );
            SetType();
        }

        private void DrawParameters(string id)
        {
            ImGui.BeginChild( id);
            NodeView.Draw( id );
            DrawAttrs( id );
            ImGui.EndChild();
        }
        private void DrawData( string id )
        {
            ImGui.BeginChild( id);
            Data.Draw( id );
            ImGui.EndChild();
        }

        public override void DrawBody( string parentId ) {
            string id = parentId + "/Ptcl";
            Type.Draw( id );
            //=====================
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
                    AnimationSplit.Draw( id + "/Animation");
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "UV Sets" + id ) ) {
                    UVSplit.Draw( id + "/UVSets");
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Textures" + id ) ) {
                    TexSplit.Draw( id + "/Tex");
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        public override string GetText() {
            return "Particle " + Idx + "(" + Particle.ParticleVariety.stringValue() + ")";
        }

        public override byte[] toBytes() {
            return Particle.toAVFX().toBytes();
        }
    }
}
