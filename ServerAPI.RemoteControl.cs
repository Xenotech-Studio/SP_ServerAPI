using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSystem.Http.DataStructures;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DataSystem.Http
{
    
    public class RemoteControl : MonoBehaviour
    {
         public Button vehicleSetShakeButton;
         public Button vehicleStopShakeButton;
         public Button vehicleResetPoseButton;
         public Button vehicleRollButton;
         public Button vehiclePitchButton;
         public Button vehicleSetHeightButton;
         public Button vehicleSetAttitudeButton;
         public Button setFanOnButton;
         public Button setFanOffButton;
         public Button setThermalOnButton;
         public Button setThermalOffButton;
         public Button setSprayOnButton;
         public Button setSprayOffButton;
         public Button setAudioOnButton;
         public Button muteAllAudioButton;

         void Start()
         { 
             vehicleSetShakeButton.onClick.AddListener(OnVehicleSetShake);
             vehicleStopShakeButton.onClick.AddListener(OnVehicleStopShake);
             vehicleResetPoseButton.onClick.AddListener(OnVehicleResetPose);
             vehicleRollButton.onClick.AddListener(OnVehicleRoll); 
             vehiclePitchButton.onClick.AddListener(OnVehiclePitch);
             vehicleSetHeightButton.onClick.AddListener(OnVehicleSetHeight);
             vehicleSetAttitudeButton.onClick.AddListener(OnVehicleSetAttitude);
             setFanOnButton.onClick.AddListener(OnSetFanOn);
             setFanOffButton.onClick.AddListener(OnSetFanOff);
             setThermalOnButton.onClick.AddListener(OnSetThermalOn);
             setThermalOffButton.onClick.AddListener(OnSetThermalOff);
             setSprayOnButton.onClick.AddListener(OnSetSprayOn);
             setSprayOffButton.onClick.AddListener(OnSetSprayOff);
             setAudioOnButton.onClick.AddListener(OnSetAudioOn);
             muteAllAudioButton.onClick.AddListener(OnMuteAllAudio);
                
         }
         
         private void OnVehicleSetShake()
         {
             ServerAPI.VehicleSetShake(VehicleID.Vehicle_1, 30, 60, 100, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
         }
         
         private void OnVehicleStopShake()
         {
             ServerAPI.VehicleStopShake(VehicleID.Vehicle_1, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
         }
         
         private void OnVehicleResetPose()
         {
             ServerAPI.VehicleResetPose(VehicleID.Vehicle_1, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
         }
         
        private void OnVehicleRoll()
        {
            ServerAPI.VehicleRoll(VehicleID.Vehicle_1, 3.5, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
        
        private void OnVehiclePitch()
        {
            ServerAPI.VehiclePitch(VehicleID.Vehicle_1, 3.0, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
        
        private void OnVehicleSetHeight()
        {
            ServerAPI.VehicleSetHeight(VehicleID.Vehicle_1, 100.0, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
        
        private void OnVehicleSetAttitude()
        {
            ServerAPI.VehicleSetAttitude(VehicleID.Vehicle_1, 30.0, 3.0, 4.0, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
        
        private void OnSetFanOn()
        {
            ServerAPI.SetFanSpeed(FanID.Fan_A, FanSpeed.Speed_1, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
        
        private void OnSetFanOff()
        {
            ServerAPI.SetFanSpeed(FanID.Fan_A, FanSpeed.Speed_OFF, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
        
        private void OnSetThermalOn()
        {
            ServerAPI.SetHeatSprayState(HeatSprayID.Thermal_A, HeatSprayState.Device_ON, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
        
        private void OnSetThermalOff()
        {
            ServerAPI.SetHeatSprayState(HeatSprayID.Thermal_A, HeatSprayState.Device_OFF, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
        
        private void OnSetSprayOn()
        {
            ServerAPI.SetHeatSprayState(HeatSprayID.Spray, HeatSprayState.Device_ON, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }

        private void OnSetSprayOff()
        {
            ServerAPI.SetHeatSprayState(HeatSprayID.Spray, HeatSprayState.Device_OFF, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }

        private void OnSetAudioOn()
        {
            ServerAPI.SetAudio(SourceID.Game01, "I01O02", onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }

        private void OnMuteAllAudio()
        {
            ServerAPI.FullMute(SourceID.Game01, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
        }
    }
}


            