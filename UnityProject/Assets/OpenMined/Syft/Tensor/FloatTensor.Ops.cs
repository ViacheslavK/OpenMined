using UnityEngine;
using System;
using System.Threading.Tasks;

namespace OpenMined.Syft.Tensor
{
public partial class FloatTensor
{

private FloatTensor emptyTensorCopy() {
	return new FloatTensor(shape, this.shader, dataOnGpu);
}

public FloatTensor Abs(bool inline = false)
// Returns a new Tensor with the smallest integer greater than or equal to each element
{
	FloatTensor result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		result.Gpu ();
		if (inline) { AbsGPU_ (); return this; }
		else { return AbsGPU (result); }
	}
	else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        result.Data [i] = (float)(Math.Abs (Data [i]));
					}
				});
	}
	return result;
}

public FloatTensor Add(FloatTensor x, bool inline = false)
{
	// Check if both tensors are compatible for sum
	SameSizeDimensionsShapeAndLocation(ref x);

	FloatTensor result = inline ? this : this.emptyTensorCopy();
	if (dataOnGpu & x.dataOnGpu) {
		result.Gpu ();
		if (inline) { AddElemGPU_ (x); return this; }
		else { return AddElemGPU (x, result); }
	}
	else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        result.Data [i] = x.Data [i] + Data [i];
					}
				});
	}
	return result;
}

public FloatTensor Acos ()
{
	if (dataOnGpu) {
		return AcosGPU ();
	} else {
		var result = new FloatTensor (shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        result.Data [i] = (float)System.Math.Acos (d);
					}
				});

		return result;
	}
}

public void Acos_ ()
{
	if (dataOnGpu) {
		AcosGPU_ ();
	} else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        Data [i] = (float)System.Math.Acos (d);
					}
				});
	}
}

public FloatTensor Asin ()
{
	if (dataOnGpu) {
		return AsinGPU ();
	} else {
		var result = new FloatTensor (shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        result.Data [i] = (float)System.Math.Asin (d);
					}
				});

		return result;
	}
}

public void Asin_ ()
{
	if (dataOnGpu) {
		AsinGPU_ ();
	} else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        Data [i] = (float)System.Math.Asin (d);
					}
				});
	}
}

public FloatTensor Atan ()
{
	if (dataOnGpu) {
		return AtanGPU ();
	} else {
		var result = new FloatTensor (shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        result.Data [i] = (float)System.Math.Atan (d);
					}
				});

		return result;
	}
}

public void Atan_ ()
{
	if (dataOnGpu) {
		AtanGPU_ ();
	} else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        Data [i] = (float)System.Math.Atan (d);
					}
				});
	}
}


public FloatTensor Add(float value, bool inline = false)
{
	FloatTensor result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		result.Gpu ();
		if (inline) { AddScalarGPU_ (value); return this; }
		else { return AddScalarGPU (value, result); }
	}
	else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        result.Data [i] = value + Data [i];
					}
				});
	}
	return result;
}

public FloatTensor AddMatrixMultiply(FloatTensor tensor1, FloatTensor tensor2)
{
	bool gpu = dataOnGpu & tensor1.DataOnGpu & tensor2.DataOnGpu;
	bool cpu = !(dataOnGpu | tensor1.DataOnGpu | tensor2.DataOnGpu);

	int[] res_shape = this.Shape;
	int[] shape1 = tensor1.Shape;
	int[] shape2 = tensor2.Shape;

	if (shape1[1] != shape2[0])
		throw new InvalidOperationException(String.Format("Matrix multiply not possible: {0} & {1}.", shape1[1], shape2[0]));
	if (res_shape[0] != shape1[0])
		throw new InvalidOperationException(String.Format("First dimension doesn't match: {0} vs {1}.", res_shape[0], shape1[0]));
	if (res_shape[1] != shape2[1])
		throw new InvalidOperationException(String.Format("Last dimension doesn't match: {0} vs {1}.", res_shape[res_shape.Length - 1],shape2[shape2.Length - 1]));

	if (gpu)
	{
		AddMatrixMultiplyGPU(tensor1, tensor2);
	}
	else if (cpu)
	{
		var nCpu = SystemInfo.processorCount;
		Parallel.For(0, nCpu, workerId =>
				{
					var max = size * (workerId + 1) / nCpu;
					for (var idx = size * workerId / nCpu; idx < max; idx++)
					{
					        int col = idx % res_shape[1];
					        int row = (idx - col) / res_shape[1];
					        int row_offset = row * shape1[1];
					        for (var j = 0; j < shape1[1]; j++)
					        {
					                Data[idx] += tensor1.Data[j + row_offset] * tensor2.Data[j * shape2[1] + col];
						}
					}
				});
		return this;
	}
	else
	{
		Debug.Log("Data for all Tensors needs to be colocated on the same device. - CPU != GPU");
	}
	return this;
}

