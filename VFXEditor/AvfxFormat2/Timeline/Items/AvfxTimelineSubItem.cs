using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
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

        private readonly List<AvfxBase> Children;

        public readonly UiNodeSelect<AvfxBinder> BinderSelect;
        public readonly UiNodeSelect<AvfxEmitter> EmitterSelect;
        public readonly UiNodeSelect<AvfxEffector> EffectorSelect;

        private readonly List<IUiBase> Parameters;

        public AvfxTimelineSubItem( AvfxTimeline timeline ) {
            Timeline = timeline;

            Children = new List<AvfxBase> {
                Enabled,
                StartTime,
                EndTime,
                BinderIdx,
                EffectorIdx,
                EmitterIdx,
                Platform,
                ClipNumber
            };
            AvfxBase.RecurseAssigned( Children, false );

            BinderSelect = new UiNodeSelect<AvfxBinder>( timeline, "Target Binder", Timeline.NodeGroups.Binders, BinderIdx );
            EmitterSelect = new UiNodeSelect<AvfxEmitter>( timeline, "Target Emitter", Timeline.NodeGroups.Emitters, EmitterIdx );
            EffectorSelect = new UiNodeSelect<AvfxEffector>( timeline, "Target Effector", Timeline.NodeGroups.Effectors, EffectorIdx );

            Parameters = new() {
                Enabled,
                StartTime,
                EndTime,
                Platform
            };
        }

        public AvfxTimelineSubItem( AvfxTimeline timeline, byte[] data ) : this( timeline ) {
            using var buffer = new MemoryStream( data );
            using var reader = new BinaryReader( buffer );
            AvfxBase.ReadNested( reader, Children, data.Length );
        }

        public void Write( BinaryWriter writer ) {
            AvfxBase.WriteNested( writer, Children );
        }

        public override void Draw( string parentId ) {
            var id = parentId + "/Item";
            DrawRename( id );

            BinderSelect.Draw( id );
            EmitterSelect.Draw( id );
            EffectorSelect.Draw( id );
            IUiBase.DrawList( Parameters, id );

            var clipAssigned = ClipNumber.IsAssigned();
            if( ImGui.Checkbox( "Clip Enabled" + id, ref clipAssigned ) ) CommandManager.Avfx.Add( new UiAssignableCommand( ClipNumber, clipAssigned ) );
            ClipNumber.Draw( id );
        }

        public override string GetDefaultText() => $"{GetIdx()}: Emitter {EmitterIdx.GetValue()}";

        public override string GetWorkspaceId() => $"{Timeline.GetWorkspaceId()}/Item{GetIdx()}";
    }
}
