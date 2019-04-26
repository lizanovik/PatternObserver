using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PatternObserverViaEventHandler
{
    static class Program
    {
        static void Main(string[] args)
        {
            //coздать объект класса Thermostat
            Thermostat thermostat = new Thermostat();
            //coздать объект класса Heater установив начальную температуру равную 30 градусов
            Heater heater = new Heater(20);
            //coздать объект класса Cooler установив начальную температуру равную 40 градусов
            Cooler cooler = new Cooler(30);
            //объект класса Heater - подписаться на событие изменения температуры класса Thermostat
            thermostat.TemperatureChanged = heater.UpdateForHeater;
            //объект класса Cooler - подписаться на событие изменения температуры класса Thermostat
            thermostat.TemperatureChanged += cooler.UpdateForCooler;
            //эмуляция изменения температуры объекта класса Thermostat
            thermostat.EmulateTemperatureChange();
            //объект класса Cooler - отписаться от события изменения температуры класса Thermostat
            thermostat.TemperatureChanged -= cooler.UpdateForCooler;
            //эмуляция изменения температуры объекта класса Thermostat на 45 градусов
            thermostat.EmulateTemperatureChange();
            //Type type = thermostat.GetType();

            //foreach (var t in type.GetMethods(BindingFlags.Instance | BindingFlags.Public|BindingFlags.NonPublic))
            //{
            //    Console.WriteLine(t.Name);
            //}
        }
    }

    public class Cooler
    {
        public Cooler(int temperature) => Temperature = temperature;

        public int Temperature { get; private set; }

        public void UpdateForCooler(object sender, EventArgs args)
        {
            TemperatureChangedEventArgs changedEventArgs = args as TemperatureChangedEventArgs;
            Console.WriteLine(changedEventArgs.Temperature > Temperature
                ? $"Cooler: On. Changed:{Math.Abs(changedEventArgs.Temperature - Temperature)}"
                : $"Cooler: Off. Changed:{Math.Abs(changedEventArgs.Temperature - Temperature)}");
        }
    }

    public class Heater
    {
        public Heater(int temperature) => Temperature = temperature;

        public int Temperature { get; private set; }

        public void UpdateForHeater(object sender, EventArgs args)
        {
            TemperatureChangedEventArgs changedEventArgs = args as TemperatureChangedEventArgs;
            //Thermostat thermostat = sender as Thermostat;
            Console.WriteLine(changedEventArgs.Temperature < Temperature
                ? $"Heater: On. Changed:{Math.Abs(changedEventArgs.Temperature - Temperature)}"
                : $"Heater: Off. Changed:{Math.Abs(changedEventArgs.Temperature - Temperature)}");
        }
    }

    public class TemperatureChangedEventArgs : EventArgs
    {
        public TemperatureChangedEventArgs(int temperature)
        {
            Temperature = temperature;
        }

        public int Temperature { get; set; }
    }

    public sealed class Thermostat
    {
        private int currentTemperature;

        private Random random = new Random(Environment.TickCount);

        public EventHandler<TemperatureChangedEventArgs> TemperatureChanged { get; set; }

        public Thermostat()
        {
            currentTemperature = 5;
        }

        public int CurrentTemperature
        {
            get => currentTemperature;
            private set
            {
                if (value > currentTemperature)
                {
                    currentTemperature = value;
                    OnTemperatureChanged();
                }
            }
        }

        public void EmulateTemperatureChange()
        {
            this.CurrentTemperature = random.Next(0, 100);
        }

        private void OnTemperatureChanged()
        {
            //execute even with exception!
            var local = TemperatureChanged;

            local?.Invoke(this, new TemperatureChangedEventArgs(CurrentTemperature));
        }
    }
}
