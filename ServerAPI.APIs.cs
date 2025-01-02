using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSystem.Http.DataStructures;
using Newtonsoft.Json;

namespace DataSystem.Http
{

    public enum VehicleID
    {
        Vehicle_1 = 1001,
        Vehicle_2 = 1002,
    }

    public enum FanID
    {
        Fan_A = 1201,
        Fan_B = 1202,
        Fan_C = 1203,
        Fan_D = 1204,
        Fan_E = 1205,
    }

    public enum FanSpeed
    {
        Speed_OFF = 0,
        Speed_1 = 1,
        Speed_2 = 2,
        Speed_3 = 3,
    }
    public enum HeatSprayID
    {
        Thermal_A = 1401,
        Thermal_B = 1402,
        Spray = 1801,
    }

    public enum HeatSprayState
    {
        Device_OFF = 0,
        Device_ON = 1,
    }

    public enum SourceID
    {
        Game01 = 01,
        Game02 = 02,
    }
    
    public partial class ServerAPI
    {
        
        
        // **************** Dynamic Center POST ****************
        
        // Send Command (callback style and async style)
        public static void SendRawCommand(string command, Action<DynamicCenterResult> onResult) => ToCallback(SendRawCommand(command), onResult);
        public static async Task<DynamicCenterResult> SendRawCommand(string the_command) => 
            await Post<DefaultJson, DynamicCenterResult>("send", new Dictionary<string, string>(){{"data", the_command}}, new DefaultJson());
        
        public static void SendRawCommandAudio(string command, Action<DynamicCenterResult> onResult) => ToCallback(SendRawCommandAudio(command), onResult);
        public static async Task<DynamicCenterResult> SendRawCommandAudio(string the_command) => 
            await Post<DefaultJson, DynamicCenterResult>("send_audio", new Dictionary<string, string>(){{"data", the_command}}, new DefaultJson());  
        
        
        // 新增两个API
        public static void VehicleStartActionPlay(VehicleID vehicle_id, Action<DynamicCenterResult> onResult = null)
        {  
            string command = "StartActionPlay" + ":" + (int)vehicle_id;
            SendRawCommand(command, onResult: onResult);
        }
        
        public static void VehicleStopActionPlay(VehicleID vehicle_id, Action<DynamicCenterResult> onResult = null)
        {  
            string command = "StopActionPlay" + ":" + (int)vehicle_id;
            SendRawCommand(command, onResult: onResult);
        }
        // End of 新增两个API
        
        
        public static void VehicleSetShake(VehicleID vehicle_id, int min_amplitude, int max_amplitude, int duration, Action<DynamicCenterResult> onResult = null)
        {  
            string command = "SetShake" + ":" + (int)vehicle_id + "," + min_amplitude + "," + max_amplitude + "," + duration;
            SendRawCommand(command, onResult: onResult);
        }
        
        public static void VehicleStopShake(VehicleID vehicle_id, Action<DynamicCenterResult> onResult = null)
        {
            string command = "StopShake" + ":" + (int)vehicle_id;
            SendRawCommand(command, onResult: onResult);
        }
        
        public static void VehicleResetPose(VehicleID vehicle_id, Action<DynamicCenterResult> onResult = null)
        {
            string command = "ResetPose" + ":" + (int)vehicle_id;
            SendRawCommand(command, onResult: onResult);
        }
        public static void VehicleRoll(VehicleID vehicle_id, double degrees, Action<DynamicCenterResult> onResult = null)
        {
            string command = "TurnRight" + ":" + (int)vehicle_id + "," + degrees;
            SendRawCommand(command, onResult: onResult);
        }
        
        public static void VehiclePitch(VehicleID vehicle_id, double degrees, Action<DynamicCenterResult> onResult = null)
        {
            string command = "HeadUp" + ":" + (int)vehicle_id + "," + degrees;
            SendRawCommand(command, onResult: onResult); 
        }
        
        public static void VehicleSetHeight(VehicleID vehicle_id, double height, Action<DynamicCenterResult> onResult = null)
        {
            string command = "SetHeight" + ":" + (int)vehicle_id + "," + height;
            SendRawCommand(command, onResult: onResult);
        }
        
        public static void VehicleSetAttitude(VehicleID vehicle_id, double height, double headUp, double turnRight, Action<DynamicCenterResult> onResult = null)
        {  
            string command = "SetPose" + ":" + (int)vehicle_id + "," + height + "," + headUp + "," + turnRight;
            SendRawCommand(command, onResult: onResult);
        }
        
