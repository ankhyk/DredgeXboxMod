using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.GameCore.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox
{
	public class Gdk : MonoBehaviour
	{
		public static Gdk Helpers
		{
			get
			{
				if (Gdk._xboxHelpers == null)
				{
					Gdk[] array = global::UnityEngine.Object.FindObjectsOfType<Gdk>();
					if (array.Length != 0)
					{
						Gdk._xboxHelpers = array[0];
						Gdk._xboxHelpers._Initialize();
					}
					else
					{
						Debug.LogError("Error: Could not find Xbox prefab. Make sure you have added the Xbox prefab to your scene.");
					}
				}
				return Gdk._xboxHelpers;
			}
		}

		public event Gdk.OnGameSaveLoadedHandler OnGameSaveLoaded;

		public event Gdk.OnErrorHandler OnError;

		private bool ValidateGuid(string guid)
		{
			string[] array = guid.Split('-', StringSplitOptions.None);
			if (array.Length != 5)
			{
				return false;
			}
			if (!array.Select((string str) => str.Length).SequenceEqual(new int[] { 8, 4, 4, 4, 12 }))
			{
				return false;
			}
			return guid.All((char c) => "1234567890abcdef-".Contains(c));
		}

		private void OnValidate()
		{
			if (this.scid == this._lastScid)
			{
				return;
			}
			if (this.scid.Length != 36 || !this.ValidateGuid(this.scid))
			{
				Debug.LogError("Invalid SCID given");
				this.scid = this._lastScid;
				return;
			}
			this._lastScid = this.scid;
			XDocument xdocument = XDocument.Load(GdkUtilities.GameConfigPath);
			try
			{
				(from node in xdocument.Descendants("ExtendedAttribute")
					where node.Attribute("Name").Value == "Scid"
					select node).First<XElement>().Attribute("Value").Value = this.scid;
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
				{
					Indent = true,
					NewLineOnAttributes = true
				};
				using (XmlWriter xmlWriter = XmlWriter.Create(GdkUtilities.GameConfigPath, xmlWriterSettings))
				{
					xdocument.WriteTo(xmlWriter);
				}
			}
			catch
			{
				Debug.LogError("Malformed MicrosoftGame.Config. Try associating with the Micosoft Store again or re-import the plugin.");
			}
		}

		private void Start()
		{
			this._Initialize();
		}

		private void _Initialize()
		{
			if (Gdk._initialized)
			{
				return;
			}
			Gdk._initialized = true;
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			Gdk._hresultToFriendlyErrorLookup = new Dictionary<int, string>();
			this.InitializeHresultToFriendlyErrorLookup();
		}

		private void InitializeHresultToFriendlyErrorLookup()
		{
			if (Gdk._hresultToFriendlyErrorLookup == null)
			{
				return;
			}
			Gdk._hresultToFriendlyErrorLookup.Add(-2143330041, "IAP_UNEXPECTED: Does the player you are signed in as have a license for the game? You can get one by downloading your game from the store and purchasing it first. If you can't find your game in the store, have you published it in Partner Center?");
			Gdk._hresultToFriendlyErrorLookup.Add(-1994108656, "E_GAMEUSER_NO_PACKAGE_IDENTITY: Are you trying to call GDK APIs from the Unity editor? To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once yourgame is launched.");
			Gdk._hresultToFriendlyErrorLookup.Add(-1994129152, "E_GAMERUNTIME_NOT_INITIALIZED: Are you trying to call GDK APIs from the Unity editor? To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once yourgame is launched.");
			Gdk._hresultToFriendlyErrorLookup.Add(-2015559675, "AM_E_XAST_UNEXPECTED: Have you added the Windows 10 PC platform on the Xbox Settings page in Partner Center? Learn more: aka.ms/sandboxtroubleshootingguide");
		}

		public void SignIn()
		{
		}

		public void Save(byte[] data)
		{
		}

		public void LoadSaveData()
		{
		}

		public void UnlockAchievement(string achievementId)
		{
		}

		private void Update()
		{
		}

		protected static bool Succeeded(int hresult, string operationFriendlyName)
		{
			bool flag = false;
			if (HR.SUCCEEDED(hresult))
			{
				flag = true;
			}
			else
			{
				string text = hresult.ToString("X8");
				string text2 = string.Empty;
				if (Gdk._hresultToFriendlyErrorLookup.ContainsKey(hresult))
				{
					text2 = Gdk._hresultToFriendlyErrorLookup[hresult];
				}
				else
				{
					text2 = operationFriendlyName + " failed.";
				}
				Debug.LogError(string.Format("{0} Error code: hr=0x{1}", text2, text));
				if (Gdk.Helpers.OnError != null)
				{
					Gdk.Helpers.OnError(Gdk.Helpers, new ErrorEventArgs(text, text2));
				}
			}
			return flag;
		}

		[Header("Changing the SCID here will also change the value in your MicrosoftGame.config")]
		[Tooltip("Service Configuration GUID in the form: 12345678-1234-1234-1234-123456789abc")]
		[Delayed]
		public string scid;

		[Tooltip("Will automatically sign the user in after XGameRuntime initialization if checked")]
		public bool signInOnStart = true;

		public Text gamertagLabel;

		private static Gdk _xboxHelpers;

		private static bool _initialized;

		private static Dictionary<int, string> _hresultToFriendlyErrorLookup;

		private string _lastScid = string.Empty;

		private const int _100PercentAchievementProgress = 100;

		private const string _GameSaveContainerName = "x_game_save_default_container";

		private const string _GameSaveBlobName = "x_game_save_default_blob";

		private const int _MaxAssociatedProductsToRetrieve = 25;

		public delegate void OnGameSaveLoadedHandler(object sender, GameSaveLoadedArgs e);

		public delegate void OnErrorHandler(object sender, ErrorEventArgs e);
	}
}
