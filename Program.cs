using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McRae2005TextTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0].Contains(".txt"))
            {
                Rebuild(args[0]);
            }
            else
            {
                Extract(args[0]);
            }
        }
        public static void Extract(string text)
        {
            var reader = new BinaryReader(File.OpenRead(text));
            reader.BaseStream.Position += 8;
            int count = reader.ReadInt32();
            int[] pointers = new int[count];
            string[] strings = new string[count];
            for (int i = 0; i < count; i++)
            {
                pointers[i] = reader.ReadInt32();
            }
            int pos = (int)reader.BaseStream.Position;
            for (int i = 0; i < count; i++)
            {
                reader.BaseStream.Position = pos;
                reader.BaseStream.Position += pointers[i];
                strings[i] = Utils.ReadString(reader, Encoding.UTF8);
            }
            File.WriteAllLines(text + ".txt", strings);
        }
        public static void Rebuild(string text)
        {
            string[] strings = File.ReadAllLines(text);
            int[] pointers = new int[strings.Length];
            using (BinaryWriter writer = new BinaryWriter(File.Create(text + ".LNG")))
            {
                writer.Write(Encoding.UTF8.GetBytes("LANG"));
                writer.Write(1);
                writer.Write(strings.Length);
                int pos = (int)writer.BaseStream.Position;
                writer.Write(new byte[strings.Length * 4]);
                for (int i = 0; i < strings.Length; i++)
                {
                    pointers[i] = (int)writer.BaseStream.Position - ((strings.Length * 4) + 12);
                    writer.Write(Encoding.UTF8.GetBytes(strings[i]));
                    writer.Write(new byte());
                    Utils.AddPadding(writer, strings[i].Length);
                }
                writer.BaseStream.Position = pos;
                for (int i = 0; i < strings.Length; i++)
                {
                    writer.Write(pointers[i]);
                }
            }
        }
    }
}