        public static void SetFanSpeed(FanID fanId, FanSpeed fanSpeed,Action<DynamicCenterResult> onResult = null)
        {
            string command = "SetFan" + ":" + (int)fanId + "," + (int)fanSpeed;
            SendRawCommand(command, onResult: onResult);
        }
        public static void SetHeatSprayState(HeatSprayID heatSprayId, HeatSprayState heatSprayState,Action<DynamicCenterResult> onResult = null)
        {
            string command = "SetEff" + ":" + (int)heatSprayId + "," + (int)heatSprayState;
            string command2 = "SetEff" + ":" + (int)heatSprayId + "," + (int)heatSprayState;
            SendRawCommand(command, onResult: onResult);
        }

        public static void FullMute(SourceID sourceID, Action<DynamicCenterResult> onResult = null)
        {
            string source = Enum.GetName(typeof(SourceID), sourceID);
            string command = "AI00OAL<!"; 

            var jsonCommand = new
            {
                Source = source,
                Command = command
            };

            string jsonCommandString = JsonConvert.SerializeObject(jsonCommand);
            SendRawCommandAudio(jsonCommandString, onResult: onResult);
        }
        public static void SetAudio(SourceID sourceID, string inputCommand, Action<DynamicCenterResult> onResult = null)
        {
            string source = Enum.GetName(typeof(SourceID), sourceID);
            string command = $"A{inputCommand}<!"; 

            var jsonCommand = new
            {
                Source = source,
                Command = command
            };

            string jsonCommandString = JsonConvert.SerializeObject(jsonCommand);
            SendRawCommandAudio(jsonCommandString, onResult: onResult);
        }
        


        // **************** User GET/POST ****************
        
        /*
        
        // Get User By ID (callback style and async style)
        public static void GetUser(string userID, Action<UserData> onResult) => ToCallback(GetUser(userID), onResult);
        public static async Task<UserData> GetUser(string userID) => 
            await Get<UserData>("user", new ictionary<string,string>(){{"user_id", userID}});
        
        // Set User By ID (callback style and async style)
        public static void SetUser(string userID, UserData data, Action<UserData> onResult) => ToCallback(SetUser(userID, data), onResult);
        public static async Task<UserData> SetUser(string userID, UserData data) =>
            await Post<UserData, UserData>("user", new Dictionary<string,string>(){{"user_id", userID}}, data);
        
        
        // **************** Patient GET/POST ****************
        
        // Get Patient By ID (callback style and async style)
        public static void GetPatient(string patientID, Action<PatientData> onResult) => ToCallback(GetPatient(patientID), onResult);
        public static async Task<PatientData> GetPatient(string patientID) =>
            await Get<PatientData>("patient", new Dictionary<string,string>(){{"patient_id", patientID}});
        
        // Set Patient By ID (callback style and async style)
        public static void SetPatient(string patientID, PatientData data, Action<PatientData> onResult) => ToCallback(SetPatient(patientID, data), onResult);
        public static async Task<PatientData> SetPatient(string patientID, PatientData data) =>
            await Post<PatientData, PatientData>("patient", new Dictionary<string,string>(){{"patient_id", patientID}}, data);
        
        
        // **************** File GET/POST ****************
        
        // Get File By ID (callback style and async style)
        public static void GetFile(string fileID, Action<FileData> onResult) => ToCallback(GetFile(fileID), onResult);
        public static async Task<FileData> GetFile(string fileID) =>
            await Get<FileData>("file", new Dictionary<string,string>(){{"file_id", fileID}});
        
        // Set File By ID (callback style and async style)
        public static void SetFile(string fileID, FileData data, Action<FileData> onResult) => ToCallback(SetFile(fileID, data), onResult);
        public static async Task<FileData> SetFile(string fileID, FileData data) =>
            await Post<FileData, FileData>("file", new Dictionary<string,string>(){{"file_id", fileID}}, data);

		// FILTER File By Patient_ID (callback style and async style)
        public static void FilterPatientsFile(string patientID, Action<ListResult<FileData>> onResult) => ToCallback(FilterPatientsFile(patientID), onResult);
        public static async Task<ListResult<FileData>> FilterPatientsFile(string patientID) =>
            await Get<ListResult<FileData>>("file", new Dictionary<string,string>(){{"patient_id", patientID}});

		// FILTER File By User_ID and Type (callback style and async style)
        public static void FilterUsersFileOfType(string userID, FileType type, Action<ListResult<FileData>> onResult) => ToCallback(FilterUsersFileOfType(userID, type), onResult);
        public static async Task<ListResult<FileData>> FilterUsersFileOfType(string userID, FileType type) =>
            await Get<ListResult<FileData>>("file", new Dictionary<string,string>(){{"user_id", userID}, {"file_type", Enum.GetName(typeof(FileType), type)}});
            
        */ 
		
    }
}