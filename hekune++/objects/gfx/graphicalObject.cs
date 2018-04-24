using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Hekune.Objects
{
    abstract class graphicalObject
    {
        abstract public void render(Graphics g);
    }
}
