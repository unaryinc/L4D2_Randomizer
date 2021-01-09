using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Unary.L4D2_Randomizer.Abstract;
using Unary.L4D2_Randomizer.Structs;

namespace Unary.L4D2_Randomizer.Systems
{
    public class CommandManager : ISystem
    {
        private Categories Categories;
        private Random Random;
        private int Chance;
        private int PollDelay;
        private List<Command> Commands;
        private Queue<Command> CommandOrder;

        public override void Init()
        {
            Categories = Sys.Ref.Get<Categories>();
            Random = Sys.Ref.Get<Random>();
            Chance = Sys.Ref.Get<Config>().Get<int>("Chance");
            PollDelay = Sys.Ref.Get<Config>().Get<int>("PollDelay");
            Commands = JsonConvert.DeserializeObject<List<Command>>(File.ReadAllText("Commands.json"));
            CommandOrder = new Queue<Command>();

            foreach(var Command in Commands.OrderBy(x => Random.Next()).ToList())
            {
                CommandOrder.Enqueue(Command);
            }
        }

        private void Process(Command TargetCommand)
        {
            Dictionary<string, string> Replacement = new Dictionary<string, string>();

            foreach(var Category in Categories.Processors)
            {
                if(TargetCommand.Start.Contains("${" + Category + "}"))
                {
                    Replacement["${" + Category.Key + "}"] = Categories.GetRandomEntry(Category.Key);
                }
            }

            if(TargetCommand.Start.Contains("${fvalue}"))
            {
                Replacement["${fvalue}"] = Random.Next(TargetCommand.Min, TargetCommand.Max).ToString("F1");
            }
            if(TargetCommand.Start.Contains("${ivalue}"))
            {
                Replacement["${ivalue}"] = Random.Next((int)TargetCommand.Min, (int)TargetCommand.Max).ToString();
            }

            foreach (var Replace in Replacement)
            {
                TargetCommand.Start = TargetCommand.Start.Replace(Replace.Key, Replace.Value);
            }

            Sys.Ref.Events.Invoke("DispatchCommand", TargetCommand.Start);
            Sys.Ref.Events.Invoke("DispatchCommand", "sm_printf \\x05Randy\\x04:\\x03 " + TargetCommand.StartText);


            if (TargetCommand.Duration != 0)
            {
                foreach (var Replace in Replacement)
                {
                    TargetCommand.Finish = TargetCommand.Finish.Replace(Replace.Key, Replace.Value);
                }

                Task.Run(async () =>
                {
                    await Task.Delay(PollDelay * TargetCommand.Duration + 16);
                    Sys.Ref.Events.Invoke("DispatchCommand", TargetCommand.Finish);
                    Sys.Ref.Events.Invoke("DispatchCommand", "sm_printf \\x05Randy\\x04:\\x02 " + TargetCommand.FinishText);
                });
            }
        }

        public override void Poll()
        {
            if (Random.Next(0, Chance) == 0)
            {
                if(CommandOrder.Count == 0)
                {
                    foreach (var Command in Commands.OrderBy(x => Random.Next()).ToList())
                    {
                        CommandOrder.Enqueue(Command);
                    }
                }

                Process(CommandOrder.Dequeue());
            }
        }
    }
}
