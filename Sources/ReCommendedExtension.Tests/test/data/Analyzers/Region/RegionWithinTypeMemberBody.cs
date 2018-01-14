namespace Test
{
    public class Class
    {
        #region region within a type

        void M() { }

        void M2() { }

        #endregion

        void Method()
        {
            #region region within a method

            M();
            M2();

            #endregion
        }

        int Property
        {
            get
            {
                #region region within a property getter

                M();
                M2();

                #endregion

                return 0;
            }
            set
            {
                #region region within a property setter

                M();
                M2();

                #endregion
            }
        }

        string this[int index]
        {
            get
            {
                #region region within an indexer getter

                M();
                M2();

                #endregion

                return null;
            }
            set
            {
                #region region within an indexer setter

                M();
                M2();

                #endregion
            }
        }

        Class()
        {
            #region region within a constructor

            M();
            M2();

            #endregion

            Action action = () =>
            {
                #region region within a lambda

                M();
                M2();

                #endregion
            };

            void LocalFunction()
            {
                #region region within a local function

                M();
                M2();

                #endregion
            }
        }

        ~Class()
        {
            #region region within a destructor

            M();
            M2();

            #endregion
        }

        public static bool operator ==(Class x, Class y)
        {
            #region region within an operator

            M();
            M2();

            #endregion
        }

        event EventHandler Changed
        {
            add
            {
                #region region within an event

                M();
                M2();

                #endregion
            }
            remove
            {
                #region region within an event

                M();
                M2();

                #endregion
            }
        }
    }
}