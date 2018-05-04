using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hekune.Core;
using Hekune.Objects;

namespace Hekune
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
            projectInfo.addGraphicalObject(new goGrid());
        }

        private void tRoutineTrigger_Tick(object sender, EventArgs e)
        {
            cadpPanel.routineListener();
        }

        private void tbFPS_Scroll(object sender, EventArgs e)
        {
            tRoutineTrigger.Interval = Convert.ToInt16(1000 / (tbFPS.Value * 0.1));
        }

        private void tbFishEye_Scroll(object sender, EventArgs e)
        {
            projectInfo.mainCamera.depthStretch = tbFishEye.Value * 0.005 + 1;
            cadpPanel.Invalidate();
        }
    }
}
