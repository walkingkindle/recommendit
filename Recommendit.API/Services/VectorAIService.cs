using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using Recommendit.Models;
using Recommendit.Infrastructure;

namespace ShowPulse.Engine
{
    public class VectorAIService:IVectorService
    {

        public class VectorObject
        {
            [JsonProperty("vector")]
            public List<double?>? Vector { get; set; }
        }
        public  double[] CalculateAverageVector(List<double[]?> vectors)
        {
            int vectorLength = vectors[0].Length;
            double[] averageVector = new double[vectorLength];

            foreach (var vector in vectors)
            {
                for (int i = 0; i < vectorLength; i++)
                {
                    averageVector[i] += vector[i];
                }
            }

            for (int i = 0; i < vectorLength; i++)
            {
                averageVector[i] /= vectors.Count;
            }

            return averageVector;
        }

        private double CalculateCosineSimilarity(double[] vectorA, double[] vectorB)
        {
            // Calculate dot product
            double dotProduct = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
            }

            // Calculate magnitudes
            double magnitudeA = Math.Sqrt(vectorA.Sum(x => x * x));
            double magnitudeB = Math.Sqrt(vectorB.Sum(x => x * x));

            // Calculate cosine similarity
            if (magnitudeA > 0 && magnitudeB > 0)
            {
                return dotProduct / (magnitudeA * magnitudeB);
            }
            else
            {
                return 0; // Default similarity when one of the vectors is a zero vector
            }
        }

        //public async Task<List<int>> GetSimilarities(IEnumerable<ShowInfo> allShows, double[] userAverageVector, int topN)
        //{
        //    var similarities = allShows
        //        .Where(s => s.VectorDouble != null)
        //        .ToDictionary(s => s.Id, s => CalculateCosineSimilarity(userAverageVector, s.VectorDouble));

        //    var sortedSimilarities = similarities.OrderByDescending(kv => kv.Value);
        //    var topShows = sortedSimilarities.Take(topN);
        //    return topShows.Select(kv => kv.Key).ToList();
        //}
    }

}

