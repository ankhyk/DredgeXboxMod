using System;
using System.Threading.Tasks;

public interface IConsoleManagementStrategy
{
	Task InitConsole();

	Task LoginUser();

	string GetUserName();

	void UpdateConsole();

	void OpenStore(StoreSKUData data);

	Task<bool> SwitchUser();
}
