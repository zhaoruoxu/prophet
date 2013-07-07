﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prophet.Frontend
{
    public class Message : IEnumerable<Context>
    {
        private readonly int _id;
        private int _traceCount;
        private int _len;
        private byte[] _data;
        private readonly List<Context> _contexts = new List<Context>();

        public int Id { get { return _id; } }
        public int TraceCount { get { return _traceCount; } }
        public int Length { get { return _len; } }
        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (_data == null)
                    _data = value;
                else
                    throw new ApplicationException("Data is already set");
            }
        }

        public Message(int id, int traceCount, int len)
        {
            _id = id;
            _traceCount = traceCount;
            _len = len;
        }

        public Context this[int index]
        {
            get { return _contexts[index]; }
            set { _contexts[index] = value; }
        }

        public List<Context> Contexts { get { return _contexts; } } 

        public void Add(Context c)
        {
            _contexts.Add(c);
        }

        public IEnumerator<Context> GetEnumerator()
        {
            return _contexts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}