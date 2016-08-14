namespace Assets.Code.Bon.Interface
{
	public interface ISampler3D
	{
		float GetSampleAt(float x, float y, float seed);
		int GetId();
	}
}
