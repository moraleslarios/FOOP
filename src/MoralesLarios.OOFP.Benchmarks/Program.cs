using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;

public static class Program
{
	public static void Main(string[] args)
		=> BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}

[MemoryDiagnoser]
public class MlResultBenchmarks
{
	private static readonly Dictionary<string, object> Metadata = new()
	{
		[MlErrorDetails.MetadataKeys.Expected] = 42,
		["Operation"] = "benchmark"
	};

	private int _value;
	private MlResult<int> _validResult = null!;
	private MlErrorDetails _leftError = null!;
	private MlErrorDetails _rightError = null!;

	[GlobalSetup]
	public void Setup()
	{
		_value = 42;
		_validResult = MlResult<int>.Valid(_value);
		_leftError = MlErrorDetails.FromErrorMessageDetails("left", Metadata);
		_rightError = MlErrorDetails.FromErrorMessageDetails("right", Metadata);
	}

	[Benchmark]
	public MlResult<int> MapBindChain()
		=> _validResult.Map(static value => value + 1)
					  .Bind(static value => MlResult<int>.Valid(value * 2));

	[Benchmark]
	public MlResult<int> CreateValidResult()
		=> MlResult<int>.Valid(_value);

	[Benchmark]
	public MlErrorDetails CreateErrorDetailsWithMetadata()
		=> MlErrorDetails.FromErrorMessageDetails("benchmark", Metadata);

	[Benchmark]
	public MlErrorDetails MergeErrorDetails()
		=> _leftError.Merge(_rightError);
}
