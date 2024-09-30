namespace Quantum.Menu
{
#if QUANTUM_ENABLE_TEXTMESHPRO
    using Text = TMPro.TMP_Text;
#else
  using Text = UnityEngine.UI.Text;
#endif
    using UnityEngine;

    public partial class QuantumMenuUILoading : QuantumMenuUIScreen
    {
        [SerializeField] protected UnityEngine.UI.Button _disconnectButton;
        [SerializeField] protected Text _text;

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
        }
        
        public override void Show()
        {
            base.Show();
            ShowUser();
        }
        
        public override void Hide()
        {
            base.Hide();
            HideUser();
        }
        
        public void SetStatusText(string text)
        {
            if (_text != null)
            {
                _text.text = text;
            }
        }
        
        protected virtual async void OnDisconnectPressed()
        {
            await Connection.DisconnectAsync(ConnectFailReason.UserRequest);
        }
    }
}