using System.Collections.Generic;

public static class ListExtensions
{
	//先頭にあるオブジェクトを削除せずに返します
	public static T Peek<T>(this IList<T> self)
	{
		return self[0];
	}

	//先頭にあるオブジェクトを削除し、返します
	public static T Pop<T>(this IList<T> self)
	{
		var result = self[0];
		self.RemoveAt(0);
		return result;
	}

	//末尾にオブジェクトを追加します
	public static void Push<T>(this IList<T> self, T item)
	{
		self.Insert(0, item);
	}
}