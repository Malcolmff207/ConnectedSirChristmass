using UnityEngine;

    public class MouseToggle: MonoBehaviour
    {
        private bool isCursorLocked = true;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ToggleCursorState();
        }

        private void ToggleCursorState()
        {
            isCursorLocked = !isCursorLocked;

            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }