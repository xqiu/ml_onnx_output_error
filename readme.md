This program is to show a problem I get when using ML.NET to load an ONNX model and predict with it, neither single float output definition, nor float[] array output definition works.


With https://netron.app/ to check sample.onnx, you can see its config
~~~
format ONNX v8
producer pytorch 2.4.0
version 0
imports ai.onnx v17
graph main_graph

input
	name: input
	tensor: float32[batch_size,1,40,40]

output
	name: output
	tensor: float32
~~~

When running ml_onnx_output_error.exe, you will see the problem:
~~~
Run PredictDirect:
Direct Prediction result:
Output[0]: -20.574154

Run PredictWithMLContext:
System.ArgumentNullException: Value cannot be null. (Parameter 'source')
   at System.Linq.ThrowHelper.ThrowArgumentNullException(ExceptionArgument argument)
   at System.Linq.Enumerable.Select[TSource,TResult](IEnumerable`1 source, Func`2 selector)
   at Microsoft.ML.Transforms.Onnx.OnnxTransformer.Mapper.<>c__DisplayClass16_0`1.<MakeObjectGetter>b__0(T& dst)
   at Microsoft.ML.Data.TypedCursorable`1.TypedRowBase.<>c__DisplayClass10_0`1.<CreateDirectSetter>b__0(TRow row)
   at Microsoft.ML.Data.TypedCursorable`1.TypedRowBase.FillValues(TRow row)
   at Microsoft.ML.Data.TypedCursorable`1.RowImplementation.FillValues(TRow row)
   at Microsoft.ML.PredictionEngineBase`2.FillValues(TDst prediction)
   at Microsoft.ML.PredictionEngine`2.Predict(TSrc example, TDst& prediction)
   at Microsoft.ML.PredictionEngineBase`2.Predict(TSrc example)
   at SpeakerModel.Program.PredictWithMLContextSingle(String modelPath) in E:\github\ml_onnx_problem_demo\Program.cs:line 131
   at SpeakerModel.Program.Main(String[] args) in E:\github\ml_onnx_problem_demo\Program.cs:line 50

Run PredictWithMLContextArray:
System.InvalidOperationException: Can't bind the IDataView column 'output' of type 'Single' to field or property 'Prediction' of type 'System.Single[]'.
   at Microsoft.ML.Data.TypedCursorable`1..ctor(IHostEnvironment env, IDataView data, Boolean ignoreMissingColumns, InternalSchemaDefinition schemaDefn)
   at Microsoft.ML.Data.TypedCursorable`1.Create(IHostEnvironment env, IDataView data, Boolean ignoreMissingColumns, SchemaDefinition schemaDefinition)
   at Microsoft.ML.PredictionEngineBase`2.PredictionEngineCore(IHostEnvironment env, InputRow`1 inputRow, IRowToRowMapper mapper, Boolean ignoreMissingColumns, SchemaDefinition outputSchemaDefinition, Action& disposer, IRowReadableAs`1& outputRow)
   at Microsoft.ML.PredictionEngineBase`2..ctor(IHostEnvironment env, ITransformer transformer, Boolean ignoreMissingColumns, SchemaDefinition inputSchemaDefinition, SchemaDefinition outputSchemaDefinition, Boolean ownsTransformer)
   at Microsoft.ML.PredictionEngine`2..ctor(IHostEnvironment env, ITransformer transformer, Boolean ignoreMissingColumns, SchemaDefinition inputSchemaDefinition, SchemaDefinition outputSchemaDefinition, Boolean ownsTransformer)
   at Microsoft.ML.PredictionEngineExtensions.CreatePredictionEngine[TSrc,TDst](ITransformer transformer, IHostEnvironment env, Boolean ignoreMissingColumns, SchemaDefinition inputSchemaDefinition, SchemaDefinition outputSchemaDefinition, Boolean ownsTransformer)
   at Microsoft.ML.ModelOperationsCatalog.CreatePredictionEngine[TSrc,TDst](ITransformer transformer, Boolean ignoreMissingColumns, SchemaDefinition inputSchemaDefinition, SchemaDefinition outputSchemaDefinition)
   at SpeakerModel.Program.PredictWithMLContextArray(String modelPath) in E:\github\ml_onnx_problem_demo\Program.cs:line 162
   at SpeakerModel.Program.Main(String[] args) in E:\github\ml_onnx_problem_demo\Program.cs:line 59
~~~
