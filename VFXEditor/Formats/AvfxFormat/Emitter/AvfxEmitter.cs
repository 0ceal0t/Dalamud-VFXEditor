using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Nodes;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;
using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEmitter : AvfxNodeWithData<EmitterType> {
        public const string NAME = "Emit";

        public readonly AvfxString Sound = new( "Sound", "SdNm", true, false, null, true );
        public readonly AvfxInt SoundNumber = new( "Sound Index", "SdNo" );
        public readonly AvfxInt LoopStart = new( "Loop Start", "LpSt" );
        public readonly AvfxInt LoopEnd = new( "Loop End", "LpEd" );
        public readonly AvfxInt ChildLimit = new( "Child Limit", "ClCn" );
        public readonly AvfxInt EffectorIdx = new( "Effector Select", "EfNo", value: -1 );
        public readonly AvfxBool AnyDirection = new( "Any Direction", "bAD", size: 1 );
        public readonly AvfxEnum<RotationDirectionBase> RotationDirectionBaseType = new( "Rotation Direction Base", "RBDT" );
        public readonly AvfxEnum<CoordComputeOrder> CoordComputeOrderType = new( "Coordinate Compute Order", "CCOT" );
        public readonly AvfxEnum<RotationOrder> RotationOrderType = new( "Rotation Order", "ROT" );
        public readonly AvfxInt ParticleCount = new( "Particle Count", "PrCn" );
        public readonly AvfxInt EmitterCount = new( "Emitter Count", "EmCn" );
        public readonly AvfxLife Life = new();
        public readonly AvfxCurve1Axis CreateCount = new( "Create Count", "CrC", locked: true );
        public readonly AvfxCurve1Axis CreateCountRandom = new( "Create Count Random", "CrCR" );
        public readonly AvfxCurve1Axis CreateInterval = new( "Create Count Interval", "CrI", locked: true );
        public readonly AvfxCurve1Axis CreateIntervalRandom = new( "Create Count Interval Random", "CrIR" );
        public readonly AvfxCurve1Axis Gravity = new( "Gravity", "Gra" );
        public readonly AvfxCurve1Axis GravityRandom = new( "Gravity Random", "GraR" );
        public readonly AvfxCurve1Axis AirResistance = new( "Air Resistance", "ARs", locked: true );
        public readonly AvfxCurve1Axis AirResistanceRandom = new( "Air Resistance Random", "ARsR" );
        public readonly AvfxCurveColor Color = new( "Color", locked: true );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos", locked: true );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot", CurveType.Angle, locked: true );
        public readonly AvfxCurve3Axis Scale = new( "Scale", "Scl", locked: true );

        public readonly AvfxCurve1Axis InjectionAngleX = new( "Injection Angle X", "IAX", CurveType.Angle );
        public readonly AvfxCurve1Axis InjectionAngleRandomX = new( "Injection Angle X Random", "IAXR", CurveType.Angle );

        public readonly AvfxCurve1Axis InjectionAngleY = new( "Injection Angle Y", "IAY", CurveType.Angle );
        public readonly AvfxCurve1Axis InjectionAngleRandomY = new( "Injection Angle Y Random", "IAYR", CurveType.Angle );

        public readonly AvfxCurve1Axis InjectionAngleZ = new( "Injection Angle Z", "IAZ", CurveType.Angle );
        public readonly AvfxCurve1Axis InjectionAngleRandomZ = new( "Injection Angle Z Random", "IAZR", CurveType.Angle );

        public readonly AvfxCurve1Axis VelocityRandomX = new( "Velocity Random X", "VRX" );
        public readonly AvfxCurve1Axis VelocityRandomY = new( "Velocity Random Y", "VRY" );
        public readonly AvfxCurve1Axis VelocityRandomZ = new( "Velocity Random Z", "VRZ" );

        private readonly List<AvfxBase> Parsed;

        public readonly List<AvfxEmitterItem> Particles = [];
        public readonly List<AvfxEmitterItem> Emitters = [];

        private readonly UiNodeGraphView NodeView;
        public readonly AvfxNodeGroupSet NodeGroups;

        public readonly AvfxNodeSelect<AvfxEffector> EffectorSelect;

        public readonly AvfxDisplaySplitView<AvfxItem> AnimationSplitDisplay;

        public readonly UiEmitterSplitView EmitterSplit;
        public readonly UiEmitterSplitView ParticleSplit;
        private readonly List<IUiItem> Parameters;

        public AvfxEmitter( AvfxNodeGroupSet groupSet ) : base( NAME, AvfxNodeGroupSet.EmitterColor, "EVT" ) {
            NodeGroups = groupSet;

            Parsed = [
                Sound,
                SoundNumber,
                LoopStart,
                LoopEnd,
                ChildLimit,
                EffectorIdx,
                AnyDirection,
                Type,
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
                VelocityRandomX,
                VelocityRandomY,
                VelocityRandomZ
            ];
            Sound.SetAssigned( false );

            AnimationSplitDisplay = new( "Animation", [
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
                VelocityRandomX,
                VelocityRandomY,
                VelocityRandomZ
            ] );

            EffectorSelect = new( this, "Effector Select", groupSet.Effectors, EffectorIdx );
            EmitterSplit = new( "Create Emitters", Emitters, this, false );
            ParticleSplit = new( "Create Particles", Particles, this, true );

            NodeView = new UiNodeGraphView( this );

            Parameters = [
                LoopStart,
                LoopEnd,
                ChildLimit,
                AnyDirection,
                RotationDirectionBaseType,
                CoordComputeOrderType,
                RotationOrderType
            ];

            Sound.Parsed.Icons.Insert( 0, new() {
                Icon = () => FontAwesomeIcon.VolumeUp,
                Remove = false,
                Action = ( string value ) => Plugin.ResourceLoader.PlaySound( value, SoundNumber.Value )
            } );
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Peek( reader, Parsed, size );

            AvfxEmitterItemContainer lastParticle = null;
            AvfxEmitterItemContainer lastEmitter = null;

            ReadNested( reader, ( BinaryReader _reader, string _name, int _size ) => {
                if( _name == "Data" ) {
                    UpdateData();
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

        public override void WriteContents( BinaryWriter writer ) {
            EmitterCount.Value = Emitters.Count;
            ParticleCount.Value = Particles.Count;
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

        protected override IEnumerable<AvfxBase> GetChildren() {
            foreach( var item in Parsed ) yield return item;
            if( Data != null ) yield return Data;
        }

        public override void UpdateData() {
            Data = Type.Value switch {
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

        public override void Draw() {
            using var _ = ImRaii.PushId( "Emitter" );
            DrawRename();
            Type.Draw();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawParameters();
            DrawData();

            using( var tab = ImRaii.TabItem( "Animation" ) ) {
                if( tab ) AnimationSplitDisplay.Draw();
            }

            using( var tab = ImRaii.TabItem( "Create Particles" ) ) {
                if( tab ) ParticleSplit.Draw();
            }

            using( var tab = ImRaii.TabItem( "Create Emitters" ) ) {
                if( tab ) EmitterSplit.Draw();
            }
        }

        private void DrawParameters() {
            using var tabItem = ImRaii.TabItem( "Parameters" );
            if( !tabItem ) return;

            using var _ = ImRaii.PushId( "Parameters" );
            using var child = ImRaii.Child( "Child" );

            NodeView.Draw();
            EffectorSelect.Draw();

            Sound.Draw();
            SoundNumber.Draw();
            ImGui.SameLine();
            UiUtils.HelpMarker( "-1 if no sound" );

            DrawItems( Parameters );
        }

        private void DrawData() {
            if( Data == null ) return;

            using var tabItem = ImRaii.TabItem( "Data" );
            if( !tabItem ) return;

            Data.Draw();
        }

        public override string GetDefaultText() => $"Emitter {GetIdx()} ({Type.Value})";

        public override string GetWorkspaceId() => $"Emit{GetIdx()}";

        public override void GetChildrenRename( Dictionary<string, string> renameDict ) {
            Emitters.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, renameDict ) );
            Particles.ForEach( item => IWorkspaceUiItem.GetRenamingMap( item, renameDict ) );
        }

        public override void SetChildrenRename( Dictionary<string, string> renameDict ) {
            Emitters.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, renameDict ) );
            Particles.ForEach( item => IWorkspaceUiItem.ReadRenamingMap( item, renameDict ) );
        }
    }
}
