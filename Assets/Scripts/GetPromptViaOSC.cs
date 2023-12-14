using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;


class GenerateModelCommand{
    public string UserID;
    
    public string Prompt;
    public string modelID;
}

public class GetPromptViaOSC : MonoBehaviour
{

    public bool inProcess=false;
    public OSC osc;
    public SHAPERuntime sHAPERuntime;

    public Queue CommandsQueue;

    public InputField inputPrompt;


    // Start is called before the first frame update
    void Start()
    {
        osc.GetComponent<OSC>();
        CommandsQueue= new Queue ();
        osc.SetAddressHandler("/PromtGenerateModel",GenerateModel);
        
    }
    void DebugGenrate(){

        GenerateModelCommand command = new GenerateModelCommand{
            UserID="1",
            Prompt=inputPrompt.text,
            modelID="1233123"
        };

        CommandsQueue.Enqueue(command);



    }


    void GenerateModel(OscMessage message){
        print(message.values[0].ToString()+" "+message.values[0].ToString()+" "+message.values[0].ToString());
        GenerateModelCommand command = new GenerateModelCommand{
            UserID=message.values[0].ToString(),
            Prompt=message.values[1].ToString(),
            modelID=message.values[2].ToString()
        };

        CommandsQueue.Enqueue(command);


    }



    // Update is called once per frame
    void Update()
    {

        if(CommandsQueue.Count>0 && !inProcess){
            inProcess=true;
            ProcessingModel();
        }

    }

    int tempID=-1;

    void ProcessingModel(){


        GenerateModelCommand recive_command = (GenerateModelCommand)CommandsQueue.Dequeue();

        sHAPERuntime.sendPrompt(recive_command.UserID,recive_command.Prompt,recive_command.modelID);
        tempID=int.Parse(recive_command.UserID);




        








    }


    public void Done(){
        inProcess=false;

        print("Done!");

        StartCoroutine(delaySendToVR());





    }

    IEnumerator delaySendToVR(){
        yield return new WaitForSeconds(30f);
        
        OscMessage message =new OscMessage()
        {
            address="/GenrateModel"
        
        };


        message.values.Add(tempID);
        message.values.Add("http://34.106.250.143/upload/model.zip");
        tempID=-1;
        



        osc.Send(message);




    }


}
