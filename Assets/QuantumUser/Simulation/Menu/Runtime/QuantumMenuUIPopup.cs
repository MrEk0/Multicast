namespace Quantum.Menu
{
    using System.Threading.Tasks;
#if QUANTUM_ENABLE_TEXTMESHPRO
    using Text = TMPro.TMP_Text;
    using InputField = TMPro.TMP_InputField;
#else
  using Text = UnityEngine.UI.Text;
  using InputField = UnityEngine.UI.InputField;
#endif
    using UnityEngine;
    using UnityEngine.UI;
    
    public partial class QuantumMenuUIPopup : QuantumMenuUIScreen
    {
        [InlineHelp, SerializeField] protected Text _text;
        [InlineHelp, SerializeField] protected Text _header;
        [InlineHelp, SerializeField] protected Button _button;
        
        protected TaskCompletionSource<bool> _taskCompletionSource;

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

            // Free the _taskCompletionSource before releasing the old one.
            var completionSource = _taskCompletionSource;
            _taskCompletionSource = null;
            completionSource?.TrySetResult(true);
        }
        
        public virtual void OpenPopup(string msg, string header)
        {
            _header.text = header;
            _text.text = msg;

            Show();
        }
        
        public virtual Task OpenPopupAsync(string msg, string header)
        {
            _taskCompletionSource?.TrySetResult(true);
            _taskCompletionSource = new TaskCompletionSource<bool>();

            OpenPopup(msg, header);

            return _taskCompletionSource.Task;
        }
    }
}