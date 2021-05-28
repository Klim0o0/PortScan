using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortScan
{
    public static class Package
    {
        public static byte[] GetDns()
        {
            var sec_dom = "a";
            var first_dom = "ru";

            return BitConverter.GetBytes((ushort) 20).Reverse()
                .Concat(BitConverter.GetBytes((ushort) 256).Reverse())
                .Concat(BitConverter.GetBytes((ushort) 1).Reverse())
                .Concat(BitConverter.GetBytes((ushort) 0).Reverse())
                .Concat(BitConverter.GetBytes((ushort) 0).Reverse())
                .Concat(BitConverter.GetBytes((ushort) 0).Reverse())
                .Concat(BitConverter.GetBytes((ushort) sec_dom.Length).Reverse())
                .Concat(Encoding.ASCII.GetBytes(sec_dom).Reverse())
                .Concat(BitConverter.GetBytes((ushort) first_dom.Length).Reverse())
                .Concat(Encoding.ASCII.GetBytes(first_dom).Reverse())
                .Concat(BitConverter.GetBytes((ushort) 0).Reverse())
                .Concat(BitConverter.GetBytes((ushort) 1).Reverse())
                .Concat(BitConverter.GetBytes((ushort) 1).Reverse())
                .ToArray();
        }

        public static byte[] GetNtp()
        {
            TimeSpan t = (DateTime.Now - new DateTime(1970, 1, 1));
            return new byte[] {(0 << 6 | 3 << 3 | 4)} // ик (нет пред), версия (3), режим (4 сервер)
                .Concat(new byte[] {1, 0, 236}) // часовой слой, интервал запроса, точность 
                .Concat(BitConverter.GetBytes(0)) // задержка
                .Concat(BitConverter.GetBytes(0)) // дисперсия
                .Concat(BitConverter.GetBytes(0)) // индификатор источника
                .Concat(BitConverter.GetBytes((uint) t.TotalSeconds))
                .Concat(BitConverter.GetBytes((uint) t.TotalMilliseconds)) //Время обновления
                .Concat(BitConverter.GetBytes((uint) t.TotalSeconds))
                .Concat(BitConverter.GetBytes((uint) t.TotalMilliseconds)) //Начальное время
                .Concat(BitConverter.GetBytes((uint) t.TotalSeconds))
                .Concat(BitConverter.GetBytes((uint) t.TotalMilliseconds)) //Время приёма
                .Concat(BitConverter.GetBytes((uint) t.TotalSeconds))
                .Concat(BitConverter.GetBytes((uint) t.TotalMilliseconds)) //Время отправки
                .ToArray();
        }
    }
}