public FloatTensor Ceil(bool inline = false)
// Returns a new Tensor with the smallest integer greater than or equal to each element
{
	FloatTensor result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		//TODO: Fix GPU operations. https://github.com/OpenMined/OpenMined/issues/126
		result.Gpu ();
		if (inline) { CeilGPU_ (); return this; }
		else { return CeilGPU (result); }
	}

	var nCpu = SystemInfo.processorCount;
	Parallel.For(0, nCpu, workerId =>
			{
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++)
				{
				        result.Data[i] = (float) (Math.Ceiling(Data[i]));
				}
			});
	return result;
}

public FloatTensor Cos()
{
	if (dataOnGpu)
	{
		return CosGPU();
	}
	else
	{
		var result = new FloatTensor(shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For(0, nCpu, workerId =>
				{
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++)
					{
					        var d = (double) Data[i];
					        result.Data[i] = (float) System.Math.Cos(d);
					}
				});

		return result;
	}
}

public void Cos_()
{
	if (dataOnGpu)
	{
		CosGPU_();
	}
	else
	{
		var nCpu = SystemInfo.processorCount;
		Parallel.For(0, nCpu, workerId =>
				{
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++)
					{
					        var d = (double) Data[i];
					        Data[i] = (float) System.Math.Cos(d);
					}
				});
	}
}

public FloatTensor      Cosh()
{
	if (dataOnGpu)
	{
		return CoshGPU();
	}
	else
	{
		var result = new FloatTensor(shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For(0, nCpu, workerId =>
				{
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++)
					{
					        var d = (double) Data[i];
					        result.Data[i] = (float) System.Math.Cosh(d);
					}
				});

		return result;
	}
}

public void Cosh_()
{
	if (dataOnGpu)
	{
		CoshGPU_();
	}
	else
	{
		var nCpu = SystemInfo.processorCount;
		Parallel.For(0, nCpu, workerId =>
				{
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++)
					{
					        var d = (double) Data[i];
					        Data[i] = (float) System.Math.Cosh(d);
					}
				});
	}
}

public FloatTensor Div(FloatTensor x, bool inline = false)
{
	// Check if both tensors are compatible for sum
	SameSizeDimensionsShapeAndLocation (ref x);

	FloatTensor result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu & x.dataOnGpu) {
		result.Gpu ();
		if (inline) { DivElemGPU_ (x); return this; }
		else { return DivElemGPU (x, result); }
	}
	else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++)
					{
					        result.Data [i] = Data [i] / x.Data [i];
					}
				});
	}
	return result;
}

public FloatTensor AddMatrixVectorProduct (FloatTensor matrix, FloatTensor vector)
{
	bool gpu = dataOnGpu & matrix.DataOnGpu & vector.DataOnGpu;
	bool cpu = !(dataOnGpu | matrix.DataOnGpu | vector.DataOnGpu);

	int[] ref_shape = this.Shape;
	int[] matrix_shape = matrix.Shape;
	int[] vector_shape = vector.Shape;

	if (ref_shape.Length != 1)
		throw new InvalidOperationException ("Cannot perform this operation on a tensor with more than one dimension");
	if (ref_shape [0] != vector_shape [0])
		throw new InvalidOperationException (String.Format ("Cannot add matrix-vector product to tensor: {0} & {1}.", ref_shape [0], vector_shape [0]));
	if (matrix_shape [1] != vector_shape [0])
		throw new InvalidOperationException (String.Format ("Last dimension of matrix doesn't match: {0} vs {1}.", matrix_shape [1], vector_shape [0]));

	if (gpu) {
		AddMatrixVectorProductGPU (matrix, vector);
	} else if (cpu) {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var idx = size * workerId / nCpu; idx < max; idx++) {
					        for (var j = 0; j < ref_shape [0]; j++) {
					                Data [idx] += vector.Data [j] * matrix.Data [j + (idx * ref_shape [0])];
						}
					}
				});
	} else {
		Debug.Log ("Data for all Tensors needs to be colocated on the same device. - CPU != GPU");
	}

	return this;
}


