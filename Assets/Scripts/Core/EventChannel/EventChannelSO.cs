using System;
using UnityEngine;

namespace TileAdventure
{

    public abstract class EventChannelSO<T> : ScriptableObject
    {
        private event Action<T> _onEvent;

        public void RaiseEvent(T value)
        {

            _onEvent?.Invoke(value);
        }

        public void Subscribe(Action<T> listener)
        {
            _onEvent += listener;
        }

        public void Unsubscribe(Action<T> listener)
        {
            _onEvent -= listener;
        }
    }
}
