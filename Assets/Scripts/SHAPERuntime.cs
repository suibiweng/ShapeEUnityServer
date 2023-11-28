using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine;
using System.Text;

public class SHAPERuntime : MonoBehaviour
{
    public string prompt;
    public int steps =64;//(8~64)
    public int cfg= 20;//(1~20)
    private string directoryPath;
    private string modelName;


    public enum Format
        {
            FBX,
            GLB,
            BLEND,
            OBJ,
            GLTF
        };
    public static Format format = Format.FBX;

    private bool postFlag = false;
    private int postProgress = 0;

     string textToMeshID = "6epa33dzxxx7c3";
    public string invoice;
    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(Verify($"https://{textToMeshID}-5000.proxy.runpod.net/verify", "{\"invoice\":\"" + invoice + "\"}"));
        // StartCoroutine(Post($"https://{textToMeshID}-5000.proxy.runpod.net/data", "{\"prompt\":\"" + $"{prompt}" + "\",\"steps\":\"" + $"{steps}" + "\",\"cfg\":\"" + $"{cfg}" + "\",\"invoice\":\"" + $"{invoice}" + "\",\"fileFormat\":\"" + $"{format}" + "\"}"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


            IEnumerator Post(string url, string bodyJsonString)
        {
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            postProgress = 1;
            postFlag = false;
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("There was an error in generating the model. \nPlease check your invoice/order number and try again or check the troubleshooting section in the documentation for more information." + "\nInfo: " + request.result + "\nError Code: " + request.responseCode);
            }
            else
            {
                if (request.downloadHandler.text == "Invalid Response")
                    Debug.Log("Invalid Invoice/Order Number. Please check your invoice/order number and try again");

                else if (request.downloadHandler.text == "Limit Reached")
                    Debug.Log("It seems that you may have reached the limit. To check your character usage, please click on the Status button. Please wait until the 1st of the next month to get a renewed character count. Thank you for using Shap-E for Unity.");
                else
                {
                    byte[] modelData = Convert.FromBase64String(request.downloadHandler.text);
                    File.WriteAllBytes($"Assets/Shap-E/Models/{modelName}.{format}", modelData);
                   // Debug.Log($"<color=green>Inference Successful: </color>Please find the model in the {directoryPath}");
                    // AssetDatabase.Refresh();
                   // Selection.activeObject = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath($"Assets/Shap-E/Models/{modelName}.{format}", typeof(UnityEngine.Object));
                }
            }

            request.Dispose();
        }

    IEnumerator Verify(string url, string bodyJsonString)
        {
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                if (request.downloadHandler.text == "Not Verified")
                    Debug.Log("Invoice/Order number verification unsuccessful. Please check your invoice/order number and try again or contact the publisher on the email given in the documentation.");
                else
                    Debug.Log($"Your invoice is verified. You have generated {request.downloadHandler.text} objects. Thank you for choosing Shap-E for Unity!");
            }
            request.Dispose();
        }
}
