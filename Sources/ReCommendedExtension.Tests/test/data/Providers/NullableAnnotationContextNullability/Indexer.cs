using System;

namespace ReCommendedExtension.Tests.test.data
{
    public class Simple
    {
        string this[int a]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        string? this[int a, int b]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        int this[int a, int b, int c]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        int? this[int a, int b, int c, int d]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        void NullCheck()
        {
            if (this[0] != null) { }
            if (this[1, 1] != null) { }
            if (this[2, 2, 2] != null) { }
            if (this[3, 3, 3, 3] != null) { }
        }

        void AssigningNull()
        {
            this[0] = null;
            this[1, 1] = null;
            this[2, 2, 2] = default;
            this[3, 3, 3, 3] = default;
        }

        void AssigningNonNullable()
        {
            this[1, 1] = this[0];
            this[3, 3, 3, 3] = this[2, 2, 2];
        }

        void AssigningNullable()
        {
            this[0] = this[1, 1];
        }

        void Dereferencing()
        {
            Console.WriteLine(this[0].Length);
            Console.WriteLine(this[1, 1].Length);
            Console.WriteLine(this[2, 2, 2].ToString());
            Console.WriteLine(this[3, 3, 3, 3].ToString());
        }
    }

    class Generic<T>
    {
        T this[int a]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        T? this[int a, int b]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        void NullCheck()
        {
            if (this[0] != null) { }
        }

        void AssigningNull()
        {
            this[0] = default;
        }

        void Dereferencing()
        {
            Console.WriteLine(this[0].ToString());
        }
    }

    class GenericForReference<T> where T : class
    {
        T this[int a]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        T? this[int a, int b]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        void NullCheck()
        {
            if (this[0] != null) { }
            if (this[1, 1] != null) { }
        }

        void AssigningNull()
        {
            this[0] = null;
            this[1, 1] = null;
        }

        void AssigningNonNullable()
        {
            this[1, 1] = this[0];
        }

        void AssigningNullable()
        {
            this[0] = this[1, 1];
        }

        void Dereferencing()
        {
            Console.WriteLine(this[0].ToString());
            Console.WriteLine(this[1, 1].ToString());
        }
    }

    class GenericForReferenceNullable<T> where T : class?
    {
        T this[int a]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        T? this[int a, int b]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        void NullCheck()
        {
            if (this[0] != null) { }
        }

        void AssigningNull()
        {
            this[0] = null;
        }

        void Dereferencing()
        {
            Console.WriteLine(this[0].ToString());
        }
    }

    class GenericForValue<T> where T : struct
    {
        T this[int a]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        T? this[int a, int b]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        void NullCheck()
        {
            if (this[1, 1] != null) { }
        }

        void AssigningNull()
        {
            this[0] = default;
            this[1, 1] = default;
        }

        void AssigningNonNullable()
        {
            this[1, 1] = this[0];
        }

        void Dereferencing()
        {
            Console.WriteLine(this[0].ToString());
            Console.WriteLine(this[1, 1].ToString());
        }
    }

    class GenericNotNull<T> where T : notnull
    {
        T this[int a]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        T? this[int a, int b]
        { 
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        void NullCheck()
        {
            if (this[0] != null) { }
        }

        void AssigningNull()
        {
            this[0] = default;
        }

        void Dereferencing()
        {
            Console.WriteLine(this[0].ToString());
        }
    }
}