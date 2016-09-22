using System;

namespace WeakDelegate
{
    
    public class EventSource
    {

        public event Action<int> Completed;
        public event Action<int, double> Completed1;
        public event Action<int, double, int> Completed2;
        public event Action<int, int, int, int> Completed3;

        public unsafe void callEventCompleted()
        {
            Completed.Invoke(1);
        }

        public void callEventCompleted1()
        {
            Completed1.Invoke(1, 2);
        }

        public void callEventCompleted2()
        {
            Completed2.Invoke(1, 2, 3);
        }

        public void callEventCompleted3()
        {
            Completed3.Invoke(1, 2, 3, 4);
        }

    }

}