﻿namespace VirtualLibrary.Books.Dtos
{
    public class BookInformationDataTransferObject
    {
        public BooksDataTransferObject book_information { get; set; }
        public AuthorsDataTransferObject author_information { get; set; }
    }
}
