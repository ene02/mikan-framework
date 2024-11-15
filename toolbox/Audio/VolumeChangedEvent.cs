using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Audio
{
    public class VolumeChangedEvent : EventArgs
    {
        public float Volume;

        public VolumeChangedEvent(float newVolume)
        {
            this.Volume = newVolume;
        }
    }
}
