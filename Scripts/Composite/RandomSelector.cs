using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;



namespace NPBehave
{
    public class RandomSelector : Composite
    {
        static System.Random rng = new System.Random();

#if UNITY_EDITOR
        static public void DebugSetSeed( int seed )
        {
            rng = new System.Random( seed );
        }
#endif

        private int currentIndex = -1;
        private int[] randomizedOrder;

        public RandomSelector( params Node[] children ) : base( "Random Selector", children )
        {
            randomizedOrder = new int[ children.Length ];
            for ( int i = 0; i < Children.Length; i++ )
            {
                randomizedOrder[ i ] = i;
            }
        }


        protected override void DoStart()
        {
            foreach ( Node child in Children )
            {
                Assert.AreEqual( child.CurrentState, State.INACTIVE );
            }

            currentIndex = -1;

            // Shuffling
            int n = randomizedOrder.Length;
            while ( n > 1 )
            {
                int k = rng.Next( n-- );
                int temp = randomizedOrder[ n ];
                randomizedOrder[ n ] = randomizedOrder[ k ];
                randomizedOrder[ k ] = temp;
            }

            ProcessChildren();
        }



        protected override void DoStop()
        {
            Children[ currentIndex ].Stop();
        }

        protected override void DoChildStopped( Node child, bool result )
        {
            if ( result )
            {
                Stopped( true );
            }
            else
            {
                ProcessChildren();
            }
        }

        private void ProcessChildren()
        {
            if ( ++currentIndex < Children.Length )
            {
                if ( IsStopRequested )
                {
                    Stopped( false );
                }
                else
                {
                    Children[ randomizedOrder[ currentIndex ] ].Start();
                }
            }
            else
            {
                Stopped( false );
            }
        }

        public override void StopLowerPriorityChildrenForChild( Node abortForChild, bool immediateRestart )
        {
            int indexForChild = 0;
            bool found = false;
            foreach ( Node currentChild in Children )
            {
                if ( currentChild == abortForChild )
                {
                    found = true;
                }
                else if ( !found )
                {
                    indexForChild++;
                }
                else if ( found && currentChild.IsActive )
                {
                    if ( immediateRestart )
                    {
                        currentIndex = indexForChild - 1;
                    }
                    else
                    {
                        currentIndex = Children.Length;
                    }
                    currentChild.Stop();
                    break;
                }
            }
        }

        override public string ToString()
        {
            return base.ToString() + "[" + this.currentIndex + "]";
        }
    }
}
