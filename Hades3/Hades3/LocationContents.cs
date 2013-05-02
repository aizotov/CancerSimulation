using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    public class LocationContents
    {
        private Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        private List<TissueCell> tissuecells;
        public List<TissueCell> TissueCells
        {
            get
            {
                return tissuecells;
            }
        }

        public List<EndothelialCell> pipes;
        public List<EndothelialCell> Pipes
        {
            get
            {
                return pipes;
            }
        }

        private List<Signal> signalsAffectingThisLocation;
        public List<Signal> SignalsAffectingThisLocation
        {
            get
            {
                return signalsAffectingThisLocation;
            }
        }

        private int pressure;
        public int Pressure
        {
            get
            {
                return pressure;
            }
            set
            {
                pressure = value;
            }
        }

        public LocationContents(Vector3 position)
        {
            this.position = position;
            tissuecells = new List<TissueCell>();
            pipes = new List<EndothelialCell>();
            signalsAffectingThisLocation = new List<Signal>();
        }

        public List<PleaseMoveSignal> GetPushSignals()
        {
            List<PleaseMoveSignal> pushSignals = new List<PleaseMoveSignal>();
            foreach (Signal s in signalsAffectingThisLocation)
            {
                if (s.GetType().Equals(typeof(PleaseMoveSignal)))
                    pushSignals.Add((PleaseMoveSignal)s);
            }
            return pushSignals;
        }

        public bool IsPushPresent()
        {
            foreach (Signal s in signalsAffectingThisLocation)
            {
                if (s.GetType().Equals(typeof(PleaseMoveSignal)))
                    return true;
            }
            return false;
        }

        public bool IsPipePresent()
        {
            if (pipes.Count == 0)
                return false;
            return true;
        }

    }
}