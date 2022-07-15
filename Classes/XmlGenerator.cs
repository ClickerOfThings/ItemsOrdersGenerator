using ItemsOrdersGenerator.Classes.Helpers;
using ItemsOrdersGenerator.Classes.Model;
using ItemsOrdersGenerator.Classes.View;
using ItemsOrdersGenerator.Classes.Extensions;
using OverpassLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ItemOrderDemonstration.Classes
{
    internal static class XmlGenerator
    {
        public static void SerializeItemsListToFile(string fileName, List<Item> itemsToSerialize)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Item>), new XmlRootAttribute("skus"));
            using FileStream writer = File.Create(fileName);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // убираем лишние неймспейсы
            serializer.Serialize(writer, itemsToSerialize, ns);
        }

        public static List<Item> DeserializeItemsFromFileToList(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Item>), new XmlRootAttribute("skus"));
            using FileStream reader = File.Open(fileName, FileMode.Open);
            var deserialised = serializer.Deserialize(reader);
            return deserialised as List<Item>;
        }


        public static void GenerateOrdersFile(List<Item> items, List<OsmClass> listOfPoints,
            DateTime ordersDateTime, string ordersFileName, int pointsCount,
            Tuple<int, int> minMaxTimeWindows,
            Tuple<int, int> minMaxItemsPerWindow, Tuple<int, int> minMaxItemCountPerPosition,
            Tuple<TimeSpan, TimeSpan> timespanFromTo, TimeSpan intervalBetweenFromTo)
        {
            items.ShuffleList();
            listOfPoints.ShuffleList();

            XmlSerializer serializer = new XmlSerializer(typeof(OrdersListView));
            using FileStream writer = File.Create(ordersFileName);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // убираем лишние неймспейсы
            OrdersListView orders = new OrdersListView()
            {
                DateOfOrders = ordersDateTime
            };
            for (int i = 0; i < pointsCount; i++)
            {
                Order newOrder = new Order
                {
                    RetailPoint = new RetailPoint
                    {
                        Address = listOfPoints[i].FullAddressString.ToUpper(),
                        Id = Guid.NewGuid(),
                        Name = listOfPoints[i].Name.ToUpper(),
                        Location = new Location
                        {
                            Longitude = listOfPoints[i].Longitude,
                            Latitude = listOfPoints[i].Latitude
                        }
                    }
                };
                newOrder.Demands = new List<Demand>();
                int demandWindowsCount = GeneralHelper.rand.Next(minMaxTimeWindows.Item1, minMaxTimeWindows.Item2 + 1);
                int itemsPerWindowCount = GeneralHelper.rand.Next(minMaxItemsPerWindow.Item1, minMaxItemsPerWindow.Item2 + 1);
                IEnumerable<Tuple<TimeSpan, TimeSpan>> timespansEnumerable =
                    Demand.RandomTimespans(GeneralHelper.rand, timespanFromTo.Item1, timespanFromTo.Item2,
                    intervalBetweenFromTo, demandWindowsCount);
                foreach (Tuple<TimeSpan, TimeSpan> tupleFromTo in
                    timespansEnumerable)
                {
                    items.ShuffleList();
                    for (int k = 0; k < itemsPerWindowCount; k++)
                    {
                        Demand newDemand = new Demand
                        {
                            Position = new Position
                            {
                                Amount = GeneralHelper.rand.Next(minMaxItemCountPerPosition.Item1, minMaxItemCountPerPosition.Item2 + 1),
                                ItemId = items[k].Id,
                            },
                            From = tupleFromTo.Item1,
                            To = tupleFromTo.Item2
                        };
                        newOrder.Demands.Add(newDemand);
                    }
                }
                orders.Orders.Add(newOrder);
            }

            serializer.Serialize(writer, orders, ns);
            return;
        }
    }
}
