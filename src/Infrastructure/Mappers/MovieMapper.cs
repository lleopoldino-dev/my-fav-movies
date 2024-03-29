﻿using Business.Models;

namespace Infrastructure.Mappers;

public static class MovieMapper
{
    public static Movie MapFromDbResult(Dictionary<string, object> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new Movie
        {
            Id = (Guid)data["id"],
            Title = (string)data["title"],
            Category = (string)data["category"],
            ReleaseDate = (DateTime)data["releasedate"]
        };
    }
}
