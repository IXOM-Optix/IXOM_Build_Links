#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.SQLiteStore;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.Store;
using FTOptix.RAEtherNetIP;
using FTOptix.S7TiaProfinet;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.SerialPort;
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.EventLogger;
using FTOptix.OPCUAClient;
using FTOptix.MQTTClient;
#endregion

public class SnF_MonthlyReport : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
[ExportMethod]


public void Insert_Row_MonthlyReport() {
        // Get the current project folder.
        var project = Project.Current;


  // Save the names of the columns of the table to an array
        string[] columns = { 
         "Time_Stamp", 
         "Water_Treated", 
         "Resin_Regenerated", 
         "Treatment_Rate", 
         "Contactor_Flow_Rate_Max", 
         "Contactor_Flow_Rate_Avg", 
         "Time_Above_90_Pct_Cap" , 
         "Time_Above_Capacity", 
         "Resin_Bed_Smallest", 
         "Resin_Bed_Average", 
         "Resin_Bed_Largest", 
         "Resin_Added", 
         "Salt_Added", 
         "Salt_Consumed",
         "Service_Water_Consumed", 
         "Power_Consumed", 
         "Waste_Produced", 
         "Paused_Time" };

        // Create and populate a matrix with values to insert into the odbc table
        var rawValues = new object[1, 18];

        // Column TimeStamp

   
        string date = Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Time_Stamp").Value;
        rawValues[0, 0] = DateTime.Parse(date);

        // Column VariableToLog1
        rawValues[0, 1] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Water_Treated/Value").Value;

        // Column VariableToLog2
        rawValues[0, 2] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Resin_Regenerated/Value").Value;

        // Column VariableToLog3
        rawValues[0, 3] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Treatment_Rate/Value").Value;

        // Column VariableToLog4
        rawValues[0, 4] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Contactor_Flow_Rate_Max/Value").Value;

         // Column VariableToLog5
        rawValues[0, 5] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Contactor_Flow_Rate_Avg/Value").Value;

        // Column VariableToLog6
        rawValues[0, 6] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Time_Above_90_Pct_Cap/Value").Value;
        
        // Column VariableToLog7
        rawValues[0, 7] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Time_Above_Capacity/Value").Value;
        
        // Column VariableToLog8
        rawValues[0, 8] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Resin_Bed_Smallest/Value").Value;
        
        // Column VariableToLog9
        rawValues[0, 9] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Resin_Bed_Average/Value").Value;
        
        // Column VariableToLog10
        rawValues[0, 10] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Resin_Bed_Largest/Value").Value;
        
        // Column VariableToLog11
        rawValues[0, 11] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Resin_Added/Value").Value;
        
        // Column VariableToLog12
        rawValues[0, 12] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Salt_Added/Value").Value;
        
        // Column VariableToLog13
        rawValues[0, 13] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Salt_Consumed/Value").Value;
                
        // Column VariableToLog14
        rawValues[0, 14] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Service_Water_Consumed/Value").Value;
                
        // Column VariableToLog15
        rawValues[0, 15] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Power_Consumed/Value").Value;
                
        // Column VariableToLog16
        rawValues[0, 16] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Waste_Produced/Value").Value;

        // Column VariableToLog17
        rawValues[0, 17] = (int)Project.Current.GetVariable("Deployment/AB_MagnaPak/Data/MonthlyReport/Paused_Time/Value").Value;
               


        var myStore = LogicObject.Owner as Store;

        // Get Table1 from myStore
        var table1 = myStore.Tables.Get<Table>("MonthlyReport");

        // Insert values into table1
        table1.Insert(columns, rawValues);



        String Path; 
        string TagAddress;

        string Deployment = "Deployment";       //need to link this to deployment path
        string PLC = "AB_MagnaPak";             //need to link this to deployment path
        string Data = "Data";                   //need to link this to deployment path
        string Regen_Data = "Regen_Data";       //need to link this to deployment path


        
        Path = "Deployment/AB_MagnaPak/Data/RegenData/"; // need to build this string

        TagAddress = Path + "Regen_Runtime/Value";

        Project.Current.GetVariable(TagAddress).Value = 666 ;

    }



}
