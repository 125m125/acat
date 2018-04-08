using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Extensions.Default.Actuators.SerialActuator
{
    class SerialActuatorSwitch: ActuatorSwitchBase
    {
        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public SerialActuatorSwitch()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.  Copies
        /// members over from switchObj
        /// </summary>
        /// <param name="switchObj">Switch object to clone</param>
        public SerialActuatorSwitch(IActuatorSwitch switchObj)
            : base(switchObj)
        {
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    if (disposing)
                    {
                        // release managed resources
                        unInit();
                    }

                    // Release the native unmanaged resources

                    _disposed = true;
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        private void unInit()
        {
        }
    }
}
