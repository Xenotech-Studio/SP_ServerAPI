using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSystem.Http.DataStructures;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DataSystem.Http
{
    
    public class RemoteControl : MonoBehaviour
    {
        public TMP_Dropdown vehicleDropdown;
        public TMP_Dropdown fanDropdown;
        public TMP_Dropdown thermalDropdown;
        private VehicleID _selectedVehicle;
        private FanID _selectedFan;
        private HeatSprayID _selectedThermal;
        public Button vehicleStartActionPlayButton;
        public Button vehicleStopActionPlayButton;
        public Button vehicleSetShakeButton;
        public Button vehicleStopShakeButton;
        public Button vehicleResetPoseButton;
        public Button vehicleRollButton;
        public Button vehiclePitchButton;
        public Button vehicleSetHeightButton;
        public Button vehicleSetAttitudeButton;
        public Button setFanOnButtonSpeed1;
        public Button setFanOnButtonSpeed2;
        public Button setFanOnButtonSpeed3;
        public Button setFanOffButton;
        public Button setThermalOnButton;
        public Button setThermalOffButton;
        public Button setSprayOnButton;
        public Button setSprayOffButton;
        public Button setAudioOnButton;
        public Button muteAllAudioButton;
        

        void Start()
        {   
            // Vehicle Dropdown
            vehicleDropdown.options.Clear();
            foreach (VehicleID vehicle in System.Enum.GetValues(typeof(VehicleID)))
            {
                vehicleDropdown.options.Add(new TMP_Dropdown.OptionData(vehicle.ToString()));
            }
            vehicleDropdown.onValueChanged.AddListener(OnVehicleDropdownChanged);
            vehicleDropdown.value = 0; 
            _selectedVehicle = (VehicleID)System.Enum.Parse(typeof(VehicleID), vehicleDropdown.options[0].text);
            
            // Fan DropDown
            fanDropdown.options.Clear();
            foreach (FanID fan in System.Enum.GetValues(typeof(FanID)))
            {
                fanDropdown.options.Add(new TMP_Dropdown.OptionData(fan.ToString()));
            }
            fanDropdown.onValueChanged.AddListener(OnFanDropdownChanged);
            fanDropdown.value = 0; 
            _selectedFan = (FanID)System.Enum.Parse(typeof(FanID), fanDropdown.options[0].text);
            
            // Thermal DropDown
            thermalDropdown.options.Clear();
            foreach (HeatSprayID thermal in System.Enum.GetValues(typeof(HeatSprayID)))
            {
                if (thermal != HeatSprayID.Spray)
                {
                thermalDropdown.options.Add(new TMP_Dropdown.OptionData(thermal.ToString()));
                }
            }
            thermalDropdown.onValueChanged.AddListener(OnThermalDropdownChanged);
            thermalDropdown.value = 0;
            _selectedThermal = (HeatSprayID)System.Enum.Parse(typeof(HeatSprayID), thermalDropdown.options[0].text);
            
            
            vehicleSetShakeButton.onClick.AddListener(OnVehicleSetShake);
            vehicleStopShakeButton.onClick.AddListener(OnVehicleStopShake);
            vehicleStartActionPlayButton.onClick.AddListener(OnVehicleStartActionPlay);
            vehicleStopActionPlayButton.onClick.AddListener(OnVehicleStopActionPlay);
            vehicleResetPoseButton.onClick.AddListener(OnVehicleResetPose);
            vehicleRollButton.onClick.AddListener(OnVehicleRoll); 
            vehiclePitchButton.onClick.AddListener(OnVehiclePitch);
            vehicleSetHeightButton.onClick.AddListener(OnVehicleSetHeight);
            vehicleSetAttitudeButton.onClick.AddListener(OnVehicleSetAttitude);
            setFanOnButtonSpeed1.onClick.AddListener(OnSetFanOn_Speed1);
            setFanOnButtonSpeed2.onClick.AddListener(OnSetFanOn_Speed2);
            setFanOnButtonSpeed3.onClick.AddListener(OnSetFanOn_Speed3);
            setFanOffButton.onClick.AddListener(OnSetFanOff);
            setThermalOnButton.onClick.AddListener(OnSetThermalOn);
            setThermalOffButton.onClick.AddListener(OnSetThermalOff);
            setSprayOnButton.onClick.AddListener(OnSetSprayOn);
            setSprayOffButton.onClick.AddListener(OnSetSprayOff);
            setAudioOnButton.onClick.AddListener(OnSetAudioOn);
            muteAllAudioButton.onClick.AddListener(OnMuteAllAudio);
            
        }
        
        private void OnVehicleDropdownChanged(int index)
        {
            _selectedVehicle = (VehicleID)System.Enum.Parse(typeof(VehicleID), vehicleDropdown.options[index].text);
            Debug.Log("Selected Vehicle: " + _selectedVehicle);

            TooltipController.Instance.Show();
        }
        
        private void OnFanDropdownChanged(int index)
        {
            _selectedFan = (FanID)System.Enum.Parse(typeof(FanID), fanDropdown.options[index].text);
            Debug.Log("Selected Fan: " + _selectedFan);

            TooltipController.Instance.Show();
        }
        
        private void OnThermalDropdownChanged(int index)
        {
            _selectedThermal = (HeatSprayID)System.Enum.Parse(typeof(HeatSprayID), thermalDropdown.options[index].text);
            Debug.Log("Selected Thermal: " + _selectedThermal);

            TooltipController.Instance.Show();
        }
        
        private void OnVehicleSetShake()       
        {
            ServerAPI.VehicleSetShake(_selectedVehicle, 30, 60, 100, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnVehicleStopShake()
        {
            ServerAPI.VehicleStopShake(_selectedVehicle, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }

        private void OnVehicleStartActionPlay()
        {
            ServerAPI.VehicleStartActionPlay(_selectedVehicle, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnVehicleStopActionPlay()
        {
            ServerAPI.VehicleStopActionPlay(_selectedVehicle, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnVehicleResetPose()
        {
            ServerAPI.VehicleResetPose(_selectedVehicle, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
         
        private void OnVehicleRoll()
        {
            ServerAPI.VehicleRoll(_selectedVehicle, 3.5, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnVehiclePitch()
        {
            ServerAPI.VehiclePitch(_selectedVehicle, 3.0, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnVehicleSetHeight()
        {
            ServerAPI.VehicleSetHeight(_selectedVehicle, 100.0, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnVehicleSetAttitude()
        {
            ServerAPI.VehicleSetAttitude(_selectedVehicle, 30.0, 3.0, 4.0, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnSetFanOn_Speed1()
        {
            ServerAPI.SetFanSpeed(_selectedFan, FanSpeed.Speed_1, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnSetFanOn_Speed2()
        {
            ServerAPI.SetFanSpeed(_selectedFan, FanSpeed.Speed_2, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnSetFanOn_Speed3()
        {
            ServerAPI.SetFanSpeed(_selectedFan, FanSpeed.Speed_3, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnSetFanOff()
        {
            ServerAPI.SetFanSpeed(_selectedFan, FanSpeed.Speed_OFF, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnSetThermalOn()
        {
            ServerAPI.SetHeatSprayState(_selectedThermal, HeatSprayState.Device_ON, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnSetThermalOff()
        {
            ServerAPI.SetHeatSprayState(_selectedThermal, HeatSprayState.Device_OFF, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
        
        private void OnSetSprayOn()
        {
            ServerAPI.SetHeatSprayState(HeatSprayID.Spray, HeatSprayState.Device_ON, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }

        private void OnSetSprayOff()
        {
            ServerAPI.SetHeatSprayState(HeatSprayID.Spray, HeatSprayState.Device_OFF, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }

        private void OnSetAudioOn()
        {
            ServerAPI.SetAudio(SourceID.Game01, "I01O02", onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }

        private void OnMuteAllAudio()
        {
            ServerAPI.FullMute(SourceID.Game01, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            TooltipController.Instance.Show();
        }
    }
}


            