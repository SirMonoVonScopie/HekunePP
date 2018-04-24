using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hekune.Core;

namespace Hekune.UI
{
    class CAD_Panel : PictureBox
    {
        #region InputListening
        //Active Inputs
        bool mouseLeft = false;
        bool cameraMode = false;
        bool mouseRight = false;

        bool keyW = false;
        bool keyA = false;
        bool keyS = false;
        bool keyD = false;
        bool keyShift = false;
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            switch (e.Button) {
                case MouseButtons.Left:
                    mouseLeft = true;
                    this.Invalidate();
                    break;
                case MouseButtons.Middle:
                    cameraMode = true;
                    this.Focus();
                    Cursor.Hide();
                    break;
                case MouseButtons.Right:
                    mouseRight = true;
                    break;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            switch (e.Button) {
                case MouseButtons.Left:
                    mouseLeft = false;
                    break;
                case MouseButtons.Middle:
                    cameraMode = false;
                    Cursor.Show();
                    break;
                case MouseButtons.Right:
                    mouseRight = false;
                    break;
            }
        }

        System.Drawing.Point previousMousePosition;

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (cameraMode) {
                projectInfo.mainCamera.posNRot.pitch += (MousePosition.Y - previousMousePosition.Y) * 0.02;
                projectInfo.mainCamera.posNRot.yaw -= (MousePosition.X - previousMousePosition.X) * 0.02;
                this.Invalidate();
            }
            previousMousePosition = MousePosition;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.W:
                    keyW = true;
                    break;
                case Keys.A:
                    keyA = true;
                    break;
                case Keys.S:
                    keyS = true;
                    break;
                case Keys.D:
                    keyD = true;
                    break;
                case Keys.Shift:
                    keyShift = true;
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            switch (e.KeyCode)
            {
                case Keys.W:
                    keyW = false;
                    break;
                case Keys.A:
                    keyA = false;
                    break;
                case Keys.S:
                    keyS = false;
                    break;
                case Keys.D:
                    keyD = false;
                    break;
                case Keys.Shift:
                    keyShift = false;
                    break;
            }
        }

        bool mustRedraw = false;

        public void routineListener()
        {
            if (cameraMode) { 
            Form.ActiveForm.Text = projectInfo.mainCamera.posNRot.position.ToString();
            if (keyW) {
                projectInfo.mainCamera.posNRot.position += projectInfo.mainCamera.posNRot.forward();
                mustRedraw = true;
            }
            if (keyA) {
                projectInfo.mainCamera.posNRot.position -= projectInfo.mainCamera.posNRot.right();
                mustRedraw = true;
            }
            if (keyS)
            {
                projectInfo.mainCamera.posNRot.position -= projectInfo.mainCamera.posNRot.forward();
                mustRedraw = true;
            }
            if (keyD)
            {
                projectInfo.mainCamera.posNRot.position += projectInfo.mainCamera.posNRot.right();
                mustRedraw = true;
            }
            if (mustRedraw) { this.Invalidate(); mustRedraw = false; }
        } }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            projectInfo.halfCanvasSize = new System.Drawing.Size(Convert.ToInt16(this.Width * 0.5), Convert.ToInt16(this.Height * 0.5));
            this.Invalidate();
        }
        #endregion

        #region Rendering
        protected override void OnPaint(PaintEventArgs pe) {
            projectInfo.renderGO_List(pe);
        }
        #endregion
    }
}
