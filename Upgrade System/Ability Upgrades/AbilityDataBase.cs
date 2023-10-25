using Sirenix.OdinInspector;
using UnityEngine;

namespace Darkan.UpgradeSystem.Ability
{
    public abstract class AbilityDataBase<TAbility> : SerializedScriptableObject where TAbility : System.Enum
    {
        public virtual string GetDescription(UpgradeManagerBase<TAbility>.UpgradeKey key)
        {
            return $"{key.AbilityType} Level {key.AbilityLevel}";
        }
    }
}
