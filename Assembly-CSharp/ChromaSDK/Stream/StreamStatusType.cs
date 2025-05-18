using System;

namespace ChromaSDK.Stream
{
	public enum StreamStatusType
	{
		READY,
		AUTHORIZING,
		BROADCASTING,
		WATCHING,
		NOT_AUTHORIZED,
		BROADCAST_DUPLICATE,
		SERVICE_OFFLINE
	}
}
