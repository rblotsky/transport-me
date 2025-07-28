using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISimulatedEntity
{
    public void BeforeSimulationStep(double deltaTimeInSim) { }
    public void AfterSimulationStep(double deltaTimeInSim) { }
    public void SimulationStep(double deltaTimeInSim) { }
}
