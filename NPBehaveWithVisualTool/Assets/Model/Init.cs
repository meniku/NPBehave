
using NETCoreTest.Framework;
using UnityEngine;

namespace ETModel
{
    public class Init : MonoBehaviour
    {
        public bool isEditorMode = false;

        private FixedUpdate fixedUpdate;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

            fixedUpdate = new FixedUpdate() {UpdateCallback = () => Game.EventSystem.FixedUpdate()};


            Game.Scene.AddComponent<UnitComponent>();

            Game.Scene.AddComponent<NP_SyncComponent>();
            Game.Scene.AddComponent<NP_TreeDataRepository>();

            NP_RuntimeTree npRuntimeTree = NP_RuntimeTreeFactory.CreateNpRuntimeTree(UnitFactory.NPBehaveTestCreate(),
                NP_Client_TreeIds.Darius_Q_Client);
            Log.Info("行为树创建完成");
            npRuntimeTree.m_NPRuntimeTreeRootNode.Start();
        }

        private void Update()
        {
            Game.EventSystem.Update();
        }

        private void FixedUpdate()
        {
            fixedUpdate.Tick();
        }

        private void OnApplicationQuit()
        {
            Game.Close();
        }
    }
}