using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public static class CollectionUtil
{
	public static object DeepClone(object obj)
	{
		object obj2 = null;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, obj);
			memoryStream.Position = 0L;
			obj2 = binaryFormatter.Deserialize(memoryStream);
		}
		return obj2;
	}

	public static T PickRandom<T>(this IEnumerable<T> source)
	{
		return source.PickRandom(1).Single<T>();
	}

	public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
	{
		return source.Shuffle<T>().Take(count);
	}

	public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
	{
		return source.OrderBy((T x) => Guid.NewGuid());
	}
}
