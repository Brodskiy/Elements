using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameElement.Data.Block
{
    [Serializable]
    public class BlockViewComponent
    {
        [SerializeField]
        private AnimatorController _animatorController;

        public AnimatorController Animations => _animatorController;
    }
}