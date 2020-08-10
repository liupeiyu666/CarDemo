using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageEffects
{
    public class MotionEffect : ImageEffectBase
    {

        [SerializeField]
        [Range(0.001f, 0.999f)]
        private float _blurSize = 0.9f;


        private void OnEnable()
        {
            material.SetFloat("_BlurSize", _blurSize);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RenderTexture.ReleaseTemporary(_accumulationRT);
            _accumulationRT = null;
        }

        private RenderTexture _accumulationRT;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_accumulationRT == null || _accumulationRT.width != source.width || _accumulationRT.height != source.height)
            {
                if (_accumulationRT != null)
                    RenderTexture.ReleaseTemporary(_accumulationRT);
                _accumulationRT = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
                _accumulationRT.hideFlags = HideFlags.HideAndDontSave;
                Graphics.Blit(source, _accumulationRT);
            }
            _accumulationRT.MarkRestoreExpected();
            Graphics.Blit(source, _accumulationRT, material);
            Graphics.Blit(_accumulationRT, destination);
        }
    }
}
