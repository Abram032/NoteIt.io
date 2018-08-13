using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NoteIt.Core.Entities
{
    public class Note : TableEntity
    {
        public Note()
        {
        }

        public Note(string note, Guid guid)
        {
            this.PartitionKey = note;
            this.RowKey = guid.ToString();
        }

        public Guid UserId { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ModifiedAt { get; set; } = null;
        public bool IsPinned { get; set; } = false;
        public bool IsShared { get; set; } = false;
        public string Url { get; set; } = String.Empty;
    }
}
