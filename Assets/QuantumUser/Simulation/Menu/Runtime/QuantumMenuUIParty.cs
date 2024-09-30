namespace Quantum.Menu
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
#if QUANTUM_ENABLE_TEXTMESHPRO
    using InputField = TMPro.TMP_InputField;
#else
  using InputField = UnityEngine.UI.InputField;
#endif
    using UnityEngine;
    
    public partial class QuantumMenuUIParty : QuantumMenuUIScreen
    {
        [InlineHelp, SerializeField] protected InputField _sessionCodeField;
        [InlineHelp, SerializeField] protected Button _createButton;
        [InlineHelp, SerializeField] protected Button _joinButton;
        [InlineHelp, SerializeField] protected Button _backButton;
        
        protected Task<List<QuantumMenuOnlineRegion>> _regionRequest;

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

            if (Config.CodeGenerator == null)
            {
                Debug.LogError("Add a CodeGenerator to the QuantumMenuConfig");
            }

            _sessionCodeField.SetTextWithoutNotify("".PadLeft(Config.CodeGenerator.Length, '-'));
            _sessionCodeField.characterLimit = Config.CodeGenerator.Length;

            if (_regionRequest == null || _regionRequest.IsFaulted)
            {
                // Request the regions already when entering the party menu
                _regionRequest = Connection.RequestAvailableOnlineRegionsAsync(ConnectionArgs);
            }

            ShowUser();
        }
        
        public override void Hide()
        {
            base.Hide();
            HideUser();
        }

     
        protected virtual async void OnCreateButtonPressed()
        {
            await ConnectAsync(true);
        }

     
        protected virtual async void OnJoinButtonPressed()
        {
            await ConnectAsync(false);
        }

       
        public virtual void OnBackButtonPressed()
        {
            Controller.Show<QuantumMenuUIMain>();
        }

    
        protected virtual async Task ConnectAsync(bool creating)
        {
            // Test for input errors before switching screen
            var inputRegionCode = _sessionCodeField.text.ToUpper();
            if (creating == false && Config.CodeGenerator.IsValid(inputRegionCode) == false)
            {
                await Controller.PopupAsync(
                    $"The session code '{inputRegionCode}' is not a valid session code. Please enter {Config.CodeGenerator.Length} characters or digits.",
                    "Invalid Session Code");
                return;
            }

            if (_regionRequest.IsCompleted == false)
            {
                // Goto loading screen
                Controller.Show<QuantumMenuUILoading>();
                Controller.Get<QuantumMenuUILoading>().SetStatusText("Fetching Regions");

                try
                {
                    // TODO: Disconnect button not usable during this time
                    await _regionRequest;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    // Error is handled in next section
                }
            }

            if (_regionRequest.IsCompletedSuccessfully == false && _regionRequest.Result.Count == 0)
            {
                await Controller.PopupAsync($"Failed to request regions.", "Connection Failed");
                Controller.Show<QuantumMenuUIMain>();
                return;
            }

            if (creating)
            {
                var regionIndex = -1;
                // Select a best region now
                regionIndex = string.IsNullOrEmpty(ConnectionArgs.PreferredRegion) ? FindBestAvailableOnlineRegionIndex(_regionRequest.Result) : _regionRequest.Result.FindIndex(r => r.Code == ConnectionArgs.PreferredRegion);

                if (regionIndex == -1)
                {
                    await Controller.PopupAsync($"Selected region is not available.", "Connection Failed");
                    Controller.Show<QuantumMenuUIMain>();
                    return;
                }

                ConnectionArgs.Session = Config.CodeGenerator.EncodeRegion(Config.CodeGenerator.Create(), regionIndex);
                ConnectionArgs.Region = _regionRequest.Result[regionIndex].Code;
            }
            else
            {
                var regionIndex = Config.CodeGenerator.DecodeRegion(inputRegionCode);
                if (regionIndex < 0 || regionIndex > Config.AvailableRegions.Count)
                {
                    await Controller.PopupAsync(
                        $"The session code '{inputRegionCode}' is not a valid session code (cannot decode the region).",
                        "Invalid Session Code");
                    return;
                }

                ConnectionArgs.Session = _sessionCodeField.text.ToUpper();
                ;
                ConnectionArgs.Region = Config.AvailableRegions[regionIndex];
            }

            ConnectionArgs.Creating = creating;

            Controller.Show<QuantumMenuUILoading>();

            var result = await Connection.ConnectAsync(ConnectionArgs);

            await Controller.HandleConnectionResult(result, this.Controller);
        }

        private static int FindBestAvailableOnlineRegionIndex(IReadOnlyList<QuantumMenuOnlineRegion> regions)
        {
            var lowestPing = int.MaxValue;
            var index = -1;
            for (var i = 0; regions != null && i < regions.Count; i++)
            {
                if (regions[i].Ping >= lowestPing)
                    continue;
                
                lowestPing = regions[i].Ping;
                index = i;
            }

            return index;
        }
    }
}