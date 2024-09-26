using System.Collections.Generic;
using Game.Scripts.Interfaces;
using UnityEngine;

namespace Game.Scripts.Services
{
    public class GameUpdater : MonoBehaviour
    {
        private readonly List<IUpdatable> _updateListeners = new();

        public void AddListener(IUpdatable listener)
        {
            _updateListeners.Add(listener);
        }

        public void RemoveListener(IUpdatable listener)
        {
            _updateListeners.Remove(listener);
        }

        public void RemoveAll()
        {
            _updateListeners.Clear();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            for (var i = 0; i < _updateListeners.Count; i++)
            {
                _updateListeners[i].OnUpdate(deltaTime);
            }
        }
    }
}
