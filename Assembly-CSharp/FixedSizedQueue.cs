using System;
using System.Collections.Concurrent;

public class FixedSizedQueue<T> : ConcurrentQueue<T>
{
	public int Size { get; private set; }

	public FixedSizedQueue(int size)
	{
		this.Size = size;
	}

	public new void Enqueue(T obj)
	{
		base.Enqueue(obj);
		object obj2 = this.syncObject;
		lock (obj2)
		{
			while (base.Count > this.Size)
			{
				T t;
				base.TryDequeue(out t);
			}
		}
	}

	private readonly object syncObject = new object();
}
