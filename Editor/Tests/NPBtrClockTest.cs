using UnityEngine;
using System.Collections;
using System;
using NUnit.Framework;

public class NPBtrClockTest
{
    private NPBtrClock sut;

    [SetUp]
    public void SetUp()
    {
        this.sut = new NPBtrClock();
    }

    [Test]
    public void ShouldUpdateObserversInOrder()
    {
        int currentAction = 0;
        Action action0 = () => { Assert.AreEqual(0, currentAction++); };
        Action action1 = () => { Assert.AreEqual(1, currentAction++); };
        Action action2 = () => { Assert.AreEqual(2, currentAction++); };
        Action action3 = () => { Assert.AreEqual(3, currentAction++); };
        Action action4 = () => { Assert.AreEqual(4, currentAction++); };

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
        Action action2 = () =>
        {
            action2Invoked = true;
        };
        Action action1 = new Action(() =>
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
        Action timer2 = () =>
        {
            timer2Invoked = true;
        };
        Action action1 = new Action(() =>
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
        Action timer2 = () =>
        {
            timer2Invoked = true;
        };
        Action timer1 = new Action(() =>
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
