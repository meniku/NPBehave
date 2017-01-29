namespace NPBehave
{
    public class BlackboardQuery : ObservingDecorator
    {
        private string[] keys;
        private System.Func<bool> query;

        public BlackboardQuery(string[] keys, Stops stopsOnChange, System.Func<bool> query, Node decoratee) : base("BlackboardQuery", stopsOnChange, decoratee)
        {
            this.keys = keys;
            this.query = query;
        }

        override protected void StartObserving()
        {
            foreach (string key in this.keys)
            {
                this.RootNode.Blackboard.AddObserver(key, onValueChanged);
            }
        }

        override protected void StopObserving()
        {
            foreach (string key in this.keys)
            {
                this.RootNode.Blackboard.RemoveObserver(key, onValueChanged);
            }
        }

        private void onValueChanged(Blackboard.Type type, object newValue)
        {
            Evaluate();
        }

        protected override bool IsConditionMet()
        {
            return this.query();
        }

        override public string ToString()
        {
            string keys = "";
            foreach (string key in this.keys)
            {
                keys += " " + key;
            }
            return Name + keys;
        }
    }
}