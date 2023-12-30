using System;
using JetBrains.Annotations;

namespace Test
{
    internal class SomeClass
    {
        public void ParameterTypes(
            string p{on}0, 
            in string p{on}1, 
            ref readonly string p{off}2, 
            ref string p{off}3, 
            out string p{off}4) => throw new NotImplementedException();

        public void Collections(string[] p{on}0, params string[] p{on}1) { }
    }

    internal class Annotated
    {
        public virtual void ParameterTypes([IgnoreSpellingAndGrammarErrors] string p{off}0, [IgnoreSpellingAndGrammarErrors] in string p{off}1) { }

        public virtual void Collections([IgnoreSpellingAndGrammarErrors] string[] p{off}0, [IgnoreSpellingAndGrammarErrors] params string[] p{off}1) { }
    }

    internal class Derived : Annotated
    {
        public override void ParameterTypes(string p{off}0, in string p{off}1) { }

        public override void Collections(string[] p{off}0, params string[] p{off}1) { }
    }
}