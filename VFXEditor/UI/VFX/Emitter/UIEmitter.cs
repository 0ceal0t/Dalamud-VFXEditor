using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEmitter : UINode {
        public AVFXEmitter Emitter;
        public UIMain Main;
        //=======================
        public UICombo<EmitterType> Type;
        List<UIItem> Animation;
        UIItemSplitView<UIItem> AnimationSplit;
        //========================
        public List<UIEmitterItem> ParticleList;
        public List<UIEmitterItem> EmitterList;
        //========================
        public UIData Data;
        //========================
        public UIEmitterSplitView EmitterSplit;
        public UIEmitterSplitView ParticleSplit;
        public UINodeSelect<UIEffector> EffectorSelect;
        public UINodeGraphView NodeView;
        //========================
        public UIString SoundInput;
        public UIInt SoundIndex;

        public UIEmitter(UIMain main, AVFXEmitter emitter, bool has_dependencies = false ) : base( UINodeGroup.EmitterColor, has_dependencies) {
            Emitter = emitter;
            Main = main;
            EffectorSelect = new UINodeSelect<UIEffector>( this, "Effector Select", Main.Effectors, Emitter.EffectorIdx );
            NodeView = new UINodeGraphView( this );
            // =====================
            Animation = new List<UIItem>();
            ParticleList = new List<UIEmitterItem>();
            EmitterList = new List<UIEmitterItem>();
            //======================
            Type = new UICombo<EmitterType>( "Type", Emitter.EmitterVariety, changeFunction: ChangeType );
            SoundInput = new UIString( "Sound", Emitter.Sound, changeFunction: SoundCheck );
            SoundIndex = new UIInt( "Sound Index", Emitter.SoundNumber );
            //======================
            Attributes.Add( new UIInt( "Loop Start", Emitter.LoopStart ) );
            Attributes.Add( new UIInt( "Loop End", Emitter.LoopEnd ) );
            Attributes.Add( new UIInt( "Child Limit", Emitter.ChildLimit ) );
            Attributes.Add( new UICheckbox( "Any Direction", Emitter.AnyDirection ) );
            Attributes.Add( new UICombo<RotationDirectionBase>( "Rotation Direction Base", Emitter.RotationDirectionBaseType ) );
            Attributes.Add( new UICombo<CoordComputeOrder>( "Coordinate Compute Order", Emitter.CoordComputeOrderType ) );
            Attributes.Add( new UICombo<RotationOrder>( "Rotation Order", Emitter.RotationOrderType ) );
            Animation.Add( new UILife( Emitter.Life ) );
            Animation.Add( new UICurve( Emitter.CreateCount, "Create Count", locked:true ) );
            Animation.Add( new UICurve( Emitter.CreateInterval, "Create Interval", locked: true ) );
            Animation.Add( new UICurve( Emitter.CreateIntervalRandom, "Create Interval Random" ) );
            Animation.Add( new UICurve( Emitter.Gravity, "Gravity" ) );
            Animation.Add( new UICurve( Emitter.GravityRandom, "Gravity Random" ) );
            Animation.Add( new UICurve( Emitter.AirResistance, "Air Resistance", locked: true ) );
            Animation.Add( new UICurve( Emitter.AirResistanceRandom, "Air Resistance Random" ) );
            Animation.Add( new UICurveColor( Emitter.Color, "Color", locked: true ) );
            Animation.Add( new UICurve3Axis( Emitter.Position, "Position", locked: true ) );
            Animation.Add( new UICurve3Axis( Emitter.Rotation, "Rotation", locked: true ) );
            Animation.Add( new UICurve3Axis( Emitter.Scale, "Scale", locked: true ) );
            //========================
            foreach( var particle in Emitter.Particles ) {
                ParticleList.Add( new UIEmitterItem( particle, true, this ) );
            }
            //============================
            foreach( var e in Emitter.Emitters ) {
                EmitterList.Add( new UIEmitterItem( e, false, this ) );
            }
            //=======================
            SetType();
            //=============================
            AnimationSplit = new UIItemSplitView<UIItem>( Animation );
            EmitterSplit = new UIEmitterSplitView( EmitterList, this, false );
            ParticleSplit = new UIEmitterSplitView( ParticleList, this, true );
            HasDependencies = false; // if imported, all set now
        }
        public void SetType() {
            Data?.Dispose();

            switch( Emitter.EmitterVariety.Value ) {
                case EmitterType.Point:
                    Data = null;
                    break;
                case EmitterType.SphereModel:
                    Data = new UIEmitterDataSphereModel( ( AVFXEmitterDataSphereModel )Emitter.Data );
                    break;
                case EmitterType.CylinderModel:
                    Data = new UIEmitterDataCylinderModel( ( AVFXEmitterDataCylinderModel )Emitter.Data );
                    break;
                case EmitterType.Model:
                    Data = new UIEmitterDataModel( ( AVFXEmitterDataModel )Emitter.Data, this );
                    break;
                case EmitterType.Cone:
                    Data = new UIEmitterDataCone( ( AVFXEmitterDataCone )Emitter.Data );
                    break;
                case EmitterType.ConeModel:
                    Data = new UIEmitterDataConeModel( ( AVFXEmitterDataConeModel )Emitter.Data );
                    break;
                default:
                    Data = null;
                    break;
            }
        }
        public void ChangeType(LiteralEnum<EmitterType> literal)
        {
            Emitter.SetVariety(literal.Value);
            SetType();
        }

        private void DrawParameters( string id )
        {
            ImGui.BeginChild( id );
            NodeView.Draw( id );
            EffectorSelect.Draw( id );
            if(UIUtils.RemoveButton("Remove Sound" + id, small: true ) ) {
                SoundInput.Value = "";
                SoundInput.Literal.GiveValue( "" );
                SoundInput.Literal.Assigned = false;

                SoundIndex.Value = -1;
                SoundIndex.Literal.GiveValue( -1 );
            }
            SoundInput.Draw( id );
            SoundIndex.Draw( id );
            DrawAttrs( id );
            ImGui.EndChild();
        }
        private void DrawData( string id )
        {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }

        public override void DrawBody( string parentId ) {
            string id = parentId + "/Emitter";
            DrawRename( id );
            Type.Draw( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            //==========================
            if( ImGui.BeginTabBar( id + "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters" + id ) ) {
                    DrawParameters( id + "/Param" );
                    ImGui.EndTabItem();
                }
                if( Data != null && ImGui.BeginTabItem( "Data" + id ) ) {
                    DrawData( id + "/Data" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Animation" + id ) ) {
                    AnimationSplit.Draw( id + "/Anim" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Create Particles" + id ) ) {
                    ParticleSplit.Draw( id + "/ItPr" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Create Emitters" + id ) ) {
                    EmitterSplit.Draw( id + "/ItEm" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        public void SoundCheck(LiteralString literal ) { // not enough to just set the value to "", need to also unassign it
            if(literal.Value.Trim('\0').Length == 0) {
                literal.Assigned = false;
            }
        }

        public override string GetDefaultText() {
            return "Emitter " + Idx + "(" + Emitter.EmitterVariety.stringValue() + ")";
        }

        public override byte[] ToBytes() {
            return Emitter.ToAVFX().ToBytes();
        }
    }
}
