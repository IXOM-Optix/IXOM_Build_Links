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
using FTOptix.AuditSigning;
using FTOptix.OPCUAClient;
using FTOptix.MQTTClient;
#endregion

public class SEQ_Replay_Query : BaseNetLogic
{
    private Store myStore;
    private object[,] resultSet;
    DateTime dateQuery;
    string itemRtn;
    public override void Start()
    {
    }

    public override void Stop()
    {
    }

    [ExportMethod]
    public void SEQ_Query()
    {
     string PLC = Owner.Owner.Owner.Owner.BrowseName;    // "AB_MagnaPak";
     string Area = Owner.Owner.Owner.BrowseName;         // "Water_Treatment";
     string Device_Name = Owner.Owner.BrowseName;        // "FT_2000";

     myStore = Project.Current.Get<Store>($"DataStores/" + PLC);
   
     
       // Build SQL Date Time Query String
         dateQuery = DateTime.Parse(Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Date_Time").Value);
         string sqlFormattedDate = dateQuery.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
                
       //Periodic
         //Query Data Base for Step_Time_Elapsed
         string queryString = $"SELECT Step_Time_Elapsed FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);
         //Store Value from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Step_Time_Elapsed").Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Step_Time_Remaining
         queryString = $"SELECT Step_Time_Remaining FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);
         //Store Value from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Step_Time_Remaining").Value = (dynamic)resultSet[0, 0];
       
         //Query Data Base for TEST_1_Value
            queryString = $"SELECT TEST_1_Value FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);
            //Store Value from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_1/Value").Value = (dynamic)resultSet[0, 0];

         //Query Data Base for TEST_2_Value
            queryString = $"SELECT TEST_2_Value FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);
            //Store Value from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_2/Value").Value = (dynamic)resultSet[0, 0];
       
         //Query Data Base for TEST_3_Value
            queryString = $"SELECT TEST_3_Value FROM "+Device_Name+" WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);
            //Store Value from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_3/Value").Value = (dynamic)resultSet[0, 0];


       //Change in Value
         //Query Data Base for STS
         queryString = $"SELECT STS FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/STS").Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Step_Number
         queryString = $"SELECT Step_Number FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Step_Number").Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Step_Time_Elapsed_Units
         queryString = $"SELECT Step_Time_Elapsed_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Step_Time_Elapsed_Units").Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Step_Time_Remaining_Units
         queryString = $"SELECT Step_Time_Remaining_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Step_Time_Remaining_Units").Value = (dynamic)resultSet[0, 0];

         //Query Data Base for Last_Pause_Reason
         queryString = $"SELECT Last_Pause_Reason FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
         myStore.Query(queryString, out _, out resultSet);                          
         //Store STS from Query
         Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Last_Pause_Reason").Value = (dynamic)resultSet[0, 0];

        //Query Data Base for Test_1 //
         //Query Data Base for Test_1_Compare
            queryString = $"SELECT Test_1_Compare FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_1/Compare").Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_1_Setpoint
            queryString = $"SELECT Test_1_Setpoint FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_1/Setpoint").Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_1_Units
            queryString = $"SELECT Test_1_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
             //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_1/Units").Value = (dynamic)resultSet[0, 0];


        //Query Data Base for Test_2 //
            //Query Data Base for Test_2_Compare
            queryString = $"SELECT Test_2_Compare FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_2/Compare").Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_2_Setpoint
            queryString = $"SELECT Test_2_Setpoint FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_2/Setpoint").Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_2_Units
            queryString = $"SELECT Test_2_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_2/Units").Value = (dynamic)resultSet[0, 0];  


        //Query Data Base for Test_3 //
            //Query Data Base for Test_3_Compare
            queryString = $"SELECT Test_3_Compare FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_3/Compare").Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_3_Setpoint
            queryString = $"SELECT Test_3_Setpoint FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_3/Setpoint").Value = (dynamic)resultSet[0, 0];

            //Query Data Base for Test_3_Units
            queryString = $"SELECT Test_3_Units FROM " +Device_Name + "_CIV WHERE LocalTimestamp <= '" + sqlFormattedDate + "'ORDER BY LocalTimestamp DESC LIMIT 2";
            myStore.Query(queryString, out _, out resultSet);                          
            //Store STS from Query
            Project.Current.GetVariable($"Deployment/"+PLC+"/"+Area+"/"+Device_Name+"/SEQ/Rewind/Test_3/Units").Value = (dynamic)resultSet[0, 0];

    }


     
}
