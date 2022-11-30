﻿namespace NPBehave
{
    public class Service : Decorator
    {
        private System.Action serviceMethod;

        private float interval = -1.0f;
        private float randomVariation;

        public Service(float interval, float randomVariation, System.Action service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.interval = interval;
            this.randomVariation = randomVariation;

            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }

        public Service(float interval, System.Action service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.interval = interval;
            this.randomVariation = interval * 0.05f;
            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }

        public Service(System.Action service, Node decoratee) : base("Service", decoratee)
        {
            this.serviceMethod = service;
            this.Label = "every tick";
        }

        protected override void DoStart()
        {
            startService();
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            stopService();
            Stopped(result);
        }

        public override void Pause()
        {
            base.Pause();
            stopService();
        }

        public override void Resume()
        {
            startService();
            base.Resume();
        }

        private void startService()
        {
            if (this.interval <= 0f)
            {
                this.Clock.AddUpdateObserver(serviceMethod);
                serviceMethod();
            }
            else if (randomVariation <= 0f)
            {
                this.Clock.AddTimer(this.interval, -1, serviceMethod);
                serviceMethod();
            }
            else
            {
                InvokeServiceMethodWithRandomVariation();
            }
        }

        private void stopService()
        {
            if (this.interval <= 0f)
            {
                this.Clock.RemoveUpdateObserver(serviceMethod);
            }
            else if (randomVariation <= 0f)
            {
                this.Clock.RemoveTimer(serviceMethod);
            }
            else
            {
                this.Clock.RemoveTimer(InvokeServiceMethodWithRandomVariation);
            }
        }
        
        private void InvokeServiceMethodWithRandomVariation()
        {
            serviceMethod();
            this.Clock.AddTimer(interval, randomVariation, 0, InvokeServiceMethodWithRandomVariation);
        }
    }
}