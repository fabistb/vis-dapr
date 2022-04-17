namespace Main.Models;

public record StateStoreEtagDto(string ETag, StateStoreDto StateStore);

public record StateStoreDto(string FirstName, string LastName, int Age);