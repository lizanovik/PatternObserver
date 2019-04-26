﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace PatternObserverViaInterfaces.Solution._1
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
            thermostat.Register(heater);
            //объект класса Cooler - подписаться на событие изменения температуры класса Thermostat
            thermostat.Register(cooler);
            //эмуляция изменения температуры объекта класса Thermostat
            thermostat.EmulateTemperatureChange();
            //объект класса Cooler - отписаться от события изменения температуры класса Thermostat
            thermostat.Unregister(cooler);
            //эмуляция изменения температуры объекта класса Thermostat на 45 градусов
            thermostat.EmulateTemperatureChange();
        }
    }

    //интерфейс класса наблюдателя
    public interface IObserver
    {
        void Update(IObservable sender, EventArgs info);
    }

    //интерфейс наблюдаемого класса 
    public interface IObservable
    {
        void Register(IObserver observer);
        void Unregister(IObserver observer);
        void Notify();
    }

    //базовый класс, предоставляющий дополнительную информацию о событии
    // для событий не предоставляющих дополнительную информацию свойство Empty
    public class EventArgs
    {
        public static readonly EventArgs Empty =new EventArgs();
    }

    // класс, предоставляющий дополнительную информацию о событии изменения температуры
    public class TemperatureInfo : EventArgs
    {
        public TemperatureInfo(int temperature)
        {
            Temperature = temperature;
        }

        public int Temperature { get; set; }
    }

    public class Cooler : IObserver
    {
        public Cooler(int temperature) => Temperature = temperature;

        public int Temperature { get; private set; }

        public void Update(IObservable sender, EventArgs args)
        {
            TemperatureInfo info = args as TemperatureInfo;
            Console.WriteLine(info.Temperature > Temperature
                ? $"Cooler: On. Changed:{Math.Abs(info.Temperature - Temperature)}"
                : $"Cooler: Off. Changed:{Math.Abs(info.Temperature - Temperature)}");
        }
    }

    public class Heater : IObserver
    {
        public Heater(int temperature) => Temperature = temperature;

        public int Temperature { get; private set; }

        public void Update(IObservable sender, EventArgs args)
        {
            TemperatureInfo info = args as TemperatureInfo;
            //Thermostat thermostat = sender as Thermostat;
            Console.WriteLine(info.Temperature < Temperature
                ? $"Heater: On. Changed:{Math.Abs(info.Temperature - Temperature)}"
                : $"Heater: Off. Changed:{Math.Abs(info.Temperature - Temperature)}");
        }
    }

    public class Thermostat : IObservable
    {
        private int currentTemperature;

        private Random random = new Random(Environment.TickCount);

        private List<IObserver> observers;

        public Thermostat()
        {
            currentTemperature = 5;
            observers = new List<IObserver>();
        }

        public int CurrentTemperature
        {
            get => currentTemperature;
            private set
            {
                if (value > currentTemperature)
                {
                    currentTemperature = value;
                    Notify();
                }
            }
        }

        public void EmulateTemperatureChange()
        {
            this.CurrentTemperature = random.Next(0, 100);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer?.Update(this, new TemperatureInfo(CurrentTemperature));
            }
        }

        public void Register(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Unregister(IObserver observer)
        {
            observers.Remove(observer);
        }
    }
}