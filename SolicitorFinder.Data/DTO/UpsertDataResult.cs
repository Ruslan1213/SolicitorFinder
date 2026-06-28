namespace SolicitorFinder.Data.DTO;

public record UpsertDataResult(List<int> Ids, int Added, int Updated);