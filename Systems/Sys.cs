/*
MIT License
Copyright (c) 2020 Unary Incorporated
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using System.Threading;

using Unary.L4D2_Randomizer.Abstract;

namespace Unary.L4D2_Randomizer.Systems
{
    class Sys
    {
        public string Version = "0.0.0";

        public static Sys Ref { get; private set; }
        public static bool Running { get; set; } = true;

        public WindowsInput.InputSimulator IO;
        public Console Console { get; private set; }
        public Events Events { get; private set; }

        private List<string> Order;
        private Dictionary<string, ISystem> Systems;

        private int Delay;
        private bool IsPressed = false;
        private bool IsReleased = false;
        private bool ShouldRun;
        private Thread Thread;

        public void Init()
        {
            Ref = this;

            IO = new WindowsInput.InputSimulator();
            Console = new Console();
            Events = new Events();

            Order = new List<string>();
            Systems = new Dictionary<string, ISystem>();


            Thread = new Thread(PauseCheck);
            Thread.Start();
        }

        public void PauseCheck()
        {
            while(Running)
            {
                if (IO.InputDeviceState.IsKeyDown(WindowsInput.Native.VirtualKeyCode.F8))
                {
                    if(IsPressed == false)
                    {
                        IsPressed = true;
                        IsReleased = false;
                    }
                }
                else
                {
                    if(IsPressed == true)
                    {
                        IsPressed = false;
                        IsReleased = true;
                    }
                }

                if (IsReleased)
                {
                    IsReleased = false;

                    if (!ShouldRun)
                    {
                        Console.Color(true);
                        ShouldRun = true;
                        Events.Invoke("DispatchCommand", "sm_printf \\x05[STORYTELLER]\\x04 Picked: \\x03 Randy.");
                    }
                    else
                    {
                        Console.Color(false);
                        ShouldRun = false;
                        Events.Invoke("DispatchCommand", "sm_printf \\x05[STORYTELLER]\\x04 Pausing \\x03 Randy.");
                    }
                }

                Thread.Sleep(33);
            }
        }

        public void Clear()
        {
            Thread.Abort();
            for (int i = Order.Count - 1; i != 0; --i)
            {
                Systems[Order[i]].Clear();
                Systems.Remove(Order[i]);
                Order.RemoveAt(i);
            }
        }

        private void PostInit()
        {
            for (int i = 0; i < Order.Count; ++i)
            {
                Systems[Order[i]].PostInit();
            }
        }

        public void Add<T>(T NewSystem) where T : ISystem
        {
            string TypeName = NewSystem.GetType().Name;

            if (Systems.ContainsKey(TypeName))
            {
                Console.Error("Tried to add already registered system" + TypeName);
            }
            else
            {
                Order.Add(TypeName);
                Systems[TypeName] = NewSystem;
                Systems[TypeName].Init();
            }
        }

        public T Get<T>() where T : ISystem
        {
            string TypeName = typeof(T).Name;

            if (Systems.ContainsKey(TypeName))
            {
                return (T)Systems[TypeName];
            }
            else
            {
                Console.Error("Tried getting non-registered system " + TypeName);
                return default;
            }
        }

        public void Run()
        {
            Add(new Random());
            Add(new Assemblies());
            Add(new Config());
            Add(new GameType());
            Add(new Categories());
            Add(new CommandDispatcher());
            Add(new CommandManager());

            PostInit();

            Delay = Get<Config>().Get<int>("PollDelay");
        }

        public bool Poll()
        {
            if(ShouldRun)
            {
                for (int i = 0; i < Order.Count; ++i)
                {
                    Systems[Order[i]].Poll();
                }
            }

            Thread.Sleep(Delay);

            return Running;
        }
    }
}