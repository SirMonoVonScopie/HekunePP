using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hekune.Objects;
using Hekune.Math;

namespace Hekune.Objects
{
    class goGrid : graphicalObject {
        public override void render(Graphics g) {
            for (int z = -8; z < 8; z++) {
                for (int x = -8; x < 8; x++) {
                    g.DrawLine(new Pen(new SolidBrush(Color.DarkGray), 1),
                        new vector(x, 1, z).ToD(), new vector(x, 1, z + 1).ToD());
                }
            }
            for (int z = -8; z < 8; z++) {
                for (int x = -8; x < 8; x++) {
                    g.DrawLine(new Pen(new SolidBrush(Color.DarkGray), 1),
                        new vector(x, 1, z).ToD(), new vector(x + 1, 1, z).ToD());
                }
            }
        }
    }
}
