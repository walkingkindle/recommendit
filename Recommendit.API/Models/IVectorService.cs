
using Recommendit.Infrastructure;

namespace Recommendit.Models;

public interface IVectorService
{
    public double[] CalculateAverageVector(List<double[]> vectors);
    

    public Task<List<int>> GetSimilarities(List<ShowInfo> allShows, double[] userAverageVector, int topN);
}