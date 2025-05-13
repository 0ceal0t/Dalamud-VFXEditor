using System.Collections.Generic;
using VfxEditor.PhybFormat.Simulator.Attract;
using VfxEditor.PhybFormat.Simulator.Chain;
using VfxEditor.PhybFormat.Simulator.CollisionData;
using VfxEditor.PhybFormat.Simulator.Connector;
using VfxEditor.PhybFormat.Simulator.Pin;
using VfxEditor.PhybFormat.Simulator.PostAlignment;
using VfxEditor.PhybFormat.Simulator.Spring;

namespace VfxEditor.PhybFormat.Simulator {
    public static class PhybSimulatorClipboard {
        private static PhybSimulator? CopiedSimulator;
        private static int CopiedSimulatorIndex = -1;

        public static void CopySimulator(PhybSimulator source) {
            // Store the simulator index
            var simulators = source.File.Simulation.Simulators;
            CopiedSimulatorIndex = simulators.IndexOf(source);
            CopiedSimulator = new PhybSimulator(source.File);

            // Copy parameters
            CopiedSimulator.Params.CopyFrom(source.Params);
            
            // Deep copy all collections
            foreach (var item in source.Collisions) CopiedSimulator.Collisions.Add(item.Clone(CopiedSimulator.File, CopiedSimulator));
            foreach (var item in source.CollisionConnectors) CopiedSimulator.CollisionConnectors.Add(item.Clone(CopiedSimulator.File, CopiedSimulator));
            foreach (var item in source.Chains) CopiedSimulator.Chains.Add(item.Clone(CopiedSimulator.File, CopiedSimulator));
            foreach (var item in source.Connectors) CopiedSimulator.Connectors.Add(item.Clone(CopiedSimulator.File, CopiedSimulator));
            foreach (var item in source.Attracts) CopiedSimulator.Attracts.Add(item.Clone(CopiedSimulator.File, CopiedSimulator));
            foreach (var item in source.Pins) CopiedSimulator.Pins.Add(item.Clone(CopiedSimulator.File, CopiedSimulator));
            foreach (var item in source.Springs) CopiedSimulator.Springs.Add(item.Clone(CopiedSimulator.File, CopiedSimulator));
            foreach (var item in source.PostAlignments) CopiedSimulator.PostAlignments.Add(item.Clone(CopiedSimulator.File, CopiedSimulator));

            Dalamud.InfoNotification( $"Copied Simulator {CopiedSimulatorIndex}" );
        }

        public static void PasteSimulator(PhybSimulator target) {
            if (CopiedSimulator == null) return;

            // Copy parameters
            target.Params.CopyFrom(CopiedSimulator.Params);

            // Clear existing collections
            target.Collisions.Clear();
            target.CollisionConnectors.Clear();
            target.Chains.Clear();
            target.Connectors.Clear();
            target.Attracts.Clear();
            target.Pins.Clear();
            target.Springs.Clear();
            target.PostAlignments.Clear();

            // Deep copy from clipboard to target
            foreach (var item in CopiedSimulator.Collisions) target.Collisions.Add(item.Clone(target.File, target));
            foreach (var item in CopiedSimulator.CollisionConnectors) target.CollisionConnectors.Add(item.Clone(target.File, target));
            foreach (var item in CopiedSimulator.Chains) target.Chains.Add(item.Clone(target.File, target));
            foreach (var item in CopiedSimulator.Connectors) target.Connectors.Add(item.Clone(target.File, target));
            foreach (var item in CopiedSimulator.Attracts) target.Attracts.Add(item.Clone(target.File, target));
            foreach (var item in CopiedSimulator.Pins) target.Pins.Add(item.Clone(target.File, target));
            foreach (var item in CopiedSimulator.Springs) target.Springs.Add(item.Clone(target.File, target));
            foreach (var item in CopiedSimulator.PostAlignments) target.PostAlignments.Add(item.Clone(target.File, target));

            target.File.OnChange();
            var target_simulator_index = target.File.Simulation.Simulators.IndexOf(target);
            Dalamud.OkNotification( $"Pasted to Simulator {target_simulator_index}" );
        }

        public static void Clear() {
            if (CopiedSimulator == null) return;
            CopiedSimulator = null;
        }

        public static bool HasCopiedData() => CopiedSimulator != null;
    }
}
