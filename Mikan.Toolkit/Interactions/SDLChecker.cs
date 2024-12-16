using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikan.Toolkit.Interactions
{
    public abstract class SDLChecker
    {
        public static bool IsSDLInitialized { get; set; } = false;
    }
}
