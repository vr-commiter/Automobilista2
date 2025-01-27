using AMS2SharedMemoryNet;
using AMS2SharedMemoryNet.Structs;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MyTrueGear;

class Program
{

    static float lastSpeed = 0;

    private static TrueGearMod _TrueGear = null;

    private static string _SteamExe;
    public const string STEAM_OPENURL = "steam://rungameid/1066890";

    private const int SW_MINIMIZE = 6;
    private const int SW_RESTORE = 9;

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    public static string SteamExePath()
    {
        return (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamExe", null);
    }
    static void Main()
    {
        //当有两个程序运行的时候，关闭前一个程序，保留当前程序
        string currentProcessName = Process.GetCurrentProcess().ProcessName;
        Process[] processes = Process.GetProcessesByName(currentProcessName);
        if (processes.Length > 1)
        {
            if (processes[0].UserProcessorTime.TotalMilliseconds > processes[1].UserProcessorTime.TotalMilliseconds)
            {
                processes[0].Kill();
            }
            else
            {
                processes[1].Kill();
            }
        }

        // 获取当前 Console 窗口句柄
        IntPtr hWnd = GetConsoleWindow();

        if (hWnd != IntPtr.Zero)
        {
            // 最小化窗口
            ShowWindow(hWnd, SW_MINIMIZE);
        }

        _SteamExe = SteamExePath();
        if (_SteamExe != null) Process.Start(_SteamExe, STEAM_OPENURL);

        Thread.Sleep(500);

        MemoryParser memp = new("$pcars2$");

        _TrueGear = new TrueGearMod();


        while (true)
        {
            try
            {
                AMS2Page page = memp.GetPage();
                float acc = (page.mSpeed - lastSpeed) / 0.1f;
                lastSpeed = page.mSpeed;
                CheckGear(page.mGear);
                CheckAcc(page.mGear, acc, page.mSpeed);
                CheckSteering(page.mSpeed,acc, page.mUnfilteredSteering);                
                Thread.Sleep(100);
            }
            catch
            {
                Thread.Sleep(5000);
            }
        }
    }
    static int lastGear = 0;
    static void CheckGear(int gear)
    {
        if (gear >= 0)
        {
            if (gear > lastGear)
            {
                Console.WriteLine("UpShift");
                _TrueGear.Play("UpShift");
            }
            else if (gear < lastGear)
            {
                Console.WriteLine("DownShift");
                _TrueGear.Play("DownShift");
            }
            lastGear = gear;
        }
    }

    static void CheckAcc(int gear,float acc,float speed)
    {
        if (gear < 0)
        {
            
            if (acc < 0)
            {
                int power = Math.Abs((int)(acc / 3));
                if (power >= 15)
                {
                    if (speed == 0) return;
                    Console.WriteLine("VehicleCollision");
                    _TrueGear.Play("VehicleCollision");
                }
                else if (power > 0)
                {
                    if (speed == 0) return;
                    if (power > 5) power = 5;
                    Console.WriteLine("SpeedDownR" + power);
                    _TrueGear.Play("SpeedDownR" + power);
                }
            }
            else if (acc > 0)
            {
                int power = (int)(acc / 2);               
                if (power > 0)
                {
                    if (power > 5) power = 5;
                    Console.WriteLine("SpeedUpR" + power);
                    _TrueGear.Play("SpeedUpR" + power);
                }
            }
        }
        else
        {
            
            if (acc < 0)
            {
                int power = Math.Abs((int)(acc / 3));
                if (power >= 15)
                {
                    if (speed == 0) return;
                    Console.WriteLine("VehicleCollision");
                    _TrueGear.Play("VehicleCollision");
                }
                else if (power > 0)
                {
                    if (speed == 0) return;
                    if (power > 5) power = 5;
                    Console.WriteLine("SpeedDown" + power);
                    _TrueGear.Play($"SpeedDown" + power);
                }
            }
            else if (acc > 0)
            {
                int power = (int)(acc / 2);
                if (power > 0)
                {
                    if (power > 5) power = 5;
                    Console.WriteLine("SpeedUp" + power);
                    _TrueGear.Play("SpeedUp" + power);   
                }
            }
        }
    }

    static void CheckSteering(float speed, float acc,float steering)
    {
        if (acc == 0 || speed < 3)
        {
            return;
        }
        if (Math.Abs(steering) > 0.01)
        {
            int power = Math.Abs((int)(steering * 5));
            if (steering < 0)
            {
                Console.WriteLine("TurnLeft" + power);
                _TrueGear.Play("TurnLeft" + power);
            }
            else if (steering > 0)
            {
                Console.WriteLine("TurnRight" + power);
                _TrueGear.Play("TurnRight" + power);
            }
        }
    }



}



