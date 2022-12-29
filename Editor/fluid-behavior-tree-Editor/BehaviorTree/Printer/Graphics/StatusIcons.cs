using System;
using NPBehave;
using UnityEngine;

namespace CleverCrow.Fluid.BTs.Trees.Editors {
    public class StatusIcons {
        private const string ICON_STATUS_PATH = "ROOT/Icons/Status";

        private TextureLoader Success { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Success.png");
        private TextureLoader Failure { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Failure.png");
        private TextureLoader Continue { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Continue.png");

        public TextureLoader GetIcon (Node  node)
        {
            switch (node.CurrentState)
            {
                case Node.State.INACTIVE:
                        return node.DebugLastResult ? Success : Failure;
                case Node.State.ACTIVE:
                case Node.State.STOP_REQUESTED:
                    return Continue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
