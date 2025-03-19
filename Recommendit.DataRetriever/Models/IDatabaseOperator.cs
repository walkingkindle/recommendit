using Recommendit.Infrastructure;
using Recommendit.Result;

namespace DataRetriever.Models;

public interface IDatabaseOperator{

    Task<Result> BulkAddShowsToDatabase(List<Show> shows);

    Task<List<ShowVectorRetrievalDto>> RetrieveVectorsFromAllShows();

    Task<Result> SaveVectorsToShowInfo(List<ShowInfo> showInfos);


}