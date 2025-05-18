using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AeLa.EasyFeedback.FormFields
{
	internal class SaveStateCollector : FormElement
	{
		public override void Awake()
		{
			base.Awake();
			this.log = new StringBuilder();
			Application.logMessageReceived += this.HandleLog;
		}

		protected override void FormClosed()
		{
		}

		protected override void FormOpened()
		{
		}

		protected override void FormSubmitted()
		{
			if (GameManager.Instance.SaveData == null)
			{
				return;
			}
			string json = JsonUtility.ToJson(GameManager.Instance.SaveData, true);
			json += "\nintVariables:\n";
			GameManager.Instance.SaveData.intVariables.Keys.ToList<string>().ForEach(delegate(string key)
			{
				int num = GameManager.Instance.SaveData.intVariables[key];
				json += string.Format("  {0}: {1}\n", key, num);
			});
			json += "\ndecimalVariables:\n";
			GameManager.Instance.SaveData.decimalVariables.Keys.ToList<string>().ForEach(delegate(string key)
			{
				decimal num2 = GameManager.Instance.SaveData.decimalVariables[key];
				json += string.Format("  {0}: {1}\n", key, num2);
			});
			json += "\nfloatVariables:\n";
			GameManager.Instance.SaveData.floatVariables.Keys.ToList<string>().ForEach(delegate(string key)
			{
				float num3 = GameManager.Instance.SaveData.floatVariables[key];
				json += string.Format("  {0}: {1}\n", key, num3);
			});
			json += "\nboolVariables:\n";
			GameManager.Instance.SaveData.boolVariables.Keys.ToList<string>().ForEach(delegate(string key)
			{
				bool flag = GameManager.Instance.SaveData.boolVariables[key];
				json += string.Format("  {0}: {1}\n", key, flag);
			});
			json += "\nstringVariables:\n";
			GameManager.Instance.SaveData.stringVariables.Keys.ToList<string>().ForEach(delegate(string key)
			{
				string text = GameManager.Instance.SaveData.stringVariables[key];
				json = string.Concat(new string[] { json, "  ", key, ": ", text, "\n" });
			});
			json += "\ngrids:\n";
			GameManager.Instance.SaveData.grids.Keys.ToList<GridKey>().ForEach(delegate(GridKey key)
			{
				string text2 = JsonUtility.ToJson(GameManager.Instance.SaveData.grids[key], true);
				json += string.Format("  {0}: {1}\n", key, text2);
			});
			json += "\nownedNonSpatialItems:\n";
			GameManager.Instance.SaveData.ownedNonSpatialItems.ForEach(delegate(NonSpatialItemInstance itemInstance)
			{
				string text3 = JsonUtility.ToJson(itemInstance, true);
				json = json + "  " + text3 + "\n";
			});
			json += "\nquestEntries:\n";
			GameManager.Instance.SaveData.questEntries.Keys.ToList<string>().ForEach(delegate(string key)
			{
				string text4 = JsonUtility.ToJson(GameManager.Instance.SaveData.questEntries[key], true);
				json = string.Concat(new string[] { json, "  ", key, ": ", text4, "\n" });
			});
			json += "\nvisitedNodes:\n";
			GameManager.Instance.SaveData.visitedNodes.ToList<string>().ForEach(delegate(string node)
			{
				json = json + "  " + node + "\n";
			});
			json += "\nhistoryOfItemsOwned:\n";
			GameManager.Instance.SaveData.historyOfItemsOwned.ToList<string>().ForEach(delegate(string item)
			{
				json = json + "  " + item + "\n";
			});
			byte[] bytes = Encoding.ASCII.GetBytes(json.ToString());
			this.Form.CurrentReport.AttachFile("save.txt", bytes);
		}

		private void HandleLog(string logString, string stackTrace, LogType logType)
		{
			if (logType != LogType.Exception)
			{
				this.log.AppendFormat("{0}: {1}", logType.ToString(), logString);
			}
			else
			{
				this.log.AppendLine(logString);
			}
			this.log.AppendLine(stackTrace);
		}

		private StringBuilder log;
	}
}
