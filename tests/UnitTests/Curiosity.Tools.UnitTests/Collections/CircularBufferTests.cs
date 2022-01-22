using System;
using System.Linq;
using Curiosity.Tools.Collections;
using FluentAssertions;
using Xunit;

namespace Curiosity.Tools.UnitTests.Collections
{
    /// <summary>
    /// Positive unit tests for <see cref="CircularBuffer{T}"/>
    /// </summary>
    public class CircularBufferTests
    {
       [Fact]
        public void CircularBuffer_GetEnumeratorConstructorCapacity_ReturnsEmptyCollection()
        {
            var buffer = new CircularBuffer<string>(5);
            buffer.ToArray().Should().BeEmpty();
        }

        [Fact]
        public void CircularBuffer_ConstructorSizeIndexAccess_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3 });

            buffer.Capacity.Should().Be(5);
            buffer.Size.Should().Be(4);
            for (int i = 0; i < 4; i++)
            {
                buffer[i].Should().Be(i);
            }
        }

        [Fact]
        public void CircularBuffer_Constructor_ExceptionWhenSourceIsLargerThanCapacity()
        {
            Assert.Throws<ArgumentException>(() => new CircularBuffer<int>(3, new[] { 0, 1, 2, 3 }));
        }

        [Fact]
        public void CircularBuffer_GetEnumeratorConstructorDefinedArray_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3 });

            int x = 0;
            foreach (var item in buffer)
            {
                item.Should().Be(x);
                x++;
            }
        }

        [Fact]
        public void CircularBuffer_PushBack_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5);

            for (int i = 0; i < 5; i++)
            {
                buffer.PushBack(i);
            }

            buffer.Front().Should().Be(0);
            for (int i = 0; i < 5; i++)
            {
                buffer[i].Should().Be(i);
            }
        }

        [Fact]
        public void CircularBuffer_PushBackOverflowingBuffer_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5);

            for (int i = 0; i < 10; i++)
            {
                buffer.PushBack(i);
            }

            buffer.ToArray().Should().Equal(5, 6, 7, 8, 9);
        }

        [Fact]
        public void CircularBuffer_GetEnumeratorOverflowedArray_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5);

            for (int i = 0; i < 10; i++)
            {
                buffer.PushBack(i);
            }

            // buffer should have [5,6,7,8,9]
            int x = 5;
            foreach (var item in buffer)
            {
                item.Should().Be(x);
                x++;
            }
        }

        [Fact]
        public void CircularBuffer_ToArrayConstructorDefinedArray_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3 });

            buffer.ToArray().Should().Equal(0, 1, 2, 3);
        }

        [Fact]
        public void CircularBuffer_ToArrayOverflowedBuffer_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5);

            for (int i = 0; i < 10; i++)
            {
                buffer.PushBack(i);
            }

            buffer.ToArray().Should().Equal(5, 6, 7, 8, 9);
        }

        [Fact]
        public void CircularBuffer_ToArraySegmentsConstructorDefinedArray_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3 });

            var arraySegments = buffer.ToArraySegments();

            arraySegments.Count.Should().Be(2); // length of 2 is part of the contract.
            arraySegments.SelectMany(x => x).Should().Equal(0, 1, 2, 3);
        }

        [Fact]
        public void CircularBuffer_ToArraySegmentsOverflowedBuffer_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5);

            for (int i = 0; i < 10; i++)
            {
                buffer.PushBack(i);
            }

            var arraySegments = buffer.ToArraySegments();
            arraySegments.Count.Should().Be(2); // length of 2 is part of the contract.
            arraySegments.SelectMany(x => x).Should().Equal(5, 6, 7, 8, 9);
        }

        [Fact]
        public void CircularBuffer_PushFront_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5);

            for (int i = 0; i < 5; i++)
            {
                buffer.PushFront(i);
            }

            buffer.ToArray().Should().Equal(4, 3, 2, 1, 0);
        }

        [Fact]
        public void CircularBuffer_PushFrontAndOverflow_CorrectContent()
        {
            var buffer = new CircularBuffer<int>(5);

            for (int i = 0; i < 10; i++)
            {
                buffer.PushFront(i);
            }

            buffer.ToArray().Should().Equal(new[] { 9, 8, 7, 6, 5 });
        }

        [Fact]
        public void CircularBuffer_Front_CorrectItem()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });

            buffer.Front().Should().Be(0);
        }

        [Fact]
        public void CircularBuffer_Back_CorrectItem()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });
            buffer.Back().Should().Be(4);
        }

        [Fact]
        public void CircularBuffer_BackOfBufferOverflowByOne_CorrectItem()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });
            buffer.PushBack(42);
            buffer.ToArray().Should().Equal(1, 2, 3, 4, 42);
            buffer.Back().Should().Be(42);
        }

        [Fact]
        public void CircularBuffer_Front_EmptyBufferThrowsException()
        {
            var buffer = new CircularBuffer<int>(5);

            Assert.Throws<InvalidOperationException>(() => buffer.Front());
        }

        [Fact]
        public void CircularBuffer_Back_EmptyBufferThrowsException()
        {
            var buffer = new CircularBuffer<int>(5);
            Assert.Throws<InvalidOperationException>(() => buffer.Back());
        }

        [Fact]
        public void CircularBuffer_PopBack_RemovesBackElement()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });

            buffer.Size.Should().Be(5);

            buffer.PopBack();

            buffer.Size.Should().Be(4);
            buffer.ToArray().Should().Equal(0, 1, 2, 3);
        }

        [Fact]
        public void CircularBuffer_PopBackInOverflowBuffer_RemovesBackElement()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });
            buffer.PushBack(5);

            buffer.Size.Should().Be(5);
            buffer.ToArray().Should().Equal(1, 2, 3, 4, 5);

            buffer.PopBack();

            buffer.Size.Should().Be(4);
            buffer.ToArray().Should().Equal(1, 2, 3, 4 );
        }

        [Fact]
        public void CircularBuffer_PopFront_RemovesBackElement()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });

            buffer.Size.Should().Be(5);

            buffer.PopFront();

            buffer.Size.Should().Be(4);
            buffer.ToArray().Should().Equal(1, 2, 3, 4);
        }

        [Fact]
        public void CircularBuffer_PopFrontInOverflowBuffer_RemovesBackElement()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });
            buffer.PushFront(5);

            buffer.Size.Should().Be(5);
            buffer.ToArray().Should().Equal( 5, 0, 1, 2, 3 );

            buffer.PopFront();

            buffer.Size.Should().Be(4);
            buffer.ToArray().Should().Equal(0, 1, 2, 3);
        }

        [Fact]
        public void CircularBuffer_SetIndex_ReplacesElement()
        {
            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });

            buffer[1] = 10;
            buffer[3] = 30;

            buffer.ToArray().Should().Equal(0, 10, 2, 30, 4);
        }

        [Fact]
        public void CircularBuffer_WithDifferentSizeAndCapacity_BackReturnsLastArrayPosition()
        {
            // test to confirm this issue does not happen anymore:
            // https://github.com/joaoportela/CircularBuffer-CSharp/issues/2

            var buffer = new CircularBuffer<int>(5, new[] { 0, 1, 2, 3, 4 });

            buffer.PopFront(); // (make size and capacity different)

            buffer.Back().Should().Be(4);
        }
    }
}
