#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.DataLogger;
using FTOptix.Store;
using FTOptix.SQLiteStore;
using FTOptix.MQTTClient;
using FTOptix.MQTTBroker;
using FTOptix.ODBCStore;
using System.Linq;
using System.Threading;
#endregion

public class rt_LogMonthlyReport : BaseNetLogic
{

    MQTTClient publishClient;
    DataLogger logger;
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        Thread.Sleep(5000);
        publishClient = Project.Current.Get<MQTTClient>("MQTT/MQTTClient1");
        logger = Project.Current.Get<DataLogger>("Loggers/Monthly");
        myPeriodicTask = new PeriodicTask(MonthlyLog, 1000, LogicObject);
        myPeriodicTask.Start();

    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        myPeriodicTask?.Dispose();
    }

    [ExportMethod]
    public void MonthlyLog()
    {
        var plcMonthlyReport = Project.Current.GetVariable("CommDrivers/RAEtherNet_IPDriver1/AB_MagnaPak/Tags/Controller Tags/Monthly_Report");
        Folder modelMonthlyReport = Project.Current.Get<Folder>("Model/Monthly_Data");

        foreach (IUAVariable month in plcMonthlyReport.Children)
        {
            IUAVariable logStatusVal = (IUAVariable)month.Children.ElementAt(18);
            Int32 logStatus = logStatusVal.RemoteRead();

            if (logStatus == 1)
            {
                foreach (FTOptix.RAEtherNetIP.Tag datum in month.Children)
                {
                    if (datum.BrowseName != "Log_Status")
                    {
                        modelMonthlyReport.Get<IUAVariable>(datum.BrowseName).Value = datum.RemoteRead();
                        Log.Info(datum.BrowseName + " = " + datum.Value);
                    }
                }
                logger.Log();
                logStatusVal.RemoteWrite(2);
            }
        }
    }

    private PeriodicTask myPeriodicTask;
}
