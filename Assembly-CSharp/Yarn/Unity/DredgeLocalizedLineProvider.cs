using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Yarn.Unity
{
	public class DredgeLocalizedLineProvider : LineProviderBehaviour
	{
		public override bool LinesAvailable
		{
			get
			{
				return true;
			}
		}

		private void Awake()
		{
			LocalizationSettings.StringDatabase.GetTableAsync(LanguageManager.YARN_TABLE, LocalizationSettings.SelectedLocale).Completed += this.OnStringTableLoaded;
		}

		private void OnStringTableLoaded(AsyncOperationHandle<StringTable> operation)
		{
			this.stringTable = operation.Result;
		}

		public override LocalizedLine GetLocalizedLine(Line line)
		{
			string text = "";
			try
			{
				text = LocalizationSettings.StringDatabase.GetLocalizedString(LanguageManager.YARN_TABLE, line.ID, LocalizationSettings.SelectedLocale, FallbackBehavior.UseProjectSettings, Array.Empty<object>());
			}
			catch (Exception ex)
			{
				CustomDebug.EditorLogError(ex.ToString());
			}
			return new LocalizedLine
			{
				TextID = line.ID,
				RawText = text,
				Substitutions = line.Substitutions,
				Metadata = base.YarnProject.lineMetadata.GetMetadata(line.ID)
			};
		}

		public override void PrepareForLines(IEnumerable<string> lineIDs)
		{
		}

		private void AssetLoadComplete(AsyncOperationHandle<string> operation)
		{
			string text;
			if (!this.pendingLoadOperations.TryGetValue(operation, out text))
			{
				return;
			}
			this.pendingLoadOperations.Remove(operation);
			AsyncOperationStatus status = operation.Status;
			if (status == AsyncOperationStatus.Succeeded)
			{
				this.completedLoadOperations.Add(text, operation);
				return;
			}
			if (status != AsyncOperationStatus.Failed)
			{
				throw new InvalidOperationException(string.Format("Load operation for {0} completed, but its status is {1}", text, operation.Status));
			}
			CustomDebug.EditorLogError("Failed to load asset for line " + text + "\"");
		}

		private StringTable stringTable;

		public Action<AsyncOperationHandle<string>> AssetLoadCompleteAction;

		public Dictionary<AsyncOperationHandle<string>, string> pendingLoadOperations = new Dictionary<AsyncOperationHandle<string>, string>();

		public Dictionary<string, AsyncOperationHandle<string>> completedLoadOperations = new Dictionary<string, AsyncOperationHandle<string>>();
	}
}
