using MoreMountains.NiceVibrations;
using UnityEngine;
using static Managers.Actions;

namespace Managers
{
    public class HapticManager : MonoBehaviour
    {
        private void OnEnable()
        {
            LevelStart += Selection;
            LevelEnd += Success;
            Actions.LightImpact += LightImpact;
            Actions.MediumImpact += MediumImpact;
            ButtonTapped += LightImpact;
        }

        private void OnDisable()
        {
            LevelStart -= Selection;
            LevelEnd -= Success;
            Actions.LightImpact -= LightImpact;
            Actions.MediumImpact -= MediumImpact;
            ButtonTapped -= LightImpact;
        }

        private void Success()
        {
            //if (PlayerDataManager.playerData.isVibrationOff) return;
            MMVibrationManager.Haptic(HapticTypes.Success);
        }
    
        private void Fail()
        {
            //if (PlayerDataManager.playerData.isVibrationOff) return;
            MMVibrationManager.Haptic(HapticTypes.Failure);
        }

        private void LightImpact()
        {
            //if (PlayerDataManager.playerData.isVibrationOff) return;
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }
    
        private void MediumImpact()
        {
            //if (PlayerDataManager.playerData.isVibrationOff) return;
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }

        private void Selection()
        {
            //if (PlayerDataManager.playerData.isVibrationOff) return;
            MMVibrationManager.Haptic(HapticTypes.Selection);
        }

        private void Warning()
        {
            //if (PlayerDataManager.playerData.isVibrationOff) return;
            MMVibrationManager.Haptic(HapticTypes.Warning);
        }
    }
}