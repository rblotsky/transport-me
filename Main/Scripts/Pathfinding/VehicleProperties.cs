using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Pathfinding
{
    public partial class VehicleProperties : RefCounted
    {
        [Export] private float chassisLength;
        [Export] private float speed;
        [Export] private float distance;
        [Export] private float friction;
        //note: when making realistic values,
        //keep braking to be 1.2x - 1.5x times stronger than acceleration
    }
}
