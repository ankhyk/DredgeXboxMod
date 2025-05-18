using System;

public interface IConstructableObjectUI<T>
{
	void Init(T recipe);

	void Close();
}
