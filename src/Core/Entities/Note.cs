using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NoteIt.Core.Entities
{
    public class Note : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public NoteContent Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool IsPinned { get; set; }
    }
}
