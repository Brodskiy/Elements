using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameElement.Data.Block
{
    [Serializable]
    public class BlockData
    {
        [SerializeField]
        private int _id;
        [SerializeField]
        private AnimatorController _animatorController;

        public int Id => _id;
        public AnimatorController Animations => _animatorController;
    }
}