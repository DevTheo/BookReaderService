CREATE TABLE "BookInfo" (
    "Id"	TEXT NOT NULL,
    "Name"	TEXT NOT NULL,
    "BookFilePath"	TEXT NOT NULL,
    "ThumbnailFileName"	TEXT NOT NULL,
    "Author"	TEXT,
    "SeriesIdentifier"	TEXT,
    "Info"	TEXT,
    PRIMARY KEY("Id")
);