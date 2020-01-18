using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MoveCostMapTile
{
    public MapTile MapTile { get; private set; }
    public MoveCostMapTile PreviousMapTile { get; set; }
    public int Cost { get; set; }

    public MoveCostMapTile(MapTile originalTile, int startingCost, MoveCostMapTile previousTile)
    {
        MapTile = originalTile;
        Cost = startingCost;
        PreviousMapTile = previousTile;
    }
}
