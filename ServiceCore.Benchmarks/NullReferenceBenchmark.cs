using BenchmarkDotNet.Attributes;

namespace ServiceCore.Benchmarks
{
    [MemoryDiagnoser]
    public class NullCheckBenchmark
    {
        private TestService? _field;

        [GlobalSetup]
        public async Task Setup()
        {
            await TestService.Instantiate();
            _field = TestService.Instance;
        }

        // Direct field baseline.
        [Benchmark(Baseline = true)]
        public int DirectField()
        {
            return _field!.Value;
        }

        // Instance property.
        [Benchmark]
        public int InstanceGetter()
        {
            return TestService.Instance.Value;
        }

        // Null check (always true)
        [Benchmark]
        public int AlwaysTrueBranch()
        {
            if (TestService.Instance is not null)
                return 1;
            return 0;
        }

        // TryGet pattern.
        [Benchmark]
        public int TryGetBranch()
        {
            if (TestService.TryGet(out var svc))
                return svc.Value;
            return 0;
        }

        // Unpredictable branch.
        private bool _flip;

        [Benchmark]
        public int UnpredictableBranch()
        {
            _flip = !_flip;

            if (_flip)
                return 1;
            return 0;
        }
    }
}