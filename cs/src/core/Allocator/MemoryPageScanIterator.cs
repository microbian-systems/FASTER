﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace FASTER.core
{
    /// <summary>
    /// Lightweight iterator for memory page (copied to buffer).
    /// Can be used outside epoch protection.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    class MemoryPageScanIterator<Key, Value> : IFasterScanIterator<Key, Value>
    {
        readonly Record<Key, Value>[] page;
        readonly int start, end;
        int offset;
        

        public MemoryPageScanIterator(Record<Key, Value>[] page, int start, int end)
        {
            this.page = new Record<Key, Value>[page.Length];
            Array.Copy(page, start, this.page, start, end - start);
            offset = start - 1;
            this.start = start;
            this.end = end;
        }

        public long CurrentAddress => offset;

        public long NextAddress => offset + 1;

        public long BeginAddress => start;

        public long EndAddress => end;

        public void Dispose()
        {
        }

        public ref Key GetKey()
        {
            return ref page[offset].key;
        }

        public bool GetNext(out RecordInfo recordInfo)
        {
            while (true)
            {
                offset++;
                if (offset >= end)
                {
                    recordInfo = default;
                    return false;
                }
                if (!page[offset].info.Invalid)
                    break;
            }

            recordInfo = page[offset].info;
            return true;
        }

        public bool GetNext(out RecordInfo recordInfo, out Key key, out Value value)
        {
            var r = GetNext(out recordInfo);
            if (r)
            {
                key = page[offset].key;
                value = page[offset].value;
            }
            else
            {
                key = default;
                value = default;
            }
            return r;
        }

        public ref Value GetValue()
        {
            return ref page[offset].value;
        }
    }
}
