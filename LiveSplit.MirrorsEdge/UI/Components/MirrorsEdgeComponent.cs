using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.MirrorsEdge
{
    public class MirrorsEdgeComponent : LogicComponent
    {
        public override string ComponentName => "Mirror's Edge";

        public MirrorsEdgeSettings Settings { get; }

        private readonly TimerModel _timer;
        private readonly GameMemory _gameMemory;
        private readonly Timer _updateTimer;

        public MirrorsEdgeComponent(LiveSplitState state)
        {
            Settings = new MirrorsEdgeSettings();

            _timer = new TimerModel { CurrentState = state };
            _timer.CurrentState.OnStart += timer_OnStart;

            _updateTimer = new Timer() { Interval = 15, Enabled = true };
            _updateTimer.Tick += updateTimer_Tick;

            _gameMemory = new GameMemory();
            _gameMemory.OnStart += gameMemory_OnStart;
            _gameMemory.OnSplit += gameMemory_OnSplit;
            _gameMemory.OnTTSplit += gameMemory_OnTTSplit;
            _gameMemory.On3StarSplit += gameMemory_On3StarSplit;
            _gameMemory.OnSDSplit += gameMemory_OnSDSplit;
            _gameMemory.OnPause += gameMemory_OnPause;
            _gameMemory.OnUnpause += gameMemory_OnUnpause;
            _gameMemory.OnReset += gameMemory_OnReset;
            _gameMemory.OnEnd += gameMemory_OnEnd;
        }

        public override void Dispose()
        {
            _timer.CurrentState.OnStart -= timer_OnStart;
            _updateTimer?.Dispose();
            _gameMemory?.Dispose();
        }

        void updateTimer_Tick(object sender, EventArgs eventArgs)
        {
            try
            {
                _gameMemory.Update();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        void timer_OnStart(object sender, EventArgs e)
        {
            _timer.InitializeGameTime();
        }

        void gameMemory_OnStart(object sender, EventArgs e)
        {
            if (Settings.AutoStart)
            {
                _timer.Start();
            }
        }
        void gameMemory_OnSplit(object sender, EventArgs e)
        {
            if (Settings.AutoSplit)
            {
                _timer.Split();
            }
        }

        void gameMemory_OnSDSplit(object sender, EventArgs e)
        {
            if (Settings.AutoSplit && Settings.SDSplit)
            {
                _timer.Split();
            }
        }

        void gameMemory_OnTTSplit(object sender, EventArgs e)
        {
            if (Settings.AutoSplit && !Settings.StarsRequired)
            {
                _timer.Split();
            }
        }

        void gameMemory_On3StarSplit(object sender, EventArgs e)
        {
            if (Settings.AutoSplit && Settings.StarsRequired)
            {
                _timer.Split();
            }
        }

        void gameMemory_OnPause(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = true;
        }

        void gameMemory_OnUnpause(object sender, EventArgs e)
        {
            _timer.CurrentState.IsGameTimePaused = false;
        }

        void gameMemory_OnReset(object sender, EventArgs e)
        {
            if (Settings.AutoReset)
            {
                _timer.Reset();
            }
        }

        void gameMemory_OnEnd(object sender, EventArgs e)
        {
            _timer.Split();
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {

        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }
    }
}
