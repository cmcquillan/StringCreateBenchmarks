using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;

namespace Benchmarks
{
    [MediumRunJob]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [Outliers(Perfolizer.Mathematics.OutlierDetection.OutlierMode.DontRemove)]
    [RankColumn(NumeralSystem.Arabic), MeanColumn, StdErrorColumn, StdDevColumn]
    public class ConcatenationBenchmarks
    {
        [Params("Lorem Ipsum", null)]
        public string FirstPart { get; set; }

        [Params(Constants.LoremIpsum, null)]
        public string SecondPart { get; set; }

        [Benchmark(Baseline = true)]
        public string Normal()
        {
            return ConcatenationNormal.Concat("Lorem Ipsum", Constants.LoremIpsum);
        }

        [Benchmark]
        public string StringCreate()
        {
            return ConcatenationStringCreate.Concat("Lorem Ipsum", Constants.LoremIpsum);
        }
    }

    public static class ConcatenationNormal
    {
        public static string Concat(string first, string second)
        {
            first ??= string.Empty;
            if (second != null)
            {
                first = first + ' ' + second;
            }

            return first;
        }
    }

    public static class ConcatenationStringCreate
    {
        public static string Concat(string first, string second)
        {
            first ??= string.Empty;
            second ??= String.Empty;
            bool addSpace = second.Length > 0;

            int length = first.Length + (addSpace ? 1 : 0) + second.Length;
            return string.Create(length, (first, second, addSpace),
            (dst, v) =>
            {
                ReadOnlySpan<char> prefix = v.first;
                prefix.CopyTo(dst);

                if (v.addSpace)
                {
                    dst[prefix.Length] = ' ';

                    ReadOnlySpan<char> detail = v.second;
                    detail.CopyTo(dst.Slice(prefix.Length + 1, detail.Length));
                }
            });
        }
    }
}