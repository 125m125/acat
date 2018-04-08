using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.SerialActuator
{
    [DescriptorAttribute("FE3DE70B-8084-46C1-BAB1-905B215C773a",
                            "Serial Actuator",
                            "An Actuator that receives events throuth a serial port")]
    class SerialActuator : ActuatorBase
    {

        /// <summary>
        /// The settings object for this actuator
        /// </summary>
        public static Settings SerialActuatorSettings = null;

        /// <summary>
        /// Name of the file that stores the settings for
        /// this actuator
        /// </summary>
        private const String SettingsFileName = "SampleActuatorSettings.xml";

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        private SerialPort _serialPort;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public SerialActuator()
        {
        }

        /// <summary>
        /// Class factory to create the switch object
        /// </summary>
        /// <returns>the switch object</returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new SerialActuatorSwitch();
        }

        /// <summary>
        /// Initialize resources
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        public override bool Init()
        {
            Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
            SerialActuatorSettings = Settings.Load();

            // TODO perform initialization here.
            _serialPort = new SerialPort(findPort(), 9600, Parity.None, 8, StopBits.One);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            _serialPort.Open();

            actuatorState = State.Running;

            OnInitDone();

            return true;
        }

        private String findPort()
        {
            //TODO
            return "com3";
        }

        /// <summary>
        /// Pause the actuator
        /// </summary>
        public override void Pause()
        {
            actuatorState = State.Paused;
        }

        /// <summary>
        /// Resume the actuator
        /// </summary>
        public override void Resume()
        {
            actuatorState = State.Running;
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
        /// Find the switch that deals with the input detected
        /// </summary>
        /// <param name="switchSource">The source name of the switch</param>
        /// <returns>Switch object, null if not found</returns>
        private IActuatorSwitch find(String switchSource)
        {
            foreach (IActuatorSwitch switchObj in Switches)
            {
                if (switchObj is SerialActuatorSwitch)
                {
                    var actuatorSwitch = (SerialActuatorSwitch)switchObj;
                    if (actuatorSwitch.Source == switchSource)
                    {
                        return new SerialActuatorSwitch(switchObj);
                    }
                }
            }

            return null;
        }

        public override void OnRegisterSwitches()
        {
            SwitchSetting switchSetting = new SwitchSetting("SerSwi", "Serial Input Reader", "CmdMainMenu");
            Context.AppActuatorManager.RegisterSwitch(this, switchSetting);
        }

        /// <summary>
        /// Event handler for when a swtich activate event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchActivate()
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find("SerSwi");
                if (actuatorSwitch != null)
                {
                    OnSwitchActivated(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Event handler for when a swtich deactivate event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchDeactivate()
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find("SerSwi");
                if (actuatorSwitch != null)
                {
                    OnSwitchDeactivated(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Event handler for when a swtich trigger event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchTrigger()
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find("SerSwi");
                if (actuatorSwitch != null)
                {
                    OnSwitchTriggered(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <returns></returns>
        private void unInit()
        {
            actuatorState = State.Stopped;

            // perform unitialization here
            _serialPort.Close();
        }

        private static string data = "";

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            data += sp.ReadExisting();

            int nl = data.IndexOf("\n");
            while((nl = data.IndexOf("\n")) > -1)
            {
                string line = data.Substring(0, nl);
                data = data.Substring(nl + 1);


                if (line.EndsWith("false"))
                {
                    sensor_EvtSwitchTrigger();
                };
            }
        }
    }
}
