namespace Quantum.Menu
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class QuantumMenuUIController : QuantumMonoBehaviour
    {
        [SerializeField] protected QuantumMenuConfig _config;
        [SerializeField] public QuantumMenuConnectionBehaviour Connection;
        [SerializeField] protected QuantumMenuUIScreen[] _screens;
        public QuantumMenuConnectArgs ConnectArgs;
        
        protected Dictionary<Type, QuantumMenuUIScreen> _screenLookup;
        protected QuantumMenuUIPopup _popupHandler;
        protected QuantumMenuUIScreen _activeScreen;
        
        protected virtual void Awake()
        {
            _screenLookup = new Dictionary<Type, QuantumMenuUIScreen>();

            foreach (var screen in _screens)
            {
                screen.Config = _config;
                screen.Connection = Connection;
                screen.ConnectionArgs = ConnectArgs;
                screen.Controller = this;

                var t = screen.GetType();
                while (true)
                {
                    _screenLookup.Add(t, screen);
                    if (t.BaseType == null || typeof(QuantumMenuUIScreen).IsAssignableFrom(t) == false ||
                        t.BaseType == typeof(QuantumMenuUIScreen))
                    {
                        break;
                    }

                    t = t.BaseType;
                }

                if (screen is QuantumMenuUIPopup popupHandler)
                {
                    _popupHandler = popupHandler;
                }
            }

            foreach (var screen in _screens)
            {
                screen.Init();
            }
        }
        
        protected virtual void Start()
        {
            if (_screens is not { Length: > 0 }) 
                return;
            
            // First screen is displayed by default
            _screens[0].Show();
            _activeScreen = _screens[0];
        }
        
        public virtual void Show<S>() where S : QuantumMenuUIScreen
        {
            if (_screenLookup.TryGetValue(typeof(S), out var result))
            {
                if (result.IsModal == false && _activeScreen != result && _activeScreen)
                {
                    _activeScreen.Hide();
                }

                if (_activeScreen != result)
                {
                    result.Show();
                }

                if (result.IsModal == false)
                {
                    _activeScreen = result;
                }
            }
            else
            {
                Debug.LogError($"Show() - Screen type '{typeof(S).Name}' not found");
            }
        }
        
        public virtual S Get<S>() where S : QuantumMenuUIScreen
        {
            if (_screenLookup.TryGetValue(typeof(S), out var result))
            {
                return result as S;
            }

            Debug.LogError($"Show() - Screen type '{typeof(S).Name}' not found");
            return null;
        }
        
        public void Popup(string msg, string header = default)
        {
            if (_popupHandler == null)
            {
                Debug.LogError("Popup() - no popup handler found");
            }
            else
            {
                _popupHandler.OpenPopup(msg, header);
            }
        }
        
        public Task PopupAsync(string msg, string header = default)
        {
            if (_popupHandler != null) 
                return _popupHandler.OpenPopupAsync(msg, header);
            
            Debug.LogError("Popup() - no popup handler found");
            return Task.CompletedTask;
        }
        
        public virtual async Task HandleConnectionResult(ConnectResult result, QuantumMenuUIController controller)
        {
            if (result.CustomResultHandling)
            {
                return;
            }

            if (result.Success)
            {
                _activeScreen.Hide();
            }
            else if (result.FailReason != ConnectFailReason.ApplicationQuit)
            {
                var popup = controller.PopupAsync(result.DebugMessage, "Connection Failed");
                if (result.WaitForCleanup != null)
                {
                    await Task.WhenAll(result.WaitForCleanup, popup);
                }
                else
                {
                    await popup;
                }

                controller.Show<QuantumMenuUIMain>();
            }
        }
    }
}