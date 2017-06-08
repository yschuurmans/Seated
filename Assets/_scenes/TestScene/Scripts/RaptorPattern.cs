using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.TestScene.Scripts
{
    public abstract class RaptorPattern
    {
        
        protected ushort[,] RaptorHead;
        protected ushort[,] RaptorBack;
        protected ushort[,] RaptorSeat;

        public IEnumerator RunPattern(/*float durationInSeconds = DefaultDurationInSeconds, float delayInSeconds = 0*/)
        {
            yield break;
        }
    }
}
