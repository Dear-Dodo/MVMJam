using System.Collections;
using System.Collections.Generic;
using UI.Generic;
using UnityEngine;
using DG.Tweening;
using Management;
using UnityAsync;

namespace Scenes
{
    public class SplashScreen : MonoBehaviour
    {
        public float FadeTime;

        [SerializeField]
        private ProgressBar _loadingBar;
        [SerializeField]
        private CanvasGroup _elementToFade;

        private bool _isUpdating = true;

        void Start()
        {
            Management.AssetLoader.OnAssetsLoaded += TransitionToMenu;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isUpdating)
            {
                var (current, total) = Management.AssetLoader.GetLoadProgress();
                if (current != -1)
                {
                    _loadingBar.UpdateProgress((float)current / total, current, total);
                }
            }
        }

        private async void TransitionToMenu()
        {
            _isUpdating = false;
            _loadingBar.UpdateProgress(1.0f);
            AsyncOperation menuLoad = SceneLoader.LoadAsync("MainMenu");
            menuLoad.allowSceneActivation = false;
            _loadingBar.SetText("Loading menu...");

            // AsyncOperation never seems to complete if allowSceneActivation is false, but it does hit 0.9 progress.
            await Await.Until(()=>menuLoad.progress >= 0.9f);

            var tween = DOTween.To(() => { return _elementToFade.alpha; }, v => _elementToFade.alpha = v, 0, FadeTime);
            tween.OnComplete(() => menuLoad.allowSceneActivation = true);
        }
    } 
}
