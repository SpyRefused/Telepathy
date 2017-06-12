using System;
using System.Runtime.InteropServices;
using Telepathy.Core.Memory.Pool;

namespace Telepathy.Core.Memory
{
    public class SegmentedLongArray
    {
        protected readonly long[][] Segments;
        protected readonly int Log2OfSegmentSize;
        protected readonly int Bitmask;

        public SegmentedLongArray(IArraySegmentRecycler memoryRecycler, long numLongs)
        {
            Log2OfSegmentSize = memoryRecycler.GetLog2OfLongSegmentSize();

            // When the left operand of the >> operator is of a signed integral type, 
            // the operator performs an arithmetic shift right wherein the value of the most 
            // significant bit(the sign bit) of the operand is propagated to the high-order empty bit positions.
            // When the left operand of the >> operator is of an unsigned integral type, the operator 
            // performs a logical shift right wherein high - order empty bit positions are always set to zero. 
            // To perform the opposite operation of that inferred from the operand type, explicit casts can be used.
            // For example, if x is a variable of type int, the operation unchecked((int)((uint) x >> y)) performs a logical shift right of x.
            var numSegments = (int)((uint)(numLongs - 1) >> Log2OfSegmentSize) + 1;
            var segments = new long[numSegments][];

            Bitmask = (1 << Log2OfSegmentSize) - 1;

            for (var i = 0; i < segments.Length; i++)
            {
                segments[i] = memoryRecycler.GetLongArray();
            }
            // The following assignment is purposefully placed *after* the population of all segments.
            // The final assignment after the initialization of the array guarantees that no thread
            // will see any of the array elements before assignment.
            // We can't risk the segment values being visible as null to any thread, because
            // FixedLengthElementArray uses Unsafe to access these values, which would cause the
            // JVM to crash with a segmentation fault.
            Segments = segments;
        }

        
        // Set the byte at the given index to the specified value        
        public unsafe void Set(long index, long value)
        {
            var segmentIndex = (int)(index >> Log2OfSegmentSize);
            var longInSegment = (int)(index & Bitmask);

            //unsafe.putOrderedLong(segments[segmentIndex], (long)Unsafe.ARRAY_LONG_BASE_OFFSET + (8 * longInSegment), value);

            // duplicate the longs here so that we can read faster.
            //if (longInSegment == 0 && segmentIndex != 0)
                //unsafe.putOrderedLong(segments[segmentIndex - 1], (long)Unsafe.ARRAY_LONG_BASE_OFFSET + (8 * (1 << log2OfSegmentSize)), value);

            Marshal.SizeOf()
            byte[] byteArray = new byte[Marshal.SizeOf(structure)];
            fixed (byte* byteArrayPtr = byteArray)
            {
                Marshal.StructureToPtr(structure, (IntPtr)byteArrayPtr, true);
            }
        }
    }
}
