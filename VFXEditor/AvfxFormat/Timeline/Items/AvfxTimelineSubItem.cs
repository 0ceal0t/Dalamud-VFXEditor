using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat {
    public class AvfxTimelineSubItem : GenericWorkspaceItem {
        public readonly AvfxTimeline Timeline;

        public readonly AvfxBool Enabled = new( "Enabled", "bEna" );
        public readonly AvfxInt StartTime = new( "Start Time", "StTm" );
        public readonly AvfxInt EndTime = new( "End Time", "EdTm" );
        public readonly AvfxInt BinderIdx = new( "Binder Index", "BdNo" );
        public readonly AvfxInt EffectorIdx = new( "Effector Index", "EfNo" );
        public readonly AvfxInt EmitterIdx = new( "Emitter Index", "EmNo" );
        public readonly AvfxInt Platform = new( "Platform", "Plfm" );
        public readonly AvfxInt ClipNumber = new( "Clip Index", "ClNo" );

        private readonly List<AvfxBase> Parsed;

        public UiNodeSelect<AvfxBinder> BinderSelect;
        public UiNodeSelect<AvfxEmitter> EmitterSelect;
        public UiNodeSelect<AvfxEffector> EffectorSelect;

        private readonly List<IAvfxUiBase> Display;

        public AvfxTimelineSubItem( AvfxTimeline timeline, bool initNodeSelects ) {
            Timeline = timeline;

            Parsed = new List<AvfxBase> {
                Enabled,
                StartTime,
                EndTime,
                BinderIdx,
                EffectorIdx,
                EmitterIdx,
                Platform,
                ClipNumber
            };
            AvfxBase.RecurseAssigned( Parsed, false );

            if( initNodeSelects ) InitializeNodeSelects();

            Display = new() {
                Enabled,
                StartTime,
                EndTime,
                Platform
            };
        }

        public AvfxTimelineSubItem( AvfxTimeline timeline, bool initNodeSelects, byte[] data ) : this( timeline, initNodeSelects ) {
            using var buffer = new MemoryStream( data );
            using var reader = new BinaryReader( buffer );
            AvfxBase.ReadNested( reader, Parsed, data.Length );
        }

        public void InitializeNodeSelects() {
            BinderSelect = new UiNodeSelect<AvfxBinder>( Timeline, "Target Binder", Timeline.NodeGroups.Binders, BinderIdx );
            EmitterSelect = new UiNodeSelect<AvfxEmitter>( Timeline, "Target Emitter", Timeline.NodeGroups.Emitters, EmitterIdx );
            EffectorSelect = new UiNodeSelect<AvfxEffector>( Timeline, "Target Effector", Timeline.NodeGroups.Effectors, EffectorIdx );
        }

        public void Write( BinaryWriter writer ) => AvfxBase.WriteNested( writer, Parsed );

        public override void Draw( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );

            BinderSelect.Draw( id );
            EmitterSelect.Draw( id );
            EffectorSelect.Draw( id );
            IAvfxUiBase.DrawList( Display, id );

            var clipAssigned = ClipNumber.IsAssigned();
            if( ImGui.Checkbox( "Clip Enabled" + id, ref clipAssigned ) ) CommandManager.Avfx.Add( new AvfxAssignCommand( ClipNumber, clipAssigned ) );
            ClipNumber.Draw( id );
        }

        public override string GetDefaultText() => $"{GetIdx()}: Emitter {EmitterIdx.GetValue()}";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Item{GetIdx()}";
    }
}
