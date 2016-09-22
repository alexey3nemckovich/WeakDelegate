using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeakDelegate;
using System.Threading;

namespace WeakDelegateTest
{

    [TestClass]
    public class WeakDelegateTest
    {

        [TestMethod]
        public void TestTotalMemoryDecrease()
        {
            EventSource eventSource = new EventSource();
            EventListener eventListener = new EventListener();
            eventSource.Completed += (Action<int>)new WeakDelegate.WeakDelegate((Action<int>)eventListener.EventHandler).Weak;
            long totalMemoryBeforeCollect = GC.GetTotalMemory(true);
            eventListener = null;
            Thread.Sleep(1);
            long totalMemoryAfterCollect = GC.GetTotalMemory(true);
            GC.WaitForFullGCComplete(100);
            GC.WaitForPendingFinalizers();
            Assert.AreEqual(true ,totalMemoryBeforeCollect > totalMemoryAfterCollect);
            Console.WriteLine("Total memory before collect: {0}", totalMemoryBeforeCollect);
            Console.WriteLine("Total memory after collect: {0}", totalMemoryAfterCollect);
        }

        [TestMethod]
        public void TestWeakReferenceIsDead()
        {
            EventListener eventListener = new EventListener();
            WeakDelegate.WeakDelegate weakDelegate = new WeakDelegate.WeakDelegate((Action<int>)eventListener.EventHandler);
            EventSource eventSource = new EventSource();
            eventSource.Completed += (Action<int>)weakDelegate.Weak;
            eventListener = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForFullGCComplete(100);
            GC.WaitForPendingFinalizers();
            Assert.AreEqual(false, weakDelegate.weakReferenceToTarget.IsAlive);
        }

    }

}