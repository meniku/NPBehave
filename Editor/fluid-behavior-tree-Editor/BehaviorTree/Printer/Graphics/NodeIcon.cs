//Create: Icarus
//ヾ(•ω•`)o
//2020-08-08 09:10
//Fluid.BehaviorTree.Editor

using System;
using NPBehave;
using Action = NPBehave.Action;

namespace CleverCrow.Fluid.BTs.Trees.Editors
{
    public static class NodeIcon
    {
        private const string ICON_STATUS_PATH = "ROOT/Icons/Tasks";

        // private TextureLoader Cancel  { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Success.png");
        //
        // private TextureLoader Checkmark  { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Failure.png");
        //
        // private TextureLoader Continue { get; } = new TextureLoader($"{ICON_STATUS_PATH}/Continue.png");
        //
        // public TextureLoader GetIcon (Node node)
        // {
        //     if (node.CurrentState == Node.State.INACTIVE)
        //     {
        //         if (node.DebugLastResult)
        //         {
        //             return Success;
        //         }
        //
        //         return Failure;
        //     }
        //
        //     return Continue;
        // }

        public static TextureLoader GetIcon(this  Node node)
        {
            string name;
            switch (node)
            {
                case Root _:
                    name = "DownArrow";
                    break;
                case Sequence _:
                    name = "RightArrow";
                    break;
                case Selector _:
                case RandomSelector _:
                case RandomSequence _:
                    name = "Question";
                    break;
                case Condition _:
                    name = "CompareArrows";
                    break;
                case Wait _:
                    name = "Hourglass";
                    break;
                case Parallel _:
                    name = "LinearScale";
                    break;
                case Inverter _:
                    name = "Invert";
                    break;
                case Repeater _:
                    name = "Repeat";
                    break;
                case Action _:
                    name = "Play";
                    break;
                case Failer f:
                    name = "Cancel";
                    if (f.ParentNode is Repeater)
                    {
                        name = "EventBusy";
                    }
                    break;
                case Succeeder s:
                    name = "Checkmark";
                    if (s.ParentNode is Repeater)
                    {
                        name = "EventAvailable";
                    }
                    break;
                case Container _:
                    name = "DownArrow";
                    break;
                default:
                    throw new NotSupportedException(node.GetType().FullName);
            }
            
            return new TextureLoader($"{ICON_STATUS_PATH}/{name}.png");
        }
    }
}