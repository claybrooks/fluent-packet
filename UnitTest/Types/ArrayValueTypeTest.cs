using System;
using System.Collections.Generic;
using FluentPacket.Factory;
using FluentPacket.Serializer;
using System.Linq;
using FluentPacket.Types;
using Xunit;

namespace UnitTest.Types
{
    public class ArrayValueTypeTest
    {
        [Fact]
        public void AVTWithDefaultSerializer_ClearShouldMakeEverythingZero()
        {
            ArrayValueType<int> avt = new ArrayValueType<int>(new int[] { 0, 1, 2 });
            avt.Clear();
            Assert.Empty(avt.Value.Where(i => i != 0));
        }

        [Fact]
        public void AVTWithProvidedSerializer_ClearShouldMakeEverythingZero()
        {
            var data = new int[] { 1, 2, 3 };
            ArrayValueType<int> avt = new ArrayValueType<int>(data, new ArrayTypeSerializer<int>(data.Length));
            avt.Clear();
            Assert.Empty(avt.Value.Where(i => i != 0));
        }
    }
}