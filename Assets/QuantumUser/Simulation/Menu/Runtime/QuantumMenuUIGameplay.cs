namespace Quantum.Menu
{
    using System.Collections;
    using System.Text;
#if QUANTUM_ENABLE_TEXTMESHPRO
    using Text = TMPro.TMP_Text;
#else
  using Text = UnityEngine.UI.Text;
#endif
    using UnityEngine;
    using UnityEngine.UI;

    public partial class QuantumMenuUIGameplay : QuantumMenuUIScreen
    {
        [SerializeField] protected Text _codeText;
        [SerializeField] protected Text _playersText;
        [SerializeField] protected Text _playersCountText;
        [SerializeField] protected Text _playersMaxCountText;
        [SerializeField] protected Text _headerText;
        [SerializeField] protected GameObject _sessionGameObject;
        [SerializeField] protected GameObject _playersGameObject;
        [SerializeField] protected Button _copySessionButton;
        [SerializeField] protected Button _disconnectButton;
        [SerializeField] protected bool _detectAndToggleMenuCamera;
        [SerializeField] protected Camera _menuCamera;
        [InlineHelp] public float UpdateUsernameRateInSeconds = 2;
        
        protected Coroutine _updateUsernamesCoroutine;

        partial void AwakeUser();
        partial void InitUser();
        partial void ShowUser();
        partial void HideUser();
        
        public override void Awake()
        {
            base.Awake();
            AwakeUser();
        }
        
        public override void Init()
        {
            base.Init();
            InitUser();

            if (_detectAndToggleMenuCamera)
            {
                _menuCamera = FindFirstObjectByType<Camera>();
            }
        }
        
        public override void Show()
        {
            base.Show();
            ShowUser();

            if (_menuCamera != null)
            {
                _menuCamera.enabled = false;
            }

            if (Config.CodeGenerator != null && Config.CodeGenerator.IsValid(Connection.SessionName))
            {
                // Only show the session UI if it is a party code
                _codeText.text = Connection.SessionName;
                _sessionGameObject.SetActive(true);
            }
            else
            {
                _codeText.text = string.Empty;
                _sessionGameObject.SetActive(false);
            }

            UpdateUsernames();

            if (UpdateUsernameRateInSeconds > 0)
            {
                _updateUsernamesCoroutine = StartCoroutine(UpdateUsernamesCoroutine());
            }
        }
        
        public override void Hide()
        {
            base.Hide();
            HideUser();

            if (_menuCamera != null)
            {
                _menuCamera.enabled = true;
            }

            if (_updateUsernamesCoroutine != null)
            {
                StopCoroutine(_updateUsernamesCoroutine);
                _updateUsernamesCoroutine = null;
            }
        }
        
        protected virtual async void OnDisconnectPressed()
        {
            await Connection.DisconnectAsync(ConnectFailReason.UserRequest);
            Controller.Show<QuantumMenuUIMain>();
        }
        
        protected virtual void OnCopySessionPressed()
        {
            GUIUtility.systemCopyBuffer = _codeText.text;
        }
        
        protected virtual IEnumerator UpdateUsernamesCoroutine()
        {
            while (UpdateUsernameRateInSeconds > 0)
            {
                yield return new WaitForSeconds(UpdateUsernameRateInSeconds);
                UpdateUsernames();
            }
        }
        
        protected virtual void UpdateUsernames()
        {
            if (Connection.Usernames != null && Connection.Usernames.Count > 0)
            {
                _playersGameObject.SetActive(true);
                var sBuilder = new StringBuilder();
                var playerCount = 0;
                foreach (var username in Connection.Usernames)
                {
                    sBuilder.AppendLine(username);
                    playerCount += string.IsNullOrEmpty(username) ? 0 : 1;
                }

                _playersText.text = sBuilder.ToString();
                _playersCountText.text = $"{playerCount}";
                _playersMaxCountText.text = $"/{Connection.MaxPlayerCount}";
            }
            else
            {
                _playersGameObject.SetActive(false);
            }
        }
    }
}