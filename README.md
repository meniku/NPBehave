[//]: # "******************************************************************************"
[//]: # "THIS DOCUMENTATION IS BEST VIEWED ONLINE AT https://github.com/meniku/NPBehave"
[//]: # "******************************************************************************"

# NPBehave - An event driven Behavior Tree Library for code based AIs in Unity

![NPBehave Logo](http://labs.nkuebler.de/npbehave/images/np-behave.png)

NPBehave aims to be:

- lightweight, fast & simple
- event driven
- easily extendable
- A framework to define AIs with code, there is no visual editing support

NPBehave builds on the powerful and flexible code based approach to define behavior trees from the [BehaviorLibrary](https://github.com/DeveloperUX/BehaviorLibrary) and mixes in some of the great concepts of [Unreal's behavior trees](https://docs.unrealengine.com/latest/INT/Engine/AI/BehaviorTrees/QuickStart/). Unlike traditional behavior trees, event driven behavior trees do not need to be traversed from the root node again each frame. They stay in their current state and only continue to traverse when they actually need to. This makes them more performant and a lot simpler to use.

In NPBehave you will find most node types from traditional behavior trees, but also some similar to those found in the Unreal engine. Please refer to the [Node Type Reference](#node-type-reference) for the full list. It's fairly easy to add your own custom node types though.

If you don't know anything about behavior trees, it's highly recommended that you gain some theory first, [this Gamasutra article](http://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php) is a good read.

