using JetBrains.Annotations;
// ReSharper disable AssignNullToNotNullAttribute

namespace Test
{
    public class LocalSuppression
    {
        [NotNull]
        string text;

        /// <summary>
        /// the summary.
        /// </summary>
        void Method()
        {
            //    ReSharper disable once AssignNullToNotNullAttribute
            text = null;

            // 		ReSharper disable once AssignNullToNotNullAttribute
            text = null;

            // ReSharper disable AssignNullToNotNullAttribute
            text = null;
            // ReSharper restore AssignNullToNotNullAttribute

            // sample comment
        }
    }
}