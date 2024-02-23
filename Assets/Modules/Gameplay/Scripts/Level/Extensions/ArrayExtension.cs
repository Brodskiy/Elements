using Modules.Gameplay.Scripts.GameElement.Data.Block;

namespace Modules.Level.Extensions
{
    internal static class ArrayExtension
    {
        public static int[,] ToTwoDimensionalArray(this int[] array, int columns, int rows)
        {
            var twoDimensionalArray = new int[columns, rows];
            for (var i = 0; i < columns; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    var index = i * rows + j;
                    twoDimensionalArray[i, j] = array[index];
                }
            }

            return twoDimensionalArray;
        }
        
        public static int[,] ToTwoDimensionalArray(this BlockData[] blockDatas, int columns, int rows)
        {
            var twoDimensionalArray = new int[columns, rows];
            for (var i = 0; i < columns; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    var index = i * rows + j;
                    twoDimensionalArray[i, j] = blockDatas[index] == null ? 0 : blockDatas[index].Id;
                }
            }

            return twoDimensionalArray;
        }

        public static int[] ToArray(this int[,] twoDimensionalArray)
        {
            var columns = twoDimensionalArray.GetLength(0);
            var rows = twoDimensionalArray.GetLength(1);
            var array = new int[rows * columns];
            for (var i = 0; i < columns; i++)
            {
                for (var j = 0; j < rows; j++)
                {
                    var index = i * rows + j;
                    array[index] = twoDimensionalArray[i, j];
                }
            }

            return array;
        }
    }
}