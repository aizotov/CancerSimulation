using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class PleaseMoveSignal : PeriodicalSignal
    {
        public static int PleaseMoveSignalMaxRange = 2;
        public static int PleaseMoveSingnalGrowTime = 1;

        public PleaseMoveSignal(TissueCell parentCell, int range, int strength)
            : base(parentCell, strength, range, PleaseMoveSingnalGrowTime)
        {

        }
    }
}
