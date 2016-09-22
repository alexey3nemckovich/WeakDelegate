using System;

namespace WeakDelegate
{

    public class EventListener
    {

        public void EventHandler(int param1)
        {
            Console.Write(param1.GetType() + " ");
            Console.WriteLine();
        }

        public void EventHandler(int param1, double param2)
        {
            Console.Write(param1.GetType() + " ");
            Console.Write(param2.GetType() + " ");
            Console.WriteLine();
        }

        public void EventHandler(int param1, double param2, int param3)
        {
            Console.Write(param1.GetType() + " ");
            Console.Write(param2.GetType() + " ");
            Console.Write(param3.GetType() + " ");
            Console.WriteLine();
        }

        public void EventHandler(int param1, int param2, int param3, int param4)
        {
            Console.Write(param1.GetType() + " ");
            Console.Write(param2.GetType() + " ");
            Console.Write(param3.GetType() + " ");
            Console.Write(param4.GetType() + " ");
            Console.WriteLine();
        }

        ~EventListener()
        {
            Console.WriteLine("Event listener destroyed");
        }

    }

}