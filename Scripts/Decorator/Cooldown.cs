using UnityEngine.Assertions;

namespace NPBehave
{

    public class Cooldown : Decorator
    {
        private bool startAfterDecoratee = false;
        private bool resetOnFailiure = false;
	    private bool failOnCooldown = false;
        private float cooldownTime = 0.0f;
        private float randomVariation = 0.05f;
        private bool isReady = true;

        /// <summary>
        /// The Cooldown decorator ensures that the branch can not be started twice within the given cooldown time.
        /// 
        /// The decorator can start the cooldown timer right away or wait until the child stopps, you can control this behavior with the
        /// `startAfterDecoratee` parameter.
        /// 
        /// The default behavior in case the cooldown timer is active and this node is started again is, that the decorator waits until
        /// the cooldown is reached and then executes the underlying node.
        /// You can change this behavior with the `failOnCooldown` parameter to make the decorator immediately fail instead.
        /// 
        /// </summary>
        /// <param name="cooldownTime">time until next execution</param>
        /// <param name="randomVariation">random variation added to the cooldown time</param>
        /// <param name="startAfterDecoratee">If set to <c>true</c> the cooldown timer is started from the point after the decoratee has been started, else it will be started right away.</param>
        /// <param name="resetOnFailiure">If set to <c>true</c> the timer will be reset in case the underlying node fails.</param>
        /// <param name="failOnCooldown">If currently on cooldown and this parameter is set to <c>true</c>, the decorator will immmediately fail instead of waiting for the cooldown.</param>
        /// <param name="decoratee">Decoratee node.</param>
        public Cooldown(float cooldownTime, float randomVariation, bool startAfterDecoratee, bool resetOnFailiure, bool failOnCooldown, Node decoratee) : base("TimeCooldown", decoratee)
        {
        	this.startAfterDecoratee = startAfterDecoratee;
        	this.cooldownTime = cooldownTime;
        	this.resetOnFailiure = resetOnFailiure;
        	this.randomVariation = randomVariation;
        	this.failOnCooldown = failOnCooldown;
        	Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, bool startAfterDecoratee, bool resetOnFailiure, bool failOnCooldown, Node decoratee) : base("TimeCooldown", decoratee)
        {
        	this.startAfterDecoratee = startAfterDecoratee;
        	this.cooldownTime = cooldownTime;
        	this.randomVariation = cooldownTime * 0.1f;
        	this.resetOnFailiure = resetOnFailiure;
        	this.failOnCooldown = failOnCooldown;
        	Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, float randomVariation, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee) : base("TimeCooldown", decoratee)
        {
        	this.startAfterDecoratee = startAfterDecoratee;
        	this.cooldownTime = cooldownTime;
        	this.resetOnFailiure = resetOnFailiure;
        	this.randomVariation = randomVariation;
        	Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee) : base("TimeCooldown", decoratee)
        {
            this.startAfterDecoratee = startAfterDecoratee;
            this.cooldownTime = cooldownTime;
            this.randomVariation = cooldownTime * 0.1f;
            this.resetOnFailiure = resetOnFailiure;
            Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, float randomVariation, Node decoratee) : base("TimeCooldown", decoratee)
        {
            this.startAfterDecoratee = false;
            this.cooldownTime = cooldownTime;
            this.resetOnFailiure = false;
            this.randomVariation = randomVariation;
        	Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, Node decoratee) : base("TimeCooldown", decoratee)
        {
            this.startAfterDecoratee = false;
            this.cooldownTime = cooldownTime;
            this.resetOnFailiure = false;
            this.randomVariation = cooldownTime * 0.1f;
        	Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        protected override void DoStart()
        {
            if (isReady)
            {
                isReady = false;
                if (!startAfterDecoratee)
                {
                    Clock.AddTimer(cooldownTime, randomVariation, 0, TimeoutReached);
                }
                Decoratee.Start();
            }
            else
            {
                if (failOnCooldown)
                {
                    Stopped(false);
                }
            }
        }

        override protected void DoStop()
        {
            if (Decoratee.IsActive)
            {
                isReady = true;
                Clock.RemoveTimer(TimeoutReached);
                Decoratee.Stop();
            }
            else
            {
                isReady = true;
                Clock.RemoveTimer(TimeoutReached);
                Stopped(false);
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            if (resetOnFailiure && !result)
            {
                isReady = true;
                Clock.RemoveTimer(TimeoutReached);
            }
            else if (startAfterDecoratee)
            {
                Clock.AddTimer(cooldownTime, randomVariation, 0, TimeoutReached);
            }
            Stopped(result);
        }

        private void TimeoutReached()
        {
            if (IsActive && !Decoratee.IsActive)
            {
                Clock.AddTimer(cooldownTime, randomVariation, 0, TimeoutReached);
                Decoratee.Start();
            }
            else
            {
                isReady = true;
            }
        }
    }
}