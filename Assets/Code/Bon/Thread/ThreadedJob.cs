using System.Collections;

namespace Assets.Code.Bon.Thread
{
	public abstract class ThreadedJob  {

		private bool _IsDone;
		private object _Handle = new object();

		private System.Threading.Thread _Thread;

		public bool IsDone
		{
			get
			{
				bool tmp;
				lock (_Handle)
				{
					tmp = _IsDone;
				}
				return tmp;
			}
			set
			{
				lock (_Handle)
				{
					_IsDone = value;
				}
			}
		}

		public virtual void Start()
		{
			_Thread = new System.Threading.Thread(Run);
			_Thread.Start();
		}

		public virtual void Abort()
		{
			if (_Thread != null) _Thread.Abort();
		}

		protected virtual void ThreadFunction() { }

		protected virtual void OnFinished() { }

		public virtual bool Update()
		{
			if (IsDone)
			{
				OnFinished();
				return true;
			}
			return false;
		}

		public IEnumerator WaitFor()
		{
			while(!Update())
			{
				yield return null;
			}
		}

		private void Run()
		{
			ThreadFunction();
			IsDone = true;
		}

		public bool IsStarted()
		{
			return _Thread != null;
		}
	}
}
