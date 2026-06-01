using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Moths.Animations
{
    [CustomPreview(typeof(UnityEditor.Animations.AnimatorState))]
    public class AnimatorStateObjectPreview : ObjectPreview
    {
        Editor _preview;
        int _animationClipId;

        public override void Initialize(Object[] targets)
        {
            base.Initialize(targets);

            if (targets.Length > 1 || Application.isPlaying) return;

            AnimationClip clip = GetAnimationClip(base.target as UnityEditor.Animations.AnimatorState);
            if (clip != null)
            {
                _preview = Editor.CreateEditor(clip);
                _animationClipId = clip.GetInstanceID();
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            CleanupPreviewEditor();
        }

        public override bool HasPreviewGUI()
        {
            return GetAnimationClip(base.target as UnityEditor.Animations.AnimatorState);
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            base.OnInteractivePreviewGUI(r, background);

            var currentClip = GetAnimationClip(base.target as UnityEditor.Animations.AnimatorState);
            if (currentClip != null && currentClip.GetInstanceID() != _animationClipId)
            {
                CleanupPreviewEditor();
                _preview = Editor.CreateEditor(currentClip);
                _animationClipId = currentClip.GetInstanceID();
                return;
            }
            else if (!currentClip)
            {
                CleanupPreviewEditor();
                return;
            }

            if (_preview != null && _preview.HasPreviewGUI())
            {
                _preview.OnInteractivePreviewGUI(r, background);
            }
        }

        private AnimationClip GetAnimationClip(UnityEditor.Animations.AnimatorState state) => state?.motion as AnimationClip;

        private void CleanupPreviewEditor()
        {
            if (_preview != null)
            {
                Object.DestroyImmediate(_preview);
                _preview = null;
                _animationClipId = 0;
            }
        }
    }
}