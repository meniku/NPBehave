using NUnit.Framework;
namespace NPBehave
{

    public class BlackboardTest
    {
        private Clock clock;
        private Blackboard sut;

        [SetUp]
        public void SetUp()
        {
            this.clock = new Clock();
            this.sut = new Blackboard(clock);
        }

        [Test]
        public void ShouldNotNotifyObservers_WhenNoClockUpdate()
        {
            bool notified = false;
            this.sut.AddObserver("test", (Blackboard.Type type, object value) =>
            {
                notified = true;
            });

            this.sut.Set("test", 1f);
            Assert.IsFalse(notified);
        }

        [Test]
        public void ShouldNotifyObservers_WhenClockUpdate()
        {
            bool notified = false;
            this.sut.AddObserver("test", (Blackboard.Type type, object value) =>
            {
                notified = true;
            });

            this.sut.Set("test", 1f);
            this.clock.Update(1f);
            Assert.IsTrue(notified);
        }

        [Test]
        public void ShouldNotNotifyObserver_WhenRemovedDuringOtherObserver()
        {
            bool notified = false;
            System.Action<Blackboard.Type, object> obs1 = null;
            System.Action<Blackboard.Type, object> obs2 = null;

            obs1 = (Blackboard.Type type, object value) =>
            {
                Assert.IsFalse(notified);
                notified = true;
                this.sut.RemoveObserver("test", obs2);
            };
            obs2 = (Blackboard.Type type, object value) =>
            {
                Assert.IsFalse(notified);
                notified = true;
                this.sut.RemoveObserver("test", obs1);
            };
            this.sut.AddObserver("test", obs1);
            this.sut.AddObserver("test", obs2);

            this.sut.Set("test", 1f);
            this.clock.Update(1f);
            Assert.IsTrue(notified);
        }
    }
}