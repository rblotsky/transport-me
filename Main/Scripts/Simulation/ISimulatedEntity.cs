using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISimulatedEntity
{
    public void BeforeSimulationStep(float deltaTimeInSim) { }
    public void AfterSimulationStep(float deltaTimeInSim) { }
    public void SimulationStep(float deltaTimeInSim) { }
}
