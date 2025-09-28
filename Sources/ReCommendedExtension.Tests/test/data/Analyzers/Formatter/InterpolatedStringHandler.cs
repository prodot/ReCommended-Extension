using System;
using System.Diagnostics;
using System.Text;
using System.Text.Unicode;
using System.Runtime.CompilerServices;

namespace Test
{
    public class Formatters
    {
        public void Handlers(
            int number,
            StringBuilder builder,
            Span<char> destination,
            Span<byte> destinationBytes,
            out int written)
        {
            var result = $"{number:G}";
            builder.Append($"{number:G}");
            destination.TryWrite($"{number:G}", out written);
            Utf8.TryWrite(destinationBytes, $"{number:G}", out written);
            Debug.Assert(number > 0, $"{number:G}");
            Debug.WriteIf(number > 0, $"{number:G}");
        }

        [InterpolatedStringHandler]
        public ref struct CustomInterpolatedStringHandler
        {
            public CustomInterpolatedStringHandler(int literalLength, int formattedCount) { }

            public void AppendFormatted<T>(T value, string? format) { }
        }

        public static void Custom(ref CustomInterpolatedStringHandler handler) { }

        public void NoDetection(int number)
        {
            Custom($"{number:G}");
        }
    }
}