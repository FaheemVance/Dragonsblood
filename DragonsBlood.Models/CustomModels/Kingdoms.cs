using System.ComponentModel;

namespace DragonsBlood.Models.CustomModels
{
    public enum Kingdoms
    {
        [Description("High Kingdom")]
        HighKingdom,
        [Description("Ice Storm Mountains")]
        IceStormMountains,
        [Description("Dark Marshes")]
        DarkMarshes
    }

    public enum ShortKingdom
    {
        High,
        Ice,
        Dark
    }
}
