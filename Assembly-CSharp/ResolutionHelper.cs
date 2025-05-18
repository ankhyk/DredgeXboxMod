using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResolutionHelper
{
	public static List<Resolution> GetSupportedResolutionsAscending()
	{
		return (from resolution in Screen.resolutions
			where ResolutionHelper.IsSupportedAspectRatio((float)resolution.width, (float)resolution.height)
			select new Resolution
			{
				width = resolution.width,
				height = resolution.height
			}).Distinct<Resolution>().ToList<Resolution>();
	}

	public static List<Resolution> GetSupportedResolutionsDescending()
	{
		return (from resolution in Screen.resolutions
			where ResolutionHelper.IsSupportedAspectRatio((float)resolution.width, (float)resolution.height)
			select new Resolution
			{
				width = resolution.width,
				height = resolution.height
			}).Distinct<Resolution>().Reverse<Resolution>().ToList<Resolution>();
	}

	public static Resolution GetBestSupportedResolution()
	{
		return ResolutionHelper.GetSupportedResolutionsDescending().FirstOrDefault<Resolution>();
	}

	public static bool IsSupportedAspectRatio(float width, float height)
	{
		return (decimal)width / (decimal)height >= ResolutionHelper.supportedAspectRatio;
	}

	private static decimal supportedAspectRatio = 1.6m;
}
