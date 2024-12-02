using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSystem.Http.DataStructures;
using UnityEditor;
using UnityEngine;

namespace DataSystem.Http
{
    #if UNITY_EDITOR
    public partial class ServerAPI
    {
        [MenuItem("XenoSDK/Test HttpClient")]
        public static void Test()
        {
            // create a ServerTest GameObject and run the Test coroutine
            new GameObject("ServerTest").AddComponent<ServerAPITest>().StartCoroutine("Test");
        }
    }
    
    public partial class ServerAPITest : MonoBehaviour
    {
        IEnumerator Test()
        {
            // Test Server Connection
            //Task<ListResult<object>> task0 = ServerAPI.Get<ListResult<object>>("test");
           // yield return new WaitUntil(() => task0.IsCompleted);
            //Debug.Log("Test 0: Server Connection Test: " + ((ListResult<object>)task0.Result).Message);
            
            // Test SendCommand
            //yield return ServerAPI.SendRawCommand("SetShake:1001,30,60,100");
            
            //ServerAPI.SendRawCommand("SetShake:1001,30,60,100", onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });
            
            ServerAPI.VehicleSetShake(VehicleID.Vehicle_1, 30, 60, 100, onResult: (DynamicCenterResult res) => { Debug.Log(res.status); });

            yield return null;


            // // *********** test UserData ***********
            //
            // // 1. post user data
            // UserData user1 = new UserData(){UserID = "user_test", Name = "Mr. 111"};
            // yield return Server.SetUser("user_test", user1);
            // Debug.Log("Test 1: Posted user data, done.");
            //
            // // 2. get user data
            // Task<UserData> task2 = Server.GetUser("user_test");
            // yield return new WaitUntil(() => task2.IsCompleted);
            // UserData user2 = (UserData)task2.Result;
            // Debug.Log("Test 2: Got user data: " + user2.Name + (user2.Name == "Mr. 111" ? " (pass)" : " (fail)"));
            //
            // // 3. updating user name to "Mr. Test 2"
            // user2.Name = "Mr. 222";
            // yield return Server.SetUser("user_test", user2);
            // Debug.Log("Test 3: Updated user data, done.");
            //
            // // 4. get user data again, see whether the name is updated
            // Task<UserData> task4 = Server.GetUser("user_test");
            // yield return new WaitUntil(() => task4.IsCompleted);
            // UserData user4 = (UserData)task4.Result;
            // Debug.Log("Test 4: Got user data: " + user4.Name + (user4.Name == "Mr. 222" ? " (pass)" : " (fail)"));
            //
            //
            // // *********** do all the same to PatientData ***********
            //
            // // 5. post patient data
            // PatientData patient1 = new PatientData(){PatientID = "patient_test", Name = "Patient 111"};
            // yield return Server.SetPatient("patient_test", patient1);
            // Debug.Log("Test 5: Posted patient data, done.");
            //
            // // 6. get patient data
            // Task<PatientData> task6 = Server.GetPatient("patient_test");
            // yield return new WaitUntil(() => task6.IsCompleted);
            // PatientData patient2 = (PatientData)task6.Result;
            // Debug.Log("Test 6: Got patient data: " + patient2.Name + (patient2.Name == "Patient 111" ? " (pass)" : " (fail)"));
            //
            // // 7. updating patient name to "Patient 222"
            // patient2.Name = "Patient 222";
            // yield return Server.SetPatient("patient_test", patient2);
            // Debug.Log("Test 7: Updated patient data, done.");
            //
            // // 8. get patient data again, see whether the name is updated
            // Task<PatientData> task8 = Server.GetPatient("patient_test");
            // yield return new WaitUntil(() => task8.IsCompleted);
            // PatientData patient3 = (PatientData)task8.Result;
            // Debug.Log("Test 8: Got patient data: " + patient3.Name + (patient3.Name == "Patient 222" ? " (pass)" : " (fail)"));
            //
            //
            // // *********** test link user and patient ***********
            //
            // // 9. link user and patient
            // user4.Patients.Add("patient_test");
            // user4.Patients = new List<string>(new HashSet<string>(user4.Patients)); // remove duplicates
            // yield return Server.SetUser("user_test", user4);
            // Debug.Log("Test 9: Linked user and patient, done.");
            //
            // // 10. get user data again, see whether the patient is linked
            // Task<UserData> task10 = Server.GetUser("user_test");
            // yield return new WaitUntil(() => task10.IsCompleted);
            // UserData user5 = (UserData)task10.Result;
            // Debug.Log("Test 10: Got user data: " + user5.Patients[0] + (user5.Patients.Contains("patient_test") ? " (pass)" : " (fail)"));
            //
            //
            // // *********** also test single-create FileData ***********
            //
            // // 11. post file data
            // string url = "https://clinic-ar-1302933783.cos.ap-guangzhou.myqcloud.com/TEST_FOLDER/FireExtinguisher01_1.FBX";
            // FileData file1 = new FileData(){FileID = "file_test", PatientID = "patient_test", FileType = FileType.FootScan, DisplayType = DisplayType.StaticModel, URL = url};
            // yield return Server.SetFile("file_test", file1);
            // Debug.Log("Test 11: Posted file data, done.");
            //
            //
            // yield return Server.SetFile("test_image", new FileData()
            // {
            //     FileType = FileType.ImageAndVideo,
            //     FileID = "test_image",
            //     PatientID = "patient_test",
            //     DisplayType = DisplayType.Image,
            //     URL = "https://clinic-ar-1302933783.cos.ap-guangzhou.myqcloud.com/TEST_FOLDER/test_image.jpg",
            //     ThumbnailURL = "https://clinic-ar-1302933783.cos.ap-guangzhou.myqcloud.com/TEST_FOLDER/test_image_thumbnail.jpg"
            // });
            // Debug.Log("Test 11.5: Upload Test Image.");
            //
            // // 12. get file data
            // Task<FileData> task12 = Server.GetFile("file_test");
            // yield return new WaitUntil(() => task12.IsCompleted);
            // FileData file2 = (FileData)task12.Result;
            // Debug.Log("Test 12: Got file data: " + file2.PatientID + (file2.PatientID == "patient_test" ? " (pass)" : " (fail)"));
            //
            //
            // // *********** test filter files ***********
            //
            // // 13. get file by patient_id
            // Task<ListResult<FileData>> task13 = Server.FilterPatientsFile("patient_test");
            // yield return new WaitUntil(() => task13.IsCompleted);
            // List<FileData> files1 = task13.Result.Results;
            // Debug.Log("Test 13: Got files by patient_id: " + files1.Count + (files1.Count >= 1 ? " (pass)" : " (fail)"));
            //
            // // 14-15. get file by user_id and type
            // Task<ListResult<FileData>> task14 = Server.FilterUsersFileOfType("user_test", FileType.FootScan);
            // yield return new WaitUntil(() => task14.IsCompleted);
            // List<FileData> files2 = task14.Result.Results;
            //
            // Task<ListResult<FileData>> task15 = Server.FilterUsersFileOfType("user_test", FileType.ImageAndVideo);
            // yield return new WaitUntil(() => task15.IsCompleted);
            // List<FileData> files3 = task15.Result.Results;
            // bool files3ContainsFileTest = false;
            // foreach (var file in files3) { if (file.FileID == "file_test") { files3ContainsFileTest = true; break; } }
            // Debug.Log("Test 14: Got files by user_id and type: " + files2.Count + ((files2.Count >= 1 && !files3ContainsFileTest) ? " (pass)" : " (fail"));
        }
    }
    #endif
}


            