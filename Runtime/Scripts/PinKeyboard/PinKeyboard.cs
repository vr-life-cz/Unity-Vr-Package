using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Plugins.com.vrlife.vr.Runtime.Scripts.PinKeyboard
{
    [Serializable]
    public class PinSubmitEvent : UnityEvent<string>
    {
    }
    public class PinKeyboard : MonoBehaviour
    {
        [Title("Objects")]
        public TextMeshProUGUI pinWindow;
        public GameObject pinBoard;
        public GameObject validationText;
        
        [Title("Events")]
        public PinSubmitEvent onConfirm;

        [Title("Settings")] 
        [Range(1, 15)] public int limit;
        public bool requiresMax;


        private string _content = string.Empty;


        private void Start()
        {
            onConfirm = new PinSubmitEvent();
        }

        public void ButtonPress(int value)
        {
            if (_content.Length < limit)
            {
                _content += value.ToString();
            }
            UpdateText();
        }

        public void BackspacePress()
        {
            if (_content != string.Empty)
            {
                _content.Remove(_content.Length - 1);
            }
            UpdateText();
        }

        public void ConfirmPress()
        {
            if (requiresMax && (_content.Length - 1) != limit) return;
            pinBoard.SetActive(false);
            validationText.SetActive(true);
            if (onConfirm is null)
            {
                OnValidationFailed();
            }
            else
            {
                onConfirm.Invoke(_content);
            }
            
        }

        private void UpdateText()
        {
            pinWindow.text = "PIN: " + _content;
        }

        public void OnValidationFailed()
        {
            pinBoard.SetActive(true);
            validationText.SetActive(false);
            pinWindow.text = "PIN: INCORRECT";
        }
    }
}
