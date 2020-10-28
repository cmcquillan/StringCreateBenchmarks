using System;
using System.Collections.Generic;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;

namespace Benchmarks
{
    public class Dog
    {
        public string Name { get; set; }

        public int? Age { get; set; }

        public string Color { get; set; }

        public override string ToString()
        {
            return StringCreate();
        }

        /// <summary>
        /// Format the description string of the Dog class using String.Create()
        /// </summary>
        public string StringCreate()
        {
            var length = 0;
            // Constants
            const string dogPrefix = "[DOG] ";
            const string unknownName = "Unknown";
            const string leftAgeChunk = " (";
            const char rightAgeChunk = ')';
            const string leftColorChunk = " [";
            const char rightColorChunk = ']';
            static int integerLength(int val) => (int)Math.Floor(Math.Log10((double)val) + 1);

            /* Compute Lengths */
            length += dogPrefix.Length + (Name ?? unknownName).Length; // Prefix + Name

            if (Color is string)
            {
                length += 3 /* left + right chunk length */ + Color.Length;
            }

            if (Age.HasValue)
            {
                length += 3 /* left + right chunk length */ + integerLength(Age.Value); /* Digits in age */
            }
            /* Use State + Computed Length to Build String */
            return String.Create<Dog>(length, this, (buffer, dog) =>
            {
                var prefixSpan = dogPrefix.AsSpan();
                prefixSpan.CopyTo(buffer);
                var span = buffer.Slice(prefixSpan.Length);

                var nameSpan = (dog.Name ?? unknownName).AsSpan();
                nameSpan.CopyTo(span);
                span = span.Slice(nameSpan.Length);

                if(dog.Color is string)
                {
                    leftColorChunk.AsSpan().CopyTo(span);
                    span = span.Slice(2);
                    var colorSpan = dog.Color.AsSpan();
                    colorSpan.CopyTo(span);
                    span = span.Slice(colorSpan.Length);
                    span[0] = rightColorChunk;
                    span = span.Slice(1);
                }

                if(dog.Age.HasValue)
                {
                    leftAgeChunk.AsSpan().CopyTo(span);
                    span = span.Slice(2);
                    dog.Age.Value.TryFormat(span, out int written, provider: CultureInfo.InvariantCulture);
                    span = span.Slice(written);
                    span[0] = rightAgeChunk;
                }
            });
        }

        /// <summary>
        /// Format the description string of the Dog class using basic concatenation.
        /// </summary>
        public string Concatenation() 
        {
            var val = "[DOG] " + (Name ?? "Unknown");

            if(Color is string)
            {
                val += " [" + Color + "]";
            }

            if(Age.HasValue)
            {
                val += " (" + Age.Value + ")";
            }

            return val;
        }

        /// <summary>
        /// Format the description string of the Dog class using String.Format()
        /// </summary>
        public string StringFormat()
        {
            return String.Format("[DOG] {0}{5}{4}{6}{2}{1}{3}",
                Name ?? "Unknown",
                Age,
                Age.HasValue ? " (" : String.Empty,
                Age.HasValue ? ")" : String.Empty,
                Color,
                Color is string ? " [" : String.Empty,
                Color is string ? "]" : String.Empty);
        }
    }

    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Arabic), MeanColumn, StdErrorColumn, StdDevColumn]
    public class ComplexFormattingBenchmarks
    {
        [ParamsSource(nameof(Dogs))]
        public Dog Dog { get; set; }

        public IEnumerable<Dog> Dogs()
        {
            return new List<Dog>()
            {
                new Dog { Name = "Fido", Age = 20, Color = "Brown" },
                new Dog { Name = "Pluto", Age = null, Color = "Yellow", },
                new Dog { Name = "Fluffy", Age = null, Color = null },
            };
        }

        [Benchmark(Baseline = true)]
        public string ComplexFormatting_StringFormat()
        {
            return Dog.StringFormat();
        }

        [Benchmark]
        public string ComplexFormatting_StringCreate()
        {
            return Dog.StringCreate();
        }

        [Benchmark]
        public string ComplexFormatting_Concatenation()
        {
            return Dog.Concatenation();
        }
    }
}