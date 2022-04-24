using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Emitter;

namespace VFXEditor.AVFX.VFX {
    public class UIEmitter : UINode {
        public readonly AVFXEmitter Emitter;
        public readonly AVFXFile Main;

        public readonly UICombo<EmitterType> Type;
        public readonly List<UIItem> Animation;
        public readonly UIItemSplitView<UIItem> AnimationSplit;

        public readonly List<UIEmitterItem> ParticleList;
        public readonly List<UIEmitterItem> EmitterList;

        public UIData Data;

        public readonly UIEmitterSplitView EmitterSplit;
        public readonly UIEmitterSplitView ParticleSplit;
        public readonly UINodeSelect<UIEffector> EffectorSelect;
        public UINodeGraphView NodeView;

        public readonly UIString SoundInput;
        public readonly UIInt SoundIndex;
        private readonly List<UIBase> Parameters;

        public UIEmitter( AVFXFile main, AVFXEmitter emitter, bool has_dependencies = false ) : base( UINodeGroup.EmitterColor, has_dependencies ) {
            Emitter = emitter;
            Main = main;
            EffectorSelect = new UINodeSelect<UIEffector>( this, "Effector Select", Main.Effectors, Emitter.EffectorIdx );
            NodeView = new UINodeGraphView( this );

            Animation = new List<UIItem>();
            ParticleList = new List<UIEmitterItem>();
            EmitterList = new List<UIEmitterItem>();


            Type = new UICombo<EmitterType>( "Type", Emitter.EmitterVariety, onChange: () => {
                Emitter.SetType( Emitter.EmitterVariety.GetValue() );
                SetType();
            } );
            SoundInput = new UIString( "Sound", Emitter.Sound, canBeUnassigned: true );
            SoundIndex = new UIInt( "Sound Index (-1 if no sound)", Emitter.SoundNumber );

            Parameters = new List<UIBase> {
                new UIInt( "Loop Start", Emitter.LoopStart ),
                new UIInt( "Loop End", Emitter.LoopEnd ),
                new UIInt( "Child Limit", Emitter.ChildLimit ),
                new UICheckbox( "Any Direction", Emitter.AnyDirection ),
                new UICombo<RotationDirectionBase>( "Rotation Direction Base", Emitter.RotationDirectionBaseType ),
                new UICombo<CoordComputeOrder>( "Coordinate Compute Order", Emitter.CoordComputeOrderType ),
                new UICombo<RotationOrder>( "Rotation Order", Emitter.RotationOrderType )
            };
            Animation.Add( new UILife( Emitter.Life ) );
            Animation.Add( new UICurve( Emitter.CreateCount, "Create Count", locked: true ) );
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

            foreach( var particle in Emitter.Particles ) {
                ParticleList.Add( new UIEmitterItem( particle, true, this ) );
            }

            foreach( var e in Emitter.Emitters ) {
                EmitterList.Add( new UIEmitterItem( e, false, this ) );
            }

            SetType();

            AnimationSplit = new UIItemSplitView<UIItem>( Animation );
            EmitterSplit = new UIEmitterSplitView( EmitterList, this, false );
            ParticleSplit = new UIEmitterSplitView( ParticleList, this, true );
            HasDependencies = false; // if imported, all set now
        }
        public void SetType() {
            Data?.Dispose();
            Data = Emitter.EmitterVariety.GetValue() switch {
                EmitterType.SphereModel => new UIEmitterDataSphereModel( ( AVFXEmitterDataSphereModel )Emitter.Data ),
                EmitterType.CylinderModel => new UIEmitterDataCylinderModel( ( AVFXEmitterDataCylinderModel )Emitter.Data ),
                EmitterType.Model => new UIEmitterDataModel( ( AVFXEmitterDataModel )Emitter.Data, this ),
                EmitterType.Cone => new UIEmitterDataCone( ( AVFXEmitterDataCone )Emitter.Data ),
                EmitterType.ConeModel => new UIEmitterDataConeModel( ( AVFXEmitterDataConeModel )Emitter.Data ),
                _ => null
            };
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            NodeView.Draw( id );
            EffectorSelect.Draw( id );
            SoundInput.Draw( id );
            SoundIndex.Draw( id );
            DrawList( Parameters, id );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Emitter";
            DrawRename( id );
            Type.Draw( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

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

        public override string GetDefaultText() => $"Emitter {Idx}({Emitter.EmitterVariety.GetValue()})";

        public override string GetWorkspaceId() => $"Emit{Idx}";

        public override void PopulateWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            EmitterList.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
            ParticleList.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
        }

        public override void ReadWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            EmitterList.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
            ParticleList.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
        }

        public override void Write( BinaryWriter writer ) => Emitter.Write( writer );
    }
}
