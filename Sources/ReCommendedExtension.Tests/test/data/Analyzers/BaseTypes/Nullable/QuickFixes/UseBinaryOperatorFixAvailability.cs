using System;
using System.Threading;

namespace Test
{
    public class Nullables
    {
        public enum ZeroBasedenum { Zero, One, Two, Three }
        public enum NonZeroBasedenum { One = 1, Two = 2, Three = 3 }

        public struct CustomStruct;

        public void GetValueOrDefault<T>(
            byte? _byte,
            sbyte? _sbyte,
            short? _short,
            ushort? _ushort,
            int? _int,
            uint? _uint,
            long? _long,
            ulong? _ulong,
            Int128? _int128,
            UInt128? _uint128,
            nint? _nint,
            nuint? _nuint,
            double? _double,
            float? _float,
            Half? _half,
            decimal? _decimal,
            bool? _bool,
            char? _char,
            ZeroBasedenum? _zeroBasedenum,
            NonZeroBasedenum? _nonZeroBasedenum,
            (int, bool, string)? _tuple,
            (Int128, Half, (bool?, int?))? _tupleNested,
            CustomStruct? _struct,
            CancellationToken? _cancellationToken,
            T? _generic) where T : struct
        {
            var result1 = _byte.GetValueOrDefault();
            var result2 = _sbyte.GetValueOrDefault();
            var result3 = _short.GetValueOrDefault();
            var result4 = _ushort.GetValueOrDefault();
            var result5 = _int.GetValueOrDefault();
            var result6 = _uint.GetValueOrDefault();
            var result7 = _long.GetValueOrDefault();
            var result8 = _ulong.GetValueOrDefault();
            var result9 = _int128.GetValueOrDefault();
            var resultA = _uint128.GetValueOrDefault();
            var resultB = _nint.GetValueOrDefault();
            var resultC = _nuint.GetValueOrDefault();
            var resultD = _double.GetValueOrDefault();
            var resultE = _float.GetValueOrDefault();
            var resultF = _half.GetValueOrDefault();
            var resultG = _decimal.GetValueOrDefault();
            var resultH = _bool.GetValueOrDefault();
            var resultI = _char.GetValueOrDefault();
            var resultJ = _zeroBasedenum.GetValueOrDefault();
            var resultK = _nonZeroBasedenum.GetValueOrDefault();
            var resultL = _tuple.GetValueOrDefault();
            var resultM = _tupleNested.GetValueOrDefault();
            var resultN = _struct.GetValueOrDefault();
            var resultO = _cancellationToken.GetValueOrDefault();
            var resultP = _generic.GetValueOrDefault();
        }

        public void GetValueOrDefault(int? nullable, int defaultValue)
        {
            var result = nullable.GetValueOrDefault(defaultValue);
        }

        public void GetValueOrDefault<T>(T? nullable, T defaultValue) where T : struct
        {
            var result = nullable.GetValueOrDefault(defaultValue);
        }
    }
}