using System;
using System.Collections.Generic;

namespace Microsoft.Xbox
{
	public class XGameSaveWrapper
	{
		~XGameSaveWrapper()
		{
		}

		public void InitializeAsync(XGameSaveWrapper.XUserHandle userHandle, string scid, XGameSaveWrapper.InitializeCallback callback)
		{
			callback(0);
		}

		public void GetQuotaAsync(XGameSaveWrapper.GetQuotaCallback callback)
		{
			callback(0, 0L);
		}

		public void QueryContainers(string containerNamePrefix, XGameSaveWrapper.QueryContainersCallback callback)
		{
			callback(0, new string[0]);
		}

		public void QueryContainerBlobs(string containerName, XGameSaveWrapper.QueryBlobsCallback callback)
		{
			callback(0, new Dictionary<string, uint>());
		}

		public void Load(string containerName, string blobName, XGameSaveWrapper.LoadCallback callback)
		{
			callback(0, new byte[0]);
		}

		public void Save(string containerName, string blobName, byte[] blobData, XGameSaveWrapper.SaveCallback callback)
		{
			callback(0);
		}

		public void Delete(string containerName, XGameSaveWrapper.DeleteCallback callback)
		{
			callback(0);
		}

		public void Delete(string containerName, string blobName, XGameSaveWrapper.DeleteCallback callback)
		{
			callback(0);
		}

		public void Delete(string containerName, string[] blobNames, XGameSaveWrapper.DeleteCallback callback)
		{
			callback(0);
		}

		private void Update(string containerName, IDictionary<string, byte[]> blobsToSave, IList<string> blobsToDelete, XGameSaveWrapper.UpdateCallback callback)
		{
			callback(0);
		}

		public struct XUserHandle
		{
		}

		public delegate void InitializeCallback(int hresult);

		public delegate void GetQuotaCallback(int hresult, long remainingQuota);

		public delegate void QueryContainersCallback(int hresult, string[] containerNames);

		public delegate void QueryBlobsCallback(int hresult, Dictionary<string, uint> blobInfos);

		public delegate void LoadCallback(int hresult, byte[] blobData);

		public delegate void SaveCallback(int hresult);

		public delegate void DeleteCallback(int hresult);

		private delegate void UpdateCallback(int hresult);
	}
}
