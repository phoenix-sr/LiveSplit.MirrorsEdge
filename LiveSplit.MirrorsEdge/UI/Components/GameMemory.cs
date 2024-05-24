using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Pipes;

namespace LiveSplit.MirrorsEdge
{
    class GameData : MemoryWatcherList
    {
        /* General / Full Game Addresses */

        public MemoryWatcher<int> IsLoading { get; } // 0 = static load, 1 = no load
        public MemoryWatcher<int> IsSaving { get; } // 1 = game is saving
        public MemoryWatcher<int> IsPauseMenu { get; } // 1 = tab or esc menu (all sub-menus except video) is open
        public MemoryWatcher<int> IsWhiteScreen { get; } // > 0 = white screen or some sort of load
        public StringWatcher RespawnCP { get; } // current minor/soft checkpoint
        public StringWatcher PersistentLevel { get; } // current persistent level

        /* Player State Addresses */
        public MemoryWatcher<Vector3f> PlayerPos { get; } // player position
        public MemoryWatcher<float> ObjectPosZ { get; } // z position of the object the player is standing on
        public MemoryWatcher<byte> MoveState { get; } // movement state of the player pawn
        public MemoryWatcher<byte> AllowMoveChange { get; } // elevator state (1 = no elevator state, 0 = elevator state)

        public MemoryWatcher<PlayerInput> IgnorePlayerInput { get; } // ignore player move and look input state (1 = ignore, 0 = don't ignore)
        public MemoryWatcher<byte> IgnoreButtonInput { get; } // ignore player button input state (1 = ignore, 0 = don't ignore)

        /* 69 Stars Addresses */
        public MemoryWatcher<TTCheckpoint> Checkpoint { get; } // tt checkpoint info (current cp, total cps)
        public MemoryWatcher<byte> ActiveTTStretch { get; } // active tt stretch (order does not match in-game order)
        public MemoryWatcher<float> FinishedTime { get; } // time it took the player to finish the tt
        public MemoryWatcher<TTStars> StarTime { get; } // qualifying times for 1, 2 and 3 stars

        /* Bag Counter */
        public MemoryWatcher<int> BagCounter { get; } // counter for the number of bags collected (does not increment by 1, just goes up)

        /* Custom Structs (the bytes for these addresses follow each other so using these only require one DeepPointer per struct) */
        [StructLayout(LayoutKind.Sequential)]
        public struct TTStars
        {
            public float Three { get; set; }
            public float Two { get; set; }
            public float One { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TTCheckpoint
        {
            public int TotalCPs { get; set; }
            public int CurrentCP { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PlayerInput
        {
            public byte Move { get; set; }
            public byte Look { get; set; }
        }

        public GameData(GameVersion version)
        {
            if (version == GameVersion.Steam)
            {
                /* Full Game/General Pointers */
                IsLoading = new MemoryWatcher<int>(new DeepPointer(0x01B6443C));
                IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C55EA8, 0xCC));
                IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01B985AC, 0x0, 0x188));
                IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C00A38));

                RespawnCP = new StringWatcher(new DeepPointer(0x01C55EA8, 0x74, 0x0, 0x3C, 0x0), 58);
                PersistentLevel = new StringWatcher(new DeepPointer(0x01BF8B20, 0x3CC, 0x0), 48);

