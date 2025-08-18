#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.RAEtherNetIP;
using FTOptix.S7TiaProfinet;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.UI;
using FTOptix.OPCUAClient;
using FTOptix.Core;
using System.Linq.Expressions;
using MQTTnet.Internal;
#endregion

public class BuildLinks : BaseNetLogic
{
    [ExportMethod]
    public void Build()
    {
        // Insert code to be executed by the method
        myLongRunningTask = new LongRunningTask(LTR_BuildLinks, LogicObject);
        myLongRunningTask.Start();
    }

    private void LTR_BuildLinks(LongRunningTask task)
    {

        foreach (IUAObject chld in Owner.Children)   // Scroll through each Deployment object
        {
            if (chld.ObjectType.BrowseName == "BaseObjectType")  // if object and exclude others (scripts)
            {
                CommunicationStation commStation = Project.Current.Get("CommDrivers").Find<CommunicationStation>(chld.BrowseName); //find PLC Deployment name in Comm drivers
                if (commStation != null)
                {
                    Log.Info($"{chld.BrowseName} Station type is {commStation.ObjectType.BrowseName}");
                    switch (commStation.ObjectType.BrowseName)
                    {
                        case "RAEtherNetIPStation":
                            Build_RAEtherNetIPStation(chld, (FTOptix.RAEtherNetIP.Station)commStation);  // goto Allen Bradley PLC section
                            break;
                        case "S7TiaProfinetStation":
                            Build_S7TiaProfinetStation(chld, (FTOptix.S7TiaProfinet.Station)commStation);  //goto Siemens PLC section
                            break;
                    }

                }
                else 
                {
                    Log.Warning($"No Communication Station match for {chld.BrowseName}");
                }
            }
        }
        myLongRunningTask?.Dispose();
    }

