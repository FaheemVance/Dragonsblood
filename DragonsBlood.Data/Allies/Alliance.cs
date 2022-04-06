using DragonsBlood.Models.Ally;

namespace DragonsBlood.Data.Allies
{
    public class Alliance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AllianceType State { get; set; }
    }
}