public FloatTensor Div(float value, bool inline = false)
{
	FloatTensor result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		result.Gpu ();
		if (inline) { DivScalarGPU_ (value); return this; }
		else { return DivScalarGPU (value, result); }
	}
	else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++)
						result.Data [i] = Data [i] / value;
				});
	}
	return result;
}

public FloatTensor Floor(bool inline = false)
{
	FloatTensor result = inline ? this : this.emptyTensorCopy();
	if (dataOnGpu)
	{
		result.Gpu();
		if (inline) { FloorGPU_ (); return this; }
		else { return FloorGPU (result); }
	}
	var nCpu = SystemInfo.processorCount;
	Parallel.For(0, nCpu, workerId =>
			{
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++)
				{
				        result.Data[i] = (float)(Math.Floor(this.Data[i]));
				}
			});
	return result;
}

public bool IsContiguous()
{
	if (strides [strides.Length - 1] == 1L) {
		return true;
	}
	return false;
}

public FloatTensor Mul(FloatTensor x, bool inline = false)
{
	// Check if both tensors are compatible for sum
	SameSizeDimensionsShapeAndLocation (ref x);

	var result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		result.Gpu ();
		if (inline) { MulElemGPU_(x); return this;}
		else { return MulElemGPU(x, result);}
	}

	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++)
					result.Data [i] = x.Data [i] * this.Data [i];
			});

	return result;
}

public FloatTensor Mul(float value, bool inline = false)
{
	var result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		if (inline) { MulScalarGPU_(value); return this; }
		else { return MulScalarGPU(value, result); }
	}

	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++)
					result.Data [i] = value * Data [i];
			});
	return result;
}


public FloatTensor Sub(FloatTensor x, bool inline = false)
{
	// Check if both tensors are compatible for sum
	SameSizeDimensionsShapeAndLocation (ref x);

	FloatTensor result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu & x.dataOnGpu) {
		result.Gpu ();
		if (inline) { SubElemGPU_(x); return this;}
		else { return SubElemGPU (x, result); }
	}
	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++)
					result.Data [i] = Data [i] - x.Data [i];
			});
	return result;
}

public FloatTensor Pow (FloatTensor x, bool inline = true)
{
	// Check if both tensors are compatible for sum
	SameSizeDimensionsShapeAndLocation (ref x);

	FloatTensor result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		result.Gpu ();
		if (inline) { result.PowElemGPU_(x); return this;}
		else { return PowElemGPU (x, result); }

	} else {

		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++)
						result.Data [i] = (float)Math.Pow ((double)Data [i], x.Data [i]);
				});
	}
	return result;
}

public FloatTensor Pow (float value, bool inline = true)
{
	var result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		result.Gpu ();
		if (inline) { PowScalarGPU_(value); return this;}
		else { return PowScalarGPU (value, result); }
	} else {

		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++)
						result.Data [i] = (float)Math.Pow ((double)Data [i], value);
				});
	}
	return result;
}


public FloatTensor Neg ()
{
	if (dataOnGpu) {
		return NegateGPU ();
	}

	var result = new FloatTensor (shape, this.shader, dataOnGpu);
	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = data.Length * (workerId + 1) / nCpu;
				for (var i = data.Length * workerId / nCpu; i < max; i++)
					result.data [i] = -data [i];
			});
	return result;
}