                PlayerPos = new MemoryWatcher<Vector3f>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0xE8));

                MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x500));
                AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x41D));
                IgnorePlayerInput = new MemoryWatcher<PlayerInput>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2FD));
                IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x575));

                ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C553D0, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                Checkpoint = new MemoryWatcher<TTCheckpoint>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x3D0));
                ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x3F9));
                FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x424));
                StarTime = new MemoryWatcher<TTStars>(new DeepPointer(0x01BFBCA4, 0x50, 0x1E0, 0x318, 0x408));

                /* Bag Counter */
                BagCounter = new MemoryWatcher<int>(new DeepPointer(0x01B73F1C, 0x64, 0x4C, 0x7A4));
            }
            else if (version == GameVersion.Reloaded)
            {
                /* Full Game/General Pointers */
                IsLoading = new MemoryWatcher<int>(new DeepPointer(0x01B685BC));
                IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0xCC));
                IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01BB166C, 0x0, 0x188));
                IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C19AF8));

                RespawnCP = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x134, 0xBC, 0x0, 0x3C, 0x0), 58);
                PersistentLevel = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x170, 0x1DC, 0x1E8, 0x3C, 0x528, 0x3CC, 0x0), 48);

                PlayerPos = new MemoryWatcher<Vector3f>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xE8));

                MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x500));
                AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x41D));
                IgnorePlayerInput = new MemoryWatcher<PlayerInput>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2FD));
                IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x575));

                ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                Checkpoint = new MemoryWatcher<TTCheckpoint>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x3D0));
                ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x3F9));
                FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x424));
                StarTime = new MemoryWatcher<TTStars>(new DeepPointer(0x01C14D5C, 0x54, 0x1E0, 0x318, 0x408));

                /* Bag Counter */
                BagCounter = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0x194, 0x128, 0x3C, 0x11C, 0x64, 0x4C, 0x7A4));
            }
            else if (version == GameVersion.Origin)
            {
                /* Full Game/General Pointers */
                IsLoading = new MemoryWatcher<int>(new DeepPointer(0x01B685BC));
                IsSaving = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0xCC));
                IsPauseMenu = new MemoryWatcher<int>(new DeepPointer(0x01C11BE0, 0x32C));
                IsWhiteScreen = new MemoryWatcher<int>(new DeepPointer(0x1C19AF8));

                RespawnCP = new StringWatcher(new DeepPointer(0x01C6EFE0, 0x134, 0xBC, 0x0, 0x3C, 0x0), 58);
                PersistentLevel = new StringWatcher(new DeepPointer(0x01C11BE0, 0x330, 0x0), 48);

                PlayerPos = new MemoryWatcher<Vector3f>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0xE8));

                MoveState = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x500));
                AllowMoveChange = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x41D));
                IgnorePlayerInput = new MemoryWatcher<PlayerInput>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2FD));
                IgnoreButtonInput = new MemoryWatcher<byte>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x575));

                ObjectPosZ = new MemoryWatcher<float>(new DeepPointer(0x01C6E50C, 0xCC, 0x1CC, 0x2F8, 0x74, 0xF0));

                /* 69 Stars Pointers */
                Checkpoint = new MemoryWatcher<TTCheckpoint>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x3D0));
                ActiveTTStretch = new MemoryWatcher<byte>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x3F9));
                FinishedTime = new MemoryWatcher<float>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x424));
                StarTime = new MemoryWatcher<TTStars>(new DeepPointer(0x01C14D64, 0x54, 0x1E0, 0x318, 0x408));

                /* Bag Counter */
                BagCounter = new MemoryWatcher<int>(new DeepPointer(0x01C6EFE0, 0x194, 0x128, 0x3C, 0x11C, 0x64, 0x4C, 0x7A4));
            }

            AddRange(GetType().GetProperties()
                .Where(p => !p.GetIndexParameters().Any())
                .Select(p => p.GetValue(this, null) as MemoryWatcher)
                .Where(p => p != null));
        }
    }

    class GameMemory : IDisposable
    {
        public event EventHandler OnStart;
        public event EventHandler OnPause;
        public event EventHandler OnUnpause;
        public event EventHandler OnSplit;
        public event EventHandler OnTTSplit;
        public event EventHandler On3StarSplit;
        public event EventHandler OnSDSplit;
        public event EventHandler OnReset;
        public event EventHandler OnEnd;

        private GameData _data;
        private Process _process;
        private Task _pipeThread;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private bool _pipeInjected;
        private bool _pipeConnected;

        private enum ExpectedDLLSizes
        {
            Steam1 = 32632832,
            Steam2 = 32976896,
            Reloaded1 = 60298504,
            Reloaded2 = 42876928,
            Origin = 42889216
        }

        private bool _timerStarted = false;
        private bool _pipePause = false;
        private bool _streaming = false;
        private bool _menuWhileStreaming = false;

        private Vector3f sdExitBtn = new Vector3f(925.2f, -6835.7f, -3130.8f);

        private readonly Dictionary<string, int> levels = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"TdMainMenu", -2},
            {"tutorial_p", -1},
            {"edge_p", 0},
            {"escape_p", 1},
            {"stormdrain_p", 2},
            {"cranes_p", 3},
            {"subway_p", 4},
            {"mall_p", 5},
            {"factory_p", 6},
            {"boat_p", 7},
            {"convoy_p", 8},
            {"scraper_p", 9}
        };

        public GameMemory()
        {
            _pipeThread = Task.Factory.StartNew(PipeThread);
        }

        public void Update()
        {
            if (_process == null || _process.HasExited)
            {
                _pipeInjected = false;
                _pipeConnected = false;

                if (!TryGetGameProcess())
                    return;
            }

            _data.UpdateAll(_process);

            Vector3f PlayerPos = new Vector3f(_data.PlayerPos.Current.X, _data.PlayerPos.Current.Y, _data.PlayerPos.Current.Z);
            bool bIgnoreMoveInput = (_data.IgnorePlayerInput.Current.Move & 1) != 0;
            bool bIgnoreLookInput = (_data.IgnorePlayerInput.Current.Look & 1) != 0;
            bool bIgnoreButtonInput = (_data.IgnoreButtonInput.Current & 1) != 0;
            bool bAllowMoveChange = (_data.AllowMoveChange.Current & (1 << 4)) != 0;

            /* Starting Conditions */

            // Full Game
            if (_data.PersistentLevel.Current == "tutorial_p" && _data.PlayerPos.Current.Z < 5854f && _data.PlayerPos.Current.Z > 5800f && !_timerStarted)
            {
                _timerStarted = true;
                OnStart?.Invoke(this, EventArgs.Empty);
                Debug.WriteLine("[MELRT] starting timer (full game)");
            }

            // 69 Stars
            if (_data.PersistentLevel.Current == "tt_TutorialA01_p" && _data.PlayerPos.Current.Z < 5086f && _data.PlayerPos.Current.Z > 5085f && !_timerStarted)
            {
                _timerStarted = true;
                OnStart?.Invoke(this, EventArgs.Empty);
                Debug.WriteLine("[MELRT] starting timer (69 Stars)");
            }

            /* Resetting Conditions */

            // Full Game
            if (_data.PersistentLevel.Old == "TdMainMenu" && _data.PersistentLevel.Current == "tutorial_p" && _timerStarted)
            {
                _timerStarted = false;
                OnReset?.Invoke(this, EventArgs.Empty);
                Debug.WriteLine("[MELRT] resetting timer");
            }

            if (_data.PersistentLevel.Old == "TdMainMenu" && _data.PersistentLevel.Current == "tt_TutorialA01_p" && _timerStarted)
            {
                _timerStarted = false;
                OnReset?.Invoke(this, EventArgs.Empty);
                Debug.WriteLine("[MELRT] resetting timer");
            }

            /* Splits & Load Removing */

            if (_timerStarted || !_timerStarted)
            {
                // Stormdrains Exit Split
                if (PlayerPos.DistanceXY(sdExitBtn) < 120f)
                {
                    OnSDSplit?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("[MELRT] splitting (sd exit)");
                }

                // Chapter Split
                if (_data.PersistentLevel.Current != _data.PersistentLevel.Old && levels[_data.PersistentLevel.Current] > levels[_data.PersistentLevel.Old] && _data.PersistentLevel.Old != "tutorial_p" && _data.PersistentLevel.Old != "TdMainMenu")
                {
                    OnSplit?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("[MELRT] splitting (chapter end)");
                }

                // Helicopter Grab (Full Game End)
                if (_data.RespawnCP.Current == "End_game" && _data.ObjectPosZ.Current == 75102 && _data.ObjectPosZ.Old != 75102)
                {
                    OnEnd?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("[MELRT] splitting (full game end)");
                }

                /* 69 Stars Splitting */
                if (_data.Checkpoint.Current.CurrentCP > _data.Checkpoint.Old.CurrentCP)
                {
                    if (_data.Checkpoint.Current.CurrentCP == _data.Checkpoint.Current.TotalCPs)
                    {
                        // 3 Star Requirement
                        if (_data.StarTime.Current.Three > Math.Round(_data.FinishedTime.Current, 2))
                        {
                            On3StarSplit?.Invoke(this, EventArgs.Empty);
                        }

                        OnTTSplit?.Invoke(this, EventArgs.Empty);
                        Debug.WriteLine("[MELRT] tt split");
                    }
                }

                // If the player opens the menu during level streaming, the timer will pause and stay paused until they close the menu
                if (_streaming || _menuWhileStreaming)
                {
                    if (_data.IsPauseMenu.Current == 1)
                    {
                        _menuWhileStreaming = true;
                        OnPause?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        _menuWhileStreaming = false;
                        OnUnpause?.Invoke(this, EventArgs.Empty);
                    }
                }
                else if (_pipePause)
                {
                    OnPause?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("[MELRT] hooked load");
                }
                else if (_data.IsWhiteScreen.Current >= 1 && _data.IsPauseMenu.Current != 1)
                {
                    OnPause?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("[MELRT] white screen");
                }
                else if (_data.IsSaving.Current == 1 && _data.IsPauseMenu.Current == 1)
                {
                    OnPause?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("[MELRT] saving while in menu");
                }
                else if (bIgnoreMoveInput && bIgnoreLookInput && bIgnoreButtonInput && bAllowMoveChange && _data.IsLoading.Current == 0)
                {
                    OnPause?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("[MELRT] block while loading");
                }
                else if ((_data.RespawnCP.Current == "SWAT_Response2" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current > 4641) || // Prologue
                    (_data.RespawnCP.Current == "Plaza" && _data.IsSaving.Current == 1 && _data.PlayerPos.Current.Z < -2466) || // Chapter 1
                    (_data.RespawnCP.Current == "Boss_checkpoint1" && _data.IsSaving.Current == 1 && _data.PlayerPos.Current.X == 20804.95117f) || // Chapter 2
                    (_data.RespawnCP.Current != null && _data.RespawnCP.Current.Contains("SP03_Plaza_") && _data.IsSaving.Current == 1 && bIgnoreButtonInput) || // Chapter 3
                    (_data.RespawnCP.Current == "Train_Ride_End" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current == -1728) || // Chapter 4
                    (_data.RespawnCP.Current == "LastCP" && _data.IsSaving.Current == 1 && _data.PlayerPos.Current.X == -51617.66406f) || // Chapter 5
                    (_data.RespawnCP.Current == "Pursuit_chase_end" && _data.IsSaving.Current == 1 && _data.ObjectPosZ.Current > 1252) || // Chapter 6
                    (_data.RespawnCP.Current == "Celeste" && _data.IsSaving.Current == 1 && _data.PlayerPos.Current.Z == 349.1499939f) || // Chapter 7
                    (_data.RespawnCP.Current == "Atrium_soft_cp" && _data.IsSaving.Current == 1 && bIgnoreButtonInput) // Chapter 8
                        )
                {

                    OnPause?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("[MELRT] chapter ending save icon");
                }
                else
                {
                    OnUnpause?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        void PipeThread()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    while (!TryGetGameProcess())
                    {
                        Thread.Sleep(250);
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                            return;
                    }

                    Debug.WriteLine("[MELRT] got game process");

                    if (!_pipeInjected)
                    {
                        if (!ProcessHasModule(_process, GetGameDLLPath()))
                        {
                            InjectDLL(_process, GetGameDLLPath());
                        }
                        _pipeInjected = true;
                    }

                    Debug.WriteLine("[MELRT] dll injected");

                    using (var pipe = new NamedPipeClientStream(".", "LiveSplit.MirrorsEdgeLRT", PipeDirection.In, PipeOptions.Asynchronous))
                    {
                        while (!_process.HasExited)
                        {
                            try
                            {
                                pipe.Connect(250);
                                break;
                            }
                            catch (TimeoutException) { }
                            catch (IOException) { }
                        }

                        if (_process.HasExited || !pipe.IsConnected)
                            continue;

                        Debug.WriteLine("[MELRT] pipe client connected");

                        _pipeConnected = true;

                        var buf = new byte[2048];
                        pipe.BeginRead(buf, 0, buf.Length, PipeRead, new PipeState { Buffer = buf, Pipe = pipe });

                        while (_pipeConnected)
                        {
                            Thread.Sleep(250);
                            if (_cancellationTokenSource.Token.IsCancellationRequested)
                                return;
                        }

                        Debug.WriteLine("[MELRT] pipe client disconnected");
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    Thread.Sleep(1000);
                }
            }
        }

        void PipeRead(IAsyncResult ar)
        {
            var state = (PipeState)ar.AsyncState;

            int read;
            if ((read = state.Pipe.EndRead(ar)) == 0)
            {
                _pipeConnected = false;
                return;
            }

            string message = Encoding.Unicode.GetString(state.Buffer, 0, read);
            Debug.WriteLine("[MELRT] Received message from pipe: " + message);

            if (message == "pause")
            {
                _pipePause = true;
            }
            else if (message == "unpause")
            {
                _pipePause = false;
            }
            else if (message == "streaming")
            {
                Debug.WriteLine("[MELRT] streaming true");
                _streaming = true;
            }
            else if (message == "end streaming")
            {
                _streaming = false;
            }

            state.Pipe.BeginRead(state.Buffer, 0, state.Buffer.Length, PipeRead, state);
        }

        bool TryGetGameProcess()
        {
            Process _game = Process.GetProcesses()
                .FirstOrDefault(p => p.ProcessName.ToLower() == "mirrorsedge" && !p.HasExited && ProcessHasModule(p, "OpenAL32.dll"));

            if (_game == null)
            {
                return false;
            }

            GameVersion _version;

            if (_game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDLLSizes.Steam1 || _game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDLLSizes.Steam2)
            {
                _version = GameVersion.Steam;
            }
            else if (_game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDLLSizes.Reloaded1 || _game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDLLSizes.Reloaded2)
            {
                _version = GameVersion.Reloaded;
            }
            else if (_game.MainModuleWow64Safe().ModuleMemorySize == (int)ExpectedDLLSizes.Origin)
            {
                _version = GameVersion.Origin;
            }
            else
            {
                MessageBox.Show("Unexpected game version.", "LiveSplit.MirrorsEdge",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Debug.WriteLine("[MELRT] game version " + _version);
            _data = new GameData(_version);
            _process = _game;
            return true;
        }

        static bool ProcessHasModule(Process process, string module)
        {
            return process.ModulesWow64Safe().Any(m => m.ModuleName.ToLower() == module.ToLower());
        }

        public void Dispose()
        {
            if (_cancellationTokenSource == null || _pipeThread == null || _pipeThread.Status != TaskStatus.Running)
                return;

            _cancellationTokenSource.Cancel();
            _pipeThread.Wait();
        }

        static string GetGameDLLPath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? String.Empty;
            return Path.Combine(dir, "MELRT.dll");
        }

        static void InjectDLL(Process process, string path)
        {
            IntPtr loadLibraryAddr = Get32BitLoadLibraryAddress();
            Debug.WriteLine("LoadLibraryA address = 0x" + loadLibraryAddr.ToString("X"));
            if (loadLibraryAddr == IntPtr.Zero)
                throw new Exception("Couldn't locate LoadLibraryA");

            IntPtr mem = IntPtr.Zero;
            IntPtr hThread = IntPtr.Zero;
            uint len = 0;

            try
            {
                if ((mem = SafeNativeMethods.VirtualAllocEx(process.Handle, IntPtr.Zero, (uint)path.Length,
                    SafeNativeMethods.AllocationType.Commit | SafeNativeMethods.AllocationType.Reserve,
                    SafeNativeMethods.MemoryProtection.ReadWrite)) == IntPtr.Zero)
                    throw new Win32Exception();

                byte[] bytes = Encoding.ASCII.GetBytes(path + "\0");
                len = (uint)bytes.Length;
                uint written;
                if (!SafeNativeMethods.WriteProcessMemory(process.Handle, mem, bytes, len, out written))
                    throw new Win32Exception();

                if ((hThread = SafeNativeMethods.CreateRemoteThread(process.Handle, IntPtr.Zero, 0, loadLibraryAddr, mem, 0, IntPtr.Zero))
                    == IntPtr.Zero)
                    throw new Win32Exception();

                SafeNativeMethods.WaitForSingleObject(hThread, 0xFFFFFFFF); // INFINITE
            }
            finally
            {
                if (mem != IntPtr.Zero && len > 0)
                    SafeNativeMethods.VirtualFreeEx(process.Handle, mem, len, SafeNativeMethods.FreeType.Release);
                if (hThread != IntPtr.Zero)
                    SafeNativeMethods.CloseHandle(hThread);
            }
        }

        static IntPtr Get32BitLoadLibraryAddress()
        {
            if (!Environment.Is64BitProcess)
                return SafeNativeMethods.GetProcAddress(SafeNativeMethods.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            string componentsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? String.Empty;
            string exe = Path.Combine(componentsDir, "menl_findloadlibrary.exe");
            if (!File.Exists(exe))
                return IntPtr.Zero;

            using (var proc = new Process())
            {
                proc.StartInfo.FileName = exe;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit();
                return (IntPtr)proc.ExitCode;
            }
        }

    }
    struct PipeState
    {
        public NamedPipeClientStream Pipe;
        public byte[] Buffer;
    }
    enum GameVersion
    {
        Steam,
        Reloaded,
        Origin
    }
}
