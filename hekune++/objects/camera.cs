using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hekune.Math;
using Hekune.Core;

namespace Hekune.Objects
{
    class camera
    {
        #region fields
        public transformator posNRot = new transformator();
        public double depthStretch = 1;
        #endregion
    }
}