public FloatTensor Sign ()
{
	var result = new FloatTensor (shape, this.shader, dataOnGpu);

	if (dataOnGpu) {
		result.Gpu ();
		return SignGPU (result);
	}

	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = data.Length * (workerId + 1) / nCpu;
				for (var i = data.Length * workerId / nCpu; i < max; i++)
					result.data [i] = (float)Math.Sign (data [i]);
			});
	return result;
}

public FloatTensor  Sin ()
{
	if (dataOnGpu) {
		return SinGPU ();
	} else {
		var result = new FloatTensor (shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        result.Data [i] = (float)System.Math.Sin (d);
					}
				});

		return result;
	}
}

public void Sin_ ()
{
	if (dataOnGpu) {
		SinGPU_ ();
	} else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        Data [i] = (float)System.Math.Sin (d);
					}
				});
	}
}

public FloatTensor SizeTensor ()
{
	float[] data = new float[shape.Length];
	int[] ndims = { shape.Length };
	for (int dim = 0; dim < shape.Length; dim++) {
		data [dim] = shape [dim];
	}

	FloatTensor result = new FloatTensor (data, ndims);
	return result;
}


public FloatTensor Sqrt ()
{
	if (dataOnGpu) {
		return SqrtGPU ();
	}

	var result = new FloatTensor (shape, shader, dataOnGpu);
	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = data.Length * (workerId + 1) / nCpu;
				for (var i = data.Length * workerId / nCpu; i < max; i++) {
				        var d = (double)data [i];
				        result.data [i] = (float)Math.Sqrt (d);
				}
			});

	return result;
}

public FloatTensor Sub(float value, bool inline = false)
{
	FloatTensor result = inline ? this : this.emptyTensorCopy();

	if (dataOnGpu) {
		result.Gpu ();
		if (inline) { SubScalarGPU_(value); return this; }
		else { return SubScalarGPU(value, result); }
	}
	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++)
					result.Data [i] = Data [i] - value;
			});
	return result;
}

public FloatTensor Sum (int dim)
{
	int[] result_shape = new int[shape.Length - 1];
	int j = 0;
	for (var i = 0; i < shape.Length; i++) {
		if (i != dim) {
			result_shape [j] = shape [i];
			j += 1;
		}
	}

	var result = new FloatTensor (result_shape, this.shader, false);


	if (dataOnGpu) {
		// TODO: write GPU kernel for summing over a dimension
//				result.Gpu ();

	} else {



		// TODO: write parallel.for for summing over a dimension
//				var nCpu = SystemInfo.processorCount;
//				Parallel.For (0, nCpu, workerId => {
//					var max = size * (workerId + 1) / nCpu;
//					for (var i = size * workerId / nCpu; i < max; i++)
//						result.Data [i] = value - Data [i];
//				});
	}
	return result;
}

public FloatTensor Tan ()
{
	if (dataOnGpu) {
		return TanGPU ();
	} else {
		var result = new FloatTensor (shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        result.Data [i] = (float)System.Math.Tan (d);
					}
				});

		return result;
	}
}

public void Tan_ ()
{
	if (dataOnGpu) {
		TanGPU_ ();
	} else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        Data [i] = (float)System.Math.Tan (d);
					}
				});
	}
}

public FloatTensor Tanh ()
{
	if (dataOnGpu) {
		return TanhGPU ();
	} else {
		var result = new FloatTensor (shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        result.Data [i] = (float)System.Math.Tanh (d);
					}
				});

		return result;
	}
}

public FloatTensor Transpose ()
{
	if (shape.Length != 2)
		throw new InvalidOperationException ("Need to specify parameters for tensors with more than 2 dims.");

	return Transpose (0, 1);
}

public FloatTensor Trunc ()
{
	if (dataOnGpu) {
		return TruncGPU ();
	} else {
		var result = new FloatTensor (shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        result.Data [i] = (float)System.Math.Truncate (d);
					}
				});

		return result;
	}
}

