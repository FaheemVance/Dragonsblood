using System;
using System.Linq;

namespace DragonsBlood.Models.AlertModels
{
    public class Coordinates
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinates Parse(string input)
        {
            var sep = input.Split(new[] {":"}, StringSplitOptions.None);
            if(sep.Length > 2)
                return null;

            var coord = new Coordinates();
            int x;
            int y;

            var successX = int.TryParse(sep.First(), out x);
            var successY = int.TryParse(sep.Last(), out y);

            if (!successX || !successY)
                return null;

            coord.X = x;
            coord.Y = y;

            return coord;
        }
    }
}
