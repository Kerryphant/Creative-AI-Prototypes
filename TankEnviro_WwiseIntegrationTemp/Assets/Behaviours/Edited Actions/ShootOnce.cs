using UnityEngine;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using BBUnity.Actions;        // GOAction

using System.Collections;

namespace BBSamples.PQSG // Programmers Quick Start Guide
{
    /// <summary>
    /// DoneShootOnce is a action inherited from GOAction and Clone a 'bullet' and shoots 
    /// it throught the Forward axis with the specified velocity.
    /// </summary>
    [Action("Samples/ProgQuickStartGuide/ShootOnce")]
    [Help("Clone a 'bullet' and shoots it throught the Forward axis with the " +
          "specified velocity.")]
    public class DoneShootOnce : GOAction
    {
        /// <summary>Initialization method of DoneShootOnce.</summary>
        /// <remarks>If the shootPoint is not established, we look for the shooting point.</remarks>

        // Initialization method. If not established, we look for the shooting point.
        /*public override void OnStart()
        {
            base.OnStart();
        } // OnStart*/


        /// <summary>Update method of DoneShootOnce.</summary>
        /// <remarks>Instantiate the bullet prefab, Search the RigitBody component in bullet instance. We add a rigitBody to bullet 
        /// if doesn´t exist, and then we give it a velocity.</remarks>
        /// <returns>Return FAILED if the shootPoint is null, and COMPLETE otherwise.</returns>
        // Main class method, invoked by the execution engine.
        public override TaskStatus OnUpdate()
        {
            //Get the TankAgent component of the gameObject (tank that is shooting)
            //and call the Shoot function from that component
            gameObject.GetComponent<TankAgent>().Shoot();

            // The action is completed. We must inform the execution engine.
            return TaskStatus.COMPLETED;
        } // OnUpdate

    } // class DoneShootOnce

} // namespace