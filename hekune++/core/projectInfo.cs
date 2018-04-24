using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hekune.Objects;

namespace Hekune.Core {
    static class projectInfo {
        static private List<graphicalObject> graphicalObjects = new List<graphicalObject>();
        static public Size halfCanvasSize = new Size();
        static public camera mainCamera = new camera();

        static public void renderGO_List(PaintEventArgs e) {
            for (int i = 0; i < graphicalObjects.Count; i++) {
                graphicalObjects.ElementAt(i).render(e.Graphics);
            }
        }

        static public void addGraphicalObject(graphicalObject gO) {
            graphicalObjects.Add(gO);
        }
    }
}
