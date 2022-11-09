using System.Collections.Generic;

namespace Test
{
    public class NonGenericClass
    {
        int[] field_int = { 0, 00, -0, 0x0, 0b0, default, default(int), new int() };
        int[] field_int_ = { 0, 1, 2 };

        int?[] field_int_nullable = { null, null as int?, default, default(int?), new int?() };
        int?[] field_int_nullable_ = { null, 0 };

        uint[] field_uint = { 0u, 00u, 0x0u, 0b0u, default, default(uint), new uint() };
        uint[] field_uint2 = { 0, 00, 0x0, 0b0, default(int) };
        uint[] field_uint_ = { 0, 1, 2 };

        long[] field_long = { 0L, 00L, -0L, 0x0L, 0b0L, default, default(long), new long() };
        long[] field_long2 = { 0, 00, -0, 0x0, 0b0, default(int), 0u };
        long[] field_long_ = { 0, 1, 2 };

        ulong[] field_ulong = { 0ul, 00ul, 0x0ul, 0b0ul, default, default(ulong), new ulong() };
        ulong[] field_ulong2 = { 0, 00, -0, 0x0, 0b0, default(int), 0L, 0u };
        ulong[] field_ulong_ = { 0, 1, 2 };

        byte[] field_byte = { 0, 00, 0x0, 0b0, default, default(byte), new byte() };
        byte[] field_byte_ = { 0, 1, 2 };

        sbyte[] field_sbyte = { 0, 00, 0x0, 0b0, default, default(sbyte), new sbyte() };
        sbyte[] field_sbyte_ = { 0, 1, 2 };

        short[] field_short = { 0, 00, 0x0, 0b0, default, default(short), new short(), default(byte), default(sbyte) };
        short[] field_short_ = { 0, 1, 2 };

        ushort[] field_ushort = { 0, 00, 0x0, 0b0, default, default(ushort), new ushort(), default(byte) };
        ushort[] field_ushort_ = { 0, 1, 2 };

        decimal[] field_decimal = { 0m, 00m, -0m, default, default(decimal), new decimal() };
        decimal[] field_decimal2 = { 0, 00, 0x0, 0b0, 0u, 0L, 0ul, default(int), default(uint), default(long), default(ulong) };
        decimal[] field_decimal_ = { 0, 1, 2 };

        float[] field_float = { 0f, 00f, -0f, default, default(float), new float() };
        float[] field_float2 = { 0, 00, 0x0, 0b0, 0u, 0L, 0ul, default(int), default(uint), default(long), default(ulong) };
        float[] field_float_ = { 0, 1, 2 };

        double[] field_double = { 0d, 0.0, 00d, -0d, default, default(double), new double() };
        double[] field_double2 = { 0, 00, 0x0, 0b0, 0u, 0L, 0ul, 0f, default(int), default(uint), default(long), default(ulong), default(float) };
        double[] field_double_ = { 0, 1, 2 };

        bool[] field_bool = { false, default, default(bool), new bool() };
        bool[] field_bool_ = { false, true };

        char[] field_char = { '\0', default, default(char), new char() };
        char[] field_char_ = { '\0', 'a', 'b', 'c' };

        string?[] field_string = { null, null as string, default, default(string) };
        string?[] field_string_ = { null, "", "one", new string('a', 0) };

        object?[] field_object = { null, null as object, default, default(object) };
        object?[] field_object_ = { null, new object() };

        int[] Property { get; } = { 0, default };

        int[] Property2 { get; set; } = { 0, default };

        void Method()
        {
            var variable = new[] { 0, default, default(int) };
            var variable = new int[] { 0, default, default(int) };
        }
    }

    public class GenericClass<T> where T : new()
    {
        T[] field = { default, default(T) };
        T[] field_ = { default, new T() };

        void Method(T arg)
        {
            var variable = new[] { default, default(T) };
            var variable_ = new[] { default, arg };

            var variable2 = new[] { null, default, default(List<T>) };
        }
    }

    public class GenericClass_ValueType<T> where T : struct
    {
        T[] field_nonNullable = { default, default(T) };
        T[] field_nonNullable2 = { default, default(T), new T() };

        T?[] field_nullable = { null, null as T?, default, default(T?), new T?() };
        T?[] field_nullable2_ = { null, default(T) };

        void Method(T nonNullable, T? nullable)
        {
            var variable_nonNullable = new[] { default, default(T) };
            var variable_nonNullable2 = new[] { default, default(T), new T() };
            var variable_nonNullable_ = new[] { default, nonNullable };

            var variable_nullable = new[] { null, null as T?, default, default(T?), new T?() };
            var variable_nullable_ = new[] { null, nullable };
        }
    }

    public class GenericClass_ReferenceType<T> where T : class
    {
        T?[] field = { null, null as T, default, default(T) };

        void Method(T arg)
        {
            var variable = new[] { null, null as T, default, default(T) };
            var variable_ = new[] { default, arg };
        }
    }
}