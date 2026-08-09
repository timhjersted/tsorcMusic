using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace tsorcMusic
{
    public class tsorcMusicConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        public bool Debug;
    }
}
