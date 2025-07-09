using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Pathfinding
{
    [GlobalClass]
    public partial class VehicleProperties : Resource
    {
        [Export] public float chassisLength;
        [Export] public float chassisWidth;
        [Export] public float maxSpeed;
        [Export] public float accelerationPower;
        [Export] public float brakingPower;
        //note: when making realistic values,
        //keep braking to be 1.2x - 1.5x times stronger than acceleration
    }
}
