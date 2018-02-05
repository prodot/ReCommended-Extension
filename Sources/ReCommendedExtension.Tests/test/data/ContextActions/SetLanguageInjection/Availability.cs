using System.Collections.Generic;

namespace Test
{
    public class Availability
    {
        strin{off}g fiel{off}d;

        string field2 = "{ \"one\": n{on}ull }";

        string field3 = "{ \"one\": null{on} }", field3 = "{ \"one\": null{on} }";

        const string constant = "<root {on}/>";

        v{off}oid Metho{off}d(strin{off}g p{off}arameter = "<html {on}/>")
        {
            strin{off}g variabl{off}e;

            var variable2 = "<html {on}/>";

            var variable3 = "<html {on}/>", variable3 = "<html {on}/>";

            variable = ".foo { color: #abcdef {on}}";

            const string localConstant = "<html {on}/>";

            const string localConstant2 = "<html {on}/>", localConstant3 = "<html {on}/>";

            field = "<html {on}/>";

            Property3 = "<html {on}/>";

            this[3] = "<html {on}/>";

            var obj = new Availability();
            obj.Property3 = "<html {on}/>";

            obj[2] = "<html {on}/>";

            var obj2 = new Availability { Property3 = "<html {on}/>" };

            var array = new[] { "<html {on}/>", "{ \"one\": null {on}}" };

            var list = new List<string> { "<html {on}/>", "{ \"one\": null {on}}" };

            list.Add("<html {on}/>");

            var concatenation = "<root>{on}" + "<{on}node />" + "</root>";

            void LocalFunction(strin{off}g localFunctionP{off}arameter = "<html {on}/>" ) { }

            var tuple = ("<html {on}/>", 0);
        }

        string this[int index{off}]
        {
            get => "<html {on}/>";
            set { }
        }

        void ExpressionBodiedMember() => field = "<html {on}/>";

        string Property { get; } = "<root {on}/>";

        string Property3 { get; set; } = "<root {on}/>";
    }
}