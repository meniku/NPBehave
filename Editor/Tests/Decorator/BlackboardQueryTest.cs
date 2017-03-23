using NUnit.Framework;

namespace NPBehave
{
    public class BlackboardQueryTest : Test
    {
        [Test]
        public void ShouldNotThrowErrors_WhenObservingKeysThatDontExist()
        {
            TestRoot behaviorTree = null;
            MockNode child = new MockNode();
            BlackboardQuery sut = new BlackboardQuery(new string[]{"key1", "key2"}, Stops.IMMEDIATE_RESTART, () => {
                object o1 = behaviorTree.Blackboard.Get<float>("key1");
                object o2 = behaviorTree.Blackboard.Get<float>("key2");
                float f1 = (float)o1;
                float f2 = (float)o2;

                if ((f1 > 0.99) && (f2 < 5.99f))
                    return true;
                return false;
            }, child);
            behaviorTree = CreateBehaviorTree(sut);

            behaviorTree.Start();
            Assert.AreEqual(Node.State.INACTIVE, sut.CurrentState);
            Assert.AreEqual(Node.State.INACTIVE, child.CurrentState);
        }
    }
}