using System;

namespace DtReader.Core.Models;

public record BookInfo(
    Guid Id, // Maybe we will switch to something strongly typed 
    string Name, 
    string BookFilePath, 
    string ThumbnailFileName, 
    string? Author, 
    string? SeriesIdentifier, 
    string? Info);