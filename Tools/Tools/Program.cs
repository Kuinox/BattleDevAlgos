using System;
using System.Collections.Generic;
using System.Text;
using static Tools.Tools;
namespace Tools
{
    class Program
    {
        static void Main( string[] args )
        {
            var comb = new AllCombination<int>(new List<int>(){ 1,2,3,4});
            foreach( int[] item in comb.GetAllCombinations() )
            {
                Console.WriteLine( string.Join( ';', item ) );
            }
        }
    }
}
