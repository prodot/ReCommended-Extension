using System;
using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        /// <summary>
        /// Line 1
        /// Line 2 <see cref="Version"/>
        /// Line 3
        /// </summary>
        /// <typeparam name="T">Typeparam text</typeparam>
        /// <param name="x">first parameter</param>
        /// <param name="y">
        ///     second parameter
        /// </param>
        /// <returns>tr{on}ue or false</returns>
        /// <exception cref="InvalidOperationException">Occurs if...</exception>
        /// <remarks>
        /// Remarks 1
        /// Remarks 2 <typeparamref name="T"/>
        /// Remarks 3 <paramref name="x"/>
        /// </remarks>
        /// <example>An example.</example>
        /// <seealso cref="Availability"/>
        /// <seealso href="https://github.com"/>
        public bool Method<T>(int x, int y) => true;

        // The above method returns tr{off}ue or false.
    }

    public class Availability_PrimaryConstructors
    {
        /// <summary>
        /// {on}
        /// </summary>
        public class Parameterless() { }

        /// <summary>
        /// {on}
        /// </summary>
        /// <param name="q"></param>
        public class WithParameter(int q) { }
    }

    public class NonAvailability
    {
        /// <summary>{off}</summary>#
        /// <remarks />
        public void TextBetweenTopLevelTags() { }
    }

    public class NonAvailability_Summary
    {
        /// <summary>{off}</summary>
        /// <summary />
        public void MultipleSummaryTags() { }

        /// <summary attr="value">{off}</summary>
        public void SummaryTagWithAttribute() { }
    }

    public class NonAvailability_TypeParameter
    {
        /// <typeparam name="T">{off}</typeparam>
        public void NoTypeParameters() { }

        /// <typeparam name="T" name="U">{off}</typeparam>
        public void DuplicateTypeParameterName<T>() { }

        /// <typeparam name="T" unknown="U">{off}</typeparam>
        public void UnknownAttribute<T>() { }

        /// <typeparam>{off}</typeparam>
        public void MissingTypeParameterName<T>() { }

        /// <typeparam name="U">{off}</typeparam>
        public void UnknownTypeParameter<T>() { }

        /// <typeparam name="T">{off}</typeparam>
        /// <typeparam name="T"></typeparam>
        public void DuplicateTypeParameter<T>() { }
    }

    public class NonAvailability_Parameter
    {
        /// <param name="x">{off}</param>
        public void NoParameters() { }

        /// <param name="x" name="y">{off}</param>
        public void DuplicateParameterName(int x) { }

        /// <param name="x" unknown="y">{off}</param>
        public void UnknownAttribute(int x) { }

        /// <param>{off}</param>
        public void MissingParameterName(int x) { }

        /// <param name="y">{off}</param>
        public void UnknownParameter(int x) { }

        /// <param name="x">{off}</param>
        /// <param name="x"></param>
        public void DuplicateParameter(int x) { }
    }

    public class NonAvailability_Returns
    {
        /// <returns>{off}</returns>
        /// <returns />
        public int MultipleReturnsTags() => 3;

        /// <returns attr="value">{off}</returns>
        public int ReturnsTagWithAttribute() => 3;
    }

    public class NonAvailability_Value
    {
        /// <value>{off}</value>
        /// <value />
        public int MultipleValueTags => 3;

        /// <value attr="value">{off}</value>
        public int ValueTagWithAttribute => 3;
    }

    public class NonAvailability_Exception
    {
        /// <exception cref="InvalidOperationException" cref="SystemException">{off}</exception>
        public void DuplicateExceptionCref() { }

        /// <exception>{off}</exception>
        public void MissingExceptionCref() { }

        /// <exception cref="InvalidOperationException" name="x">{off}</exception>
        public void UnknownAttribute() { }
    }

    public class NonAvailability_Remarks
    {
        /// <remarks>{off}</remarks>
        /// <remarks />
        public void MultipleRemarksTags() { }

        /// <remarks attr="value">{off}</remarks>
        public void RemarksTagWithAttribute() { }
    }

    public class NonAvailability_Example
    {
        /// <example>{off}</example>
        /// <example />
        public void MultipleExampleTags() { }

        /// <example attr="value">{off}</example>
        public void ExampleTagWithAttribute() { }
    }

    public class NonAvailability_Seealso
    {
        /// <seealso cref="InvalidOperationException" cref="SystemException">{off}</seealso>
        public void DuplicateSeealsoCref() { }

        /// <seealso href="https://github.com" href="https://jetbrains.com">{off}</seealso>
        public void DuplicateSeealsoHref() { }

        /// <seealso cref="InvalidOperationException" href="https://github.com">{off}</seealso>
        public void DuplicateSeealsoCrefAndHref() { }

        /// <seealso>{off}</seealso>
        public void MissingSeealsoCrefOrHref() { }

        /// <seealso cref="InvalidOperationException" name="x">{off}</exception>
        public void UnknownAttribute() { }
    }
}