namespace Main.Models;

public record QueryStoreDto(string FirstName, string LastName, double Age);

public record QueryStoreTokenDto(string Token, List<QueryStoreDto> QueryStoreDto);