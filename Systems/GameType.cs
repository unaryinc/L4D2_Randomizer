using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unary.L4D2_Randomizer.Abstract;

namespace Unary.L4D2_Randomizer.Systems
{
    public class GameType : ISystem
    {
        public bool IsSecondGame = true;
        public bool IsVersus = false;

        public override void Init()
        {
            Sys.Ref.Events.Subscribe("SetGameType", "OnGameType", this);
        }

        public void OnGameType(bool NewSecondGame, bool NewVersus)
        {
            IsSecondGame = NewSecondGame;
            IsVersus = NewVersus;
        }
    }
}
