using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitter : UiNode {
        public readonly AVFXEmitter Emitter;
        public readonly UiNodeGroupSet NodeGroups;

        public readonly UiCombo<EmitterType> Type;
        public readonly List<UiItem> Animation;
        public readonly UiItemSplitView<UiItem> AnimationSplit;

        public readonly List<UiEmitterItem> ParticleList;
        public readonly List<UiEmitterItem> EmitterList;

        public UiData Data;

        public readonly UiEmitterSplitView EmitterSplit;
        public readonly UiEmitterSplitView ParticleSplit;
        public readonly UiNodeSelect<UiEffector> EffectorSelect;
        private UiNodeGraphView NodeView;

        public readonly UiString SoundInput;
        public readonly UiInt SoundIndex;
        private readonly List<IUiBase> Parameters;

        public UiEmitter( AVFXEmitter emitter, UiNodeGroupSet nodeGroups, bool hasDependencies = false ) : base( UiNodeGroup.EmitterColor, hasDependencies ) {
            Emitter = emitter;
            NodeGroups = nodeGroups;

            EffectorSelect = new UiNodeSelect<UiEffector>( this, "Effector Select", nodeGroups.Effectors, Emitter.EffectorIdx );
            NodeView = new UiNodeGraphView( this );

            Animation = new List<UiItem>();
            ParticleList = new List<UiEmitterItem>();
            EmitterList = new List<UiEmitterItem>();


            Type = new UiCombo<EmitterType>( "Type", Emitter.EmitterVariety, extraCommand: () => {
                return new UiEmitterDataExtraCommand( this );
            } );
            SoundInput = new UiString( "Sound", Emitter.Sound, showRemoveButton: true );
            SoundIndex = new UiInt( "Sound Index (-1 if no sound)", Emitter.SoundNumber );

            Parameters = new List<IUiBase> {
                new UiInt( "Loop Start", Emitter.LoopStart ),
                new UiInt( "Loop End", Emitter.LoopEnd ),
                new UiInt( "Child Limit", Emitter.ChildLimit ),
                new UiCheckbox( "Any Direction", Emitter.AnyDirection ),
                new UiCombo<RotationDirectionBase>( "Rotation Direction Base", Emitter.RotationDirectionBaseType ),
                new UiCombo<CoordComputeOrder>( "Coordinate Compute Order", Emitter.CoordComputeOrderType ),
                new UiCombo<RotationOrder>( "Rotation Order", Emitter.RotationOrderType )
            };
            Animation.Add( new UiLife( Emitter.Life ) );
            Animation.Add( new UiCurve( Emitter.CreateCount, "Create Count", locked: true ) );
            Animation.Add( new UiCurve( Emitter.CreateInterval, "Create Interval", locked: true ) );
            Animation.Add( new UiCurve( Emitter.CreateIntervalRandom, "Create Interval Random" ) );
            Animation.Add( new UiCurve( Emitter.Gravity, "Gravity" ) );
            Animation.Add( new UiCurve( Emitter.GravityRandom, "Gravity Random" ) );
            Animation.Add( new UiCurve( Emitter.AirResistance, "Air Resistance", locked: true ) );
            Animation.Add( new UiCurve( Emitter.AirResistanceRandom, "Air Resistance Random" ) );
            Animation.Add( new UiCurveColor( Emitter.Color, "Color", locked: true ) );
            Animation.Add( new UiCurve3Axis( Emitter.Position, "Position", locked: true ) );
            Animation.Add( new UiCurve3Axis( Emitter.Rotation, "Rotation", locked: true ) );
            Animation.Add( new UiCurve3Axis( Emitter.Scale, "Scale", locked: true ) );

            foreach( var particle in Emitter.Particles ) {
                ParticleList.Add( new UiEmitterItem( particle, true, this ) );
            }

            foreach( var e in Emitter.Emitters ) {
                EmitterList.Add( new UiEmitterItem( e, false, this ) );
            }

            UpdateDataType();

            AnimationSplit = new UiItemSplitView<UiItem>( Animation );
            EmitterSplit = new UiEmitterSplitView( EmitterList, this, false );
            ParticleSplit = new UiEmitterSplitView( ParticleList, this, true );
            HasDependencies = false; // if imported, all set now
        }

        public void UpdateDataType() {
            Data?.Disable();
            Data = Emitter.EmitterVariety.GetValue() switch {
                EmitterType.SphereModel => new UiEmitterDataSphereModel( ( AVFXEmitterDataSphereModel )Emitter.Data ),
                EmitterType.CylinderModel => new UiEmitterDataCylinderModel( ( AVFXEmitterDataCylinderModel )Emitter.Data ),
                EmitterType.Model => new UiEmitterDataModel( ( AVFXEmitterDataModel )Emitter.Data, this ),
                EmitterType.Cone => new UiEmitterDataCone( ( AVFXEmitterDataCone )Emitter.Data ),
                EmitterType.ConeModel => new UiEmitterDataConeModel( ( AVFXEmitterDataConeModel )Emitter.Data ),
                _ => null
            };
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            NodeView.DrawInline( id );
            EffectorSelect.DrawInline( id );

            SoundInput.DrawInline( id );
            SoundIndex.DrawInline( id );

            IUiBase.DrawList( Parameters, id );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.DrawInline( id );
            ImGui.EndChild();
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Emitter";
            DrawRename( id );
            Type.DrawInline( id );
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
                    AnimationSplit.DrawInline( id + "/Anim" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Create Particles" + id ) ) {
                    ParticleSplit.DrawInline( id + "/ItPr" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Create Emitters" + id ) ) {
                    EmitterSplit.DrawInline( id + "/ItEm" );
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
            EmitterList.ForEach( item => item.ReadWorkspaceMeta( RenameDict ) );
            ParticleList.ForEach( item => item.ReadWorkspaceMeta( RenameDict ) );
        }

        public override void Write( BinaryWriter writer ) => Emitter.Write( writer );
    }
}
