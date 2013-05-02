using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public class SignalWave
    {
        private HashSet<Vector3> currentPosition;
        public HashSet<Vector3> CurrentPosition
        {
            get
            {
                return currentPosition;
            }
        }

        private HashSet<Vector3> previousPosition;
        public HashSet<Vector3> PreviousPosition
        {
            get
            {
                return previousPosition;
            }
        }

        private PeriodicalSignal parentSignal;

        private double strength;
        public double Strength
        {
            get
            {
                return strength;
            }
        }

        private int maxRange;
        public int MaxRange
        {
            get
            {
                return maxRange;
            }
        }

        private int currRange;
        public int CurrRange
        {
            get
            {
                return currRange;
            }
        }

        public SignalWave(PeriodicalSignal parentSignal, double strength, int maxRange)
        {
            currentPosition = new HashSet<Vector3>();
            previousPosition = new HashSet<Vector3>();

            this.parentSignal = parentSignal;
            this.strength = strength;
            this.maxRange = maxRange;
            currRange = 0;

            currentPosition.Add(parentSignal.ParentCell.CellLocation);      // starts at location of cell
        }

        public void Tick()
        {
            currRange++;

            // gets all neighbors, will get points behind and in front of wave
            HashSet<Vector3> neighbors = new HashSet<Vector3>();
            foreach (Vector3 pos in currentPosition)
            {
                HashSet<Vector3> tmpNeighbrs = Environment.Instance.GetAllNeighborLocations(pos);
                neighbors.UnionWith(tmpNeighbrs);
            }

            neighbors.ExceptWith(currentPosition);     // removes locations behind wave, leaving only where the wave will go in the next tick
            neighbors.ExceptWith(previousPosition);

            previousPosition.Clear();
            previousPosition = new HashSet<Vector3>(currentPosition);

            currentPosition = neighbors;
        }

    }
}
