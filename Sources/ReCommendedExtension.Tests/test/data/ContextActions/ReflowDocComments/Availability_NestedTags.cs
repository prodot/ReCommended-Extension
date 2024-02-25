using System;
using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        /// <summary>
        /// {on}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <remarks>
        /// <paramref name="x"/><para/><br/>
        /// <list type="bullet">
        ///     <listheader>header</listheader>
        ///     <item>one</item>
        ///     <item>two</item>
        /// </list>
        /// <list type="table">
        ///     <listheader>
        ///         <term></term>
        ///         <description></description>
        ///     </listheader>
        ///     <item>
        ///         <term></term>
        ///         <description></description>
        ///     </item>
        ///     <item>
        ///         <term></term>
        ///         <description></description>
        ///     </item>
        /// </list>
        /// <c>one.two.three</c> and <typeparamref name="T"/>
        /// <code>
        ///     Console.WriteLine(0);
        /// </code>
        /// <see cref="Availability"/>, <see href="https://github.com"/>
        /// </remarks>
        public bool Method<T>(int x, int y) => true;

        // The above method returns tr{off}ue or false.
    }

    public class NonAvailability_ParamRef
    {
        /// <remarks>
        /// <paramref name="x" name="x"/>{off}
        /// </remarks>
        public void DuplicateName(int x) { }

        /// <remarks>
        /// <paramref name="x" unknown="y"/>{off}
        /// </remarks>
        public void UnknownAttribute(int x) { }

        /// <remarks>
        /// <paramref/>{off}
        /// </remarks>
        public void MissingName(int x) { }

        /// <remarks>
        /// <paramref name="x">{off}text</paramref>
        /// </remarks>
        public void NestedText(int x) { }

        /// <remarks>
        /// <paramref name="x">{off}<c></c></paramref>
        /// </remarks>
        public void NestedTag(int x) { }
    }

    public class NonAvailability_TypeParamRef
    {
        /// <remarks>
        /// <typeparamref name="T" name="U"/>{off}
        /// </remarks>
        public void DuplicateName<T>() { }

        /// <remarks>
        /// <typeparamref name="T" unknown="U"/>{off}
        /// </remarks>
        public void UnknownAttribute<T>() { }

        /// <remarks>
        /// <typeparamref/>{off}
        /// </remarks>
        public void MissingName<T>() { }

        /// <remarks>
        /// <typeparamref name="T">{off}text</paramref>
        /// </remarks>
        public void NestedText<T>() { }

        /// <remarks>
        /// <typeparamref name="T">{off}<c></c></paramref>
        /// </remarks>
        public void NestedTag<T>() { }
    }

    public class NonAvailability_Para
    {
        /// <remarks>
        /// <para attr="value"/>{off}
        /// </remarks>
        public void WithAttribute() { }
    }

    public class NonAvailability_BR
    {
        /// <remarks>
        /// <br attr="value"/>{off}
        /// </remarks>
        public void WithAttribute() { }

        /// <remarks>
        /// <br>{off}text</br>
        /// </remarks>
        public void NestedText() { }

        /// <remarks>
        /// <br>{off}<c></c></br>
        /// </remarks>
        public void NestedTag() { }
    }

    public class NonAvailability_C
    {
        /// <remarks>
        /// <c attr="value">{off}</c>
        /// </remarks>
        public void WithAttribute() { }

        /// <remarks>
        /// <c>{off}<br/></c>
        /// </remarks>
        public void NestedTag() { }
    }

    public class NonAvailability_Code
    {
        /// <remarks>
        /// <code attr="value">{off}</code>
        /// </remarks>
        public void WithAttribute() { }

        /// <remarks>
        /// <code>{off}<br/></code>
        /// </remarks>
        public void NestedTag() { }
    }    

    public class NonAvailability_See
    {
        /// <remarks>
        /// <see cref="InvalidOperationException" cref="SystemException"/>{off}
        /// </remarks>
        public void DuplicateCref() { }

        /// <remarks>
        /// <see href="https://github.com" href="https://jetbrains.com"/>{off}
        /// </remarks>
        public void DuplicateSeeHref() { }

        /// <remarks>
        /// <see cref="InvalidOperationException" href="https://github.com"/>{off}
        /// </remarks>
        public void DuplicateCrefAndHref() { }

        /// <remarks>
        /// <see/>{off}
        /// </remarks>
        public void MissingCrefOrHref() { }

        /// <remarks>
        /// <see name="x"/>{off}
        /// </remarks>
        public void UnknownAttribute() { }

        /// <remarks>
        /// <see cref="InvalidOperationException">{off}<c></c></see>
        /// </remarks>
        public void NestedTag() { }
    }

    public class NonAvailability_List
    {
        /// <remarks>
        /// <list type="bullet" type="table">{off}</list>
        /// </remarks>
        public void DuplicateType() { }

        /// <remarks>
        /// <list unknown="value">{off}</list>
        /// </remarks>
        public void UnknownAttribute() { }

        /// <remarks>
        /// <list>{off}</list>
        /// </remarks>
        public void MissingType() { }

        /// <remarks>
        /// <list type="bullet">{off}text</list>
        /// </remarks>
        public void NestedText() { }

        /// <remarks>
        /// <list type="bullet">{off}<custom/></list>
        /// </remarks>
        public void UnknownNestedTag() { }

        /// <remarks>
        /// <list type="bullet">{off}<listheader/><listheader/></list>
        /// </remarks>
        public void MultipleListHeaders() { }
    }

    public class NonAvailability_ListHeader
    {
        /// <remarks>
        /// <list type="bullet">
        ///     <listheader attr="x"/>{off}
        /// </list>
        /// </remarks>
        public void WithAttribute() { }
    }

    public class NonAvailability_Item
    {
        /// <remarks>
        /// <list type="bullet">
        ///     <item attr="x">{off}</item>
        /// </list>
        /// </remarks>
        public void WithAttribute() { }
    }

    public class NonAvailability_Term
    {
        /// <remarks>
        /// <list type="table">
        ///     <listheader>
        ///         <term attr="x">{off}</term>
        ///     </listheader>
        /// </list>
        /// </remarks>
        public void WithAttribute() { }
    }

    public class NonAvailability_Description
    {
        /// <remarks>
        /// <list type="table">
        ///     <listheader>
        ///         <description attr="x">{off}</description>
        ///     </listheader>
        /// </list>
        /// </remarks>
        public void WithAttribute() { }
    }
}