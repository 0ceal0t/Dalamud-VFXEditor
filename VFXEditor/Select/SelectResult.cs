using System;

namespace VfxEditor.Select {
    public enum SelectResultType {
        Local,
        GamePath,
        GameItem,
        GameStatus,
        GameAction,
        GameZone,
        GameEmote,
        GameGimmick,
        GameCutscene,
        GameNpc,
        GameMusic,
        GameQuest,
        GameCharacter,
        GameJob,
        GameMisc,
        GameMount,
        GameHousing,
        GameUi
    }

    [Serializable]
    public class SelectResult {
        public SelectResultType Type;
        public string Name;
        public string DisplayString; // [PREFIX] Name
        public string Path;

        public SelectResult( SelectResultType type, string name, string displayString, string path ) {
            Type = type;
            Name = name;
            DisplayString = displayString;
            Path = path;
        }

        public bool CompareTo( SelectResult other ) => Type == other.Type && DisplayString == other.DisplayString && Path == other.Path;
    }
}