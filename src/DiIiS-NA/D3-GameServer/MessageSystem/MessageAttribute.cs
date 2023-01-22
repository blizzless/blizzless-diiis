﻿//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public List<Opcodes> Opcodes { get; private set; }
        public Consumers Consumer { get; private set; }

        public MessageAttribute(Opcodes opcode, Consumers consumer = Consumers.None)
        {
            Opcodes = new List<Opcodes> { opcode };
            Consumer = consumer;
        }

        public MessageAttribute(Opcodes[] opcodes, Consumers consumer = Consumers.None)
        {
            Opcodes = new List<Opcodes>();
            Consumer = consumer;

            foreach (var opcode in opcodes)
            {
                Opcodes.Add(opcode);
            }
        }
    }
}
