using UnityEditor.Animations;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameElement.Data.Block
{
    [CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/Blocks")]
    public class BlockData : ScriptableObject
    {
        [SerializeField]
        private int _id;
        [SerializeField]
        private AnimatorController _animatorController;

        public int Id => _id;
        public AnimatorController Animations => _animatorController;
    }
}