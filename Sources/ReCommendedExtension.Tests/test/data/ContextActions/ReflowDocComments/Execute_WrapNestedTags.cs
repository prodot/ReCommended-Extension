using System;

namespace Test
{
    public class Execute
    {
        /// <summary>
        ///
        /// 
        /// {caret}
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns><see cref="Execute"/>s</returns>
        /// <remarks>   Lorem ipsum   dolor sit amet consequat duo elitr justo feugait molestie augue in sed ea facilisi
        /// vero hendrerit. Gubergren ut diam <paramref name="x"/> dolor vel et dolor eu in takimata nibh
        /// dolores ut.
        /// <para/>Eirmod vero rebum sit adipiscing sed lorem facilisi magna.<br> </br><list type="bullet"><listheader/><item>Lorem</item><item>ipsum</item></list>
        /// <list   type="table">  <listheader> <term>Lorem</term><description>ipsum</description></listheader>
        /// <item><term>dolor</term><description/></item>
        /// </list><c>Lorem ipsum dolor sit amet consequat duo elitr justo feugait molestie augue in sed ea facilisi vero hendrerit.
        /// Gubergren ut diam dolor vel et dolor eu in takimata nibh dolores ut.</c>
        /// <code>Lorem ipsum dolor sit amet consequat
        /// duo elitr justo feugait molestie
        /// augue in sed ea
        /// </code><code>
        /// <![CDATA[ <html /> ]]>
        /// </code><see cref="Execute">Lorem ipsum dolor sit amet</see>, <see href="https://github.com"/>, <see href="https://jetbrains.com"/>
        /// <custom_1>Lorem ipsum dolor sit amet consequat duo elitr justo feugait molestie augue in sed ea</custom_1>
        /// <custom_2   custom_attribute="custom attribute value">
        ///     Lorem ipsum dolor sit amet consequat duo elitr justo feugait molestie augue in sed ea
        ///     facilisi vero hendrerit. Gubergren ut diam dolor vel et dolor eu in takimata nibh
        ///     dolores <custom_2_1/>ut.
        /// </custom_2>
        /// <custom_3 a="1"  b="2"></custom_3><custom_4/>
        /// </remarks>
        public bool Method<T, U>(int x, int y) => true;
    }
}