using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AvfxFormat.Nodes;
using VfxEditor.Ui.Interfaces;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitter : AvfxNode {
        public const string NAME = "Emit";

        public readonly AvfxString Sound = new( "Sound", "SdNm", showRemoveButton: true );
        public readonly AvfxInt SoundNumber = new( "Sound Index (-1 if no sound)", "SdNo" );
        public readonly AvfxInt LoopStart = new( "Loop Start", "LpSt" );
        public readonly AvfxInt LoopEnd = new( "Loop End", "LpEd" );
        public readonly AvfxInt ChildLimit = new( "Child Limit", "ClCn" );
        public readonly AvfxInt EffectorIdx = new( "Effector Select", "EfNo", defaultValue: -1 );
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
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot", CurveType.Angle, locked: true );
        public readonly AvfxCurve3Axis Scale = new( "Scale", "Scl", locked: true );
        public readonly AvfxCurve InjectionAngleX = new( "Injection Angle X", "IAX", CurveType.Angle );
        public readonly AvfxCurve InjectionAngleRandomX = new( "Injection Angle X Random", "IAXR", CurveType.Angle );
        public readonly AvfxCurve InjectionAngleY = new( "Injection Angle Y", "IAY", CurveType.Angle );
        public readonly AvfxCurve InjectionAngleRandomY = new( "Injection Angle Y Random", "IAYR", CurveType.Angle );
        public readonly AvfxCurve InjectionAngleZ = new( "Injection Angle Z", "IAX", CurveType.Angle );
        public readonly AvfxCurve InjectionAngleRandomZ = new( "Injection Angle Z Random", "IAZR", CurveType.Angle );
        public readonly AvfxCurve VelocityRandomZ = new( "Velocity Random Z", "VRZ" );
        public AvfxData Data;

        private readonly List<AvfxBase> Parsed;

        public readonly List<AvfxEmitterItem> Particles = new();
        public readonly List<AvfxEmitterItem> Emitters = new();

        private readonly UiNodeGraphView NodeView;
        public readonly AvfxNodeGroupSet NodeGroups;

        public readonly UiNodeSelect<AvfxEffector> EffectorSelect;

        public readonly AvfxDisplaySplitView<AvfxItem> AnimationSplitDisplay;

        public readonly UiEmitterSplitView EmitterSplit;
        public readonly UiEmitterSplitView ParticleSplit;
        private readonly List<IUiItem> Display;

        public AvfxEmitter( AvfxNodeGroupSet groupSet ) : base( NAME, AvfxNodeGroupSet.EmitterColor ) {
            NodeGroups = groupSet;

            Parsed = new() {
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
                Scale,
                InjectionAngleX,
                InjectionAngleRandomX,
                InjectionAngleY,
                InjectionAngleRandomY,
                InjectionAngleZ,
                InjectionAngleRandomZ,
                VelocityRandomZ
            };
            Sound.SetAssigned( false );

            Display = new() {
                LoopStart,
                LoopEnd,
                ChildLimit,
                AnyDirection,
                RotationDirectionBaseType,
                CoordComputeOrderType,
                RotationOrderType
            };

            AnimationSplitDisplay = new( new() {
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
                Scale,
                InjectionAngleX,
                InjectionAngleRandomX,
                InjectionAngleY,
                InjectionAngleRandomY,
                InjectionAngleZ,
                InjectionAngleRandomZ,
                VelocityRandomZ
            } );

            EffectorSelect = new( this, "Effector Select", groupSet.Effectors, EffectorIdx );

            EmitterSplit = new( Emitters, this, false );
            ParticleSplit = new( Particles, this, true );

            EmitterVariety.Parsed.ExtraCommandGenerator = () => {
                return new AvfxEmitterDataCommand( this );
            };

            NodeView = new UiNodeGraphView( this );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Parsed, size );
            var emitterType = EmitterVariety.GetValue();

            AvfxEmitterItemContainer lastParticle = null;
            AvfxEmitterItemContainer lastEmitter = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Data" ) {
                    SetData( emitterType );
                    Data?.Read( _reader, _size );
                }
                else if( _name == "ItPr" ) {
                    lastParticle = new AvfxEmitterItemContainer( "ItPr", true, this );
                    lastParticle.Read( _reader, _size );

                }
                else if( _name == "ItEm" ) {
                    lastEmitter = new AvfxEmitterItemContainer( "ItEm", false, this );
                    lastEmitter.Read( _reader, _size );

                }
            }, size );

            if( lastParticle != null ) {
                Particles.AddRange( lastParticle.Items );
                Particles.ForEach( x => x.InitializeNodeSelects() );
            }

            if( lastEmitter != null ) {
                var startIndex = Particles.Count;
                var emitterCount = lastEmitter.Items.Count - Particles.Count;
                Emitters.AddRange( lastEmitter.Items.GetRange( startIndex, emitterCount ) ); // remove particles
                Emitters.ForEach( x => x.InitializeNodeSelects() );
            }

            EmitterSplit.UpdateIdx();
            ParticleSplit.UpdateIdx();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) {
            RecurseAssigned( Parsed, assigned );
            RecurseAssigned( Data, assigned );
        }

        protected override void WriteContents( BinaryWriter writer ) {
            EmitterCount.SetValue( Emitters.Count );
            ParticleCount.SetValue( Particles.Count );
            WriteNested( writer, Parsed );

            // ItPr
            for( var i = 0; i < Particles.Count; i++ ) {
                var ItPr = new AvfxEmitterItemContainer( "ItPr", true, this );
                ItPr.Items.AddRange( Particles.GetRange( 0, i + 1 ) );
                ItPr.Write( writer );
            }

            // ItEm
            for( var i = 0; i < Emitters.Count; i++ ) {
                var ItEm = new AvfxEmitterItemContainer( "ItEm", false, this );
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

        public override void Draw( string parentId ) {
            var id = parentId + "/Emitter";
            DrawRename( id );
            EmitterVariety.Draw( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( $"{id}/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters( $"{id}/Params" );
            DrawData( $"{id}/Data" );
            DrawAnimation( $"{id}/Animation" );
            DrawParticles( $"{id}/ItPr" );
            DrawEmitters( $"{id}/ItEm" );
        }

        private void DrawParameters( string id ) {
            using var tabItem = ImRaii.TabItem( $"Parameters{id}/Tab" );
            if( !tabItem ) return;

            using var child = ImRaii.Child( id );
            NodeView.Draw( id );
            EffectorSelect.Draw( id );
            Sound.Draw( id );
            SoundNumber.Draw( id );
            DrawItems( Display, id );
        }

        private void DrawData( string id ) {
            if( Data == null ) return;

            using var tabItem = ImRaii.TabItem( $"Data{id}/Tab" );
            if( !tabItem ) return;

            using var child = ImRaii.Child( id );
            Data.Draw( id );
        }

        private void DrawAnimation( string id ) {
            using var tabItem = ImRaii.TabItem( $"Animation{id}/Tab" );
            if( !tabItem ) return;

            AnimationSplitDisplay.Draw( id );
        }

        private void DrawParticles( string id ) {
            using var tabItem = ImRaii.TabItem( $"Create Particles{id}/Tab" );
            if( !tabItem ) return;

            ParticleSplit.Draw( id );
        }

        private void DrawEmitters( string id ) {
            using var tabItem = ImRaii.TabItem( $"Create Emitters{id}/Tab" );
            if( !tabItem ) return;

            EmitterSplit.Draw( id );
        }

        public override string GetDefaultText() => $"Emitter {GetIdx()}({EmitterVariety.GetValue()})";

        public override string GetWorkspaceId() => $"Emit{GetIdx()}";

        public override void GetChildrenRename( Dictionary<string, string> renameDict ) {
            Emitters.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, renameDict ) );
            Particles.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, renameDict ) );
        }

        public override void SetChildrenRename( Dictionary<string, string> renameDict ) {
            Emitters.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, renameDict ) );
            Particles.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, renameDict ) );
        }

        public bool HasSound => Sound.IsAssigned() && SoundNumber.GetValue() > 0 && Sound.GetValue().Trim( '\0' ).Length > 0;
    }
}
