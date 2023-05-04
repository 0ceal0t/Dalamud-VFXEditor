using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Select.Pap.Action;
using VfxEditor.Select.Pap.Emote;
using VfxEditor.Select.Pap.IdlePose;
using VfxEditor.Select.Pap.Job;
using VfxEditor.Select.Pap.Mount;
using VfxEditor.Select.Pap.Npc;

namespace VfxEditor.Select.Pap {
    public struct PoseData {
        public int PoseIndex;
        public string Start;
        public string Loop;

        public bool HasData =>
            !string.IsNullOrEmpty(Start) ||
            !string.IsNullOrEmpty(Loop);
    }

    public struct GeneralData {
        public string IdlePath;
        public string MovePathA;
        public string MovePathB;
        public string DrawWeaponPath;

        public bool HasData => 
            !string.IsNullOrEmpty(IdlePath) ||
            !string.IsNullOrEmpty(MovePathA) ||
            !string.IsNullOrEmpty(MovePathB) ||
            !string.IsNullOrEmpty(DrawWeaponPath);
    }

    public class PapSelectDialog : SelectDialog {
        private readonly List<SelectTab> GameTabs;

        public PapSelectDialog( string id, FileManagerWindow manager, bool isSourceDialog ) : base( id, "pap", manager, isSourceDialog ) {

            GameTabs = new List<SelectTab>( new SelectTab[]{
                new ActionTab( this, "Action" ),
                new NonPlayerActionTab( this, "Non-Player Action" ),
                new EmoteTab( this, "Emote" ),
                new NpcPapTab( this, "Npc" ),
                new CharacterPapTab( this, "Character" ),
                new JobTab( this, "Job" ),
                new MountTab( this, "Mount" )
            } );
        }

        protected override List<SelectTab> GetTabs() => GameTabs;
    }
}
