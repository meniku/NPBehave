using System;

namespace NPBehave
{
    public class Condition : ObservingDecorator
    {
        private Func<bool> condition;
        private float checkInterval;
        private float checkVariance;

        public Condition(Func<bool> condition, Node decoratee) : base("Condition", Stops.NONE, decoratee)
        {
            this.condition = condition;
            this.checkInterval = 0.0f;
            this.checkVariance = 0.0f;
        }

        public Condition(Func<bool> condition, Stops stopsOnChange, Node decoratee) : base("Condition", stopsOnChange, decoratee)
        {
            this.condition = condition;
            this.checkInterval = 0.0f;
            this.checkVariance = 0.0f;
        }

        public Condition(Func<bool> condition, Stops stopsOnChange, float checkInterval, float randomVariance, Node decoratee) : base("Condition", stopsOnChange, decoratee)
        {
            this.condition = condition;
            this.checkInterval = checkInterval;
            this.checkVariance = randomVariance;
        }

        override protected void StartObserving()
        {
            this.RootNode.Clock.AddTimer(checkInterval, checkVariance, -1, Evaluate);
        }

        override protected void StopObserving()
        {
            this.RootNode.Clock.RemoveTimer(Evaluate);
        }

        protected override bool IsConditionMet()
        {
            return this.condition();
        }
    }
}