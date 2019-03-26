using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    class Tools
    {



        public interface IValueParser<T>
        {
            T Parse( string value );
        }

        public abstract class EntryParser<T> : IValueParser<T>
        {
            public abstract T Parse( string value );
            public T Parse() => Parse( Console.ReadLine() );
        }

        public class IntParser : EntryParser<int>
        {
            public override int Parse( string value ) => int.Parse( value );
        }

        public class LineParser<T> : EntryParser<T[]>
        {
            readonly IValueParser<T> _parser;
            public LineParser( IValueParser<T> parser )
            {
                _parser = parser;
            }
            public override T[] Parse( string value )
            {
                return value.Split( ' ' ).Select( p => _parser.Parse( p ) ).ToArray();
            }
        }

        public class ArrayParser<T>
        {
            readonly EntryParser<T> _parser;

            public ArrayParser( EntryParser<T> parser )
            {
                _parser = parser;
            }
            T[] Parse()
            {
                T[] output = new T[int.Parse( Console.ReadLine() )];
                for( int i = 0; i < output.Length; i++ )
                {
                    output[i] = _parser.Parse();
                }
                return output;
            }
        }
        public struct NumberBaseX
        {
            public readonly int Base;
            public int[] Digits;
            public bool Overflowed;
            public NumberBaseX( int numberBase, int numberOfDigit )
            {
                Base = numberBase;
                Digits = new int[numberOfDigit];
                Overflowed = false;
            }

            public static NumberBaseX operator +( NumberBaseX a, long b )
            {
                for( int i = 0; i < a.Digits.Length; i++ )
                {
                    a.Digits[i] += (int)(b % a.Base);
                    if( a.Digits[i] >= a.Base )
                    {
                        a.Digits[i] -= a.Base;
                        b += (long)Math.Pow( a.Base, i + 1 );
                    }
                    b /= a.Base;
                    if( b == 0 ) break;
                }
                if( b != 0 ) a.Overflowed = true;
                return a;
            }
            public bool HaveDuplicateDigits()
            {
                BoolArray boolArray = new BoolArray( Base );
                for( int i = 0; i < Digits.Length; i++ )
                {
                    if( boolArray[Digits[i]] )
                    {
                        return true;
                    }
                    boolArray[Digits[i]] = true;
                }
                return false;
            }
        }
        public struct NumberBaseXWithUniqueDigits
        {
            public NumberBaseX NumberBaseX;
            public NumberBaseXWithUniqueDigits( int numberBase, int numberOfDigit )
            {
                NumberBaseX = new NumberBaseX( numberBase, numberOfDigit );
                if( NumberBaseX.HaveDuplicateDigits() )
                {
                    Increment();
                }
            }

            public void Increment()
            {
                do
                {
                    NumberBaseX += 1;
                } while( NumberBaseX.HaveDuplicateDigits() );
            }
        }

        public struct BoolArray
        {
            readonly ulong[] _bools;
            readonly byte _unusedBits;
            public BoolArray( int size )
            {
                _bools = new ulong[(int)Math.Ceiling( size / 64.0 )];
                _unusedBits = (byte)(64 - (size % 64));
            }

            public bool this[int i]
            {
                get
                {
                    return ((_bools[i / 64] >> (63 - i % 64)) & 1) == 1;
                }

                set
                {
                    _bools[i / 64] |= ((uint)(value ? 1 : 0) << (63 - (i % 64)));
                }
            }

            public bool AreAllZero()
            {
                for( int i = 0; i < _bools.Length; i++ )
                {
                    if( _bools[i] != 0 ) return false;
                }
                return true;
            }

            public bool AreAllOne()
            {
                for( int i = 0; i < _bools.Length; i++ )
                {
                    if( _bools[i] != long.MaxValue )
                    {
                        if( i == _bools.Length - 1 )
                        {
                            for( int j = 0; j < 64 - _unusedBits; j++ )
                            {
                                if( !this[i * 64 + j] ) return false;
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public class AllCombination<T>
        {
            readonly T[] _elements;
            NumberBaseXWithUniqueDigits _currentMix;
            public AllCombination( List<T> enumerable )
            {
                _elements = enumerable.ToArray();
                _currentMix = new NumberBaseXWithUniqueDigits( enumerable.Count(), enumerable.Count() );
            }

            public T[] GetNextCombination()
            {
                T[] output = new T[_elements.Length];
                for( int i = 0; i < output.Length; i++ )
                {
                    output[i] = _elements[_currentMix.NumberBaseX.Digits[i]];
                }
                _currentMix.Increment();
                return output;
            }

            public IEnumerable<T[]> GetAllCombinations()
            {
                while( !_currentMix.NumberBaseX.Overflowed )
                {
                    yield return GetNextCombination();
                }
            }
        }
    }
}
