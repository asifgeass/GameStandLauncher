using ExceptionManager;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class ComPort
    {
        public static SerialPort port=null;
        public static async Task PortReaderStart()
        {
            await PortSearching();
            PortReading().RunAsync();
        }

        private static async Task PortSearching()
        {
            Ex.Log("ComPort.PortSearching()");
            await Task.Delay(2000);
            while (port == null)
            {
                port = await FindPort();
                await Task.Delay(2000);
            }
            Ex.Log($"ComPort.PortSearching() Получен порт ардуино с нужными данными - {port.PortName}.");
        }

        private static async Task PortReading()
        {
            Ex.Log("ComPort.PortReading()");
            bool NoErrors = true;
            while (NoErrors)
            {
                try
                {
                    if (port.IsOpen)
                    {
                        int gotbytes = 0;
                        Ex.Try(() =>
                        { gotbytes = port.BytesToRead; });
                        if (gotbytes > 0)
                        {
                            await Task.Run(async () =>
                            {
                                var answer = ReadPortIncomes(port);
                                if (answer.Txt.Contains("{button=1}"))
                                {
                                    Ex.Log("ComPort.PortReading() Кнопка закрытия игры нажата.");
                                    await GameManager.KillAllGames();
                                    await Task.Delay(2000);
                                    port.DiscardInBuffer();
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    NoErrors = false;
                    port = null;
                    PortReaderStart().RunAsync();
                    Ex.Log($"ComPort.PortReading() ERROR & Finish: {ex.Message}");
                }
                await Task.Delay(800);
            }
        }

        public static async Task<SerialPort> FindPort()
        {            
            string[] ports = SerialPort.GetPortNames();
            foreach (var item in ports)
            {
                //Ex.Log($"В системе найден порт={item};");
                SerialPort port = null;
                try
                {
                    port = new SerialPort(item);
                    port.BaudRate = 9600;
                    port.RtsEnable = true;
                    //port.ReadTimeout = 2000;                   
                }
                catch (Exception ex)
                {
                    //ex.Log($"Не удалось создать порт с именем={item};");
                    continue;
                }
                try
                {
                    port.Open();
                }
                catch (Exception ex)
                {
                    //ex.Log($"Не удалось открыть порт={item};");
                    continue;
                }
                //Ex.Log($"{item}: Ошибок нет. Осталось прочитать.");
                var data = ReadPortIncomes(port);
                //Ex.Log($"Прочитано из порта (текст)={data.Txt};");
                //Ex.Log($"Прочитано из порта (код)={data.Hex};");
                if (data.Txt.Contains("{button="))
                {                    
                    return port;
                }
                await Task.Delay(1000);
                data = ReadPortIncomes(port);
                //Ex.Log($"Прочитано из порта (текст)={data.Txt};");
                //Ex.Log($"Прочитано из порта (код)={data.Hex};");
                if (data.Txt.Contains("{button="))
                {                    
                    return port;
                }
            }
            return null;
        }
        private static HexString ReadPortIncomes(SerialPort port)
        {
            int gotbytes = 0;
            Ex.Try(() =>
            { gotbytes = port.BytesToRead; });
            //Ex.Log($"gotbytes={gotbytes}");
            //Ex.Log($"Обнаружено {gotbytes} байт для чтения.");
            var gotPortIncome = new HexString();
            try
            {
                for (int i = 0; i < gotbytes; i++)
                {
                    int bit = port.ReadByte();
                    gotPortIncome.AddByte(bit);
                }
            }
            catch (TimeoutException ex)
            { ex.Log("ReadPortIncomes TimeoutException"); }
            catch (Exception ex)
            { ex.Log("ReadPortIncomes Exception"); }
            return gotPortIncome;
        }
    }
}
