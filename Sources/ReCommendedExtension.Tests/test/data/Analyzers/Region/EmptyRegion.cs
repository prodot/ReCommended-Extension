namespace Test
{
    public class Class
    {
        #region region without members

        #endregion

        #region region with members

        void M() { }

        void M2() { }

        #endregion

        void Method()
        {
            #region region without statements
            
            
            #endregion

            #region region with statements

            M();
            M2();

            #endregion
        }

        #region


        #region nested region without members

  
        #endregion

        #endregion

        #region

        #region nested region with members

        void M3() { }

        void M4() { }

        #endregion

        #endregion
    }

    #region region without types
    

    #endregion

    #region region with types

    class Class2 { }

    class Class3 { }

    #endregion
}