namespace DelaunatorSharp
{

	public interface IVoronoiCell
	{
		IPoint[] Points { get; }

		int Index { get; }
	}
}