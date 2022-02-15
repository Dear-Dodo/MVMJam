using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace UI.Generic
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _foreground;
        [SerializeField]
        private TextMeshProUGUI _progressText;

        private RectTransform _foregroundParent;

        /// <summary>
        /// Called to update the value of the progress bar. If null, only updates manually.
        /// </summary>
        public Func<float> GetProgress;

        private void Start()
        {
            _foregroundParent = (RectTransform)_foreground.parent;
        }

        // Update is called once per frame
        void Update()
        {
            if (_foreground && GetProgress != null)
            {
                UpdateProgress(GetProgress());
            }
        }

        private void UpdateBar(float percentage)
        {
            float maxWidth = _foregroundParent.rect.width;
            _foreground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, percentage * maxWidth);
        }

        /// <summary>
        /// Manually update progress bar with percentage.
        /// </summary>
        /// <param name="progress"></param>
        public void UpdateProgress(float progress)
        {
            UpdateBar(progress);

            if (_progressText)
            {
                _progressText.text = $"{Mathf.Floor(progress * 100)}%";
            }
        }

        /// <summary>
        /// Manually update progress bar with percentage and absolute values.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="current"></param>
        /// <param name="max"></param>
        public void UpdateProgress(float progress, int current, int max)
        {
            UpdateBar(progress);

            if (_progressText)
            {
                _progressText.text = $"{current}/{max} ({Mathf.Floor(progress * 100)}%)";
            }
        }

        /// <summary>
        /// Directly set the value of the progress text.
        /// </summary>
        /// <param name="value"></param>
        public void SetText(string value)
        {
            if (_progressText)
            {
                _progressText.text = value;
            }
        }
    } 
}
