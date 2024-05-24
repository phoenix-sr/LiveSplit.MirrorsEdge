using System.Reflection;
using LiveSplit.MirrorsEdge;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

[assembly: ComponentFactory(typeof(MirrorsEdgeFactory))]

namespace LiveSplit.MirrorsEdge
{
    public class MirrorsEdgeFactory : IComponentFactory
    {
        public string ComponentName => "Mirror's Edge";
        public string Description => "Automates splitting and load removal for Mirror's Edge.";
        public ComponentCategory Category => ComponentCategory.Control;

        public IComponent Create(LiveSplitState state)
        {
            return new MirrorsEdgeComponent(state);
        }

        // TODO: Implement Update Info
        public string UpdateName => this.ComponentName;
        public string UpdateURL => "";
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public string XMLURL => this.UpdateURL + "";
    }
}
