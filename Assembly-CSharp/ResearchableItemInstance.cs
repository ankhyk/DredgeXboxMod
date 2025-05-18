using System;

[Serializable]
public class ResearchableItemInstance : NonSpatialItemInstance
{
	public bool IsResearchComplete
	{
		get
		{
			return this.progress >= 1f;
		}
	}

	public bool isActive;

	public float progress;
}
