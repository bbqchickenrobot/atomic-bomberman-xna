﻿using System.IO;
using BomberEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BomberEngineTests
{
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

    [TestClass]
    public class BitBufferTests
    {
        [TestMethod]
        public void TestReadWrite()
        {
            byte byteValue = 32;
            int numBitsByte = BitUtils.BitsToHoldUInt(byteValue);

            uint uintValue = 123456;
            int numBitsUInt = BitUtils.BitsToHoldUInt((uint)uintValue);

            BitWriteBuffer wb = new BitWriteBuffer();
            wb.Write(true);
            wb.Write(false);
            wb.Write(byteValue);
            wb.Write(byteValue, numBitsByte);
            wb.Write(uintValue);
            wb.Write(uintValue, numBitsUInt);

            byte[] data = wb.Data;
            using (Stream stream = File.OpenWrite("data.bin"))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(wb.LengthBits);
                    writer.Write(wb.LengthBytes);
                    writer.Write(data, 0, wb.LengthBytes);
                }
            }

            BitReadBuffer rb = null;

            using (Stream stream = File.OpenRead("data.bin"))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int lenBits = reader.ReadInt32();
                    int lenBytes = reader.ReadInt32();
                    data = new byte[lenBytes];
                    reader.Read(data, 0, lenBytes);

                    rb = new BitReadBuffer(data, lenBits);
                }
            }

            Assert.AreEqual(true, rb.ReadBoolean());
            Assert.AreEqual(false, rb.ReadBoolean());

            Assert.AreEqual(byteValue, rb.ReadByte());
            Assert.AreEqual(byteValue, rb.ReadByte(numBitsByte));

            Assert.AreEqual(uintValue, rb.ReadUInt32());
            Assert.AreEqual(uintValue, rb.ReadUInt32(numBitsUInt));
        }
    }
}
