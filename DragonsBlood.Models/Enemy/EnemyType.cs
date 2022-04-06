using System.ComponentModel;

namespace DragonsBlood.Models.Enemy
{
    public enum EnemyType
    {
        [Description("At War")]
        AtWar,
        [Description("Cease Fire")]
        CeaseFire,
        [Description("Neutral")]
        Neutral,
        [Description("Unknown")]
        Unknown
    }
}