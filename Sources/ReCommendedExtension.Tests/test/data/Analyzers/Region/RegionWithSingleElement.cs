namespace Test
{
    public class Class
    {
        #region region with one member

        void M() { }

        #endregion

        #region region with more members

        void M2() { }

        void M3() { }

        #endregion

        void Method()
        {
            #region region with one statement

            M();

            #endregion

            #region region with more statements

            M2();
            M3();

            #endregion
        }

        #region region with one nested region

        #region nested region with one member

        void M4() { }

        #endregion

        #endregion

        #region region with more nested regions

        #region nested region with one member

        void M5() { }

        #endregion

        #region nested region with more members

        void M6() { }

        void M7() { }

        #endregion

        #endregion
    }

    #region region with one type

    class Class2 { }

    #endregion

    #region region with more types

    class Class3 { }

    class Class4 { }

    #endregion
}