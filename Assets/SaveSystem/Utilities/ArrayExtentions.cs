using System.Linq;

public static class ArrayExtentions
{
    public static T[] Add<T>(T item, T[] array)
    {
        var newArray = new T[array.Length + 1];

        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }

        newArray[array.Length] = item;

        return newArray;
    }

    public static T[] Remove<T>(int removeIndex, T[] array)
    {
        var newArray = array.Where((source, index) => index != removeIndex).ToArray();

        return newArray;
    }
}
