using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NoteIt.Core.Entities
{
    public class NoteContent : BaseEntity
    {
        public string Text { get; set; }

        public long NoteId { get; set; }
        public Note Note { get; set; }
    }
}
