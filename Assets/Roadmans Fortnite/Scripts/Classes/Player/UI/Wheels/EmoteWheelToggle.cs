using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteWheelToggle : MonoBehaviour
{
   
      
    /*
    /// <summary>
    /// Toggles the emote wheel. This looks a mess. And must have a massive amount of other ways to do this
    /// </summary>
    public void EmoteWheelUIToggle()
    {
        if (!inWeaponWheel && !GetComponent<EmoteWheel>().inEmoteWheel && 
            !inPropertyArea && !inShop && !inCar && !usesParachute && !this.GetComponent<EmoteWheel>().isPlayingAnimation && 
            isAiming && !this.GetComponent<Health>().isDeath && _inputs.shoot && _slot.amount > 0 && _slot.item.itemSO != null && _slot.item.itemSO is WeaponItemSO weaponItemSO) {
            _interval = weaponItemSO.cooldownInSeconds;
            if (NetworkTime.time >= _lastTime + _interval) {
                if (weaponItemSO.automatic) {
                    CmdUseItem(0);
                }
                else if (_mouse.leftButton.wasPressedThisFrame || Gamepad.current.rightTrigger.wasPressedThisFrame) {
                    CmdUseItem(0);
                }
                _lastTime = NetworkTime.time;
            }
        }
    }
    */
}
