using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Views.General
{
    public class PanelView : MonoBehaviour
    {
        public Action<PanelView,int> PressBtnAction { get; set; }

        [SerializeField]
        private List<Button> _btns;

        private void OnEnable()
        {
            _btns.ForEach(x => x.onClick.AddListener(() =>Notification(x)));
        }

        private void OnDisable()
        {
            _btns.ForEach(x => x.onClick.RemoveAllListeners());
        }

        protected void SetInteractableBtn(int index, bool value)
        {
            if (index >= _btns.Count)
            {
                return;
            }

            _btns[index].interactable = value;
        }

        public void Open()
        {
            SetActive(true);
        }

        public void Close()
        {
            SetActive(false);
        }

        private void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        private void Notification(Button btn)
        {
            PressBtnAction?.Invoke(this,_btns.IndexOf(btn));
        }
    }
}