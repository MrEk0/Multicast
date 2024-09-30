#if QUANTUM_ENABLE_TEXTMESHPRO
using Text = TMPro.TMP_Text;
using InputField = TMPro.TMP_InputField;
#else
  using Text = UnityEngine.UI.Text;
  using InputField = UnityEngine.UI.InputField;
#endif
using UnityEngine;

namespace Quantum.Menu
{
    public partial class QuantumMenuUIMain : QuantumMenuUIScreen
    {
        [SerializeField] protected Text _usernameLabel;
        [SerializeField] protected GameObject _usernameView;
        [SerializeField] protected InputField _usernameInput;
        [SerializeField] protected UnityEngine.UI.Button _usernameConfirmButton;
        [SerializeField] protected UnityEngine.UI.Button _usernameButton;
        [SerializeField] protected UnityEngine.UI.Button _partyButton;
        [SerializeField] protected UnityEngine.UI.Button _playButton;
        [SerializeField] protected UnityEngine.UI.Button _quitButton;

        partial void AwakeUser();
        partial void InitUser();
        partial void ShowUser();
        partial void HideUser();

        public override void Awake()
        {
            base.Awake();

            new QuantumMenuGraphicsSettings().Apply();

#if UNITY_STANDALONE
            _quitButton.gameObject.SetActive(true);
#else
            _quitButton.gameObject.SetActive(false);
#endif

            AwakeUser();
        }

        public override void Init()
        {
            base.Init();

            ConnectionArgs.LoadFromPlayerPrefs();
            ConnectionArgs.SetDefaults(Config);

            InitUser();
        }


        public override void Show()
        {
            base.Show();

            _usernameView.SetActive(false);
            if (_usernameLabel)
            {
                _usernameLabel.text = ConnectionArgs.Username;
            }

            if (string.IsNullOrEmpty(ConnectionArgs.Scene.NameOrSceneName))
            {
                _playButton.interactable = false;
                _partyButton.interactable = false;
                
                Debug.LogWarning("No valid scene to start found. Configure the menu config.");
            }
            
            ShowUser();
        }

        public override void Hide()
        {
            base.Hide();
            HideUser();
        }

        protected virtual void OnFinishUsernameEdit()
        {
            OnFinishUsernameEdit(_usernameInput.text);
        }

        protected virtual void OnFinishUsernameEdit(string username)
        {
            _usernameView.SetActive(false);

            if (string.IsNullOrEmpty(username) == false)
            {
                _usernameLabel.text = username;
                ConnectionArgs.Username = username;
                ConnectionArgs.SaveToPlayerPrefs();
            }
        }

        protected virtual void OnUsernameButtonPressed()
        {
            _usernameView.SetActive(true);
            _usernameInput.text = _usernameLabel.text;
        }

        protected virtual async void OnPlayButtonPressed()
        {
            ConnectionArgs.Session = null;
            ConnectionArgs.Creating = false;
            ConnectionArgs.Region = ConnectionArgs.PreferredRegion;

            Controller.Show<QuantumMenuUILoading>();

            var result = await Connection.ConnectAsync(ConnectionArgs);

            await Controller.HandleConnectionResult(result, this.Controller);
        }

        protected virtual void OnPartyButtonPressed()
        {
            Controller.Show<QuantumMenuUIParty>();
        }

        protected virtual void OnScenesButtonPressed()
        {
            Controller.Show<QuantumMenuUIScenes>();
        }

        protected virtual void OnSettingsButtonPressed()
        {
            Controller.Show<QuantumMenuUISettings>();
        }


        protected virtual void OnCharacterButtonPressed()
        {
        }

        protected virtual void OnQuitButtonPressed()
        {
            Application.Quit();
        }
    }
}