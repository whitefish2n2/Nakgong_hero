using System;
using Source.UiCode;
using UnityEngine;

namespace Source.PlayerCode
{
    public class KeyInputManager : MonoBehaviour
    {
        public InventoryUiManager inventoryUiManager;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                inventoryUiManager.SwitchInvUi();
            }
        }
    }
}
