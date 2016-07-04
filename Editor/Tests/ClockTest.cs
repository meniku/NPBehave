using NUnit.Framework;
namespace NPBehave
{

    public class ClockTest
    {
        private NPBehave.Clock sut;

        [SetUp]
        public void SetUp()
        {
            this.sut = new NPBehave.Clock();
        }

        [Test]
        public void ShouldUpdateObserversInOrder()
        {
            int currentAction = 0;
            System.Action action0 = () => { Assert.AreEqual(0, currentAction++); };
            System.Action action1 = () => { Assert.AreEqual(1, currentAction++); };
            System.Action action2 = () => { Assert.AreEqual(2, currentAction++); };
            System.Action action3 = () => { Assert.AreEqual(3, currentAction++); };
            System.Action action4 = () => { Assert.AreEqual(4, currentAction++); };

            this.sut.AddUpdateObserver(action4);
            this.sut.AddUpdateObserver(action0);
            this.sut.AddUpdateObserver(action1);
            this.sut.AddUpdateObserver(action2);
            this.sut.AddUpdateObserver(action3);
            this.sut.RemoveUpdateObserver(action4);
            this.sut.AddUpdateObserver(action4);

            this.sut.Update(0);
            Assert.AreEqual(5, currentAction);
        }

        [Test]
        public void ShouldNotUpdateObserver_WhenRemovedDuringUpdate()
        {
            bool action2Invoked = false;
            System.Action action2 = () =>
            {
                action2Invoked = true;
            };
            System.Action action1 = new System.Action(() =>
            {
                Assert.IsFalse(action2Invoked);
                this.sut.RemoveUpdateObserver(action2);
            });

            this.sut.AddUpdateObserver(action1);
            this.sut.AddUpdateObserver(action2);
            this.sut.Update(0);
            Assert.IsFalse(action2Invoked);
        }

        [Test]
        public void ShouldNotUpdateTimer_WhenRemovedDuringUpdate()
        {
            bool timer2Invoked = false;
            System.Action timer2 = () =>
            {
                timer2Invoked = true;
            };
            System.Action action1 = new System.Action(() =>
            {
                Assert.IsFalse(timer2Invoked);
                this.sut.RemoveTimer(timer2);
            });

            this.sut.AddUpdateObserver(action1);
            this.sut.AddTimer(0f, 0, timer2);
            this.sut.Update(1);
            Assert.IsFalse(timer2Invoked);
        }

        [Test]
        public void ShouldNotUpdateTimer_WhenRemovedDuringTimer()
        {
            // TODO: as it's a dictionary, the order of events could not always be correct...
            bool timer2Invoked = false;
            System.Action timer2 = () =>
            {
                timer2Invoked = true;
            };
            System.Action timer1 = new System.Action(() =>
            {
                Assert.IsFalse(timer2Invoked);
                this.sut.RemoveTimer(timer2);
            });

            this.sut.AddTimer(0f, 0, timer1);
            this.sut.AddTimer(0f, 0, timer2);
            this.sut.Update(1);
            Assert.IsFalse(timer2Invoked);
        }
    }
}