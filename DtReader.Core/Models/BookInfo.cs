using System;

namespace DtReader.Core.Models;

public record BookInfo(
    Guid Id, // Maybe we will switch to something strongly typed
    string Name,
    string BookFilePath,
    string ThumbnailFileName,
    string? Author,
    string? SeriesIdentifier,
    string? Info)
{
    // Dapper materializes from SQLite TEXT columns, which arrive as strings.
    public BookInfo(string Id, string Name, string BookFilePath, string ThumbnailFileName, string? Author, string? SeriesIdentifier, string? Info)
        : this(Guid.Parse(Id), Name, BookFilePath, ThumbnailFileName, Author, SeriesIdentifier, Info)
    {
    }
}