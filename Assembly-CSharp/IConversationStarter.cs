using System;
using Cinemachine;

public interface IConversationStarter
{
	bool IsOneTimeOnly { get; }

	string ConversationNodeName { get; }

	bool ReleaseCameraOnComplete { get; }

	CinemachineVirtualCamera VCam { get; }

	void OnConversationStarted();

	void OnConversationCompleted();
}
