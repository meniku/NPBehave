using UnityEngine.Assertions;

namespace NPBehave
{

    public class Cooldown : Decorator
    {

        private bool startAfterDecoratee;
        private bool resetOnFailiure = false;
        private float cooldownTime = 0.0f;
        private float randomVariation = 0.05f;
        private bool isReady = true;

        public Cooldown(float cooldownTime, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee) : base("TimeCooldown", decoratee)
        {
            this.startAfterDecoratee = startAfterDecoratee;
            this.cooldownTime = cooldownTime;
            this.randomVariation = cooldownTime * 0.1f;
            this.resetOnFailiure = resetOnFailiure;
            Assert.IsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, float randomVariation, bool startAfterDecoratee, bool resetOnFailiure, Node decoratee) : base("TimeCooldown", decoratee)
        {
            this.startAfterDecoratee = startAfterDecoratee;
            this.cooldownTime = cooldownTime;
            this.resetOnFailiure = resetOnFailiure;
            this.randomVariation = randomVariation;
            Assert.IsTrue(cooldownTime > 0f, "limit has to be set");
        }

        public Cooldown(float cooldownTime, float randomVariation, Node decoratee) : base("TimeCooldown", decoratee)
        {
            this.startAfterDecoratee = false;
            this.cooldownTime = cooldownTime;
            this.resetOnFailiure = false;
            this.randomVariation = randomVariation;
            Assert.IsTrue(cooldownTime > 0f, "limit has to be set");
        }

        public Cooldown(float cooldownTime, Node decoratee) : base("TimeCooldown", decoratee)
        {
            this.startAfterDecoratee = false;
            this.cooldownTime = cooldownTime;
            this.resetOnFailiure = false;
            this.randomVariation = cooldownTime * 0.1f;
            Assert.IsTrue(cooldownTime > 0f, "limit has to be set");
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