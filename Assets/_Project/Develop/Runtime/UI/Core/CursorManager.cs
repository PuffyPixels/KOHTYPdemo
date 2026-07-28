using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Core
{
    public class CursorManager
    {
        public void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}