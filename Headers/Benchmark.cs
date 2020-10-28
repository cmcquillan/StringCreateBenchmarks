using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;
using Microsoft.Net.Http.Headers;

namespace Benchmarks
{
    
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Arabic), MeanColumn, StdErrorColumn, StdDevColumn]
    public class SetHeaderCookieBenchmarks
    {
        private static SetCookieHeaderValue _cookieValue = new SetCookieHeaderValue("DotNet")
        {
            Domain = "www.example.com",
            MaxAge = TimeSpan.FromMinutes(30),
            Expires = DateTimeOffset.Now.AddDays(30),
            Path = "/",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Value = Constants.LoremIpsum,
        };

        [Benchmark]
        public string SetCookieHeaderStringCreate()
        {
            return _cookieValue.ToString();
        }

        [Benchmark(Baseline = true)]
        public string SetCookieHeaderStringBuilder()
        {
            var builder = new StringBuilder();
            _cookieValue.AppendToStringBuilder(builder);
            return builder.ToString();
        }
    }
}