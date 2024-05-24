using LiveSplit.UI;
using System;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.MirrorsEdge
{
    public partial class MirrorsEdgeSettings : UserControl
    {
        public bool AutoStart { get; set; }
        public bool AutoReset { get; set; }
        public bool AutoSplit { get; set; }
        public bool SDSplit { get; set; }
        public bool StarsRequired { get; set; }
        public MirrorsEdgeSettings()
        {
            InitializeComponent();

            chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoSplit.DataBindings.Add("Checked", this, "AutoSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSDSplit.DataBindings.Add("Checked", this, "SDSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkStarsRequired.DataBindings.Add("Checked", this, "StarsRequired", false, DataSourceUpdateMode.OnPropertyChanged);

            // defaults
            AutoStart = true;
            AutoSplit = true;
            AutoReset = true;
            SDSplit = false;
            StarsRequired = true;
        }

        private void MirrorsEdgeSettings_Load(object sender, EventArgs e)
        {

        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            AutoStart = SettingsHelper.ParseBool(element["AutoStart"]);
            AutoSplit = SettingsHelper.ParseBool(element["AutoSplit"]);
            AutoReset = SettingsHelper.ParseBool(element["AutoReset"]);
            SDSplit = SettingsHelper.ParseBool(element["SDSplit"]);
            StarsRequired = SettingsHelper.ParseBool(element["StarsRequired"]);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "AutoStart", AutoStart) ^
                SettingsHelper.CreateSetting(document, parent, "AutoSplit", AutoSplit) ^
                SettingsHelper.CreateSetting(document, parent, "AutoReset", AutoReset) ^
                SettingsHelper.CreateSetting(document, parent, "SDSplit", SDSplit) ^
                SettingsHelper.CreateSetting(document, parent, "StarsRequired", StarsRequired);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private void chkSDSplit_CheckedChanged(object sender, EventArgs e)
        {
            SDSplit = chkSDSplit.Checked;
        }

        private void chkAutoReset_CheckedChanged(object sender, EventArgs e)
        {
            AutoReset = chkAutoReset.Checked;
        }

        private void chkAutoSplit_CheckedChanged(object sender, EventArgs e)
        {
            AutoSplit = chkAutoSplit.Checked;
        }

        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            AutoStart = chkAutoStart.Checked;
        }

        private void chkStarsRequired_CheckedChanged(object sender, EventArgs e)
        {
            StarsRequired = chkStarsRequired.Checked;
        }
    }
}
