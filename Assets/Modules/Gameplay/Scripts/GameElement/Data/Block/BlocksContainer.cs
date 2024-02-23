using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameElement.Data.Block
{
    [CreateAssetMenu(fileName = "BlocksContainer", menuName = "ScriptableObjects/BlocksContainer")]
    public class BlocksContainer : ScriptableObject
    {
        [SerializeField]
        private List<BlockData> _blockDatas;

        public AnimatorController GetBlockAnimatorById(int blockId)
        {
            if (_blockDatas.Exists(block => block.Id == blockId))
            {
                return _blockDatas.Find(block => block.Id == blockId).Animations;
            }
            
            Debug.LogError($"Block with id - {blockId} not found.");
            return null;
        }
    }
}