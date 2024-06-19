using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/Bonus")]
    public class ItemWithBonus : ItemBase
    {
        private enum BonusType { Multiply, Add }
        public enum BonusTarget 
        { 
            FinalScore = 1 << 0, 
            HeatedStrength = 1 << 1, 
            CoolingRate = 1 << 2,
            CookingRate = 1 << 3,
            MachineCost = 1 << 4
        }
        
        [Tooltip("The cost of the item in the shop.")]
        [SerializeField] private int _cost;
        [Tooltip("How the bonus will be applied, either by multiplying or adding to the original value.")]
        [SerializeField, EnumToggleButtons] private BonusType _bonusType;
        [Tooltip("What value will be modified by the bonus.")]
        [SerializeField, EnumToggleButtons] private BonusTarget _bonusTarget;
        [Tooltip("The value that will be added or multiplied to the original value.")]
        [SerializeField] private float _bonusValue;

        /// <summary>
        /// What value will be modified by the bonus.
        /// </summary>
        public BonusTarget Modifies => _bonusTarget;
        
        public override string Name => name.Split("_")[1].SplitPascalCase();
        
        public override string Description
        {
            get
            {
                if (_bonusValue == 0) return "Does nothing when used.";
                
                var increaseOrDecrease = _bonusValue > 0 ? "increase" : "decrease";
                var modifier = _bonusType == BonusType.Multiply ? $"{_bonusValue * 100f}%" : $"{_bonusValue}";
                return $"Will {increaseOrDecrease} the {Modifies.ToString().SplitPascalCase()} by {modifier} when used.";
            }
        }
        
        public override int Cost => _cost;
        
        /// <summary>
        /// Using the value provided, apply the bonus to it then return the new value.
        /// </summary>
        public float ApplyBonus(float originalValue)
        {
            return _bonusType switch
            {
                BonusType.Multiply => originalValue * (1 + _bonusValue),
                BonusType.Add => originalValue + _bonusValue,
                _ => originalValue
            };
        }
    }
}