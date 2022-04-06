using DragonsBlood.Models.Enemy;

namespace DragonsBlood.Data.Enemies
{
    public class Enemy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnemyType State { get; set; }
    }
}