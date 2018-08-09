using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NoteIt.Core.Entities
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }
    }
}
