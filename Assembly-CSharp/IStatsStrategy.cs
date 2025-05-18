using System;

public interface IStatsStrategy
{
	void Init();

	void Update();

	void List();

	void Get(string id);

	void GetF(string id);

	void Set(string id, float num);

	void Set(string id, int num);

	void Add(string id, float num);

	void Add(string id, int num);
}
