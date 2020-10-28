using System;
using System.Text;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;

namespace Benchmarks
{

    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Arabic), MeanColumn, StdErrorColumn, StdDevColumn]
    public class IdGeneratorBenchmarks
    {
        [Benchmark]
        public string StringCreate() => CorrelationIdGeneratorStringCreate.GetNextId();

        [Benchmark(Baseline = true)]
        public string StringBuilder() => CorrelationIdGeneratorStringBuilder.GetNextId();

        [Benchmark]
        public string StringConcatenation() => CorrelationIdGeneratorConcatenation.GetNextId();

        [Benchmark]
        public string StringBuilderNoCapacity() => CorrelationIdGeneratorStringBuilderNoCap.GetNextId();
    }

    public static class CorrelationIdGeneratorConcatenation
    {
        private static readonly char[] s_encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();
        private static long _lastId = DateTime.UtcNow.Ticks;
        public static string GetNextId() => GenerateId(Interlocked.Increment(ref _lastId));
        private static string GenerateId(long id)
        {
            char[] encode32Chars = s_encode32Chars;

            return String.Concat(
                encode32Chars[(id >> 60) & 31],
                encode32Chars[(id >> 55) & 31],
                encode32Chars[(id >> 50) & 31],
                encode32Chars[(id >> 45) & 31],
                encode32Chars[(id >> 40) & 31],
                encode32Chars[(id >> 35) & 31],
                encode32Chars[(id >> 30) & 31],
                encode32Chars[(id >> 25) & 31],
                encode32Chars[(id >> 20) & 31],
                encode32Chars[(id >> 15) & 31],
                encode32Chars[(id >> 10) & 31],
                encode32Chars[(id >> 5) & 31],
                encode32Chars[id & 31]
            );
        }
    }

    public static class CorrelationIdGeneratorStringBuilderNoCap
    {
        // Base32 encoding - in ascii sort order for easy text based sorting
        private static readonly char[] s_encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();

        // Seed the _lastConnectionId for this application instance with
        // the number of 100-nanosecond intervals that have elapsed since 12:00:00 midnight, January 1, 0001
        // for a roughly increasing _lastId over restarts
        private static long _lastId = DateTime.UtcNow.Ticks;

        public static string GetNextId() => GenerateId(Interlocked.Increment(ref _lastId));

        private static string GenerateId(long id)
        {
            char[] encode32Chars = s_encode32Chars;

            var builder = new StringBuilder();
            builder.Append(encode32Chars[(id >> 60) & 31]);
            builder.Append(encode32Chars[(id >> 55) & 31]);
            builder.Append(encode32Chars[(id >> 50) & 31]);
            builder.Append(encode32Chars[(id >> 45) & 31]);
            builder.Append(encode32Chars[(id >> 40) & 31]);
            builder.Append(encode32Chars[(id >> 35) & 31]);
            builder.Append(encode32Chars[(id >> 30) & 31]);
            builder.Append(encode32Chars[(id >> 25) & 31]);
            builder.Append(encode32Chars[(id >> 20) & 31]);
            builder.Append(encode32Chars[(id >> 15) & 31]);
            builder.Append(encode32Chars[(id >> 10) & 31]);
            builder.Append(encode32Chars[(id >> 5) & 31]);
            builder.Append(encode32Chars[id & 31]);

            return builder.ToString();
        }
    }


    public static class CorrelationIdGeneratorStringBuilder
    {
        // Base32 encoding - in ascii sort order for easy text based sorting
        private static readonly char[] s_encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();

        // Seed the _lastConnectionId for this application instance with
        // the number of 100-nanosecond intervals that have elapsed since 12:00:00 midnight, January 1, 0001
        // for a roughly increasing _lastId over restarts
        private static long _lastId = DateTime.UtcNow.Ticks;

        public static string GetNextId() => GenerateId(Interlocked.Increment(ref _lastId));

        private static string GenerateId(long id)
        {
            char[] encode32Chars = s_encode32Chars;

            var builder = new StringBuilder(13);
            builder.Append(encode32Chars[(id >> 60) & 31]);
            builder.Append(encode32Chars[(id >> 55) & 31]);
            builder.Append(encode32Chars[(id >> 50) & 31]);
            builder.Append(encode32Chars[(id >> 45) & 31]);
            builder.Append(encode32Chars[(id >> 40) & 31]);
            builder.Append(encode32Chars[(id >> 35) & 31]);
            builder.Append(encode32Chars[(id >> 30) & 31]);
            builder.Append(encode32Chars[(id >> 25) & 31]);
            builder.Append(encode32Chars[(id >> 20) & 31]);
            builder.Append(encode32Chars[(id >> 15) & 31]);
            builder.Append(encode32Chars[(id >> 10) & 31]);
            builder.Append(encode32Chars[(id >> 5) & 31]);
            builder.Append(encode32Chars[id & 31]);

            return builder.ToString();
        }
    }

    public static class CorrelationIdGeneratorStringCreate
    {
        // Base32 encoding - in ascii sort order for easy text based sorting
        private static readonly char[] s_encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV".ToCharArray();

        // Seed the _lastConnectionId for this application instance with
        // the number of 100-nanosecond intervals that have elapsed since 12:00:00 midnight, January 1, 0001
        // for a roughly increasing _lastId over restarts
        private static long _lastId = DateTime.UtcNow.Ticks;

        public static string GetNextId() => GenerateId(Interlocked.Increment(ref _lastId));

        private static string GenerateId(long id)
        {
            return string.Create(13, (value: id, map: s_encode32Chars), (buffer, p) =>
            {
                char[] encode32Chars = p.map;
                long value = p.value;

                buffer[12] = encode32Chars[value & 31];
                buffer[11] = encode32Chars[(value >> 5) & 31];
                buffer[10] = encode32Chars[(value >> 10) & 31];
                buffer[9] = encode32Chars[(value >> 15) & 31];
                buffer[8] = encode32Chars[(value >> 20) & 31];
                buffer[7] = encode32Chars[(value >> 25) & 31];
                buffer[6] = encode32Chars[(value >> 30) & 31];
                buffer[5] = encode32Chars[(value >> 35) & 31];
                buffer[4] = encode32Chars[(value >> 40) & 31];
                buffer[3] = encode32Chars[(value >> 45) & 31];
                buffer[2] = encode32Chars[(value >> 50) & 31];
                buffer[1] = encode32Chars[(value >> 55) & 31];
                buffer[0] = encode32Chars[(value >> 60) & 31];
            });
        }
    }
}