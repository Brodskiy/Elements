using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Extensions
{
    public static class AnimatorExtensions
    {
        private const int MillisecondsPerSecond = 1000;

        public static UniTask PlayAsync(this Animator animator, int clipIndex, CancellationToken cancellationTokenSource)
        {
            animator.Play(animator.runtimeAnimatorController.animationClips[clipIndex].name);
            return UniTask.Delay(animator.ToMillisecondsDuration(clipIndex), cancellationToken: cancellationTokenSource);
        }
        
        private static int ToMillisecondsDuration(this Animator animator, int clipIndex)
        {
            return (int)(animator.runtimeAnimatorController.animationClips[clipIndex].averageDuration * MillisecondsPerSecond);
        }
    }
}