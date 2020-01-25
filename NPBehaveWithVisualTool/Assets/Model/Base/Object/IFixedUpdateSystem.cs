using System;

namespace ETModel
{
	public interface IFixedUpdateSystem
	{
		Type Type();
		void Run(object o);
	}

	public abstract class FixedUpdateSystem<T> : IFixedUpdateSystem
    {
		public void Run(object o)
		{
			this.FixedUpdate((T)o);
		}

		public Type Type()
		{
			return typeof(T);
		}

		public abstract void FixedUpdate(T self);
	}
}
