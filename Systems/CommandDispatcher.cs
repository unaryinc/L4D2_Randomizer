using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using WindowsInput;

using Unary.IOManager;

using Unary.L4D2_Randomizer.Abstract;

namespace Unary.L4D2_Randomizer.Systems
{
    public class CommandDispatcher : ISystem
    {
        InputSimulator IOTest;
        List<string> CommandQueue;
        string OutputPath;

        public override void Init()
        {
            IOTest = new InputSimulator();
            CommandQueue = new List<string>();
            OutputPath = Sys.Ref.Get<Config>().Get<string>("OutputPath");
            Sys.Ref.Events.Subscribe("DispatchCommand", nameof(Enqueue), this);
        }

        public override void Poll()
        {
            string Result = default;

            for(int i = 0; i < CommandQueue.Count;++i)
            {
                string Target = CommandQueue[i];
                //Sys.Ref.Console.Message(Target);
                Result += Target + Environment.NewLine;
                //Result += "scripted_user_func say,[" + Target.Replace("scripted_user_func ", "").Replace(" ", ",") + ']' + Environment.NewLine;
            }

            CommandQueue.Clear();

            File.WriteAllText(OutputPath, Result);
            Sys.Ref.IO.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F7);
        }

        public void Enqueue(string Command)
        {
            CommandQueue.Add(Command);
        }
    }
}
