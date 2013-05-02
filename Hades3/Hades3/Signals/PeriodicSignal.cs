using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public abstract class PeriodicalSignal : Signal
    {
        protected List<SignalWave> signalWaves;
        public List<SignalWave> SignalWaves
        {
            get
            {
                return signalWaves;
            }
        }

        private int currentTime;
        protected int CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
            }
        }

        private int growTime;
        public int GrowTime
        {
            get
            {
                return growTime;
            }
        }

        protected int maxRange;
        protected int strength;

        public PeriodicalSignal(TissueCell parentCell, int strength, int maxRange, int growTime) : base(parentCell)
        {
            this.maxRange = maxRange;
            this.growTime = growTime;
            this.strength = strength;

            signalWaves = new List<SignalWave>();
            SignalWave newWave = new SignalWave(this, maxStrength, maxRange);
            signalWaves.Add(newWave);
            currentTime = 1;

            affectedLocations.UnionWith(newWave.CurrentPosition);
            affectedLocations.UnionWith(newWave.PreviousPosition);
            foreach (Vector3 affectedLoc in affectedLocations)  // step through every new location
            {
                if (Environment.Instance.GoodPoint(affectedLoc))  // if its valid
                    Environment.Instance.GetContentsAt(affectedLoc).SignalsAffectingThisLocation.Add(this);       // add this signal from that location
            }
        }

        public override void Tick()
        {
            while (currentTime < growTime)
            {
                signalWaves.Add(new SignalWave(this, maxStrength, maxRange));
                currentTime++;
            }

            foreach (Vector3 affectedLoc in affectedLocations)  // step through every old location
            {
                if (Environment.Instance.GoodPoint(affectedLoc))  // if its valid
                    Environment.Instance.GetContentsAt(affectedLoc).SignalsAffectingThisLocation.Remove(this);       // remove this signal from that location
            }
            affectedLocations.Clear();

            signalWaves.RemoveAll(item => item.CurrRange > item.MaxRange);
            foreach (SignalWave wave in signalWaves)
            {
                wave.Tick();
                affectedLocations.UnionWith(wave.CurrentPosition);
                affectedLocations.UnionWith(wave.PreviousPosition); // not sure why this is necessary
            }

            if (signalWaves.Count == 0)
            {
                Environment.Instance.DeleteSignal(this);
                return;
            }

            foreach (Vector3 affectedLoc in affectedLocations)  // step through every new location
            {
                if (Environment.Instance.GoodPoint(affectedLoc))  // if its valid
                    Environment.Instance.GetContentsAt(affectedLoc).SignalsAffectingThisLocation.Add(this);       // add this signal from that location
            }
        }

    }
}
