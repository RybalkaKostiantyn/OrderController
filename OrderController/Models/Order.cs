using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderController.Models
{
    public class Order
    {
        public int id { get; set; }
        public string providerName { get; set; }
        public string description { get; set; }
        public string creationData { get; set; }
        public List<Modification> modificationData { get; set; }
        public string manager { get; set; }
        public int quantity { get; set; }
        public int amount { get; set; }
        public string region { get; set; }

        public Order()
        {

        }
        public Order(int ID, string Provider, string Description, string CreationData, string Manager, int Quantity, int Amount, string Region)
        {
            id = ID;
            providerName = Provider;
            description = Description;
            creationData = CreationData;
            //modificationData = new List<Modification>(); //его в том месте где они нужны
            manager = Manager;
            quantity = Quantity;
            amount = Amount;
            region = Region;
        }

        public Order(Order o)
        {
            id = o.id;
            providerName = o.providerName;
            description = o.description;
            creationData = o.creationData;
            modificationData = o.modificationData;
            manager = o.manager;
            quantity = o.quantity;
            amount = o.amount;
            region = o.region;
        }

        /// <summary>
        /// Replacing data in Order
        /// </summary>
        /// <param name="m">Data to replace</param>
        public void Undo(Modification m)
        {
            providerName = m.providerName;
            description = m.description;
            manager = m.manager;
            quantity = m.quantity;
            amount = m.amount;
            region = m.region;
        }
    }
}

/*
 * в web-config sql connection string - оставить
 * объект страницы - номер, размер, сортировка
 * вытягивать не все значения
 * sql connection - в config file
 * sql commands - перед page load
 * sessions - убрать
 * table - убрать и забайндить куда-то в другое место
 * button - не добавлять динамически, закинуть на форму
 * сортировка в control
 * exceprion - вкури
 * связанная страница - передавать ордеры через конструктор страницы
 * 
 * 
 * Собственный атрибут и с помошью рефлексии работать (property отмеченные атрибутом)
 * smartgit - познакомиться
 * visual studio 19 - перейти
 * xmpy - 
 * андрей дмитренко - пм
 * разобраться как работают плагины вообще
 * 
 */
