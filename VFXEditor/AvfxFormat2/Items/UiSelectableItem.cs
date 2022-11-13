using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public interface IUiSelectableItem : IUiItem {
        public int GetIdx();
        public void SetIdx(int idx);
    }
}
