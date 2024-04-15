using System;

namespace Test
{
    public class Execute
    {
        /// <remarks></remarks>
        /// <typeparam name="U"/>
        /// <typeparam name="T"></typeparam>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <exception cref="InvalidOperationException">1</exception>
        /// <exception cref="InvalidOperationException">2</exception>
        /// <exception cref="InvalidOperationException">3</exception>
        /// <exception cref="InvalidOperationException">4</exception>
        /// <summary></summary>
        /// <returns></returns>
        /// <seealso cref="Execute"></seealso>
        /// <seealso href="https://github.com"/>
        /// <seealso href="https://jetbrains.com"></seealso>
        /// <custom_1></custom_1>
        /// <custom_2 custom_attribute="custom attribute value"></custom_2>{caret}
        /// <example></example>
        public bool Method<T, U>(int x, int y) => true;
    }
}