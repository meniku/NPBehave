using UnityEngine;
using System.Collections;
using System;
using NUnit.Framework;

public class NPBtrBlackboardTest
{
    private NPBtrClock clock;
    private NPBtrBlackboard sut;

    [SetUp]
    public void SetUp()
    {
        this.clock = new NPBtrClock();
        this.sut = new NPBtrBlackboard(clock);
    }

    [Test]
    public void ShouldNotNotifyObservers_WhenNoClockUpdate()
    {
        bool notified = false;
        this.sut.AddObserver("test", (NPBtrBlackboard.Type type, object value) => {
            notified = true;
        });

        this.sut.Set("test", 1f);
        Assert.IsFalse(notified);
    }

    [Test]
    public void ShouldNotifyObservers_WhenClockUpdate()
    {
        bool notified = false;
        this.sut.AddObserver("test", (NPBtrBlackboard.Type type, object value) => {
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
        System.Action<NPBtrBlackboard.Type, object> obs1 = null;
        System.Action<NPBtrBlackboard.Type, object> obs2 = null;
        
        obs1 = (NPBtrBlackboard.Type type, object value) => {
            Assert.IsFalse(notified);
            notified = true;
            this.sut.RemoveObserver("test", obs2);
        };
        obs2 = (NPBtrBlackboard.Type type, object value) => {
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
