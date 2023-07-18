// See https://aka.ms/new-console-template for more information
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace consoledemo
{
    class Program
    {

        private static AutoResetEvent _exitEvent;

        static async Task Main(string[] args)
        {
            // 确保只有一个实例在运行
            bool isRuned;
            Mutex mutex = new Mutex(true, "OnlyRunOneInstance", out isRuned);

            if (!isRuned)
            {
                return;
            }


            // 后台等待
            _exitEvent = new AutoResetEvent(false);

            await DoWork(_exitEvent);


            _exitEvent.WaitOne();
        }

        private static async Task DoWork(AutoResetEvent autoResetEvent)
        {
            await Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (i < 10)
                {
                    Thread.Sleep(500);

                    Console.WriteLine($"current count is {i}");
                    i++;
                }
                autoResetEvent.Set();
                Console.WriteLine($"autoResetEvent set");
            });
        }
    }
}

