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
using FTOptix.OPCUAClient;
using FTOptix.EventLogger;
using FTOptix.MQTTClient;
#endregion

public class RTDateTimeAdjust : BaseNetLogic
{
    DateTime temp;

    //DateTime dateTime;
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void currentTime()
    {
       //Local Time
       Owner.Get<DateTimePicker>("Rewind_DateAndTime").Value = DateTime.Now;
       temp = DateTime.Now;

       //UTC Time
       //Owner.Get<DateTimePicker>("Rewind_DateAndTime").Value = DateTime.UtcNow;
       //temp = DateTime.UtcNow;
    }

    [ExportMethod]
    public void adjustTime(DateTime Date_Time , float seconds, float minutes, float hours, float days)
    {
        Double mSec = (seconds * 1000) + (minutes * 60000) + (hours * 3600000) + (days * 86400000);
        Owner.Get<DateTimePicker>("Rewind_DateAndTime").Value = temp.AddMilliseconds(mSec);
    }

    [ExportMethod]
    public void Add_Time(DateTime Date_Time , float seconds, float minutes, float hours, float days)
    {
        Double mSec = (seconds * 1000) + (minutes * 60000) + (hours * 3600000) + (days * 86400000);
        Owner.Get<DateTimePicker>("Rewind_DateAndTime").Value = Owner.Get<DateTimePicker>("Rewind_DateAndTime").Value.AddMilliseconds(mSec);
    }
    

}
