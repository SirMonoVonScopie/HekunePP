using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hekune.Math
{
    public class transformator
    {
        #region fields
        public vector position = new vector();
        public double yaw = 0;
        public double pitch = 0;
        public double roll = 0;
        #endregion

        public vector forward()
        {
            return new Math.vector(0, 0, 1) * this * new Math.vector(-1, -1, 1);
        }

        public vector right()
        {
            return new Math.vector(1, 0, 0) * this * new Math.vector(1, -1, -1);
        }
    }
}