    private void Build_RAEtherNetIPStation(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        foreach(IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "PLC_UDT":
                    AB_PLC(chld, Station);
                    break;
                case "BaseObjectType":
                    if (chld.BrowseName == "Config")
                    {
                        AB_Config(chld, Station);
                    }
                    else
                    {
                        foreach (IUAObject item in chld.Children)
                        {
                            switch (item.ObjectType.BrowseName)
                            {
                                case "ALZ_object":
                                    AB_ALZ_Object(item, Station);
                                    break;
                                case "LVL_object":
                                    AB_LVL_Object(item, Station);
                                    break;
                                case "DIG_object":
                                    AB_DIG_Object(item, Station);
                                    break;
                                case "VLV_object":
                                    AB_VLV_Object(item, Station);
                                    break;
                                case "MTR_object":
                                    AB_MTR_Object(item, Station);
                                    break;
                                case "VFD_object":
                                    AB_VFD_Object(item, Station);
                                    break;
                                case "SEQ_object":
                                    AB_SEQ_Object(item, Station);
                                    break;
                                case "PID_object":
                                    AB_PID_Object(item, Station);
                                    break;
                                case "Setpoints_UDT":
                                    AB_Setpoints_Object(item, Station);
                                    break;
                                case "CV_UDT":
                                    AB_CV_Object(item, Station);
                                    break;
                                case "Bools_object":
                                    AB_Bools_Object(item, Station);
                                    break;
                                case "ACC_UDT":
                                    AB_ACC_Object(item, Station);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
        Log.Info($"Rockwell {Obj.BrowseName} {Station.BrowseName}");
    }


    private void Build_S7TiaProfinetStation(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "PLC_UDT":
                    SIE_PLC(chld, Station);
                    break;
                case "BaseObjectType":
                    if (chld.BrowseName == "Config")
                    {
                        SIE_Config(chld, Station);
                    }
                    else
                    {
                        foreach (IUAObject item in chld.Children)
                        {
                            switch (item.ObjectType.BrowseName)
                            {
                                case "ALZ_object":
                                    SIE_ALZ_Object(item, Station);
                                    break;
                                case "LVL_object":
                                    SIE_LVL_Object(item, Station);
                                    break;
                                case "DIG_object":
                                    SIE_DIG_Object(item, Station);
                                    break;
                                case "VLV_object":
                                    SIE_VLV_Object(item, Station);
                                    break;
                                case "MTR_object":
                                    SIE_MTR_Object(item, Station);
                                    break;
                                case "VFD_object":
                                    SIE_VFD_Object(item, Station);
                                    break;
                                case "SEQ_object":
                                    SIE_SEQ_Object(item, Station);
                                    break;
                                case "PID_object":
                                    SIE_PID_Object(item, Station);
                                    break;
                                case "Setpoints_UDT":
                                    SIE_Setpoints_Object(item, Station);
                                    break;
                                case "CV_UDT":
                                    SIE_CV_Object(item, Station);
                                    break;
                                case "Bools_object":
                                    SIE_Bools_Object(item, Station);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        Log.Info($"Siemens {Obj.BrowseName} {Station.BrowseName}");
    }


private void DataLogger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        switch (Obj.BrowseName)
        {
            case "ALZ_PD_Logger":
                PD_Logger(Obj, Station);
                break;
            case "LVL_PD_Logger":
                PD_Logger(Obj, Station);
                break;
            case "VLV_PD_Logger":
                PD_Logger(Obj, Station);
                break;
            case "MTR_PD_Logger":
                PD_Logger(Obj, Station);
                break;
            case "VFD_PD_Logger":
                PD_Logger(Obj, Station);
                break;
            case "SEQ_PD_Logger":
                PD_Logger(Obj, Station);
                break;
            case "PID_PD_Logger":
                PD_Logger(Obj, Station);
                break;


            case "ALZ_CIV_Logger":
                CIV_Logger(Obj, Station);
                break;
            case "LVL_CIV_Logger":
                CIV_Logger(Obj, Station);
                break;
            case "DIG_CIV_Logger":
                CIV_Logger(Obj, Station);
                break;
            case "VLV_CIV_Logger":
                CIV_Logger(Obj, Station);
                break;
            case "MTR_CIV_Logger":
                CIV_Logger(Obj, Station);
                break;
            case "VFD_CIV_Logger":
                CIV_Logger(Obj, Station);
                break;
            case "SEQ_CIV_Logger":
                CIV_Logger(Obj, Station);
                break;


            case "Bool_CIV_Logger":
                Bool_Logger(Obj, Station);
                break;

            default:
                break;
        }
    }
    private void PD_Logger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        //Log.Info($"PD_LOGGER - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");
        Obj.GetVariable("TableName").ResetDynamicLink();
        Obj.GetVariable("TableName").Value = Obj.Owner.BrowseName;

        Obj.GetVariable("Store").ResetDynamicLink();
        Obj.GetVariable("Store").SetDynamicLink(Project.Current.GetVariable("DataStores/" + Obj.Owner.Owner.Owner.BrowseName + "/Filename"), DynamicLinkMode.ReadWrite);
        string tmp1 = Obj.GetVariable("Store").GetVariable("DynamicLink").Value;
    
        Obj.GetVariable("Store").GetVariable("DynamicLink").Value = tmp1.Replace("/Filename", "@NodeId");
    }
     private void CIV_Logger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        //Log.Info($"CIV_LOGGER - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
        Obj.GetVariable("TableName").ResetDynamicLink();
        Obj.GetVariable("TableName").Value = Obj.Owner.BrowseName + "_CIV";

        Obj.GetVariable("Store").ResetDynamicLink();
        Obj.GetVariable("Store").SetDynamicLink(Project.Current.GetVariable("DataStores/" + Obj.Owner.Owner.Owner.BrowseName + "/Filename"), DynamicLinkMode.ReadWrite);
        string tmp2 = Obj.GetVariable("Store").GetVariable("DynamicLink").Value;
        Obj.GetVariable("Store").GetVariable("DynamicLink").Value = tmp2.Replace("/Filename", "@NodeId");              
    }

     private void Bool_Logger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        //Log.Info($"CIV_LOGGER - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
        Obj.GetVariable("TableName").ResetDynamicLink();
        Obj.GetVariable("TableName").Value = "Bools_" + Obj.Owner.BrowseName;

        Obj.GetVariable("Store").ResetDynamicLink();
        Obj.GetVariable("Store").SetDynamicLink(Project.Current.GetVariable("DataStores/" + Obj.Owner.Owner.Owner.BrowseName + "/Filename"), DynamicLinkMode.ReadWrite);
        string tmp2 = Obj.GetVariable("Store").GetVariable("DynamicLink").Value;
        Obj.GetVariable("Store").GetVariable("DynamicLink").Value = tmp2.Replace("/Filename", "@NodeId");              
    }




    /*************************************************************************************************************
    **************************************** Allen Bradley   PLC  ************************************************
    **************************************************************************************************************/
    private void AB_PLC(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.BrowseName;

        Obj.GetVariable("System_Name").ResetDynamicLink();
        Obj.GetVariable("System_Name").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/System_Name"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("System_Location").ResetDynamicLink();
        Obj.GetVariable("System_Location").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/System_Location"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Info/Model").ResetDynamicLink();
        Obj.GetVariable("Info/Model").SetDynamicLink(Station.GetVariable("StationStatusVariables/CatalogNumber"), DynamicLinkMode.ReadWrite);

        //String Formatter
        Obj.GetVariable("Info/Firmware").ResetDynamicLink();
        var stringFormatter1 = InformationModel.Make<StringFormatter>("StringFormatter1");
        stringFormatter1.Format = "{0}.{1}";
        var source0 = InformationModel.MakeVariable("Source0", OpcUa.DataTypes.BaseDataType);
        stringFormatter1.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source0);
        var source1 = InformationModel.MakeVariable("Source1", OpcUa.DataTypes.BaseDataType);
        stringFormatter1.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source1);
        Obj.GetVariable("Info/Firmware").SetConverter(stringFormatter1);
        source0.SetDynamicLink(Station.GetVariable("StationStatusVariables/MajorRev"), DynamicLinkMode.Read);
        source1.SetDynamicLink(Station.GetVariable("StationStatusVariables/MinorRev"), DynamicLinkMode.Read);

        //Expression Evaluator
        Obj.GetVariable("Fault").ResetDynamicLink();
        var expressionEvaluator1 = InformationModel.MakeObject<ExpressionEvaluator>("ExpressionEvaluator");
        expressionEvaluator1.Expression = "{0}&{1}";
        var source2 = InformationModel.MakeVariable("Source0", OpcUa.DataTypes.BaseDataType);
        source2.SetDynamicLink(Obj.GetVariable("Fault/Run_Mode"), DynamicLinkMode.Read);
        var source3 = InformationModel.MakeVariable("Source1", OpcUa.DataTypes.BaseDataType);
        source3.SetDynamicLink(Obj.GetVariable("Fault/Comm_OK"), DynamicLinkMode.Read);
        expressionEvaluator1.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source2);
        expressionEvaluator1.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source3);
        Obj.GetVariable("Fault").SetConverter(expressionEvaluator1);

        Obj.GetVariable("Fault/Run_Mode").ResetDynamicLink();
        Obj.GetVariable("Fault/Run_Mode").SetDynamicLink(Station.GetVariable("StationStatusVariables/RunMode"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Fault/Comm_OK").ResetDynamicLink();
        Obj.GetVariable("Fault/Comm_OK").SetDynamicLink(Station.GetVariable("StationStatusVariables/Present"), DynamicLinkMode.ReadWrite);

        int i = 0;
        foreach (IUAVariable chldVar in Obj.GetVariable("Global").GetNodesByType<IUAVariable>())
        {
            chldVar.ResetDynamicLink();
            chldVar.SetDynamicLink(Station.GetVariable("Tags/Controller Tags/Global"), DynamicLinkMode.ReadWrite);
            chldVar.GetVariable("DynamicLink").Value += $".{i}";
            i++;
        }

        //String Formatter
        Obj.GetVariable("DateTime").ResetDynamicLink();
        var stringFormatter2 = InformationModel.Make<StringFormatter>("StringFormatter1");
        stringFormatter2.Format = "{0}/{1}/{2} {3}:{4}:{5}";
        var source4 = InformationModel.MakeVariable("Source0", OpcUa.DataTypes.BaseDataType);
        stringFormatter2.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source4);
        var source5 = InformationModel.MakeVariable("Source1", OpcUa.DataTypes.BaseDataType);
        stringFormatter2.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source5);
        var source6 = InformationModel.MakeVariable("Source2", OpcUa.DataTypes.BaseDataType);
        stringFormatter2.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source6);
        var stringFormatter3 = InformationModel.Make<StringFormatter>("StringFormatter2");
        stringFormatter3.Format = "{0:d}";
        var source10 = InformationModel.MakeVariable("Source0", OpcUa.DataTypes.BaseDataType);
        stringFormatter3.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source10);
        source6.SetConverter(stringFormatter3);
        source10.SetDynamicLink(Obj.GetVariable("DateTime/YYYY"), DynamicLinkMode.Read);
        var source7 = InformationModel.MakeVariable("Source3", OpcUa.DataTypes.BaseDataType);
        stringFormatter2.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source7);
        var source8 = InformationModel.MakeVariable("Source4", OpcUa.DataTypes.BaseDataType);
        stringFormatter2.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source8);
        var source9 = InformationModel.MakeVariable("Source5", OpcUa.DataTypes.BaseDataType);
        stringFormatter2.Refs.AddReference(FTOptix.CoreBase.ReferenceTypes.HasSource, source9);
        Obj.GetVariable("DateTime").SetConverter(stringFormatter2);
        source4.SetDynamicLink(Obj.GetVariable("DateTime/MM"), DynamicLinkMode.Read);
        source5.SetDynamicLink(Obj.GetVariable("DateTime/DD"), DynamicLinkMode.Read);
        source7.SetDynamicLink(Obj.GetVariable("DateTime/HH"), DynamicLinkMode.Read);
        source8.SetDynamicLink(Obj.GetVariable("DateTime/mm"), DynamicLinkMode.Read);
        source9.SetDynamicLink(Obj.GetVariable("DateTime/ss"), DynamicLinkMode.Read);

        i = 0;
        foreach (IUAVariable chldVar in Obj.GetVariable("DateTime").GetNodesByType<IUAVariable>())
        {
            chldVar.ResetDynamicLink();
            chldVar.SetDynamicLink(Station.GetVariable("Tags/Controller Tags/PLC/DateTime"), DynamicLinkMode.ReadWrite);
            chldVar.GetVariable("DynamicLink").Value += $"[{i}]";
            i++;
        }

        i = 0;
        foreach (IUAVariable chldVar in Obj.GetVariable("SetDateTime").GetNodesByType<IUAVariable>())
        {
            if (chldVar.BrowseName == "Set")
            {
                chldVar.ResetDynamicLink();
                chldVar.SetDynamicLink(Station.GetVariable("Tags/Controller Tags/PLC/DateTime_Sync"), DynamicLinkMode.ReadWrite);
            }
            else
            {
                chldVar.ResetDynamicLink();
                chldVar.SetDynamicLink(Station.GetVariable("Tags/Controller Tags/PLC/DateTime_Set"), DynamicLinkMode.ReadWrite);
                chldVar.GetVariable("DynamicLink").Value += $"[{i}]";
            }
            i++;
        }

    }

    /*************************************************************************************************************
    ****************************************        AB Config     ************************************************
    **************************************************************************************************************/
    private void AB_Config(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        Obj.GetVariable("Unit_Count").ResetDynamicLink();
        Obj.GetVariable("Unit_Count").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/Config/Unit_Count"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Duty_Mode").ResetDynamicLink();
        Obj.GetVariable("Duty_Mode").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/Config/Duty_Mode"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Simulate").ResetDynamicLink();
        Obj.GetVariable("Simulate").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/Config/Simulate"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Interlock_System_Request").ResetDynamicLink();
        Obj.GetVariable("Interlock_System_Request").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/Config/Interlock_System_Request"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Interlock_Raw_Water_Feed").ResetDynamicLink();
        Obj.GetVariable("Interlock_Raw_Water_Feed").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/Config/Interlock_Raw_Water_Feed"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Interlock_Regen_Permit").ResetDynamicLink();
        Obj.GetVariable("Interlock_Regen_Permit").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/Config/Interlock_Regen_Permit"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Interlock_Well_Pumps_Running").ResetDynamicLink();
        Obj.GetVariable("Interlock_Well_Pumps_Running").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/Config/Interlock_Well_Pumps_Running"), DynamicLinkMode.ReadWrite);
    }




    /*************************************************************************************************************
    ****************************************      AB Analyzer     ************************************************
    **************************************************************************************************************/



    private void AB_ALZ_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "ALZ_UDT":
                    AB_ALZ_UDT(chld, Station);
                    break;
                case "OffNormalAlarmController":
                    AB_ALZ_Alarm(chld, Station);
                    break;
                case "DataLogger":
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }

    private void AB_ALZ_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("STS").ResetDynamicLink();
        Obj.GetVariable("STS").SetDynamicLink(deviceTag.GetVariable("STS"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD").ResetDynamicLink();
        int i = 0;
        foreach (IUAVariable chldVar in Obj.GetVariable("CMD").GetNodesByType<IUAVariable>())
        {
            chldVar.ResetDynamicLink();
            chldVar.SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);
            chldVar.GetVariable("DynamicLink").Value += $".{i}";
            i++;
        }
        Obj.GetVariable("CMD").SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("DELAY_LL").ResetDynamicLink();
        Obj.GetVariable("DELAY_LL").SetDynamicLink(deviceTag.GetVariable("DELAY_LL"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("DELAY_L").ResetDynamicLink();
        Obj.GetVariable("DELAY_L").SetDynamicLink(deviceTag.GetVariable("DELAY_L"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("DELAY_H").ResetDynamicLink();
        Obj.GetVariable("DELAY_H").SetDynamicLink(deviceTag.GetVariable("DELAY_H"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("DELAY_HH").ResetDynamicLink();
        Obj.GetVariable("DELAY_HH").SetDynamicLink(deviceTag.GetVariable("DELAY_HH"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SP_LL").ResetDynamicLink();
        Obj.GetVariable("SP_LL").SetDynamicLink(deviceTag.GetVariable("SP_LL"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SP_L").ResetDynamicLink();
        Obj.GetVariable("SP_L").SetDynamicLink(deviceTag.GetVariable("SP_L"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SP_H").ResetDynamicLink();
        Obj.GetVariable("SP_H").SetDynamicLink(deviceTag.GetVariable("SP_H"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SP_HH").ResetDynamicLink();
        Obj.GetVariable("SP_HH").SetDynamicLink(deviceTag.GetVariable("SP_HH"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("EU_Min").ResetDynamicLink();
        Obj.GetVariable("EU_Min").SetDynamicLink(deviceTag.GetVariable("EU_Min"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("EU_Max").ResetDynamicLink();
        Obj.GetVariable("EU_Max").SetDynamicLink(deviceTag.GetVariable("EU_Max"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Input_Filter_Rate").ResetDynamicLink();
        Obj.GetVariable("Input_Filter_Rate").SetDynamicLink(deviceTag.GetVariable("Input_Filter_Rate"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Value").ResetDynamicLink();
        Obj.GetVariable("Value").SetDynamicLink(deviceTag.GetVariable("Value"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Units").ResetDynamicLink();
        Obj.GetVariable("Units").SetDynamicLink(deviceTag.GetVariable("Units"), DynamicLinkMode.ReadWrite);        
     }

     private void AB_ALZ_Alarm(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        string almDesc = Obj.BrowseName;
        string area = Obj.Owner.Owner.BrowseName;
        string device_name = Obj.Owner.BrowseName;
        string device_description = Obj.Owner.GetVariable("ALZ/Device_Description").Value;

        switch (Obj.BrowseName)
        {
            case "Comm_Fail":
                almDesc = "Comm Fail";
                break;
            case "Out_of_Range":
                almDesc = "Out of Range";
                break;
            case "Low_Alarm":
                almDesc = "Low Alarm";
                break;
            case "Low_Warning":
                almDesc = "Low Warning";
                break;
            case "High_Warning":
                almDesc = "High Warning";
                break;
            case "High_Alarm":
                almDesc = "High Alarm";
                break;
            default:
                break;
        }
        Obj.GetVariable("Message").ResetDynamicLink();
        Obj.GetVariable("Message").Value = $"{device_name} {almDesc} - {area} {device_description}";
    }

    




    
    



    /*************************************************************************************************************
    ****************************************      AB Level        ************************************************
    **************************************************************************************************************/
    private void AB_LVL_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
{
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "LVL_UDT":
                    AB_LVL_UDT(chld, Station);
                    break;
                case "OffNormalAlarmController":
                    AB_LVL_Alarm(chld, Station);
                    break;
                case "DataLogger":
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }

    private void AB_LVL_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {

        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("STS").ResetDynamicLink();
        Obj.GetVariable("STS").SetDynamicLink(deviceTag.GetVariable("STS"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD").ResetDynamicLink();
        int i = 0;
        foreach (IUAVariable chldVar in Obj.GetVariable("CMD").GetNodesByType<IUAVariable>())
        {
            chldVar.ResetDynamicLink();
            chldVar.SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);
            chldVar.GetVariable("DynamicLink").Value += $".{i}";
            i++;
        }
        Obj.GetVariable("CMD").SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("DELAY_LL").ResetDynamicLink();
        Obj.GetVariable("DELAY_LL").SetDynamicLink(deviceTag.GetVariable("DELAY_LL"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("DELAY_L").ResetDynamicLink();
        Obj.GetVariable("DELAY_L").SetDynamicLink(deviceTag.GetVariable("DELAY_L"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("DELAY_H").ResetDynamicLink();
        Obj.GetVariable("DELAY_H").SetDynamicLink(deviceTag.GetVariable("DELAY_H"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("DELAY_HH").ResetDynamicLink();
        Obj.GetVariable("DELAY_HH").SetDynamicLink(deviceTag.GetVariable("DELAY_HH"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SP_LL").ResetDynamicLink();
        Obj.GetVariable("SP_LL").SetDynamicLink(deviceTag.GetVariable("SP_LL"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SP_L").ResetDynamicLink();
        Obj.GetVariable("SP_L").SetDynamicLink(deviceTag.GetVariable("SP_L"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SP_H").ResetDynamicLink();
        Obj.GetVariable("SP_H").SetDynamicLink(deviceTag.GetVariable("SP_H"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SP_HH").ResetDynamicLink();
        Obj.GetVariable("SP_HH").SetDynamicLink(deviceTag.GetVariable("SP_HH"), DynamicLinkMode.ReadWrite);


        Obj.GetVariable("Input_Filter_Rate").ResetDynamicLink();
        Obj.GetVariable("Input_Filter_Rate").SetDynamicLink(deviceTag.GetVariable("Input_Filter_Rate"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("ROC_Sample_Interval").ResetDynamicLink();
        Obj.GetVariable("ROC_Sample_Interval").SetDynamicLink(deviceTag.GetVariable("ROC_Sample_Interval"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Distance_at_100").ResetDynamicLink();
        Obj.GetVariable("Distance_at_100").SetDynamicLink(deviceTag.GetVariable("Distance_at_100"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Volume_at_100").ResetDynamicLink();
        Obj.GetVariable("Volume_at_100").SetDynamicLink(deviceTag.GetVariable("Volume_at_100"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Rate_of_Change").ResetDynamicLink();
        Obj.GetVariable("Rate_of_Change").SetDynamicLink(deviceTag.GetVariable("Rate_of_Change"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Volume").ResetDynamicLink();
        Obj.GetVariable("Volume").SetDynamicLink(deviceTag.GetVariable("Volume"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Distance").ResetDynamicLink();
        Obj.GetVariable("Distance").SetDynamicLink(deviceTag.GetVariable("Distance"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Percent").ResetDynamicLink();
        Obj.GetVariable("Percent").SetDynamicLink(deviceTag.GetVariable("Percent"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Percent_of_Range_at_100").ResetDynamicLink();
        Obj.GetVariable("Percent_of_Range_at_100").SetDynamicLink(deviceTag.GetVariable("Percent_of_Range_at_100"), DynamicLinkMode.ReadWrite);
    }


    private void AB_LVL_Alarm(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        string almDesc = Obj.BrowseName;
        string area = Obj.Owner.Owner.BrowseName;
        string device_name = Obj.Owner.BrowseName;
        string device_description = Obj.Owner.GetVariable("LVL/Device_Description").Value;

        switch (Obj.BrowseName)
            {
                case "Comm_Fail":
                    almDesc = "Comm Fail";
                    break;
                case "Out_of_Range":
                    almDesc = "Out of Range";
                    break;
                case "Low_Alarm":
                    almDesc = "Low Level Alarm";
                    break;
                case "Low_Warning":
                    almDesc = "Low Level Warning";
                    break;
                case "High_Warning":
                    almDesc = "High Level Warning";
                    break;
                case "High_Alarm":
                    almDesc = "High Level Alarm";
                    break;
                default:
                    break;
            }
        Obj.GetVariable("Message").ResetDynamicLink();
        Obj.GetVariable("Message").Value = $"{device_name} {almDesc} - {area} {device_description}";
    }


    /*************************************************************************************************************
    ***************************************      AB Digital       ************************************************
    **************************************************************************************************************/
    private void AB_DIG_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "DIG_UDT":
                    AB_DIG_UDT(chld, Station);
                    break;
                case "OffNormalAlarmController":
                    AB_DIG_Alarm(chld, Station);
                    break;
                case "DataLogger":
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }

    private void AB_DIG_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("STS").ResetDynamicLink();
        Obj.GetVariable("STS").SetDynamicLink(deviceTag.GetVariable("STS"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD").ResetDynamicLink();
            int i = 0;
            foreach (IUAVariable chldVar in Obj.GetVariable("CMD").GetNodesByType<IUAVariable>())
            {
                chldVar.ResetDynamicLink();
                chldVar.SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);
                chldVar.GetVariable("DynamicLink").Value += $".{i}";
                i++;
            }
        Obj.GetVariable("CMD").SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Input_Delay").ResetDynamicLink();
        Obj.GetVariable("Input_Delay").SetDynamicLink(deviceTag.GetVariable("Input_Delay"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Input_Hold_Delay").ResetDynamicLink();
        Obj.GetVariable("Input_Hold_Delay").SetDynamicLink(deviceTag.GetVariable("Input_Hold_Delay"), DynamicLinkMode.ReadWrite);
     }

    
    private void AB_DIG_Alarm(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        string area = Obj.Owner.Owner.BrowseName;
        string device_name = Obj.Owner.BrowseName;
        string device_description = Obj.Owner.GetVariable("DIG/Device_Description").Value;

        Obj.GetVariable("Message").ResetDynamicLink();
        Obj.GetVariable("Message").Value = $"Alarm {device_name} {area} {device_description}";
    }


    /*************************************************************************************************************
    ****************************************      AB Valve        ************************************************
    **************************************************************************************************************/
    private void AB_VLV_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "VLV_UDT":
                    AB_VLV_UDT(chld, Station);
                    break;
                case "OffNormalAlarmController":
                    AB_VLV_Alarm(chld, Station);
                    break;
                case "DataLogger":
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }


    private void AB_VLV_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("STS").ResetDynamicLink();
        Obj.GetVariable("STS").SetDynamicLink(deviceTag.GetVariable("STS"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD").ResetDynamicLink();
        int i = 0;
        foreach (IUAVariable chldVar in Obj.GetVariable("CMD").GetNodesByType<IUAVariable>())
        {
            chldVar.ResetDynamicLink();
            chldVar.SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);
            chldVar.GetVariable("DynamicLink").Value += $".{i}";
            i++;
        }
        Obj.GetVariable("CMD").SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Position_Actual").ResetDynamicLink();
        Obj.GetVariable("Position_Actual").SetDynamicLink(deviceTag.GetVariable("Position_Actual"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Position_Manual").ResetDynamicLink();
        Obj.GetVariable("Position_Manual").SetDynamicLink(deviceTag.GetVariable("Position_Manual"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Pre_Fail_Open").ResetDynamicLink();
        Obj.GetVariable("Pre_Fail_Open").SetDynamicLink(deviceTag.GetVariable("Pre_Fail_Open"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Pre_Fail_Close").ResetDynamicLink();
        Obj.GetVariable("Pre_Fail_Close").SetDynamicLink(deviceTag.GetVariable("Pre_Fail_Close"), DynamicLinkMode.ReadWrite);
        
        Obj.GetVariable("Position_Auto").ResetDynamicLink();
        Obj.GetVariable("Position_Auto").SetDynamicLink(deviceTag.GetVariable("Position_Auto"), DynamicLinkMode.ReadWrite);

    }
    


    private void AB_VLV_Alarm(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        string almDesc = Obj.BrowseName;
        string area = Obj.Owner.Owner.BrowseName;
        string device_name = Obj.Owner.BrowseName;
        string device_description = Obj.Owner.GetVariable("VLV/Device_Description").Value;

        switch (Obj.BrowseName)
            {
                case "Comm_Fault":
                    almDesc = "Comm Fail";
                    break;
                case "LSS_Alarm":
                    almDesc = "Limit Switch Short";
                    break;
                case "FTO_Alarm":
                    almDesc = "Fail to Open";
                    break;
                case "FTC_Alarm":
                    almDesc = "Fail to Close";
                    break;
                default:
                    break;
            }
        Obj.GetVariable("Message").ResetDynamicLink();
        Obj.GetVariable("Message").Value = $"{device_name} {almDesc} - {area} {device_description}";
    }


    /*************************************************************************************************************
    ****************************************      AB Motor        ************************************************
    **************************************************************************************************************/
    private void AB_MTR_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "MTR_UDT":
                    AB_MTR_UDT(chld, Station);
                    break;
                case "OffNormalAlarmController":
                    AB_MTR_Alarm(chld, Station);
                    break;
                case "DataLogger":
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }


    private void AB_MTR_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("STS").ResetDynamicLink();
        Obj.GetVariable("STS").SetDynamicLink(deviceTag.GetVariable("STS"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD").ResetDynamicLink();
        int i = 0;
        foreach (IUAVariable chldVar in Obj.GetVariable("CMD").GetNodesByType<IUAVariable>())
        {
            chldVar.ResetDynamicLink();
            chldVar.SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);
            chldVar.GetVariable("DynamicLink").Value += $".{i}";
            i++;
        }
        Obj.GetVariable("CMD").SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Speed_Actual").ResetDynamicLink();
        Obj.GetVariable("Speed_Actual").SetDynamicLink(deviceTag.GetVariable("Speed_Actual"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Speed_Manual").ResetDynamicLink();
        Obj.GetVariable("Speed_Manual").SetDynamicLink(deviceTag.GetVariable("Speed_Manual"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Pre_Fail_Start").ResetDynamicLink();
        Obj.GetVariable("Pre_Fail_Start").SetDynamicLink(deviceTag.GetVariable("Pre_Fail_Start"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Pre_Fail_Stop").ResetDynamicLink();
        Obj.GetVariable("Pre_Fail_Stop").SetDynamicLink(deviceTag.GetVariable("Pre_Fail_Stop"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Runtime").ResetDynamicLink();
        Obj.GetVariable("Runtime").SetDynamicLink(deviceTag.GetVariable("Runtime"), DynamicLinkMode.ReadWrite);
        
        Obj.GetVariable("Speed_Auto").ResetDynamicLink();
        Obj.GetVariable("Speed_Auto").SetDynamicLink(deviceTag.GetVariable("Speed_Auto"), DynamicLinkMode.ReadWrite);

    }
    

    private void AB_MTR_CIV_Logger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {

    }
    private void AB_MTR_Alarm(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        string almDesc = Obj.BrowseName;
        string area = Obj.Owner.Owner.BrowseName;
        string device_name = Obj.Owner.BrowseName;
        string device_description = Obj.Owner.GetVariable("MTR/Device_Description").Value;

        switch (Obj.BrowseName)
            {
                case "Comm_Fault":
                    almDesc = "Comm Fail";
                    break;
                case "External_Fault_Alarm":
                    almDesc = "External Fault";
                    break;
                case "FT_Start_Alarm":
                    almDesc = "Fail to Start";
                    break;
                case "FT_Stop_Alarm":
                    almDesc = "Fail to Stop";
                    break;
                default:
                    break;
            }
        Obj.GetVariable("Message").ResetDynamicLink();
        Obj.GetVariable("Message").Value = $"{device_name} {almDesc} - {area} {device_description}";
    }

    /*************************************************************************************************************
    ****************************************      AB VFD          ************************************************
    **************************************************************************************************************/
    private void AB_VFD_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "VFD_UDT":
                    AB_VFD_UDT(chld, Station);
                    break;
                case "OffNormalAlarmController":
                    AB_VFD_Alarm(chld, Station);
                    break;
                case "DataLogger":
                    //AB_VFD_CIV_Logger(chld, Station);
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }


    private void AB_VFD_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("STS").ResetDynamicLink();
        Obj.GetVariable("STS").SetDynamicLink(deviceTag.GetVariable("STS"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD").ResetDynamicLink();
        int i = 0;
        foreach (IUAVariable chldVar in Obj.GetVariable("CMD").GetNodesByType<IUAVariable>())
        {
            chldVar.ResetDynamicLink();
            chldVar.SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);
            chldVar.GetVariable("DynamicLink").Value += $".{i}";
            i++;
        }
        Obj.GetVariable("CMD").SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Speed_Actual").ResetDynamicLink();
        Obj.GetVariable("Speed_Actual").SetDynamicLink(deviceTag.GetVariable("Speed_Actual"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Speed_Manual").ResetDynamicLink();
        Obj.GetVariable("Speed_Manual").SetDynamicLink(deviceTag.GetVariable("Speed_Manual"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Pre_Fail_Start").ResetDynamicLink();
        Obj.GetVariable("Pre_Fail_Start").SetDynamicLink(deviceTag.GetVariable("Pre_Fail_Start"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Pre_Fail_Stop").ResetDynamicLink();
        Obj.GetVariable("Pre_Fail_Stop").SetDynamicLink(deviceTag.GetVariable("Pre_Fail_Stop"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Runtime").ResetDynamicLink();
        Obj.GetVariable("Runtime").SetDynamicLink(deviceTag.GetVariable("Runtime"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Current").ResetDynamicLink();
        Obj.GetVariable("Current").SetDynamicLink(deviceTag.GetVariable("Current"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Current_Delay").ResetDynamicLink();
        Obj.GetVariable("Current_Delay").SetDynamicLink(deviceTag.GetVariable("Current_Delay"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Current_High_SP").ResetDynamicLink();
        Obj.GetVariable("Current_High_SP").SetDynamicLink(deviceTag.GetVariable("Current_High_SP"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Current_Low_SP").ResetDynamicLink();
        Obj.GetVariable("Current_Low_SP").SetDynamicLink(deviceTag.GetVariable("Current_Low_SP"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Pre_Accel").ResetDynamicLink();
        Obj.GetVariable("Pre_Accel").SetDynamicLink(deviceTag.GetVariable("Pre_Accel"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Pre_Decel").ResetDynamicLink();
        Obj.GetVariable("Pre_Decel").SetDynamicLink(deviceTag.GetVariable("Pre_Decel"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Fault_Code").ResetDynamicLink();
        Obj.GetVariable("Fault_Code").SetDynamicLink(deviceTag.GetVariable("Fault_Code"), DynamicLinkMode.ReadWrite);
        
        

    }
    

    private void AB_VFD_CIV_Logger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {

    }
    private void AB_VFD_Alarm(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        string almDesc = Obj.BrowseName;
        string area = Obj.Owner.Owner.BrowseName;
        string device_name = Obj.Owner.BrowseName;
        string device_description = Obj.Owner.GetVariable("VFD/Device_Description").Value;

        switch (Obj.BrowseName)
        {
            case "Comm_Fault":
                almDesc = "Comm Fail";
                break;
            case "External_Fault":
                almDesc = "External Fault";
                break;
            case "FT_Start":
                almDesc = "Fail to Start";
                break;
            case "FT_Stop":
                almDesc = "Fail to Stop";
                break;
            case "Low_Current":
                almDesc = "Low Current";
                break;
            case "High_Current":
                almDesc = "Low Current";
                break;
            case "Drive_Fault":
                almDesc = "Drive Fault";
                break;
            default:
                break;
        }
        //Obj.GetVariable("Message").ResetDynamicLink();
        //Obj.GetVariable("Message").Value = $"{device_name} {almDesc} - {area} {device_description}";
    }

    /*************************************************************************************************************
    ****************************************    AB Sequencer      ************************************************
    **************************************************************************************************************/
    private void AB_SEQ_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "SEQ_UDT":
                    AB_SEQ_UDT(chld, Station);
                    break;
                case "OffNormalAlarmController":
                    AB_SEQ_Alarm(chld, Station);
                    break;
                case "DataLogger":
                    //AB_SEQ_CIV_Logger(chld, Station);
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }


    private void AB_SEQ_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("CMD").ResetDynamicLink();

        Obj.GetVariable("CMD/Abort").ResetDynamicLink();
        Obj.GetVariable("CMD/Abort").SetDynamicLink(deviceTag.GetVariable("CMD/Abort"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD/Auto_Advance").ResetDynamicLink();
        Obj.GetVariable("CMD/Auto_Advance").SetDynamicLink(deviceTag.GetVariable("CMD/Auto_Advance"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD/Auto_Resume").ResetDynamicLink();
        Obj.GetVariable("CMD/Auto_Resume").SetDynamicLink(deviceTag.GetVariable("CMD/Auto_Resume"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD/Auto_Start").ResetDynamicLink();
        Obj.GetVariable("CMD/Auto_Start").SetDynamicLink(deviceTag.GetVariable("CMD/Auto_Start"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD/Jog_Back").ResetDynamicLink();
        Obj.GetVariable("CMD/Jog_Back").SetDynamicLink(deviceTag.GetVariable("CMD/Jog_Back"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD/Jog_Forward").ResetDynamicLink();
        Obj.GetVariable("CMD/Jog_Forward").SetDynamicLink(deviceTag.GetVariable("CMD/Jog_Forward"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD/Pause").ResetDynamicLink();
        Obj.GetVariable("CMD/Pause").SetDynamicLink(deviceTag.GetVariable("CMD/Pause"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD/Recover").ResetDynamicLink();
        Obj.GetVariable("CMD/Recover").SetDynamicLink(deviceTag.GetVariable("CMD/Recover"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD/Resume").ResetDynamicLink();
        Obj.GetVariable("CMD/Resume").SetDynamicLink(deviceTag.GetVariable("CMD/Resume"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Step_Number").ResetDynamicLink();
        Obj.GetVariable("Step_Number").SetDynamicLink(deviceTag.GetVariable("Step/Number"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Step_Time_Elapsed").ResetDynamicLink();
        Obj.GetVariable("Step_Time_Elapsed").SetDynamicLink(deviceTag.GetVariable("Time/Elapsed"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Step_Time_Elapsed_Units").ResetDynamicLink();
        Obj.GetVariable("Step_Time_Elapsed_Units").SetDynamicLink(deviceTag.GetVariable("Time/Elapsed_Units"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Step_Time_Remaining").ResetDynamicLink();
        Obj.GetVariable("Step_Time_Remaining").SetDynamicLink(deviceTag.GetVariable("Time/Remaining"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Step_Time_Remaining_Units").ResetDynamicLink();
        Obj.GetVariable("Step_Time_Remaining_Units").SetDynamicLink(deviceTag.GetVariable("Time/Remaining_Units"), DynamicLinkMode.ReadWrite);


        Obj.GetVariable("Test_1").ResetDynamicLink();
        Obj.GetVariable("Test_1").SetDynamicLink(deviceTag.GetVariable("Test/0/Done"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_1/Value").ResetDynamicLink();
        Obj.GetVariable("Test_1/Value").SetDynamicLink(deviceTag.GetVariable("Test/0/Value"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_1/Compare").ResetDynamicLink();
        Obj.GetVariable("Test_1/Compare").SetDynamicLink(deviceTag.GetVariable("Test/0/Compare"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_1/Setpoint").ResetDynamicLink();
        Obj.GetVariable("Test_1/Setpoint").SetDynamicLink(deviceTag.GetVariable("Test/0/Setpoint"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_1/Units").ResetDynamicLink();
        Obj.GetVariable("Test_1/Units").SetDynamicLink(deviceTag.GetVariable("Test/0/Units"), DynamicLinkMode.ReadWrite);


        Obj.GetVariable("Test_2").ResetDynamicLink();
        Obj.GetVariable("Test_2").SetDynamicLink(deviceTag.GetVariable("Test/1/Done"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_2/Value").ResetDynamicLink();
        Obj.GetVariable("Test_2/Value").SetDynamicLink(deviceTag.GetVariable("Test/1/Value"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_2/Compare").ResetDynamicLink();
        Obj.GetVariable("Test_2/Compare").SetDynamicLink(deviceTag.GetVariable("Test/1/Compare"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_2/Setpoint").ResetDynamicLink();
        Obj.GetVariable("Test_2/Setpoint").SetDynamicLink(deviceTag.GetVariable("Test/1/Setpoint"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_2/Units").ResetDynamicLink();
        Obj.GetVariable("Test_2/Units").SetDynamicLink(deviceTag.GetVariable("Test/1/Units"), DynamicLinkMode.ReadWrite);


        Obj.GetVariable("Test_3").ResetDynamicLink();
        Obj.GetVariable("Test_3").SetDynamicLink(deviceTag.GetVariable("Test/2/Done"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_3/Value").ResetDynamicLink();
        Obj.GetVariable("Test_3/Value").SetDynamicLink(deviceTag.GetVariable("Test/2/Value"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_3/Compare").ResetDynamicLink();
        Obj.GetVariable("Test_3/Compare").SetDynamicLink(deviceTag.GetVariable("Test/2/Compare"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_3/Setpoint").ResetDynamicLink();
        Obj.GetVariable("Test_3/Setpoint").SetDynamicLink(deviceTag.GetVariable("Test/2/Setpoint"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Test_3/Units").ResetDynamicLink();
        Obj.GetVariable("Test_3/Units").SetDynamicLink(deviceTag.GetVariable("Test/2/Units"), DynamicLinkMode.ReadWrite);


        Obj.GetVariable("Last_Pause_Reason").ResetDynamicLink();
        Obj.GetVariable("Last_Pause_Reason").SetDynamicLink(deviceTag.GetVariable("Last_Pause_Reason"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Alarm_Word").ResetDynamicLink();
        Obj.GetVariable("Alarm_Word").SetDynamicLink(deviceTag.GetVariable("Alarm_Word"), DynamicLinkMode.ReadWrite);
    }
    

    private void AB_SEQ_CIV_Logger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {

    }
    private void AB_SEQ_Alarm(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {

        string almDesc = Obj.BrowseName;
        string area = Obj.Owner.Owner.BrowseName;
        string device_name = Obj.Owner.BrowseName;
        string device_description = Obj.Owner.GetVariable("SEQ/Device_Description").Value;


        switch (Obj.BrowseName)
        {
            case "SEQ_Paused":
                almDesc = "Sequencer Paused";
                break;
            case "SEQ_No_Auto_Start":
                almDesc = "Sequencer in Manual Start Mode";
                break;
            case "SEQ_No_Auto_Advance":
                almDesc = "Sequencer in Manual Advance Mode";
                break;
            case "SEQ_Recovery_Active":
                almDesc = "Recovery Sequence Active";
                break;
            default:
                break;
        }
        Obj.GetVariable("Message").ResetDynamicLink();
        Obj.GetVariable("Message").Value = $"{device_name} {almDesc}";

        
    }



    /*************************************************************************************************************
    ****************************************        AB PID        ************************************************
    **************************************************************************************************************/
    private void AB_PID_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "PID_UDT":
                    AB_PID_UDT(chld, Station);
                    break;
                case "DataLogger":
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }


    private void AB_PID_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("P").ResetDynamicLink();
        Obj.GetVariable("P").SetDynamicLink(deviceTag.GetVariable("P"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("I").ResetDynamicLink();
        Obj.GetVariable("I").SetDynamicLink(deviceTag.GetVariable("I"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("D").ResetDynamicLink();
        Obj.GetVariable("D").SetDynamicLink(deviceTag.GetVariable("D"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Update_Rate").ResetDynamicLink();
        Obj.GetVariable("Update_Rate").SetDynamicLink(deviceTag.GetVariable("Update_Rate"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Deadband").ResetDynamicLink();
        Obj.GetVariable("Deadband").SetDynamicLink(deviceTag.GetVariable("Deadband"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("CMD").ResetDynamicLink();
        Obj.GetVariable("CMD").SetDynamicLink(deviceTag.GetVariable("CMD"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Mode").ResetDynamicLink();
        Obj.GetVariable("Mode").SetDynamicLink(deviceTag.GetVariable("Mode"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Control_Setpoint").ResetDynamicLink();
        Obj.GetVariable("Control_Setpoint").SetDynamicLink(deviceTag.GetVariable("Control/Setpoint"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Control_PV").ResetDynamicLink();
        Obj.GetVariable("Control_PV").SetDynamicLink(deviceTag.GetVariable("Control/PV"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Control_Out").ResetDynamicLink();
        Obj.GetVariable("Control_Out").SetDynamicLink(deviceTag.GetVariable("Control/Out"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Input_Min").ResetDynamicLink();
        Obj.GetVariable("Input_Min").SetDynamicLink(deviceTag.GetVariable("Input_Min"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Input_Max").ResetDynamicLink();
        Obj.GetVariable("Input_Max").SetDynamicLink(deviceTag.GetVariable("Input_Max"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Input_Units").ResetDynamicLink();
        Obj.GetVariable("Input_Units").SetDynamicLink(deviceTag.GetVariable("Input_Units"), DynamicLinkMode.ReadWrite);



        Obj.GetVariable("Output_Min").ResetDynamicLink();
        Obj.GetVariable("Output_Min").SetDynamicLink(deviceTag.GetVariable("Output_Min"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Output_Max").ResetDynamicLink();
        Obj.GetVariable("Output_Max").SetDynamicLink(deviceTag.GetVariable("Output_Max"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Output_Units").ResetDynamicLink();
        Obj.GetVariable("Output_Units").SetDynamicLink(deviceTag.GetVariable("Output_Units"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Output_Start").ResetDynamicLink();
        Obj.GetVariable("Output_Start").SetDynamicLink(deviceTag.GetVariable("Output_Start"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Output_Start_Delay").ResetDynamicLink();
        Obj.GetVariable("Output_Start_Delay").SetDynamicLink(deviceTag.GetVariable("Output_Start_Delay"), DynamicLinkMode.ReadWrite);
    }
    

    private void AB_PID_CIV_Logger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {

    }


    /*************************************************************************************************************
    ****************************************     AB Setpoints     ************************************************
    **************************************************************************************************************/
    private void AB_Setpoints_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("Max").ResetDynamicLink();
        Obj.GetVariable("Max").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Max"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Min").ResetDynamicLink();
        Obj.GetVariable("Min").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Min"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("SavedValue").ResetDynamicLink();
        Obj.GetVariable("SavedValue").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/SavedValue"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Value").ResetDynamicLink();
        Obj.GetVariable("Value").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Value"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Units").ResetDynamicLink();
        Obj.GetVariable("Units").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Units"), DynamicLinkMode.ReadWrite);
    }


    
    /*************************************************************************************************************
    **************************************** AB Calculated Values ************************************************
    **************************************************************************************************************/
    private void AB_CV_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.BrowseName;



        Obj.GetVariable("Value").ResetDynamicLink();
        var myVar = Obj.GetVariable("Value");                                // Get the variable
        IUAVariable tempVariable2 = null;                                    // Create a fake variable            
        myVar.SetDynamicLink(tempVariable2, DynamicLinkMode.ReadWrite);      // Add a dynamic link to the fake variable (to trigger the link mechanism)
        string string1 = "../../../../../CommDrivers/RAEtherNet_IPDriver1/AB_MagnaPak";  // @Erik  how to make this dynamic??
        string string2 = "/Tags/Controller Tags/Calc_Value";
        string string3 = "[" + Obj.BrowseName +"]";
        string strings = string1 + string2 + string3;                       // ../../../../../CommDrivers/RAEtherNet_IPDriver1/AB_MagnaPak/Tags/Controller Tags/Calc_Value[0]                                                                      
        myVar.GetVariable("DynamicLink").Value = strings;                   // Replace the dynamic link content to the real target value   
    }



    /*************************************************************************************************************
    ****************************************       AB Bools       ************************************************
    **************************************************************************************************************/
    private void AB_Bools_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
        {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        foreach (IUAObject chld in Obj.Children)
        {
            switch (chld.ObjectType.BrowseName)
            {
                case "Bools_UDT":
                    AB_Bools_UDT(chld, Station);
                    break;
                case "OffNormalAlarmController":
                    AB_Bools_Alarm(chld, Station);
                    break;
                case "DataLogger":
                    //AB_Bools_CIV_Logger(chld, Station);
                    DataLogger(chld, Station);
                    break;
                default:
                    break;
            }
        }
    }

    private void AB_Bools_UDT(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.Owner.BrowseName;


        Obj.GetVariable("Value").ResetDynamicLink();
        var myVar = Obj.GetVariable("Value");                                // Get the variable
        IUAVariable tempVariable2 = null;                                    // Create a fake variable            
        myVar.SetDynamicLink(tempVariable2, DynamicLinkMode.ReadWrite);      // Add a dynamic link to the fake variable (to trigger the link mechanism)
        string string1 = "../../../../../../CommDrivers/RAEtherNet_IPDriver1/AB_MagnaPak";  // @Erik  how to make this dynamic??
        string string2 = "/Tags/Controller Tags/BOOLS";
        string string3 = "[" + Obj.Owner.BrowseName +"]";
        string strings = string1 + string2 + string3;                       // ../../../../../../CommDrivers/RAEtherNet_IPDriver1/AB_MagnaPak/Tags/Controller Tags/BOOLS[0]
        myVar.GetVariable("DynamicLink").Value = strings;                   // Replace the dynamic link content to the real target value   
    }

    private void AB_Bools_CIV_Logger(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
       // Obj.GetVariable("TableName").ResetDynamicLink();
       // Obj.GetVariable("TableName").Value = Obj.Owner.BrowseName + "_CIV";

       // Obj.GetVariable("Store").ResetDynamicLink();
        //Obj.GetVariable("Store").SetDynamicLink(Project.Current.GetVariable("DataStores/" + Obj.Owner.Owner.Owner.BrowseName + "/Filename"), DynamicLinkMode.ReadWrite);
       // string tmp = Obj.GetVariable("Store").GetVariable("DynamicLink").Value;
       // Obj.GetVariable("Store").GetVariable("DynamicLink").Value = tmp.Replace("/Filename", "@NodeId");

    }
    private void AB_Bools_Alarm(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        string area = Obj.Owner.Owner.BrowseName;
        string device_name = Obj.Owner.BrowseName;
        string device_description = Obj.Owner.GetVariable("Bool/Device_Description").Value;

        Obj.GetVariable("Message").ResetDynamicLink();
        Obj.GetVariable("Message").Value = $"Alarm {device_name} {area} {device_description}";
        
    }


    /*************************************************************************************************************
    ****************************************    AB Accumulator    ************************************************
    **************************************************************************************************************/
    private void AB_ACC_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");

        Obj.GetVariable("PLC").ResetDynamicLink();
        Obj.GetVariable("PLC").Value = Obj.Owner.Owner.BrowseName;

        Obj.GetVariable("Area").ResetDynamicLink();
        Obj.GetVariable("Area").Value = Obj.Owner.BrowseName;

        Obj.GetVariable("Device_Name").ResetDynamicLink();
        Obj.GetVariable("Device_Name").Value = Obj.BrowseName;

        //Get the device
        var deviceTag = Station.GetVariable("Tags/Controller Tags/" + Obj.Owner.BrowseName);

        Obj.GetVariable("This_Day").ResetDynamicLink();
        Obj.GetVariable("This_Day").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/This_Day"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("This_Week").ResetDynamicLink();
        Obj.GetVariable("This_Week").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/This_Week"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("This_Month").ResetDynamicLink();
        Obj.GetVariable("This_Month").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/This_Month"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("This_Year").ResetDynamicLink();
        Obj.GetVariable("This_Year").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/This_Year"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("This_Lifetime").ResetDynamicLink();
        Obj.GetVariable("This_Lifetime").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/This_Lifetime"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Last_Day").ResetDynamicLink();
        Obj.GetVariable("Last_Day").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Last_Day"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Last_Week").ResetDynamicLink();
        Obj.GetVariable("Last_Week").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Last_Week"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Last_Month").ResetDynamicLink();
        Obj.GetVariable("Last_Month").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Last_Month"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Last_Year").ResetDynamicLink();
        Obj.GetVariable("Last_Year").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Last_Year"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Average_Count").ResetDynamicLink();
        Obj.GetVariable("Average_Count").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Average_Count"), DynamicLinkMode.ReadWrite);

        Obj.GetVariable("Average_Value").ResetDynamicLink();
        Obj.GetVariable("Average_Value").SetDynamicLink(deviceTag.GetVariable($"{Obj.BrowseName}" + "/Average_Value"), DynamicLinkMode.ReadWrite);

        
    }






















    /*************************************************************************************************************
    ****************************************       Siemens       ************************************************
    **************************************************************************************************************/






    /*************************************************************************************************************
    ****************************************     Siemens   PLC    ************************************************
    **************************************************************************************************************/
    private void SIE_PLC(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");
        //Obj.GetVariable("PLC").ResetDynamicLink();
        //Obj.GetVariable("PLC").Value = Obj.Owner.BrowseName;

        //Obj.GetVariable("System_Name").ResetDynamicLink();
        //Obj.GetVariable("System_Name").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/System_Name"), DynamicLinkMode.ReadWrite);

        //Obj.GetVariable("System_Location").ResetDynamicLink();
        //Obj.GetVariable("System_Location").SetDynamicLink(Station.GetVariable("Tags/Controller Tags/System_Location"), DynamicLinkMode.ReadWrite);

        //Obj.GetVariable("Info/Model").ResetDynamicLink();
        //Obj.GetVariable("Info/Model").SetDynamicLink(Station.GetVariable("StationStatusVariables/CatalogNumber"), DynamicLinkMode.ReadWrite);

    }


    /*************************************************************************************************************
    ****************************************   Siemens Config     ************************************************
    **************************************************************************************************************/
    private void SIE_Config(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
    }


    /*************************************************************************************************************
    ****************************************      AB Analyzer     ************************************************
    **************************************************************************************************************/
    private void SIE_ALZ_Object(IUAObject Obj, FTOptix.RAEtherNetIP.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");
    }
   

    /*************************************************************************************************************
    ***************************************  Siemens Analyzer     ************************************************
    **************************************************************************************************************/
    private void SIE_ALZ_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");
    }
    

    /*************************************************************************************************************
    ****************************************  Siemens Level       ************************************************
    **************************************************************************************************************/
    private void SIE_LVL_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");
    }


    /*************************************************************************************************************
    ***************************************   Siemens Digital     ************************************************
    **************************************************************************************************************/
    private void SIE_DIG_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}");
    }


    /*************************************************************************************************************
    ***************************************   Siemens Valve     ************************************************
    **************************************************************************************************************/
    private void SIE_VLV_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
    }


    /*************************************************************************************************************
    ***************************************     Siemens Motor     ************************************************
    **************************************************************************************************************/
    private void SIE_MTR_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
    }

    /*************************************************************************************************************
    ***************************************     Siemens VFD       ************************************************
    **************************************************************************************************************/
    private void SIE_VFD_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
    }


    /*************************************************************************************************************
    ***************************************   Siemens Sequencer   ************************************************
    **************************************************************************************************************/
    private void SIE_SEQ_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
    }

    /*************************************************************************************************************
    ***************************************     Siemens PID       ************************************************
    **************************************************************************************************************/
    private void SIE_PID_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
    }

    /*************************************************************************************************************
    ***************************************   Siemens Setpoints   ************************************************
    **************************************************************************************************************/
    private void SIE_Setpoints_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
    }


    /*************************************************************************************************************
    ***************************************  Siemens Calc Value   ************************************************
    **************************************************************************************************************/
    private void SIE_CV_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        Log.Info($"Adding Device - {Obj.Owner.Owner.Owner.BrowseName} -> {Obj.Owner.Owner.BrowseName} -> {Obj.Owner.BrowseName} -> {Obj.BrowseName}"); 
    }


    /*************************************************************************************************************
    ***************************************    Siemens Bools      ************************************************
    **************************************************************************************************************/
    private void SIE_Bools_Object(IUAObject Obj, FTOptix.S7TiaProfinet.Station Station)
    {
        
    }



    private LongRunningTask myLongRunningTask;
}





