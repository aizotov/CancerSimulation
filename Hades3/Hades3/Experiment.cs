using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hades3
{
    public class Experiment
    {
        private Selection whichCells;
        public Selection WhichCells
        {
            get
            {
                return whichCells;
            }
        }

        private List<GeneType> mutations;
        public List<GeneType> Mutations
        {
            get
            {
                return mutations;
            }
        }

        private double modifier;
        public double Modifier
        {
            get
            {
                return modifier;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        public Experiment(Selection whichCells, List<GeneType> mutations, double modifier, string name)
        {
            this.whichCells = whichCells;
            this.mutations = mutations;
            this.modifier = modifier;
            this.name = name;
        }

        public enum GeneType
        {
            SpaceTolerance,
            SelfRepair, //todo
            SelfDeath   //todo
        }

        public enum Selection
        {
            All,
            AllBlast,
            AllFinal,
            OneBlast,
            OneFinal
        }
    }
}
