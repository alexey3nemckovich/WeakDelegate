using System;
using System.Runtime.InteropServices;

namespace WeakDelegate
{

    class Program
    {
        
        static void Main(string[] args)
        {
            EventSource eventSource = new EventSource();
            EventListener eventListener = new EventListener();

            eventSource.Completed += (Action<int>)new WeakDelegateDynamicMethod((Action<int>)eventListener.EventHandler).Weak;
            eventSource.callEventCompleted();
            
            eventSource.Completed += (Action<int>)new WeakDelegate((Action<int>)eventListener.EventHandler).Weak;
            eventSource.Completed1 += (Action<int, double>)new WeakDelegate((Action<int, double>)eventListener.EventHandler).Weak;
            eventSource.Completed2 += (Action<int, double, int>)new WeakDelegate((Action<int, double, int>)eventListener.EventHandler).Weak;
            eventSource.Completed3 += (Action<int, int, int, int>)new WeakDelegate((Action<int, int, int, int>)eventListener.EventHandler).Weak;
            eventSource.callEventCompleted();
            eventSource.callEventCompleted1();
            eventSource.callEventCompleted2();
            eventSource.callEventCompleted3();
            
            Console.Read();
        }

    }

}