public FloatTensor Transpose (int dimension1, int dimension2)
{
	//TODO: Should we create a new Tensor object here?
	if (dimension1 < 0 || dimension1 >= shape.Length)
		throw new ArgumentOutOfRangeException ("dimension1");
	if (dimension2 < 0 || dimension2 >= shape.Length)
		throw new ArgumentOutOfRangeException ("dimension2");

	if (dimension1 == dimension2) {
		return this;
	}

	int[] new_shape = (int[])Shape.Clone ();
	int tmp_dim = new_shape [dimension1];
	new_shape [dimension1] = new_shape [dimension2];
	new_shape [dimension2] = tmp_dim;

	var result = new FloatTensor (new_shape, this.shader, dataOnGpu);
	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++) {
				        var idxs = GetIndices (i);
				        long tmp = idxs [dimension1];
				        idxs [dimension1] = idxs [dimension2];
				        idxs [dimension2] = tmp;
				        result [idxs] = this [i];
				}
			});

	return result;
}

public void Triu_ (int k)
{
	if (shape.Length != 2) {
		throw new InvalidOperationException (String.Format ("Matrix multiply not possible: Num. Dimensions {0} != 2.", shape.Length));
	}
	if (dataOnGpu) {
		UnityEngine.Debug.Log ("Entra");
		TriuGPU_ (k);
		return;
	}
	var nCpu = SystemInfo.processorCount;
	Parallel.For (0, nCpu, workerId => {
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++) {
				        int col = i % this.shape [1];
				        int row = (i - col) / this.shape [1];
				        if (col < row + k) {
				                Data [i] = 0.0f;
					}
				}
			});

}

public FloatTensor Sinh ()
{
	if (dataOnGpu) {
		return SinhGPU ();
	} else {
		var result = new FloatTensor (shape, this.shader, dataOnGpu);
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        result.Data [i] = (float)System.Math.Sinh (d);
					}
				});

		return result;
	}
}

public void Sinh_ ()
{
	if (dataOnGpu) {
		SinhGPU_ ();
	} else {
		var nCpu = SystemInfo.processorCount;
		Parallel.For (0, nCpu, workerId => {
					var max = size * (workerId + 1) / nCpu;
					for (var i = size * workerId / nCpu; i < max; i++) {
					        var d = (double)Data [i];
					        Data [i] = (float)System.Math.Sinh (d);
					}
				});
	}
}

public FloatTensor Sigmoid(bool inline = false)
{
	if (dataOnGpu)
	{
		if (inline) {this.SigmoidGPU_(); return this;}
		else { return SigmoidGPU(this.emptyTensorCopy()); }
	}

	FloatTensor result = inline ? this : this.emptyTensorCopy();
	var nCpu = SystemInfo.processorCount;
	Parallel.For(0, nCpu, workerId =>
			{
				var max = size * (workerId + 1) / nCpu;
				for (var i = size * workerId / nCpu; i < max; i++)
				{
				        double s = Math.Exp((double)this.Data[i]);
				        result.Data[i] = (float)(s / (1.0f + s));
				}
			});
	return result;
}

public FloatTensor View (int[] new_shape)
{
	int new_size = 1;
	for (int i = 0; i < new_shape.Length; i++) {
		new_size *= new_shape [i];
	}

	if (new_size == size) {
		shape = new_shape;


		if (dataOnGpu) {
			return new FloatTensor (dataBuffer, new_shape, size, this.shader);
		} else {
			// public FloatTensor(float[] _data, int[] _shape, ComputeShader _shader, bool _initOnGpu = false)
			var result = new FloatTensor (data, new_shape, shader);
			return result;
		}
	}
	return this;

}

public void View_ (int[] new_shape)
{

	int new_size = 1;
	for (int i = 0; i < new_shape.Length; i++) {
		new_size *= new_shape [i];
	}

	if (new_size == size) {

		shape = new_shape;

		if (dataOnGpu) {
			shapeBuffer.Release ();
			shapeBuffer.SetData (shape);
		}
	}
}

public void Zero_()
{
	if (dataOnGpu)
	{
		ZeroGPU_(); return;
	}

	var nCpu = SystemInfo.processorCount;
	Parallel.For(0, nCpu, workerId =>
			{
				var max = data.Length * (workerId + 1) / nCpu;
				for (int i = data.Length * workerId / nCpu; i < max; i++)
				{ this.Data[i] = 0; }
			});
}

}
}
