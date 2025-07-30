using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportme.Main.Scripts.Simulation
{
    public interface ISimulatable
    {
        void BeforeSimulationStep();
        void Simulate(double delta);
        void AfterSimulationStep();
    }
}
