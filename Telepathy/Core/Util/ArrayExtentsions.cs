namespace Telepathy.Core.Util
{
    public static class ArrayExtentsions
    {
        public static void Fill<T>(this T[] originalArray, T with)
        {
            for (var i = 0; i < originalArray.Length; i++)
            {
                originalArray[i] = with;
            }
        }        
    }
}