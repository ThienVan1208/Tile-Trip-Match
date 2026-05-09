using System;
using UnityEngine;

namespace TileAdventure
{

    [CreateAssetMenu(menuName = "Events/Void Event Channel", fileName = "NewVoidEvent")]
    public class VoidEventChannelSO : ScriptableObject
    {
        private event Action _onEvent;

        public void RaiseEvent()
        {
            _onEvent?.Invoke();
        }

        public void Subscribe(Action listener)
        {
            _onEvent += listener;
        }

        public void Unsubscribe(Action listener)
        {
            _onEvent -= listener;
        }
    }
}
