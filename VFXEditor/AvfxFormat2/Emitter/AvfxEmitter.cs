using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEmitter : AvfxNode {
        public const string NAME = "Emit";

        public readonly AvfxString Sound = new( "Sound", "SdNm", showRemoveButton: true );
        public readonly AvfxInt SoundNumber = new( "Sound Index (-1 if no sound)", "SdNo" );
        public readonly AvfxInt LoopStart = new( "Loop Start", "LpSt" );
        public readonly AvfxInt LoopEnd = new( "Loop End", "LpEd" );
        public readonly AvfxInt ChildLimit = new( "Child Limit", "ClCn" );
        public readonly AvfxInt EffectorIdx = new( "Effector Select", "EfNo" );
        public readonly AvfxBool AnyDirection = new( "Any Direction", "bAD", size: 1 );
        public readonly AvfxEnum<EmitterType> EmitterVariety = new( "Type", "EVT" );
        public readonly AvfxEnum<RotationDirectionBase> RotationDirectionBaseType = new( "Rotation Direction Base", "RBDT" );
        public readonly AvfxEnum<CoordComputeOrder> CoordComputeOrderType = new( "Coordinate Compute Order", "CCOT" );
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxInt ParticleCount = new( "Particle Count", "PrCn" );
        public readonly AvfxInt EmitterCount = new( "Emitter Count", "EmCn" );
        public readonly AvfxLife Life = new();
        public readonly AvfxCurve CreateCount = new( "Create Count", "CrC", locked: true );
        public readonly AvfxCurve CreateCountRandom = new( "Create Count Random", "CrCR" );
        public readonly AvfxCurve CreateInterval = new( "Create Count Interval", "CrI", locked: true );
        public readonly AvfxCurve CreateIntervalRandom = new( "Create Count Interval Random", "CrIR" );
        public readonly AvfxCurve Gravity = new( "Gravity", "Gra" );
        public readonly AvfxCurve GravityRandom = new( "Gravity Random", "GraR" );
        public readonly AvfxCurve AirResistance = new( "Air Resistance", "ARs", locked: true );
        public readonly AvfxCurve AirResistanceRandom = new( "Air Resistance Random", "ARsR" );
        public readonly AvfxCurveColor Color = new( "Color", locked: true );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos", locked: true );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot", locked: true );
        public readonly AvfxCurve3Axis Scale = new( "Scale", "Scl", locked: true );
        public AvfxData Data;

        private readonly List<AvfxBase> Children;

        public readonly List<AvfxEmitterItem> Particles = new();
        public readonly List<AvfxEmitterItem> Emitters = new();

        private readonly UiNodeGraphView NodeView;
        public readonly UiNodeGroupSet NodeGroups;

        public readonly UiNodeSelect<AvfxEffector> EffectorSelect;

        public readonly List<AvfxItem> Animation;
        public readonly AvfxDisplaySplitView<AvfxItem> AnimationSplit;

        public readonly UiEmitterSplitView EmitterSplit;
        public readonly UiEmitterSplitView ParticleSplit;
        private readonly List<IUiBase> Parameters;

        public AvfxEmitter( UiNodeGroupSet groupSet, bool hasDependencies ) : base( NAME, UiNodeGroup.EmitterColor, hasDependencies ) {
            NodeGroups = groupSet;

            Children = new() {
                Sound,
                SoundNumber,
                LoopStart,
                LoopEnd,
                ChildLimit,
                EffectorIdx,
                AnyDirection,
                EmitterVariety,
                RotationDirectionBaseType,
                CoordComputeOrderType,
                RotationOrderType,
                ParticleCount,
                EmitterCount,
                Life,
                CreateCount,
                CreateCountRandom,
                CreateInterval,
                CreateIntervalRandom,
                Gravity,
                GravityRandom,
                AirResistance,
                AirResistanceRandom,
                Color,
                Position,
                Rotation,
                Scale
            };
            Sound.SetAssigned( false );

            Parameters = new() {
                LoopStart,
                LoopEnd,
                ChildLimit,
                AnyDirection,
                RotationDirectionBaseType,
                CoordComputeOrderType,
                RotationOrderType
            };

            Animation = new() {
                Life,
                CreateCount,
                CreateCountRandom,
                CreateInterval,
                CreateIntervalRandom,
                Gravity,
                GravityRandom,
                AirResistance,
                AirResistanceRandom,
                Color,
                Position,
                Rotation,
                Scale
            };
            AnimationSplit = new( Animation );

            EffectorSelect = new( this, "Effector Select", groupSet.Effectors, EffectorIdx );

            EmitterSplit = new( Emitters, this, false );
            ParticleSplit = new( Particles, this, false );

            EmitterVariety.ExtraCommand = () => {
                return new AvfxEmitterDataExtraCommand( this );
            };

            NodeView = new UiNodeGraphView( this );
            HasDependencies = false; // if imported, all set now
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Children, size );
            var emitterType = EmitterVariety.GetValue();

            AvfxEmitterCreate lastParticle = null;
            AvfxEmitterCreate lastEmitter = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Data" ) {
                    SetData( emitterType );
                    Data?.Read( _reader, _size );
                }
                else if( _name == "ItPr" ) {
                    lastParticle = new AvfxEmitterCreate( "ItPr", true, this );
                    lastParticle.Read( _reader, _size );

                }
                else if( _name == "ItEm" ) {
                    lastEmitter = new AvfxEmitterCreate( "ItEm", false, this );
                    lastEmitter.Read( _reader, _size );

                }
            }, size );

            if( lastParticle != null ) {
                Particles.AddRange( lastParticle.Items );
            }
            if( lastEmitter != null ) {
                var startIndex = Particles.Count;
                var emitterCount = lastEmitter.Items.Count - Particles.Count;
                Emitters.AddRange( lastEmitter.Items.GetRange( startIndex, emitterCount ) ); // remove particles
            }

            EmitterSplit.UpdateIdx();
            ParticleSplit.UpdateIdx();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) {
            RecurseAssigned( Children, assigned );
            RecurseAssigned( Data, assigned );
        }

        protected override void WriteContents( BinaryWriter writer ) {
            EmitterCount.SetValue( Emitters.Count );
            ParticleCount.SetValue( Particles.Count );
            WriteNested( writer, Children );

            // ItPr
            for( var i = 0; i < Particles.Count; i++ ) {
                var ItPr = new AvfxEmitterCreate( "ItPr", true, this );
                ItPr.Items.AddRange( Particles.GetRange( 0, i + 1 ) );
                ItPr.Write( writer );
            }

            // ItEm
            for( var i = 0; i < Emitters.Count; i++ ) {
                var ItEm = new AvfxEmitterCreate( "ItEm", false, this );
                ItEm.Items.AddRange( Particles );
                ItEm.Items.AddRange( Emitters.GetRange( 0, i + 1 ) );
                ItEm.Write( writer );
            }

            Data?.Write( writer );
        }

        public void SetData( EmitterType type ) {
            Data = type switch {
                EmitterType.Point => null,
                EmitterType.Cone => new AvfxEmitterDataCone(),
                EmitterType.ConeModel => new AvfxEmitterDataConeModel(),
                EmitterType.SphereModel => new AvfxEmitterDataSphereModel(),
                EmitterType.CylinderModel => new AvfxEmitterDataCylinderModel(),
                EmitterType.Model => new AvfxEmitterDataModel( this ),
                _ => null,
            };
            Data?.SetAssigned( true );
        }

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            NodeView.Draw( id );
            EffectorSelect.Draw( id );

            Sound.Draw( id );
            SoundNumber.Draw( id );

            IUiBase.DrawList( Parameters, id );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Emitter";
            DrawRename( id );
            EmitterVariety.Draw( id );
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

        public override string GetDefaultText() => $"Emitter {GetIdx()}({EmitterVariety.GetValue()})";

        public override string GetWorkspaceId() => $"Emit{GetIdx()}";

        public override void PopulateWorkspaceMetaChildren( Dictionary<string, string> renameDict ) {
            Emitters.ForEach( item => IUiWorkspaceItem.PopulateWorkspaceMeta( item, renameDict ) );
            Particles.ForEach( item => IUiWorkspaceItem.PopulateWorkspaceMeta( item, renameDict ) );
        }

        public override void ReadWorkspaceMetaChildren( Dictionary<string, string> renameDict ) {
            Emitters.ForEach( item => IUiWorkspaceItem.ReadWorkspaceMeta( item, renameDict ) );
            Particles.ForEach( item => IUiWorkspaceItem.ReadWorkspaceMeta( item, renameDict ) );
        }
    }
}
