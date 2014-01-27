using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace TestNetWork_Server.serialize
{
    static public class SerializeB
    {
        /// <summary>
        /// Функция сериализует обьект класса Packet в массив байт
        /// </summary>
        /// <param name="packet"> Обьект для серилизации</param>
        /// <returns> Массив байт полученый при серилизации </returns>
        public static byte[] serialize(Object obj)
        {
            MemoryStream stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(stream, obj);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка сериализации: " + e.ToString());
            }
            stream.Seek(0, 0);
            byte[] msg = stream.GetBuffer();
            return msg;
        }
        /// <summary>
        /// Функция десериализует массив байт в обьект класса Packet 
        /// </summary>
        /// <param name="buffer">массив байт для десерилизации</param>
        /// <returns>обьект класса Packet полученый при десерилизации</returns>
        public static Object deserialize(byte[] buffer)
        {
            Object obj = null;

            MemoryStream stream = new MemoryStream(buffer);
            var formatter = new BinaryFormatter();
            stream.Seek(0, 0);

            try
            {
                obj = formatter.Deserialize(stream);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка десериализации: " + e.ToString());
            }

            return obj;
        }
    }
}
