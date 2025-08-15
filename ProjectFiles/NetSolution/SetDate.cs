#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.SQLiteStore;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.Store;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Recipe;
using FTOptix.Core;
using FTOptix.EventLogger;
using FTOptix.AuditSigning;
using FTOptix.OPCUAClient;
using FTOptix.SerialPort;
using FTOptix.MQTTClient;
#endregion

public class SetDate : BaseNetLogic
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
    public void SetCurrDate()
    {
        Project.Current.GetVariable("Model/Resin_Concentrations/LocalTimestamp").Value = DateTime.UtcNow;
        Project.Current.GetVariable("Model/DateQuery").Value = DateTime.UtcNow;
        Project.Current.GetVariable("Model/Rewind/Local_DateTime").Value = DateTime.UtcNow;

    }
}
