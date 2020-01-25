### 介绍
什么是NPBehave？[https://www.lfzxb.top/npbehave_doc/](https://www.lfzxb.top/npbehave_doc/)
### 使用到的库
[ET](https://github.com/egametang/ET?tdsourcetag=s_pcqq_aiomsg "ET")：一个基于C#的游戏客户端，服务端框架！
[Node_Editor_Framework](https://github.com/Seneral/Node_Editor_Framework "Node_Editor_Framework")：一个强大的可视化工具！
[Odin](https://odininspector.com "Odin")：Unity编辑器拓展神器！
### 编辑器截图
这是一个官方例子的可视化版本
![这是一个官方例子的可视化版本](https://github.com/wqaetly/Visual_Tool_For_NPBehave/blob/dev_plan_to_visual/pictures/QQ截图20200125144426.png)
支持导出配置文件，供客户端或者服务端读取
![支持导出配置文件，供客户端或者服务端读取](https://github.com/wqaetly/Visual_Tool_For_NPBehave/blob/dev_plan_to_visual/pictures/QQ截图20200125144610.png)
运行结果与官方示例一致
![运行结果与官方示例一致](https://github.com/wqaetly/Visual_Tool_For_NPBehave/blob/dev_plan_to_visual/pictures/QQ截图20200125144651.png)
### 使用方法
1.在Unity编辑器的菜单栏，选择`Tools/其他实用工具/多功能可视化编辑器`即可进入编辑界面
2.然后这样可以创建一个Canvas
![这样可以创建一个Canvas](https://github.com/wqaetly/Visual_Tool_For_NPBehave/blob/dev_plan_to_visual/pictures/QQ截图20200125150013.png)
3.随便找个空地进行右击
![这样就可以选择自己想要创建的数据结点了](https://github.com/wqaetly/Visual_Tool_For_NPBehave/blob/dev_plan_to_visual/pictures/QQ截图20200125150121.png)
4.鼠标左键点击某一个数据结点即可在Inspector面板显示其包含的数据
![鼠标左键点击某一个数据结点即可在Inspector面板显示其包含的数据](https://github.com/wqaetly/Visual_Tool_For_NPBehave/blob/dev_plan_to_visual/pictures/QQ截图20200125150230.png)
5.然后按照自己想要的结果把他们连接起来
6.鼠标左键点击一个空地，即可调出导出配置界面，然后即可进行导出工作。
7.最后在代码里创建自己想要的行为树，Start即可，其中的ID即为我们导出配置时上面显示的根节点ID！
```csharp
NP_RuntimeTree npRuntimeTree = NP_RuntimeTreeFactory.CreateNpRuntimeTree(UnitFactory.NPBehaveTestCreate(), 103542430171146);
npRuntimeTree.m_NPRuntimeTreeRootNode.Start();
```
### 他能用来做什么
我们知道，游戏中有几个比较重要且困难的模块，譬如怪物AI，技能系统，战斗系统这些非常复杂的模块。
他们的复杂体现在如下几个方面：`数据的配置与管理`，`逻辑的架构`，`修改与维护`。
这三点结合起来就会使游戏的开发工作变得非常复杂，那么有没有一种三位一体的解决方案呢？
答案是肯定的，那就是这个逻辑+数据共同编辑的可视化方案。
它不依赖于Unity所以可以导出配置到服务端来执行逻辑。
总体来说就是Unity编辑器内通过连接已经写好的功能结点来配置逻辑，然后导出为配置文件，供客户端或者服务端读取并执行。
虽然我现在的这个工作流可能还不够成熟和完善，但是已经可以用来做一些东西了。
比如我用它开发的Moba项目：[https://gitee.com/NKG_admin/MKGMobaBasedOnET](https://gitee.com/NKG_admin/MKGMobaBasedOnET "https://gitee.com/NKG_admin/MKGMobaBasedOnET")
后面准备再完善一下单独出一篇文章来讲讲基于双端行为树的技能系统。
### 那么哪里能下载到呢？
[GitHub](https://github.com/wqaetly/Visual_Tool_For_NPBehave "https://github.com/wqaetly/Visual_Tool_For_NPBehave")
[国内码云](https://gitee.com/NKG_admin/Visual_Tool_For_NPBehave "国内码云")
